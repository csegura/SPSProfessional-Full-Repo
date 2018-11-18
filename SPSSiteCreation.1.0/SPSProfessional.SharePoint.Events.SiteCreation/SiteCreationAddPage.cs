using System;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    public class SiteCreationAddPage : LayoutsPageBase
    {
        private const string MAIN_URL = "SPSProfessional_SiteCreation.aspx";

        protected DropDownList ddlLists;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                foreach (SPList list in SPContext.Current.Web.Lists)
                {
                    if (!list.Hidden)
                    {
                        ddlLists.Items.Add(new ListItem(list.Title, list.ID.ToString("B")));
                    }
                }
            }
        }

        protected void BtnNext_Click(Object sender, EventArgs e)
        {
            string listID = ddlLists.SelectedValue;
            string url = string.Format("SPSProfessional_SiteCreationEdit.aspx?List={0}", 
                                            SPHttpUtility.UrlKeyValueEncode(listID));
            SPUtility.Redirect(url, SPRedirectFlags.RelativeToLayoutsPage, Context);

        }

        protected void CancelButtonClick(object sender, EventArgs e)
        {
            SPUtility.Redirect(MAIN_URL, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }
        
    }
}
