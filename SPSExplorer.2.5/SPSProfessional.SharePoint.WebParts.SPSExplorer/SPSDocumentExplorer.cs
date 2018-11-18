using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Tools;


namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    [DefaultProperty("CategoryFilter")]
    [ToolboxData("<{0}:SPSDocumentExplorer runat=server></{0}:SPSDocumentExplorer>")]
    [XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.SPSExplorer")]
    public class SPSDocumentExplorer : SPSListExplorer
    {
        private bool _showUploadButton;

        #region WebPart Properties

        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(true)]
        public bool ShowUploadButton
        {
            get { return _showUploadButton; }
            set { _showUploadButton = value; }
        }

        #endregion

        public SPSDocumentExplorer()
        {
            SPSInit("8A297689-7031-444C-B1E9-D52FF62EB20D",
                   "SPSExplorer.2.0",
                   "SPSDocumentExplorer WebPart",
                   "http://www.spsprofessional.com/");

            EditorParts.Clear();
            EditorParts.Add(new SPSDocumentExplorerEditorPart());
        }

        #region SPSListExplorer overrides

        internal override void CustomButtons()
        {
            //base.CustomButtons();

            //if (_showUploadButton)
            //{
            //    // Up Folder button
            //    SPLinkButton buttonUpload = new SPLinkButton
            //                                    {
            //                                            Text = SPSResources.GetResourceString("SPS_UploadButton"),
            //                                            ImageUrl = "/_layouts/images/UPLOAD.GIF",
            //                                            HoverCellActiveCssClass = "ms-buttonactivehover",
            //                                            HoverCellInActiveCssClass = "ms-buttoninactivehover",
            //                                            NavigateUrl = "#",
            //                                            OnClientClick = GetLinkToUpload(),
            //                                            EnableViewState = false
            //                                    };
            //}
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (CheckParameters())
            {
                if (!_showUploadButton)
                {
                    HiddeButton(_toolbar, typeof(UploadMenu));
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the link to upload.
        /// </summary>
        /// <returns>javascript navigate to upload page</returns>
        private string GetLinkToUpload()
        {
            string prefixedUrl = string.Format("{0}/_layouts/Upload.aspx?List={1}&RootFolder={2}&Source={3}",
                                               SPContext.Current.Web.ServerRelativeUrl,
                                               SPHttpUtility.UrlKeyValueEncode(
                                                       new Guid(_listGuid).ToString("B").ToUpper()),
                                               _breadCrumb.GetCurrentFolder(),
                                               SPSTools.GetCurrentUrl());
            return string.Format("javascript:STSNavigate('{0}')",
                                 SPHttpUtility.EcmaScriptStringLiteralEncode(
                                         SPUtility.GetServerRelativeUrlFromPrefixedUrl(prefixedUrl)));
        }
    }
}