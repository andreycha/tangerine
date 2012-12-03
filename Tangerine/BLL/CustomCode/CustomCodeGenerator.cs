using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using Mono.Cecil;
using Tangerine.BLL.Hooks;

namespace Tangerine.BLL.CustomCode
{
    internal class CustomCodeGenerator : ICustomCodeGenerator
    {
        private const string CustomNamespace = "TangerineCustom";
        private const string ReferencesAssembliesPathx86 = @"C:\Program Files\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\Profile\WindowsPhone71\";
        private const string ReferencesAssembliesPathx64 = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\Profile\WindowsPhone71\";

        private readonly string[] m_referencedAssemblies = new string[]
        {
            "mscorlib.dll",
            "mscorlib.Extensions.dll",
            "System.dll",
            "System.Core.dll",
            "System.Net.dll",
            "System.Windows.dll",
            "Microsoft.Phone.dll"
        };
        private readonly List<string> m_referencedNamespaces = new List<string>()
        {
            "System",
            "System.Collections",
            "System.Collections.Generic",
            "System.IO",
            "System.IO.IsolatedStorage",
            "System.Text",
            "System.Windows"
        };

        void ICustomCodeGenerator.GenerateAssembly(MethodHook methodHook, string path)
        {
            if (methodHook == null)
            {
                throw new ArgumentNullException("methodHook");
            }
            if (!methodHook.RunCustomCode)
            {
                throw new ArgumentException("RunCustomCode should be set in order to generate custom code");
            }

            StringBuilder customCode = new StringBuilder();
            foreach (string ns in m_referencedNamespaces)
            {
                customCode.AppendLine(String.Format("using {0};", ns));
            }
            customCode.AppendLine();

            customCode.AppendLine(String.Format("namespace {0}", CustomNamespace));
            customCode.AppendLine("{");

            string name = GetName(methodHook);
            string className = String.Format("{0}_{1}", name, "Class");
            customCode.AppendLine(String.Format("public class {0}", className));
            customCode.AppendLine("{");

            string returnType = GetReturnType(methodHook);
            string methodName = String.Format("{0}_{1}", name, "Hook");
            var parameters = methodHook.Method.Parameters.Cast<ParameterDefinition>().Select(
                p => "ref " + p.ParameterType.Name + " " + p.Name
                );
            string parametersString = String.Join(", ", parameters);
            customCode.AppendLine(String.Format("public {0} {1}({2})", returnType, methodName, parametersString));
            customCode.AppendLine("{");

            customCode.AppendLine(methodHook.Code);

            // method
            customCode.AppendLine("}");
            // class
            customCode.AppendLine("}");
            // namespace
            customCode.AppendLine("}");

            GenerateAssembly(methodHook, customCode.ToString(), path);
        }

        private string GetName(MethodHook methodHook)
        {
            return methodHook.GetSafeName();
        }

        private string GetReturnType(MethodHook methodHook)
        {
            string returnTypeName = methodHook.Method.ReturnType.ReturnType.FullName;
            return (returnTypeName != "System.Void") ? returnTypeName : "void";
        }

        private void GenerateAssembly(MethodHook methodHook, string code, string path)
        {
            var compilerOptions = new Dictionary<string, string>();
            compilerOptions.Add("CompilerVersion", "v4.0");

            CSharpCodeProvider provider = new CSharpCodeProvider(compilerOptions);

            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Clear();
            cp.ReferencedAssemblies.AddRange(GetReferencedAssemblies());
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            // do not include standard mscorlib.dll
            cp.CompilerOptions = "/noconfig /nostdlib";
            string dllName = String.Format("{0}.dll", methodHook.GetSafeName());
            cp.OutputAssembly = Path.Combine(path, dllName);

            CompilerResults results = provider.CompileAssemblyFromSource(cp, code);
            if (results.Errors.Count > 0)
            {
                string errors = String.Join(Environment.NewLine, results.Errors.Cast<CompilerError>().Select(e => e.ErrorText));
                throw new InvalidOperationException("Unable to compile custom code: " + Environment.NewLine + errors);
            }
        }

        private string[] GetReferencedAssemblies()
        {
            var assemblies = new List<string>();
            string path = GetReferencedAssembliesPath();
            foreach (string assembly in m_referencedAssemblies)
            {
                assemblies.Add(Path.Combine(path, assembly));
            }
            return assemblies.ToArray();
        }

        private string GetReferencedAssembliesPath()
        {
            if (Directory.Exists(ReferencesAssembliesPathx64))
            {
                return ReferencesAssembliesPathx64;
            }
            if (Directory.Exists(ReferencesAssembliesPathx86))
            {
                return ReferencesAssembliesPathx86;
            }
            throw new InvalidOperationException("Windows Phone 7.1 SDK assemblies was not found");
        }
    }
}
