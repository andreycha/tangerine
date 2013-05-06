using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Tangerine.BLL.CustomCode;
using Tangerine.BLL.Hooks;
using Tangerine.Common;
using Tangerine.Devices;
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
        private readonly DeviceType m_deviceType;
        private readonly PlatformVersion m_version;
        private readonly ICustomCodeGenerator m_codeGenerator;
        private readonly List<string> m_typeDefsToExclude = new List<string>()
        {
            "<Module>", 
            "<PrivateImplementationDetails>", 
            "__StaticArrayInitTypeSize"
        };

        private MethodReference m_refWritelnStr;
        private MethodReference m_refWritelnInt;
        private MethodReference m_refWritelnObj;
        private MethodReference m_refByteToString;
        private MethodReference m_refWritelnChar;

        public AssemblyPatcher(string assemblyPath, IHookProvider hookProvider, DeviceType deviceType, PlatformVersion version)
        {
            AssemblyDefinition def = LoadAssembly(assemblyPath);

            m_assemblyDefinition = def;
            m_assemblyPath = assemblyPath;
            m_hookProvider = hookProvider;
            m_deviceType = deviceType;
            m_version = version;
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
                if (ExcludeType(typeDefinition))
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

        private bool ExcludeType(TypeDefinition typeDefinition)
        {
            foreach (string typeToExclude in m_typeDefsToExclude)
            {
                if (typeDefinition.Name.StartsWith(typeToExclude))
                {
                    return false;
                }
            }
            return true;
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
                Instruction targetInstruction = LogMethodName(methodDefinition, typeDefinition);
                if (m_hookProvider.LogParameterValues || (methodHook != null && methodHook.LogParameterValues))
                {
                    LogMethodParameters(methodDefinition, targetInstruction);
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
            string assemblyName = methodHook.GetSafeName();
            AssemblyDefinition generatedAssembly = LoadGeneratedAssembly(assemblyName);

            // get method reference with custom code
            string generatedClassName = String.Format("{0}_{1}", assemblyName, "Class");
            var generatedType = generatedAssembly.MainModule.Types.Cast<TypeDefinition>().First(t => t.Name == generatedClassName);
            string generatedMethodName = String.Format("{0}_{1}", assemblyName, "Hook");
            // TODO: take parameter types into account, to resolve methods with the same names
            var generatedMethod = generatedType.Methods.Cast<MethodDefinition>().First(m => m.Name == generatedMethodName);
            MethodReference generatedTypeCtor = null;
            for (int i = 0; i <= generatedType.Constructors.Count; i++)
            {
                if (!generatedType.Constructors[i].HasParameters)
                {
                    generatedTypeCtor = generatedType.Constructors[i];
                    break;
                }
            }
            if (generatedTypeCtor == null)
            {
                throw new InvalidOperationException("Default constructor was on found on generated assembly");
            }

            // inject call
            MethodReference generatedTypeCtorRef = m_assemblyDefinition.MainModule.Import(generatedTypeCtor);
            MethodReference generatedMethodRef = m_assemblyDefinition.MainModule.Import(generatedMethod);

            method.Body.InitLocals = true;

            CilWorker cilWorker = method.Body.CilWorker;

            if (methodHook.HookType == HookType.ReplaceMethod)
            {
                method.Body.Instructions.Clear();
                method.Body.Variables.Clear();
                // return value
                Instruction returnInstruction = cilWorker.Create(OpCodes.Ret);
                cilWorker.Append(returnInstruction);
                InsertCustomCodeCall(method, generatedTypeCtorRef, generatedMethodRef, cilWorker, returnInstruction, true);
            }
            else if ((methodHook.HookType & HookType.OnMethodEnter) == HookType.OnMethodEnter)
            {
                Instruction firstInstruction = method.Body.Instructions[0];
                InsertCustomCodeCall(method, generatedTypeCtorRef, generatedMethodRef, cilWorker, firstInstruction, false);
            }
            else if ((methodHook.HookType & HookType.OnMethodExit) == HookType.OnMethodExit)
            {
                Instruction returnInstruction = method.Body.Instructions[method.Body.Instructions.Count - 1];
                if (returnInstruction.OpCode != OpCodes.Ret)
                {
                    throw new InvalidOperationException(String.Format("Method '{0}' has no valid ret instruction", method.Name));
                }
                InsertCustomCodeCall(method, generatedTypeCtorRef, generatedMethodRef, cilWorker, returnInstruction, false);
            }
        }

        private static void InsertCustomCodeCall(
            MethodDefinition method, 
            MethodReference generatedTypeCtorRef, 
            MethodReference generatedMethodRef, 
            CilWorker cilWorker, 
            Instruction instructionToInsertBefore,
            bool replaceMethod
            )
        {
            bool hasReturnValue = (method.ReturnType.ReturnType.FullName != typeof(void).FullName);

            Instruction nop = cilWorker.Create(OpCodes.Nop);
            cilWorker.InsertBefore(instructionToInsertBefore, nop);
            
            // call ctor
            Instruction newGeneratedType = cilWorker.Create(OpCodes.Newobj, generatedTypeCtorRef);
            cilWorker.InsertBefore(instructionToInsertBefore, newGeneratedType);
            
            // load arguments on stack if any
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                Instruction loadArg = cilWorker.Create(OpCodes.Ldarga_S, method.Parameters[i]);
                cilWorker.InsertBefore(instructionToInsertBefore, loadArg);
            }

            // call replacing method
            Instruction callGeneratedMethod = cilWorker.Create(OpCodes.Call, generatedMethodRef);
            cilWorker.InsertBefore(instructionToInsertBefore, callGeneratedMethod);

            if (hasReturnValue)
            {
                if (replaceMethod)
                {
                    // add variable to list
                    var generatedValueVar = new VariableDefinition(method.ReturnType.ReturnType);
                    method.Body.Variables.Add(generatedValueVar);
                    // assign to variable
                    Instruction assignNewGeneratedValue = cilWorker.Create(OpCodes.Stloc, generatedValueVar);
                    cilWorker.InsertBefore(instructionToInsertBefore, assignNewGeneratedValue);
                    Instruction ldLoc = cilWorker.Create(OpCodes.Ldloc_0);
                    Instruction brs = cilWorker.Create(OpCodes.Br_S, ldLoc);
                    cilWorker.InsertBefore(instructionToInsertBefore, brs);
                    cilWorker.InsertBefore(instructionToInsertBefore, ldLoc);
                }
                else
                {
                    // remove value from stack
                    Instruction pop = cilWorker.Create(OpCodes.Pop);
                    cilWorker.InsertBefore(instructionToInsertBefore, pop);
                }
            }
        }

        private AssemblyDefinition LoadGeneratedAssembly(string assemblyName)
        {
            string generatedAssemblyName = String.Format("{0}.dll", assemblyName);
            string generatedAssemblyPath = Path.Combine(Path.GetDirectoryName(m_assemblyPath), generatedAssemblyName);
            return AssemblyFactory.GetAssembly(generatedAssemblyPath);
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
            Instruction firstInstruction = methodDefinition.Body.Instructions[0];

            Instruction varMethodName = body.CilWorker.Create(
                OpCodes.Ldstr,
                "*Type: " + typeDefinition.FullName + ", Method name: " + methodDefinition.Name
                );
            body.CilWorker.InsertBefore(firstInstruction, varMethodName);
            LogText(body, firstInstruction);

            return firstInstruction;
        }

        private void LogText(MethodBody body, Instruction insertBefore)
        {
            switch (m_deviceType)
            {
                case DeviceType.Device:
                    // TODO: log value to file
                    break;

                case DeviceType.Emulator:
                    if (m_version == PlatformVersion.Version71)
                    {
                        Instruction logText = body.CilWorker.Create(OpCodes.Call, m_refWritelnStr);
                        body.CilWorker.InsertBefore(insertBefore, logText);
                    }
                    else
                    {
                        // TODO: log value to file
                    }
                    break;

                default:
                    throw new NotSupportedException(m_deviceType.ToString());
            }
        }

        private void LogMethodParameters(MethodDefinition methodDefinition, Instruction targetInstruction)
        {
            if (!methodDefinition.HasParameters)
            {
                return;
            }

            ParameterDefinitionCollection methodParameters = methodDefinition.Parameters;
            MethodBody methodBody = methodDefinition.Body;

            for (int i = methodParameters.Count - 1; i >= 0; i--)
            {
                ParameterDefinition parameter = methodParameters[i];
                LogParameterName(methodBody, parameter.Name, targetInstruction);
                LogParameterValue(methodBody, parameter, targetInstruction);
            }
        }

        private void LogParameterName(MethodBody body, string parameterName, Instruction targetInstruction)
        {
            Instruction varParamName = body.CilWorker.Create(OpCodes.Ldstr, "*Parameter name: " + parameterName);
            body.CilWorker.InsertBefore(targetInstruction, varParamName);
            LogText(body, targetInstruction);
        }

        private void LogParameterValue(MethodBody body, ParameterDefinition parameter, Instruction targetInstruction)
        {
            Instruction varValue = body.CilWorker.Create(OpCodes.Ldarg, parameter);
            body.CilWorker.InsertBefore(targetInstruction, varValue);
            LogValue(body, parameter.ParameterType, targetInstruction);
        }

        private void LogValue(MethodBody body, TypeReference valueType, Instruction targetInstruction)
        {
            switch (m_deviceType)
            {
                case DeviceType.Device:
                    // TODO: log value to file
                    break;

                case DeviceType.Emulator:
                    if (m_version == PlatformVersion.Version71)
                    {
                        LogValueToConsole(body, valueType, targetInstruction);
                    }
                    else
                    {
                        // TODO: log value to file
                    }
                    break;

                default:
                    throw new NotSupportedException(m_deviceType.ToString());
            }
        }

        private void LogValueToConsole(MethodBody body, TypeReference valueType, Instruction targetInstruction)
        {
            switch (valueType.Name)
            {
                case "String":
                    {
                        Instruction logStr = body.CilWorker.Create(OpCodes.Call, m_refWritelnStr);
                        body.CilWorker.InsertBefore(targetInstruction, logStr);
                    }
                    break;

                case "Int32":
                    {
                        Instruction logInt = body.CilWorker.Create(OpCodes.Call, m_refWritelnInt);
                        body.CilWorker.InsertBefore(targetInstruction, logInt);
                    }
                    break;

                case "Byte[]":
                    {
                        // convert byte array to string
                        Instruction toString = body.CilWorker.Create(OpCodes.Call, m_refByteToString);
                        body.CilWorker.InsertBefore(targetInstruction, toString);
                        // log string representation
                        Instruction logByte = body.CilWorker.Create(OpCodes.Call, m_refWritelnObj);
                        body.CilWorker.InsertBefore(targetInstruction, logByte);
                    }
                    break;

                default:
                    {
                        ReferenceType refType = valueType as ReferenceType;
                        bool isByRef = (refType != null);

                        // box value types
                        if (valueType.IsValueType)
                        {
                            var paramTypeRef = isByRef ? refType.ElementType : valueType;
                            if (isByRef)
                            {
                                // load value type object by address on stack
                                Instruction objInstr = body.CilWorker.Create(OpCodes.Ldobj, paramTypeRef);
                                body.CilWorker.InsertBefore(targetInstruction, objInstr);
                            }
                            // box it
                            Instruction boxInstr = body.CilWorker.Create(OpCodes.Box, paramTypeRef);
                            body.CilWorker.InsertBefore(targetInstruction, boxInstr);
                            if (isByRef && paramTypeRef.Name == "Byte[]")
                            {
                                // convert byte array to string
                                Instruction toString = body.CilWorker.Create(OpCodes.Call, m_refByteToString);
                                body.CilWorker.InsertBefore(targetInstruction, toString);
                            }
                        }
                        else if (isByRef)
                        {
                            Instruction refInstr = body.CilWorker.Create(OpCodes.Ldind_Ref);
                            body.CilWorker.InsertBefore(targetInstruction, refInstr);
                        }
                        // log value
                        Instruction logStr = body.CilWorker.Create(OpCodes.Call, m_refWritelnObj);
                        body.CilWorker.InsertBefore(targetInstruction, logStr);
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
            Instruction retInstruction = body.Instructions[instrCount - 1];
            if (retInstruction.OpCode != OpCodes.Ret)
            {
                throw new InvalidOperationException(String.Format("Method '{0}' has no valid ret instruction", methodDefinition.Name));
            }

            // add temp variable
            body.InitLocals = true;
            var returnValueVar = new VariableDefinition("tmp", body.Variables.Count, methodDefinition, returnType);
            body.Variables.Add(returnValueVar);

            // load return value from stack to temp variable
            Instruction saveRetValueInstr = body.CilWorker.Create(OpCodes.Stloc, returnValueVar);
            body.CilWorker.InsertBefore(retInstruction, saveRetValueInstr);

            // return value marker
            string methodName = "*Return value: " + methodDefinition.Name;
            Instruction varParamName = body.CilWorker.Create(OpCodes.Ldstr, methodName);
            body.CilWorker.InsertBefore(retInstruction, varParamName);
            Instruction logParamName = body.CilWorker.Create(OpCodes.Call, m_refWritelnStr);
            body.CilWorker.InsertBefore(retInstruction, logParamName);

            // load return value on stack
            Instruction retValue = body.CilWorker.Create(OpCodes.Ldloc, returnValueVar);
            body.CilWorker.InsertBefore(retInstruction, retValue);

            LogValue(body, returnType, retInstruction);

            // load return value from temp variable back on stack
            Instruction loadRetValueInstr = body.CilWorker.Create(OpCodes.Ldloc, returnValueVar);
            body.CilWorker.InsertBefore(retInstruction, loadRetValueInstr);
        }
    }
}
