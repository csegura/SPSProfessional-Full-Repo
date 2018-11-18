using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormValidatorRegExp : ActionEditorFormValidatorBase
    {
        public FormValidatorRegExp(Field field, Validator validator) : base(field, validator)
        {
        }

        #region Overrides of ActionEditorFormValidatorBase

        protected override Control CreateValidatorControl()
        {
            InputFormRegularExpressionValidator control = new InputFormRegularExpressionValidator
                                                          {
                                                                  ID = _validator.GetHashCode().ToString(),
                                                                  ValidationExpression = _validator.Expression,
                                                                  ErrorMessage = _validator.ErrorMessage,
                                                                  ControlToValidate = ("c" + _field.Name),
                                                                  Display = ValidatorDisplay.Dynamic,
                                                                  SetFocusOnError = true
                                                          };
            return control;
        }

        #endregion
    }
}