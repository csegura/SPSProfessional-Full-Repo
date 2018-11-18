using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Comms;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    public class RollUpBase : SPSWebPart, IPostBackEventHandler, IWebPartCache
    {
        private const string CMD_Order = "Order:";
        private const string CMD_Page = "Page:";
        private const string CMD_Select = "Select:";
        private const string VS_LastRow = "LastRow";
        private const string VS_LastFilter = "LastFilter";
        private const string VS_XslOrder = "XslOrder";
        private const string VS_XslPage = "XslPage";
        private const string VS_XslSelectedRow = "XslSelectedRow";

        protected SPSErrorBoxControl _errorBox;
        protected string _topSite;
        protected string _lists;
        protected string _fields;
        protected string _camlQuery;
        protected string _xsl;
        private int _maxRecords;
        private bool _camlQueryRecursive;
        private bool _debugResults;
        private bool _debugResultsXML;
        private bool _debugQuery;
        private bool _debugEvaluator;
        private bool _includeListData;
        private bool _showExtendedErrors;
        protected bool _dateTimeISO;
        protected bool _fixLookUp;
        private CamlPreprocessor _camlPreprocessor;
        private SPSRowProvider _rowProvider;
        internal SPSRollUpEngine _rollupEngine;
        private SPSKeyValueList _lastRow;
        private SPSKeyValueList _lastFilter;
        private bool _processed;
        private bool _provideFirstRow;

        #region WebPart Properties

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowExtendedErrors
        {
            get { return _showExtendedErrors; }
            set { _showExtendedErrors = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string TopSite
        {
            get { return _topSite; }
            set { _topSite = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string Lists
        {
            get { return _lists; }
            set { _lists = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string CamlQuery
        {
            get { return _camlQuery; }
            set { _camlQuery = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public int MaxRecords
        {
            get { return _maxRecords; }
            set { _maxRecords = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool CamlQueryRecursive
        {
            get { return _camlQueryRecursive; }
            set { _camlQueryRecursive = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool DebugResults
        {
            get { return _debugResults; }
            set { _debugResults = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool DebugResultsXML
        {
            get { return _debugResultsXML; }
            set { _debugResultsXML = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool DebugQuery
        {
            get { return _debugQuery; }
            set { _debugQuery = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool DebugEvaluator
        {
            get { return _debugEvaluator; }
            set { _debugEvaluator = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool IncludeListData
        {
            get { return _includeListData; }
            set { _includeListData = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string Xsl
        {
            get { return _xsl; }
            set { _xsl = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool DateTimeISO
        {
            get { return _dateTimeISO; }
            set { _dateTimeISO = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool FixLookUp
        {
            get { return _fixLookUp; }
            set { _fixLookUp = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ProvideFirstRow
        {
            get { return _provideFirstRow; }
            set { _provideFirstRow = value; }
        }

        #endregion

        #region WebPart ViewState

        public int? XslPage
        {
            get { return (int?)ViewState[VS_XslPage]; }
            set { ViewState[VS_XslPage] = value; }
        }

        public int? XslSelectedRow
        {
            get { return (int?)ViewState[VS_XslSelectedRow]; }
            set { ViewState[VS_XslSelectedRow] = value; }
        }

        public string XslOrder
        {
            get { return (string)ViewState[VS_XslOrder]; }
            set { ViewState[VS_XslOrder] = value; }
        }

        //public int? XslPage
        //{
        //    get { return SPSPersist.TryGetFromSessionState<int?>(Page,this,ViewState,VS_XslPage); }
        //    set { SPSPersist.TrySaveInSessionState(Page, this, ViewState, VS_XslPage,value); }
        //}

        //public int? XslSelectedRow
        //{
        //    get { return SPSPersist.TryGetFromSessionState<int?>(Page, this,ViewState, VS_XslSelectedRow); }
        //    set { SPSPersist.TrySaveInSessionState(Page, this, ViewState, VS_XslSelectedRow, value); }
        //}

        //public string XslOrder
        //{
        //    get { return SPSPersist.TryGetFromSessionState<string>(Page, this, ViewState, VS_XslOrder); }
        //    set { SPSPersist.TrySaveInSessionState(Page, this, ViewState, VS_XslOrder, value); }
        //}

        public SPSKeyValueList LastRow
        {
            get
            {
                if (_lastRow == null)
                {
                    if (ViewState[VS_LastRow] != null)
                    {
                        _lastRow = (SPSKeyValueList)SPSSerialization.Deserialize(
                                                             (string)ViewState[VS_LastRow]);
                    }                   
                }
                return _lastRow;
            }
            set
            {
                if (value == null)
                {
                    ViewState[VS_LastRow] = null;
                }
                else
                {
                    ViewState[VS_LastRow] = SPSSerialization.Serialize(value);
                }
            }
        }

        public SPSKeyValueList LastFilter
        {
            get
            {
                if (_lastFilter == null)
                {
                    if (ViewState[VS_LastFilter] != null)
                    {
                        _lastFilter = (SPSKeyValueList)SPSSerialization.Deserialize(
                                                             (string)ViewState[VS_LastFilter]);
                    }
                }
                return _lastFilter;
            }
            set
            {
                if (value == null)
                {
                    ViewState[VS_LastFilter] = null;
                }
                else
                {
                    ViewState[VS_LastFilter] = SPSSerialization.Serialize(value);
                }
            }
        }

        #endregion

        public RollUpBase()
        {
           // InitializeEngine();
        }

        #region Parameters Consumer - Connection Point

        /// <summary>
        /// Connection point for consume parameters
        /// Make a schema of necesary parameters based on IdentityColumnCollection
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        [ConnectionConsumer("Caml Parameters", "RollUpParametersConsumer", AllowsMultipleConnections = true)]
        public void ConnectionParametersConsumer(IWebPartParameters parameters)
        {
            // Set the schema
            CamlPreprocessor camlPreprocessor = new CamlPreprocessor(CamlQuery);
            parameters.SetConsumerSchema(camlPreprocessor.GetSchema());
            // The get parameters callback
            parameters.GetParametersData(ConsumeParameterValues);
        }

        /// <summary>
        /// Consumes the parameter values.
        /// </summary>
        /// <param name="parametersData">The parameters data.</param>
        public void ConsumeParameterValues(IDictionary parametersData)
        {
            Debug.WriteLine("SPSRollUp ConsumeParameterValues " + Title);

            if (parametersData != null)
            {
                foreach (string key in parametersData.Keys)
                {
                    if (!string.IsNullOrEmpty(parametersData[key].ToString()))
                    {
                        _camlPreprocessor.AddVariable(
                                key,
                                parametersData[key].ToString());
                    }
                    Debug.WriteLine("*Parameter: " + key + " - " + parametersData[key]);
                }
            }

            if (Page != null)
            {
                // Process the new query
                _rollupEngine.CamlQuery = _camlPreprocessor.Evaluate();
                _rollupEngine.SelectedRow = 0;

                InternalCrawl(false);                
            }
        }

        #endregion

        #region Row Provider - Connection Point

        /// <summary>
        /// Gets the connection interface. (Row Provider)
        /// </summary>
        /// <returns></returns>
        [ConnectionProvider("RollUp Row", "RollUpRowProvider", AllowsMultipleConnections = true)]
        public IWebPartRow ConnectionRowProvider()
        {
            Debug.WriteLine("ConnectionRowProvider " + Title);
            // Using our special our SPSRowProvider class
            _rowProvider = new SPSRowProvider(GetRowViewForProvider());
            return _rowProvider;
        }

        private DataRowView GetRowViewForProvider()
        {

            if (Page == null)
            {
                Debug.WriteLine("GetRowViewForProvider (null page) " + Title);
                // Initialize a dummy engine
                InitializeEngine();
                // Send the Schema only 
                return _rollupEngine.GetSchema().GetDataView();
            }

            Debug.WriteLine("GetRowViewForProvider " + Title);
            
            SPSSchemaValue schemaValue = new SPSSchemaValue();

            if (LastRow == null && _provideFirstRow)
            {
                if (!_rollupEngine.SelectedRow.HasValue)
                {
                    _rollupEngine.SelectedRow = 0;
                }
                LastRow = _rollupEngine.GetDataValues();
            }

            if (LastRow != null)
            {
                foreach (SPSKeyValuePair keyValue in LastRow)
                {
                    schemaValue.AddParameter(keyValue.Key, "System.String");
                    schemaValue.AddDataValue(keyValue.Key, keyValue.Value);
                }
            }

            return schemaValue.GetDataView();
        }

        #endregion

        #region IPostBackEventHandler Members

        /// <summary>
        /// When implemented by a class, enables a server control to process an event 
        /// raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A <see cref="T:System.String"/> that represents an optional event argument to be passed to the event handler.</param>
        public void RaisePostBackEvent(string eventArgument)
        {
            // Process our postbacks
            Debug.WriteLine("RaisePostBackEvent " + Title);
            Debug.WriteLine("RaisePostBackEvent " + eventArgument);

            ProcessPostBackCommand(eventArgument);
        }

        private void ProcessPostBackCommand(string eventArgument)
        {
            if (eventArgument.StartsWith(CMD_Select))
            {
                // Get the selected row
                XslSelectedRow = Int32.Parse(eventArgument.Substring(7));
                if (_rollupEngine!=null)
                _rollupEngine.SelectedRow = XslSelectedRow;
                InternalCrawl(true);
                
            }

            if (eventArgument.StartsWith(CMD_Page))
            {
                // Get the selected row
                XslPage = Int32.Parse(eventArgument.Substring(5));
                InternalCrawl(true);
            }

            if (eventArgument.StartsWith(CMD_Order))
            {
                // Get the sort
                XslOrder = eventArgument.Substring(6);
                InternalCrawl(true);
            }

            
        }

        #endregion

        #region Control Methods

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad " + Title);
            const string scriptName = "SPSProfessional_SPSRollUp";
            ClientScriptManager clientScript = Page.ClientScript;
            if (!clientScript.IsClientScriptBlockRegistered(scriptName))
            {
                const string csstext = "<link href=\"_layouts/SPSRollUp/SPSRollUp.css\" " +
                                       "type=\"text/css\" rel=\"stylesheet\"></link>";

                clientScript.RegisterClientScriptBlock(
                        GetType(),
                        scriptName,
                        csstext,
                        false);
            }

            InitializeEngine();

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            //InitializeEngine();
            Debug.WriteLine("RollUp: CreateChildControls " + Title);

            // The errorbox control
            _errorBox.ShowExtendedErrors = ShowExtendedErrors;
            Controls.Add(_errorBox);
            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            Debug.WriteLine("RollUp: OnPreRender " + Title);

            // Crawl
            InternalCrawl(false);

            // Save last filter
            // LastFilter = _camlPreprocessor.VariablesValues;
            if (LastFilter == null)
                LastFilter = new SPSKeyValueList();
            _camlPreprocessor.CopyVariableValues(LastFilter);
        }

        #endregion

        #region Engine

        public void ForceCrawl()
        {
            _processed = false;
            InternalCrawl(false);
        }

        private void InternalCrawl(bool isPostBack)
        {
            Debug.WriteLine("InternalCrawl " + Title);

            if (isPostBack)
            {                
                _camlPreprocessor.InitializeVariableValues(LastFilter);
            }

            if (!_processed)
            {
                if (_rollupEngine.CamlQuery == null)
                {
                    _rollupEngine.CamlQuery = _camlPreprocessor.Evaluate();
                }

                // Get the data
                _rollupEngine.CrawlData();

                // Save the selected row
                if (_rollupEngine.SelectedRow.HasValue)
                {
                    LastRow = _rollupEngine.GetDataValues();
                }

                _processed = true;
            }
        }

        private void InitializeEngine()
        {
            Debug.WriteLine("InitializeEngine " + Title);

            _camlPreprocessor = new CamlPreprocessor(CamlQuery);
            
            _rollupEngine = new SPSRollUpEngine(
                    _topSite,
                    _lists,
                    _fields,
                    _maxRecords,
                    _camlQueryRecursive,
                    _includeListData,
                    _dateTimeISO,
                    _fixLookUp,
                    GetCacheService());

            _rollupEngine.OnError += TrapSubsystemError;
        }

        protected void DebugRender(HtmlTextWriter writer)
        {
            SPSEditorPartsTools editorTools = new SPSEditorPartsTools(writer);
            
            if (DebugQuery)
            {
                string resultQuery = editorTools.FormatXml(CamlQuery);
                writer.Write("<font color=red>DEBUG CAML Query (Original)</font><br><font color=blue>");
                writer.Write("<pre>" + SPEncode.HtmlEncode(resultQuery) + "</pre>");
                writer.Write("</font><br>");
                resultQuery = editorTools.FormatXml(_camlPreprocessor.Evaluate());
                writer.Write("<font color=red>DEBUG CAML Query (Processed)</font><br><font color=blue>");
                writer.Write("<pre>" + SPEncode.HtmlEncode(resultQuery) + "</pre>");
                writer.Write("</font><br>");
            }
            

            if (DebugResults)
            {
                writer.Write("<font color=red>DEBUG XML DATA</font><br><font color=blue>");
                writer.Write("<pre>" + 
                    SPEncode.HtmlEncode(editorTools.FormatXml(_rollupEngine.Data.GetXml())) + 
                    "</pre>");
                writer.Write("</font><br>");
            }

            if (DebugEvaluator)
            {
                writer.Write("<font color=red>DEBUG Evaluator</font><br>");
                writer.Write(EvaluatorSample());
                writer.Write("<br>");
            }
        }

        /// <summary>
        /// Test the evaluator
        /// </summary>
        /// <returns>An string with samples</returns>
        private static string EvaluatorSample()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] functions = new[]
                    {
                            "DateNow()",
                            "DateTimeNow()",
                            "MonthNumber()",
                            "DayNumber()",
                            "YearNumber()",
                            "Guid()",
                            "UserLogin()",
                            "UserName()",
                            "UserId()",
                            "UserEmail()",
                            "WebName()",
                            "WebTitle()",
                            "WebUrl()",
                            "Empty()",
                            "DateFormat('d')",
                            "DateFormat('s')",
                            "DateFormat('T')",
                            "DateCalcFormat('s',M+1)",
                            "DateCalcFormat('T',D+1)",
                            "DateCalcFormat('d',Y-1)",
                            "QueryString('Test')"
                    };

            SPSEvaluator evaluator = new SPSEvaluator();

            stringBuilder.Append("<table width='100%' border='0'>");

            foreach (string function in functions)
            {
                stringBuilder.AppendFormat("<tr><td width=50%><font color=blue>{0}</font></td><Td>{1}</td></tr>",
                                           function,
                                           evaluator.Evaluate(function));
            }
            stringBuilder.Append("</table>");
            return stringBuilder.ToString();
        }

        #endregion

        /// <summary>
        /// Traps the error messages from subsystems.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        protected void TrapSubsystemError(object sender, SPSErrorArgs args)
        {
            _errorBox.AddError(args);
        }

        #region Implementation of IWebPartCache

        public SPSCacheService GetCacheService()
        {
            return CacheService;
        }

        #endregion
    }
}