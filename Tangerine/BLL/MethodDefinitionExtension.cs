using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Tangerine.BLL.Hooks;

namespace Tangerine.BLL
{
    public static class MethodDefinitionExtension
    {
        /// <summary>
        /// Returns full method name in format {Namespace}.{Method}
        /// </summary>
        public static string ToShortString(this MethodDefinition methodDefinition)
        {
            return String.Format("{0}.{1}", methodDefinition.DeclaringType.FullName, methodDefinition.Name);
        }

        /// <summary>
        /// Returns method name in format {Return type} {Namespace}.{Method}({Parameter types})
        /// </summary>
        /// <param name="methodDefinition"></param>
        /// <returns></returns>
        public static string ToDisplayString(this MethodDefinition methodDefinition)
        {
            return String.Format(
                "{0} {1}.{2}({3})",
                methodDefinition.ReturnType.ReturnType.Name,
                methodDefinition.DeclaringType.FullName,
                methodDefinition.Name,
                String.Join(", ", methodDefinition.Parameters.Cast<ParameterDefinition>().Select(p => p.ParameterType.Name))
                );
        }

        /// <summary>
        /// Returns method name in format {Return type} {Namespace}.{Method}({Parameters})
        /// </summary>
        public static string ToLongString(this MethodDefinition methodDefinition)
        {
            return String.Format(
                "{0} {1}.{2}({3})",
                methodDefinition.ReturnType.ReturnType.Name,
                methodDefinition.DeclaringType.FullName,
                methodDefinition.Name,
                String.Join(", ", methodDefinition.Parameters.Cast<ParameterDefinition>().Select(p => p.ParameterType.Name + " " + p.Name))
                );
        }

        /// <summary>
        /// Returns method name in format {Return type} {Method}({Parameter types})
        /// </summary>
        /// <param name="methodDefinition"></param>
        /// <returns></returns>
        public static string ToTreeDisplayString(this MethodDefinition methodDefinition)
        {
            return String.Format(
                "{0} {1}({2})",
                methodDefinition.ReturnType.ReturnType.Name,
                methodDefinition.Name,
                String.Join(", ", methodDefinition.Parameters.Cast<ParameterDefinition>().Select(p => p.ParameterType.Name))
                );
        }
    }


    public static class MethodHookExtension
    {
        public static string GetSafeName(this MethodHook methodHook)
        {
            return methodHook.ToString()
                .Replace(" ", "_")
                .Replace(".", "_")
                .Replace("(", "_")
                .Replace(")", "")
                .Replace("[", "_")
                .Replace("]", "_")
                .Replace(",", "")
                .Replace("/", "_")
                .Replace("\\", "_");
        }
    }
}
