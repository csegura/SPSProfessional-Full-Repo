using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using SPSProfessional.SharePoint.Framework.Comms;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionGridConfig;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Controls;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    [ToolboxData("<{0}:ActionGrid runat=server></{0}:ActionGrid>"),
     XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionGrid")]
    public class ActionGrid : SPSWebPart, IPostBackEventHandler
    {
        private SPSActionGridConfig _config;

        private SPSGridView _gridView;
        private readonly SPSErrorBoxControl _actionGridError;

        private SPSRowProvider _rowProvider;
        private int _selectedRowIndex;
        private bool _isSelectingRow;

        private MenuTemplate _menuTemplate;
        private SqlDataSource _sqlDataSource;

        private readonly List<IFilterValues> _parameterValues;
        private bool _processed;

        #region WebPart Properties

        private string _xmlConfig;

        public string XmlConfig
        {
            get { return _xmlConfig; }
            set
            {
                _xmlConfig = value;
                _config = null;
            }
        }

        private bool _showExtendedErrors;

        public bool ShowExtendedErrors
        {
            get { return _showExtendedErrors; }
            set { _showExtendedErrors = value; }
        }

        #endregion

        #region Properties

        public SPSActionGridConfig Config
        {
            get
            {
                if (_config == null)
                {
                    ReadConfig();
                }
                return _config;
            }
        }

        #endregion

        #region Constructor

        public ActionGrid()
        {
            _parameterValues = new List<IFilterValues>();
            _isSelectingRow = false;

            SPSInit("DD434B08-2854-4f29-81A7-44FC86E56886",
                    "ActionDataBase.1.0",
                    "WebParts ActionDataBase",
                    "http://www.spsprofessional.com/");

            EditorParts.Add(new ActionGridEditorPart());

            _actionGridError = new SPSErrorBoxControl();
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

            // TODO: Change Row$ -> Select
            if (eventArgument.StartsWith("Row$"))
            {
                _selectedRowIndex = Int32.Parse(eventArgument.Substring(4));
                _isSelectingRow = true;
            }
        }

        #endregion

        #region Row Provider - Connection Point

        /// <summary>
        /// Gets the connection interface. (Row Provider)
        /// </summary>
        /// <returns></returns>
        [ConnectionProvider("Grid Row", "ActionDataBaseGridRowProvider", AllowsMultipleConnections = true)]
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
        /// <returns></returns>
        private DataRowView GetRowViewForProvider()
        {
            Debug.WriteLine("GetRowViewForProvider " + Title);

            DataRowView rowView;

            // When connecting the webpart there is not 
            // Page context available (manual strategy)
            if (Page == null)
            {
                EnsureChildControls();

                Debug.WriteLine("GetRowViewForProvider ManualDataView");

                rowView = GetColumnsForConnectionPoint();
                Debug.WriteLine("GetRowViewForProvider ManualDataView End");
            }
            else
            {
                // Normal strategy get the provider data from grid
                BindGrid();
                rowView = _gridView.SelectedDataRow;
            }
            return rowView;
        }

        /// <summary>
        /// Gets the columns for connection point.
        /// When connecting the grid we dont´t have 
        /// the filter parameters definied we need get the result columns to define
        /// the interface. To do it we need do a query without filter (Where clause) 
        /// </summary>
        /// <returns>The data row view that contains the columns</returns>
        private DataRowView GetColumnsForConnectionPoint()
        {
            // Create a DataView using the SqlDataSource
            DataView dv;

            // Remove filter (where clause)
            // We don´t have filter data and we need get all result
            // columns in order to define the connection point
            if (_sqlDataSource.SelectCommand.ToUpper().Contains("WHERE"))
            {
                _sqlDataSource.SelectCommand =
                    _sqlDataSource.SelectCommand.Remove(
                        _sqlDataSource.SelectCommand.IndexOf("WHERE",
                                                             StringComparison.InvariantCultureIgnoreCase));
            }

            DumpFilter();

            // Do the query
            dv = (DataView) _sqlDataSource.Select(DataSourceSelectArguments.Empty);

            Debug.Assert(dv != null);
            Debug.Assert(dv.Table != null);

            return dv.Table.DefaultView[0];
        }

        #endregion

        #region Parameters Consumer - Connection Point

        /// <summary>
        /// Connection point for consume parameters
        /// Make a schema of necesary parameters based on IdentityColumnCollection
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        [ConnectionConsumer("Key Parameters", "ActionGridParametersConsumer", AllowsMultipleConnections = true)]
        public void ConnectionParametersConsumer(IWebPartParameters parameters)
        {
            Debug.WriteLine("ConnectionParametersConsumer " + Title);

            if (parameters != null && Config != null)
            {
                SPSSchema schema = new SPSSchema();

                // Add each necesary parameter 
                foreach (Param param in Config.Filter)
                {
                    schema.AddParameterSql(param.Name, param.Type);
                }

                // Set the schema
                parameters.SetConsumerSchema(schema.Schema);

                // The get parameters callback
                parameters.GetParametersData(ConsumeParameterValues);
            }
        }

        /// <summary>
        /// Consumes the parameter values.
        /// </summary>
        /// <param name="parametersData">The parameters data.</param>
        public void ConsumeParameterValues(IDictionary parametersData)
        {
            Debug.WriteLine("ConsumeParameterValues " + Title);

            if (parametersData != null)
            {
                foreach (Object key in parametersData.Keys)
                {
                    _parameterValues.Add(new ActionFilterValues(key.ToString(), 
                                                                parametersData[key].ToString()));
                    Debug.WriteLine("->ConsumeParameterValues:" + Title + " ->" + key + " - " + parametersData[key]);
                }
            }
        }

        #endregion

        #region Internal Engine

        /// <summary>
        /// Binds the data to the grid.
        /// </summary>
        private void BindGrid()
        {
            Debug.WriteLine("BindGrid " + Title);
            if (!_processed)
            {
                try
                {
                    // We need the controls created
                    EnsureChildControls();

                    // Add Parameters to query
                    AddParameters();

                    // Set the selected row
                    if (_isSelectingRow)
                    {
                        _gridView.SelectedDataItem = _selectedRowIndex;
                    }

                    _gridView.ForceBinding();
                    _processed = true;

                    Debug.WriteLine(_sqlDataSource.SelectCommand);
                    Debug.WriteLine("BindGrid Completed -" + Title);
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs(Title, ex.Message, ex));
                    DumpException("BindGrid", ex);
                }
            }
        }

        /// <summary>
        /// Deserialize the configuration XML
        /// </summary>
        private void ReadConfig()
        {
            if (!string.IsNullOrEmpty(XmlConfig))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SPSActionGridConfig));
                    StringReader stringReader = new StringReader(XmlConfig);
                    _config = (SPSActionGridConfig) serializer.Deserialize(stringReader);
                    stringReader.Close();
                }
                catch (InvalidOperationException ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs(Title, ex.Message, ex));
                    DumpException("ReadConfig", ex);
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs(Title, ex.Message, ex));
                    DumpException("ReadConfig", ex);
                }
            }
        }

        /// <summary>
        /// Add columns from configuration to Grid
        /// </summary>
        private void AddColumns()
        {
            foreach (DataField dataField in Config.Columns)
            {
                // Generate menus
                if (dataField.ContextMenu.Count > 0)
                {
                    SPMenuField menu = new SPMenuField();
                    menu.HeaderText = dataField.Header;
                    menu.TextFields = dataField.Name;
                    menu.SortExpression = dataField.Name;
                    menu.MenuTemplateId = "MT" + ID + dataField.Name;

                    if (string.IsNullOrEmpty(dataField.ContextMenu.Fields))
                    {
                        menu.NavigateUrlFields = dataField.ContextMenu.Fields;
                    }

                    // TODO: Change 
                    if (!string.IsNullOrEmpty(dataField.ContextMenu.Format))
                    {
                        //menu.NavigateUrlFormat = dataField.ContextMenu.Format;
                        menu.TokenNameAndValueFields = dataField.ContextMenu.Format;
                    }

                    // Menu Template linked
                    _menuTemplate = new MenuTemplate();
                    _menuTemplate.ID = "MT" + ID + dataField.Name;

                    foreach (ContextMenuItem menuItem in dataField.ContextMenu)
                    {
                        MenuItemTemplate optionMenu = new MenuItemTemplate(menuItem.Name, menuItem.Image);

                        if (menuItem.Url.Contains("Row"))
                        {
                            Debug.WriteLine(string.Format("Menu {0} - {1} - {2}", Title, dataField.Name, menuItem.Name));
                            //TODO: Check the send item (see Test1) Send value is the column name 
                            //not the field value

                            // Page become null when we are connecting webparts
                            if (Page != null)
                            {
                                // Generate the script
                                string script = Page.ClientScript.GetPostBackEventReference(this, "_");
                                // Add the special SPSGetRowId call
                                optionMenu.ClientOnClickScript =
                                    script.Replace("'_'", "'Row$'+SPSGetRowId('%MENUCLIENTID%')");
                            }
                        }
                        else
                        {
                            // Other url
                            optionMenu.ClientOnClickNavigateUrl = menuItem.Url;
                        }

                        _menuTemplate.Controls.Add(optionMenu);
                    }
                    Controls.Add(_menuTemplate);
                    _gridView.Columns.Add(menu);
                }
                else
                {
                    
                        // Normal Bound field
                        BoundField boundField = new BoundField();
                        boundField.DataField = dataField.Name;
                        boundField.HeaderText = dataField.Header;

                        if (string.IsNullOrEmpty(dataField.SortExpression))
                        {
                            boundField.SortExpression = dataField.Name;
                        }
                        else
                        {
                            boundField.SortExpression = dataField.SortExpression;
                        }

                        if (dataField.IsHtml)
                        {
                            boundField.HtmlEncode = false;
                        }

                        if (!string.IsNullOrEmpty(dataField.Format))
                        {
                            boundField.DataFormatString = dataField.Format;
                            boundField.HtmlEncode = false;
                        }

                        if (dataField.Select)
                        {
                            boundField.DataFormatString = string.Format("<a href=\"#\" onclick=\"{0}\">", SelectRowScript());
                            boundField.DataFormatString += "{0}</a>";
                            boundField.HtmlEncode = false;
                        }

                        _gridView.Columns.Add(boundField);
                    
                }
            }
        }

        private string SelectRowScript()
        {
            // Page become null when we are connecting webparts
            if (Page != null)
            {
                // Generate the script
                string script = Page.ClientScript.GetPostBackEventReference(this, "_");
                Debug.WriteLine("__>" + script.Replace("'_'", "'Row$'+SPSGetRowId('%MENUCLIENTID%')"));
                // Add the special SPSGetRowId call
                return "javascript:" + script.Replace("'_'", "'Row$'+SPSGetRowId2(this)");
            }
            return null;
        }

        private void AddParameters()
        {
            Debug.WriteLine("AddParameters " + Title);

            if (_sqlDataSource != null)
            {
                // Add default parameters
                AddSelectParamsDefaults(_sqlDataSource);

                // If any add filter params
                if (_parameterValues != null && _parameterValues.Count > 0)
                {
                    AddSelectParamsFromFilter(_sqlDataSource);
                }
            }
        }


        /// <summary>
        /// Adds the filter params to the select using default the values
        /// </summary>
        /// <param name="sqlSource">The data source</param>
        private void AddSelectParamsDefaults(SqlDataSource sqlSource)
        {
            Debug.WriteLine("AddSelectParamsDefaults " + Title);
            ActionFunctions functions = new ActionFunctions();
            sqlSource.SelectParameters.Clear();

            foreach (Param param in Config.Filter)
            {
                Debug.WriteLine(string.Format("*Default {0} - {1}", param.Name, functions.Evaluate(param.Default)));
                sqlSource.SelectParameters.Add(param.Name, functions.Evaluate(param.Default));
                Debug.WriteLine(string.Format("Default {0} - {1}", param.Name, functions.Evaluate(param.Default)));
            }
        }

        /// <summary>
        /// Modify the parameters using values that comming from filter connection
        /// </summary>
        /// <param name="sqlSource">The data source</param>
        private void AddSelectParamsFromFilter(SqlDataSource sqlSource)
        {
            Debug.WriteLine("AddSelectParamsFromFilter " + Title);
            Debug.Assert(sqlSource != null);

            foreach (IFilterValues filter in _parameterValues)
            {
                try
                {
                    if (!string.IsNullOrEmpty(filter.ParameterValues[0]))
                    {
                        sqlSource.SelectParameters[filter.ParameterName].DefaultValue = filter.ParameterValues[0];
                        Debug.WriteLine(
                            string.Format("ActionGrid Add Parameter {0} - {1}",
                                          filter.ParameterName,
                                          filter.ParameterValues[0]));
                    }
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs(Title, ex.Message, ex));
                    DumpException("ReadConfig", ex);
                }
            }
        }

        /// <summary>
        /// Ptrepare the grid before data binding
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DoDataLoad(object sender, EventArgs e)
        {
            Debug.WriteLine("DoDataLoad " + Title);

            // Turn on paging and add event handler                    
            _gridView.AllowPaging = Config.Grid.Pageable;

            if (Config.Grid.Pageable)
            {
                _gridView.PageSize = Config.Grid.PageSize;

                // Must be called after Controls.Add                    
                _gridView.PagerTemplate = null;
            }
        }

        /// <summary>
        /// Determines whether this instance is providing data via connection interface.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is providing; otherwise, <c>false</c>.
        /// </returns>
        private bool IsProviding()
        {
            foreach (WebPartConnection connection in WebPartManager.Connections)
            {
                if (connection.ProviderID == ID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Traps the error messages from subsystems.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void TrapSubsystemError(object sender, SPSErrorArgs args)
        {
            _actionGridError.AddError(args);
        }

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        #endregion

        #region Control Methods

        protected override void CreateChildControls()
        {
            Debug.WriteLine("CreateChildControls " + Title);
            if (Config != null)
            {
                try
                {
                    _actionGridError.ShowExtendedErrors = _showExtendedErrors;
                    Controls.Add(_actionGridError);

                    // Setup the datasource - Database 
                    _sqlDataSource = new SqlDataSource(Config.DataBase.ConnectionString,
                                                       Config.Query.SelectCommand);
                    _sqlDataSource.ID = "ds_" + ID;
                    _sqlDataSource.EnableCaching = Config.Query.Cache;

                    Controls.Add(_sqlDataSource);

                    // Setup the grid
                    _gridView = new SPSGridView();
                    _gridView.ID = "gr_" + ID;
                    _gridView.DataSourceID = "ds_" + ID;

                    _gridView.AutoGenerateColumns = false;
                    _gridView.DoDataLoad += DoDataLoad;
                    _gridView.EnableViewState = true;
                    _gridView.OnError += TrapSubsystemError;

                    Debug.WriteLine("ActionGrid: AddColumns");
                    AddColumns();

                    _gridView.Width = new Unit("100%");
                    Controls.Add(_gridView);
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs(Title, ex.Message, ex));
                    DumpException("CreateChildControls", ex);
                }
            }
            else
            {
                Debug.WriteLine("Config is null");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Debug.WriteLine("OnPreRender " + Title);
            base.OnPreRender(e);

            if (Config != null)
            {
                // If not processed (no provider) bind now               
                if (!_processed)
                {
                    BindGrid();
                }
            }
        }


        protected override void SPSRender(HtmlTextWriter writer)
        {
            Debug.WriteLine("SPSRender " + Title);

            _actionGridError.RenderControl(writer);

            if (Config != null)
            {
                if (IsProviding())
                {
                    _gridView.HiligthSelectedRow = true;
                }

                if (_menuTemplate != null)
                {
                    _menuTemplate.RenderControl(writer);
                }

                _gridView.RenderControl(writer);

                //base.Render(writer);
            }
            else
            {
                writer.Write(MissingConfiguration);
            }
        }

        #endregion

        #region DUMP

        private void DumpFilter()
        {
            //foreach (IFilterValues filter in _parameterValues)
            //{
            //    writer.WriteLine(string.Format("Parameter: {0} <br>", filter.ParameterName));

            //    if (filter.ParameterValues != null)
            //    {
            //        foreach (string value in filter.ParameterValues)
            //            if (!string.IsNullOrEmpty(value))
            //                writer.WriteLine(string.Format("  value: {0} <br>", value));
            //    }
            //}

            foreach (Parameter param in _sqlDataSource.SelectParameters)
            {
                Debug.WriteLine("Param:" + param.Name + " -> " + param.DefaultValue + " -> " + param.Type + "<br/>");
            }
            //writer.WriteLine(Config.Query.SelectCommand);
            Debug.WriteLine(_sqlDataSource.SelectCommand);
        }

        #endregion
    }
}