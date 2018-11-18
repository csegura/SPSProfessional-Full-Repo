using System;
using System.Diagnostics;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.HierarchyOnDemand
{
    /// <summary>
    /// Get the tree and act as HierarchyDataSource
    /// </summary>
    public class SPSHierarchyODDataSource : IDisposable
    {
        private SPSHierarchyFilter _filter;
        private ISPSTreeNode<ISPSHierarchyNode> _root;
        private string _webUrl;
        private readonly string _url;

        #region Internal Properties

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        public ISPSTreeNode<ISPSHierarchyNode> Root
        {
            get { return _root ?? GetAll(); }
        }

        #endregion

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public SPSHierarchyFilter Filter
        {
            get { return _filter ?? new SPSHierarchyFilter(); }
            set { _filter = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyDataSource"/> class.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        public SPSHierarchyODDataSource(string webUrl)
        {
            _webUrl = webUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyDataSource"/> class.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="url">The URL.</param>
        public SPSHierarchyODDataSource(string webUrl, string url)
        {
            _webUrl = webUrl;
            _url = url;
        }


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <exception cref="ArgumentException"><c>ArgumentException</c>.</exception>
        public ISPSTreeNode<ISPSHierarchyNode> GetAll()
        {
            SPSHierarchyODFactory factory = new SPSHierarchyODFactory(Filter);

            string webUrl = _webUrl.TrimEnd('|');
            string listUrl = string.Empty;
            string folderUrl = string.Empty;

            if (!string.IsNullOrEmpty(_url))
            {
                webUrl = _url.Split('|')[0];
                listUrl = _url.Split('|')[1];
                folderUrl = _url.Split('|')[2];
            }

            using(SPWeb web = TryGetWebToUse(webUrl))
            {
                try
                {
                    if (!string.IsNullOrEmpty(listUrl) && !string.IsNullOrEmpty(folderUrl))
                    {
                        SPFolder folder = web.GetFolder(webUrl + folderUrl);
                        _root = factory.MakeFolderNodes(folder);
                    }
                    else if (!string.IsNullOrEmpty(listUrl))
                    {
                        SPList list = web.Lists[listUrl];
                        _root = factory.MakeFolderNodes(list.RootFolder);
                    }
                    else
                    {
                        _root = factory.MakeWebAndListNodes(web);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw new ArgumentException(string.Format(
                                                string.Format("<br/>{0} {1}", 
                                                ex.Message, 
                                                SPSLocalization.GetResourceString("SPSFW_Err_Open_Url")), _url));
                }
            }

            return _root;
        }


        //public ISPSTreeNode<ISPSHierarchyNode> GetAll1()
        //{
        //    SPSHierarchyODFactory factory = new SPSHierarchyODFactory(Filter);

        //    string webUrl = _webUrl;
        //    string listUrl = string.Empty;
        //    string pathUrl = string.Empty;

        //    if (!string.IsNullOrEmpty(_url))
        //    {
        //        webUrl = _url.Split('|')[0];
        //        listUrl = _url.Split('|')[1];
        //        pathUrl = _url.Split('|')[2];
        //    }

        //    //using (SPWeb web = TryGetWebToUse(_webUrl))
        //    using(SPWeb web = TryGetWebToUse(webUrl))
        //    {
        //        try
        //        {
        //            //if (!string.IsNullOrEmpty(_url))
        //            if (!string.IsNullOrEmpty(pathUrl))
        //            {
        //                try
        //                {
        //                    //Object element = web.GetObject(_url);
        //                    Object element = web.GetObject(pathUrl);

        //                    if (element is SPList)
        //                    {
        //                        _root = factory.MakeFolderNodes(((SPList) element).RootFolder);
        //                    }
        //                    else if (element is SPFolder)
        //                    {
        //                        _root = factory.MakeFolderNodes(((SPFolder) element));
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    //try
        //                    //{
        //                    //    _root = factory.MakeFolderNodes(web.GetFolder(_url));
        //                    //}
        //                    //catch (Exception ex)
        //                    //{
        //                    Debug.WriteLine(webUrl);
        //                    Debug.WriteLine(pathUrl);
        //                    Debug.WriteLine(ex);

        //                    //using(SPWeb webWork = TryGetWebToUse(_url))
        //                    using(SPWeb webWork = TryGetWebToUse(webUrl))
        //                    {
        //                        _root = factory.MakeWebAndListNodes(webWork);
        //                    }
        //                    //}
        //                }
        //            }
        //            else
        //            {
        //                _root = factory.MakeWebAndListNodes(web);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("2");
        //            Debug.WriteLine(_url);
        //            Debug.WriteLine(ex);
        //            throw new ArgumentException(
        //                    string.Format(ex.Message + " " + SPSLocalization.GetResourceString("SPSFW_Err_Open_Url"),
        //                                  _url));
        //        }
        //    }

        //    return _root;
        //}

        /// <summary>
        /// Gets the web to use.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><c>ArgumentException</c>.</exception>
        private SPWeb TryGetWebToUse(string webUrl)
        {
            SPWeb web;

            if (!string.IsNullOrEmpty(webUrl))
            {
                try
                {
                    using(SPSite site = new SPSite(webUrl))
                    {
                        web = site.OpenWeb();
                    }
                }
                catch (Exception)
                {
                    throw new ArgumentException(string.Format("<br/>{0}",
                                                string.Format(SPSLocalization.GetResourceString("SPSFW_Err_Open_Url"),
                                                              webUrl)));
                }
            }
            else
            {
                web = SPContext.Current.Web.Site.OpenWeb();
            }

            return web;
        }

        #region IDisposable

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_root != null)
                    {
                        _root = null;
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SPSHierarchyDataSource"/> is reclaimed by garbage collection.
        /// </summary>
        ~SPSHierarchyODDataSource()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("SPSHierarchyODDataSource", "Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}