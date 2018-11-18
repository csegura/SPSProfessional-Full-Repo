using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.Framework.WebPartCache;
using WebPart=System.Web.UI.WebControls.WebParts.WebPart;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// SPSProparts WebPart
    /// </summary>
    public class SPSWebPart : WebPart
    {
        private const string SPS_MESSAGE = "<span><font color={0}>{1} - {2}<br/>{3}</font></span>";
        private const string SPSPROPARTS = "<b>SPSProfessional</b>";
        private const string SPSLICENSED = "Licensed";

        private readonly List<EditorPart> _editorParts;
        private string _spsGuid;
        private string _spsHelpUrl = "http://www.spsprofessional.com/";
        private string _spsPartName;
        private string _spsVersion;
        private string _errorMessage;

        private SPSCacheService _cacheService;
        private int _cacheTimeInSeconds;
        private bool _enableCache;


        private bool Licensed
        {
            get { return (bool) (ViewState[SPSLICENSED] ?? false); }
            set { ViewState[SPSLICENSED] = value; }
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSWebPart"/> class.
        /// </summary>
        public SPSWebPart()
        {
            //ERR_LICENSE_FILE = SPSLocalization.GetResourceString("SPSFW_Err_CheckLicenseFile");
            //ERR_MISSING_CONF = SPSLocalization.GetResourceString("SPSFW_Err_CheckWebPartProperties");
            //SPS_DESIGNMODE = SPSLocalization.GetResourceString("SPSFW_InDesignMode");

            _editorParts = new List<EditorPart>();
            _errorMessage = string.Empty;
        }

        #endregion

        #region WebPart Properties

        /// <summary>
        /// Gets or sets a value indicating whether [enable cache].
        /// </summary>
        /// <value><c>true</c> if [enable cache]; otherwise, <c>false</c>.</value>
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(false)]
        public bool EnableCache
        {
            get { return _enableCache; }
            set { _enableCache = value; }
        }

        /// <summary>
        /// Gets or sets the cache time in seconds.
        /// </summary>
        /// <value>The cache time in seconds.</value>
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue("")]
        public int CacheTimeInSeconds
        {
            get { return _cacheTimeInSeconds; }
            set { _cacheTimeInSeconds = value; }
        }

        #endregion

        #region Initializer

        /// <summary>
        /// Initialize the SPS WebPart
        /// </summary>
        /// <param name="spsGuid">The SPS GUID.</param>
        /// <param name="spsVersion">The SPS version.</param>
        /// <param name="spsName">Name of the SPS.</param>
        /// <param name="spsHelpUrl">The SPS help URL.</param>
        public void SPSInit(string spsGuid, string spsVersion, string spsName, string spsHelpUrl)
        {
            _spsGuid = spsGuid;
            _spsVersion = spsVersion;
            _spsPartName = spsName;
            _spsHelpUrl = spsHelpUrl;
        }

        /// <summary>
        /// SPSs the init.
        /// </summary>
        /// <param name="spsGuid">The SPS GUID.</param>
        /// <param name="spsVersion">The SPS version.</param>
        /// <param name="spsName">Name of the SPS.</param>
        /// <param name="spsHelpUrl">The SPS help URL.</param>
        public void SPSInit(string spsGuid, string spsVersion, string spsName, Uri spsHelpUrl)
        {
            SPSInit(spsGuid, spsVersion, spsName, spsHelpUrl);
        }

        #endregion

        #region SPS PROPERTIES

        /// <summary>
        /// Gets the editor parts.
        /// </summary>
        /// <value>The editor parts.</value>
        public List<EditorPart> EditorParts
        {
            get { return _editorParts; }
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        private bool CheckLicense
        {
            get
            {
                if (!Licensed)
                {
                    SPSControlar controlar = new SPSControlar(_spsGuid, _spsVersion);
                    Licensed = controlar.Aceptado();
                }
                return Licensed;
            }
        }

        /// <summary>
        /// Gets the missing configuration.
        /// </summary> 
        /// <value>The missing configuration.</value>
        protected string MissingConfiguration
        {
            get
            {
                string configureLink =
                        string.Format("<a href=\"javascript:{0};\">{1}</a>",
                                      ToolPane.GetShowExtensibleToolPaneEvent(string.Format(@"'{0}'", UniqueID)),
                                      SPSLocalization.GetResourceString("SPSFW_Err_CheckWebPartProperties"));

                return string.Format(SPS_MESSAGE,
                                     "DarkBlue",
                                     SPSPROPARTS,
                                     _spsPartName,
                                     configureLink.Replace("ExtensibleView", "Edit"));
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the URL to a Help file for a <see cref="T:System.Web.UI.WebControls.WebParts.WebPart"/> 
        /// control.
        /// </summary>
        /// <value></value>
        /// <returns>A string that represents the URL to a Help file. The default value is an empty string ("").</returns>
        /// <exception cref="T:System.ArgumentException">The internal validation system has determined that the URL might contain script attacks.</exception>
        public override string HelpUrl
        {
            get { return _spsHelpUrl; }
            set { base.HelpUrl = value; }
        }


        /// <summary>
        /// Gets the cache service.
        /// </summary>
        /// <value>The cache service.</value>
        public SPSCacheService CacheService
        {
            get { return _enableCache ? _cacheService : null; }
        }

        #region WebPart Override

        /// <summary>
        /// In the constructor add the custom editor Parts
        /// EditorParts.Add(new xxxEditor());
        /// </summary>
        /// <returns></returns>
        public override EditorPartCollection CreateEditorParts()
        {
            if (this is IWebPartCache)
            {
                EditorParts.Add(new SPSCacheEditorPart());
            }

            EditorParts.Add(new SPSAboutEditor());

            // Assign an ID automatically to each editor part
            foreach (EditorPart editorPart in EditorParts)
            {
                editorPart.ID = editorPart.Title + ID;
            }

            return new EditorPartCollection(EditorParts);
        }


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ExportMode = WebPartExportMode.All;
            if (this is IWebPartCache)
            {
                if (EnableCache)
                {
                    _cacheService = new SPSCacheService(CacheTimeInSeconds, UniqueID);
                }
            }
        }

        ///<summary>
        ///Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        ///</summary>
        ///
        ///<param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            if (!Licensed)
            {
                Licensed = CheckLicense;
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Internally we must use SPSRender method
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            // Check Design Mode
            if (DesignMode)
            {
                writer.Write(SPS_MESSAGE,
                             "blue",
                             SPSPROPARTS,
                             _spsPartName,
                             SPSLocalization.GetResourceString("SPSFW_InDesignMode"));
            }
            else
            {
                // Check SPS License
                if (Licensed)
                {
                    try
                    {
                        EnsureChildControls();
                        SPSRender(writer);

                        // Display errors if any
                        if (!string.IsNullOrEmpty(ErrorMessage))
                        {
                            writer.Write(SPS_MESSAGE, "red", SPSPROPARTS, _spsPartName, ErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new SPException(
                                string.Format("Error: {0}\nMessage: {1}\n Trace: {2}\n",
                                              ex.GetType(),
                                              ex.Message,
                                              ex));
                    }
                }
                else
                {
                    writer.Write(SPS_MESSAGE,
                                 "red",
                                 SPSPROPARTS,
                                 _spsPartName,
                                 SPSLocalization.GetResourceString("SPSFW_Err_CheckLicenseFile"));
                }
            }
            //base.Render(writer);
        }

        /// <summary>
        /// Clears the viewstate of child controls.
        /// Call here from EditorPart ApplyChanges 
        /// </summary>
        public void ClearControlState()
        {
            // Clear the control state
            ClearChildControlState();
            // Save the empty state 
            SaveControlState();
            // Discard controls (force creation)
            ChildControlsCreated = false;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            if (this is IWebPartCache)
            {
                if (_cacheService != null)
                {
                    _cacheService.RemoveAll();
                }
            }
        }

        #endregion

        /// <summary>
        /// Our render, override in child classes
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void SPSRender(HtmlTextWriter writer)
        {
        }
    }
}