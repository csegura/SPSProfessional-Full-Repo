using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// This factory is used to create a tree
    /// </summary>
    internal sealed class SPSHierarchyFactory
    {
        private SPSHierarchyFilter _filter;
        private SPSNodeFactory _nodeFactory;

        /// <summary>
        /// Gets the node factory.
        /// </summary>
        /// <value>The node factory.</value>
        private SPSNodeFactory NodeFactory
        {
            get { return _nodeFactory ?? (_nodeFactory = new SPSNodeFactory(Filter)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyFactory"/> class.
        /// </summary>
        public SPSHierarchyFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyFactory"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public SPSHierarchyFactory(SPSHierarchyFilter filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public SPSHierarchyFilter Filter
        {
            get { return _filter ?? new SPSHierarchyFilter(); }
            set { _filter = value; }
        }

        /// <summary>
        /// Makes the web tree.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns>The root node</returns>
        public SPSTreeNode<ISPSHierarchyNode> MakeWebTree(SPWeb web)
        {
            SPSTreeNode<ISPSHierarchyNode> rootNode = new SPSTreeNode<ISPSHierarchyNode>(NodeFactory.MakeWebNode(web));
            VisitWeb(rootNode, web);
            return rootNode;
        }

        /// <summary>
        /// Makes the list tree.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns>The root node</returns>
        public SPSTreeNode<ISPSHierarchyNode> MakeListTree(SPWeb web)
        {
            SPSTreeNode<ISPSHierarchyNode> rootNode = new SPSTreeNode<ISPSHierarchyNode>(NodeFactory.MakeWebNode(web));
            VisitList(rootNode, web);
            return rootNode;
        }

        /// <summary>
        /// Makes the folder tree.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>The root node</returns>
        public SPSTreeNode<ISPSHierarchyNode> MakeFolderTree(SPList list)
        {
            SPSTreeNode<ISPSHierarchyNode> rootNode =
                    new SPSTreeNode<ISPSHierarchyNode>(NodeFactory.MakeFolderNode(list, list.RootFolder));
            VisitFolder(rootNode, list, list.RootFolder);
            return rootNode;
        }

        #region Visitors

        ///// <summary>
        ///// Visits the web.
        ///// </summary>
        ///// <param name="rootNode">The root node.</param>
        ///// <param name="web">The web.</param>
        //private void VisitWeb(ISPSTreeNode<ISPSHierarchyNode> rootNode, SPWeb web)
        //{
        //     SPSTreeNode<ISPSHierarchyNode> newNode = rootNode.Add(NodeFactory.MakeWebNode(web));

        //     if (newNode.Deep < Filter.MaxDeepth)
        //     {
        //         if (Filter.IncludeLists)
        //         {
        //             VisitList(newNode, web);
        //         }
        //     }

        //     VisitSubWebs(newNode,web);
        //}

        private void VisitWeb(ISPSTreeNode<ISPSHierarchyNode> rootNode, SPWeb web)
        {

            if (rootNode.Deep < Filter.MaxDeepth)
            {
                if (Filter.IncludeLists)
                {
                    VisitList(rootNode, web);
                }
            }

            foreach (SPWeb subWeb in web.GetSubwebsForCurrentUser())
            {
                ISPSTreeNode<ISPSHierarchyNode> newNode = rootNode.Add(NodeFactory.MakeWebNode(subWeb));

                if (newNode.Deep < Filter.MaxDeepth)
                {                    
                    if (Filter.Apply(web))
                    {
                        VisitWeb(newNode, subWeb);
                    }
                }

                subWeb.Dispose();
            }

            if (Filter.SortHierarchy)
                rootNode.Sort();
        }

        /// <summary>
        /// Visits the list.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        /// <param name="web">The web.</param>
        private void VisitList(ISPSTreeNode<ISPSHierarchyNode> rootNode, SPWeb web)
        {
            foreach (SPList list in web.Lists)
            {
                if (Filter.Apply(list))
                {
                    ISPSTreeNode<ISPSHierarchyNode> newNode = rootNode.Add(NodeFactory.MakeListNode(list));

                    if (newNode.Deep < Filter.MaxDeepth && Filter.IncludeFolders)
                    {
                        VisitFolder(newNode, list, list.RootFolder);
                    }
                }
            }

            if (Filter.SortHierarchy)
                rootNode.Sort();
        }

        /// <summary>
        /// Visits the folder.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        /// <param name="list">The list.</param>
        /// <param name="rootFolder">The root folder.</param>
        private void VisitFolder(ISPSTreeNode<ISPSHierarchyNode> rootNode, SPList list, SPFolder rootFolder)
        {
            foreach (SPFolder folder in rootFolder.SubFolders)
            {
                if (Filter.Apply(folder))
                {
                    ISPSTreeNode<ISPSHierarchyNode> newNode = rootNode.Add(NodeFactory.MakeFolderNode(list, folder));
                    
                    if (newNode.Deep < Filter.MaxDeepth)
                    {
                        VisitFolder(newNode, list, folder);
                    }
                }
            }

            if (Filter.SortHierarchy)
                rootNode.Sort();
        }

        #endregion
    }
}