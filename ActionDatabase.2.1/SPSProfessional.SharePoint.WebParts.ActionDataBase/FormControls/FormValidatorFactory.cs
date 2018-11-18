using System.Web.UI;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormValidatorFactory
    {
        public ActionEditorFormValidatorBase CreateValidator(Field field, Validator validator)
        {
            ActionEditorFormValidatorBase control = null;

            switch (validator.Type)
            {
                case ActionEditorTypeValidation.Range:
                    control = new FormValidatorRange(field, validator);
                    break;
                case ActionEditorTypeValidation.RegEx:
                    control = new FormValidatorRegExp(field, validator);
                    break;
                case ActionEditorTypeValidation.Compare:
                    control = new FormValidatorCompare(field, validator);
                    break;
            }

            return control;
        } 
      
        public ActionEditorFormValidatorBase CreateRequiredValidator(Field field, Control control)
        {
            return new FormValidatorRequired(field, control);
        }
    }
}