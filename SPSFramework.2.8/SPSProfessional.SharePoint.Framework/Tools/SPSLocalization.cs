using System;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    internal static class SPSLocalization
    {
        /// <summary>
        /// Gets the resource string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The resource, otherwise the key</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static string GetResourceString(string key)
        {
            const string resourceClass = "SPSProfessional.SharePoint.Framework";
            uint lang = GetLanguage();
            string value = key;

            // Possible SecurityException
            try
            {
                value = SPUtility.GetLocalizedString("$Resources:" + key, resourceClass, lang);
            }
            catch(SecurityException ex)
            {
                Debug.WriteLine(ex);
            }
            
            return value;
        }


        /// <summary>
        /// Gets the language.
        /// Get the language when we are out of web context (ie) event receivers. 
        /// Then default language is used 1033.
        /// </summary>
        /// <returns></returns>
        private static uint GetLanguage()
        {
            uint language;
            try
            {
                language = SPContext.Current.Web.Language;
            }
            catch(NullReferenceException)
            {
                language = 1033;
            }
            return language;
        }
    }
}
