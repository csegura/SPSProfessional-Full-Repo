// File : SPSSchema.cs
// Date : 30/01/2008
// User : csegura
// Logs

using System;
using System.Data;

namespace SPSProfessional.SharePoint.Framework.Comms
{
    /// <summary>
    /// This class is used to generate a custom schema based on TypeDescriptors.
    /// Internally use a DataTable the AddParameter methods adds 
    /// new columns to the table and the Schema property get it 
    /// from the table DefaultView using a TypeDescriptor.
    /// </summary>
    public class SPSSchema : SPSSchemaBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SPSSchema"/> class.
        /// </summary>
        public SPSSchema()
        {
        }

        /// <summary>
        /// Adds the parameter using an SqlDbType.
        /// Internally is converted (cast) to system.type
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">If type is null</exception>
        /// <exception cref="DuplicateNameException">If parameter exist</exception>
        /// <example>AddParameterSql("Parameter","NChar");</example>
        public void AddParameterSql(string parameterName, string type)
        {

            if (type == null)
            {
                throw new ArgumentNullException("type", "type cannot be null");
            }

            AddParameter(parameterName,GetSystemTypeFromSqlDbType(type));
        }

        /// <summary>
        /// Adds the parameter using an SqlDbType.
        /// Internally is converted (cast) to system.type
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">If type is null</exception>
        /// <exception cref="DuplicateNameException">If parameter exist</exception>
        /// <example>AddParameterSql("Parameter","NChar");</example>
        public void AddParameterSql(string parameterName, SqlDbType type)
        {
            AddParameter(parameterName, GetSystemTypeFromSqlDbType(type));
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentException">If type is invalid</exception>
        /// <exception cref="DuplicateNameException">If parameter exist</exception>
        /// <exexample>AddParameter("Parameter","System.String");</exexample>
        public void AddParameter(string parameterName, string type)
        {
            Type systemType = Type.GetType(type);
            AddParameter(parameterName, systemType);
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="type">The type.</param>
        /// <example>AddParameter("Parameter",typeof(System.Int));</example>
        /// <exception cref="DuplicateNameException">If parameter exist</exception>
        /// <exception cref="ArgumentNullException">If type is null</exception>
        public void AddParameter(string parameterName, Type type)
        {
            try
            {
                BuilderTable.Columns.Add(parameterName, type);
            }
            catch(DuplicateNameException)
            {                
                // Ignore
            }
        }

        /// <summary>
        /// Gets the type system type from a SqlDbType.
        /// </summary>
        /// <param name="sqlDbType">Type of the SQL db.</param>
        /// <returns>The system Type</returns>
        private Type GetSystemTypeFromSqlDbType(string sqlDbType)
        {
            SqlDbType sqlType = (SqlDbType) Enum.Parse(typeof(SqlDbType), sqlDbType);
            return GetSystemTypeFromSqlDbType(sqlType);
        }

        /// <summary>
        /// Gets the type of the system type from SQL db.
        /// Not contempled Udt, Structured, DateTime2, DateTimeOffset
        /// </summary>
        /// <param name="sqlType">Type of the SQL.</param>
        /// <returns></returns>
        private static Type GetSystemTypeFromSqlDbType(SqlDbType sqlType)
        {
            Type type = null;
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    type = typeof(Int64);
                    break;
                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                    type = typeof(Byte[]);
                    break;
                case SqlDbType.Bit:
                    type = typeof(Boolean);
                    break;
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.NText:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    type = typeof(String);
                    break;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    type = typeof(DateTime);
                    break;
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    type = typeof(Decimal);
                    break;
                case SqlDbType.Float:
                    type = typeof(Double);
                    break;
                case SqlDbType.Int:
                    type = typeof(Int32);
                    break;
                case SqlDbType.Real:
                    type = typeof(Single);
                    break;
                case SqlDbType.SmallInt:
                    type = typeof(Int16);
                    break;
                case SqlDbType.TinyInt:
                    type = typeof(Byte);
                    break;
                case SqlDbType.UniqueIdentifier:
                    type = typeof(Guid);
                    break;
                case SqlDbType.Variant:
                    type = typeof(object);
                    break;
                    #region OLDCODE

                    //case "BigInt":
                    //    type = typeof (Int64);
                    //    break;
                    //case "Binary":
                    //case "Image":
                    //case "TimeStamp":
                    //    type = typeof (Byte[]);
                    //    break;
                    //case "Bit":
                    //    type = typeof (Boolean);
                    //    break;
                    //case "Char":
                    //case "NChar":
                    //case "NText":
                    //case "NVarChar":
                    //case "SysName":
                    //case "Text":
                    //case "VarChar":
                    //    type = typeof (String);
                    //    break;
                    //case "Datetime":
                    //case "SmallDateTime":
                    //    type = typeof (DateTime);
                    //    break;
                    //case "Decimal":
                    //case "Money":
                    //case "Numeric":
                    //case "SmallMoney":
                    //    type = typeof (Decimal);
                    //    break;
                    //case "Float":
                    //    type = typeof (Double);
                    //    break;
                    //case "Int":
                    //    type = typeof (Int32);
                    //    break;
                    //case "Real":
                    //    type = typeof (Single);
                    //    break;
                    //case "SmallInt":
                    //    type = typeof (Int16);
                    //    break;
                    //case "TinyInt":
                    //    type = typeof (Byte);
                    //    break;      

                    #endregion
            }
            return type;
        }

        #region Not Used

        //private SqlDbType GetDBType(Type theType)
        //{
        //    SqlParameter param;
        //    TypeConverter tc;
        //    param = new SqlParameter();
        //    tc = TypeDescriptor.GetConverter(param.DbType);
        //    if (tc.CanConvertFrom(theType))
        //    {
        //        param.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //    }
        //    else
        //    {
        //        // try to forcefully convert
        //        try
        //        {
        //            param.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("SPSSchema - GetDbType - Error" + ex);
        //        }
        //    }
        //    return param.SqlDbType;
        //}

        #endregion

        // Wrapper to test important private methods

        #region DEBUG_FOR_TESTS

        #if DEBUG
        /// <summary>
        /// Gets the type from SQL db type_ test.
        /// </summary>
        /// <param name="sqlDbType">Type of the SQL db.</param>
        /// <returns></returns>
        public Type GetSystemTypeFromSqlDbType_Test(string sqlDbType)
        {
            return GetSystemTypeFromSqlDbType(sqlDbType);
        }

        /// <summary>
        /// Gets the type from SQL db type_ test.
        /// </summary>
        /// <param name="sqlDbType">Type of the SQL db.</param>
        /// <returns></returns>
        public Type GetSystemTypeFromSqlDbType_Test(SqlDbType sqlDbType)
        {
            return GetSystemTypeFromSqlDbType(sqlDbType);
        }
        #endif

        #endregion
    }
}