using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    internal class FolderExplorerControl : FollowViewControl
    {
        private bool _autoCollapse;
        private bool _expandAll;
        private int _expandDepth = 99;
        private string _filter;
        private bool _followListView;
        private bool _showCounter;
        private bool _hideUnderscoreFolders;
        private bool _navigateToList;
        private bool _sortHierarchy;
        private string _navigateToListView;
        private ISPSCacheService _cacheService;

        // Controls
        private SPSHierarchyDataSource dataSource;
        private TreeView treeView;

        #region Constructor

        public FolderExplorerControl(string listGuid, string listViewGuid) : base(listGuid, listViewGuid)
        {
        }

        #endregion

        #region Public Properties

        public int ExpandDepth
        {
            get { return _expandDepth; }
            set { _expandDepth = value; }
        }


        public bool ExpandAll
        {
            get { return _expandAll; }
            set { _expandAll = value; }
        }


        public bool FollowListView
        {
            get { return _followListView; }
            set { _followListView = value; }
        }


        public bool ShowCounter
        {
            get { return _showCounter; }
            set { _showCounter = value; }
        }


        public bool AutoCollapse
        {
            get { return _autoCollapse; }
            set { _autoCollapse = value; }
        }


        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }


        public bool HideUnderscoreFolders
        {
            get { return _hideUnderscoreFolders; }
            set { _hideUnderscoreFolders = value; }
        }

        public bool NavigateToList
        {
            get { return _navigateToList; }
            set { _navigateToList = value; }
        }

        public string NavigateToListView
        {
            get { return _navigateToListView; }
            set { _navigateToListView = value; }
        }

        public ISPSCacheService CacheService
        {
            get { return _cacheService; }
            set { _cacheService = value; }
        }

        public bool SortHierarchy
        {
            get { return _sortHierarchy; }
            set { _sortHierarchy = value; }
        }

        #endregion

        #region Private Properties

        #endregion

        #region Control

        /// <exception cref="SPSException"><c>SPSException</c>.</exception>
        protected override void CreateChildControls()
        {
            try
            {
                using(SPWeb web = SPContext.Current.Web.Site.OpenWeb())
                {
                    using(dataSource = new SPSHierarchyDataSource(web, GetList()))
                    {
                        dataSource.CacheService = CacheService;
                        SPSHierarchyFilter dataFilter = new SPSHierarchyFilter
                                                            {
                                                                    IncludeFolders = true,
                                                                    IncludeNumberOfFiles = ShowCounter,
                                                                    IncludeWebs = false,
                                                                    IncludeLists = false,
                                                                    SortHierarchy = _sortHierarchy,
                                                                    HideUnderscoreFolders = HideUnderscoreFolders
                                                            };

                        if (!string.IsNullOrEmpty(Filter))
                        {
                            dataFilter.OnFilter += DataSourceFilter;
                        }

                        dataSource.Filter = dataFilter;

                        treeView = SPSTreeViewHelper.MakeTreeView(dataSource,GenerateLinkForPath);                        
                        treeView.ExpandDepth = ExpandDepth;

                        Controls.Add(treeView);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SPSException(ex.TargetSite.Name, ex);             
            }
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            EnsureChildControls();

            //GetFolderFromQueryString();

            if (FollowListView)
            {
                FollowListViewNavigation();
            }

            treeView.RenderControl(writer);            
        }

        #endregion

        #region Public methods

        #endregion

        /// <summary>
        /// Filter the hierarchy.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        private bool DataSourceFilter(object sender, SPSHierarchyFilterArgs args)
        {
            if (args.Folder != null)
            {
                return args.Folder.Name.ToUpper().Contains(Filter);
            }

            return true;
        }

        /// <summary>
        /// Generates the link for an specified path.
        /// The behaviour depends on NavigateToList flag.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="hierarchyNode">The hierarchy node.</param>
        private void GenerateLinkForPath(TreeNode treeNode, ISPSHierarchyNode hierarchyNode)
        {
            //if (treeNode != null)
            {
                if (_navigateToList)
                {
                    treeNode.NavigateUrl = GenerateLinkToList(hierarchyNode.Path);
                }
                else if (hierarchyNode.NodeType == SPSHierarchyNodeType.Folder) //treeNode.Parent != null)
                {                   
                    treeNode.NavigateUrl = GenerateLinkToView(hierarchyNode.Path);
                }
                else
                {
                    treeNode.NavigateUrl = GenerateLinkToView(GetList().RootFolder.ServerRelativeUrl);
                }
            }
        }

        /// <summary>
        /// Follows the list view navigation.
        /// Auto collapse all nodes except the selected one
        /// </summary>
        private void FollowListViewNavigation()
        {
#if DEBUG
            Debug.WriteLine("FollowListViewNavigation:" + AdjustFolderToTree());
#endif
            // Get the node in tree
            TreeNode treeNode = treeView.FindNode(AdjustFolderToTree());

            if (treeNode != null)
            {
                FollowSelectNode(treeNode);
            }
            else
            {
                if (treeView.Nodes != null)
                {
                    FollowSelectNode(treeView.Nodes[0]);
                }
            }
        }

        /// <summary>
        /// Follows the select node.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        private void FollowSelectNode(TreeNode treeNode)
        {
            // Collapse all nodes except the selected
            treeView.CollapseAll();
            treeNode.Expand();
            treeNode.Select();
            treeNode.Selected = true;

            if (!AutoCollapse)
            {
                while ((treeNode = treeNode.Parent) != null)
                {
                    treeNode.Expand();
                }
            }
        }

        /// <summary>
        /// AdjustFolderToTree
        /// CurrentFolder comes in format site/lists/listName/folderName for example
        /// /Team/Lists/Tasks/2008 in order to search in the tree we need remove
        /// the site and list for lists and the first slash for document libraries
        /// For doc libs we nees Shared Documents/Folder and the CurrentFolder contains
        /// /site/Shared Documents/Folder
        /// </summary>
        /// <returns>
        /// The relative list path used to find a node in a tree
        /// </returns>
        private string AdjustFolderToTree()
        {
            string pathToRemove = SPContext.Current.Web.ServerRelativeUrl;

            if (ListIsDocumentLibrary)
            {
                pathToRemove += SLASH;
            }
            else
            {
                pathToRemove += SLASH + "Lists" + SLASH;
            }

            return CurrentFolder.Replace(pathToRemove, string.Empty);
        }

        /// <summary>
        /// Generates the link to list.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Link to view url and passed folder path</returns>
        protected override string GenerateLinkToList(string path)
        {
            // use base GenerateLinkToList url
            SPView view;
            string hrefArgs = string.Empty;

            if (!string.IsNullOrEmpty(_navigateToListView))
            {
                view = GetList().Views[new Guid(_navigateToListView)];
                hrefArgs = string.Format("{0}/{1}?RootFolder={2}&View={3}",
                                         SPContext.Current.Web.Url,
                                         SPHttpUtility.UrlKeyValueEncode(view.Url),
                                         SPHttpUtility.UrlKeyValueEncode(path),
                                         SPHttpUtility.UrlKeyValueEncode(view.ID));
            }

            return hrefArgs;
        }
    }
}