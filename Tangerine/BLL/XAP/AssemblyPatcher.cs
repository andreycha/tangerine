using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Tangerine.BLL.CustomCode;
using Tangerine.BLL.Hooks;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace Tangerine.BLL
{
    /// <summary>
    /// Patches given XAP assembly that is used to instrumentate application:
    ///  - logging method calls (names)
    ///  - logging method parameters (names and values)
    ///  - logging return values
    ///  - run custom code instead of method
    ///  - run custom code on method enter
    ///  - run custom code on method exit
    /// </summary>
    internal sealed class AssemblyPatcher
    {
        private readonly AssemblyDefinition m_assemblyDefinition;
        private readonly string m_assemblyPath;
        private readonly IHookProvider m_hookProvider;
        private readonly ICustomCodeGenerator m_codeGenerator;

        private MethodReference m_refWritelnStr;
        private MethodReference m_refWritelnInt;
        private MethodReference m_refWritelnObj;
        private MethodReference m_refByteToString;
        private MethodReference m_refWritelnChar;

        public AssemblyPatcher(string assemblyPath, IHookProvider hookProvider)
        {
            AssemblyDefinition def = LoadAssembly(assemblyPath);

            m_assemblyDefinition = def;
            m_assemblyPath = assemblyPath;
            m_hookProvider = hookProvider;
            // TODO: dependency
            m_codeGenerator = new CustomCodeGenerator();

            InitializeConsoleMethods();
        }

        private AssemblyDefinition LoadAssembly(string assemblyPath)
        {
            AssemblyDefinition def;
            try
            {
                def = AssemblyFactory.GetAssembly(assemblyPath);
            }
            catch (Exception e)
            {
                throw new Exception("Error loading assembly: " + assemblyPath, e);
            }
            return def;
        }

        private void InitializeConsoleMethods()
        {
            m_refWritelnStr = m_assemblyDefinition.MainModule.Import(
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) })
                );
            m_refWritelnInt = m_assemblyDefinition.MainModule.Import(
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) })
                );
            m_refWritelnChar = m_assemblyDefinition.MainModule.Import(
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(char) })
                );
            m_refWritelnObj = m_assemblyDefinition.MainModule.Import(
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(object) })
                );
            m_refByteToString = m_assemblyDefinition.MainModule.Import(
                typeof(BitConverter).GetMethod("ToString", new Type[] { typeof(byte[]) })
                );
        }

        public void PatchAssembly()
        {
#if DEBUG
            File.Delete("dump.txt");
            File.Delete("dump_changed.txt");
#endif

            foreach (TypeDefinition typeDefinition in m_assemblyDefinition.MainModule.Types)
            {
                if (typeDefinition.Name.StartsWith("<Module>")
                    || typeDefinition.Name.StartsWith("<PrivateImplementationDetails>")
                    || typeDefinition.Name.StartsWith("__StaticArrayInitTypeSize"))
                {
                    continue;
                }

                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                {
                    MethodHook hook = m_hookProvider.GetMethodHook(methodDefinition);
                    PatchMethod(methodDefinition, typeDefinition, hook);
                }
            }

            AssemblyFactory.SaveAssembly(m_assemblyDefinition, m_assemblyPath);
        }

        private void PatchMethod(MethodDefinition methodDefinition, TypeDefinition typeDefinition, MethodHook methodHook)
        {
#if DEBUG
            DumpMethod("dump.txt", methodDefinition);
#endif

            if (!methodDefinition.HasBody || methodDefinition.IsGetter || methodDefinition.IsSetter)
            {
                return;
            }

            if (methodHook != null && methodHook.RunCustomCode)
            {
                InjectCustomCode(methodDefinition, methodHook);
            }

            if (m_hookProvider.LogMethodNames || (methodHook != null && methodHook.LogMethodName))
            {
                Instruction logMethodName = LogMethodName(methodDefinition, typeDefinition);
                if (m_hookProvider.LogParameterValues || (methodHook != null && methodHook.LogParameterValues))
                {
                    LogMethodParameters(methodDefinition, logMethodName);
                }
                if (m_hookProvider.LogReturnValues || (methodHook != null && methodHook.LogReturnValues))
                {
                    LogReturnValue(methodDefinition);
                }
            }

#if DEBUG
            DumpMethod("dump_changed.txt", methodDefinition);
#endif
        }

        private void InjectCustomCode(MethodDefinition method, MethodHook methodHook)
        {
            // generate assembly with custom code
            m_codeGenerator.GenerateAssembly(methodHook, Path.GetDirectoryName(m_assemblyPath));

            // load generated assembly
            string name = methodHook.GetSafeName();
            string generatedAssemblyName = String.Format("{0}.dll", name);
            string generatedAssemblyPath = Path.Combine(Path.GetDirectoryName(m_assemblyPath), generatedAssemblyName);
            var genAsm = AssemblyFactory.GetAssembly(generatedAssemblyPath);

            // get method reference with custom code
            string generatedClassName = String.Format("{0}_{1}", name, "Class");
            var genType = genAsm.MainModule.Types.Cast<TypeDefinition>().First(t => t.Name == generatedClassName);
            string generatedMethodName = String.Format("{0}_{1}", name, "Hook");
            // TODO: take parameter types into account, to resolve methods with the same names
            var genMet = genType.Methods.Cast<MethodDefinition>().First(m => m.Name == generatedMethodName);
            MethodReference genTypeCtor = null;
            for (int i = 0; i <= genType.Constructors.Count; i++)
            {
                if (!genType.Constructors[i].HasParameters)
                {
                    genTypeCtor = genType.Constructors[i];
                    break;
                }
            }
            if (genTypeCtor == null)
            {
                throw new InvalidOperationException("Default constructor was on found on generated assembly");
            }

            // inject call
            MethodReference generatedTypeCtorRef = m_assemblyDefinition.MainModule.Import(genTypeCtor);
            MethodReference generatedMethodRef = m_assemblyDefinition.MainModule.Import(genMet);

            method.Body.InitLocals = true;
            var generatedValueVar = new VariableDefinition(method.ReturnType.ReturnType);

            CilWorker cilWorker = method.Body.CilWorker;

            bool hasReturnValue = (method.ReturnType.ReturnType.FullName != typeof(void).FullName);

            if (methodHook.HookType == HookType.ReplaceMethod)
            {
                method.Body.Instructions.Clear();
                method.Body.Variables.Clear();

                if (hasReturnValue)
                {
                    method.Body.Variables.Add(generatedValueVar);
                }

                Instruction nop = cilWorker.Create(OpCodes.Nop);
                cilWorker.Append(nop);
                // call ctor
                Instruction newGeneratedType = cilWorker.Create(OpCodes.Newobj, generatedTypeCtorRef);
                cilWorker.Append(newGeneratedType);
                // load arguments on stack if any
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    Instruction loadArg = cilWorker.Create(OpCodes.Ldarga_S, method.Parameters[i]);
                    cilWorker.Append(loadArg);
                }
                // call replacing method
                Instruction callGeneratedMethod = cilWorker.Create(OpCodes.Call, generatedMethodRef);
                cilWorker.Append(callGeneratedMethod);
                if (hasReturnValue)
                {
                    // assign to variable
                    Instruction assignNewGeneratedValue = cilWorker.Create(OpCodes.Stloc, generatedValueVar);
                    cilWorker.Append(assignNewGeneratedValue);
                    Instruction ldLoc = cilWorker.Create(OpCodes.Ldloc_0);
                    Instruction brs = cilWorker.Create(OpCodes.Br_S, ldLoc);
                    cilWorker.Append(brs);
                    cilWorker.Append(ldLoc);
                }
                // return value
                Instruction ret = cilWorker.Create(OpCodes.Ret);
                cilWorker.Append(ret);
            }
            else if ((methodHook.HookType & HookType.OnMethodEnter) == HookType.OnMethodEnter)
            {
                Instruction firstInstruction = method.Body.Instructions[0];

                Instruction nop = cilWorker.Create(OpCodes.Nop);
                cilWorker.InsertBefore(firstInstruction, nop);
                // call ctor
                Instruction newGeneratedType = cilWorker.Create(OpCodes.Newobj, generatedTypeCtorRef);
                cilWorker.InsertBefore(firstInstruction, newGeneratedType);
                // load arguments on stack if any
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    Instruction loadArg = cilWorker.Create(OpCodes.Ldarga_S, method.Parameters[i]);
                    cilWorker.InsertBefore(firstInstruction, loadArg);
                }
                // call replacing method
                Instruction callGeneratedMethod = cilWorker.Create(OpCodes.Call, generatedMethodRef);
                cilWorker.InsertBefore(firstInstruction, callGeneratedMethod);
                if (hasReturnValue)
                {
                    // remove value from stack
                    Instruction pop = cilWorker.Create(OpCodes.Pop);
                    cilWorker.InsertBefore(firstInstruction, pop);
                }
            }
            else if ((methodHook.HookType & HookType.OnMethodExit) == HookType.OnMethodExit)
            {
                var body = method.Body;
                int instrCount = body.Instructions.Count;
                Instruction retInstr = body.Instructions[instrCount - 1];
                if (retInstr.OpCode != OpCodes.Ret)
                {
                    throw new InvalidOperationException(String.Format("Method '{0}' has no valid ret instruction", method.Name));
                }

                Instruction nop = cilWorker.Create(OpCodes.Nop);
                cilWorker.InsertBefore(retInstr, nop);
                // call ctor
                Instruction newGeneratedType = cilWorker.Create(OpCodes.Newobj, generatedTypeCtorRef);
                cilWorker.InsertBefore(retInstr, newGeneratedType);
                // load arguments on stack if any
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    Instruction loadArg = cilWorker.Create(OpCodes.Ldarga_S, method.Parameters[i]);
                    cilWorker.InsertBefore(retInstr, loadArg);
                }
                // call replacing method
                Instruction callGeneratedMethod = cilWorker.Create(OpCodes.Call, generatedMethodRef);
                cilWorker.InsertBefore(retInstr, callGeneratedMethod);
                if (hasReturnValue)
                {
                    // remove value from stack
                    Instruction pop = cilWorker.Create(OpCodes.Pop);
                    cilWorker.InsertBefore(retInstr, pop);
                }
            }
        }

        private void DumpMethod(string file, MethodDefinition methodDefinition)
        {
            File.AppendAllText(file, Environment.NewLine + "Dump started... " + Environment.NewLine);

            File.AppendAllText(file, "Method: " + methodDefinition.ToString());
            File.AppendAllText(file, Environment.NewLine);

            File.AppendAllText(file, "Variables: ");
            for (int i = 0; i < methodDefinition.Body.Variables.Count; i++)
            {
                File.AppendAllText(file, methodDefinition.Body.Variables[i].ToString() + " ");
            }
            File.AppendAllText(file, Environment.NewLine);

            for (int i = 0; i < methodDefinition.Body.Instructions.Count; i++)
            {
                File.AppendAllText(file, methodDefinition.Body.Instructions[i].ToString() + Environment.NewLine);
            }
        }

        private Instruction LogMethodName(MethodDefinition methodDefinition, TypeDefinition typeDefinition)
        {
            MethodBody body = methodDefinition.Body;

            Instruction varMethodName = body.CilWorker.Create(
                OpCodes.Ldstr,
                "*Type: " + typeDefinition.FullName + ", Method name: " + methodDefinition.Name
                );
            body.CilWorker.InsertBefore(methodDefinition.Body.Instructions[0], varMethodName);
            Instruction logMethodName = body.CilWorker.Create(OpCodes.Call, m_refWritelnStr);
            body.CilWorker.InsertAfter(varMethodName, logMethodName);

            return logMethodName;
        }

        private void LogMethodParameters(MethodDefinition methodDefinition, Instruction previousInstruction)
        {
            if (!methodDefinition.HasParameters)
            {
                return;
            }

            ParameterDefinitionCollection methodParameters = methodDefinition.Parameters;
            MethodBody methodBody = methodDefinition.Body;

            // NOTE: parameters are loaded in reverse order
            int parameterPosition = methodDefinition.IsStatic ? methodParameters.Count - 1 : methodParameters.Count;
            for (int i = methodParameters.Count - 1; i >= 0; i--)
            {
                ParameterDefinition parameter = methodParameters[i];
                Instruction logParamName = LogParameterName(parameter.Name, methodBody, previousInstruction);
                LogParameterValue(parameter, parameterPosition, methodBody, logParamName);
                parameterPosition--;
            }
        }

        private Instruction LogParameterName(string parameterName, MethodBody body, Instruction previousInstruction)
        {
            Instruction varParamName = body.CilWorker.Create(OpCodes.Ldstr, "*Parameter name: " + parameterName);
            body.CilWorker.InsertAfter(previousInstruction, varParamName);
            Instruction logParamName = body.CilWorker.Create(OpCodes.Call, m_refWritelnStr);
            body.CilWorker.InsertAfter(varParamName, logParamName);
            return logParamName;
        }

        private void LogParameterValue(
            ParameterDefinition parameter,
            int parameterPosition,
            MethodBody body,
            Instruction previousInstruction
            )
        {
            ReferenceType refType = parameter.ParameterType as ReferenceType;
            bool isRefParameter = (refType != null);

            Instruction varValue = body.CilWorker.Create(OpCodes.Ldarg, parameter);

            switch (parameter.ParameterType.Name)
            {
                case "String":
                    {
                        body.CilWorker.InsertAfter(previousInstruction, varValue);
                        Instruction logStr = body.CilWorker.Create(OpCodes.Call, m_refWritelnStr);
                        body.CilWorker.InsertAfter(varValue, logStr);
                    }
                    break;

                case "Int32":
                    {
                        body.CilWorker.InsertAfter(previousInstruction, varValue);
                        Instruction logInt = body.CilWorker.Create(OpCodes.Call, m_refWritelnInt);
                        body.CilWorker.InsertAfter(varValue, logInt);
                    }
                    break;

                case "Byte[]":
                    {
                        // convert byte array to string
                        body.CilWorker.InsertAfter(previousInstruction, varValue);
                        Instruction toString = body.CilWorker.Create(OpCodes.Call, m_refByteToString);
                        body.CilWorker.InsertAfter(varValue, toString);
                        // log string representation
                        Instruction logByte = body.CilWorker.Create(OpCodes.Call, m_refWritelnObj);
                        body.CilWorker.InsertAfter(toString, logByte);
                    }
                    break;

                default:
                    {
                        body.CilWorker.InsertAfter(previousInstruction, varValue);
                        Instruction lastInstr = varValue;
                        // box value types
                        if (parameter.ParameterType.IsValueType)
                        {
                            var paramTypeRef = isRefParameter ? refType.ElementType : parameter.ParameterType;
                            if (isRefParameter)
                            {
                                // load value type object by address on stack
                                Instruction objInstr = body.CilWorker.Create(OpCodes.Ldobj, paramTypeRef);
                                body.CilWorker.InsertAfter(lastInstr, objInstr);
                                lastInstr = objInstr;
                            }
                            // box it
                            Instruction boxInstr = body.CilWorker.Create(OpCodes.Box, paramTypeRef);
                            body.CilWorker.InsertAfter(lastInstr, boxInstr);
                            lastInstr = boxInstr;
                            if (isRefParameter && paramTypeRef.Name == "Byte[]")
                            {
                                // convert byte array to string
                                Instruction toString = body.CilWorker.Create(OpCodes.Call, m_refByteToString);
                                body.CilWorker.InsertAfter(lastInstr, toString);
                                lastInstr = toString;
                            }
                        }
                        else if (isRefParameter)
                        {
                            Instruction refInstr = body.CilWorker.Create(OpCodes.Ldind_Ref);
                            body.CilWorker.InsertAfter(lastInstr, refInstr);
                            lastInstr = refInstr;
                        }
                        // log value
                        Instruction logStr = body.CilWorker.Create(OpCodes.Call, m_refWritelnObj);
                        body.CilWorker.InsertAfter(lastInstr, logStr);
                    }
                    break;
            }
        }

        private void LogReturnValue(MethodDefinition methodDefinition)
        {
            TypeReference returnType = methodDefinition.ReturnType.ReturnType;
            if (returnType.FullName == typeof(void).FullName)
            {
                return;
            }

            var body = methodDefinition.Body;
            int instrCount = body.Instructions.Count;
            Instruction retInstr = body.Instructions[instrCount - 1];
            if (retInstr.OpCode != OpCodes.Ret)
            {
                throw new InvalidOperationException(String.Format("Method '{0}' has no valid ret instruction", methodDefinition.Name));
            }

            // add temp variable
            body.InitLocals = true;
            var returnValueVar = new VariableDefinition("tmp", body.Variables.Count, methodDefinition, returnType);
            body.Variables.Add(returnValueVar);

            // load return value from stack to temp variable
            Instruction saveRetValueInstr = body.CilWorker.Create(OpCodes.Stloc, returnValueVar);
            body.CilWorker.InsertBefore(retInstr, saveRetValueInstr);

            // return value marker
            string methodName = "*Return value: " + methodDefinition.Name;
            Instruction varParamName = body.CilWorker.Create(OpCodes.Ldstr, methodName);
            body.CilWorker.InsertBefore(retInstr, varParamName);
            Instruction logParamName = body.CilWorker.Create(OpCodes.Call, m_refWritelnStr);
            body.CilWorker.InsertBefore(retInstr, logParamName);

            // load return value on stack
            Instruction retValue = body.CilWorker.Create(OpCodes.Ldloc, returnValueVar);
            body.CilWorker.InsertBefore(retInstr, retValue);

            LogValue(body, retInstr, returnType);

            // load return value from temp variable back on stack
            Instruction loadRetValueInstr = body.CilWorker.Create(OpCodes.Ldloc, returnValueVar);
            body.CilWorker.InsertBefore(retInstr, loadRetValueInstr);
        }

        private void LogValue(MethodBody body, Instruction targetInstr, TypeReference valueType)
        {
            MethodReference logMethodRef;

            switch (valueType.Name)
            {
                case "String":
                    logMethodRef = m_refWritelnStr;
                    break;

                case "Int32":
                    logMethodRef = m_refWritelnInt;
                    break;

                case "Char":
                    logMethodRef = m_refWritelnChar;
                    break;

                default:
                    logMethodRef = m_refWritelnObj;
                    break;
            }

            if (valueType.Name == "Byte[]")
            {
                Instruction toStringInstr = body.CilWorker.Create(OpCodes.Call, m_refByteToString);
                body.CilWorker.InsertBefore(targetInstr, toStringInstr);
            }

            // box other value types
            if (valueType.IsValueType
                && valueType.FullName != typeof(int).FullName
                && valueType.FullName != typeof(char).FullName)
            {
                Instruction boxInstr = body.CilWorker.Create(OpCodes.Box, valueType);
                body.CilWorker.InsertBefore(targetInstr, boxInstr);
            }

            Instruction logInstr = body.CilWorker.Create(OpCodes.Call, logMethodRef);
            body.CilWorker.InsertBefore(targetInstr, logInstr);
        }
    }
}
