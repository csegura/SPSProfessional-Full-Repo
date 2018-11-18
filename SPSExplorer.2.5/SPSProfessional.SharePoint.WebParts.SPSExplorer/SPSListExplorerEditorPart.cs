using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Tools;


namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    internal class SPSListExplorerEditorPart : EditorPart
    {
        private DropDownList ddlLists;
        private DropDownList ddlViews;
        private CheckBox chkShowNewButton;
        private CheckBox chkShowActionsButton;
        private CheckBox chkShowUpButton;
        private CheckBox chkShowNumberOfItems;
        private CheckBox chkShowBreadCrumb;
        private CheckBox chkShowTree;
        private CheckBox chkSortHierarchyTree;

        internal bool _documentLibrary;

        private SPSEditorPartsTools tools;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ID = "SPSListExplorerParamsEditor";
            Title = "SPSListExplorer";
            _documentLibrary = false;
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            SPSListExplorer webpart = WebPartToEdit as SPSListExplorer;

            if (webpart != null)
            {
                webpart.ListGuid = ddlLists.SelectedValue;
                webpart.ListViewGuid = ddlViews.SelectedValue;
                webpart.ShowNewButton = chkShowNewButton.Checked;
                webpart.ShowActionsButton = chkShowActionsButton.Checked;
                webpart.ShowUpButton = chkShowUpButton.Checked;
                webpart.ShowNumberOfItems = chkShowNumberOfItems.Checked;
                webpart.ShowBreadCrumb = chkShowBreadCrumb.Checked;
                webpart.ShowTree = chkShowTree.Checked;
                webpart.SortHierarchyTree = chkSortHierarchyTree.Checked;

                Debug.WriteLine("ListGuid:" + ddlLists.SelectedValue);
                Debug.WriteLine("ListViewGuid:" + ddlViews.SelectedValue);

                webpart.ClearControlState();
                webpart.ClearCache();

                return true;
            }
            return false;
        }

        
        public override void SyncChanges()
        {
            EnsureChildControls();
            SPSListExplorer webpart = WebPartToEdit as SPSListExplorer;

            if (webpart != null)
            {
                Debug.WriteLine("*ListGuid:" + ddlLists.SelectedValue);
                Debug.WriteLine("*ListViewGuid:" + ddlViews.SelectedValue);

                if (!string.IsNullOrEmpty(webpart.ListGuid))
                {
                    ddlLists.SelectedValue = webpart.ListGuid;

                    if (!string.IsNullOrEmpty(webpart.ListViewGuid))                        
                    {
                        SPSEditorPartsTools.FillListViews(ddlViews, webpart.ListGuid);
                        ddlViews.SelectedValue = webpart.ListViewGuid;
                    }

                    chkShowNewButton.Checked = webpart.ShowNewButton;
                    chkShowActionsButton.Checked = webpart.ShowActionsButton;
                    chkShowUpButton.Checked = webpart.ShowUpButton;
                    chkShowNumberOfItems.Checked = webpart.ShowNumberOfItems;
                    chkShowBreadCrumb.Checked = webpart.ShowBreadCrumb;
                    chkShowTree.Checked = webpart.ShowTree;
                    chkSortHierarchyTree.Checked = webpart.SortHierarchyTree;
                }                               
            }
        }

        protected override void CreateChildControls()
        {
            ddlLists = new DropDownList();
            ddlLists.Width = new Unit("100%");
            
            if (_documentLibrary)
            {
                SPSEditorPartsTools.FillLists(ddlLists, SPBaseType.DocumentLibrary);
            }
            else
            {
                SPSEditorPartsTools.FillListsExclude(ddlLists,SPBaseType.DocumentLibrary);
            }

            ddlLists.SelectedIndexChanged += ddlLists_SelectedIndexChanged;
            ddlLists.AutoPostBack = true;
            //NavigationTools.FillWebParts(Context, ddlLists);
            Controls.Add(ddlLists);

            ddlViews = new DropDownList();
            ddlViews.Width = new Unit("100%");
            SPSEditorPartsTools.FillListViews(ddlViews, ddlLists.SelectedValue);
            Controls.Add(ddlViews);

            chkShowTree = new CheckBox();
            chkShowTree.Text = SPSResources.GetResourceString("SPSPE_ShowTree");
            Controls.Add(chkShowTree);

            chkSortHierarchyTree = new CheckBox();
            chkSortHierarchyTree.Text = SPSResources.GetResourceString("SPSPE_SortHierarchyTree");
            Controls.Add(chkSortHierarchyTree);

            chkShowBreadCrumb = new CheckBox();
            chkShowBreadCrumb.Text = SPSResources.GetResourceString("SPSPE_ShowBreadCrumb");
            Controls.Add(chkShowBreadCrumb);

            chkShowNewButton = new CheckBox();
            chkShowNewButton.Text = SPSResources.GetResourceString("SPSPE_ShowNewButton");
            Controls.Add(chkShowNewButton);

            chkShowActionsButton = new CheckBox();
            chkShowActionsButton.Text = SPSResources.GetResourceString("SPSPE_ShowActionsButton");
            Controls.Add(chkShowActionsButton);

            chkShowUpButton = new CheckBox();
            chkShowUpButton.Text = SPSResources.GetResourceString("SPSPE_ShowUpFolderButton");
            Controls.Add(chkShowUpButton);

            chkShowNumberOfItems = new CheckBox();
            chkShowNumberOfItems.Text = SPSResources.GetResourceString("SPSPE_CountItemsInTree");
            Controls.Add(chkShowNumberOfItems);
        }

        private void ddlLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            SPSEditorPartsTools.FillListViews(ddlViews, ddlLists.SelectedValue);
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            tools = new SPSEditorPartsTools(writer);

            tools.DecorateControls(Controls);
            tools.SectionBeginTag();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_ListName"));
            ddlLists.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_ViewName"));
            ddlViews.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkShowTree.RenderControl(writer);
            chkSortHierarchyTree.RenderControl(writer);
            chkShowBreadCrumb.RenderControl(writer);
            chkShowNewButton.RenderControl(writer);
            chkShowActionsButton.RenderControl(writer);
            chkShowUpButton.RenderControl(writer);
            chkShowNumberOfItems.RenderControl(writer);        
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            tools.SectionFooterTag();
            tools.SectionEndTag();
        }

    }
}
