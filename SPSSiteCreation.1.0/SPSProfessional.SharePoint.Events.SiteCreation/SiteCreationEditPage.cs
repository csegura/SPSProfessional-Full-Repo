using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.ConfigurationManager;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    public class SiteCreationEditPage : LayoutsPageBase
    {
        private string _listGuid;
        private SPList _list;

        private SiteCreationEngine _engine;

        protected DropDownList ddlSiteTitleField;
        protected Label lblSiteTitleField;
        protected DropDownList ddlSiteUrlField;
        protected Label lblSiteUrlField;
        protected DropDownList ddlTemplateField;
        protected Label lblTemplateField;
        protected Repeater rptTemplateMap;
        protected Button BtnSave;
        protected Button BtnDelete;
        protected EncodedLiteral encDescription;
        protected CheckBox chkOptHideTemplateField;
        protected CheckBox chkOptLogError;
        protected CheckBox chkOptForceCreation;

        protected CheckBox chkOptOnQuickLaunch;
        protected CheckBox chkOptUseSharedNavBar;
        protected CheckBox chkOptUniquePermissons;
        protected InputFormTextBox txtOptNewPermissions;
        protected CheckBox chkOptAttachDelete;

        #region Private Constants
        private const string MAIN_URL = "SPSProfessional_SiteCreation.aspx";
        #endregion

        #region Overrides of LayoutsPageBase

        protected override bool RequireSiteAdministrator
        {
            get { return true; }
        }

        #endregion

        #region Overrides of UnsecuredLayoutsPageBase

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Check CM
            if (!SPSConfigurationManager.Check())
            {
                throw new ApplicationException("This application requires SPSProfessional Configuration Manager.");
            }

            // Get the list
            _listGuid = SPHttpUtility.UrlKeyValueDecode(Page.Request.QueryString["List"]);

            // Check the list
            if (string.IsNullOrEmpty(_listGuid))
            {
                throw new ArgumentException("Invalid List parameter");
            }

            // Clean
            _listGuid = SPHttpUtility.UrlKeyValueDecode(_listGuid);

            // Get the list
            try
            {
                var listID = new Guid(_listGuid);
                _list = SPContext.Current.Web.Lists[listID];
            }
            catch (IndexOutOfRangeException)
            {
                throw new KeyNotFoundException("List not found");
            }
            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _engine = new SiteCreationEngine(_list);
            //_category = _list.ID.ToString("B");

            if (!Page.IsPostBack)
            {
                //GetDataFromConfigurationManager();

                // No current data
                if (!_engine.ListHasEventHandler())
                {
                    ddlSiteTitleField.Items.Clear();
                    ddlSiteUrlField.Items.Clear();

                    foreach (SPField field in _list.Fields)
                    {
                        if (!field.Hidden && !field.ReadOnlyField)
                        {
                            if (field.Type == SPFieldType.Text)
                            {
                                ddlSiteTitleField.Items.Add(new ListItem(field.Title,field.StaticName));
                            }

                            if (field.Type == SPFieldType.URL)
                            {
                                ddlSiteUrlField.Items.Add(new ListItem(field.Title, field.StaticName));
                            }

                            if (field.Type == SPFieldType.Choice)
                            {
                                ddlTemplateField.Items.Add(new ListItem(field.Title, field.StaticName));
                            }
                        }
                    }

                    if (ddlSiteTitleField.Items.Count == 0)
                    {
                        lblSiteTitleField.Text = "Require almost a field type text to store the Site Title.";
                    }

                    if (ddlSiteUrlField.Items.Count == 0)
                    {
                        lblSiteUrlField.Text = "Require almost a field type hyperlink to store the Url.";
                    }

                    if (ddlTemplateField.Items.Count == 0)
                    {
                        lblTemplateField.Text = "Require almost a field type choice to store the Templates.";
                    }

                    if (ddlSiteTitleField.Items.Count == 0 
                        || ddlSiteUrlField.Items.Count == 0 
                        || ddlTemplateField.Items.Count == 0)
                    {
                        BtnSave.Enabled = false;
                    }

                    BtnDelete.Enabled = false;


                    chkOptHideTemplateField.Checked = false;
                    chkOptOnQuickLaunch.Checked = true;
                    chkOptUseSharedNavBar.Checked = true;


                    chkOptLogError.Checked = true;
                    chkOptForceCreation.Checked = true;
                    
                    BindTemplateMap(true);
                }
                else
                {
                    ddlSiteTitleField.Items.Add(GetFieldNameFromStaticName(_engine.SiteField));
                    ddlSiteTitleField.Enabled = false;
                    ddlSiteUrlField.Items.Add(GetFieldNameFromStaticName(_engine.UrlField));
                    ddlSiteUrlField.Enabled = false;
                    ddlTemplateField.Items.Add(GetFieldNameFromStaticName(_engine.TemplateField));
                    ddlTemplateField.Enabled = false;

                    chkOptHideTemplateField.Checked = _engine.OptHideTemplateField;
                    chkOptHideTemplateField.Enabled = false;

                    chkOptLogError.Checked = _engine.OptLogError;
                    chkOptForceCreation.Checked = _engine.OptForceDup;

                    chkOptOnQuickLaunch.Checked = _engine.OptOnQuickLaunch;
                    chkOptUseSharedNavBar.Checked = _engine.OptUseSharedNavBar;
                    chkOptUniquePermissons.Checked = _engine.OptUniquePermissions;
                    txtOptNewPermissions.Text = _engine.NewPermissions;
                    
                    chkOptAttachDelete.Checked = _engine.OptAttachDelete;
                    chkOptAttachDelete.Enabled = false;

                    BindTemplateMap(false);
                }
            }

            encDescription.Text = string.Format("Selected list: {0} at {1}", _list.Title, _list.ParentWeb.Url);
        }

        #endregion

        //private bool SelectDropDown(ListControl dropDownList, string value)
        //{
        //    ListItem item;

        //    item = dropDownList.Items.FindByValue(value);

        //    if (item != null)
        //    {
        //        dropDownList.SelectedIndex = dropDownList.Items.IndexOf(item);
        //        return true;
        //    }
        //    return false;
        //}

        //private void GetDataFromConfigurationManager()
        //{
        //    _siteField = SPSConfigurationManager.EnsureGetValue(_category, FLD_SITE_NAME);
        //    _urlField = SPSConfigurationManager.EnsureGetValue(_category, FLD_SITE_URL);
        //    _templateField = SPSConfigurationManager.EnsureGetValue(_category, FLD_TEMPLATE);
        //    _templateMap = SPSConfigurationManager.EnsureGetValue(_category, TEMPLATE_MAP);
        //}

        #region Control Related

        private List<SPSKeyValuePair> GetEmptyTemplateMap()
        {
            var choiceTemplateMap = new List<SPSKeyValuePair>();

            if (!string.IsNullOrEmpty(ddlTemplateField.SelectedValue))
            {
                var fieldChoice = _list.Fields.GetFieldByInternalName(ddlTemplateField.SelectedValue) as SPFieldChoice;
                //var fieldChoice = _list.Fields[ddlTemplateField.SelectedValue] as SPFieldChoice;
                
                if (fieldChoice != null)
                {
                    foreach (string choice in fieldChoice.Choices)
                    {
                        choiceTemplateMap.Add(new SPSKeyValuePair(choice, string.Empty));
                    }
                }
            }

            return choiceTemplateMap;
        }

        private List<SPSKeyValuePair> GetTemplateMapFromConfigManager()
        {
            string[] map = _engine.TemplateMap.Split(';');

            var choiceTemplateMap = new List<SPSKeyValuePair>();

            foreach(string choiceTemplate in map)
            {
                if (!string.IsNullOrEmpty(choiceTemplate))
                {
                    string choice = choiceTemplate.Substring(0, choiceTemplate.IndexOf(':'));
                    string template = choiceTemplate.Substring(choiceTemplate.IndexOf(':') + 1);
                    choiceTemplateMap.Add(new SPSKeyValuePair(choice, template));
                }
            }

            return choiceTemplateMap;
        }

        private List<SPSKeyValuePair> GetAvailableSiteTemplates()
        {
            SPWeb web = SPContext.Current.Web;
            SPSite site = web.Site;
            SPWebTemplateCollection siteTemplates = site.GetCustomWebTemplates((uint)web.Locale.LCID);
            var templates = new List<SPSKeyValuePair>();

            foreach(SPWebTemplate template in siteTemplates)
            {
                templates.Add(new SPSKeyValuePair(template.Title, template.DisplayCategory));
            }

            siteTemplates = site.GetWebTemplates((uint) web.Locale.LCID);

            foreach (SPWebTemplate template in siteTemplates)
            {
                if (!template.IsHidden)
                    templates.Add(new SPSKeyValuePair(template.Title, template.DisplayCategory));
            }

            return templates;
        }

        private string GetFieldNameFromStaticName(string fieldStaticName)
        {
            SPField field = _list.Fields.GetFieldByInternalName(fieldStaticName);
            if (field != null)
            {
                return field.Title;
            }
            return fieldStaticName;
        }

        #endregion

        #region Page Events

        protected void ddlTemplateField_SelectedIndexChanged(Object Sender, EventArgs e)
        {
            BindTemplateMap(true);
        }

        private void BindTemplateMap(bool enabled)
        {

            List<SPSKeyValuePair> data = enabled ? GetEmptyTemplateMap() : GetTemplateMapFromConfigManager();

            rptTemplateMap.DataSource = data;
            rptTemplateMap.DataBind();
            
            List<SPSKeyValuePair> templates = GetAvailableSiteTemplates();
            
            for (int i = 0; i < rptTemplateMap.Items.Count; i++)
            {
                RepeaterItem item = rptTemplateMap.Items[i];
                var ddlTemplates = item.FindControl("ddlTemplates") as DropDownList;

                if (ddlTemplates != null)
                {
                    if (enabled)
                    {
                        ddlTemplates.DataSource = templates;
                        ddlTemplates.DataValueField = "Key";
                        ddlTemplates.DataTextField = "Key";
                        ddlTemplates.EnableViewState = true;
                        ddlTemplates.DataBind();
                    }
                    else
                    {
                        ddlTemplates.Items.Add(data[i].Value);
                        ddlTemplates.Enabled = false;
                    }
                }
               
            }
        }

        private string GetTemplateMap()
        {
            List<SPSKeyValuePair> templateMap = GetEmptyTemplateMap();
            int i = 0;

            string map = string.Empty;

            foreach(SPSKeyValuePair mapPair in templateMap)
            {
                var ddlTemplates = rptTemplateMap.Items[i++].FindControl("ddlTemplates") as DropDownList;
                
                if (ddlTemplates != null)
                {
                    map += mapPair.Key + ":" + ddlTemplates.SelectedValue + ";";
                }
            }

            return map;
        }

        protected void BtnSave_Click(Object sender, EventArgs e)
        {
            _engine.SiteField = ddlSiteTitleField.SelectedValue;
            _engine.UrlField = ddlSiteUrlField.SelectedValue;
            _engine.TemplateField = ddlTemplateField.SelectedValue;
            _engine.TemplateMap = GetTemplateMap();
            _engine.OptHideTemplateField = chkOptHideTemplateField.Checked;
            _engine.OptLogError = chkOptLogError.Checked;
            _engine.OptForceDup = chkOptForceCreation.Checked;

            _engine.OptOnQuickLaunch = chkOptOnQuickLaunch.Checked;
            _engine.OptUseSharedNavBar = chkOptUseSharedNavBar.Checked;
            _engine.OptUniquePermissions = chkOptUniquePermissons.Checked;
            _engine.NewPermissions = txtOptNewPermissions.Text;
            _engine.OptAttachDelete = chkOptAttachDelete.Checked;

            _engine.SaveConfiguration();
            
            SPUtility.Redirect(MAIN_URL, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }


        protected void BtnDelete_Click(Object sender, EventArgs e)
        {
            _engine.DeleteConfiguration();
            SPUtility.Redirect(MAIN_URL, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }
      
        protected void CancelButtonClick(object sender, EventArgs e)
        {
            SPUtility.Redirect(MAIN_URL, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        #endregion
    }
}