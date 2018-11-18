namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISPSHierarchyNode : ISPSSPReference
    {
        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        string ImageUrl
        {
            get;
        }

        /// <summary>
        /// Gets the URL segment.
        /// </summary>
        /// <value>The URL segment.</value>
        string UrlSegment
        {
            get;
        }

        /// <summary>
        /// Gets the navigate URL.
        /// </summary>
        /// <value>The navigate URL.</value>
        string NavigateUrl
        {
            get;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name
        {
            get;
        }

        string OpenUrl
        {
            get;
        }
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        string Path
        {
            get;
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        SPSHierarchyNodeType NodeType
        {
            get;
        }

        bool HasChilds
        {
            get;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        SPSSPProxy Item
        {
            get;
        }
    }
}