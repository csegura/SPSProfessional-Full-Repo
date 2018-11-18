using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Controls;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.Relations
{
    /// <summary>
    /// DataBase relations are managed with ActionEntity, this class manage 
    /// the DataBase operations for the ActionEntities 
    /// </summary>
    internal class ActionEntityDataBase : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly ActionEntity _actionEntity;

        private const string SelectKeyValue = "SELECT {0}, {1} FROM {2}";
        private const string SelectKeyValueForKey = "SELECT {1} FROM {2} WHERE {0}=@{0}";
        private const string SelectKeyValueLikeValue = "SELECT {0}, {1} FROM {2} WHERE {1} LIKE '%{3}%'";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEntityDataBase"/> class.
        /// </summary>
        /// <param name="actionEntity">The action entity.</param>
        public ActionEntityDataBase(ActionEntity actionEntity)
        {
            _actionEntity = actionEntity;
            _connection = new SqlConnection(actionEntity.Connection);
        }

        #endregion

        /// <summary>
        /// Return all records that match the pattern
        /// </summary>
        /// <param name="pattern">The text pattern</param>
        /// <returns>DataSet with the data that match</returns>
        private DataSet InternalDBMatch(string pattern)
        {
            SqlDataAdapter dataAdapter;
            DataSet dataSet = null;

            string sqlQuery = string.Format(SelectKeyValueLikeValue,
                                            _actionEntity.Key,
                                            _actionEntity.Value,
                                            _actionEntity.Table,
                                            pattern);
            try
            {
                _connection.Open();
                dataAdapter = new SqlDataAdapter(sqlQuery, _connection);
                dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                _connection.Close();
            }
            catch (Exception ex)
            {
                DumpException("InternalDBMatch", ex);
            }

            return dataSet;
        }

        /// <summary>
        /// Resolver for a key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>the value for the key</returns>
        private string InternalDBResolver(string key)
        {
            string value = null;

            if (!string.IsNullOrEmpty(key))
            {
                string sqlQuery = string.Format(SelectKeyValueForKey,
                                                _actionEntity.Key,
                                                _actionEntity.Value,
                                                _actionEntity.Table);

                Debug.WriteLine(sqlQuery);

                try
                {
                    _connection.Open();

                    SqlCommand sqlCommand = new SqlCommand(sqlQuery, _connection);
                    SqlDbType dbType = (SqlDbType) Enum.Parse(typeof(SqlDbType), _actionEntity.KeyDbType, true);
                    Debug.WriteLine(dbType);
                    SqlParameter parameter = new SqlParameter("@" + _actionEntity.Key, dbType)
                                             {
                                                     Value = AdjustParameter(dbType, key)
                                             };

                    sqlCommand.Parameters.Add(parameter);

                    SqlDataReader dataReader = sqlCommand.ExecuteReader();

                    if (dataReader != null)
                    {
                        if (dataReader.HasRows)
                        {
                            dataReader.Read();
                            value = dataReader[0].ToString();
                            dataReader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    DumpException("ResolvePickerEntity", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
            return value;
        }

        /// <summary>
        /// Decorate a DropDownList to select the current entity
        /// </summary>
        /// <param name="dropDonwList">The drop donw list.</param>
        public void BindDropDown(DropDownList dropDonwList)
        {
            SqlDataAdapter dataAdapter;
            DataSet dataSet;
            string sqlCommand;

            sqlCommand = string.Format(SelectKeyValue,
                                       _actionEntity.Key,
                                       _actionEntity.Value,
                                       _actionEntity.Table);

            Debug.WriteLine(sqlCommand);

            dataAdapter = new SqlDataAdapter(sqlCommand, _connection);
            dataSet = new DataSet();
            dataAdapter.Fill(dataSet, _actionEntity.Table);

            dropDonwList.DataSource = dataSet;

            dropDonwList.DataTextField = GetFieldName(_actionEntity.Value);
            dropDonwList.DataValueField = _actionEntity.Key;
            dropDonwList.DataBind();
        }

        private string GetFieldName(string fieldName)
        {
            if (fieldName.ToUpper().Contains(" AS "))
            {
                return fieldName.Substring(fieldName.ToUpper().LastIndexOf(" AS ") + 4);
            }
            return fieldName;
        }

        /// <summary>
        /// Decorate a ActionEntityPickerControl to select the current entity
        /// </summary>
        /// <param name="actionPicker">The action picker.</param>
        public void BindActionPicker(ActionEntityPickerControl actionPicker)
        {
            PickerEntity pickerEntity = new PickerEntity
                                        {
                                                Key = _actionEntity.CurrentKey,
                                                DisplayText = _actionEntity.Value,
                                                IsResolved = false
                                        };

            ArrayList list = new ArrayList
                             {
                                     pickerEntity
                             };

            actionPicker.UpdateEntities(list);
        }

        /// <summary>
        /// Matcheses the entities.
        /// </summary>
        /// <param name="query">The text.</param>
        /// <returns>List of Picker entities tat match the passed query</returns>
        public List<PickerEntity> MatchesEntities(string query)
        {
            List<PickerEntity> list = new List<PickerEntity>();
            DataSet dataSet = InternalDBMatch(query);

            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    PickerEntity pickerEntity = new PickerEntity
                                                {
                                                        Key = row[0].ToString(),
                                                        Description = row[1].ToString()
                                                };
                    pickerEntity.DisplayText = pickerEntity.Description;

                    Debug.WriteLine(string.Format("* {0},{1}", pickerEntity.Key, pickerEntity.Description));
                    list.Add(pickerEntity);
                }
            }
            return list;
        }

        /// <summary>
        /// Matcheses the table.
        /// </summary>
        /// <param name="query">The text.</param>
        /// <returns>DataTable Key  Value pairs that match the query</returns>
        public DataTable MatchesTable(string query)
        {
            DataSet dataSet = InternalDBMatch(query);
            DataTable table = null;

            if (dataSet != null)
            {
                table = dataSet.Tables[0].DefaultView.ToTable();
                table.Columns[0].ColumnName = "Key";
                table.Columns[1].ColumnName = "Value";
            }

            return table;
        }

        /// <summary>
        /// Resolves the picker entity.
        /// </summary>
        /// <param name="needsValidation">The needs validation.</param>
        public void ResolvePickerEntity(PickerEntity needsValidation)
        {
            string value = InternalDBResolver(needsValidation.Key);

            if (value != null)
            {
                needsValidation.Description = value;
                needsValidation.DisplayText = value;
                needsValidation.IsResolved = true;
            }
        }

        /// <summary>
        /// Resolves the action entity.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value for the key</returns>
        public string ResolveActionEntity(string key)
        {
            return InternalDBResolver(key);
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
                    value = new Guid(value.ToString());
                    break;
            }
            return value;
        }

        #region Debug

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                }
            }
        }

        #endregion
    }
}