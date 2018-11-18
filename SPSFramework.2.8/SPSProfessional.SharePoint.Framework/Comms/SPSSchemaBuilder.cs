// File : SPSSchemaBuilder.cs
// Date : 03/02/2008
// User : csegura
// Logs
// 03/02/2008 (csegura) - Created.

using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace SPSProfessional.SharePoint.Framework.Comms
{
    ///<summary>
    /// Abstract class to build Schemas
    ///</summary>
    public abstract class SPSSchemaBuilder : IDisposable
    {
        private readonly DataTable _builderTable;
        private readonly DataRow _row;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSSchemaBuilder"/> class.
        /// </summary>
        internal SPSSchemaBuilder()
        {
            _builderTable = new DataTable();
            _builderTable.Locale = CultureInfo.InvariantCulture;
            _row = _builderTable.Rows.Add();
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public PropertyDescriptorCollection Schema
        {
            get { return TypeDescriptor.GetProperties(GetDataView()); }
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>The row.</value>
        protected internal DataTable BuilderTable
        {
            get
            {
                return _builderTable;
            }
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>The row.</value>
        protected internal DataRow Row
        {
            get
            {               
                return _row;
            }
        }

        /// <summary>
        /// Gets the data view.
        /// </summary>
        /// <returns></returns>
        /// TODO: Convert to property
        public DataRowView GetDataView()
        {
            return _builderTable.DefaultView[0];
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_builderTable != null)
                {
                    _builderTable.Dispose();
                }
            }
        }

        #endregion
    }
}