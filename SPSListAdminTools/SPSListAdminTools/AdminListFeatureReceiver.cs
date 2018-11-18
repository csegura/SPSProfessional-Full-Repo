using System;
using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Admin.ListTools
{
   
    public class AdminListFeatureReceiver : SPFeatureReceiver
    {
        #region Events

        /// <summary>
        /// Occurs after a Feature is activated.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var manager = new SPSTemplateManager();
            var tag = new SPSRegisterableTag
                                      {
                                          TagName = "SPSProfessional",
                                          Assembly = "SPSProfessional.SharePoint.Admin.ListTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4031063ddba1c7c7",
                                          NameSpace = "SPSProfessional.SharePoint.Admin.ListTools",
                                          Src = "",
                                          TagPrefix = "SPS"
                                      };

            manager.AddRegisterTagNamespace(tag);

            if (manager.RenderingTemplateExists("ListForm"))
            {
                string managerOrigine = manager.GetRenderingTemplate("ListForm");
                
                if (managerOrigine.Contains("<SharePoint:ListFieldIterator"))
                {
                    string managerUpdated = managerOrigine.Replace("<SharePoint:ListFieldIterator", "<SPS:AdminFormFieldsIterator");
                    manager.AlterRenderingTemplate("ListForm", managerUpdated);
                    manager.Save(string.Format("DefaultTemplates.ascx.{0}", DateTime.Now.ToString("yyyyMMdd.hhmmss")));
                }
                else
                    throw new Exception("Install is not able to update SharePoint manager file. The ListForm manager is not standard.");
            }
            else
                throw new Exception("Install is not able to update SharePoint manager file. The ListForm manager doesn't exist.");
            
            if (manager.RenderingTemplateExists("ViewSelector"))
            {
                string managerOrigine = manager.GetRenderingTemplate("ViewSelector");
                if (managerOrigine.Contains("<SharePoint:ViewSelectorMenu"))
                {
                    string managerUpdated = managerOrigine.Replace("<SharePoint:ViewSelectorMenu", "<SPS:AdminViewSelectorMenu");
                    manager.AlterRenderingTemplate("ViewSelector", managerUpdated);
                    manager.Save(string.Format("DefaultTemplates.ascx.{0}", DateTime.Now.ToString("yyyyMMdd.hhmmss")));
                }
                else
                    throw new Exception("Install is not able to update SharePoint manager file. The ViewSelector manager is not standard.");
            }
            else
                throw new Exception("Install is not able to update SharePoint manager file. The ViewSelector manager doesn't exist.");
        }

        /// <summary>
        /// Occurs when a Feature is deactivated.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var manager = new SPSTemplateManager();

            var tag = new SPSRegisterableTag
            {
                TagName = "SPSProfessional",
                Assembly = "SPSProfessional.SharePoint.Admin.ListTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4031063ddba1c7c7",
                NameSpace = "SPSProfessional.SharePoint.Admin.ListTools",
                Src = "",
                TagPrefix = "SPS"
            };
           
            manager.RemoveRegisterTagNamespace(tag);

            if (manager.RenderingTemplateExists("ListForm"))
            {
                string managerOrigine = manager.GetRenderingTemplate("ListForm");
                if (managerOrigine.Contains("<SPS:AdminFormFieldsIterator"))
                {
                    string managerUpdated = managerOrigine.Replace("<SPS:AdminFormFieldsIterator", "<SharePoint:ListFieldIterator");
                    manager.AlterRenderingTemplate("ListForm", managerUpdated);
                    manager.Save();
                }
            }

            if (manager.RenderingTemplateExists("ViewSelector"))
            {
                string managerOrigine = manager.GetRenderingTemplate("ViewSelector");
                if (managerOrigine.Contains("<SPS:AdminViewSelectorMenu"))
                {
                    string managerUpdated = managerOrigine.Replace("<SPS:AdminViewSelectorMenu", "<SharePoint:ViewSelectorMenu");
                    manager.AlterRenderingTemplate("ViewSelector", managerUpdated);
                    manager.Save();
                }
            }
        }

        /// <summary>
        /// Occurs after a Feature is installed.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
        }

        /// <summary>
        /// Occurs when a Feature is uninstalled.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
        }

        #endregion
    }
}
