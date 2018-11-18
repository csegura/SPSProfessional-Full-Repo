using System;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    public class SiteCreationPage : LayoutsPageBase
    {
        protected Repeater rptSiteCreationLists;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            rptSiteCreationLists.DataSource = SiteCreationEngine.GetLists();
            rptSiteCreationLists.DataBind();           
        }

    }
}
