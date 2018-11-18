using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormValidatorRange : ActionEditorFormValidatorBase
    {
        public FormValidatorRange(Field field, Validator validator) : base(field, validator)
        {
        }

        #region Overrides of ActionEditorFormValidatorBase

        protected override Control CreateValidatorControl()
        {
            InputFormRangeValidator control = new InputFormRangeValidator
                                              {
                                                      ID = _validator.GetHashCode().ToString(),
                                                      MaximumValue = _validator.MaxValue,
                                                      MinimumValue = _validator.MinValue,
                                                      Type = ((ValidationDataType)
                                                              Enum.Parse(typeof(ValidationDataType),
                                                                         _validator.DataType.ToString())),
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