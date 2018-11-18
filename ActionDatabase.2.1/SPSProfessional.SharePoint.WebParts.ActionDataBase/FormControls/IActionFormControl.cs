using System.Web.UI;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    interface IActionEditorFormControl
    {
        Control FieldControl 
        {
            get; 
        }

        Field Field
        {
            get;
        }
    }
}
