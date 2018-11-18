using System.Collections.Generic;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Tree node implemented as Composite
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISPSTreeNode<T>
    {
        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>The node.</value>
        T Node
        {
            get;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        List<ISPSTreeNode<T>> Children
        {
            get;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        ISPSTreeNode<T> Parent
        {
            get;
        }

        /// <summary>
        /// Gets the deep.
        /// </summary>
        /// <value>The deep.</value>
        int Deep
        {
            get;
        }

        /// <summary>
        /// Adds the specified child.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        ISPSTreeNode<T> Add(T child);

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        void Sort();

        /// <summary>
        /// Displays the specified indentation.
        /// </summary>
        /// <param name="indentation">The indentation.</param>
        void Display(int indentation);
    }
}