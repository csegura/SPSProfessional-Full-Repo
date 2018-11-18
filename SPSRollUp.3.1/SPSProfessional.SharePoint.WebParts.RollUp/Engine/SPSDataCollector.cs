using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Comms;

namespace SPSProfessional.SharePoint.WebParts.RollUp.Engine
{
    internal class SPSDataCollector
    {
        private readonly SPSRollUpData _rollUpData;
        private readonly SPSCrawler _crawler;
        private readonly SPSRollUpOptions _options;
        private readonly CamlPreprocessor _camlPreprocessor;
        private string _camlQuery;

        public SPSDataCollector(SPSRollUpOptions options, CamlPreprocessor camlPreprocessor)
        {
            _options = options;
            _camlPreprocessor = camlPreprocessor;

            _rollUpData = new SPSRollUpData(_options.IncludeListData, _options.FieldsList,_options.SortFields);
            _crawler = new SPSCrawler(_options.TopSite, _options.ListsList, _options.CamlQueryRecursive);
            _crawler.Collector += CollectData;
        }

        public void Collect()
        {
            _camlQuery = _camlPreprocessor.Evaluate();
            _crawler.Crawl();
        }
        
        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns>The schema</returns>
        public SPSSchemaValue GetSchema()
        {
            return _rollUpData.GetSchema();
        }

        /// <summary>
        /// Gets the row data.
        /// </summary>
        /// <returns>Contains the schema if no data, otherwise data and schema</returns>
        public SPSKeyValueList GetRowValues(int? selectedRow)
        {
            return _rollUpData.GetRowValues(selectedRow);
        }

        public string GetXml()
        {
            return _rollUpData.GetXml();
        }

        public DataView GetView()
        {
            return _rollUpData.GetView();
        }


        /// <summary>
        /// Collectors the specified web.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        /// <exception cref="SPSDataCollectorException"><c>SPSDataCollectorException</c>.</exception>
        private void CollectData(SPWeb web, SPList list)
        {
            Debug.WriteLine(string.Format("CollectData {0} - {1}", web.Title, list.Title));
            Debug.Assert(list != null);

            // CAML Query
            SPListItemCollection items = GetListItems(list);

            if (items == null)
            {
                Debug.WriteLine("No results in " + list.Title);
            }
            else
            {
                try
                {
                    Debug.WriteLine(string.Format("{0} Items", items.Count));
                    foreach (SPListItem item in items)
                    {
                        if (MaxRecordsExceed())
                        {
                            break;
                        }

                        _rollUpData.AddRow();

                        if (_options.IncludeListData)
                        {
                            AddListData(list, item);
                        }

                        foreach (string fieldName in _options.FieldsList)
                        {
                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                _rollUpData.SetRowValue(fieldName, GetFieldData(item, fieldName));
                            }
                        }

                        _rollUpData.SaveRow();
                    }
                }
                catch (Exception ex)
                {
                    throw new SPSDataCollectorException(SPSResources.GetString("SPS_Err_AddFieldContent"), ex);
                }
            }
        }

        /// <summary>
        /// Maxes the records.
        /// </summary>
        /// <returns></returns>
        private bool MaxRecordsExceed()
        {
            return (_options.MaxRecords != 0 && _rollUpData.RowNumber > _options.MaxRecords);
        }

        /// <summary>
        /// Adds the list data.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        private void AddListData(SPList list, SPItem item)
        {
            //_rollUpData.SetRowValue("_ListTitle", list.Title);
            _rollUpData.SetRowValue("_ListTitle", list.Title);
            _rollUpData.SetRowValue("_ListUrl", list.DefaultViewUrl);
            _rollUpData.SetRowValue("_ListId", list.ID.ToString("B"));
            _rollUpData.SetRowValue("_SiteTitle", list.ParentWeb.Title);
            _rollUpData.SetRowValue("_SiteUrl", list.ParentWeb.Url);
            _rollUpData.SetRowValue("_ItemId", item.ID.ToString());

            //TODO - ver como poner las urls para el calendar

            string baseViewUrl;
            baseViewUrl = list.DefaultViewUrl.Substring(0, list.DefaultViewUrl.LastIndexOf('/'));

            _rollUpData.SetRowValue("_ItemUrl", string.Format("{0}/DispForm.aspx?ID={1}", baseViewUrl, item.ID));
            _rollUpData.SetRowValue("_ItemEdit", string.Format("{0}/EditForm.aspx?ID={1}", baseViewUrl, item.ID));
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

            if (ItemContainsFieldAndData(item, fieldName))
            {
                try
                {
                    fieldData = SPEncode.HtmlDecode(item[fieldName].ToString());
                    fieldData = ProcessFieldData(item, fieldName, fieldData);
                }
                catch(FormatException ex)
                {
                    Debug.WriteLine(fieldName);
                    Debug.WriteLine("GetFieldData " + ex);   
                }                
            }
            Debug.WriteLine("GetFieldData " + fieldName + "="+fieldData);
            return fieldData;
        }

        /// <summary>
        /// Processes the field data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldData">The field data.</param>
        /// <returns></returns>
        private string ProcessFieldData(SPItem item, string fieldName, string fieldData)
        {
            const string LOOKUP_FIELD_SEPARATOR = ";#";
            // Dates in ISO8601
            SPField field = item.Fields.GetField(fieldName);

            if (_options.UseDateISO && field.Type == SPFieldType.DateTime)
            {
                fieldData = SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                        DateTime.Parse(fieldData, CultureInfo.InvariantCulture).ToUniversalTime().ToLocalTime());
            }
            else
            {
                // fix: lookup fields
                if (_options.FixLookups)
                {
                    if (fieldData.IndexOf(LOOKUP_FIELD_SEPARATOR) > 0)
                    {
                        fieldData = fieldData.Substring(fieldData.IndexOf(LOOKUP_FIELD_SEPARATOR) + 2);
                    }
                }
            }
            return fieldData;
        }

        /// <summary>
        /// Items the contains field and data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private bool ItemContainsFieldAndData(SPItem item, string fieldName)
        {
            if (item.Fields.ContainsField(fieldName))
            {
                if (item[fieldName] != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the list items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        /// <exception cref="SPSDataCollectorException"><c>SPSDataCollectorException</c>.</exception>
        private SPListItemCollection GetListItems(SPList list)
        {
            Debug.WriteLine("GetListItems " + list.Title);
            Debug.WriteLine("Query (" + _camlQuery + ")");

            SPListItemCollection items;

            if (!string.IsNullOrEmpty(_camlQuery))
            {
                SPQuery query = GetSPQuery(_camlQuery);
                items = list.GetItems(query);
                items = CheckItemsAfterQuery(items);
            }
            else
            {
                items = list.Items;
            }
            return items;
        }

        public SPListItemCollection CheckItemsAfterQuery(SPListItemCollection items)
        {
            try
            {
                // Check Query
                if (items.Count > 0)
                {
                    return items;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                items = null;
                // throw new SPSDataCollectorException(SPSResources.GetString("SPS_Err_Query"), ex);
            }

            return items;
        }

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
            if (_options.CamlQueryRecursive)
            {
                query.ViewAttributes = CAML_Recursive_Attribute;
            }

            query.IncludeMandatoryColumns = true;

            return query;
        }
    }
}