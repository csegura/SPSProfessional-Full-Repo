using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.UI.WebControls.WebParts;
// ReSharper restore RedundantUsingDirective

namespace SPSProfessional.SharePoint.Framework.Comms
{
    ///<summary>
    ///</summary>
    public class SPSParametersProvider : SPSSchemaValue, IWebPartParameters
    {
        private PropertyDescriptorCollection _schema;

        #region IWebPartParameters Members

        ///<summary>
        /// Gets the value of the data from the connection provider.
        /// The GetParametersData method is used to retrieve the values from the provider 
        /// based on a parameter. The method represented by the callback parameter processes 
        /// the data for use by the consumer.
        ///</summary>
        ///<param name="callback">The method to call to process the data from the provider.</param>
        public void GetParametersData(ParametersCallback callback)
        {
            Dictionary<string, object> parametersData = new Dictionary<string, object>();

            if (_schema != null)
            {
                // build the data dictionary
                foreach (PropertyDescriptor property in _schema)
                {
                    foreach (DataColumn column in BuilderTable.Columns)
                    {
                        if (column.Caption.Equals(property.Name) && column.DataType.Equals(property.PropertyType))
                        {
                            parametersData.Add(column.Caption, Row[column]);
                        }
                    }
                }
            }

            callback(parametersData);
        }

        /// <summary>
        /// Sets the property descriptors for the properties that the consumer receives 
        /// when calling the <see cref="M:System.Web.UI.WebControls.WebParts.IWebPartParameters.GetParametersData(System.Web.UI.WebControls.WebParts.ParametersCallback)"/> method.
        /// </summary>
        /// <param name="expectedSchema">The <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> returned by <see cref="P:System.Web.UI.WebControls.WebParts.IWebPartParameters.Schema"/>.</param>
        public void SetConsumerSchema(PropertyDescriptorCollection expectedSchema)
        {            
            _schema = expectedSchema;
        }

        #endregion

        #region Object overrides

        ///<summary>
        ///Returns a <see cref="T:System.String" /> that represents the current consumer schema<see cref="T:System.Object" />.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            const string format = "{0} [{1}]";
            StringBuilder builder = new StringBuilder();
            foreach (PropertyDescriptor property in _schema)
            {
                builder.AppendFormat(format, property.Name, property.PropertyType);
                builder.AppendLine();
            }
            return builder.ToString();
        }

        #endregion

        #region DEBUG

#if  (DEBUG1)
        ///<summary>
        /// Dump Schemas and Values
        ///</summary>
        [Conditional("DEBUG")]
        public void Dump()
        {
            Debug.WriteLine("== Internal Schema ==");
            DumpSchema(_schema);

            Debug.WriteLine("== Base Schema ==");
            DumpSchema(Schema);

            Debug.WriteLine("== Values ==");
            DataRowView rowView = GetDataView();
            if (rowView != null && Schema != null)
            {
                foreach (PropertyDescriptor property in Schema)
                {
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                  "{0} -> [{1}]",
                                                  property.Name,
                                                  rowView[property.Name]));
                }
            }
            else
            {
                Debug.WriteLine("Null GetDataView()");
            }
        }

        /// <summary>
        /// Dumps the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        [Conditional("DEBUG")]
        private static void DumpSchema(PropertyDescriptorCollection schema)
        {
            Debug.WriteLine("== Schema ==");
            if (schema != null)
            {
                foreach (PropertyDescriptor property in schema)
                {
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                  "{0} [{1}]",
                                                  property.Name,
                                                  property.PropertyType));
                }
            }
            else
            {
                Debug.WriteLine("Null schema.");
            }
        }
#endif

        #endregion
    }
}