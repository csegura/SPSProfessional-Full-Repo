using System;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Define a node to use in composite tree 
    /// </summary>
    public class SPSHierarchyNode : ISPSHierarchyNode
    {
        private readonly string _imageUrl;
        private readonly string _urlSegment;
        private readonly string _navigateUrl;
        private readonly string _name;
        private readonly string _openUrl;
        private readonly bool _hasChilds;
        private readonly Guid _siteId;
        private readonly Guid _webId;

        private readonly string _path;
        private readonly SPSHierarchyNodeType _type;

        private readonly Guid _listId;
        private readonly Guid _folderId;

        #region Implementation of ISPSHierarchyNode      

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SPSHierarchyNode(SPSHierarchyNodeType type,
                                Guid siteId,
                                Guid webId,
                                string name,
                                string openUrl,
                                string urlSegment,
                                string navigateUrl,
                                string path,
                                string imageUrl,
                                bool hasChilds)
        {
            _siteId = siteId;
            _imageUrl = imageUrl;
            _urlSegment = urlSegment;
            _navigateUrl = navigateUrl;
            _name = name;
            _openUrl = openUrl;
            _webId = webId;
            _path = path;
            _type = type;
            _hasChilds = hasChilds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SPSHierarchyNode(SPSHierarchyNodeType type,
                                Guid siteId,
                                Guid webId,
                                Guid listId,
                                string name,
                                string openUrl,
                                string urlSegment,
                                string navigateUrl,
                                string path,
                                string imageUrl,
                                bool hasChilds)
                : this(type, siteId, webId, name, openUrl, urlSegment, navigateUrl, path, imageUrl, hasChilds)
        {
            _listId = listId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SPSHierarchyNode(SPSHierarchyNodeType type,
                                Guid siteId,
                                Guid webId,
                                Guid listId,
                                Guid folderId,
                                string name,
                                string openUrl,
                                string urlSegment,
                                string navigateUrl,
                                string path,
                                string imageUrl,
                                bool hasChilds)
                : this(type, siteId, webId, listId, name, openUrl, urlSegment, navigateUrl, path, imageUrl, hasChilds)
        {
            _folderId = folderId;
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageUrl
        {
            get { return _imageUrl; }
        }

        /// <summary>
        /// Gets the URL segment.
        /// </summary>
        /// <value>The URL segment.</value>
        public string UrlSegment
        {
            get { return _urlSegment; }
        }

        /// <summary>
        /// Gets the navigate URL.
        /// </summary>
        /// <value>The navigate URL.</value>
        public string NavigateUrl
        {
            get { return _navigateUrl; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the open URL.
        /// </summary>
        /// <value>The open URL.</value>
        public string OpenUrl
        {
            get { return _openUrl; }
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public SPSHierarchyNodeType NodeType
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public SPSSPProxy Item
        {
            get { return new SPSSPProxy(this); }
        }

        #endregion

        #region Implementation of ISPSSPReference

        /// <summary>
        /// Gets the web ID.
        /// </summary>
        /// <value>The web ID.</value>
        public Guid WebID
        {
            get { return _webId; }
        }

        /// <summary>
        /// Gets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        public Guid SiteID
        {
            get { return _siteId; }
        }

        /// <summary>
        /// Gets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        public Guid ListID
        {
            get { return _listId; }
        }

        /// <summary>
        /// Gets the folder ID.
        /// </summary>
        /// <value>The folder ID.</value>
        public Guid FolderID
        {
            get { return _folderId; }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get { return _path; }
        }

        public bool HasChilds
        {
            get { return _hasChilds; }
        }

        #endregion

        #region Overrides of Object

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return _path + " [" + Name + "]";
        }

        #endregion
    }
}