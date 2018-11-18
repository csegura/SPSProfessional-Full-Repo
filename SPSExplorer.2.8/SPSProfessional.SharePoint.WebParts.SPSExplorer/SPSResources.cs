using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    internal class SPSResources
    {
        public static string GetResourceString(string key)
        {
            const string resourceClass = "SPSProfessional.SharePoint.WebParts.SPSExplorer";
            uint lang = SPContext.Current.Web.Language;
            string value = SPUtility.GetLocalizedString("$Resources:" + key, resourceClass, lang);
            return value;
        }
    }
}
