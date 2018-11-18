// File : SPSListTools.cs
// Date : 28/07/2008
// User : csegura
// Logs

using System;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using WebPart=Microsoft.SharePoint.WebPartPages.WebPart;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    public class SPSListTools
    {

        /// <summary>
        /// Determines whether [is list document library] [the specified list ID].        
        /// </summary>
        /// <remarks>
        /// Search for listID is in current web
        /// </remarks>
        /// <param name="listID">The list ID.</param>
        /// <returns>
        /// 	<c>true</c> if [is list document library] [the specified list ID]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsListDocumentLibrary(Guid listID)
        {
            bool isDocumentLibrary = false;
            SPWeb web = SPContext.Current.Web;

            // check if is a document library
            if (web.Lists[listID].BaseTemplate == SPListTemplateType.DocumentLibrary)
            {
                isDocumentLibrary = true;
            }

            return isDocumentLibrary;
        }

        /// <summary>
        /// Get the ListGuid from a ListViewWebPart that contains a specified ViewGuid
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="viewGuid">View Guid</param>
        /// <returns>The Guid of the List, null if not found</returns>
        internal static string GetListGuidFromListViewGuid(HttpContext context, string viewGuid)
        {
            SPWeb curWeb = SPContext.Current.Web;

            using (SPLimitedWebPartManager webpartManager =
                    curWeb.GetLimitedWebPartManager(context.Request.Url.ToString(),
                                                    PersonalizationScope.Shared))
            {
                foreach (WebPart webpart in webpartManager.WebParts)
                {
                    ListViewWebPart listViewWebPart = webpart as ListViewWebPart;

                    // if the list is a ListView WebPart
                    if (listViewWebPart != null)
                    {
                        // and is the view
                        if (listViewWebPart.ViewGuid == viewGuid)
                        {
                            return listViewWebPart.ListName;
                        }
                    }
                    webpart.Dispose();
                }
                webpartManager.Web.Dispose();
            }

            return null;
        }
    }
}
