using System.Collections.Generic;
using SPSProfessional.SharePoint.Framework.ConfigurationManager;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.WebParts.RollUp.Engine
{
    internal sealed class SPSRollUpOptions
    {
        private readonly string _topSite;
        private readonly int _maxRecords;
        private readonly bool _camlQueryRecursive;
        private readonly bool _includeListData;
        private readonly List<string> _listsList;
        private readonly List<string> _fieldsList;
        private readonly string _sortFields;
        private readonly bool _useDateIso;
        private readonly bool _fixLookups;
        private readonly ISPSCacheService _cacheService;
        private const string CM_Category = "SPSRollUp";

        public SPSRollUpOptions(string topSite,
                                string lists,
                                string fields,
                                string sortFields,
                                int maxResults,
                                bool camlQueryRecursive,
                                bool includeListData,
                                bool dateTimeISO,
                                bool fixLookUp,
                                ISPSCacheService cacheService)
        {
            _topSite = CheckInConfigurationManager(topSite);
            _maxRecords = maxResults;
            _camlQueryRecursive = camlQueryRecursive;
            _includeListData = includeListData;
            _sortFields = sortFields;

            lists = CheckInConfigurationManager(lists);
            _listsList = new List<string>(lists.Split(','));

            if (fields != null)
            {
                _fieldsList = new List<string>(fields.Split(','));
            }

            _useDateIso = dateTimeISO;
            _fixLookups = fixLookUp;
            _cacheService = cacheService;
        }

        public SPSRollUpOptions(string topSite,
                                string lists,
                                bool camlQueryRecursive) : this(topSite,
                                                                lists,
                                                                null,
                                                                null,
                                                                0,
                                                                camlQueryRecursive,
                                                                false,
                                                                false,
                                                                false,
                                                                null)
        {
        }

        public SPSRollUpOptions(string topSite,
                                string lists,
                                string fields,
                                bool camlQueryRecursive)
            : this(topSite,
                   lists,
                   fields,
                   null,
                   0,
                   camlQueryRecursive,
                   false,
                   false,
                   false,
                   null)
        {
        }
        public string TopSite
        {
            get { return _topSite; }
        }

        public ISPSCacheService CacheService
        {
            get { return _cacheService; }
        }

        public bool FixLookups
        {
            get { return _fixLookups; }
        }

        public bool UseDateISO
        {
            get { return _useDateIso; }
        }

        public IEnumerable<string> FieldsList
        {
            get { return _fieldsList; }
        }

        public IList<string> ListsList
        {
            get { return _listsList.AsReadOnly(); }
        }

        public bool IncludeListData
        {
            get { return _includeListData; }
        }

        public bool CamlQueryRecursive
        {
            get { return _camlQueryRecursive; }
        }

        public int MaxRecords
        {
            get { return _maxRecords; }
        }

        public string SortFields
        {
            get { return _sortFields; }
        }

        /// <summary>
        /// Checks the in configuration manager.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CheckInConfigurationManager(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string key = text.Substring(0, 1);

                if (key == ":")
                {
                    return SPSConfigurationManager.EnsureGetValue(CM_Category, text.Substring(1));
                }
            }
            return text;
        }
    }
}