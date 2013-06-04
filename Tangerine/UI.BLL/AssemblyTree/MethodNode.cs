using Aga.Controls.Tree;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Drawing;
using System.Linq;
using Tangerine.BLL;
using Tangerine.Properties;

namespace Tangerine.UI.BLL
{
    public static class MethodNodeFactory
    {
        private const string GetLabel = "get";
        private const string SetLabel = "set";

        private static string[] ioNamespaces = new string[] { 
            //wp7
            "Microsoft.Devices", "Microsoft.Phone.BackgroundAudio", "Microsoft.Phone.Data.Linq", "Microsoft.Phone.Storage",
            "System.Data.Linq", "System.Device.Location", "System.IO", "System.Xml",
            //wp8
            "Windows.ApplicationModel.DataTransfer", "Windows.Devices.Input", "Windows.Devices.Sensors", "Windows.Storage", 
            "Windows.Phone.Devices", "Windows.Phone.Media.Capture", " Windows.Phone.Media.Devices", "Windows.Phone.Speech",
            "Windows.Phone.Storage"
        };

        private static string[] netNamespaces = new string[] { 
            //wp7
            "Microsoft.Phone.BackgroundTransfer", "Microsoft.Phone.Net.NetworkInformation", "Microsoft.Phone.Networking.Voip", 
            "Microsoft.Phone.Notification", "System.Net", 
            //wp8
            "Windows.Devices.Geolocation", "Windows.Networking", "Windows.Phone.Networking"
        };

        private static string[] securityNamespaces = new string[] { 
            //wp7
            "Microsoft.Phone.Marketplace", "Microsoft.Phone.SecureElement", "Microsoft.Phone.Wallet", "System.Security",
            //wp8
            "Windows.ApplicationModel.Store", "Windows.Security", "Windows.Phone.Management.Deployment"
        };

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
                    var operandNS = (instruction.Operand as MemberReference).DeclaringType.Namespace;
                    //haha funny way to check if searched Namespace is the first in operand Namespace
                    //if (ioNamespaces.Where(ns => operandNS.Contains(ns) && new string(operandNS.Take(ns.Length).ToArray()) == ns).Count() > 0)
                    if (ioNamespaces.Where(ns => operandNS.Contains(ns)).Count() > 0)
                    {
                        node.IOIcon = Resources.file;
                    }
                    else if (netNamespaces.Where(ns => operandNS.Contains(ns)).Count() > 0)
                    {
                        node.NetIcon = Resources.network;
                    }
                    else if (securityNamespaces.Where(ns => operandNS.Contains(ns)).Count() > 0)
                    {
                        node.SecurityIcon = Resources.security;
                    }
                }
            }
        }
    }


    public class MethodNode : TreeNode
    {
        public MethodNode(MethodDefinition definition, string text)
            : base(text)
        {
            Definition = definition;
            Icon = Resources.method;
        }

        public Image IOIcon { get; set; }

        public Image NetIcon { get; set; }

        public Image SecurityIcon { get; set; }

        public MethodDefinition Definition { get; private set; }

    }
}
