using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.WebPartCache
{
    /// <summary>
    /// Cache Service 
    /// </summary>
    /// TODO : heritate from ISPSCacheManager 
    public interface ISPSCacheService
    {
        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> callback);

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Insert(string key, object value);

        /// <summary>
        /// Removes all.
        /// </summary>
        void RemoveAll();
    }
}