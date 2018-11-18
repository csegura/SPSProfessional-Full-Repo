using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// Create hierarchy tree nodes
    /// </summary>
    internal sealed class SPSNodeFactory
    {
        private readonly SPSHierarchyFilter _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SPSNodeFactory(SPSHierarchyFilter filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// Makes the web node.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns>A new web node</returns>
        public ISPSHierarchyNode MakeWebNode(SPWeb web)
        {
            ISPSHierarchyNode node = new SPSHierarchyNode(SPSHierarchyNodeType.Web,
                                                          web.Site.ID,
                                                          web.ID,
                                                          web.Title,
                                                          web.Url + "||",
                                                          web.Name,                                                          
                                                          web.ServerRelativeUrl,
                                                          web.ServerRelativeUrl,
                                                          "/_layouts/images/"
                                                          + (string) SPUtility.MapWebToIcon(web).First,
                                                          HasChilds(web));
            return node;
        }

        /// <summary>
        /// Makes the list node.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>A new list node</returns>
        public ISPSHierarchyNode MakeListNode(SPList list)
        {
            
            ISPSHierarchyNode node = new SPSHierarchyNode(
                    SPSHierarchyNodeType.List,
                    list.ParentWeb.Site.ID,
                    list.ParentWeb.ID,
                    list.ID,
                    GetListName(list),
                    list.ParentWeb.Url+"|"+list.Title+"|",
                    list.Title,
                    list.DefaultViewUrl,
                    GetListPath(list),
                    list.ImageUrl,
                    HasChilds(list));
            return node;
        }

        /// <summary>
        /// Makes the folder node.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="folder">The folder.</param>
        /// <returns>A new folder node</returns>
        public ISPSHierarchyNode MakeFolderNode(SPList list, SPFolder folder)
        {
            ISPSHierarchyNode node = new SPSHierarchyNode(
                    SPSHierarchyNodeType.Folder,
                    list.ParentWeb.Site.ID,
                    list.ParentWeb.ID,
                    list.ID,
                    folder.UniqueId,
                    GetFolderName(list, folder),
                    folder.ParentWeb.Url+"|"+list.Title+"|/"+folder.Url,
                    folder.Name,
                    folder.ParentWeb.Url + "/" + folder.Url,
                    GetFolderPath(list, folder),
                    "/_layouts/images/folder.gif",
                    HasChilds(folder));
            return node;
        }

        #region Private Methods

        /// <summary>
        /// Gets the name of the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>The list name</returns>
        private string GetListName(SPList list)
        {
            string value;
            if (_filter.IncludeNumberOfFiles &&
                list.BaseType == SPBaseType.DocumentLibrary)
            {
                value = list.Title + " (" + list.RootFolder.Files.Count + ")";
            }
            // TODO 
            //else if (_filter.IncludeNumberOfFiles)
            //{
            //    value = list.Title + " (" + CountFolderItems() + ")";
            //}
            else
            {
                value = list.Title;
            }
            return value;
        }

        /// <summary>
        /// Gets the list path.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>The list path, depends if filter IncludeWebs</returns>
        private string GetListPath(SPList list)
        {
            string value;
            if (_filter.IncludeWebs)
            {
                value = list.ParentWeb.ServerRelativeUrl + "/" + list.Title;
            }
            else
            {
                value = list.Title;
            }
            return value;
        }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="folder">The folder.</param>
        /// <returns>The Folder name</returns>
        private string GetFolderName(SPList list, SPFolder folder)
        {
            string value;

            if (_filter.IncludeNumberOfFiles)
            {
                if (list.BaseType == SPBaseType.DocumentLibrary)
                {
                    value = folder.Name + " (" + folder.Files.Count + ")";
                }
                else
                {
                    value = folder.Name + " (" + CountFolderItems(list, folder) + ")";
                }
            }
            else
            {
                value = folder.Name;
            }
            return value;
        }

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="folder">The folder.</param>
        /// <returns>The Folder path</returns>
        private string GetFolderPath(SPList list, SPFolder folder)
        {
            string value;
            if (list.BaseType == SPBaseType.DocumentLibrary)
            {
                // changed before folder.Url;
                value = folder.ServerRelativeUrl;
            }
            else
            {
                value = list.ParentWeb.ServerRelativeUrl + "/" + folder.Url;
            }
            return value;
        }

        /// <summary>
        /// Determines whether the specified web has childs.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns>
        /// 	<c>true</c> if the specified web has childs; otherwise, <c>false</c>.
        /// </returns>
        private bool HasChilds(SPWeb web)
        {
            bool availableLists = false;

            if (_filter.IncludeWebs)
            {
                if ((web.GetSubwebsForCurrentUser().Count > 0))
                    return true;
            }

            if (_filter.IncludeLists)
            {
                foreach (SPList list in web.Lists)
                {
                    if (!list.Hidden)
                    {
                        availableLists = true;
                        break;
                    }
                }
            }

            return availableLists;
        }

        /// <summary>
        /// Determines whether the specified list has childs.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>
        /// 	<c>true</c> if the specified list has childs; otherwise, <c>false</c>.
        /// </returns>
        private bool HasChilds(SPList list)
        {
            return (_filter.IncludeFolders && list.Folders.Count > 0);
        }

        /// <summary>
        /// Determines whether the specified folder has childs.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>
        /// 	<c>true</c> if the specified folder has childs; otherwise, <c>false</c>.
        /// </returns>
        private bool HasChilds(SPFolder folder)
        {
            return (_filter.IncludeFolders && folder.SubFolders.Count > 0);
        }

        /// <summary>
        /// Counts the folder items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="folder">The folder.</param>
        /// <returns>Count the items in the folder</returns>
        private int CountFolderItems(SPList list, SPFolder folder)
        {
            SPQuery query = new SPQuery();

            if (folder != null)
            {
                query.Folder = folder;
            }
            else
            {
                query.Folder = list.RootFolder;
            }

            query.Query = "<Where><Neq>" +
                          "<FieldRef Name='ContentType' /><Value Type='Text'>Folder</Value>" +
                          "</Neq></Where>";

            return list.GetItems(query).Count;
        }

        #endregion
    }
}