using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Comms;

namespace SPSProfessional.SharePoint.WebParts.RollUp.Engine
{
    internal class SPSRollUpData
    {
        private DataSet _dataSet;
        private DataTable _dataTable;
        private DataRow _dataRow;
        private int _rowNumber;
        private readonly bool _includeListData;
        private readonly IEnumerable<string> _fieldList;
        private readonly string _sortFields;

        public bool HasResults
        {
            get { return _dataTable.Rows.Count > 0; }
        }

        public int RowNumber
        {
            get { return _rowNumber; }
        }

        public SPSRollUpData(bool includeListData, IEnumerable<string> fieldList)
        {
            _includeListData = includeListData;
            _fieldList = fieldList;            
            InitializeDataSet();
        }

        public SPSRollUpData(bool includeListData, IEnumerable<string> fieldList, string sortFields) : this(includeListData,fieldList)
        {
            _sortFields = sortFields;
        }

        public void InitializeDataSet()
        {
            _dataSet = new DataSet("Rows");
            _dataTable = new DataTable("Row");
            var columnType = typeof(string);

            if (_includeListData)
            {
                _dataTable.Columns.Add(new DataColumn("_ListTitle", columnType));
                _dataTable.Columns.Add(new DataColumn("_ListUrl", columnType));
                _dataTable.Columns.Add(new DataColumn("_ListId", columnType));
                _dataTable.Columns.Add(new DataColumn("_SiteTitle", columnType));
                _dataTable.Columns.Add(new DataColumn("_SiteUrl", columnType));
                _dataTable.Columns.Add(new DataColumn("_ItemId", columnType));
                _dataTable.Columns.Add(new DataColumn("_ItemUrl", columnType));
                _dataTable.Columns.Add(new DataColumn("_ItemEdit", columnType));
            }

            _dataTable.Columns.Add(new DataColumn("_RowNumber", columnType));

            Debug.WriteLine("InitializeDataSet ");

            foreach (string columnName in _fieldList)
            {
                Debug.WriteLine(columnName);

                try
                {
                    _dataTable.Columns.Add(new DataColumn(columnName, columnType));
                }
                catch(DuplicateNameException)
                {
                    Debug.WriteLine("DuplicateNameException");
                }
            }

            _dataSet.Tables.Add(_dataTable);
            _rowNumber = 0;

            //_dataSet.SchemaSerializationMode = SchemaSerializationMode.ExcludeSchema;
            _dataSet.EnforceConstraints = false;
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns>The schema</returns>
        public SPSSchemaValue GetSchema()
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();

            // Generate Schema
            foreach (DataColumn column in _dataTable.Columns)
            {
                schemaValue.AddParameter(column.ColumnName, column.DataType);
            }
            return schemaValue;
        }

        /// <summary>
        /// Gets the row data.
        /// </summary>
        /// <returns>Contains the schema if no data, otherwise data and schema</returns>
        public SPSKeyValueList GetRowValues(int? selectedRow)
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();

            // Generate Data
            if (selectedRow != null && HasResults)
            {
                DataRowView rowView = _dataTable.DefaultView[selectedRow.Value];

                foreach (DataColumn column in _dataTable.Columns)
                {
                    keyValues.Add(column.ColumnName, rowView[column.ColumnName].ToString());
                }
            }

            return keyValues;
        }


        public void AddRow()
        {
            _dataRow = _dataTable.NewRow();
            _dataRow["_RowNumber"] = _rowNumber++;
        }

        public void SetRowValue(string column, string value)
        {
            _dataRow[column] = value;
        }

        public void SaveRow()
        {
            _dataTable.Rows.Add(_dataRow);
        }

        public string GetXml()
        {
            Sort();
            return _dataSet.GetXml();
        }

        public DataView GetView()
        {
            return _dataTable.DefaultView;
        }

        private void Sort()
        {
            if (!string.IsNullOrEmpty(_sortFields))
            {
                _dataTable.DefaultView.Sort = _sortFields;
                _dataSet.Tables.Clear();
                _dataTable = _dataTable.DefaultView.ToTable();
                _dataSet.Tables.Add(_dataTable);                
            }
        }
        
    }
}
