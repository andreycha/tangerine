using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Tangerine.BLL.Hooks
{
    internal class HookProvider : IHookProvider
    {
        private readonly Dictionary<string, MethodHook> m_hooks =
            new Dictionary<string, MethodHook>();

        public bool LogMethodNames { get; private set; }

        public bool LogParameterValues { get; private set; }

        public bool LogReturnValues { get; private set; }

        public MethodHook GetMethodHook(MethodDefinition method)
        {
            MethodHook hook;
            if (m_hooks.TryGetValue(method.ToDisplayString(), out hook))
            {
                return hook;
            }
            return null;
        }

        public HookProvider(bool logMethodNames, bool logParameterValues, bool logReturnValues, IEnumerable<MethodHook> methodHooks)
        {
            LogMethodNames = logMethodNames;
            LogParameterValues = logParameterValues;
            LogReturnValues = logReturnValues;
            foreach (var hook in methodHooks)
            {
                m_hooks.Add(hook.ToString(), hook);
            }
        }
    }
}
