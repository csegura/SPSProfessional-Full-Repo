using System;
using System.Diagnostics;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Get the tree and act as HierarchyDataSource
    /// </summary>
    public class SPSHierarchyDataSource : IDisposable
    {
        private SPSHierarchyFilter _filter;
        private SPSTreeNode<ISPSHierarchyNode> _root;
        private readonly SPList _list;
        private readonly SPWeb _web;
        private ISPSCacheService _cacheService;

        #region Internal Properties

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        public SPSTreeNode<ISPSHierarchyNode> Root
        {
            get { return _root ?? TryGetFromCache(); }
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
        /// Gets or sets the cache service.
        /// </summary>
        /// <value>The cache service.</value>
        public ISPSCacheService CacheService
        {
            get { return _cacheService; }
            set { _cacheService = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyDataSource"/> class.
        /// </summary>
        /// <param name="web">The web.</param>
        public SPSHierarchyDataSource(SPWeb web)
        {
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyDataSource"/> class.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        public SPSHierarchyDataSource(SPWeb web, SPList list)
        {
            _web = web;
            _list = list;
        }

        /// <summary>
        /// Tries the get from cache.
        /// </summary>
        /// <returns></returns>
        public SPSTreeNode<ISPSHierarchyNode> TryGetFromCache()
        {
            if (CacheService != null)
            {
                _root = CacheService.Get<SPSTreeNode<ISPSHierarchyNode>>("SPSHDS", GetAll);
            }
            else
            {
                _root = GetAll();
            }

            return _root;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        public SPSTreeNode<ISPSHierarchyNode> GetAll()
        {
            SPSHierarchyFactory factory = new SPSHierarchyFactory(Filter);
         
            if (_web !=null && _list !=null)
            {
                _root = factory.MakeFolderTree(_list);
            }
            else
            {
                _root = factory.MakeWebTree(_web);
            }
            return _root;
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
        ~SPSHierarchyDataSource()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("SPSHierarchyDataSource", "Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
