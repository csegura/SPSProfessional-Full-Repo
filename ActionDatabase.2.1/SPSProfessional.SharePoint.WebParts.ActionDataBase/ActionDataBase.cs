using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal sealed class ActionDataBase : ISPSErrorControl, IDisposable
    {
        private const string SPSSubSystemName = "ActionDataBase";

        private readonly SPSActionEditConfig _config;
        private SqlDataAdapter _adapter;
        private SqlCommandBuilder _builder;
        private SqlConnection _connection;
        private DataRow _dataRow;
        private DataSet _dataSet;
        private Dictionary<IdentityColumn, string> _identityColumnValue;
        private readonly string _connectionString;
        private readonly string _tableName;

        #region Constructor

        public ActionDataBase(SPSActionEditConfig config)
        {
            _config = config;
            _connectionString = _config.DataBase.ConnectionString;
            _tableName = _config.DataBase.Table.Name;
        }

        public ActionDataBase(string connectionString, string tableName)
        {
            _tableName = tableName;
            _connectionString = connectionString;
        }

        #endregion

        #region Properties

        public DataRow DataRow
        {
            get { return _dataRow; }
        }

        public SqlConnection Connection
        {
            get { return _connection; }
        }

        #endregion

        #region ISPSErrorControl Members

        public event SPSControlOnError OnError;

        #endregion

        #region Engine

        /// <summary>
        /// Initializes the connection.
        /// Required after create the ActionDataBase
        /// TODO: move to Connection property ???
        /// </summary>
        internal void InitializeConnection()
        {
            try
            {
                _connection = new SqlConnection(_connectionString);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, "Error database connection.", ex));
                }

                DumpException("InitializeDataSet", ex);
            }
        }

        /// <summary>
        /// Initializes the data set.
        /// </summary>
        internal void InitializeDataSet()
        {
            try
            {
                _connection.Open();
                _adapter = new SqlDataAdapter(string.Empty, _connection)
                           {
                                   MissingSchemaAction = MissingSchemaAction.Ignore
                           };
                _dataSet = new DataSet();
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, "Error database initialization.", ex));
                }

                DumpException("InitializeDataSet", ex);
            }
            finally
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Selects the record.
        /// </summary>
        /// <param name="filterValues">The filter values.</param>
        /// <param name="addError">if set to <c>true</c> add the error to error subsystem.</param>
        internal int SelectRecord(SPSKeyValueList filterValues, bool addError)
        {
            Debug.WriteLine("SelectRecord");
            int recordCount = 0;

            try
            {
                SqlCommand sqlCommand = new SqlCommand(PrepareSelectFields() +
                                                       PrepareSelectForFilter(),
                                                       _connection);

                AddFilterParameters(sqlCommand, filterValues, false);

                Debug.WriteLine(sqlCommand.CommandText);

                _adapter = new SqlDataAdapter(sqlCommand);

                // Read
                recordCount = _adapter.Fill(_dataSet, _tableName);

                // The current data
                if (recordCount > 0)
                {
                    _dataRow = _dataSet.Tables[0].Rows[0];
                }
            }
            catch (Exception ex)
            {
                if (OnError != null && addError)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, "Error selecting record.", ex));
                }

                DumpException("SelectRecord", ex);
            }
            finally
            {
                _connection.Close();
            }

            Debug.WriteLine("SelectRecord End");
            return recordCount;
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        /// <param name="filterValues">The filter values.</param>
        /// <returns>Rows affected</returns>
        internal void DeleteRecord(SPSKeyValueList filterValues)
        {
            Debug.WriteLine("DeleteRecord");

            try
            {
                InitializeDataSet();
                SelectRecord(filterValues, true);

                _adapter.MissingSchemaAction = MissingSchemaAction.Ignore;
                _builder = new SqlCommandBuilder(_adapter)
                           {
                                   ConflictOption = ConflictOption.OverwriteChanges
                           };

                SqlCommand command = new SqlCommand(_builder.GetDeleteCommand(true).CommandText,
                                                    _connection);

                AddFilterParameters(command, filterValues, true);

                Debug.WriteLine(command.CommandText);

                _connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, "Error deleting record.", ex));
                }

                DumpException("DeleteRecord", ex);
            }
            finally
            {
                _connection.Close();
            }

            Debug.WriteLine("DeleteRecord End");
        }

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <param name="fieldValues">The field values.</param>
        /// <param name="filterValues">The filter values.</param>
        /// <returns>Rows affected</returns>
        internal int UpdateRecord(IDictionary<string, string> fieldValues, SPSKeyValueList filterValues)
        {
            Debug.WriteLine("UpdateRecord");

            int countRecords = 0;

            try
            {
                InitializeDataSet();
                SelectRecord(filterValues, true);

                _adapter.MissingSchemaAction = MissingSchemaAction.Ignore;
                _builder = new SqlCommandBuilder(_adapter)
                           {
                                   ConflictOption = ConflictOption.OverwriteChanges
                           };

                SqlCommand command = new SqlCommand(_builder.GetUpdateCommand(true).CommandText,
                                                    _connection);

                Debug.WriteLine(command.CommandText);

                AddDataParameters(command, fieldValues);
                AddFilterParameters(command, filterValues, true);

                _connection.Open();
                countRecords = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, "Error updating record.", ex));
                }

                DumpException("UpdateRecord", ex);
            }
            finally
            {
                _connection.Close();
            }

            Debug.WriteLine("UpdateRecord End");
            return countRecords;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="fieldValues">The field values.</param>
        /// <param name="filterValues">The filter values.</param>
        /// <returns>New record identity</returns>
        internal int CreateRecord(IDictionary<string,string> fieldValues, SPSKeyValueList filterValues)
        {
            Debug.WriteLine("CreateRecord");

            int createdRows = 0;

            try
            {
                InitializeDataSet();
                PrepareSchema();

                _adapter.MissingSchemaAction = MissingSchemaAction.Ignore;
                _builder = new SqlCommandBuilder(_adapter)
                           {
                                   ConflictOption = ConflictOption.OverwriteChanges
                           };

                SqlCommand command = new SqlCommand(_builder.GetInsertCommand(true).CommandText,
                                                    _connection);

                Debug.WriteLine(command.CommandText);

                AddDataParameters(command, fieldValues);

                _connection.Open();
                createdRows = command.ExecuteNonQuery();


                if (createdRows > 0)
                {
                    // Try to get the last added record 
                    TryGetLastAddedRecord(fieldValues, filterValues);
                }
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, "Error creating new record.", ex));
                }

                DumpException("CreateRecord", ex);
            }
            finally
            {
                _connection.Close();
            }

            Debug.WriteLine("CreateRecord End");
            return createdRows;
        }

        /// <summary>
        /// Tries the get last added record.
        /// </summary>
        /// <param name="fieldValues">The field values.</param>
        /// <param name="filterValues">The filter values.</param>
        private void TryGetLastAddedRecord(IDictionary<string,string> fieldValues, SPSKeyValueList filterValues)
        {
            try
            {
                // Try to get identity
                SqlCommand command = new SqlCommand("SELECT @@IDENTITY", _connection);
                _adapter = new SqlDataAdapter(command);
                string key = command.ExecuteScalar().ToString();

                // If there is a key
                if (!string.IsNullOrEmpty(key))
                {
                    // Replace the key in the current filter
                    filterValues.ReplaceValue(_config.DataBase.Table.IdentityColumnCollection[0].Name, key);
                }
                else
                {
                    // Set the new filter vales
                    SetNewFilterValues(fieldValues, filterValues);
                }

                // Select the added record using the new filter
                if (filterValues != null)
                {
                    SelectRecord(filterValues, false);
                }
            }
            catch (SqlException ex)
            {
                DumpException("TryGetLastAddedRecord", ex);
            }
        }

        #region OLD CODE

        ///// <summary>
        ///// Determines whether [is identity column] [the specified column name].
        ///// </summary>
        ///// <param name="columnName">Name of the column.</param>
        ///// <returns>
        ///// 	<c>true</c> if [is identity column] [the specified column name]; otherwise, <c>false</c>.
        ///// </returns>
        //private bool IsIdentityColumn(string columnName)
        //{
        //    Debug.Assert(columnName != null);
        //    Debug.Assert(_config != null);

        //    foreach (IdentityColumn column in _config.DataBase.Table.IdentityColumnCollection)
        //    {
        //        if (column.Name == columnName)
        //        {
        //            bool isNumeric = (column.Type == ActionDataBaseSqlType.Int) ||
        //                             (column.Type == ActionDataBaseSqlType.BigInt) ||
        //                             (column.Type == ActionDataBaseSqlType.SmallInt) ||
        //                             (column.Type == ActionDataBaseSqlType.TinyInt);

        //            bool treatAsIdentity;

        //            if (isNumeric && !column.Incremental)
        //            {
        //                treatAsIdentity = false;
        //            }
        //            else
        //            {
        //                treatAsIdentity = true;
        //            }

        //            return treatAsIdentity;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// Determines whether [is read only] [the specified column name].
        ///// </summary>
        ///// <param name="columnName">Name of the column.</param>
        ///// <returns>
        ///// 	<c>true</c> if [is read only] [the specified column name]; otherwise, <c>false</c>.
        ///// </returns>
        //private bool IsReadOnly(string columnName)
        //{
        //    Debug.Assert(columnName != null);
        //    Debug.Assert(_config != null);

        //    foreach (Field field in _config.Fields)
        //    {
        //        if (field.Name == columnName &&
        //            (field.New == ActionEditorFieldVisibility.Disabled ||
        //             field.Edit == ActionEditorFieldVisibility.Disabled))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        #endregion


        /// <summary>
        /// Sets the new filter values.
        /// </summary>
        /// <param name="fieldValues">The field values.</param>
        /// <param name="filterValues">The filter values.</param>
        private void SetNewFilterValues(IDictionary<string,string> fieldValues, IEnumerable<SPSKeyValuePair> filterValues)
        {
            foreach (SPSKeyValuePair keyValue in filterValues)
            {
                keyValue.Value = fieldValues[keyValue.Key];
            }
        }

        /// <summary>
        /// Prepares the select command.
        /// </summary>
        /// <returns></returns>
        private string PrepareSelect()
        {
            Debug.Assert(_config != null);

            string sqlSelect = string.Format("SELECT * FROM {0}",
                                             AdjustFieldOrTable(_config.DataBase.Table.Name));
            return sqlSelect;
        }

        /// <summary>
        /// Prepares the select command using config fields list.
        /// </summary>
        /// <returns></returns>
        private string PrepareSelectFields()
        {
            string fieldList = string.Empty;

            Debug.Assert(_config != null);

            foreach (Field field in _config.Fields)
            {
                fieldList += string.Format("{0},", AdjustFieldOrTable(field.Name));
            }

            return PrepareSelect().Replace("*", fieldList.TrimEnd(','));
        }

        /// <summary>
        /// Prepares the where clause for the select statement.
        /// </summary>
        /// <returns></returns>
        private string PrepareSelectForFilter()
        {
            string sqlWhere = " WHERE";

            Debug.Assert(_config != null);

            foreach (IdentityColumn column in _config.DataBase.Table.IdentityColumnCollection)
            {
                sqlWhere += string.Format(" ([{0}]=@{0}) AND", column.Name);
            }

            // remove last AND
            sqlWhere = sqlWhere.Remove(sqlWhere.Length - 4, 4);

            return sqlWhere;
        }

        /// <summary>
        /// Setup the adapter with table schema
        /// </summary>
        private void PrepareSchema()
        {
            Debug.WriteLine("PrepareSchema");

            try
            {
                SqlCommand sqlCommand = new SqlCommand(PrepareSelectFields(),
                                                       _connection);

                Debug.WriteLine(sqlCommand.CommandText);

                _adapter = new SqlDataAdapter(sqlCommand);
                _adapter.FillSchema(_dataSet, SchemaType.Source, _tableName);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, "Error selecting record.", ex));
                }

                DumpException("PrepareSchema", ex);
            }
            finally
            {
                _connection.Close();
            }

            Debug.WriteLine("End PrepareSchema");
        }

        /// <summary>
        /// Check if there are values for each identity column
        /// Values come in IfilterVales list
        /// </summary>
        /// <param name="filterValues">Filter consumer values</param>
        /// <returns></returns>
        private bool EnsureFilterParameters(IEnumerable<SPSKeyValuePair> filterValues)
        {
            bool ensure = false;
            _identityColumnValue = new Dictionary<IdentityColumn, string>();

            Debug.Assert(_config != null);

            // TODO: There is a little problem when we have two providers, then we have two filtervalues with the same key

            foreach (IdentityColumn column in _config.DataBase.Table.IdentityColumnCollection)
            {
                foreach (SPSKeyValuePair keyValue in filterValues)
                {
                    if (column.Name == keyValue.Key)
                    {
                        Debug.WriteLine(
                            string.Format("EnsureFilterParameters: Column {0} Value {1}",
                                          column.Name,
                                          keyValue.Value));

                        _identityColumnValue.Add(column, keyValue.Value);

                        break;
                    }
                }
            }

            // Check 
            if (_identityColumnValue.Count == _config.DataBase.Table.IdentityColumnCollection.Count)
            {
                ensure = true;
                Debug.WriteLine("Ensured.");
            }

            return ensure;
        }

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="fieldValues">The field values.</param>
        private void AddDataParameters(SqlCommand command, IDictionary<string,string> fieldValues)
        {
            Debug.WriteLine("AddDataParameters");
            Debug.Assert(_config != null);

            foreach (Field field in _config.Fields)
            {
                Debug.WriteLine("Add:" + field.Name);
                command.Parameters.Add(GenerateParameter(field.Name,
                                                          field.Type,
                                                          fieldValues[field.Name],
                                                          false));
            }

            Debug.WriteLine("End AddDataParameters");
        }

        /// <summary>
        /// Adds the filter parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="filterValues">The filter values.</param>
        /// <param name="original">if set to <c>true</c> [original].</param>
        private void AddFilterParameters(SqlCommand command,
                                         IEnumerable<SPSKeyValuePair> filterValues,
                                         bool original)
        {
            Debug.WriteLine("AddFilterParameters");

            // Ensure parameters
            bool ensured = EnsureFilterParameters(filterValues);

            // Ensure & Build 
            if (ensured)
            {
                // Add parameters
                foreach (KeyValuePair<IdentityColumn, string> column in _identityColumnValue)
                {
                    command.Parameters.Add(GenerateParameter(column.Key.Name,
                                                             column.Key.Type,
                                                             column.Value,
                                                             original));
                }
            }

            Debug.WriteLine("End AddFilterParameters");
        }

        /// <summary>
        /// Generates the SqlParameter.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="original">if set to <c>true</c> [original].</param>
        /// <returns></returns>
        private SqlParameter GenerateParameter(string fieldName,
                                               ActionDataBaseSqlType fieldType,
                                               object value,
                                               bool original)
        {
            SqlDbType fieldDbType = (SqlDbType) Enum.Parse(typeof (SqlDbType), fieldType.ToString(), true);

            

            Debug.WriteLine(string.Format("Adjust {0}", fieldName));

            try
            {
                value = AdjustParameter(fieldDbType, value);

                if (string.IsNullOrEmpty(value.ToString()))
                {
                    Debug.WriteLine(string.Format("Adjust for null {0}", fieldName));
                    value = AdjustParameterForNull(fieldDbType, value);
                }
            }
            catch(Exception ex)
            {
                DumpException("AdjustParameters",ex);
            }

            #region OLD CODE

            //if (string.IsNullOrEmpty(value.ToString()))
            //{
            //    switch (fieldDbType)
            //    {
            //        case SqlDbType.TinyInt:
            //        case SqlDbType.SmallInt:
            //        case SqlDbType.BigInt:
            //        case SqlDbType.Float:
            //        case SqlDbType.Int:
            //        case SqlDbType.SmallMoney:
            //        case SqlDbType.Money:
            //        case SqlDbType.Real:
            //            value = 0;
            //            break;
            //    }
            //}

            #endregion

            SqlParameter parameter = new SqlParameter
                                     {
                                             ParameterName = ((original ? "@Original_" : "@") + fieldName),
                                             SqlDbType = fieldDbType,
                                             Value = value,
                                             Direction = ParameterDirection.Input
                                     };

            Debug.WriteLine(
                string.Format("Adding param {2,-10} -> {0,15} = {1}",
                              parameter.ParameterName,
                              parameter.Value,
                              parameter.SqlDbType));

            return parameter;
        }

        /// <summary>
        /// Adjusts the parameter make a conversion previous to pass the value to database
        /// </summary>
        /// <param name="fieldDbType">Type of the field db.</param>
        /// <param name="value">The value.</param>
        /// <returns>The value in the correct format</returns>
        private object AdjustParameter(SqlDbType fieldDbType, object value)
        {
            switch (fieldDbType)
            {
                case SqlDbType.Int:
                case SqlDbType.BigInt:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                    if (value != null)
                    {
                        if (value.ToString() == "on")
                        {
                            value = 1;
                        }
                        else if (value.ToString() == "off")
                        {
                            value = 0;
                        }
                    }
                    break;
                case SqlDbType.Bit:
                    if (value != null)
                    {
                        value = (value.ToString() == "on" ? 1 : 0);
                    }
                    else
                    {
                        value = 0;
                    }
                    break;
                case SqlDbType.UniqueIdentifier:
                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        value = new Guid(value.ToString());
                    }
                    break;
            }
            return value;
        }

        /// <summary>
        /// Adjusts the parameter if its a null value
        /// </summary>
        /// <param name="fieldDbType">Type of the field db.</param>
        /// <param name="value">The value.</param>
        /// <returns>The correct null value</returns>
        private object AdjustParameterForNull(SqlDbType fieldDbType, object value)
        {
            switch (fieldDbType)
            {
                case SqlDbType.TinyInt:
                case SqlDbType.SmallInt:
                    value = SqlInt16.Null;
                    break;
                case SqlDbType.BigInt:
                    value = SqlInt64.Null;
                    break;
                case SqlDbType.Float:
                    value = SqlDouble.Null;
                    break;
                case SqlDbType.Int:
                    value = SqlInt32.Null;
                    break;
                case SqlDbType.SmallMoney:
                case SqlDbType.Money:
                    value = SqlMoney.Null;
                    break;
                case SqlDbType.Real:
                    value = SqlSingle.Null;
                    break;
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    value = SqlDateTime.Null;
                    break;
                case SqlDbType.UniqueIdentifier:
                    value = SqlGuid.Null;
                    break;
            }
            return value;
        }

        /// <summary>
        /// Adjusts the field or table adding '[' if the field or table contains spaces
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string AdjustFieldOrTable(string name)
        {
            string adjusted = name;
            char[] delimiters = {' ', '-', '!', '%', '^', '~', '&', '(', ')', '{', '}', '\'', '.', '\\'};
            

            if (adjusted.IndexOfAny(delimiters) > 0 && !adjusted.Contains("["))
            {
              adjusted = string.Format("[{0}]", name);
            }
            return adjusted;
        }

        #endregion

        public void Close()
        {
            Debug.WriteLine("### ActionDataBase CLOSE");
            Debug.WriteLine("Init:" + GC.GetTotalMemory(false));
            if (_connection != null)
            {
                _connection.Dispose();
            }
            if (_adapter != null)
            {
                _adapter.Dispose();
            }
            if (_dataSet != null)
            {
                _dataSet.Dispose();
            }
            if (_builder != null)
            {
                _builder.Dispose();
            }
            Debug.WriteLine("End:" + GC.GetTotalMemory(false));
        }

        ~ActionDataBase()
        {
            Dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion

        #region Debug

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        #endregion
    }
}