// File : SPSRowProvider.cs
// Date : 30/01/2008
// User : csegura
// Logs

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls.WebParts;

namespace SPSProfessional.SharePoint.Framework.Comms
{
    /// <summary>
    /// This class send a row of data using IWebPartRow interface internally 
    /// use a queue to manually send the data if there is no data available 
    /// to send in the moment that the GetRowData is called
    /// </summary> 
    ///  /// <example>
    /// [ConnectionProvider("Parameters Row", "ActionFilterQueryStringRowProvider", AllowsMultipleConnections = true)]
    /// public IWebPartRow ConnectionRowProvider()
    /// {
    ///     // Using our special SPSRowProvider class
    ///     _rowProvider = new SPSRowProvider(GetRowViewForProvider());
    ///     return _rowProvider;
    /// }
    /// // <summary>
    /// // Gets the row view for provider.
    /// // </summary>
    /// // <returns></returns>
    /// private DataRowView GetRowViewForProvider()
    /// {
    ///    SPSSchemaValue schemaValue = new SPSSchemaValue();
    ///    string[] parameters = QueryParameters.Split(',');
    ///    if (parameters.Length > 0 and !string.IsNullOrEmpty(QueryParameters))
    ///    {
    ///        // Generate Schema
    ///        foreach (string parameter in parameters)
    ///        {
    ///            schemaValue.AddParameter(parameter, "System.String");
    ///        }
    ///        // Generate Data
    ///        if (Page != null)
    ///        {
    ///            foreach (string parameter in parameters)
    ///            {
    ///                string value = Page.Request.QueryString.Get(parameter);
    ///                schemaValue.AddDataValue(parameter, value);
    ///            }
    ///        }
    ///    }
    ///    return schemaValue.GetDataView();
    /// }
    /// </example>           
    public sealed class SPSRowProvider : IWebPartRow
    {
        private readonly Queue _callbackQueue;
        private DataRowView _rowView;

        #region Properties

        /// <summary>
        /// Gets or sets the row view.
        /// </summary>
        /// <value>The row view.</value>
        public DataRowView RowView
        {
            get { return _rowView; }
            set { _rowView = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSRowProvider"/> class.
        /// </summary>
        public SPSRowProvider()
        {
            _callbackQueue = new Queue();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSRowProvider"/> class.
        /// Use the DefaultView of table to initialize the Schema
        /// </summary>
        /// <param name="dataTable">BuilderTable to initialize the Schema.</param>
        public SPSRowProvider(DataTable dataTable) : this()
        {
            RowView = dataTable.DefaultView[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSRowProvider"/> class.
        /// </summary>
        /// <param name="dataRowView">Row to initialize the Schema.</param>
        public SPSRowProvider(DataRowView dataRowView) : this()
        {
            RowView = dataRowView;
        }

        #endregion

        #region IWebPartRow Members

        /// <summary>
        /// Gets the schema information for a data row that is used to 
        /// share data between two <see cref="T:System.Web.UI.WebControls.WebParts.WebPart"/> 
        /// controls.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> describing the data.</returns>
        PropertyDescriptorCollection IWebPartRow.Schema
        {
            get
            {
                //if (RowView != null)
                {
                    PropertyDescriptorCollection propertyDescriptorCollection;
                    propertyDescriptorCollection = TypeDescriptor.GetProperties(RowView);
                    return propertyDescriptorCollection;
                }
                //else
                //{
                //    return null;
                //}
            }
        }


        /// <summary>
        /// Returns the data for the row that is being used by the interface 
        /// as the basis of a connection between two 
        /// <see cref="T:System.Web.UI.WebControls.WebParts.WebPart"/> controls.
        /// </summary>
        /// <param name="callback">A <see cref="T:System.Web.UI.WebControls.WebParts.RowCallback"/> delegate that contains the address of a method that receives the data.</param>
        void IWebPartRow.GetRowData(RowCallback callback)
        {
            // If there is data
            if (RowView != null)
            {
                // Provide             
                callback(RowView);
            }
            else
            {
                // Enqueue the call for later use
                _callbackQueue.Enqueue(callback);
            }
        }

        #endregion

        #region Engine

        /// <summary>
        /// Sends the row
        /// We can use it later when the data is available
        /// </summary>
        public void SendRow()
        {
            while (_callbackQueue.Count > 0)
            {
                RowCallback callback = (RowCallback) _callbackQueue.Dequeue();
                callback(RowView);
            }
        }

        #endregion

        #region DEBUG

//#if (DEBUG)
//    /// <summary>
//    /// Dumps the property descriptor schema.
//    /// </summary>
//    /// <param name="propertyDescriptorCollection">The property descriptor collection.</param>
//        [System.Diagnostics.Conditional("DEBUG")]
//        private void DumpPropertyDescriptorSchema(PropertyDescriptorCollection propertyDescriptorCollection)
//        {
//            Debug.WriteLine("--> Request Schema");

//            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
//            {
//                Debug.Write(string.Format("{0}: {1} ({2})<br/>",
//                                          (propertyDescriptor.Name),
//                                          (propertyDescriptor.GetValue(_rowView)),
//                                          (propertyDescriptor.PropertyType)));
//            }

//            Debug.WriteLine("--> End Request");
//        }
//#endif

        #endregion
    }
}