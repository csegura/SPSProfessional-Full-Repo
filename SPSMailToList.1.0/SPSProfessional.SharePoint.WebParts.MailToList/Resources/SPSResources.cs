using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.WebParts.MailToList.Resources
{
    internal class SPSResources
    {
        public static string GetResourceString(string key)
        {
            const string resourceClass = "SPSProfessional.SharePoint.WebParts.MailToList";
            uint lang = (uint)System.Globalization.CultureInfo.CurrentCulture.LCID;            
            string value = SPUtility.GetLocalizedString("$Resources:" + key, resourceClass, lang);
            return value;           
        }
    }
}