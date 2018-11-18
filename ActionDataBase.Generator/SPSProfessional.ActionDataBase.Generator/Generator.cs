using System.Data.SqlClient;

namespace SPSProfessional.ActionDataBase.Generator
{
    internal sealed class Generator
    {
        private static readonly Generator _instance = new Generator();
        private string _connectionString;
        private string _tableName;

        private Generator()
        {
        }

        public SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }           
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public static Generator GetGenerator()
        {
            return _instance;
        }
    }
}