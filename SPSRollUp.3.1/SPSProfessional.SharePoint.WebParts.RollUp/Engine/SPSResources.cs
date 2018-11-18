using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.WebParts.RollUp.Engine
{
    public sealed class SPSResources
    {
        public static string GetString(string key)
        {
            const string resourceClass = "SPSProfessional.SharePoint.WebParts.RollUp";
            const string resources = "$Resources:";
            uint lang = (uint) System.Globalization.CultureInfo.CurrentCulture.LCID;
            //uint lang = SPContext.Current.Web.Language;
            string value = SPUtility.GetLocalizedString(resources + key, resourceClass, lang);
            return value;
        }

    }
}
