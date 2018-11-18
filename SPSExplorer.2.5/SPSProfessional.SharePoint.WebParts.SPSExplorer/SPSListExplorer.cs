using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.Framework.WebPartCache;
using SPSProfessional.SharePoint.WebParts.SPSExplorer.Controls;


namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    [DefaultProperty("CategoryFilter")]
    [ToolboxData("<{0}:SPSListExplorer runat=server></{0}:SPSListExplorer>")]
    [XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.SPSExplorer")]
    public class SPSListExplorer : SPSWebPart, IWebPartCache
    {
        internal string _listViewGuid = string.Empty;
        internal string _listGuid = string.Empty;

        private bool _showTree;
        private bool _sortHierarchyTree;
        private bool _showBreadCrumb;
        private bool _showNewButton;
        private bool _showUpButton;
        private bool _showNumberOfItems;
        private bool _showActionsButton;

        private SPSListView _listViewWP;
        internal BreadCrumbControl _breadCrumb;
        private SPSErrorBoxControl _errorBox;
        private FolderExplorerControl _folderExplorer;
        internal ViewToolBar _toolbar;
        SPLinkButton _buttonUpFolder;

        #region WebPart Properties

        [Personalizable(PersonalizationScope.Shared)]
        public string ListGuid
        {
            get { return _listGuid; }
            set { _listGuid = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string ListViewGuid
        {
            get { return _listViewGuid; }
            set { _listViewGuid = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(true)]
        public bool ShowNewButton
        {
            get { return _showNewButton; }
            set { _showNewButton = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(true)]
        public bool ShowActionsButton
        {
            get { return _showActionsButton; }
            set { _showActionsButton = value; }
        }
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(true)]
        public bool ShowUpButton
        {
            get { return _showUpButton; }
            set { _showUpButton = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(false)]
        public bool ShowNumberOfItems
        {
            get { return _showNumberOfItems; }
            set { _showNumberOfItems = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(true)]
        public bool ShowTree
        {
            get { return _showTree; }
            set { _showTree = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(true)]
        public bool SortHierarchyTree
        {
            get { return _sortHierarchyTree; }
            set { _sortHierarchyTree = value; }
        }


        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(false)]
        public bool ShowBreadCrumb
        {
            get { return _showBreadCrumb; }
            set { _showBreadCrumb = value; }
        }

        #endregion

        public SPSListExplorer()
        {
            SPSInit("8A297689-7031-444C-B1E9-D52FF62EB20D",
                    "SPSExplorer.2.0",
                    "SPSListExplorer WebPart",
                    "http://www.spsprofessional.com/");

            EditorParts.Add(new SPSListExplorerEditorPart());
        }

        #region Control

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ExportMode = WebPartExportMode.All;
        }

        protected override void CreateChildControls()
        {
            _errorBox = new SPSErrorBoxControl
            {
                ShowExtendedErrors = true
            };

            Controls.Add(_errorBox);

            try
            {
                CreateChildControlsInternal();
            }
            catch (Exception ex)
            {
                _errorBox.AddError(new SPSErrorArgs(ex));
            }
        }

        private void CreateChildControlsInternal()
        {
            if (CheckParameters())
            {
                if (_showTree)
                {
                    _folderExplorer = new FolderExplorerControl(_listGuid, _listViewGuid)
                    {
                        ExpandAll = true,
                        FollowListView = true,
                        ID = (ID + "fe"),
                        AutoCollapse = false,
                        ShowCounter = _showNumberOfItems,
                        SortHierarchy = _sortHierarchyTree,
                        CacheService = GetCacheService()
                    };

                    Controls.Add(_folderExplorer);
                }

                // we are using this control to check RootFolder and generate
                // upload and goback buttons
                // we can hide it but we need create this always
                _breadCrumb = new BreadCrumbControl(_listGuid, _listViewGuid)
                {
                    MaxLevels = 3
                };

                Controls.Add(_breadCrumb);

                _listViewWP = new SPSListView
                {
                    ListId = _listGuid,
                    ViewId = _listViewGuid,
                    EnableViewState = true,                    
                };
                
                SPWeb web = SPContext.Current.Web;
                SPList list = web.Lists[new Guid(_listGuid)];
                SPView view = list.Views[new Guid(_listViewGuid)];

                SPContext context = SPContext.GetContext(Context, view.ID, list.ID, web);

                _toolbar = new ViewToolBar
                           {
                                   RenderContext = context
                           };

                CustomButtons();
                               
               // Up Folder button
                    
               _buttonUpFolder = new SPLinkButton
                                         {
                                                 Text = SPSResources.GetResourceString("SPS_UpFolderButton"),
                                                 ImageUrl = "/_layouts/images/UPFOLDER.GIF",
                                                 HoverCellActiveCssClass = "ms-buttonactivehover",
                                                 HoverCellInActiveCssClass = "ms-buttoninactivehover",
                                                 NavigateUrl = "#",
                                                 EnableViewState = false
                                         };

               
                              
               Controls.Add(_toolbar);                
               Controls.Add(_listViewWP);
            }
        }

        internal virtual void CustomButtons()
        {            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (CheckParameters())
            {
                _listViewWP.RootFolder = _breadCrumb.GetCurrentFolder();

                if (!_showNewButton)
                {
                    HiddeButton(_toolbar,typeof(NewMenu));
                }

                if (!_showActionsButton)
                {
                    HiddeButton(_toolbar,typeof(ActionsMenu));
                }

                HiddeRightButtons(_toolbar);

                InsertButtonAfter(_toolbar,typeof(NewMenu), _buttonUpFolder);

                if (_showUpButton && !_breadCrumb.IsRootFolder)
                {
                    _buttonUpFolder.OnClientClick = _breadCrumb.GetLinkToParentView();
                }
                else
                {
                    _buttonUpFolder.Visible = false;
                }
            }
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            try
            {
                if (!_errorBox.HasErrors)
                {
                    if (CheckParameters())
                    {
                        writer.WriteLine("<div><div style='float:left{0}'>", _showTree ? string.Empty : ";display=none");

                        if (_showTree)
                        {
                            _folderExplorer.RenderControl(writer);
                        }

                        writer.WriteLine("</div><div style='float:right;padding-left:{0}'>", _showTree ? "5px" : string.Empty);

                        _toolbar.RenderControl(writer);

                        if (_showBreadCrumb)
                        {
                            _breadCrumb.RenderControl(writer);
                        }

                        writer.WriteLine("<!-- ListView -->");
                        _listViewWP.RenderControl(writer);
                        writer.WriteLine("<!-- End -- ListView -->");

                        writer.WriteLine("</div></div>");
                    }
                    else
                    {
                        writer.Write(MissingConfiguration);
                    }
                }

                _errorBox.RenderControl(writer);
            }
            catch (Exception ex)
            {
                writer.Write(ex);
            }
        }
       

        #endregion

        /// <summary>
        /// Checks the parameters.
        /// </summary>
        /// <returns></returns>
        internal bool CheckParameters()
        {
            return !string.IsNullOrEmpty(_listGuid)
                   && !string.IsNullOrEmpty(_listViewGuid);
        }

        #region Implementation of IWebPartCache

        public SPSCacheService GetCacheService()
        {
            return CacheService;
        }

        #endregion

        internal void InsertButtonAfter(Control viewToolBar, Type menuType, Control control)
        {
            Control target = viewToolBar.Controls[0].Controls[1].Controls[0];
            
            int controlPosition = 1;

            foreach (Control ctrl in target.Controls)
            {
                if (ctrl.GetType() == menuType)
                {
                    break;
                }
                controlPosition++;
            }

            target.Controls.AddAt(controlPosition,control);
        }

        internal void HiddeButton(Control viewToolBar, Type menuType)
        {
            Control target = viewToolBar.Controls[0].Controls[1].Controls[0];

            foreach (Control ctrl in target.Controls)
            {
                if (ctrl.GetType() == menuType)
                {
                    ctrl.Visible = false;
                }
            }
        }

        internal void HiddeRightButtons(Control viewToolBar)
        {
            Control target = viewToolBar.Controls[0].Controls[1].Controls[1];
            target.Visible = false;            
        }
    }
}
