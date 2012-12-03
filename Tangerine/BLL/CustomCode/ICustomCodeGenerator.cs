using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangerine.BLL.Hooks;

namespace Tangerine.BLL.CustomCode
{
    internal interface ICustomCodeGenerator
    {
        void GenerateAssembly(MethodHook methodHook, string path);
    }
}
