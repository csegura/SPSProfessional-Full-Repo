using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.Framework.HierarchyOnDemand
{
    /// <summary>
    /// This factory is used to create a tree
    /// </summary>
    internal sealed class SPSHierarchyODFactory
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
        public SPSHierarchyODFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyFactory"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public SPSHierarchyODFactory(SPSHierarchyFilter filter)
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
        /// Makes the folder nodes.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        public ISPSTreeNode<ISPSHierarchyNode> MakeFolderNodes(SPFolder rootFolder)
        {
            ISPSTreeNode<ISPSHierarchyNode> rootNode = new SPSTreeNode<ISPSHierarchyNode>(
                NodeFactory.MakeFolderNode(rootFolder.ParentWeb.Lists[rootFolder.ParentListId], rootFolder));

            if (Filter.IncludeFolders)
            {
                foreach (SPFolder folder in rootFolder.SubFolders)
                {
                    if (Filter.Apply(folder))
                    {
                        rootNode.Add(NodeFactory.MakeFolderNode(folder.ParentWeb.Lists[folder.ParentListId], folder));
                    }
                }

                if (Filter.SortHierarchy)
                    rootNode.Sort();
            }

            return rootNode;
        }

        /// <summary>
        /// Makes the web and list nodes.
        /// </summary>
        /// <param name="web">The web.</param>
        public ISPSTreeNode<ISPSHierarchyNode> MakeWebAndListNodes(SPWeb web)
        {
            ISPSTreeNode<ISPSHierarchyNode> rootNode = 
                new SPSTreeNode<ISPSHierarchyNode>(NodeFactory.MakeWebNode(web));

            if (Filter.IncludeWebs)
            {
                foreach (SPWeb subWeb in web.GetSubwebsForCurrentUser())
                {
                    if (Filter.Apply(subWeb))
                    {
                        rootNode.Add(NodeFactory.MakeWebNode(subWeb));
                    }
                    subWeb.Dispose();
                }
            }

            if (Filter.IncludeLists)
            {
                foreach (SPList list in web.Lists)
                {
                    if (Filter.Apply(list))
                    {
                        rootNode.Add(NodeFactory.MakeListNode(list));
                    }
                }
            }

            if (Filter.SortHierarchy)
                rootNode.Sort();

            return rootNode;
        }
         
    }
}