using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tangerine.UI.BLL
{
    public class AssemblyTreeModel : ITreeModel
    {
        private Node _root;

        public event EventHandler<TreePathEventArgs> StructureChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreeModelEventArgs> NodesChanged;

        public Node Root
        {
            get { return _root; }
        }
        public Collection<Node> Nodes
        {
            get { return _root.Nodes; }
        }

        private bool m_filterIO;

        public bool FilterIO
        {
            get
            {
                return m_filterIO;
            }
            set
            {
                m_filterIO = value;
                OnStructureChanged(new TreePathEventArgs());
            }
        }

        private bool m_filterNet;

        public bool FilterNet 
        { 
            get
            {
                return m_filterNet;
            }
            set
            {
                m_filterNet = value;
                OnStructureChanged(new TreePathEventArgs());
            }
        }

        private bool m_filterSecurity;

        public bool FilterSecurity 
        {
            get
            {
                return m_filterSecurity;
            }
            set
            {
                m_filterSecurity = value;
                OnStructureChanged(new TreePathEventArgs());
            }
        }

        public AssemblyTreeModel()
        {
            _root = new Node();
        }

        public TreePath GetPath(Node node)
        {
            if (node == _root)
                return TreePath.Empty;
            else
            {
                Stack<object> stack = new Stack<object>();
                while (node != _root)
                {
                    stack.Push(node);
                    node = node.Parent;
                }
                return new TreePath(stack.ToArray());
            }
        }
        public Node FindNode(TreePath path)
        {
            if (path.IsEmpty())
                return _root;
            else
                return FindNode(_root, path, 0);
        }
        private Node FindNode(Node root, TreePath path, int level)
        {
            foreach (Node node in root.Nodes)
            {
                if (node == path.FullPath[level])
                {
                    if (level == path.FullPath.Length - 1)
                        return node;
                    else
                        return FindNode(node, path, level + 1);
                }
            }
            return null;
        }

        bool IsContainIO(Node node)
        {
            if (node is MethodNode)
                return ((MethodNode)node).IOIcon != null;
            else
            {
                var result = false;
                foreach (var child in node.Nodes)
                    result = result || IsContainIO(child);
                return result;
            }
        }

        bool IsContainNet(Node node)
        {
            if (node is MethodNode)
                return ((MethodNode)node).NetIcon != null;
            else
            {
                var result = false;
                foreach (var child in node.Nodes)
                    result = result || IsContainNet(child);
                return result;
            }
        }

        bool IsContainSecurity(Node node)
        {
            if (node is MethodNode)
                return ((MethodNode)node).SecurityIcon != null;
            else
            {
                var result = false;
                foreach (var child in node.Nodes)
                    result = result || IsContainSecurity(child);
                return result;
            }
        }

        public virtual System.Collections.IEnumerable GetChildren(TreePath treePath)
        {
            Node node = FindNode(treePath);

            if (node != null)
                if (treePath.FullPath.Length == 0 && node.Nodes.Count() == 1)
                    yield return node.Nodes.ElementAt(0);
                else foreach (Node n in node.Nodes)
                {
                    var needToAdd = false;
                    if (!m_filterIO && !m_filterNet && !m_filterSecurity)
                        needToAdd = true;
                    if (!needToAdd && m_filterIO && IsContainIO(n))
                        needToAdd = true;
                    if (!needToAdd && m_filterNet && IsContainNet(n))
                        needToAdd = true;
                    if (!needToAdd && m_filterSecurity && IsContainSecurity(n))
                        needToAdd = true;
                    if (needToAdd)
                        yield return n;
                }
            else
                yield break;
        }
        public virtual bool IsLeaf(TreePath treePath)
        {
            Node node = FindNode(treePath);
            if (node != null)
                return node.IsLeaf;
            else
                throw new ArgumentException("treePath");
        }

        public virtual void OnStructureChanged(TreePathEventArgs args)
        {
            if (StructureChanged != null)
                StructureChanged(this, args);
        }

        internal protected virtual void OnNodesChanged(Node parent, int index, Node node)
        {
            if (NodesChanged != null)
            {
                TreePath path = GetPath(parent);
                if (path == null) return;
                TreeModelEventArgs args = new TreeModelEventArgs(path, new int[] { index }, new object[] { node });
                NodesChanged(this, args);
            }
        }
        internal protected virtual void OnNodeInserted(Node parent, int index, Node node)
        {
            if (NodesInserted != null)
            {
                TreeModelEventArgs args = new TreeModelEventArgs(GetPath(parent), new int[] { index }, new object[] { node });
                NodesInserted(this, args);
            }

        }
        internal protected virtual void OnNodeRemoved(Node parent, int index, Node node)
        {
            if (NodesRemoved != null)
            {
                TreeModelEventArgs args = new TreeModelEventArgs(GetPath(parent), new int[] { index }, new object[] { node });
                NodesRemoved(this, args);
            }
        }
    }
}
