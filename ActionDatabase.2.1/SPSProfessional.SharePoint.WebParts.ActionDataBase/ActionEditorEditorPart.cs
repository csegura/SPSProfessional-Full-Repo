using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionEditorEditorPart : EditorPart
    {
        private TextBox txtConfig;
        private CheckBox chkDevErrors;

        public ActionEditorEditorPart()
        {
            ID = "ActionEditorEditorPart";
            Title = "ActionDataBaseEditor";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ActionEditor webpart = WebPartToEdit as ActionEditor;

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
            ActionEditor webpart = WebPartToEdit as ActionEditor;

            if (webpart != null)
            {
                txtConfig.Text = webpart.XmlConfig;
                chkDevErrors.Checked = webpart.ShowExtendedErrors;
            }
        }

        protected override void CreateChildControls()
        {
            txtConfig = new TextBox
                        {
                                Width = new Unit("176px"),
                                Text = string.Empty,
                                ID = "c1"
                        };
            Controls.Add(txtConfig);

            chkDevErrors = new CheckBox
                           {
                                   Text = "Show Developer Errors",
                                   Checked = false
                           };
            Controls.Add(chkDevErrors);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools partsTools = new SPSEditorPartsTools(writer);

            partsTools.SectionBeginTag();

            partsTools.SectionHeaderTag("Configuration:");
            partsTools.CreateTextBoxAndBuilderXml(txtConfig);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            chkDevErrors.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionEndTag();
        }
    }
}