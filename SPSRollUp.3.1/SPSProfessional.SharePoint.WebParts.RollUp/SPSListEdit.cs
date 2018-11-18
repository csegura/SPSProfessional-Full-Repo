using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using SPSProfessional.SharePoint.Framework.Controls;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    [DefaultProperty("CategoryFilter")]
    [ToolboxData("<{0}:SPSListEdit runat=server></{0}:SPSListEdit>")]
    [XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.SPSListEdit")]
    public class SPSListEdit : SPSWebPart
    {
        
        private ListFormWebPart _listEditWebPart;

        public SPSListEdit()
        {
            SPSInit("9DA7AF31-81EB-4dbe-AF29-026FABB97223",
                    "SPSRollUp.1.0",
                    "RollUp WebPart",
                    "http://www.spsprofessional.com/page/SPSRollUp-WebPart.aspx");

            ExportMode = WebPartExportMode.All;
            EditorParts.Add(new RollUpEditorPart());
        }

        protected override void CreateChildControls()
        {
            _listEditWebPart = new ListFormWebPart();
            _listEditWebPart.HideIfNoPermissions = true;
            _listEditWebPart.ListItemId = 1;
            _listEditWebPart.ListName = "{43137E6B-D1B3-4144-884B-D87C7080CEFC}";
            //_listEditWebPart.ItemContext = SPContext.Current;
            //_listEditWebPart.Page = this.Page;
            _listEditWebPart.ControlMode = SPControlMode.Edit;
            _listEditWebPart.FormType = 8;
            //_listEditWebPart.TemplateName = "ListForm";
            Controls.Add(_listEditWebPart);

            //WebPartManager manager = new WebPartManager();
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            try
            {
                _listEditWebPart.RenderControl(writer);
            }
            catch(Exception ex)
            {
                writer.Write(ex);
            }
        }
    }
}
