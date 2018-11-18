// File : SPSKeyValueList.cs
// Date : 06/02/2008
// User : csegura
// Logs
// 06/02/2008 (csegura) - Created.
// 17/02/2008 (csegura) - Added index by key

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace SPSProfessional.SharePoint.Framework.Common
{
    /// <summary>
    /// List of SPSKeyValuePair this class solve the problem with serializable Dictionaries
    /// Internally has a Base64String serializer and deserializer to use with ViewState
    /// </summary>
    [Serializable]
    public sealed class SPSKeyValueList : List<SPSKeyValuePair>
    {
        
        ///<summary>
        ///</summary>
        public SPSKeyValueList()
        {
        }

        /// <summary>
        /// Adds the specified key value
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, string value)
        {
            SPSKeyValuePair keyValuePair = new SPSKeyValuePair(key, value);
            Add(keyValuePair);
        }

        /////// <summary>
        /////// Adds the specified key, if a same key exist the value is not inserted
        /////// </summary>
        /////// <param name="key">The key.</param>
        /////// <param name="value">The value.</param>
        ////public void AddUnique(string key, string value)
        ////{
        ////    SPSKeyValuePair keyValuePair = new SPSKeyValuePair(key, value);

        ////    if (!Contains(keyValuePair))
        ////    {
        ////        Add(keyValuePair);
        ////    }
        ////}

        /////// <summary>
        /////// Adds the specified key, if a same key exist the value is not inserted
        /////// </summary>
        /////// <param name="keyValuePair">The key value pair.</param>
        ////public void AddUnique(SPSKeyValuePair keyValuePair)
        ////{           
        ////    if (!Contains(keyValuePair))
        ////    {
        ////        Add(keyValuePair);
        ////    }
        ////}

        /// <summary>
        /// Replaces the specified newValue for the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The value.</param>
        public void ReplaceValue(string key, string newValue)
        {
            foreach(SPSKeyValuePair keyValue in this)
            {
                if (keyValue.Key == key)
                {
                    keyValue.Value = newValue;
                    break;
                }
            }            
        }

        /// <summary>
        /// Replaces the specified newValue for the all keys.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The value.</param>
        public void ReplaceAllValues(string key, string newValue)
        {
            foreach (SPSKeyValuePair keyValue in this)
            {
                if (keyValue.Key == key)
                {
                    keyValue.Value = newValue;
                }
            }
        }

        /// <summary>
        /// Determines whether this the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string key)
        {
            bool contains = false;           
            foreach (SPSKeyValuePair keyValue in this)
            {
                if (keyValue.Key == key)
                {
                    contains = true;
                    break;
                }
            }
            
            return contains;
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// NOTE: If the key does not exist return null
        /// </summary>
        /// <value></value>
        public string this[string key]
        {
            get
            {
                foreach (SPSKeyValuePair keyValue in this)
                {
                    if (keyValue.Key == key)
                    {
                        return keyValue.Value;
                    }
                }
                return null;
            }

        }

        /// <summary>
        /// Deserializes the specified serialized data thats comes in a Base64String
        /// </summary>
        /// <param name="serializedData">The serialized data.</param>
        /// <returns>A new SPSKeyValueList</returns>
        [Obsolete("Use SPSSerialization class")]
        public static SPSKeyValueList Deserialize(string serializedData)
        {            
            byte[] buffer = Convert.FromBase64String(serializedData);      
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream(buffer);            
            SPSKeyValueList list = (SPSKeyValueList)formatter.Deserialize(serializationStream);
            return list;
        }

        /// <summary>
        /// Serializes the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>An string that contains the Base64String</returns>
        [Obsolete("Use SPSSerialization class")]
        public static string Serialize(SPSKeyValueList list)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            MemoryStream serializationStream = new MemoryStream();
            formatter.Serialize(serializationStream, list);
            serializationStream.Close();
            return Convert.ToBase64String(serializationStream.GetBuffer());
        }       
    }
}