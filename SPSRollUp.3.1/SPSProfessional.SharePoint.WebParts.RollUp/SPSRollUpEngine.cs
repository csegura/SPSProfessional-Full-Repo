using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Comms;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    internal class SPSRollUpEngine : ISPSErrorControl
    {
        private const string SPSROLLUP = "SPSROLLUP";
        private const string LOOKUP_FIELD_SEPARATOR = ";#";
        private readonly bool _camlQueryRecursive;
        private DataSet _dataSet;
        private readonly string _fields;
        private readonly List<string> _fieldsList;
        private readonly bool _includeListData;
        private readonly string _lists;

        private readonly List<string> _listsList;
        private readonly string _topSite;
        private readonly bool _bDateTimeISO8601;
        private readonly bool _bFixLookUp;
        private bool _processed;
        private int _rowNumber;
        private int _recordCounter;
        private readonly int _maxRecords;
        private int? _selectedRow;
        private string _camlQuery;

        private readonly ISPSCacheService _cacheService;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSRollUpEngine"/> class.
        /// </summary>
        /// <param name="topSite">The top site.</param>
        /// <param name="lists">The lists.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="maxResults">Max Records to retry</param>
        /// <param name="camlQueryRecursive">if set to <c>true</c> [caml query recursive].</param>
        /// <param name="includeListData">if set to <c>true</c> [include list data].</param>
        /// <param name="dateTimeISO">if set to <c>true</c> [date time ISO].</param>
        /// <param name="fixLookUp">if set to <c>true</c> [fix look up].</param>
        /// <param name="cacheService">The cache service.</param>
        public SPSRollUpEngine(string topSite,
                               string lists,
                               string fields,
                               int maxResults,
                               bool camlQueryRecursive,
                               bool includeListData,
                               bool dateTimeISO,
                               bool fixLookUp,
                               ISPSCacheService cacheService)
        {
            _topSite = topSite;
            _lists = lists;
            _fields = fields;
            _maxRecords = maxResults;
            _camlQueryRecursive = camlQueryRecursive;
            _includeListData = includeListData;
            _listsList = new List<string>(_lists.Split(','));
            _fieldsList = new List<string>(_fields.Split(','));
            _dataSet = GenerateDataSet();
            _bDateTimeISO8601 = dateTimeISO;
            _bFixLookUp = fixLookUp;
            _cacheService = cacheService;
        }

        #endregion

        #region Public Properties

        public DataSet Data
        {
            get { return _dataSet; }
        }

        public bool HasResults
        {
            get { return Data.Tables[0].Rows.Count > 0; }
        }

        public int? SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; }
        }

        public string CamlQuery
        {
            get { return _camlQuery; }
            set { _camlQuery = value; }
        }

        #endregion

        #region ISPSErrorControl Members

        public event SPSControlOnError OnError;

        #endregion

        #region Public Methods

        /// <summary>
        /// Crawls the data. Using a caml query
        /// </summary>
        public void CrawlData()
        {
            if (_cacheService != null)
            {
                _dataSet = _cacheService.Get<DataSet>(SPSROLLUP, RollUp);
            }
            else
            {
                RollUp();
            }
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns>The schema</returns>
        public SPSSchemaValue GetSchema()
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();

            // Generate Schema
            foreach (DataColumn column in Data.Tables[0].Columns)
            {
                schemaValue.AddParameter(column.ColumnName, column.DataType);
            }
            return schemaValue;
        }

        /// <summary>
        /// Gets the row data.
        /// </summary>
        /// <returns>Contains the schema if no data, otherwise data and schema</returns>
        public SPSKeyValueList GetDataValues()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();

            // Generate Data
            if (SelectedRow != null && HasResults)
            {
                DataRowView rowView = Data.Tables[0].DefaultView[SelectedRow.Value];
                foreach (DataColumn column in Data.Tables[0].Columns)
                {
                    keyValues.Add(column.ColumnName, rowView[column.ColumnName].ToString());
                }
                return keyValues;
            }

            return null;
        }

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        public void Invalidate()
        {
            _processed = false;
        }

        #endregion

        //public DataRowView GetSelectedData()
        //{
        //    if (SelectedRow != null && HasResults)
        //    {
        //        return Data.Tables[0].DefaultView[SelectedRow.Value];
        //    }
        //    Data.Tables[0].Rows.Add(Data.Tables[0].NewRow());
        //    return Data.Tables[0].DefaultView[0];
        //}

        #region Private Methods

        /// <summary>
        /// Rolls the up.
        /// </summary>
        private DataSet RollUp()
        {
            Debug.WriteLine("RollUp Called");

            if (!_processed)
            {
                Debug.WriteLine("RollUp Processing");

                _dataSet.Tables[0].Rows.Clear();
                _rowNumber = 0;

                // Select top web or current
                using(SPWeb web = GetWebToUse())
                {
                    try
                    {
                        // Filter we only need lists
                        SPSHierarchyFilter dataFilter = new SPSHierarchyFilter
                                                            {
                                                                    SortHierarchy = false,
                                                                    IncludeLists = true,
                                                                    IncludeWebs = _camlQueryRecursive,
                                                                    IncludeFolders = false
                                                            };

                        dataFilter.OnFilter += DataSourceFilter;

                        // DataSource
                        using(SPSHierarchyDataSource source = new SPSHierarchyDataSource(web))
                        {
                            source.Filter = dataFilter;
                            // get all elements . on filter does the rollup
                            source.GetAll();
                        }
                        _processed = true;
                    }
                    catch (SPException ex)
                    {
                        SendError(new SPSErrorArgs(ex.TargetSite.Name, ex.Message, ex));
                    }
                }
            }
            return _dataSet;
        }

        /// <summary>
        /// Data Source Filter
        /// Check if the list needed matches our selection and is added to the data source
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <returns>True if pass the filter</returns>
        private bool DataSourceFilter(object sender, SPSHierarchyFilterArgs args)
        {
            if (args.List != null)
            {
                bool include = _listsList.Contains(args.List.Title);
                //Debug.WriteLine(string.Format("{0}Include:{1} - {2}",
                //                              include ? "   " : "",
                //                              args.List.ParentWebUrl,
                //                              args.List.Title));
                if (include)
                {
                    FillDataTable(args.Web, args.List);
                }
                return include;
            }
            return true;
        }

        /// <summary>
        /// Gets the web to use.
        /// </summary>
        /// <returns>The TopSite or the current web</returns>
        /// <exception cref="SPException"><c>SPException</c>.</exception>
        private SPWeb GetWebToUse()
        {
            SPWeb web;
            if (!string.IsNullOrEmpty(_topSite))
            {
                try
                {
                    using(SPSite site = new SPSite(_topSite))
                    {
                        Uri uri = new Uri(_topSite, UriKind.RelativeOrAbsolute);
                        //string relativeWeb = uri.MakeRelativeUri(new Uri(_topSite,UriKind.Absolute)).ToString();
                        web = site.OpenWeb(uri.LocalPath);
                    }
                }
                catch (SPException ex)
                {
                    SendError(new SPSErrorArgs(ex.TargetSite.Name,
                                               string.Format(GetResourceString("SPS_Err_OpenSite"), _topSite),
                                               ex));
                    throw;
                }
            }
            else
            {
                web = SPContext.Current.Web.Site.OpenWeb();
            }

            return web;
        }

        /// <summary>
        /// Generates the data set.
        /// </summary>
        /// <returns>Dataset to store results</returns>
        private DataSet GenerateDataSet()
        {
            DataSet dataSet = new DataSet("Rows");
            DataTable dataTable = new DataTable("Row");

            if (_includeListData)
            {
                dataTable.Columns.Add(new DataColumn("_ListTitle", typeof(string)));
                dataTable.Columns.Add(new DataColumn("_ListUrl", typeof(string)));
                dataTable.Columns.Add(new DataColumn("_ListId", typeof(string)));
                dataTable.Columns.Add(new DataColumn("_SiteTitle", typeof(string)));
                dataTable.Columns.Add(new DataColumn("_SiteUrl", typeof(string)));
                dataTable.Columns.Add(new DataColumn("_ItemId", typeof(string)));
                dataTable.Columns.Add(new DataColumn("_ItemUrl", typeof(string)));
                dataTable.Columns.Add(new DataColumn("_ItemEdit", typeof(string)));
            }

            dataTable.Columns.Add(new DataColumn("_RowNumber", typeof(string)));

            foreach (string columnName in _fieldsList)
            {
                dataTable.Columns.Add(new DataColumn(columnName, typeof(string)));
            }

            dataSet.Tables.Add(dataTable);

            return dataSet;
        }

        ///// <summary>
        ///// Fills the data table.
        ///// </summary>
        ///// <param name="web">The web.</param>
        ///// <param name="list">The list.</param>
        //private void BuildDataTable(SPWeb web, SPList list)
        //{
        //    Debug.Assert(list != null);

        //    try
        //    {
        //        DataTable dataTable = _dataSet.Tables[0];

        //        // CAML Query
        //        SPListItemCollection listItems = GetListItems(list);

        //        if (listItems != null)
        //        {
        //            foreach (SPListItem item in listItems)
        //            {
        //                if (MaxRecordsNotExceed())
        //                {
        //                    break;
        //                }

        //                try
        //                {
        //                    #region Fields

        //                    foreach (string fieldName in _fieldsList)
        //                    {
        //                        try
        //                        {
        //                            if (item.Fields.ContainsField(fieldName))
        //                            {
        //                                EnsureTableColumn(item.Fields.GetField(fieldName), dataTable);
        //                            }
        //                        }
        //                        catch (ArgumentException ex)
        //                        {
        //                            SendError(new SPSErrorArgs("BuildDataTable",
        //                                                       "Field not found. "
        //                                                       + fieldName,
        //                                                       ex));
        //                        }
        //                    }

        //                    #endregion
        //                }
        //                catch (ArgumentOutOfRangeException ex)
        //                {
        //                    SendError(new SPSErrorArgs("BuildDataTable",
        //                                               GetResourceString("SPS_Err_AddData"),
        //                                               ex));
        //                }
        //            }
        //        }
        //    }
        //    catch (SPException ex)
        //    {
        //        SendError(new SPSErrorArgs("*BuildDataTable", GetResourceString("SPS_Err_AddData"), ex));
        //    }
        //    catch (Exception ex)
        //    {
        //        SendError(new SPSErrorArgs("**Fill-Data", GetResourceString("SPS_Err_AddData"), ex));
        //    }
        //}

        ///// <summary>
        ///// Adds the field data.
        ///// </summary>
        ///// <param name="field">The field.</param>
        ///// <param name="table">The table.</param>
        //private static void EnsureTableColumn(SPField field, DataTable table)
        //{
        //    if (!table.Columns.Contains(field.InternalName))
        //    {
        //        if (field.FieldValueType != null
        //            && field.FieldValueType.BaseType.ToString() == "System")
        //        {
        //            table.Columns.Add(field.InternalName, field.FieldValueType);
        //        }
        //        else
        //        {
        //            table.Columns.Add(field.InternalName, typeof(string));
        //        }
        //    }
        //}

        /// <summary>
        /// Fills the data table.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        private void FillDataTable(SPWeb web, SPList list)
        {
            Debug.Assert(list != null);
            
            try
            {
                DataTable table = _dataSet.Tables[0];

                // CAML Query
                SPListItemCollection items = GetListItems(list);

                if (items != null)
                {
                    foreach (SPListItem item in items)
                    {
                        if (MaxRecordsNotExceed())
                        {
                            DataRow dataRow = table.NewRow();

                            dataRow["_RowNumber"] = _rowNumber++;

                            if (_includeListData)
                            {
                                AddListData(dataRow, web, list, item);
                            }

                            try
                            {
                                #region Fields

                                foreach (string fieldName in _fieldsList)
                                {
                                    AddFieldData(dataRow, item, fieldName);
                                }

                                #endregion
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                SendError(new SPSErrorArgs(ex.TargetSite.Name, GetResourceString("SPS_Err_AddData"), ex));
                            }

                            table.Rows.Add(dataRow);
                            _recordCounter++;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No results in " + list.Title);
                }
            }
            catch (SPException ex)
            {
                SendError(new SPSErrorArgs(ex.TargetSite.Name, GetResourceString("SPS_Err_AddFieldContent"), ex));
            }
            catch (Exception ex)
            {
                SendError(new SPSErrorArgs(ex.TargetSite.Name, GetResourceString("SPS_Err_AddFieldContent"), ex));
            }
        }

        /// <summary>
        /// Maxes the records.
        /// </summary>
        /// <returns></returns>
        private bool MaxRecordsNotExceed()
        {
            return _maxRecords == 0
                   || (_maxRecords != 0 && _recordCounter < _maxRecords);
        }

        /// <summary>
        /// Adds the field data to the result table
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        private void AddFieldData(DataRow dataRow, SPItem item, string fieldName)
        {
            try
            {
                if (item.Fields.ContainsField(fieldName))
                {
                    dataRow[fieldName] = GetFieldData(item, fieldName);
                }
                else
                {
                    dataRow[fieldName] = String.Empty;
                }
            }
            catch (ArgumentException ex)
            {
                SendError(new SPSErrorArgs(ex.TargetSite.Name,
                                           string.Format(GetResourceString("SPS_Err_FieldNotFound"), fieldName),
                                           ex));
            }
        }

        /// <summary>
        /// Gets and ensure the field data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The field content</returns>
        private string GetFieldData(SPItem item, string fieldName)
        {
            string fieldData = string.Empty;

            if (item[fieldName] != null)
            {
                fieldData = SPEncode.HtmlDecode(item[fieldName].ToString());

                // Dates in ISO8601
                if (_bDateTimeISO8601
                    && item.Fields.GetField(fieldName).Type == SPFieldType.DateTime)
                {
                    fieldData = SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                            DateTime.Parse(fieldData, CultureInfo.InvariantCulture).ToUniversalTime().ToLocalTime());
                }
                else
                {
                    // fix: lookup fields
                    if (_bFixLookUp)
                    {
                        if (fieldData.IndexOf(LOOKUP_FIELD_SEPARATOR) > 0)
                        {
                            fieldData = fieldData.Substring(fieldData.IndexOf(LOOKUP_FIELD_SEPARATOR) + 2);
                        }
                    }
                }
            }
            return fieldData;
        }

        /// <summary>
        /// Adds the list data.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        private static void AddListData(DataRow dataRow, SPWeb web, SPList list, SPItem item)
        {
            dataRow["_ListTitle"] = list.Title;
            dataRow["_ListUrl"] = list.DefaultViewUrl;
            dataRow["_ListId"] = list.ID.ToString("B");
            dataRow["_SiteTitle"] = web.Title;
            dataRow["_SiteUrl"] = web.Url;
            dataRow["_ItemId"] = item.ID;

            //SPFormCollection forms = list.Forms;

            //dataRow["_ItemUrl"] = string.Format("{0}?ID={1}", forms[PAGETYPE.PAGE_DISPLAYFORM].Url, item.ID);
            //dataRow["_ItemEdit"] = string.Format("{0}?ID={1}", forms[PAGETYPE.PAGE_EDITFORM].Url, item.ID);

            //TODO - ver como poner las urls para el calendar

            string baseViewUrl;
            baseViewUrl = list.DefaultViewUrl.Substring(0, list.DefaultViewUrl.LastIndexOf('/'));
            dataRow["_ItemUrl"] = string.Format("{0}/DispForm.aspx?ID={1}", baseViewUrl, item.ID);
            dataRow["_ItemEdit"] = string.Format("{0}/EditForm.aspx?ID={1}", baseViewUrl, item.ID);
        }

        private SPListItemCollection GetListItems(SPList list)
        {
            Debug.WriteLine("GetListItems " + list.Title);
            Debug.WriteLine(CamlQuery);
            SPListItemCollection items;
            try
            {
                if (!string.IsNullOrEmpty(CamlQuery))
                {
                    SPQuery query = GetSPQuery(CamlQuery);
                    items = list.GetItems(query);
                }
                else
                {
                    items = list.Items;
                }
                return items;
            }
            catch (SPException ex)
            {
                SendError(new SPSErrorArgs(ex.TargetSite.Name, GetResourceString("SPS_Err_Query"), ex));
            }
            finally
            {
                items = null;
            }
            return items;
        }

        // (<[^>]+>)  remove html tags.

        /// <summary>
        /// Traps the error messages from subsystems.
        /// </summary>
        /// <param name="args">The args.</param>
        private void SendError(SPSErrorArgs args)
        {
            if (OnError != null)
            {
                OnError(this, args);
            }
            DumpException(args.UserMessage, args.InternalException);
        }

        #endregion

        #region Debug

        [Conditional("DEBUG")]
        private static void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        #endregion

        #region Localization

        public static string GetResourceString(string key)
        {
            const string resourceClass = "SPSProfessional.SharePoint.WebParts.RollUp";
            const string resources = "$Resources:";
            uint lang = SPContext.Current.Web.Language;
            string value = SPUtility.GetLocalizedString(resources + key, resourceClass, lang);
            return value;
        }

        #endregion

        #region PARSE QUERY

        /// <summary>
        /// Gets the SP query.
        /// </summary>
        /// <param name="camlQuery">The caml query.</param>
        /// <returns>The SPQuery object with the options</returns>
        private SPQuery GetSPQuery(string camlQuery)
        {
            const string CAML_Recursive_Attribute = "Scope='Recursive' ";
            SPQuery query = new SPQuery
                                {
                                        Query = camlQuery
                                };

            // add Recursive 
            if (_camlQueryRecursive)
            {
                query.ViewAttributes = CAML_Recursive_Attribute;
            }

            //// add Recurrence
            //if (_bQueryRecurrence)
            //{
            //    query.ViewAttributes += " RecurrenceRowset='TRUE' ";
            //}

            //// add Moderator
            //if (_bQueryModerator)
            //{
            //    query.ViewAttributes += " ModerationType='Moderator' ";
            //}

            //// add RowLimit
            //if (_sQueryRowLimit.Length > 0)
            //{
            //    query.RowLimit = Convert.ToUInt32(_sQueryRowLimit);
            //}

            query.IncludeMandatoryColumns = true;

            return query;
        }

        #endregion
    }
}