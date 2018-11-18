using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Generic SPSTreeNode class
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    public class SPSTreeNode<T> : ISPSTreeNode<T>
    {
        private readonly T _node;
        private int _deep;
        private readonly List<ISPSTreeNode<T>> _children;
        private ISPSTreeNode<T> _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSTreeNode&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public SPSTreeNode(T node) 
        {
            _children = new List<ISPSTreeNode<T>>();
            _node = node;
        }

        /// <summary>
        /// Adds the specified child.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        public ISPSTreeNode<T> Add(T child)
        {
            SPSTreeNode<T> newNode = new SPSTreeNode<T>(child)
                                         {
                                                 _deep = (_deep + 1),
                                                 _parent = this
                                         };
            _children.Add(newNode);
            return newNode;
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>The node.</value>
        public T Node
        {
            get { return _node; }
        }


        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public List<ISPSTreeNode<T>> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public ISPSTreeNode<T> Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the deep.
        /// </summary>
        /// <value>The deep.</value>
        public int Deep
        {
            get { return _deep; }
        }

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        public void Sort()
        {
            _children.Sort((x, y) => string.Compare(x.Node.ToString(), y.Node.ToString()));
        }

        // Display _node and _children at given indentation
        /// <summary>
        /// Displays the specified indentation.
        /// </summary>
        /// <param name="indentation">The indentation.</param>
        // TODO: Remove or Conditional Debug
        public void Display(int indentation)
        {
            string line = new String('-', indentation) + " + ";

            Debug.WriteLine(Deep + line + Node);

            foreach (SPSTreeNode<T> treeNode in _children)
            {
                int indent = indentation + 1;

                if (treeNode.Children.Count > 0)
                {
                    treeNode.Display(++indent);
                }
                else
                {
                    line = new String('-', indent) + "  >";
                    Debug.WriteLine(Deep + line + treeNode.Node);
                }
            }
        }
    }
}