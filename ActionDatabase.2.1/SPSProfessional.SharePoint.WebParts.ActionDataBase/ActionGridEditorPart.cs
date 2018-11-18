using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionGridEditorPart : EditorPart
    {
        private TextBox txtConfig;
        private CheckBox chkDevErrors;

        public ActionGridEditorPart()
        {
            Title = "ActionDataBaseGrid";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ActionGrid webpart = WebPartToEdit as ActionGrid;

            if (webpart != null)
            {
                webpart.ClearControlState();
                webpart.XmlConfig = txtConfig.Text;
                webpart.ShowExtendedErrors = chkDevErrors.Checked;
            }

            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            ActionGrid webpart = WebPartToEdit as ActionGrid;

            if (webpart != null)
            {
                txtConfig.Text = webpart.XmlConfig;
                chkDevErrors.Checked = webpart.ShowExtendedErrors;
            }
        }

        protected override void CreateChildControls()
        {
            txtConfig = new TextBox();
            txtConfig.Width = new Unit("176px");
            txtConfig.Text = string.Empty;
            txtConfig.ID = "c1" + ID;
            Controls.Add(txtConfig);

            chkDevErrors = new CheckBox();
            chkDevErrors.Text = "Show Developer Errors";
            chkDevErrors.Checked = false;
            chkDevErrors.ID = "c2" + ID;
            Controls.Add(chkDevErrors);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools tools = new SPSEditorPartsTools(writer);

            tools.SectionBeginTag();

            tools.SectionHeaderTag("Configuration:");
            tools.CreateTextBoxAndBuilderXml(txtConfig);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkDevErrors.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionEndTag();
        }
    }
}