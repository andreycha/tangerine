using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Tangerine.BLL
{
    public sealed class XAPAssembly
    {
        private readonly string m_assemblyPath;
        private readonly AssemblyDefinition m_assemblyDefinition;

        public string AssemblyPath
        {
            get { return m_assemblyPath; }
        }

        public AssemblyDefinition AssemblyDefinition
        {
            get { return m_assemblyDefinition; }
        }

        public XAPAssembly(string assemblyPath)
        {
            m_assemblyPath = assemblyPath;
            m_assemblyDefinition = LoadAssembly(assemblyPath);
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

        public IEnumerable<TypeDefinition> GetTypes()
        {
            foreach (TypeDefinition typeDefinition in m_assemblyDefinition.MainModule.Types)
            {
                if (typeDefinition.Name != "<Module>")
                {
                    yield return typeDefinition;
                }
            }
        }

        public IEnumerable<MethodDefinition> GetMethods()
        {
            foreach (TypeDefinition typeDefinition in GetTypes())
            {
                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                {
                    yield return methodDefinition;
                }
            }
        }

        public IEnumerable<MethodDefinition> GetMethods(string fullTypeName)
        {
            TypeDefinition typeRef = m_assemblyDefinition.MainModule.Types.Cast<TypeDefinition>().FirstOrDefault(t => t.FullName == fullTypeName);
            if (typeRef == null)
            {
                throw new InvalidOperationException(String.Format("No type defined with name '{0}'", fullTypeName));
            }

            foreach (MethodDefinition methodDefinition in typeRef.Methods)
            {
                yield return methodDefinition;
            }
        }
    }
}
