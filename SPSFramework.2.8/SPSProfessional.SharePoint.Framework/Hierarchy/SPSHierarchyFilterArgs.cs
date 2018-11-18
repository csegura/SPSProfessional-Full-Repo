using System;
using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Filter argumnets passed in OnFilter event
    /// </summary>
    public class SPSHierarchyFilterArgs : EventArgs
    {
        private readonly SPList _list;
        private readonly SPWeb _web;
        private readonly SPFolder _folder;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyFilterArgs"/> class.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        /// <param name="folder">The folder.</param>
        public SPSHierarchyFilterArgs(SPWeb web, SPList list, SPFolder folder)
        {
            _web = web;
            _list = list;
            _folder = folder;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the web.
        /// </summary>
        /// <value>The web.</value>
        public SPWeb Web
        {
            get { return _web; }
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>The list.</value>
        public SPList List
        {
            get { return _list; }
        }

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <value>The folder.</value>
        public SPFolder Folder
        {
            get { return _folder; }
        }

        #endregion

    }
}