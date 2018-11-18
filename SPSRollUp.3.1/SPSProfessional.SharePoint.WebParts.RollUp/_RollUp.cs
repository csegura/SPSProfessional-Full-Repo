using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using SPSProfessional.SharePoint.Framework.Controls;


namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    [DefaultProperty("CategoryFilter"),
     ToolboxData("<{0}:RollUp runat=server></{0}:RollUp>"),
     XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.RollUp")]
    public class RollUp : RollUpBase
    {
        private SPSXsltControl _xsltControl;

        #region Constructor

        public RollUp()
        {
            SPSInit("9DA7AF31-81EB-4dbe-AF29-026FABB97223",
                    "SPSRollUp.2.0",
                    "RollUp WebPart",
                    "http://www.spsprofessional.com/page/SPSRollUp-WebPart.aspx");

            ExportMode = WebPartExportMode.All;
            EditorParts.Add(new RollUpEditorPart());

            _topSite = string.Empty;
            _lists = string.Empty;
            _fields = string.Empty;
            _camlQuery = string.Empty;
            _xsl = string.Empty;
            _dateTimeISO = false;
            _fixLookUp = false;
            XslPage = 1;
            _errorBox = new SPSErrorBoxControl();
        }

        #endregion

        #region Control Methods

        protected override void CreateChildControls()
        {
            _xsltControl = new SPSXsltControl
                               {
                                       Xsl = Xsl,
                                       DebugSource = DebugResults,
                                       DebugTransformation = DebugResultsXML
                               };
            _xsltControl.OnError += TrapSubsystemError;
            Controls.Add(_xsltControl);

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            _xsltControl.XmlData = _rollupEngine.Data.GetXml();
            _xsltControl.XslOrder = XslOrder;
            _xsltControl.XslPage = XslPage;
            _xsltControl.XslSelectedRow = XslSelectedRow;
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            Debug.WriteLine("RollUp: Render " + Title);

            if (! ValidProperties())
            {
                writer.WriteLine(MissingConfiguration);
            }
            else
            {
                EnsureChildControls();
                
                DebugRender(writer);

                _xsltControl.RenderControl(writer);
                //_errorBox.ShowExtendedErrors = ShowExtendedErrors;
                _errorBox.RenderControl(writer);
            }
        }

        #endregion

        #region Engine

        private bool ValidProperties()
        {
            return Lists.Length > 0 && Fields.Length > 0;
        }

        #endregion
    
    }
}