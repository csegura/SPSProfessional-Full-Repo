using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace SPSProfessional.ActionDataBase.Generator
{
    public class SqlFKPKInfo
    {
        public string PKColumnName;
        public string PKTable;
    }

    public static class SPSDbTools
    {
        private const string ERR_CONNECTING = "Error connecting to database.";

        /// <summary>
        /// 1. ServerName - Name of the server.
        /// 2. InstanceName - Name of the server instance. Blank if the server is running as the default instance.
        /// 3. IsClustered - Indicates whether the server is part of a cluster.
        /// 4. Version - Version of the server (8.00.x for SQL Server 2000, and 9.00.x for SQL Server 2005).
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetActiveServers()
        {
            Collection<string> result = new Collection<string>();
            SqlDataSourceEnumerator instanceEnumerator = SqlDataSourceEnumerator.Instance;
            DataTable instancesTable = instanceEnumerator.GetDataSources();

            foreach (DataRow row in instancesTable.Rows)
            {
                if (!string.IsNullOrEmpty(row["InstanceName"].ToString()))
                    result.Add(string.Format(@"{0}\{1}", row["ServerName"], row["InstanceName"]));
                else
                    result.Add(row["ServerName"].ToString());
            }

            return result;
        }

        public static IList<string> GetDatabases(string serverName,
                                                 string userId,
                                                 string password,
                                                 bool windowsAuthentication)

        {
            Collection<string> result = new Collection<string>();

            using (SqlConnection connection = GetActiveConnection(serverName, userId, password, windowsAuthentication))
            {
                try
                {
                    connection.Open();
                    DataTable dt = connection.GetSchema(SqlClientMetaDataCollectionNames.Databases);
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(string.Format("{0}", row[0]));
                    }
                }
                catch (SqlException ex)
                {
                    throw new SPSDbToolsException(ERR_CONNECTING, ex);
                }
            }

            return result;
        }

        private static SqlConnection GetActiveConnection(string serverName,
                                                         string userName,
                                                         string password,
                                                         bool useIntegratedSecurity)

        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
            connBuilder.DataSource = serverName;
            connBuilder.IntegratedSecurity = useIntegratedSecurity;
            connBuilder.UserID = userName;
            connBuilder.Password = password;
            return new SqlConnection(connBuilder.ConnectionString);
        }


        /// <summary>
        /// 0. database name
        /// 1. owner/schema name ("dbo")
        /// 2. table name (which should contain null value if we want to retrieve all tables of database)
        /// 3. table type (which can have values "VIEW" for views and "BASE TABLE" for tables
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IList<string> GetTables(SqlConnection connection)

        {
            string[] restrictions = new string[4];
            restrictions[0] = connection.Database; // database/catalog name
            restrictions[1] = null; // owner/schema name
            restrictions[2] = null; // table name
            restrictions[3] = "BASE TABLE"; // table type            

            return GetSchema(restrictions, connection);
        }

        /// <summary>
        /// 0. database name
        /// 1. owner/schema name ("dbo")
        /// 2. table name (which should contain null value if we want to retrieve all tables of database)
        /// 3. table type (which can have values "VIEW" for views and "BASE TABLE" for tables
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IList<string> GetViews(SqlConnection connection)
        {
            string[] restrictions = new string[4];
            restrictions[0] = connection.Database; // database/catalog name
            restrictions[1] = null; // owner/schema name
            restrictions[2] = null; // table name
            restrictions[3] = "VIEW"; // table type            

            return GetSchema(restrictions, connection);
        }

        private static IList<string> GetSchema(string[] restrictions, SqlConnection connection)
        {
            bool sql2000 = true;
       
            
            Collection<string> result = new Collection<string>();

            using (connection)
            {
                connection.Open();

                if (!connection.ServerVersion.StartsWith("08"))
                    sql2000 = false;

                DataTable dt = connection.GetSchema(SqlClientMetaDataCollectionNames.Tables);

                foreach (DataRow row in dt.Rows)
                {
                    if (!row[2].ToString().StartsWith("sys"))
                    {
                        if (sql2000)
                        {
                            result.Add(string.Format(@"{0}", row[2]));
                        }
                        else
                        {
                            result.Add(string.Format(@"[{0}].[{1}]", row[1], row[2]));
                        }
                    }
                }
            }
            return result;
        }

        public static string AdjustTable(string tableName)
        {
            string adjusted = tableName;
            if (adjusted.Contains(" "))
                 adjusted = string.Format("[{0}]", tableName);
            return adjusted;
        }

        public static SqlFKPKInfo GetFkPkInfoVersion(SqlConnection connection, string column, string table)
        {
            SqlFKPKInfo sqlFkPkInfo = null;
            bool sql2000 = true;
            string[] schemaTable = null;
            

            using (connection)
            {
                connection.Open();

                if (!connection.ServerVersion.StartsWith("08"))
                {
                    sql2000 = false;
                    schemaTable = table.Split('.');
                }

                // get the constraint - if one exists
                int id = 0;
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (sql2000)
                        command.CommandText =
                            "SELECT sysobjects.id FROM sysobjects INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE " +
                            "ON sysobjects.name = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.Constraint_Name WHERE " +
                            "(column_name = @p1) AND (Table_Name = @p2) AND (sysobjects.xtype = 'F')";
                    else
                        command.CommandText =
                            "SELECT object_id FROM sys.objects WHERE (name = (SELECT Constraint_Name FROM " +
                            "INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE (column_name=@p1) AND (Table_Name=@p2) AND (Table_Schema=@p3) )) AND (type='F')";
                    command.Parameters.Add(new SqlParameter("@p1", column));
                    if (sql2000)
                    {
                        command.Parameters.Add(new SqlParameter("@p2", table));                      
                    }
                    else
                    {
                        if (schemaTable != null)
                        {
                            command.Parameters.Add(new SqlParameter("@p2", schemaTable[1]));
                            command.Parameters.Add(new SqlParameter("@p3", schemaTable[0]));
                        }
                    }

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        try
                        {
                            if (dataReader.Read())
                                id = dataReader.GetInt32(0);
                        }
                        catch(SqlException)
                        {                            
                        }
                    }
                }

                if (id != 0)
                {
                    sqlFkPkInfo = new SqlFKPKInfo();

                    // ok, we have it - go get it.
                    int rkeyid, rkey;
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        if (sql2000)
                            command.CommandText = "SELECT rkeyid, rkey FROM sysforeignkeys WHERE constid = @p1";
                        else
                            command.CommandText =
                                "SELECT referenced_object_id, referenced_column_id FROM sys.foreign_key_columns WHERE constraint_object_id = @p1";
                        command.Parameters.Add(new SqlParameter("@p1", id));
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            dataReader.Read();
                            rkeyid = dataReader.GetInt32(0);                            
                            rkey = (sql2000 ? dataReader.GetInt16(1) : dataReader.GetInt32(1));
                        }
                    }

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        if (sql2000)
                            command.CommandText = "select name from sysobjects where id = @p1";
                        else
                            command.CommandText = "select name from sys.objects where object_id = @p1";
                        command.Parameters.Add(new SqlParameter("@p1", rkeyid));
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            dataReader.Read();
                            sqlFkPkInfo.PKTable = dataReader.GetString(0);
                        }
                    }

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        if (sql2000)
                            command.CommandText =
                                "select Column_Name from INFORMATION_SCHEMA.COLUMNS where table_name=@p1 and Ordinal_position=@p3";
                        else
                            command.CommandText =
                                "select Column_Name from INFORMATION_SCHEMA.COLUMNS where table_name=@p1 and table_schema=@p2 and Ordinal_position=@p3";
                        if (sql2000)
                        {
                            command.Parameters.Add(new SqlParameter("@p1", sqlFkPkInfo.PKTable));
                        }
                        else
                        {
                            if (schemaTable != null)
                            {
                                command.Parameters.Add(new SqlParameter("@p1", schemaTable[1]));
                                command.Parameters.Add(new SqlParameter("@p2", schemaTable[0]));
                            }
                        }
                        command.Parameters.Add(new SqlParameter("@p3", rkey));
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            dataReader.Read();
                            sqlFkPkInfo.PKColumnName = dataReader.GetString(0);
                        }
                    }
                }
            }
            return sqlFkPkInfo;
        }
    }
}