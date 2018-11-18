using System;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Reference a Sharepoint OM base object
    /// We can instantiate this with SPSSPProxy
    /// </summary>
    public interface ISPSSPReference
    {
        /// <summary>
        /// Gets the web ID.
        /// </summary>
        /// <value>The web ID.</value>
        Guid WebID
        {
            get;
        }

        /// <summary>
        /// Gets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        Guid SiteID
        {
            get;
        }

        /// <summary>
        /// Gets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        Guid ListID
        {
            get;
        }

        /// <summary>
        /// Gets the folder ID.
        /// </summary>
        /// <value>The folder ID.</value>
        Guid FolderID
        {
            get;
        }
    }
}
