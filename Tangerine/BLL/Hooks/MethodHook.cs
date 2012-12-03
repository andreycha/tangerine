using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Tangerine.BLL.Hooks
{
    public class MethodHook
    {
        private bool m_logMethodName;
        private bool m_runCustomCode;

        public MethodDefinition Method { get; private set; }

        public bool LogMethodName
        {
            get { return m_logMethodName; }
            set 
            { 
                m_logMethodName = value;
                if (!value)
                {
                    LogParameterValues = false;
                    LogReturnValues = false;
                }
            }
        }

        public bool LogParameterValues { get; set; }

        public bool LogReturnValues { get; set; }

        public bool RunCustomCode
        {
            get { return m_runCustomCode; }
            set 
            { 
                m_runCustomCode = value;
                if (!value)
                {
                    HookType = HookType.None;
                }
            }
        }

        public HookType HookType { get; set; }

        public string Code { get; set; }

        public MethodHook(MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            Method = method;
        }

        internal string ToShortString()
        {
            return Method.ToShortString();
        }

        public override string ToString()
        {
            return Method.ToDisplayString();
        }

        internal string ToLongString()
        {
            return Method.ToLongString();
        }
    }


    [Flags]
    public enum HookType
    {
        None = 0,
        ReplaceMethod = 2,
        OnMethodEnter = 4,
        OnMethodExit = 8,
    }
}
