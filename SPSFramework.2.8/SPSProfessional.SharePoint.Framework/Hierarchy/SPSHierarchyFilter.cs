using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// OnFilter Delegate
    /// </summary>
    public delegate bool HierarchyFilterDelegate(object sender, SPSHierarchyFilterArgs args);

    /// <summary>
    /// Filter for the Hierarchy Data Source
    /// </summary>
    public sealed class SPSHierarchyFilter
    {
        private const int MAX_DEEPH = 99;

        private bool _sortHierarchy;
        private bool _includeFolders;
        private bool _includeItems;
        private bool _includeLists = true;
        private int _maxDeepth = MAX_DEEPH;
        private bool _includeHiddenLists;
        private bool _includeHiddenFolders;
        private bool _includeNumberOfFiles;
        private bool _inlcudeRoot;
        private bool _includeWebs = true;
        private bool _hideUnderscoreFolders = true;

        private bool _recursive = true;

        /// <summary>
        /// Occurs when [on filter].
        /// </summary>
        public event HierarchyFilterDelegate OnFilter;

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether [inlcude root].
        /// </summary>
        /// <value><c>true</c> if [inlcude root]; otherwise, <c>false</c>.</value>
        public bool InlcudeRoot
        {
            get { return _inlcudeRoot; }
            set { _inlcudeRoot = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SPSHierarchyFilter"/> is recursive.
        /// </summary>
        /// <value><c>true</c> if recursive; otherwise, <c>false</c>.</value>
        public bool Recursive
        {
            get { return _recursive; }
            set { _recursive = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include lists].
        /// </summary>
        /// <value><c>true</c> if [include lists]; otherwise, <c>false</c>.</value>
        public bool IncludeLists
        {
            get { return _includeLists; }
            set { _includeLists = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include folders].
        /// </summary>
        /// <value><c>true</c> if [include folders]; otherwise, <c>false</c>.</value>
        public bool IncludeFolders
        {
            get { return _includeFolders; }
            set { _includeFolders = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include items].
        /// </summary>
        /// <value><c>true</c> if [include items]; otherwise, <c>false</c>.</value>
        public bool IncludeItems
        {
            get { return _includeItems; }
            set { _includeItems = value; }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether [include webs].
        ///// </summary>
        ///// <value><c>true</c> if [include webs]; otherwise, <c>false</c>.</value>
        //public bool IncludeWebs
        //{
        //    get { return _includeWebs; }
        //    set { _includeWebs = value; }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether [include hidden lists].
        /// </summary>
        /// <value><c>true</c> if [include hidden lists]; otherwise, <c>false</c>.</value>
        public bool IncludeHiddenLists
        {
            get { return _includeHiddenLists; }
            set { _includeHiddenLists = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include hidden folders].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [include hidden folders]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeHiddenFolders
        {
            get { return _includeHiddenFolders; }
            set { _includeHiddenFolders = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include number of files].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [include number of files]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeNumberOfFiles
        {
            get { return _includeNumberOfFiles; }
            set { _includeNumberOfFiles = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sort hierarchy].
        /// </summary>
        /// <value><c>true</c> if [sort hierarchy]; otherwise, <c>false</c>.</value>
        public bool SortHierarchy
        {
            get { return _sortHierarchy; }
            set { _sortHierarchy = value; }
        }

        /// <summary>
        /// Gets or sets the max deepth.
        /// </summary>
        /// <value>The max deepth.</value>
        public int MaxDeepth
        {
            get { return _maxDeepth; }
            set { _maxDeepth = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include all webs].
        /// </summary>
        /// <value><c>true</c> if [include all webs]; otherwise, <c>false</c>.</value>
        public bool IncludeWebs
        {
            get { return _includeWebs; }
            set { _includeWebs = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hide underscore folders].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [hide underscore folders]; otherwise, <c>false</c>.
        /// </value>
        public bool HideUnderscoreFolders
        {
            get { return _hideUnderscoreFolders; }
            set { _hideUnderscoreFolders = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies the filter to specified web.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns>If pass the filter</returns>
        public bool Apply(SPWeb web)
        {
            if (Filter(web))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Applies the filter to specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>If pass the filter</returns>
        public bool Apply(SPList list)
        {
            if ((!list.Hidden) || IncludeHiddenLists)
            {
                if (Filter(list))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Applies the filter to specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>If pass the filter</returns>
        public bool Apply(SPFolder folder)
        {
            if (!(folder.Item == null) || IncludeHiddenFolders)
            {
                if (Filter(folder))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Filters the specified web.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns>True if pass the filter</returns>
        private bool Filter(SPWeb web)
        {
            if (OnFilter != null)
            {
                return OnFilter(this, new SPSHierarchyFilterArgs(web, null, null));
            }
            return true;
        }

        /// <summary>
        /// Filters the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>True if pass the filter</returns>
        private bool Filter(SPList list)
        {
            if (OnFilter != null)
            {
                bool pass = OnFilter(this, new SPSHierarchyFilterArgs(list.ParentWeb, list, null));
                //EnsureDispose(list.ParentWeb);
                return pass;
            }
            return true;
        }

        /// <summary>
        /// Filters the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>True if pass the filter</returns>
        private bool Filter(SPFolder folder)
        {
            if (OnFilter != null)
            {
                bool pass = OnFilter(this, new SPSHierarchyFilterArgs(folder.ParentWeb, folder.Item.ParentList, folder));
                //EnsureDispose(folder.ParentWeb);
                return pass;
            }
            return true;
        }

        private void EnsureDispose(SPWeb web)
        {
            if (!SPContext.Current.Web.Equals(web))
            {
                web.Dispose();
            }
        }

        #endregion

    }
}