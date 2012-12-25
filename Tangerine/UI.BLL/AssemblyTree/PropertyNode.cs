using Aga.Controls.Tree;
using Mono.Cecil;
using Tangerine.Properties;

namespace Tangerine.UI.BLL
{
    public static class PropertyNodeFactory
    {
        public static PropertyNode CreateNode(PropertyDefinition definition)
        {
            var propertyNode = new PropertyNode(definition, definition.Name);

            if (definition.GetMethod != null)
            {
                var getterNode = MethodNodeFactory.CreateNode(definition.GetMethod);
                propertyNode.Nodes.Add(getterNode);
            }

            if (definition.SetMethod != null)
            {
                var setterNode = MethodNodeFactory.CreateNode(definition.SetMethod);
                propertyNode.Nodes.Add(setterNode);
            }

            return propertyNode;
        }
    }

    public class PropertyNode : TreeNode
    {
        public PropertyNode(PropertyDefinition definition, string text)
            : base(text)
        {
            Definition = definition;
            Icon = Resources.property;
        }

        public PropertyDefinition Definition { get; private set; }
    }
}
