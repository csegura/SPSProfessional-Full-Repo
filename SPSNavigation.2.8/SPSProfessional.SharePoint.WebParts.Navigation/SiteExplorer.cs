using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.HierarchyOnDemand;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    public class SiteExplorer : SPSWebPart
    {
        internal const char SLASH = '/';

        private int _expandDepth = 99;
        private string _filterList = string.Empty;
        private string _filterWeb = string.Empty;
        private string _rootWeb = string.Empty;
        private bool _showLists;
        private bool _showSubSites;
        private bool _showFolders;

        private TreeView _treeView;

        public SiteExplorer()
        {
            SPSInit("71D70B8F-1556-4d4c-925D-342EE0EE59C0",
                    "Navigation.2.0",
                    "SiteExplorer",
                    "http://www.spsprofessional.com/page/Navigation-Web-Parts.aspx");
            EditorParts.Add(new SiteExplorerParamsEditor());
            ShowSubSites = true;
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
                _treeView = new TreeView
                           {
                                   ID = "treeView"
                           };

                Debug.WriteLine("Nodes");

                AddSiteCollections(_treeView.Nodes);

                Debug.WriteLine("Controls");
                SPSTreeViewHelper.DecorateTree(_treeView);
                _treeView.TreeNodePopulate += PopulateNode;
                _treeView.EnableClientScript = true;
                _treeView.PopulateNodesFromClient = true;
                _treeView.ExpandDepth = ExpandDepth;

                Controls.Add(_treeView);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ErrorMessage += ex.Message;
            }

            Debug.WriteLine("End CreateChildControls");
        }

        private void AddSiteCollections(TreeNodeCollection childNodes)
        {
            Debug.WriteLine("AddSiteCollections");

            var serverUri = new Uri(_rootWeb);

            Debug.WriteLine("RootUrl:" + _rootWeb);

            SPWebApplication webApplication = SPWebApplication.Lookup(serverUri);


            foreach (SPSite siteCollection in webApplication.Sites)
            {
                Debug.WriteLine("** SiteUrl:" + siteCollection.Url);
                Debug.WriteLine("** PortalName:" + siteCollection.PortalName);
                
                try
                {
                    AddSiteNodeToTreeViewNodesCollection(childNodes, siteCollection);    
                }
                catch(Exception ex)
                {
                    Debug.Write(ex);
                }
                

                // AddNodes(siteCollection.Url,node.ChildNodes);
                siteCollection.Dispose();
            }
        }

        public TreeNode AddSiteNodeToTreeViewNodesCollection(TreeNodeCollection nodes, SPSite siteCollection)
        {
            Debug.WriteLine("AddSiteNodeToTreeViewNodesCollection");

            if (nodes.Count > 0)
            Debug.WriteLine("Root: "+nodes[0].Text);
            else
            {
                Debug.WriteLine("Root: (Empty)");
            }

            ISPSHierarchyNode node = new SPSHierarchyNode(SPSHierarchyNodeType.Web,
                                                            siteCollection.ID,
                                                            siteCollection.RootWeb.ID,
                                                            siteCollection.RootWeb.Title,
                                                            siteCollection.Url,
                                                            siteCollection.Url,
                                                            siteCollection.Url,
                                                            siteCollection.Url,
                                                            "/_layouts/images/"
                                                            +
                                                            (string)
                                                            SPUtility.MapWebToIcon(siteCollection.RootWeb).First,
                                                            siteCollection.RootWeb.Webs.Count > 0);

            Debug.WriteLine("Node: " + node.Name);

            var treeNode = new TreeNode(node.Name,
                                             node.OpenUrl,
                                             node.ImageUrl,
                                             node.NavigateUrl,
                                             string.Empty)
                                    {
                                            PopulateOnDemand = true
                                    };

            nodes.Add(treeNode);

            return treeNode;
        }

        private void AddHierarchyNodeToTreeViewNodesCollection(TreeNodeCollection nodes, ISPSHierarchyNode node)
        {
            Debug.WriteLine("AddHierarchyNodeToTreeViewNodesCollection");
            Debug.WriteLine(node.NavigateUrl);

            var treeNode = new TreeNode(node.Name,
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
            Debug.WriteLine("* AddNodes "+url);

            if (url.Contains("|") && !url.EndsWith("||"))
            {
                return;
            }

            var dataFilter = new SPSHierarchyFilter
                                            {
                                                    SortHierarchy = true,
                                                    IncludeLists = _showLists,
                                                    IncludeWebs = true,
                                                    IncludeFolders = false,
                                                    MaxDeepth = 9999
                                            };

            if (!string.IsNullOrEmpty(FilterWeb) || !string.IsNullOrEmpty(FilterList))
            {
                dataFilter.OnFilter += DataSourceFilter;
            }
            
            
            Debug.WriteLine("* DS " + url);
            
            using(var dataSource = new SPSHierarchyODDataSource(url))
            {
                dataSource.Filter = dataFilter;

                Debug.WriteLine("SPSHierarchyIterator");
                var hierarchyIterator = new SPSHierarchyODIterator(dataSource);

                foreach (ISPSHierarchyNode node in hierarchyIterator)
                {
                    Debug.WriteLine("* "+node.NavigateUrl);
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
            if (string.IsNullOrEmpty(RootWeb))
            {
                writer.Write(MissingConfiguration);
            }
            else
            {
                _treeView.RenderControl(writer);
            }
        }
       
    }
}