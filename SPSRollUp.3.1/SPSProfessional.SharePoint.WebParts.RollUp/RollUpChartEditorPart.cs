using System.Web.UI;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    internal class RollUpChartEditorPart : RollUpEditorPartBase
    {

        private CheckBox _debugResults;
        private CheckBox _debugResultsXML;
        private CheckBox _debugQuery;
        private CheckBox _debugEvaluator;
        private CheckBox _includeListData;
        private DropDownList _chartType;
        private TextBox _width;
        private TextBox _height;

        //private CheckBox _showExtendedErrors;

        public RollUpChartEditorPart()
        {
            ID = "SPSRollUpChartEditorPart";
            Title = "SPSRollUpChart";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            RollUpChart webpart = WebPartToEdit as RollUpChart;

            if (webpart != null)
            {
                webpart.ClearControlState();
                webpart.TopSite = _topSite.Text;
                webpart.Lists = _lists.Text;
                webpart.Fields = _fields.Text;
                webpart.CamlQuery = _camlQuery.Text;
                webpart.SortByFields = _sortFields.Text;
                webpart.Xsl = _xsl.Text;
                webpart.CamlQueryRecursive = _camlQueryRecursive.Checked;
                webpart.IncludeListData = _includeListData.Checked;
                webpart.DebugResults = _debugResults.Checked;
                webpart.DateTimeISO = _dateTimeISO.Checked;
                webpart.FixLookUp = _fixLookUp.Checked;
                webpart.DebugResultsXML = _debugResultsXML.Checked;
                webpart.DebugQuery = _debugQuery.Checked;
                webpart.DebugEvaluator = _debugEvaluator.Checked;
                webpart.ShowExtendedErrors = _showExtendedErrors.Checked;
                webpart.GraphWidth = _width.Text;
                webpart.GraphHeight = _height.Text;
                webpart.GraphType = _chartType.SelectedValue;

                int maxRecords;
                if (int.TryParse(_maxResults.Text, out maxRecords))
                {
                    webpart.MaxRecords = maxRecords;
                }
                webpart.ClearCache();
            }

            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            RollUpChart webpart = WebPartToEdit as RollUpChart;

            if (webpart != null)
            {
                _topSite.Text = webpart.TopSite;
                _lists.Text = webpart.Lists;
                _fields.Text = webpart.Fields;
                _camlQuery.Text = webpart.CamlQuery;
                _sortFields.Text = webpart.SortByFields;
                _xsl.Text = webpart.Xsl;
                _maxResults.Text = webpart.MaxRecords.ToString();
                _camlQueryRecursive.Checked = webpart.CamlQueryRecursive;
                _includeListData.Checked = webpart.IncludeListData;
                _dateTimeISO.Checked = webpart.DateTimeISO;
                _fixLookUp.Checked = webpart.FixLookUp;
                _debugResults.Checked = webpart.DebugResults;
                _debugResultsXML.Checked = webpart.DebugResultsXML;
                _debugQuery.Checked = webpart.DebugQuery;
                _debugEvaluator.Checked = webpart.DebugEvaluator;
                _showExtendedErrors.Checked = webpart.ShowExtendedErrors;
                _width.Text = webpart.GraphWidth;
                _height.Text = webpart.GraphHeight;
                _chartType.SelectedValue = webpart.GraphType;

                webpart.ResetCamlPreprocessor();
                webpart.InitializeEngine();
            }
        }

        protected override void CreateChildControls()
        {
            _topSite = new TextBox();
            _topSite.Text = string.Empty;
            _topSite.ID = "c1";
            Controls.Add(_topSite);

            _lists = new TextBox();
            _lists.Text = string.Empty;
            _lists.ID = "c2";
            Controls.Add(_lists);

            _btnListInfo = new LinkButton();
            _btnListInfo.Text = SPSResources.GetString("SPSEP_ListInfo");
            _btnListInfo.ID = "b1";
            _btnListInfo.Attributes.Add("OnClick", "javascript:ListInfo();return false;");
            _btnListInfo.PostBackUrl = "javascript:void(0);";
            Controls.Add(_btnListInfo);

            _fields = new TextBox();
            _fields.Text = string.Empty;
            _fields.ID = "c3";
            Controls.Add(_fields);

            _btnFieldInfo = new LinkButton();
            _btnFieldInfo.Text = SPSResources.GetString("SPSEP_FieldInfo");
            _btnFieldInfo.ID = "b2";
            _btnFieldInfo.Attributes.Add("OnClick", "javascript:FieldInfo();return false;");
            _btnFieldInfo.PostBackUrl = "javascript:void(0);";
            Controls.Add(_btnFieldInfo);

            _camlQuery = new TextBox();
            _camlQuery.Text = string.Empty;
            _camlQuery.ID = "c4";
            Controls.Add(_camlQuery);

            _sortFields = new TextBox();
            _sortFields.Text = string.Empty;
            _sortFields.ID = "c5";
            Controls.Add(_sortFields);

            _xsl = new TextBox();
            _xsl.Text = string.Empty;
            _xsl.ID = "c6";
            Controls.Add(_xsl);

            _maxResults = new TextBox();
            _maxResults.Text = string.Empty;
            _maxResults.ID = "c6a";
            Controls.Add(_maxResults);

            _width = new TextBox();
            _width.Text = string.Empty;
            _width.ID = "c7";
            Controls.Add(_width);

            _height = new TextBox();
            _height.Text = string.Empty;
            _height.ID = "c8";
            Controls.Add(_height);

            _chartType = new DropDownList();
            _chartType.Items.Add("Line");
            _chartType.Items.Add("Bar2D");
            _chartType.Items.Add("Area2D");
            _chartType.Items.Add("Column2D");
            _chartType.Items.Add("Column3D");            
            _chartType.Items.Add("Pie2D");
            _chartType.Items.Add("Pie3D");
            _chartType.Items.Add("MSColumn3D");
            _chartType.Items.Add("MSColumn3DLineDY");
            _chartType.Items.Add("StackedColumn3D");
            _chartType.ID = "c9";
            Controls.Add(_chartType);

            _camlQueryRecursive = new CheckBox();
            _camlQueryRecursive.Text = SPSResources.GetString("SPSEP_CamlQueryRecursive");
            _camlQueryRecursive.Checked = false;
            Controls.Add(_camlQueryRecursive);

            _includeListData = new CheckBox();
            _includeListData.Text = SPSResources.GetString("SPSEP_IncludeListData");
            _includeListData.Checked = false;
            Controls.Add(_includeListData);

            _dateTimeISO = new CheckBox();
            _dateTimeISO.Text = SPSResources.GetString("SPSEP_DateTimeInISO");
            _dateTimeISO.Checked = false;
            Controls.Add(_dateTimeISO);

            _fixLookUp = new CheckBox();
            _fixLookUp.Text = SPSResources.GetString("SPSEP_LookUpFix");
            _fixLookUp.Checked = false;
            Controls.Add(_fixLookUp);

            _debugResults = new CheckBox();
            _debugResults.Text = SPSResources.GetString("SPSEP_DebugXMLResults");
            _debugResults.Checked = false;
            Controls.Add(_debugResults);

            _debugResultsXML = new CheckBox();
            _debugResultsXML.Text = SPSResources.GetString("SPSEP_DebugXMLChart");
            _debugResultsXML.Checked = false;
            Controls.Add(_debugResultsXML);

            _debugQuery = new CheckBox();
            _debugQuery.Text = SPSResources.GetString("SPSEP_DebugCAMLQuery");
            _debugQuery.Checked = false;
            Controls.Add(_debugQuery);

            _debugEvaluator = new CheckBox();
            _debugEvaluator.Text = SPSResources.GetString("SPSEP_DebugEvaluator");
            _debugEvaluator.Checked = false;
            Controls.Add(_debugEvaluator);

            _showExtendedErrors = new CheckBox();
            _showExtendedErrors.Text = SPSResources.GetString("SPSEP_ShowDeveloperErrors");
            _showExtendedErrors.Checked = false;
            Controls.Add(_showExtendedErrors);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools partsTools = new SPSEditorPartsTools(writer);

            partsTools.DecorateControls(Controls);
            partsTools.SectionBeginTag();

            partsTools.SectionHeaderTag(SPSResources.GetString("SPSEP_TopSite"));
            partsTools.CreateTextBoxAndBuilder(_topSite);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag(SPSResources.GetString("SPSEP_Lists"));
            partsTools.CreateTextBoxAndBuilder(_lists);
            partsTools.SectionNextLineTag();
            _btnListInfo.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag(SPSResources.GetString("SPSEP_Fields"));
            partsTools.CreateTextBoxAndBuilder(_fields);
            partsTools.SectionNextLineTag();
            _btnFieldInfo.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_CAMLQuery"));
            partsTools.CreateTextBoxAndBuilderXml(_camlQuery);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag(SPSResources.GetString("SPSEP_OrderFields"));
            partsTools.CreateTextBoxAndBuilder(_sortFields);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_XSL"));
            partsTools.CreateTextBoxAndBuilderXml(_xsl);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_MaxResults"));
            partsTools.CreateTextBoxAndBuilderXml(_maxResults);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag(SPSResources.GetString("SPSEP_ChartType"));
            _chartType.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag(SPSResources.GetString("SPSEP_ChartWidth"));
            _width.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag(SPSResources.GetString("SPSEP_ChartHeight"));
            _height.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _camlQueryRecursive.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _includeListData.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _dateTimeISO.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _fixLookUp.RenderControl(writer);
            partsTools.SectionFooterTag();  

            partsTools.SectionHeaderTag();
            _debugResults.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _debugResultsXML.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _debugQuery.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _debugEvaluator.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag();
            _showExtendedErrors.RenderControl(writer);
            partsTools.SectionFooterTag();

            partsTools.SectionEndTag();
        }
    }
}