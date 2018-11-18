using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.WebPartCache
{
    internal class SPSCacheEditorPart : EditorPart
    {
        private CheckBox chkEnableCache;
        private TextBox txtCacheTimeInSeconds;
        private string _errorMessage;

        /// <summary>
        /// Initializes the class for use by an inherited class instance. This constructor can be called only by an inherited class.
        /// </summary>
        public SPSCacheEditorPart()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            ID = "CacheEditor";
            Title = SPSLocalization.GetResourceString("SPSFW_CacheEditorPartTitle");
            ChromeState = PartChromeState.Normal;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            try
            {
                SPSWebPart webpart = WebPartToEdit as SPSWebPart;

                if (webpart != null)
                {
                    webpart.EnableCache = chkEnableCache.Checked;

                    int converted;
                    if (int.TryParse(txtCacheTimeInSeconds.Text, out converted))
                    {
                        webpart.CacheTimeInSeconds = converted;
                    }

                    webpart.ClearControlState();
                    webpart.ClearCache();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _errorMessage += ex.ToString();
            }

            return false;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            SPSWebPart webpart = WebPartToEdit as SPSWebPart;

            if (webpart != null)
            {
                chkEnableCache.Checked = webpart.EnableCache;
                txtCacheTimeInSeconds.Text = webpart.CacheTimeInSeconds.ToString();
            }
        }

        protected override void CreateChildControls()
        {
            chkEnableCache = new CheckBox();
            chkEnableCache.Text = SPSLocalization.GetResourceString("SPSFW_EnableCache");
            Controls.Add(chkEnableCache);

            txtCacheTimeInSeconds = new TextBox();
            txtCacheTimeInSeconds.Width = new Unit("100");
            Controls.Add(txtCacheTimeInSeconds);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools tools = new SPSEditorPartsTools(writer);

            tools.DecorateControls(Controls);

            tools.SectionBeginTag();

            tools.SectionHeaderTag();
            chkEnableCache.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSLocalization.GetResourceString("SPSFW_CacheTimeOut"));
            txtCacheTimeInSeconds.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionEndTag();

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                Zone.ErrorText += Environment.NewLine + _errorMessage;
            }
        }
    }
}