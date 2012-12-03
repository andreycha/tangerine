using Aga.Controls.Tree;

namespace Tangerine.UI.BLL.AssemblyTree
{
    internal sealed class ToolTipProv : IToolTipProvider
    {
        private string m_text;

        public ToolTipProv(string toolTipText)
        {
            m_text = toolTipText;
        }

        public string GetToolTip(TreeNodeAdv node)
        {
            return m_text;
        }
    }
}
