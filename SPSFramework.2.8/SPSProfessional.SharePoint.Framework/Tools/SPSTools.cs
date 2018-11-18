// File : SPSTools.cs
// Date : 29/07/2008
// User : csegura
// Logs

using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    public enum WssVersion
    {
        Wss3,
        Wss3Sp1,
        Wss3Sp2,
        Unknow,
        WssAgust2008
    }

    public static class SPSTools
    {
        /// <summary>
        /// Determines whether [is I e55 plus] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// 	<c>true</c> if [is I e55 plus] [the specified context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIE55Plus(HttpContext context)
        {
            if (context != null)
            {
                if (context.Request == null)
                {
                    return true;
                }
                HttpBrowserCapabilities browser = context.Request.Browser;
                if (browser == null)
                {
                    return true;
                }
                if (((browser.Type.IndexOf("IE") >= 0) && browser.Win32)
                    && ((browser.MajorVersion >= 6) 
                    || ((browser.MajorVersion >= 5) 
                    && (browser.MinorVersion >= 0.5))))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the current page URL.
        /// </summary>
        /// <returns>The page url without parameters</returns>        
        public static string GetCurrentPageBaseUrl(Page page)
        {
            string currentPageUrl = page.Request.Url.ToString();

            if (currentPageUrl.IndexOf('?') > 0)
            {
                currentPageUrl = currentPageUrl.Substring(0, currentPageUrl.IndexOf('?'));
            }

            return currentPageUrl;
        }

        /// <summary>
        /// Gets the current URL.
        /// </summary>
        /// <returns>The current full Url</returns>
        public static string GetCurrentUrl()
        {
            return
                    SPHttpUtility.UrlKeyValueEncode(
                            SPContext.Current.Web.Site.MakeFullUrl(SPUtility.OriginalServerRelativeRequestUrl));
        }

        public static string GetWssVersion()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SPContext));
// ReSharper disable AssignNullToNotNullAttribute
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(assembly.Location);
// ReSharper restore AssignNullToNotNullAttribute
            return version.FileVersion;
        }

        /// <summary>
        /// http://sharepointworks.blogspot.com/2009/05/todos-los-service-packs-y-parches.html
        /// </summary>
        /// <returns></returns>
        public static WssVersion GetSharePointVersion()
        {
            string version = GetWssVersion();

            switch (version)
            {
                case "12.0.4518.1016":
                    return WssVersion.Wss3;
                case "12.0.6219.1000":
                    return WssVersion.Wss3Sp1;
                case "12.0.6327.5000":
                    return WssVersion.WssAgust2008;
                case "12.0.6421.1000":
                    return WssVersion.Wss3Sp2;
            }
            return WssVersion.Unknow;
        }
    }
}