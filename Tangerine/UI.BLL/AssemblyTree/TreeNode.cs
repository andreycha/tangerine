using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Controls.Tree;
using System.Drawing;
using Tangerine.Properties;

namespace Tangerine.UI.BLL
{
    public abstract class TreeNode : Node
    {
        public TreeNode(string text)
            : base(text)
        { }

        public Image Icon { get; set; }
    }

    public class AssemblyNode : TreeNode
    {
        public AssemblyNode(string text)
            : base(text)
        {
            Icon = Resources.assembly;
        }
    }

    public class TypeNode : TreeNode
    {
        public TypeNode(string text)
            : base(text)
        {
            Icon = Resources._class;
        }
    }

    public class EnumNode : TreeNode
    {
        public EnumNode(string text)
            : base(text)
        {
            Icon = Resources._enum;
        }
    }
}
