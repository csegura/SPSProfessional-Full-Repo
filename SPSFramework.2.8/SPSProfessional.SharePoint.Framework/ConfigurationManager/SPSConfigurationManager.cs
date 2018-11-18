using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Caching;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.ConfigurationManager
{
    /// <summary>
    /// Utility class for retrieving configuration values within a SharePoint site. 
    /// Where multiple values need to 
    /// be retrieved, the GetMultipleValues() method should be used where possible.
    /// 
    /// Created by Chris O'Brien (www.sharepointnutsandbolts.com).
    /// </summary>
    public static class SPSConfigurationManager
    {
        #region Private Fields

        private static readonly AppSettingsReader _reader = new AppSettingsReader();
        private static readonly string _defaultWebName = string.Empty;
        
        #endregion

        #region CONSTANTS

        public const string LIST_NAME = "SPSProfessional Configuration Manager"; //"Config store"; 
        public const string FIELD_CATEGORY = "Category"; // "Config_x0020_category";
        public const string FIELD_KEY = "Title";
        public const string FIELD_VALUE = "Value"; //"Config_x0020_value";
        public const string FIELD_DESCRIPTION = "Description"; //"Config_x0020_value";

        public const string WC_SITE_URL = "SPSSiteUrl"; //"ConfigSiteUrl";
        public const string WC_WEB_NAME = "SPSWebName"; //"ConfigWebName";
        public const string WC_LIST_NAME = "SPSListName"; //"ConfigListName";

        private const string ERR_BASE = "SPSProfessional Configuration Manager Error: ";

        private const string ERR_MULTIPLE_CATEGORY =
                ERR_BASE +
                "Multiple config items were found for the requested item. Please check " +
                "SPS Configuration Manager settings list for category '{0}' and key '{1}'.";

        private static readonly string ERR_KEY_NOT_FOUND = SPSLocalization.GetResourceString("SPSCM_Err_KeyNotFound");
        private static readonly string ERR_LIST_NOT_FOUND = SPSLocalization.GetResourceString("SPSCM_Err_ListNotFound");

        private static readonly string ERR_WEB_NOT_FOUND = ERR_BASE
                                                           + SPSLocalization.GetResourceString("SPSCM_Err_WebNotFound");

        //"Unable to find configuration web in current site collection with name '{0}'.";

        private static readonly string ERR_OUT_CONTEXT_WEB_CONFIG = ERR_BASE
                                                                    +
                                                                    SPSLocalization.GetResourceString(
                                                                            "SPSCM_Err_OutContextWebConfig");

        //"To use the SPS Configuration Manager where no SPContext is present, you must specify the " +
        //"URL of the parent site collection for the 'config' web in application config. "+
        //"If you are using the SPS Configuration Manager outside of your SharePoint web application "+
        //"e.g. a console app, your console app will require an app.config file with the "+
        //"required settings. The value should be stored in an " +
        //"appSettings key named '{0}'.";

        private static readonly string ERR_INVALID_URL_IN_WEB_CONTEXT = ERR_BASE
                                                                        +
                                                                        SPSLocalization.GetResourceString(
                                                                                "SPSCM_Err_InvalidUrlInWebContext");

        //"Unable to contact site '{0}' specified in appSettings/{1} as " +
        //"URL for SPS Configuration Manager site. Please check the URL.";

        //private const string ERR_NO_URL_IN_WEB_CONTEXT =
        //        ERR_BASE +
        //        "An override URL for the config site was specified but it was invalid. " +
        //        "Please check your configuration.";

        private const string ERR_USE_GETVALUE_INSTEAD =
                ERR_BASE +
                "Invalid use of SPS Configuration Manager - the GetMultipleValues() method " +
                "must only be used to retrieve multiple config values.";

        #endregion

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns>the value or exception</returns>
        /// <exception cref="SPSConfigurationManagerException"></exception>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static string GetValue(string category, string key)
        {
            return GetValueInternal(category, key, true);
        }

        /// <summary>
        /// Ensures the get value.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static string EnsureGetValue(string category, string key)
        {
            return GetValueInternal(category, key, false);
        }

        /// <summary>
        /// Gets the multiple values.
        /// </summary>
        /// <param name="configIdentifiers">The config identifiers.</param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static IDictionary<SPSConfigurationManagerIdentifier, string> GetMultipleValues(
                ICollection<SPSConfigurationManagerIdentifier> configIdentifiers)
        {
            return GetMultipleValuesInternal(configIdentifiers);
        }

        /// <summary>
        /// Checks the value.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static bool HasValue(string category, string key)
        {
            return !string.IsNullOrEmpty(GetValueInternal(category, key, false));
        }

        /// <summary>
        /// Checks if Configuration Manager is available.
        /// </summary>
        /// <returns>True if Configuration Manager is available</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static bool Check()
        {
            try
            {
                GetValueInternal(string.Empty, string.Empty, true);
                return true;
            }
            catch (SPSConfigurationManagerException ex)
            {
                if (ex.Message == FormatError(ERR_KEY_NOT_FOUND, string.Empty, string.Empty))
                {
                    return true;
                }                
            }
            return false;
        }


        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if changed or aded, false if error</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static bool SetKey(string category, string key, string value)
        {
            return SetKeyInternal(category, key, value, string.Empty);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="description">The description.</param>
        /// <returns>True if changed or aded, false if error</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static bool SetKey(string category, string key, string value, string description)
        {
            return SetKeyInternal(category, key, value, description);
        }

        /// <summary>
        /// Deletes the key.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static bool DeleteKey(string category, string key)
        {
            return DeleteKeyInternal(category, key);
        }

        /// <summary>
        /// Retrieves a single value from the SPS Configuration Manager list.
        /// </summary>
        /// <param name="category">Category of the item to retrieve.</param>
        /// <param name="key">Key (item name) of the item to retrieve.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns>The config item's value.</returns>
        /// <exception cref="SPSConfigurationManagerException"><c>SPSConfigurationManagerException</c>.</exception>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        private static string GetValueInternal(string category, string key, bool throwException)
        {
            // first let's trim the supplied values..
            category = category.Trim();
            key = key.Trim();

            bool listFoundFromSPContext = true;
            string value = null;

            // attempt retrieval from cache..
            HttpContext httpCtxt = HttpContext.Current;
            string cachedValue = null;
            string cacheKey = FormatKey(category, key);

            if (httpCtxt != null)
            {
                cachedValue = httpCtxt.Cache[cacheKey] as string;
            }

            if (!string.IsNullOrEmpty(cachedValue))
            {
                return cachedValue;
            }

            // no value found, proceed with query..
            SPSecurity.RunWithElevatedPrivileges(
                    delegate
                    {
                        SPList configStoreList = TryGetConfigStoreListFromContext();

                        if (configStoreList == null)
                        {
                            listFoundFromSPContext = false;
                            configStoreList = TryGetConfigStoreListFromConfig(throwException);
                        }

                        SPListItemCollection items = null;

                        if (configStoreList != null)
                        {
                            try
                            {
                                items = configStoreList.GetItems(CompoundSingleQuery(category, key));

                                if (items.Count == 1)
                                {
                                    if (items[0][0] != null)
                                    {
                                        value = items[0][0].ToString();
                                    }
                                    else
                                    {
                                        value = string.Empty;
                                    }
                                }
                                else if (items.Count > 1)
                                {
                                    throw new SPSConfigurationManagerException(
                                            FormatError(
                                                    ERR_MULTIPLE_CATEGORY,
                                                    category,
                                                    key));
                                }
                                else if (items.Count == 0 && throwException)
                                {
                                    throw new SPSConfigurationManagerException(
                                            FormatError(
                                                    ERR_KEY_NOT_FOUND,
                                                    category,
                                                    key));
                                }
                            }
                            finally
                            {
                                if (!listFoundFromSPContext)
                                {
                                    // disposals are required.. 
                                    configStoreList.ParentWeb.Site.Dispose();
                                    configStoreList.ParentWeb.Dispose();
                                }
                            }
                        }

                        // add to cache if we have a HttpContext..
                        if (httpCtxt != null && items != null && items.Count == 1)
                        {
                            httpCtxt.Cache.Insert(cacheKey,
                                                  value,
                                                  null,
                                                  DateTime.MaxValue,
                                                  Cache.NoSlidingExpiration);
                        }
                    });

            return value;
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <param name="Category">The category.</param>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public static string FormatKey(string Category, string Key)
        {
            const string format = "{0}|{1}";
            return string.Format(CultureInfo.InvariantCulture, format, Category, Key);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        private static bool SetKeyInternal(string category, string key, string value, string description)
        {
            // first let's trim the supplied values..
            category = category.Trim();
            key = key.Trim();

            bool changed = false;

            // no value found, proceed with query..
            SPSecurity.RunWithElevatedPrivileges(
                    delegate
                    {
                        SPList configStoreList = TryGetConfigStoreListFromContext();

                        if (configStoreList != null)
                        {
                            try
                            {
                                SPListItemCollection items = configStoreList.GetItems(CompoundSingleQuery(category, key));
                                SPListItem currentItem;

                                if (items != null && items.Count == 1)
                                {
                                    // Modify
                                    currentItem = items[0];
                                }
                                else
                                {
                                    // Add
                                    currentItem = configStoreList.Items.Add();
                                    currentItem[FIELD_CATEGORY] = category;
                                    currentItem[FIELD_KEY] = key;
                                }

                                currentItem[FIELD_VALUE] = value;
                                currentItem[FIELD_DESCRIPTION] = description;
                                currentItem.Update();

                                //configStoreList.ParentWeb.AllowUnsafeUpdates = true; 
                                configStoreList.Update();
                                //configStoreList.ParentWeb.AllowUnsafeUpdates = false;
                                changed = true;
                            }
                            finally
                            {
                                // disposals are required.. 
                                configStoreList.ParentWeb.Site.Dispose();
                                configStoreList.ParentWeb.Dispose();
                            }
                        }
                    });

            return changed;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        private static bool DeleteKeyInternal(string category, string key)
        {
            // first let's trim the supplied values..
            category = category.Trim();
            key = key.Trim();

            bool deleted = false;

            // no value found, proceed with query..
            SPSecurity.RunWithElevatedPrivileges(
                    delegate
                    {
                        SPList configStoreList = TryGetConfigStoreListFromContext();

                        if (configStoreList != null)
                        {
                            try
                            {
                                SPListItemCollection items = configStoreList.GetItems(CompoundSingleQuery(category, key));
                                SPListItem currentItem;

                                if (items != null && items.Count == 1)
                                {
                                    // Modify
                                    currentItem = items[0];
                                    currentItem.Delete();
                                    configStoreList.Update();
                                    deleted = true;
                                }
                            }
                            finally
                            {
                                // disposals are required.. 
                                configStoreList.ParentWeb.Site.Dispose();
                                configStoreList.ParentWeb.Dispose();
                            }
                        }
                    });

            return deleted;
        }

        /// <summary>
        /// Searches the items.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns>List items</returns>
        private static SPQuery CompoundSingleQuery(string category, string key)
        {
            const string CAML_Query = "<Where><And><Eq><FieldRef Name=\"{0}\" /><Value Type=\"Text\">{1}</Value></Eq>" +
                                      "<Eq><FieldRef Name=\"{2}\" /><Value Type=\"Text\">{3}</Value></Eq></And></Where>";

            const string CAML_Field = "<FieldRef Name=\"{0}\" />";
            SPQuery query = new SPQuery
                                {
                                        Query = string.Format(CultureInfo.InvariantCulture,
                                                              CAML_Query,
                                                              FIELD_CATEGORY,
                                                              category,
                                                              FIELD_KEY,
                                                              key),
                                        ViewFields = string.Format(CultureInfo.InvariantCulture,
                                                                   CAML_Field,
                                                                   FIELD_VALUE)
                                };

            return query;
        }

        /// <summary>
        /// Retrieves multiple config values with a single query. 
        /// </summary>
        /// <param name="configIdentifiers">List of SPSConfigurationManagerIdentifier objects to retrieve.</param>
        /// <returns>A Dictionary object containing the requested config values. Items are keyed by SPSConfigurationManagerIdentifier.</returns>
        /// <exception cref="SPSConfigurationManagerException">SPSProfessional Configuration Manager Error: Invalid use of SPS Configuration Manager - the GetMultipleValues() method must only be used to retrieve multiple config values.</exception>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        private static IDictionary<SPSConfigurationManagerIdentifier, string> GetMultipleValuesInternal(
                ICollection<SPSConfigurationManagerIdentifier> configIdentifiers)
        {
            // first let's trim the supplied values..
            TrimDictionaryEntries(configIdentifiers);

            Dictionary<SPSConfigurationManagerIdentifier, string> configDictionary =
                    new Dictionary<SPSConfigurationManagerIdentifier, string>();

            // attempt retrieval from cache..
            HttpContext httpCtxt = HttpContext.Current;

            if (httpCtxt != null)
            {
                bool foundAllValuesInCache = true;

                foreach (SPSConfigurationManagerIdentifier identifier in configIdentifiers)
                {
                    if (!TryGetValuesFromCache(httpCtxt, identifier, configDictionary))
                    {
                        foundAllValuesInCache = false;
                        break;
                    }
                }

                if (foundAllValuesInCache)
                {
                    return configDictionary;
                }

                // clear the dictionary since we'll add fresh config items to it from our query..
                configDictionary.Clear();
            }

            // no value found, proceed with query..
            if (configIdentifiers.Count < 2)
            {
                throw new SPSConfigurationManagerException(ERR_USE_GETVALUE_INSTEAD);
            }

            SPSecurity.RunWithElevatedPrivileges(
                    delegate
                    {
                        bool listFoundFromSPContext = true;
                        SPList configStoreList = TryGetConfigStoreListFromContext();

                        if (configStoreList == null)
                        {
                            listFoundFromSPContext = false;
                            configStoreList = TryGetConfigStoreListFromConfig(true);
                        }

                        if (configStoreList != null)
                        {
                            try
                            {
                                SPQuery query = CompoundMultipleQuery(configIdentifiers);
                                SPListItemCollection items = configStoreList.GetItems(query);

                                //string category = string.Empty;
                                //string key = string.Empty;
                                //string configValue = string.Empty;
                                //string cacheKey = string.Empty;

                                foreach (SPListItem item in items)
                                {
                                    foreach (SPSConfigurationManagerIdentifier configID in configIdentifiers
                                            )
                                    {
                                        if (ListItemContainsIdentifier(item, configID))
                                        {
                                            // category = item[FIELD_CATEGORY].ToString();
                                            // key = item[FIELD_KEY].ToString();

                                            string configValue = item[FIELD_VALUE].ToString();

                                            if (!configDictionary.ContainsKey(configID))
                                            {
                                                configDictionary.Add(configID, configValue);

                                                // also add to cache..
                                                if (httpCtxt != null)
                                                {
                                                    string cacheKey =
                                                            FormatKey(configID.Category,
                                                                      configID.Key);
                                                    httpCtxt.Cache.Insert(cacheKey,
                                                                          configValue,
                                                                          null,
                                                                          DateTime.MaxValue,
                                                                          Cache.
                                                                                  NoSlidingExpiration);
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                throw new SPSConfigurationManagerException(
                                                        FormatError(
                                                                ERR_MULTIPLE_CATEGORY,
                                                                configID.Category,
                                                                configID.Key));
                                            }
                                        }
                                    }
                                }
                            }

                            finally
                            {
                                if (!listFoundFromSPContext)
                                {
                                    // disposals are required.. 
                                    configStoreList.ParentWeb.Site.Dispose();
                                    configStoreList.ParentWeb.Dispose();
                                }
                            }
                        }
                    });

            return configDictionary;
        }

        private static bool TryGetValuesFromCache(HttpContext httpCtxt,
                                                  SPSConfigurationManagerIdentifier identifier,
                                                  IDictionary<SPSConfigurationManagerIdentifier, string>
                                                          configDictionary)
        {
            string cacheValue = TryGetValueFromCache(httpCtxt, identifier);

            if (cacheValue != null)
            {
                configDictionary.Add(identifier, cacheValue);
                return true;
            }
            return false;
        }

        private static string TryGetValueFromCache(HttpContext httpCtxt, SPSConfigurationManagerIdentifier identifier)
        {
            string cacheKey = FormatKey(identifier.Category, identifier.Key);
            return httpCtxt.Cache[cacheKey] as string;
        }

        private static bool ListItemContainsIdentifier(SPItem item, SPSConfigurationManagerIdentifier configID)
        {
            return (item[FIELD_CATEGORY].ToString()
                    == configID.Category)
                   && (item[FIELD_KEY].ToString() == configID.Key);
        }

        private static SPQuery CompoundMultipleQuery(IEnumerable<SPSConfigurationManagerIdentifier> configIdentifiers)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.Append("<Where><Or>");

            foreach (SPSConfigurationManagerIdentifier configID in configIdentifiers)
            {
                queryBuilder.AppendFormat(CultureInfo.InvariantCulture,
                                          "<And><Eq><FieldRef Name=\"{0}\" /><Value Type=\"Text\">{1}</Value></Eq>" +
                                          "<Eq><FieldRef Name=\"{2}\" /><Value Type=\"Text\">{3}</Value></Eq></And>",
                                          FIELD_CATEGORY,
                                          configID.Category,
                                          FIELD_KEY,
                                          configID.Key);
            }

            queryBuilder.Append("</Or></Where>");

            const string CAML_ViewFields = "<FieldRef Name=\"{0}\" /><FieldRef Name=\"{1}\" /><FieldRef Name=\"{2}\" />";
            SPQuery query = new SPQuery
                                {
                                        Query = queryBuilder.ToString(),
                                        ViewFields = string.Format(CultureInfo.InvariantCulture,
                                                                   CAML_ViewFields,
                                                                   FIELD_CATEGORY,
                                                                   FIELD_KEY,
                                                                   FIELD_VALUE)
                                };
            return query;
        }

        #region -- Private helper methods --

        private static void TrimDictionaryEntries(IEnumerable<SPSConfigurationManagerIdentifier> ConfigIdentifiers)
        {
            foreach (SPSConfigurationManagerIdentifier configId in ConfigIdentifiers)
            {
                configId.Category = configId.Category.Trim();
                configId.Key = configId.Key.Trim();
            }
        }

        /// <summary>
        /// Tries get config store list from context.
        /// Without launch exceptions
        /// </summary>
        /// <returns>The list</returns>        
        private static SPList TryGetConfigStoreListFromContext()
        {
            SPList configList = null;
            SPContext currentContext = SPContext.Current;

            if (currentContext != null)
            {
                SPSite currentSite = currentContext.Site;
                SPWeb configWeb = TryGetConfigStoreWeb(currentSite, false);

                if (configWeb != null)
                {
                    configList = TryGetConfigStoreList(configWeb, false);
                }

                {
                    // we're not in the same site collection as the config web. Exit without disposing any references since
                    // we haven't created any objects..
                }
            }

            return configList;
        }

        /// <summary>
        /// Tries the get config store list from config.
        /// </summary>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns>The list</returns>
        /// <exception cref="SPSConfigurationManagerException"><c>SPSConfigurationManagerException</c>.</exception>
        private static SPList TryGetConfigStoreListFromConfig(bool throwException)
        {
            SPList configStoreList = null;

            SPSite configSite = GetConfigSiteFromConfiguredUrl();

            if (configSite != null)
            {
                SPWeb configStoreWeb = TryGetConfigStoreWeb(configSite, throwException);
                configStoreList = TryGetConfigStoreList(configStoreWeb, throwException);
            }

            return configStoreList;
        }


        /// <exception cref="SPSConfigurationManagerException"><c>SPSConfigurationManagerException</c>.</exception>
        private static SPList TryGetConfigStoreList(SPWeb configStoreWeb, bool bThrowOnNotFound)
        {
            SPList configStoreList = null;

            // do we have an override list name specified in config? 
            string sOverrideList = GetAppSettingsValue(WC_LIST_NAME);
            string sListName = (string.IsNullOrEmpty(sOverrideList) ? LIST_NAME : sOverrideList);

            try
            {
                configStoreList = configStoreWeb.Lists[sListName];
            }
            catch (ArgumentException argExc)
            {
                if (bThrowOnNotFound)
                {
                    throw new SPSConfigurationManagerException(
                            FormatError(ERR_LIST_NOT_FOUND, LIST_NAME),
                            argExc);
                }
            }

            return configStoreList;
        }

        /// <exception cref="SPSConfigurationManagerException"><c>SPSConfigurationManagerException</c>.</exception>
        private static SPWeb TryGetConfigStoreWeb(SPSite configSite, bool bThrowOnNotFound)
        {
            SPWeb configStoreWeb = null;

            // do we have an override web name specified in config? 
            string sOverrideWeb = GetAppSettingsValue(WC_WEB_NAME);

            // if so, find web with this name. If not, default to root web..
            if (!string.IsNullOrEmpty(sOverrideWeb))
            {
                try
                {
                    configStoreWeb = configSite.AllWebs[sOverrideWeb];
                }
                catch (ArgumentException argExc)
                {
                    if (bThrowOnNotFound)
                    {
                        throw new SPSConfigurationManagerException(
                                FormatError(
                                        ERR_WEB_NOT_FOUND,
                                        _defaultWebName),
                                argExc);
                    }
                }
            }
            else
            {
                configStoreWeb = configSite.RootWeb;
            }

            return configStoreWeb;
        }

        /// <exception cref="SPSConfigurationManagerException"><c>SPSConfigurationManagerException</c>.</exception>
        private static SPSite GetConfigSiteFromConfiguredUrl()
        {
            SPSite configSite;
            string sOverrideConfigSiteUrl = GetAppSettingsValue(WC_SITE_URL);

            if (sOverrideConfigSiteUrl == null)
            {
                throw new SPSConfigurationManagerException(
                        FormatError(ERR_OUT_CONTEXT_WEB_CONFIG,
                                    WC_SITE_URL));
            }

            if (sOverrideConfigSiteUrl.Length == 0)
            {
                throw new SPSConfigurationManagerException(
                        ERR_INVALID_URL_IN_WEB_CONTEXT);
            }

            try
            {
                configSite = new SPSite(sOverrideConfigSiteUrl);
            }
            catch (FileNotFoundException e)
            {
                throw new SPSConfigurationManagerException(
                        FormatError(ERR_INVALID_URL_IN_WEB_CONTEXT,
                                    sOverrideConfigSiteUrl,
                                    WC_SITE_URL),
                        e);
            }

            return configSite;
        }

        private static string GetAppSettingsValue(string sKey)
        {
            string sValue = null;

            try
            {
                sValue = _reader.GetValue(sKey, typeof(string)) as string;
            }
            catch (InvalidOperationException)
            {
            }

            return sValue;
        }


        private static string FormatError(string errorMessage, params string[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, errorMessage, args);
        }

        #endregion
    }
}