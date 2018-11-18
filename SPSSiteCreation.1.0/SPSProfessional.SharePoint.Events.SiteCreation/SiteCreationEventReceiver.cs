using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    public class SiteCreationEventReceiver : SPItemEventReceiver
    {
        private const string ERR_LICENSE = "License Error.";

        public override void ItemAdding(SPItemEventProperties properties)
        {
            try
            {
                SPSControlar controlador = new SPSControlar("754806B7-B6B5-444C-BB3A-97B2D016404C",
                                                            "SiteCreationEvent.1.0");

                SiteCreationEngine engine = new SiteCreationEngine(properties.ListId);

                if (controlador.Aceptado())
                {

                    SiteCreationEventActions actions = new SiteCreationEventActions();

                    foreach (DictionaryEntry prop in properties.AfterProperties)
                    {                        
                        Debug.WriteLine(prop.Key + " -> " + prop.Value);
                    }

                    string siteTitleValue = properties.AfterProperties[engine.SiteField].ToString();

                    if (string.IsNullOrEmpty(siteTitleValue))
                    {
                        throw new ArgumentException(SiteCreationEngine.GetResourceString("ErrCantBeEmpty"));
                    }
                    
                    string siteTemplateValue;

                    Dictionary<string, string> templates = engine.GetTemplates();

                    if (engine.OptHideTemplateField)
                    {
                        IEnumerator enumerator = templates.Keys.GetEnumerator();
                        enumerator.MoveNext();
                        siteTemplateValue = enumerator.Current as string;
                    }
                    else
                    {
                        siteTemplateValue = properties.AfterProperties[engine.TemplateField].ToString();
                    }

                    
                    if (string.IsNullOrEmpty(siteTemplateValue) || !templates.ContainsKey(siteTemplateValue))
                    {
                        throw new ArgumentException(SiteCreationEngine.GetResourceString("ErrBadTemplateDefinition"));
                    }

                    DebugData(engine, properties, siteTitleValue, templates[siteTemplateValue]);

                    SPWeb web;

                    try
                    {
                        web = actions.CreateWeb(properties.WebUrl,
                                                siteTitleValue,
                                                templates[siteTemplateValue],
                                                engine.OptUniquePermissions,
                                                engine.OptForceDup);

                        properties.AfterProperties[engine.UrlField] = web.Url;

                        if (engine.OptOnQuickLaunch)
                        {
                            actions.AddOnQuickLaunchBar(web);
                        }

                        actions.SetUseSharedNavbar(web, engine.OptUseSharedNavBar);
                        actions.SetPermissions(web, engine.NewPermissions);
                    }
                    catch (SPSException ex)
                    {
                        Debug.WriteLine(ex.Message);

                        if (engine.OptLogError)
                        {
                            properties.AfterProperties[engine.UrlField] = "http://, " + ex.Message;
                            // + " " + ex.InnerException.Message
                        }
                    }
                }
                else
                {
                    Debug.WriteLine(ERR_LICENSE);
                    properties.AfterProperties[engine.UrlField] = "http://, " + ERR_LICENSE;
                }

                base.ItemAdding(properties);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        /// <summary>
        /// Asynchronous After event that occurs after a new item has been added to its containing object.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPItemEventProperties"></see> object that represents properties of the event handler.</param>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            
            SPList currentList = properties.ListItem.ParentList;
            SiteCreationEngine engine = new SiteCreationEngine(currentList);
            SiteCreationEventActions actions = new SiteCreationEventActions();            
            
            string siteTitleValue = properties.ListItem[engine.SiteField].ToString();
            string siteTemplateValue = properties.ListItem[engine.TemplateField].ToString();
            
            if (string.IsNullOrEmpty(siteTitleValue))
            {
                throw new ArgumentException(SiteCreationEngine.GetResourceString("ErrCantBeEmpty"));                
            }

            Dictionary<string, string> templates = engine.GetTemplates();

            if (!templates.ContainsKey(siteTemplateValue))
            {
                throw new ArgumentException(SiteCreationEngine.GetResourceString("ErrBadTemplateDefinition"));             
            }            

            DebugData(engine, properties, siteTitleValue, templates[siteTemplateValue]);

            SPWeb web;
                        
            web = actions.CreateWeb(properties.WebUrl, 
                                    siteTitleValue, 
                                    templates[siteTemplateValue],
                                    engine.OptUniquePermissions,
                                    engine.OptForceDup);

            properties.ListItem[engine.UrlField] = web.Url;
            properties.ListItem.Update();

            if (engine.OptOnQuickLaunch)
            {
                actions.AddOnQuickLaunchBar(web);
            }

            actions.SetUseSharedNavbar(web,engine.OptUseSharedNavBar);

            base.ItemAdded(properties);
        }

        /// <summary>
        /// Synchronous before event that occurs before an existing item is completely deleted.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPItemEventProperties"></see> object that represents properties of the event handler.</param>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            SPList currentList = properties.ListItem.ParentList;
            SiteCreationEngine engine = new SiteCreationEngine(currentList);
            SiteCreationEventActions actions = new SiteCreationEventActions();

            string siteUrlValue = properties.ListItem[engine.UrlField].ToString();
            //SPFieldUrl fieldUrl = properties.ListItem[engine.UrlField] as SPFieldUrl;

            if (!string.IsNullOrEmpty(siteUrlValue))
            {
                if (siteUrlValue.IndexOf(',') > 0)
                {
                    siteUrlValue = siteUrlValue.Substring(0, siteUrlValue.IndexOf(','));
                }
                actions.DeleteWeb(siteUrlValue);
            }

            base.ItemDeleting(properties);
        }

        private void DebugData(SiteCreationEngine engine, SPItemEventProperties properties, string siteTitleValue, string siteTemplateValue)
        {
            Debug.WriteLine(string.Format("siteTitleField [{0}]", engine.SiteField));
            Debug.WriteLine(string.Format("siteTemplateField [{0}]", engine.TemplateField));
            Debug.WriteLine(string.Format("siteUrlField [{0}]", engine.UrlField));

            Debug.WriteLine(string.Format("siteTitleValue [{0}]", siteTitleValue));
            Debug.WriteLine(string.Format("siteTemplateValue [{0}]", siteTemplateValue));

            Debug.WriteLine(string.Format("webUrl [{0}]", properties.WebUrl));
            Debug.WriteLine(string.Format("listId [{0}]", properties.ListId));
        }
               
    }
}
