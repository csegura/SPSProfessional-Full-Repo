using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
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
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    public class RollUpBase3 : SPSWebPart, IPostBackEventHandler, IWebPartCache, ISPSViewCommandParameters
    {
        private const string CMD_Order = "Order:";
        private const string CMD_Page = "Page:";
        private const string CMD_Select = "Select:";
        private const string VS_LastRow = "LastRow";
        private const string VS_LastFilter = "LastFilter";
        private const string VS_XslOrder = "XslOrder";
        private const string VS_XslPage = "XslPage";
        private const string VS_XslSelectedRow = "XslSelectedRow";

        protected SPSErrorControl _errorBox;
        protected string _topSite;
        protected string _lists;
        protected string _fields;
        protected string _sortFields;
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
        private SPSKeyValueList _lastRow;
        private SPSKeyValueList _lastFilter;
        private bool _processed;
        private bool _provideFirstRow;

        private SPSRollUpOptions _options;
        internal SPSDataCollector _collector;

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
            get { return SPSRollUpOptions.CheckInConfigurationManager(_camlQuery); }
            set { _camlQuery = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string SortByFields
        {
            get { return _sortFields; }
            set { _sortFields = value; }
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
            get { return SPSRollUpOptions.CheckInConfigurationManager(_xsl); ; }
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

        #region ISPSXsltCommandControl Members
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

        #endregion

        #region WebPart ViewState

        
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
            private get
            {
                if (_lastRow == null)
                {
                    _lastRow = SPSSerialization.Deserialize<SPSKeyValueList>(
                            (string) ViewState[VS_LastRow]);
                }
                return _lastRow;
            }
            set { ViewState[VS_LastRow] = SPSSerialization.Serialize(value); }
        }

        public SPSKeyValueList LastFilter
        {
            private get
            {
                if (_lastFilter == null)
                {
                    _lastFilter = SPSSerialization.Deserialize<SPSKeyValueList>(
                            (string) ViewState[VS_LastFilter]);
                }
                return _lastFilter;
            }
            set { ViewState[VS_LastFilter] = SPSSerialization.Serialize(value); }
        }

        #endregion

        #region Parameters Consumer - Connection Point

        /// <summary>
        /// Connection point for consume parameters
        /// Make a schema of necesary parameters based on IdentityColumnCollection
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        [ConnectionConsumer("Caml Parameters", "RollUp3ParametersConsumer", AllowsMultipleConnections = true)]
        public void ConnectionParametersConsumer(IWebPartParameters parameters)
        {
            Debug.WriteLine("ConnectionParametersConsumer " + Title);
            // Set the schema
            // Using a dummy CamlProcessor
            if (Page == null)
            {
                Debug.WriteLine("- null Page (DummyCamlEngine) " + Title);
                InitializeCamlEngine();
            }
            // _camlPreprocessor = new CamlPreprocessor(CamlQuery);
            parameters.SetConsumerSchema(_camlPreprocessor.GetSchema());
            // The get parameters callback
            parameters.GetParametersData(ConsumeParameterValues);
        }

        /// <summary>
        /// Consumes the parameter values.
        /// </summary>
        /// <param name="parametersData">The parameters data.</param>
        public void ConsumeParameterValues(IDictionary parametersData)
        {
            Debug.WriteLine("ConsumeParameterValues " + Title);
            _camlPreprocessor.AddValues(parametersData);
        }

        #endregion

        #region Row Provider - Connection Point

        /// <summary>
        /// Gets the connection interface. (Row Provider)
        /// </summary>
        /// <returns>The IWebPartRow</returns>
        [ConnectionProvider("RollUp Row", "RollUpRowProvider", AllowsMultipleConnections = true)]
        public IWebPartRow ConnectionRowProvider()
        {
            Debug.WriteLine("ConnectionRowProvider " + Title);
            // Using our special our SPSRowProvider class
            _rowProvider = new SPSRowProvider(GetRowViewForProvider());
            return _rowProvider;
        }

        /// <summary>
        /// Gets the row view for provider.
        /// </summary>
        /// <returns>The DataRowView</returns>
        private DataRowView GetRowViewForProvider()
        {
            Debug.WriteLine("GetRowViewForProvider " + Title);

            if (Page == null)
            {
                Debug.WriteLine("- null Page (DummyEngine) " + Title);
                // Initialize a dummy engine
                InitializeEngine();
                // Send the Schema only 
                return _collector.GetSchema().GetDataView();
            }
           
            InternalCollect();
            SaveLastRowValuesFromCollector();

            SPSSchemaValue schemaValue = GetSchemaFromLastRow();

            return schemaValue.GetDataView();
        }

        /// <summary>
        /// Gets the schema from last row.
        /// </summary>
        /// <returns>The SPSSchemaValue</returns>
        private SPSSchemaValue GetSchemaFromLastRow()
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();

            foreach (SPSKeyValuePair keyValue in LastRow)
            {
                //schemaValue.AddParameter(keyValue.Key, "System.String");
                //schemaValue.AddDataValue(keyValue.Key, keyValue.Value);
                schemaValue.Add(keyValue.Key, keyValue.Value);
            }
            return schemaValue;
        }

        /// <summary>
        /// Saves the last row values from collector.
        /// It's necesary because the SelectedRow can be different we need
        /// save the new LastRow to send the correct row to a provider.
        /// </summary>
        internal void SaveLastRowValuesFromCollector()
        {
            if (XslSelectedRow.HasValue)
            {
                LastRow = _collector.GetRowValues(XslSelectedRow);
            }
            else
            {
                LastRow = _collector.GetRowValues(0);
            }
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

            if (eventArgument.StartsWith(CMD_Select))
            {
                // Get the selected row
                XslSelectedRow = Int32.Parse(eventArgument.Substring(7));
            }

            if (eventArgument.StartsWith(CMD_Page))
            {
                // Get the selected row
                XslPage = Int32.Parse(eventArgument.Substring(5));
            }

            if (eventArgument.StartsWith(CMD_Order))
            {
                // Get the sort
                XslOrder = eventArgument.Substring(6);
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
            
            InternalCollect();

            // Save last filter
            _camlPreprocessor.CopyVariableValues(LastFilter);
        }

        #endregion

        #region Engine

        /// <summary>
        /// Internals the collect.
        /// </summary>
        internal void InternalCollect()
        {
            Debug.WriteLine("InternalCollect " + Title);

            try
            {
                if (!_processed)
                {
                    Debug.WriteLine("- Collect");
                    _collector.Collect();
                    Debug.WriteLine("- Save LastRow");
                    LastRow = _collector.GetRowValues(XslSelectedRow);
                }
                else
                {
                    Debug.WriteLine("- Collect already processed.");
                }
            }
            catch (Exception ex)
            {
                TrapSubsystemError(this,
                                   new SPSErrorArgs(GetType().Name,
                                                    ex.InnerException.Message,
                                                    ex));
            }
            finally
            {
                _processed = true;
            }
        }

        /// <summary>
        /// Initializes the engine.
        /// </summary>
        public void InitializeEngine()
        {
            Debug.WriteLine("InitializeEngine " + Title);

            InitializeCamlEngine();

            _options = new SPSRollUpOptions(_topSite,
                                            _lists,
                                            _fields,
                                            _sortFields,
                                            _maxRecords,
                                            _camlQueryRecursive,
                                            _includeListData,
                                            _dateTimeISO,
                                            _fixLookUp,
                                            GetCacheService());

            _collector = new SPSDataCollector(_options, _camlPreprocessor);
        }

        /// <summary>
        /// Initializes the caml engine.
        /// </summary>
        private void InitializeCamlEngine()
        {
            Debug.WriteLine("InitializeCamlEngine " + Title);

            // There is a prevous initialization in 
            // ConnectionParametersConsumer
            if (_camlPreprocessor == null)
            {
                _camlPreprocessor = new CamlPreprocessor(CamlQuery);
            }

            // Editing the connection Page is null
            if (Page != null && Page.IsPostBack)
            {
                Debug.WriteLine("- InitializeVariableValues (LastFilter)");
                _camlPreprocessor.InitializeVariableValues(LastFilter);
            }
        }       

        public void ResetCamlPreprocessor()
        {
            _camlPreprocessor = null;
        }

        /// <summary>
        /// Debugs the render.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected void DebugRender(HtmlTextWriter writer)
        {
            SPSEditorPartsTools editorTools = new SPSEditorPartsTools(writer);

            if (DebugQuery)
            {
                if (!string.IsNullOrEmpty(CamlQuery))
                {
                    WriteDebug(writer, "DEBUG CAML Query (No Query)", string.Empty);
                }
                else
                {
                    string resultQuery = editorTools.FormatXml(CamlQuery);
                    WriteDebug(writer, "DEBUG CAML Query (Original)", resultQuery);
                    resultQuery = editorTools.FormatXml(_camlPreprocessor.Evaluate());
                    WriteDebug(writer, "DEBUG CAML Query (Processed)", resultQuery);
                }
            }

            if (DebugResults)
            {
                WriteDebug(writer, "DEBUG XML DATA", editorTools.FormatXml(_collector.GetXml()));
            }

            if (DebugEvaluator)
            {
                writer.Write("<font color=red>DEBUG Evaluator</font><br>");
                writer.Write(EvaluatorSample());
                writer.Write("<br>");
            }
        }

        /// <summary>
        /// Writes the debug.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        private void WriteDebug(TextWriter writer, string title, string message)
        {
            writer.Write("<font color=red>" + title + "</font><br><font color=blue>");
            writer.Write("<pre>" + SPEncode.HtmlEncode(message) + "</pre>");
            writer.Write("</font><br>");
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