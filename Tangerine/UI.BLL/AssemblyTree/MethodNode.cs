using System.Drawing;
using Aga.Controls.Tree;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Tangerine.BLL;
using Tangerine.Properties;

namespace Tangerine.UI.BLL
{
    public static class MethodNodeFactory
    {
        private const string GetLabel = "get";
        private const string SetLabel = "set";

        public static MethodNode CreateNode(MethodDefinition definition)
        {
            string methodName = GetMethodName(definition);
            var node = new MethodNode(definition, methodName);
            ProcessDefinition(node, definition);
            return node;
        }

        private static string GetMethodName(MethodDefinition definition)
        {
            string methodName = definition.ToTreeDisplayString();
            if (definition.IsGetter)
            {
                methodName = GetLabel;
            }
            else if (definition.IsSetter)
            {
                methodName = SetLabel;
            }
            return methodName;
        }

        private static void ProcessDefinition(MethodNode node, MethodDefinition definition)
        {
            if (!definition.HasBody)
            {
                return;
            }

            foreach (Instruction instruction in definition.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Call
                    || instruction.OpCode == OpCodes.Callvirt
                    || instruction.OpCode == OpCodes.Calli)
                {
                    var operand = instruction.Operand.ToString();
                    if (operand.Contains("System.IO"))
                    {
                        node.IOIcon = Resources.file;
                    }
                    else if (operand.Contains("System.Net"))
                    {
                        node.NetIcon = Resources.network;
                    }
                    else if (operand.Contains("System.Security"))
                    {
                        node.SecurityIcon = Resources.security;
                    }
                }
            }
        }
    }


    public class MethodNode : Node
    {
        public MethodNode(MethodDefinition definition, string text)
            : base(text)
        {
            Definition = definition;
        }

        public Image IOIcon { get; set; }

        public Image NetIcon { get; set; }

        public Image SecurityIcon { get; set; }

        public MethodDefinition Definition { get; private set; }

    }
}
