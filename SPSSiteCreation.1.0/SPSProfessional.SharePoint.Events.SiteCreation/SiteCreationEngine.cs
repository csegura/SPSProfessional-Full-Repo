using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.ConfigurationManager;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{    
    internal class SiteCreationEngine
    {
        private readonly string _category;
        private readonly SPList _list;

        private readonly SPSKeyValueList _properties;
        

        #region Constants

        internal const string FLD_SITE_NAME = "FldSiteName";
        internal const string FLD_SITE_URL = "FldSiteUrl";
        internal const string FLD_TEMPLATE = "FldTemplate";
        internal const string TEMPLATE_MAP = "TemplateMap";

        internal const string OPT_HIDE_TEMPLATE_FIELD = "OptHideTemplateField";
        internal const string OPT_ON_QUICKLAUNCH = "OptOnQuickLaunch";
        internal const string OPT_ON_TOPNAVBAR = "OptUseSharedNavBar";
        internal const string OPT_UNIQUE_PERMISSIONS = "OptUniquePermissions";
        internal const string OPT_NEW_PERMISSIONS = "OptNewPermissions";        
        internal const string OPT_ATTACH_DELETE_SITE = "OptDeleteSite";
        internal const string OPT_LOG_ERROR = "OptLogError";
        internal const string OPT_FORCE_DUP = "OptForceDup";
        
        internal const string CM_UPDATE = "SPSProfessional Site Creation. Don´t edit this value.";

        private const string ERR_EMPTY_TEMPLATE_MAP = "SPSSiteCreation - Empty template map.";
        private const string ERR_INVALID_TEMPLATE_MAP = "SPSSiteCreation - Invalid template map.";

        #endregion

        #region Public Properties

        public string SiteField
        {
            get { return _properties[FLD_SITE_NAME]; }
            set { _properties.ReplaceValue(FLD_SITE_NAME,value);  }
        }
       
        public string UrlField
        {
            get { return _properties[FLD_SITE_URL]; }
            set { _properties.ReplaceValue(FLD_SITE_URL, value); }
        }

        public string TemplateField
        {
            get { return _properties[FLD_TEMPLATE]; }
            set { _properties.ReplaceValue(FLD_TEMPLATE, value); }
        }

        public string TemplateMap
        {
            get { return _properties[TEMPLATE_MAP]; }
            set { _properties.ReplaceValue(TEMPLATE_MAP, value); }
        }

        public bool OptHideTemplateField
        {
            get { return _properties[OPT_HIDE_TEMPLATE_FIELD] == "True"; }
            set { _properties.ReplaceValue(OPT_HIDE_TEMPLATE_FIELD, value.ToString()); }
        }

        public bool OptLogError
        {
            get { return _properties[OPT_LOG_ERROR] == "True"; }
            set { _properties.ReplaceValue(OPT_LOG_ERROR, value.ToString()); }
        }

        public bool OptForceDup
        {
            get { return _properties[OPT_FORCE_DUP] == "True"; }
            set { _properties.ReplaceValue(OPT_FORCE_DUP, value.ToString()); }
        }

        public bool OptOnQuickLaunch
        {
            get { return _properties[OPT_ON_QUICKLAUNCH] == "True"; }
            set { _properties.ReplaceValue(OPT_ON_QUICKLAUNCH,value.ToString());}
        }

        public bool OptUseSharedNavBar
        {
            get { return _properties[OPT_ON_TOPNAVBAR] == "True"; }
            set { _properties.ReplaceValue(OPT_ON_TOPNAVBAR, value.ToString()); }
        }

        public bool OptUniquePermissions
        {
            get { return _properties[OPT_UNIQUE_PERMISSIONS] == "True"; }
            set { _properties.ReplaceValue(OPT_UNIQUE_PERMISSIONS, value.ToString()); }
        }

        public string NewPermissions
        {
            get { return _properties[OPT_NEW_PERMISSIONS]; }
            set { _properties.ReplaceValue(OPT_NEW_PERMISSIONS, value); }
        }

        public bool OptAttachDelete
        {
            get { return _properties[OPT_ATTACH_DELETE_SITE] == "True"; }
            set { _properties.ReplaceValue(OPT_ATTACH_DELETE_SITE, value.ToString()); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteCreationEngine"/> class.
        /// </summary>
        public SiteCreationEngine(SPList list)
        {            
            _list = list;
            _category = list.ID.ToString("B");
            _properties = new SPSKeyValueList();            
            GetDataFromConfigurationManager();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SiteCreationEngine"/> class.
        /// </summary>
        /// <param name="listId">The list id.</param>
        public SiteCreationEngine(Guid listId)
        {
            _category = listId.ToString("B");
            _properties = new SPSKeyValueList();
            GetDataFromConfigurationManager();
        }

        /// <summary>
        /// Gets the lists.
        /// </summary>
        /// <returns>Lists that has our event receiver enabled</returns>
        public static List<SiteCreationList> GetLists()
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;            
            var siteCreationLists = new List<SiteCreationList>();

            foreach(SPList list in SPContext.Current.Web.Lists)
            {
                foreach(SPEventReceiverDefinition definition in list.EventReceivers)
                {
                    if (definition.Assembly == assemblyName)
                    {
                        siteCreationLists.Add(new SiteCreationList(list.Title,list.ID.ToString("B")));
                        break;
                    }
                }
            }

            return siteCreationLists;
        }

        /// <summary>
        /// Lists the has event handler.
        /// </summary>
        /// <returns></returns>
        public bool ListHasEventHandler()
        {
            List<SiteCreationList> lists = GetLists();
            return lists.Exists(
                    list => list.ListName == _list.Title && list.ListID == _list.ID.ToString("B"));
        }


        /// <summary>
        /// Saves the configuration.
        /// </summary>
        public void SaveConfiguration()
        {
            if (!ListHasEventHandler())
            {
                AddItemAddEventHandler();
                ModifyFields(true);

                if (OptAttachDelete)
                {
                    AddItemDeletingEventHandler();
                }                
            }

            // save values in config manager
            foreach (SPSKeyValuePair keyValuePair in _properties)
            {
                SPSConfigurationManager.SetKey(_category, keyValuePair.Key, keyValuePair.Value, CM_UPDATE);
            }
        }

        /// <summary>
        /// Deletes the configuration.
        /// </summary>
        public void DeleteConfiguration()
        {
            if (ListHasEventHandler())
            {
                RemoveEventHandler();
                ModifyFields(false);

                // delete values from config manager
                foreach (SPSKeyValuePair keyValuePair in _properties)
                {
                    SPSConfigurationManager.DeleteKey(_category, keyValuePair.Key);
                }
            }
        }

        /// <summary>
        /// Gets the templates.
        /// </summary>
        /// <returns>A dictionary containing the Field - Template map</returns>
        public Dictionary<string, string> GetTemplates()
        {
            var templateDictionary = new Dictionary<string, string>();

            try
            {
                string[] valueTemplateArray = TemplateMap.Split(';');

                foreach (string valueTemplate in valueTemplateArray)
                {
                    if (!string.IsNullOrEmpty(valueTemplate))
                    {
                        string value = valueTemplate.Substring(0, valueTemplate.IndexOf(':'));
                        string template = valueTemplate.Substring(valueTemplate.IndexOf(':') + 1);

                        templateDictionary.Add(value, template);
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException(ERR_INVALID_TEMPLATE_MAP);
            }

            if (templateDictionary.Count == 0)
            {
                throw new ArgumentOutOfRangeException(ERR_EMPTY_TEMPLATE_MAP);
            }

            return templateDictionary;
        }

        /// <summary>
        /// Removes the event handler.
        /// </summary>
        private void RemoveEventHandler()
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;

            var eventsToDelete = new List<Guid>();

            foreach (SPEventReceiverDefinition eventReceiver in _list.EventReceivers)
            {
                if (eventReceiver.Assembly.Equals(assemblyName))
                {
                    eventsToDelete.Add(eventReceiver.Id);
                }
            }

            foreach (Guid guid in eventsToDelete)
            {
                Debug.WriteLine(string.Format("Deleting event {0}", _list.EventReceivers[guid].Type));
                _list.EventReceivers[guid].Delete();
            }
            
            _list.Update();
            _list.ParentWeb.Update();
        }

        /// <summary>
        /// Adds the event handler.
        /// </summary>
        private void AddItemAddEventHandler()
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            string className = typeof(SiteCreationEventReceiver).FullName;

            _list.EventReceivers.Add(SPEventReceiverType.ItemAdding, assemblyName, className);
        }

        /// <summary>
        /// Adds the event handler.
        /// </summary>
        private void AddItemDeletingEventHandler()
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            string className = typeof(SiteCreationEventReceiver).FullName;

            _list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, assemblyName, className);
        }


        private void GetDataFromConfigurationManager()
        {
            _properties.Add(FLD_SITE_NAME, SPSConfigurationManager.EnsureGetValue(_category, FLD_SITE_NAME));
            _properties.Add(FLD_SITE_URL, SPSConfigurationManager.EnsureGetValue(_category, FLD_SITE_URL));
            _properties.Add(FLD_TEMPLATE, SPSConfigurationManager.EnsureGetValue(_category, FLD_TEMPLATE));
            _properties.Add(TEMPLATE_MAP, SPSConfigurationManager.EnsureGetValue(_category, TEMPLATE_MAP));
            _properties.Add(OPT_LOG_ERROR, SPSConfigurationManager.EnsureGetValue(_category, OPT_LOG_ERROR));
            _properties.Add(OPT_HIDE_TEMPLATE_FIELD, SPSConfigurationManager.EnsureGetValue(_category, OPT_HIDE_TEMPLATE_FIELD));
            _properties.Add(OPT_FORCE_DUP, SPSConfigurationManager.EnsureGetValue(_category, OPT_FORCE_DUP));
            _properties.Add(OPT_ON_QUICKLAUNCH, SPSConfigurationManager.EnsureGetValue(_category, OPT_ON_QUICKLAUNCH));
            _properties.Add(OPT_ON_TOPNAVBAR, SPSConfigurationManager.EnsureGetValue(_category, OPT_ON_TOPNAVBAR));
            _properties.Add(OPT_UNIQUE_PERMISSIONS, SPSConfigurationManager.EnsureGetValue(_category, OPT_UNIQUE_PERMISSIONS));
            _properties.Add(OPT_NEW_PERMISSIONS, SPSConfigurationManager.EnsureGetValue(_category, OPT_NEW_PERMISSIONS));
            _properties.Add(OPT_ATTACH_DELETE_SITE, SPSConfigurationManager.EnsureGetValue(_category, OPT_ATTACH_DELETE_SITE));
        }

        /// <summary>
        /// Modifies the fields.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        private void ModifyFields(bool enabled)
        {
            try
            {
                SPField field = _list.Fields.GetFieldByInternalName(UrlField);
                field.ShowInNewForm = !enabled;
                field.ShowInEditForm = !enabled;
                field.ReadOnlyField = enabled;
                field.ShowInDisplayForm = enabled;
                field.Update();

                if (OptHideTemplateField)
                {
                    field = _list.Fields.GetFieldByInternalName(TemplateField);
                    field.ShowInNewForm = !enabled;
                    field.ShowInEditForm = !enabled;
                    field.ReadOnlyField = enabled;
                    field.ShowInDisplayForm = enabled;
                    field.Update();
                }
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine(ex);
            }           
        }

        public static string GetResourceString(string key)
        {
            const string resourceClass = "$Resources:SPSProfessional.SharePoint.Events.SiteCreation";
            uint lang = SPContext.Current.Web.Language;
            try
            {
                string value = SPUtility.GetLocalizedString(key, resourceClass, lang);
                return value;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return string.Empty;
        }
    }
}
