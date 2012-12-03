using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Tangerine.BLL.Hooks
{
    internal interface IHookProvider
    {
        bool LogMethodNames { get; }

        bool LogParameterValues { get; }

        bool LogReturnValues { get; }

        MethodHook GetMethodHook(MethodDefinition method);
    }
}
