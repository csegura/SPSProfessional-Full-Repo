using System;
using System.Diagnostics;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    internal class FollowViewControl : SPControl, INamingContainer
    {
        private const string CurrentFolderParameter = "CurrentFolder";
        private const string RootFolderParameter = "RootFolder";
        internal const char SLASH = '/';
        private const string ViewParameter = "View";

        private readonly string _listGuid = string.Empty;
        private readonly string _listViewGuid = string.Empty;

        private SPList _list;
        private SPView _view;
        private SPFolder _folder;

        #region Public Properties

        /// <summary>
        /// Gets the list GUID.
        /// </summary>
        /// <value>The list GUID.</value>
        public string ListGuid
        {
            get { return _listGuid; }
        }

        /// <summary>
        /// Gets the list view GUID.
        /// </summary>
        /// <value>The list view GUID.</value>
        public string ListViewGuid
        {
            get { return _listViewGuid; }
        }

        /// <summary>
        /// Gets or sets the current folder.
        /// </summary>
        /// <value>The current folder.</value>
        protected string CurrentFolder
        {
            get
            {                
                if (ViewState[CurrentFolderParameter] == null)
                {
#if DEBUG
                    Debug.WriteLine("*CurrentFolder:" + GetList().RootFolder.ServerRelativeUrl);
#endif
                    // return list root folder
                    return GetList().RootFolder.ServerRelativeUrl;
                }

#if DEBUG
                Debug.WriteLine("CurrentFolder:" + ViewState[CurrentFolderParameter].ToString());
#endif

                // return saved view satate
                return ViewState[CurrentFolderParameter].ToString();
            }
            set { ViewState[CurrentFolderParameter] = value; }
        }

        //protected string CurrentFolder
        //{
        //    get
        //    {
        //        if (ViewState[CurrentFolderParameter + UniqueID] == null)
        //        {
        //            return GetList().RootFolder.ServerRelativeUrl;
        //        }
        //        return ViewState[CurrentFolderParameter + UniqueID].ToString();
        //    }
        //    set { ViewState[CurrentFolderParameter + UniqueID] = value; }
        //}


        /// <summary>
        /// Gets a value indicating whether this instance is following a root folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if current folder is root folder; otherwise, <c>false</c>.
        /// </value>
        public bool IsRootFolder
        {
            get
            {
                return GetFolder().ServerRelativeUrl == GetList().RootFolder.ServerRelativeUrl;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowViewControl"/> class.
        /// </summary>
        /// <param name="listGuid">The list GUID.</param>
        /// <param name="listViewGuid">The list view GUID.</param>
        /// <exception cref="ArgumentException">listGuid</exception>
        public FollowViewControl(string listGuid, string listViewGuid)
        {
            if (string.IsNullOrEmpty(listGuid))
            {
                throw new ArgumentException("listGuid");
            }

            if (string.IsNullOrEmpty(listViewGuid))
            {
                throw new ArgumentException("listViewGuid");
            }

            _listGuid = listGuid;
            _listViewGuid = listViewGuid;            
        }

        #endregion

        #region Control Override

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            GetFolderFromQueryString();
            base.OnLoad(e);
        }    

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the current folder.
        /// </summary>
        /// <returns>The current folder read from RootFolder url</returns>
        public string GetCurrentFolder()
        {
#if DEBUG
            Debug.WriteLine("+CurrentFolder");
#endif            
            GetFolderFromQueryString();
            return CurrentFolder;
        }

        /// <summary>
        /// Gets the link to parent view.
        /// </summary>
        /// <returns>A link to parent view (current url + RootFolder=Parent + View=CurentView) or 
        /// empty if current folder is the root folder</returns>        
        public string GetLinkToParentView()
        {
            string parentFolder = GetParentFolder();
            return string.IsNullOrEmpty(parentFolder) ? string.Empty : GenerateLinkToView(parentFolder);
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance is doc lib.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is doc lib; otherwise, <c>false</c>.
        /// </value>
        protected bool ListIsDocumentLibrary
        {
            get
            {
                return GetList().BaseTemplate == SPListTemplateType.DocumentLibrary;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is list context.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is list context; otherwise, <c>false</c>.
        /// </value>
        protected bool IsListContext
        {
            get
            {
                return (SPContext.Current.List != null ? true : false);
            }
        }

        /// <summary>
        /// Generates the link to view.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The current page url with the selected folder and view (javascript :EnterFolder)
        /// </returns>
        protected virtual string GenerateLinkToView(string path)
        {
            string hrefArgs = string.Format("{0}?RootFolder={1}&View={2}",
                                            SPSTools.GetCurrentPageBaseUrl(Page),
                                            SPHttpUtility.UrlKeyValueEncode(path),
                                            SPHttpUtility.UrlKeyValueEncode(GetView().ID));

            string url = string.Format("javascript:EnterFolder('{0}');", hrefArgs);
            
            return url;
        }

        /// <summary>
        /// Generates the link to list.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <returns>Link to view url and passed folder path</returns>
        protected virtual string GenerateLinkToList(string path)
        {
            return string.Format("{0}?RootFolder={1}",
                                 SPHttpUtility.UrlKeyValueEncode(GetView().Url),
                                 SPHttpUtility.UrlKeyValueEncode(path));
        }
       

        /// <summary>
        /// Gets the folder from query string.
        /// </summary>
        protected void GetFolderFromQueryString()
        {       
            string folder = Page.Request.QueryString[RootFolderParameter];
            string view = Page.Request.QueryString[ViewParameter];

            if (folder != null && view != null)
            {
                if (SPHttpUtility.UrlKeyValueEncode(GetView().ID) == view)
                {
                    CurrentFolder = SPHttpUtility.UrlKeyValueDecode(folder);
                }
            }        
        }

        /// <summary>
        /// Gets the parent folder.
        /// </summary>
        /// <returns>The parent folder server relative url, empty if the folder is root folder</returns>
        private string GetParentFolder()
        {
            // ensure that we have the correct CurrentFolder
            // it's necesary because this function can be called
            // before load control
            GetFolderFromQueryString();
            return GetFolder().ParentFolder.ServerRelativeUrl;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns>The List</returns>
        protected SPList GetList()
        {
            if (_list == null)
            {
                _list = SPContext.Current.Web.Lists[new Guid(ListGuid)];
            }

            return _list;
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <returns>The View</returns>
        protected SPView GetView()
        {
            if (_view == null)
            {
                _view = GetList().Views[new Guid(ListViewGuid)];
            }

            return _view;
        }

        /// <summary>
        /// Gets the current folder
        /// </summary>
        /// <returns>The folder</returns>
        protected SPFolder GetFolder()
        {
            if (_folder == null)
            {
               _folder = GetList().ParentWeb.GetFolder(CurrentFolder);              
            }
            return _folder;
        }
    }
}