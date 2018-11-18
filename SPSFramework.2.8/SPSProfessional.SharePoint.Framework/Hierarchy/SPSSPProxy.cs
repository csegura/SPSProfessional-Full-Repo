using System;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Proxy to handle internal SharePoint objects
    /// </summary>
    public class SPSSPProxy : IDisposable
    {
        private readonly Guid _siteID;
        private readonly Guid _webID;
        private readonly Guid _folderID;
        private readonly Guid _listID;
        private SPSite _site;
        private SPWeb _web;

        private SPSite Site
        {
            get
            {
                return _site ?? OpenSite();
            }
        }
       
        private SPWeb Web
        {
            get
            {
                return _web ?? OpenWeb();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSSPProxy"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        internal SPSSPProxy(ISPSSPReference node)
        {
            _siteID = node.SiteID;
            _webID = node.WebID;
            _listID = node.ListID;
            _folderID = node.FolderID; 
        }

        /// <summary>
        /// Gets the web.
        /// </summary>
        /// <returns>The web, necesary Dispose</returns>
        public SPWeb RequestWeb()
        {
            return Web;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns>The List</returns>
        public SPList RequestList()
        {
            return Web.Lists[_listID];
        }

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <returns></returns>
        public SPFolder RequestFolder()
        {
            return RequestList().Folders[_folderID].Folder;
        }

        #region Private 

        private SPSite OpenSite()
        {
            _site = new SPSite(_siteID);
            return _site;
        }

        private SPWeb OpenWeb()
        {
            _web = Site.OpenWeb(_webID);
            return _web;
        }

        #endregion

        #region Implementation of IDisposable

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_site != null)
                    {
                        _site.Dispose();
                    }
                    if (_web != null)
                    {
                        _web.Dispose();
                    }
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SPSSPProxy"/> is reclaimed by garbage collection.
        /// </summary>
        ~SPSSPProxy()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("SPSHierarchyItem", "Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
