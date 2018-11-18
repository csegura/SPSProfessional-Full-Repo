using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using SPSProfessional.SharePoint.Framework.Controls;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    [DefaultProperty(""),
     ToolboxData("<{0}:RollUpChart runat=server></{0}:RollUpChart>"),
     XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.RollUpChart")]
    public class RollUpChart : RollUpBase3 //, IPostBackEventHandler
    {
        private string _graphWidth = "200";
        private string _graphHeight = "150";
        private string _graphType = "Column2D";

        private SPSXsltChartControl _xsltChartControl;

        #region Constructor

        public RollUpChart()
        {
            SPSInit("9DA7AF31-81EB-4dbe-AF29-026FABB97224",
                    "SPSRollUpChart.2.0",
                    "RollUpChart WebPart",
                    "http://www.spsprofessional.com/page/SPSRollUp-WebPart.aspx");

            ExportMode = WebPartExportMode.All;
            EditorParts.Add(new RollUpChartEditorPart());

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

        #region WebPart Properties

        [Personalizable(PersonalizationScope.Shared)]
        public string GraphWidth
        {
            get { return _graphWidth; }
            set { _graphWidth = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string GraphHeight
        {
            get { return _graphHeight; }
            set { _graphHeight = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string GraphType
        {
            get { return _graphType; }
            set { _graphType = value; }
        }

        #endregion

        #region Control Methods

        protected override void CreateChildControls()
        {
            _xsltChartControl = new SPSXsltChartControl
                                    {
                                            Xsl = Xsl,
                                            GraphWidth = GraphWidth,
                                            GraphHeight = GraphHeight,
                                            GraphType = GraphType,
                                            DebugSource = DebugResults,
                                            DebugTransformation = DebugResultsXML
                                    };

            _xsltChartControl.OnError += TrapSubsystemError;
            Controls.Add(_xsltChartControl);

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            _xsltChartControl.XmlData = _collector.GetXml();
            _xsltChartControl.XslOrder = XslOrder;
            _xsltChartControl.XslPage = XslPage;
            _xsltChartControl.XslSelectedRow = XslSelectedRow;
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

               _xsltChartControl.RenderControl(writer);
               // _errorBox.ShowExtendedErrors = ShowExtendedErrors;
                _errorBox.RenderControl(writer);
            }
        }

        #endregion

        #region Engine       

        private bool ValidProperties()
        {
            return Lists.Length > 0 && Fields.Length > 0 && GraphType.Length > 0;
        }

        #endregion
       
    }
}