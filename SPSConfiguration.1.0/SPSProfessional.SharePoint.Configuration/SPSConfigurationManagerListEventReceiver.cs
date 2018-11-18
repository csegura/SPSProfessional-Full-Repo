using System;
using System.Web;
using System.Web.Caching;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.ConfigurationManager;

namespace SPSProfessional.SharePoint.Configuration
{
    /// <summary>
    /// This list event receiver manages the memory cache used by the Config Store. In the current implementation 
    /// it is essential that the Config Store list is configured with this event receiver, otherwise stale 
    /// values will be retrieved.
    /// </summary>
    public class SPSConfigurationManagerListEventReceiver : SPItemEventReceiver
    {
        public override void ItemAdded(SPItemEventProperties properties)
        {
            // add item to cache..
            //TODO:Remove
            //HttpRuntime runtime = new HttpRuntime();

            string category = properties.AfterProperties[SPSConfigurationManager.FIELD_CATEGORY] as string;
            string key = properties.AfterProperties[SPSConfigurationManager.FIELD_KEY] as string;
            string cacheKey = SPSConfigurationManager.FormatKey(category, key);
            string value = properties.AfterProperties[SPSConfigurationManager.FIELD_VALUE] as string;

            if (value != null)
            {
                HttpRuntime.Cache.Insert(cacheKey, value, null, DateTime.MaxValue, Cache.NoSlidingExpiration);
            }

            base.ItemAdded(properties);
        }

        public override void ItemDeleted(SPItemEventProperties properties)
        {
            // delete item from cache..
            // TODO: Remove
            // HttpRuntime runtime = new HttpRuntime();

            string category = properties.BeforeProperties[SPSConfigurationManager.FIELD_CATEGORY] as string;
            string key = properties.BeforeProperties[SPSConfigurationManager.FIELD_KEY] as string;
            string cacheKey = SPSConfigurationManager.FormatKey(category, key);

            HttpRuntime.Cache.Remove(cacheKey);

            base.ItemDeleted(properties);
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            //TODO: Remove
            //HttpRuntime runtime = new HttpRuntime();

            /* Unfortunately properties.BeforeProperties does not get populated in this event, so if the 'category' or 
             * 'key' of the config item changes (i.e. a rename), we can't identity the old item in the cache to remove. 
             * However, all this means is our cache will have an extra item in which isn't used, so this is no big deal..
             */
            string category = properties.ListItem[SPSConfigurationManager.FIELD_CATEGORY] as string;
            string key = properties.ListItem[SPSConfigurationManager.FIELD_KEY] as string;
            string cacheKey = SPSConfigurationManager.FormatKey(category, key);

            HttpRuntime.Cache.Remove(cacheKey);

            // also proactively add item to cache so first user doesn't get the hit. Note we also account for 
            // any renames with use of BeforeProperties/AfterProperties..
            string value = properties.ListItem[SPSConfigurationManager.FIELD_VALUE] as string;
            if (value != null)
            {
                HttpRuntime.Cache.Insert(cacheKey, value, null, DateTime.MaxValue, Cache.NoSlidingExpiration);
            }
            
            base.ItemUpdated(properties);
        }
    }
}