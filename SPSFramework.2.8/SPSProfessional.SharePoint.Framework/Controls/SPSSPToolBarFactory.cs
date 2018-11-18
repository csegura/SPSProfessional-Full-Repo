using System.Web.UI;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    public class SPSSPToolBarFactory
    {
        public static ToolBar GetToolBar(Page page)
        {
            ToolBar toolbar = (ToolBar)page.LoadControl("~/_controltemplates/ToolBar.ascx");
            return toolbar;
        }

        public static ToolBarButton GetToolBarButton(Page page)
        {
            ToolBarButton toolbutton =
                 (ToolBarButton)page.LoadControl("~/_controltemplates/ToolBarButton.ascx");
            return toolbutton;
        }
    }
}
