// ReSharper disable RedundantUsingDirective
using System.Diagnostics;
// ReSharper restore RedundantUsingDirective
using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.WebPartCache
{
    /// <summary>
    /// 
    /// </summary>
    public class SPSCacheManager : ISPSCacheManager, IEnumerable
    {
        private readonly int _cacheTimeInSeconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SPSCacheManager(int cacheTimeInSeconds)
        {
            _cacheTimeInSeconds = cacheTimeInSeconds > 30 ? cacheTimeInSeconds : 30;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="callback">The callback, is the alternative method to get the key/param>
        /// <returns>The cached object</returns>
        /// <example>string data = Get<string>("Test", () => GetFromSource());</example>
        public T Get<T>(string key, Func<T> callback)
        {
            Debug.WriteLine(string.Format("Cache try get [{0}]", key));
            T item = (T) HttpRuntime.Cache.Get(key);

            if (item == null)
            {
                Debug.WriteLine(string.Format("Cache using callback"));
                item = callback();
                Insert(key, item);
            }
            return item;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            Debug.WriteLine(string.Format("Cache try get [{0}]", key));
            T item = (T)HttpRuntime.Cache.Get(key);

            if (item == null)
            {
                Debug.WriteLine(string.Format("Cache key [{0}] not found!!",key));            
            }
            return item;
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Insert(string key, object value)
        {
            Debug.WriteLine(string.Format("Cache insert [{0}]", key));

            //HttpRuntime.Cache.Insert(key,
            //                         value,
            //                         null,
            //                         DateTime.UtcNow.AddSeconds(_cacheTimeInSeconds),
            //                         TimeSpan.Zero,
            //                         CacheItemPriority.Normal,
            //                         RemoveCallback);
            HttpRuntime.Cache.Insert(key,
                                     value,
                                     null,
                                     DateTime.UtcNow.AddSeconds(_cacheTimeInSeconds),
                                     TimeSpan.Zero);

        }


        [Conditional("DEBUG")]
        private void RemoveCallback(string key, object value, CacheItemRemovedReason reason)
        {
            Debug.WriteLine(string.Format("Cache remove [{0}] {1}", key, reason));
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return HttpRuntime.Cache.GetEnumerator();
        }

        #endregion
    }
}