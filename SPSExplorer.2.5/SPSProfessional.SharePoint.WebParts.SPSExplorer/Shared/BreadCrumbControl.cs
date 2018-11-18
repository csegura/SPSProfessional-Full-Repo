using System;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    internal class BreadCrumbControl : FollowViewControl
    {
        private const string BREADCRUMB_SEPARATOR = " > ";
        private const string HTML_SPACE = "&nbsp;";
        private const string BREADCRUMB_PREVOIUS = "... >";
        private int _maxLevels;
        private bool _navigateToList;
        private string _navigateToListView;
        private string _html = string.Empty;

        #region Constructor

        public BreadCrumbControl(string listGuid, string listViewGuid) : base(listGuid, listViewGuid)
        {
        }

        #endregion

        #region Public Properties

        [Personalizable(PersonalizationScope.Shared)]
        public int MaxLevels
        {
            get { return (_maxLevels < 3 ? 3 : _maxLevels); }
            set { _maxLevels = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool NavigateToList
        {
            get { return _navigateToList; }
            set { _navigateToList = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string NavigateToListView
        {
            get { return _navigateToListView; }
            set { _navigateToListView = value; }
        }

        #endregion

        #region Control Overrides

        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);

            GenerateBreadCrumb();

            if (IsListContext)
            {
                _html = "<div class=ms-listdescription>" + _html + "</div>";
            }

            writer.Write(_html);
        }

        #endregion

        /// <summary>
        /// Generates the bread crumb.
        /// </summary>
        private void GenerateBreadCrumb()
        {
            string[] paths = CurrentFolder.Split(SLASH);
            string fullPath = string.Empty;
            int contextStart = GetCurrentWebLevel();
            int start = contextStart;

            if (paths.Length > MaxLevels + contextStart)
            {
                start = paths.Length - MaxLevels;
            }

            for (int i = 1; i < start && i < paths.Length; i++)
            {
                fullPath += SLASH + paths[i];
            }

            if (paths.Length > MaxLevels + contextStart)
            {
                _html = GenerateLink(BREADCRUMB_PREVOIUS, fullPath);
                _html += HTML_SPACE;
            }

            for (int i = start; i < paths.Length; i++)
            {
                string path = paths[i];

                fullPath += SLASH + path;

                if (!path.Equals("Lists"))
                {
                    _html += GenerateLink(path, fullPath);

                    if (i != paths.Length - 1)
                    {
                        _html += BREADCRUMB_SEPARATOR;
                    }
                }
            }

            if (_html.Length == 0)
            {
                _html = GenerateLink(paths[paths.Length - 1], fullPath);
            }
        }


        /// <summary>
        /// Number of paths levels in the relative url
        /// </summary>
        /// <returns>number of path levels</returns>
        private int GetCurrentWebLevel()
        {
            string relativeUrl = SPContext.Current.Web.ServerRelativeUrl;
            int level = relativeUrl.Split(SLASH).Length;
            return level;
        }

        /// <summary>
        /// Generates the link.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private string GenerateLink(string path, string fullPath)
        {
            string href;

            if (NavigateToList)
            {
                href = GenerateLinkToList(path, fullPath);
            }
            else
            {
                href = GenerateLinkToView(path, fullPath);
            }

            return href;
        }

        /// <summary>
        /// Generates the link to view.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private string GenerateLinkToView(string path, string fullPath)
        {
            // generate url + args
            string hrefArgs = string.Format("{0}?RootFolder={1}&View={2}",
                                            SPSTools.GetCurrentPageBaseUrl(Page),
                                            SPHttpUtility.UrlKeyValueEncode(fullPath),
                                            SPHttpUtility.UrlKeyValueEncode(GetView().ID));

            //  onclick
            string onclick = string.Format("javascript:EnterFolder('{0}');javascript: return false;", hrefArgs);

            string href;

            // different html if we are in list context
            if (IsListContext)
            {
                href = string.Format("<a href=\"{0}\">{1}</a>", hrefArgs, path);
            }
            else
            {
                href = string.Format("<a href=\"{0}\" onclick=\"{1}\">{2}</a>", hrefArgs, onclick, path);
            }

            return href;
        }

        /// <summary>
        /// Generates the link to list.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private string GenerateLinkToList(string path, string fullPath)
        {
            // use base GenerateLinkToList url
            SPView view;
            string hrefArgs;

            if (!string.IsNullOrEmpty(_navigateToListView))
            {
                view = GetList().Views[new Guid(_navigateToListView)];
                hrefArgs = string.Format("{0}?RootFolder={1}&View={2}",
                                         SPHttpUtility.UrlKeyValueEncode(view.Url),
                                         SPHttpUtility.UrlKeyValueEncode(fullPath),
                                         SPHttpUtility.UrlKeyValueEncode(view.ID));
            }
            else
            {
                hrefArgs = GenerateLinkToList(fullPath);
            }

            // adjust html
            string href = string.Format("<a href=\"{0}\">{1}</a>", hrefArgs, path);
            return href;
        }
    }
}