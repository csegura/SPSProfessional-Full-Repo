using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SPSProfessional.SharePoint.Framework.Error
{
    /// <summary>
    /// 
    /// </summary>
    [XmlType(TypeName = "Errors"), Serializable]
    public class SPSConfigErrors
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return ErrorCollection.GetEnumerator();
        }

        /// <summary>
        /// Adds the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public SPSConfigError Add(SPSConfigError obj)
        {
            return ErrorCollection.Add(obj);
        }

        /// <summary>
        /// Gets the <see cref="SPSProfessional.SharePoint.Framework.Error.SPSConfigError"/> at the specified index.
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        public SPSConfigError this[int index]
        {
            get { return ErrorCollection[index]; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        [XmlIgnore]
        public int Count
        {
            get { return ErrorCollection.Count; }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            ErrorCollection.Clear();
        }

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public SPSConfigError Remove(int index)
        {
            SPSConfigError obj = ErrorCollection[index];
            ErrorCollection.Remove(obj);
            return obj;
        }

        /// <summary>
        /// Removes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void Remove(object obj)
        {
            ErrorCollection.Remove(obj);
        }

        [XmlElement(Type = typeof (SPSConfigError), ElementName = "Error", IsNullable = false,
            Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public SPSConfigErrorCollection __ErrorCollection;

        /// <summary>
        /// Gets or sets the error collection.
        /// </summary>
        /// <value>The error collection.</value>
        [XmlIgnore]
        public SPSConfigErrorCollection ErrorCollection
        {
            get
            {
                if (__ErrorCollection == null)
                {
                    __ErrorCollection = new SPSConfigErrorCollection();
                }
                return __ErrorCollection;
            }
            //TODO: Clean set { __ErrorCollection = value; }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "[Errors]";
        }
    }
}