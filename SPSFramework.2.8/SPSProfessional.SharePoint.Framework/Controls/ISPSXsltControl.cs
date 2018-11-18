using System.Web.UI;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    public interface ISPSXsltControl
    {
        void RenderControlInternal(HtmlTextWriter writer);
    }
}