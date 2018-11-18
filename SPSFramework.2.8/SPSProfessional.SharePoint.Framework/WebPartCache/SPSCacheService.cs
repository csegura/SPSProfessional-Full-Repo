using System.Collections;
using System.Diagnostics;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.Framework.WebPartCache
{
    /// <summary>
    /// 
    /// </summary>
    public class SPSCacheService : ISPSCacheService
    {
        private readonly string _prefix;
        private readonly SPSCacheManager _cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSCacheService"/> class.
        /// </summary>
        /// <param name="timeInSeconds">The time in seconds.</param>
        /// <param name="prefix">The id.</param>
        public SPSCacheService(int timeInSeconds, string prefix)
        {
            Debug.WriteLine(string.Format("SPSCacheService [{0}] expires ( {1}s )", prefix, timeInSeconds) );            
            _prefix = prefix;
            _cacheManager = new SPSCacheManager(timeInSeconds);
        }

        #region Implementation of ISPSCacheManager

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> callback)
        {
            return _cacheManager.Get(_prefix + key, callback);
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Insert(string key, object value)
        {
            _cacheManager.Insert(_prefix + key, value);
        }

        #endregion

        /// <summary>
        /// Removes all.
        /// </summary>
        public void RemoveAll()
        {
            foreach (DictionaryEntry dictionary in _cacheManager)
            {
                if (dictionary.Key.ToString().StartsWith(_prefix))
                {
                    Debug.WriteLine("Remove cache key " + dictionary.Key);
                    _cacheManager.Remove(dictionary.Key.ToString());
                }
            }
        }
    }
}