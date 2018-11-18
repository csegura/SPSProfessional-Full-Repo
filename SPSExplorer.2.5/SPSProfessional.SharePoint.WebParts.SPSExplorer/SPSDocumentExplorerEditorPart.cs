using System;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    internal class SPSDocumentExplorerEditorPart : SPSListExplorerEditorPart
    {

        private CheckBox chkShowUploadButton;

        //TODO
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ID = "SPSDocumentExplorerParamsEditor";
            Title = "SPSDocumentExplorer";
            _documentLibrary = true;
        }

        public override bool ApplyChanges()
        {            
            EnsureChildControls();
            SPSDocumentExplorer webpart = WebPartToEdit as SPSDocumentExplorer;

            if (webpart != null)
            {
                webpart.ShowUploadButton = chkShowUploadButton.Checked;
                webpart.ClearControlState();

                return base.ApplyChanges();
            }
            return false;
        }

        public override void SyncChanges()
        {            
            EnsureChildControls();
            SPSDocumentExplorer webpart = WebPartToEdit as SPSDocumentExplorer;

            if (webpart != null)
            {
                if (!string.IsNullOrEmpty(webpart.ListGuid))
                {
                    chkShowUploadButton.Checked = webpart.ShowUploadButton;
                }                
            }
            base.SyncChanges();
        }

        protected override void CreateChildControls()
        {         
            base.CreateChildControls();
            chkShowUploadButton = new CheckBox();
            chkShowUploadButton.Text = SPSResources.GetResourceString("SPSPE_ShowUploadButton");
            Controls.Add(chkShowUploadButton);
        }
       
        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            chkShowUploadButton.RenderControl(writer);            
        }
    }
}
