using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.HierarchyOnDemand;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    public class WebExplorer : SPSWebPart
    {
        internal const char SLASH = '/';

        private int _expandDepth = 99;
        private string _filterList = string.Empty;
        private string _filterWeb = string.Empty;
        private string _rootWeb = string.Empty;
        private bool _showLists;
        private bool _showSubSites;
        private bool _showFolders;

        private TreeView treeView;

        public WebExplorer()
        {
            SPSInit("71D70B8F-1556-4d4c-925D-342EE0EE59C0",
                    "Navigation.2.0",
                    "WebExplorer",
                    "http://www.spsprofessional.com/page/Navigation-Web-Parts.aspx");
            EditorParts.Add(new WebExplorerParamsEditor());
        }

        #region CONFIGURATION PROPERTIES

        [Personalizable(PersonalizationScope.Shared)]
        public int ExpandDepth
        {
            get { return _expandDepth; }
            set { _expandDepth = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public override string HelpUrl
        {
            get { return "http://www.spsprofessional.com/page/Folder-Explorer-WebPart.aspx"; }
            set { base.HelpUrl = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string RootWeb
        {
            get { return _rootWeb; }
            set { _rootWeb = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowSubSites
        {
            get { return _showSubSites; }
            set { _showSubSites = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowLists
        {
            get { return _showLists; }
            set { _showLists = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string FilterWeb
        {
            get { return _filterWeb; }
            set { _filterWeb = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string FilterList
        {
            get { return _filterList; }
            set { _filterList = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowFolders
        {
            get { return _showFolders; }
            set { _showFolders = value; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            Debug.WriteLine("******* OnInit");
        }


        protected override void CreateChildControls()
        {
            Debug.WriteLine("Begin CreateChildControls");
            base.CreateChildControls();

            try
            {
                treeView = new TreeView
                           {
                                   ID = "treeView"
                           };

                Debug.WriteLine("Nodes");

                AddNodes(null, treeView.Nodes);

                Debug.WriteLine("Controls");
                SPSTreeViewHelper.DecorateTree(treeView);
                treeView.TreeNodePopulate += PopulateNode;
                treeView.EnableClientScript = true;
                treeView.PopulateNodesFromClient = true;
                treeView.ExpandDepth = ExpandDepth;

                Controls.Add(treeView);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ErrorMessage += ex.Message;
            }

            Debug.WriteLine("End CreateChildControls");
        }

        private void AddHierarchyNodeToTreeViewNodesCollection(TreeNodeCollection nodes, ISPSHierarchyNode node)
        {
            Debug.WriteLine("AddHierarchyNodeToTreeViewNodesCollection");
            Debug.WriteLine(node.OpenUrl);

            TreeNode treeNode = new TreeNode(node.Name,
                                             node.OpenUrl,
                                             node.ImageUrl,
                                             node.NavigateUrl,
                                             string.Empty)
                                {
                                        PopulateOnDemand = node.HasChilds
                                };


            nodes.Add(treeNode);
        }

        protected void PopulateNode(Object sender, TreeNodeEventArgs e)
        {
            Debug.WriteLine("Begin PopulateNode");
            Debug.WriteLine(e.Node.Value);
            AddNodes(e.Node.Value, e.Node.ChildNodes);
            Debug.WriteLine("End PopulateNode");
        }

        private void AddNodes(string url, TreeNodeCollection childNodes)
        {
            SPSHierarchyFilter dataFilter = new SPSHierarchyFilter
                                            {
                                                    SortHierarchy = true,
                                                    IncludeLists = _showLists,
                                                    IncludeWebs = _showSubSites,
                                                    IncludeFolders = _showFolders,
                                                    MaxDeepth = 9999
                                            };

            if (!string.IsNullOrEmpty(FilterWeb) || !string.IsNullOrEmpty(FilterList))
            {
                dataFilter.OnFilter += DataSourceFilter;
            }

            using(var dataSource = new SPSHierarchyODDataSource(_rootWeb, url))
            {
                dataSource.Filter = dataFilter;

                Debug.WriteLine("SPSHierarchyIterator");
                SPSHierarchyODIterator hierarchyIterator = new SPSHierarchyODIterator(dataSource);

                foreach (ISPSHierarchyNode node in hierarchyIterator)
                {
                    AddHierarchyNodeToTreeViewNodesCollection(childNodes, node);
                }
            }
        }

        private bool DataSourceFilter(object sender, SPSHierarchyFilterArgs args)
        {
            if (!string.IsNullOrEmpty(FilterWeb) && (args.Web != null))
            {
                return args.Web.Name.Contains(FilterWeb);
            }

            if (!string.IsNullOrEmpty(FilterList) && (args.List != null))
            {
                return args.List.Title.Contains(FilterList);
            }

            return true;
        }       

        /// <exception cref="System.ArgumentNullException">page is null.</exception>
        protected override void SPSRender(HtmlTextWriter writer)
        {
            if (!ShowSubSites && !ShowLists && !ShowFolders)
            {
                writer.Write(MissingConfiguration);
            }
            else
            {
                treeView.RenderControl(writer);
            }
        }
       
    }
}