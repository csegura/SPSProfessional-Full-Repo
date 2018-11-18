using System;
using System.Collections;
using System.ComponentModel;

namespace SPSProfessional.SharePoint.Framework.Error
{
    /// <summary>
    /// XML Based configuration to replace error messages with 
    /// custom errors
    /// </summary>
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public class SPSConfigErrorCollection : ArrayList
    {
        /// <summary>
        /// Adds the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public SPSConfigError Add(SPSConfigError obj)
        {
            if (obj == null)
            {
                base.Add(new SPSConfigError());
            }
            else
            {
                base.Add(obj);
            }
            return obj;
        }

        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public SPSConfigError Add()
        {
            return Add(new SPSConfigError());
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="obj">The obj.</param>
        public void Insert(int index, SPSConfigError obj)
        {
            base.Insert(index, obj);
        }

        /// <summary>
        /// Removes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void Remove(SPSConfigError obj)
        {
            base.Remove(obj);
        }

        /// <summary>
        /// Gets or sets the <see cref="SPSProfessional.SharePoint.Framework.Error.SPSConfigError"/> at the specified index.
        /// </summary>
        /// <value></value>
        public new SPSConfigError this[int index]
        {
            get { return (SPSConfigError) base[index]; }
            set { base[index] = value; }
        }
    }
}