namespace SPSProfessional.SharePoint.Framework.WebPartCache
{
    /// <summary>
    /// Interface to implement fro WebParts using the CacheService
    /// </summary>
    public interface IWebPartCache
    {
        /// <summary>
        /// Gets the cache service.
        /// </summary>
        /// <returns></returns>
        SPSCacheService GetCacheService();
    }
}