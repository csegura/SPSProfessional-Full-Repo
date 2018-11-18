using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SPSProfessional.SharePoint.Framework.ConfigurationManager;

namespace SPSProfessional.SharePoint.Configuration
{
    public class SPSConfigurationManagerFeatureReceiver : SPFeatureReceiver
    {       
        private const string APP_SETTINGS_XPATH = "/configuration/appSettings";

        private const string MODIFICATION_OWNER = "SPSCM";

        private SPWebConfigModification _siteEntry;
        private SPWebConfigModification _listEntry;
        private SPSite _currentSite;
        private SPWebApplication _webApplication;

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            // Debugger.Launch();

            Initialize(properties);
           
            if (_currentSite != null)
            {
                // web.config modifications
                try
                {                   
                    ModifyWebApplication( _listEntry, false);
                    ModifyWebApplication( _siteEntry, false);

                    UpdateServices();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.TargetSite);
                    Debug.WriteLine(ex);
                }

                // Associate event receivers
                try
                {
                    SPWeb rootWeb = _currentSite.RootWeb;
                    SPList configList = rootWeb.Lists[SPSConfigurationManager.LIST_NAME];
                    configList.Hidden = true;
                    AddEventReceivers(configList);
                    configList.Update();
                }
                catch (Exception ex)
                {                   
                    // if we can't find the list we won't try and add event receivers..
                    Debug.WriteLine(ex.TargetSite);
                    Debug.WriteLine(ex);
                }
            }
        }


        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            // Debugger.Launch();

            Initialize(properties);

            if (_currentSite != null)
            {
                // remove web.config entries
                try
                {
                    ModifyWebApplication( _listEntry, true);
                    ModifyWebApplication( _siteEntry, true);

                    UpdateServices();
                }
                catch (Exception ex)
                {
                    // if we can't find the list we won't try and add event receivers..
                    Debug.WriteLine(ex.TargetSite);
                    Debug.WriteLine(ex);
                }

                // deactivate events
                try
                {
                    SPWeb rootWeb = _currentSite.RootWeb;
                    SPList configList = rootWeb.Lists[SPSConfigurationManager.LIST_NAME];
                    DeleteEventReceivers(configList);
                }
                catch (Exception ex)
                {
                    // if we can't find the list we won't try and add event receivers..
                    Debug.WriteLine(ex.TargetSite);
                    Debug.WriteLine(ex);
                }
            }
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            // nothing to do here.            
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            // nothing to do here..
        }

        private void Initialize(SPFeatureReceiverProperties properties)
        {
            _currentSite = properties.Feature.Parent as SPSite;
            _webApplication = _currentSite.WebApplication;

            _siteEntry = PrepareModification(APP_SETTINGS_XPATH,
                                      SPSConfigurationManager.WC_SITE_URL,
                                      _currentSite.Url);

            _listEntry = PrepareModification(APP_SETTINGS_XPATH,
                                      SPSConfigurationManager.WC_LIST_NAME,
                                      SPSConfigurationManager.LIST_NAME);
            
            ClearWebConfigModifications();
            UpdateServices();
            PrepareSection();
            UpdateServices();
        }

        

        #region Event Receiver
        private void AddEventReceivers(SPList configList)
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            string className = typeof(SPSConfigurationManagerListEventReceiver).FullName;

            configList.EventReceivers.Add(SPEventReceiverType.ItemUpdated, assemblyName, className);
            configList.EventReceivers.Add(SPEventReceiverType.ItemAdded, assemblyName, className);
            configList.EventReceivers.Add(SPEventReceiverType.ItemDeleted, assemblyName, className);
        }

        private void DeleteEventReceivers(SPList configList)
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;

            List<Guid> eventsToDelete = new List<Guid>();

            foreach (SPEventReceiverDefinition eventReceiver in configList.EventReceivers)
            {
                if (eventReceiver.Assembly.Equals(assemblyName))
                {
                    eventsToDelete.Add(eventReceiver.Id);
                }
            }

            foreach (Guid guid in eventsToDelete)
            {
                Debug.WriteLine(string.Format("Deleting event {0}", configList.EventReceivers[guid].Type));
                configList.EventReceivers[guid].Delete();
            }
            
            configList.Update();
        }
        #endregion

        #region Modify web.config
        private void ModifyWebApplication(SPWebConfigModification modification,
                                            bool removeModification)
        {
            //Debugger.Launch();

            Debug.WriteLine("ModifyWebApplication");
            foreach (SPWebConfigModification m in _webApplication.WebConfigModifications)
            {
                Debug.WriteLine("->" + m.Name + " " + m.Value + " " + m.Owner);
            }

            if (removeModification && _webApplication.WebConfigModifications.Contains(modification))
            {
                Debug.WriteLine("* Remove " + modification.Name);
                _webApplication.WebConfigModifications.Remove(modification);
            }
            else
            {
                
                Debug.WriteLine("* Add " + modification.Name);
                _webApplication.WebConfigModifications.Add(modification);
            }            
        }

        private SPWebConfigModification PrepareModification(string xpath, string key, string value)
        {
            string modification = string.Format("add[@key='{0}']", key);
            
            var configMod = new SPWebConfigModification(modification,xpath)
                                {
                                    Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode,
                                    Owner = MODIFICATION_OWNER,
                                    Value = string.Format(CultureInfo.InvariantCulture,
                                                          string.Format("<add key=\"{0}\" value=\"{1}\"/>", key, value)),
                                    Sequence = 0
                                };

            return configMod;
        }


        private void PrepareSection()
        {
            var sectionMod = new SPWebConfigModification("appSettings", "configuration")
                                {
                                    Type = SPWebConfigModification.SPWebConfigModificationType.EnsureSection,
                                    Owner = MODIFICATION_OWNER,
                                    Value = "<appSettings></appSettings>",
                                    Sequence = 0
                                };

            _webApplication.WebConfigModifications.Add(sectionMod);
        }

        private void UpdateServices()
        {
            try
            {
                _webApplication.Update();
                Thread.Sleep(1000);
                Debug.WriteLine("WebApplication updated.");
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Source);
                Debug.WriteLine(ex);
                Debug.WriteLine("WebApplication NOT updated.");
            }

            try
            {
                _webApplication.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                Thread.Sleep(1000);
                Debug.WriteLine("Services updated.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Source);
                Debug.WriteLine(ex);
                Debug.WriteLine("Services NOT updated.");
            }
        }

        private void ClearWebConfigModifications()
        {
            try
            {
                _webApplication.WebConfigModifications.Clear();                
                UpdateServices();
            }
            catch (XmlException)
            {
                Debug.WriteLine("WebConfigModifications cleared.");
                Debug.WriteLine(_webApplication.WebConfigModifications.Count);
            }
        }
        #endregion
    }
}