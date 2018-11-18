using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using SPSProfessional.SharePoint.Framework.Controls;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    [DefaultProperty("CategoryFilter")]
    [Category("SPSProfessional WebParts")]
    [ToolboxData("<{0}:RollUpCalendar runat=server></{0}:RollUpCalendar>")]
    [XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.RollUpCalendar")]
    public class RollUpCalendar : RollUpBase3
    {
        private SPSXsltCalendarControl _calendar;

        #region Constructor

        public RollUpCalendar()
        {
            SPSInit("9DA7AF31-81EB-4dbe-AF29-026FABB97223",
                    "SPSRollUp.2.0",
                    "RollUp WebPart",
                    "http://www.spsprofessional.com/page/SPSRollUp-WebPart.aspx");

            ExportMode = WebPartExportMode.All;
            EditorParts.Add(new RollUpCalendarEditorPart());

            _topSite = string.Empty;
            _lists = string.Empty;
            _fields = string.Empty;
            _camlQuery = string.Empty;
            _xsl = string.Empty;
            _dateTimeISO = false;
            _fixLookUp = false;
            XslPage = 1;
            _errorBox = new SPSErrorControl();
        }

        #endregion

        #region Control Methods

        protected override void OnLoad(EventArgs e)
        {
            const string scriptName = "SPSProfessional_SPSCalendarControl";
            ClientScriptManager clientScript = Page.ClientScript;
            if (!clientScript.IsClientScriptBlockRegistered(scriptName))
            {
                const string csstext = "<link href=\"_layouts/SPSRollUp/SPSCalendar.css\" " +
                                       "type=\"text/css\" rel=\"stylesheet\"></link>";

                clientScript.RegisterClientScriptBlock(
                        GetType(),
                        scriptName,
                        csstext,
                        false);
            }

            base.OnLoad(e);
        }


        protected override void CreateChildControls()
        {
            _calendar = new SPSXsltCalendarControl
                            {
                                    Xsl = Xsl,
                                    DebugSource = DebugResultsXML,
                                    DebugTransformation = DebugResults
                            };
            _calendar.OnError += TrapSubsystemError;
            Controls.Add(_calendar);
            
            Controls.Add(_errorBox);

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            _calendar.XmlData = _collector.GetXml();
            _calendar.XslOrder = XslOrder;
            _calendar.XslPage = XslPage;
            _calendar.XslSelectedRow = XslSelectedRow;
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {

            Debug.WriteLine("RollUp: Render " + Title);

            if (!ValidProperties())
            {
                writer.WriteLine(MissingConfiguration);
            }
            else
            {
                EnsureChildControls();

                DebugRender(writer);
                _calendar.RenderControl(writer);

                _errorBox.ShowExtendedErrors = ShowExtendedErrors;
                _errorBox.RenderControl(writer);
            }
        }

        #endregion

        private bool ValidProperties()
        {
            return Lists.Length > 0 && Fields.Length > 0;
        }
    }
}