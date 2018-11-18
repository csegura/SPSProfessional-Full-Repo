// File : SPSSchemaValue.cs
// Date : 30/01/2008
// User : csegura
// Logs
// 01/02/2008 (csegura) - Changed AddDataValue if value is null DBNull value is passed 

using System;
using System.Data;

namespace SPSProfessional.SharePoint.Framework.Comms
{
    /// <summary>
    /// This class complement SPSSchema and can 
    /// create a schema and assign values to it.
    /// </summary>
    public class SPSSchemaValue : SPSSchema
    {
        /// <summary>
        /// Adds the data.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="data">The data.</param>
        public void AddDataValue(string columnName, object data)
        {
            if (data == null)
            {
                Row[columnName] = DBNull.Value;
            }
            else
            {
                Row[columnName] = data;
            }
        }

        /// <summary>
        /// Adds the data.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        public void AddDataValue(string columnName, string value)
        {
            if (value == null)
            {
                Row[columnName] = DBNull.Value;
            }
            else
            {
                Row[columnName] = value;
            }
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="DuplicateNameException">If parameter exist</exception>
        /// <exception cref="ArgumentNullException">If type is null</exception>
        public void Add(string columnName, string value)
        {
            AddParameter(columnName, typeof(string));
            AddDataValue(columnName,value);
        }
    }
}