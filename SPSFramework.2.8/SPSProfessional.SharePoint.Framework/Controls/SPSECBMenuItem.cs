// File : SPSECBMenuItem.cs
// Date : 28/07/2008
// User : csegura
// Logs

using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    public sealed class SPSECBMenuItem
    {
        private string _title;
        private string _image;
        private string _urlAction;
        private string _listGuid;
        private SPBasePermissions _rights;
        private int _sequence;

        #region Public Properties

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string UrlAction
        {
            get { return UrlActionProcessed(_urlAction); }
            set { _urlAction = value; }
        }

        public string ListGuid
        {
            get { return _listGuid; }
            set { _listGuid = value; }
        }

        public SPBasePermissions Rights
        {
            get { return _rights; }
            set { _rights = value; }
        }

        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SPSECBMenuItem(string title,
                              string image,
                              string urlAction,
                              string listGuid,
                              SPBasePermissions rights,
                              int sequence)
        {
            _title = title;
            _image = image;
            _urlAction = urlAction;
            _listGuid = listGuid;
            _rights = rights;
            _sequence = sequence;
        }


        /// <summary>
        /// URLs the action processed.
        /// </summary>
        /// <param name="urlAction">The URL action.</param>
        /// <returns>The url</returns>
        private string UrlActionProcessed(string urlAction)
        {
            if (urlAction.StartsWith("~sitecollection"))
            {
                string siteUrl = SPContext.Current.Site.ServerRelativeUrl;
                urlAction = urlAction.Replace("~sitecollection", siteUrl);
            }
            else if (urlAction.StartsWith("~site"))
            {
                string siteUrl = SPContext.Current.Web.ServerRelativeUrl;
                urlAction = urlAction.Replace("~site", siteUrl);
            }
            return urlAction;
        }
    }
}