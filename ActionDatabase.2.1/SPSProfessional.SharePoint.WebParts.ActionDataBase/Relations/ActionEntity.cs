using System;
using System.Data;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.Relations
{
    [Serializable]
    public class ActionEntity
    {
        private string _value;
        private string _key;
        private string _keyDbType;
        private string _table;
        private string _currentKey;
        private string _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEntity"/> class.
        /// </summary>
        public ActionEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEntity"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        /// <param name="table">The table.</param>
        /// <param name="currentKey">The current key.</param>
        /// <param name="connection">The connection.</param>
        public ActionEntity(string value, string key, string table, string currentKey, IDbConnection connection)
        {
            _value = value;
            _key = key;
            _table = table;
            _currentKey = currentKey;
            _connection = connection.ConnectionString;            
        }

        public ActionEntity(Lookup lookupDefinition, string currentKey, IDbConnection connection)
        {
            _value = lookupDefinition.TextField;
            _key = lookupDefinition.ValueField;
            _table = lookupDefinition.Table;
            _keyDbType = lookupDefinition.ValueFieldType.ToString();
            _currentKey = currentKey;
            _connection = connection.ConnectionString;                
        }

        #region Properties

        internal string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        internal string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        internal string Table
        {
            get { return _table; }
            set { _table = value; }
        }

        internal string Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        internal string CurrentKey
        {
            get { return _currentKey; }
            set { _currentKey = value; }
        }

        public string KeyDbType
        {
            get { return _keyDbType; }
            set { _keyDbType = value; }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4}", _key, _value, _table, _currentKey, _connection);
        }
    }
}