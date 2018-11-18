using System.IO;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    public static class SPSFeatureReceiverHelper
    {
        private const string RESX_EXTENSION = "*.resx";
        /// <summary>
        /// Installs the resources.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="name">The feature name. (properties.Definition.DisplayName) </param>        
        public static void InstallResources(SPWeb web, string name)
        {
            SPWebApplication webApp = web.Site.WebApplication;

            foreach (SPUrlZone zone in webApp.IisSettings.Keys)
            {
                // determine the source and destination path
                string featureResourcePath = GetFeatureResourcePath(name);
                string globalResourcesPath = GetGlobalResourcesPath(webApp.IisSettings[zone]);

                string[] filePaths = Directory.GetFiles(featureResourcePath, RESX_EXTENSION);

                // copy files
                foreach (string filePath in filePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    File.Copy(filePath, Path.Combine(globalResourcesPath, fileName), true);
                }
            }
        }


        /// <summary>
        /// Deinstalls the resources.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="name">The name.</param>
        public static void DeinstallResources(SPWeb web, string name)
        {
            SPWebApplication webApp = web.Site.WebApplication;

            foreach (SPUrlZone zone in webApp.IisSettings.Keys)
            {
                // determine the source and destination path
                string featureResourcePath = GetFeatureResourcePath(name);
                string globalResourcesPath = GetGlobalResourcesPath(webApp.IisSettings[zone]);

                string[] filePaths = Directory.GetFiles(featureResourcePath, RESX_EXTENSION);

                // delete files
                foreach (string filePath in filePaths)
                {
                    string fileName = Path.Combine(globalResourcesPath, Path.GetFileName(filePath));
                    File.Delete(fileName);
                }
            }
        }

        /// <summary>
        /// Gets the global resources path.
        /// </summary>
        /// <param name="oSettings">The o settings.</param>
        /// <returns>The AppGlobalResourcespath</returns>
        private static string GetGlobalResourcesPath(SPIisSettings oSettings)
        {
            // The settings of the IIS application to update
            // SPIisSettings oSettings = webApp.IisSettings[zone];
            return Path.Combine(oSettings.Path.ToString(), "App_GlobalResources");
        }

        /// <summary>
        /// Gets the feature resource path.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The feature path</returns>
        private static string GetFeatureResourcePath(string name)
        {
            return string.Format("{0}\\FEATURES\\{1}\\Resources",
                                 SPUtility.GetGenericSetupPath("Template"),
                                 name);
        }
    }
}