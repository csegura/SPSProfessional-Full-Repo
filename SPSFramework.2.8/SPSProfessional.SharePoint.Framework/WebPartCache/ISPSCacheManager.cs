using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.WebPartCache
{
    /// <summary>
    /// Cache Manager Interface
    /// </summary>
    public interface ISPSCacheManager
    {
        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="callback">The callback to store the vualue if this not exist</param>
        /// <returns>The value</returns>
        T Get<T>(string key, Func<T> callback);

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Insert(string key, object value);

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T Get<T>(string key);
    }
}