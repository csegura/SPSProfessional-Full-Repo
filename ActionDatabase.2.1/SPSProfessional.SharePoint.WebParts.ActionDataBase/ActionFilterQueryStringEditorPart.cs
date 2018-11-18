using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionFilterQueryStringEditorPart : EditorPart
    {
        private TextBox txtQueryParameters;
        private TextBox txtDefaultParameters;

        public ActionFilterQueryStringEditorPart()
        {
            Title = "ActionFilterQueryString";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ActionFilterQueryString webpart = WebPartToEdit as ActionFilterQueryString;

            if (webpart != null)
            {
                webpart.ClearControlState();
                webpart.QueryParameters = txtQueryParameters.Text;
                webpart.DefaultParameters = txtDefaultParameters.Text;
            }

            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            ActionFilterQueryString webpart = WebPartToEdit as ActionFilterQueryString;

            if (webpart != null)
            {
                txtQueryParameters.Text = webpart.QueryParameters;
                txtDefaultParameters.Text = webpart.DefaultParameters;
            }
        }

        protected override void CreateChildControls()
        {
            txtQueryParameters = new TextBox();
            //txtQueryParameters.Width = new Unit("176px");
            txtQueryParameters.Text = string.Empty;
            txtQueryParameters.ID = "c1" + ID;
            Controls.Add(txtQueryParameters);

            txtDefaultParameters = new TextBox();
            //txtDefaultParameters.Width = new Unit("176px");
            txtDefaultParameters.Text = string.Empty;
            txtDefaultParameters.ID = "c2" + ID;
            Controls.Add(txtDefaultParameters);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools tools = new SPSEditorPartsTools(writer);

            tools.SectionBeginTag();
            tools.DecorateControls(Controls);

            tools.SectionHeaderTag("Variable Names:");
            txtQueryParameters.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag("Default Values:");
            txtDefaultParameters.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionEndTag();
        }
    }
}