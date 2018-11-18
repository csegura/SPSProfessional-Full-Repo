using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    /// <summary>
    ///  Help to load a tree view using ISPSHierarchyNodes or SPSHiearchyDataSource
    /// </summary>
    public static class SPSTreeViewHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public delegate void TreeBound(TreeNode treeNode, ISPSHierarchyNode hierarchyNode);

        /// <summary>
        /// Makes the tree view.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="delegateBound">The delegate bound.</param>
        /// <returns></returns>
        public static TreeView MakeTreeView(SPSHierarchyDataSource dataSource, TreeBound delegateBound)
        {
            TreeView tree = new TreeView();
            tree.Nodes.Add(MakeTreeViewInternal(dataSource.Root, delegateBound));
            DecorateTree(tree);
            return tree;
        }

        /// <summary>
        /// Makes the tree view.
        /// </summary>
        /// <param name="spsRoot">The SPS root.</param>
        /// <param name="delegateBound">The delegate bound.</param>
        /// <returns></returns>
        public static TreeView MakeTreeView(ISPSTreeNode<ISPSHierarchyNode> spsRoot, TreeBound delegateBound)
        {
            TreeView tree = new TreeView();
            tree.Nodes.Add(MakeTreeViewInternal(spsRoot, delegateBound));     
            
            DecorateTree(tree);
            return tree;
        }

        /// <summary>
        /// Fill the tree view.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="spsRoot">The SPS root.</param>
        /// <param name="delegateBound">The delegate bound.</param>
        public static void FillTreeView(TreeView treeView, ISPSTreeNode<ISPSHierarchyNode> spsRoot, TreeBound delegateBound)
        {            
            treeView.Nodes.Add(MakeTreeViewInternal(spsRoot, delegateBound));
            DecorateTree(treeView);
        }

        /// <summary>
        /// Gets the tree.
        /// </summary>
        /// <param name="spsRoot">The SPS root.</param>
        /// <param name="delegateBound">The delegate bound.</param>
        /// <returns></returns>
        private static TreeNode MakeTreeViewInternal(ISPSTreeNode<ISPSHierarchyNode> spsRoot, TreeBound delegateBound)
        {
            ISPSHierarchyNode node = spsRoot.Node;
            TreeNode treeNode = new TreeNode(node.Name, node.UrlSegment, node.ImageUrl, node.NavigateUrl, string.Empty);

            if (delegateBound != null)
            {
                delegateBound(treeNode, node);
            }

            foreach (ISPSTreeNode<ISPSHierarchyNode> child in spsRoot.Children)
            {
                treeNode.ChildNodes.Add(MakeTreeViewInternal(child, delegateBound));
            }

            return treeNode;
        }

        /// <summary>
        /// Decorates the tree.
        /// </summary>
        /// <param name="tree">The tree.</param>
        public static void DecorateTree(TreeView tree)
        {
            tree.ShowLines = true;
            tree.EnableClientScript = true;
            tree.EnableViewState = true;
            tree.NodeStyle.CssClass = "ms-navitem";
            tree.NodeStyle.HorizontalPadding = 2;
            tree.SelectedNodeStyle.CssClass = "ms-tvselected";
            tree.SkipLinkText = "";
            tree.NodeIndent = 12;
            tree.ExpandImageUrl = "/_layouts/images/tvplus.gif";
            tree.CollapseImageUrl = "/_layouts/images/tvminus.gif";
            tree.NoExpandImageUrl = "/_layouts/images/tvblank.gif";

            if (SPUtility.IsRightToLeft(SPContext.Current.Web,SPContext.Current.Web.Language))
            {
                tree.LineImagesFolder = "/_layouts/SPSProfessionalFW/TreeRTL";
            }
        }

        //private static TreeNode GetTreeRecursive(TreeNode root, SPSTreeNode<ISPSHierarchyNode> spsRoot)
        //{
        //    ISPSHierarchyNode node = spsRoot.Node;

        //    TreeNode treeNode = new TreeNode(node.Name, node.UrlSegment, node.ImageUrl, node.NavigateUrl, "");
        //    root.ChildNodes.Add(treeNode);

        //    foreach (SPSTreeNode<ISPSHierarchyNode> child in spsRoot.Children)
        //    {
        //        treeNode.ChildNodes.Add(GetTreeRecursive(root, child));
        //    }

        //    return treeNode;
        //}

    }
}