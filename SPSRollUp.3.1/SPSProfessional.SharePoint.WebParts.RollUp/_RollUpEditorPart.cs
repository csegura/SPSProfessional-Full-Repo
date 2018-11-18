using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    internal class RollUpEditorPart : EditorPart
    {

        private TextBox _topSite;
        private TextBox _lists;
        private TextBox _fields;
        private TextBox _camlQuery;
        private TextBox _xsl;
        private TextBox _maxResults;
        private CheckBox _camlQueryRecursive;
        private CheckBox _dateTimeISO;
        private CheckBox _fixLookUp;
        private CheckBox _provideFirstRow;
        private CheckBox _debugResults;
        private CheckBox _debugQuery;
        private CheckBox _debugEvaluator;
        private CheckBox _includeListData;
        private CheckBox _showExtendedErrors;

        //private CheckBox _showExtendedErrors;


        public RollUpEditorPart()
        {
            ID = "SPSRollUpEditorPart";
            Title = "SPSRollUp";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            RollUp webpart = WebPartToEdit as RollUp;

            if (webpart != null)
            {
                //webpart.ClearControlState();
                webpart.TopSite = _topSite.Text;
                webpart.Lists = _lists.Text;
                webpart.Fields = _fields.Text;
                webpart.CamlQuery = _camlQuery.Text;
                webpart.Xsl = _xsl.Text;                
                webpart.CamlQueryRecursive = _camlQueryRecursive.Checked;
                webpart.IncludeListData = _includeListData.Checked;
                webpart.DateTimeISO = _dateTimeISO.Checked;
                webpart.FixLookUp = _fixLookUp.Checked;
                webpart.DebugResults = _debugResults.Checked;
                webpart.DebugQuery = _debugQuery.Checked;
                webpart.DebugEvaluator = _debugEvaluator.Checked;
                webpart.ShowExtendedErrors = _showExtendedErrors.Checked;
                webpart.ProvideFirstRow = _provideFirstRow.Checked;

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
            RollUp webpart = WebPartToEdit as RollUp;

            if (webpart != null)
            {
                _topSite.Text = webpart.TopSite;
                _lists.Text = webpart.Lists;
                _fields.Text = webpart.Fields;
                _camlQuery.Text = webpart.CamlQuery;
                _xsl.Text = webpart.Xsl;
                _maxResults.Text = webpart.MaxRecords.ToString();
                _camlQueryRecursive.Checked = webpart.CamlQueryRecursive;
                _includeListData.Checked = webpart.IncludeListData;
                _dateTimeISO.Checked = webpart.DateTimeISO;
                _fixLookUp.Checked = webpart.FixLookUp;
                _debugResults.Checked = webpart.DebugResults;
                _debugQuery.Checked = webpart.DebugQuery;
                _debugEvaluator.Checked = webpart.DebugEvaluator;
                _showExtendedErrors.Checked = webpart.ShowExtendedErrors;
                _provideFirstRow.Checked = webpart.ProvideFirstRow;
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

            _fields = new TextBox();
            _fields.Text = string.Empty;
            _fields.ID = "c3";
            Controls.Add(_fields);

            _camlQuery = new TextBox();
            _camlQuery.Text = string.Empty;
            _camlQuery.ID = "c4";
            Controls.Add(_camlQuery);

            _xsl = new TextBox();
            _xsl.Text = string.Empty;
            _xsl.ID = "c5";
            Controls.Add(_xsl);

            _maxResults = new TextBox();
            _maxResults.Text = string.Empty;
            _maxResults.ID = "c6";
            Controls.Add(_maxResults);

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


            _provideFirstRow = new CheckBox();
            _provideFirstRow.Text = SPSResources.GetString("SPSEP_ProvideFirst");
            _provideFirstRow.Checked = false;
            Controls.Add(_provideFirstRow);

            _debugResults = new CheckBox();
            _debugResults.Text = SPSResources.GetString("SPSEP_DebugXMLResults");
            _debugResults.Checked = false;
            Controls.Add(_debugResults);

            _debugQuery = new CheckBox();
            _debugQuery.Text =  SPSResources.GetString("SPSEP_DebugCAMLQuery");
            _debugQuery.Checked = false;
            Controls.Add(_debugQuery);

            _debugEvaluator = new CheckBox();
            _debugEvaluator.Text =  SPSResources.GetString("SPSEP_DebugEvaluator");
            _debugEvaluator.Checked = false;
            Controls.Add(_debugEvaluator);

            _showExtendedErrors = new CheckBox();
            _showExtendedErrors.Text =  SPSResources.GetString("SPSEP_ShowDeveloperErrors");
            _showExtendedErrors.Checked = false;
            Controls.Add(_showExtendedErrors);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools partsTools = new SPSEditorPartsTools(writer);

            partsTools.DecorateControls(Controls);
            partsTools.SectionBeginTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_TopSite"));
            partsTools.CreateTextBoxAndBuilder(_topSite);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_Lists"));
            partsTools.CreateTextBoxAndBuilder(_lists);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_Fields"));
            partsTools.CreateTextBoxAndBuilder(_fields);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_CAMLQuery"));
            partsTools.CreateTextBoxAndBuilderXml(_camlQuery);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_XSL"));
            partsTools.CreateTextBoxAndBuilderXml(_xsl);
            partsTools.SectionFooterTag();

            partsTools.SectionHeaderTag( SPSResources.GetString("SPSEP_MaxResults"));
            partsTools.CreateTextBoxAndBuilderXml(_maxResults);
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
            _provideFirstRow.RenderControl(writer);
            partsTools.SectionFooterTag();   

            partsTools.SectionHeaderTag();
            _debugResults.RenderControl(writer);
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