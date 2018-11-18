using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    public class SiteCreationFeatureReceiver : SPFeatureReceiver
    {
        #region Events

        /// <summary>
        /// Occurs after a Feature is activated.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;                
            FeatureResourcesHelper.InstallResources(web, properties.Definition.DisplayName);
        }

        /// <summary>
        /// Occurs when a Feature is deactivated.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;
            FeatureResourcesHelper.DeinstallResources(web, properties.Definition.DisplayName);
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