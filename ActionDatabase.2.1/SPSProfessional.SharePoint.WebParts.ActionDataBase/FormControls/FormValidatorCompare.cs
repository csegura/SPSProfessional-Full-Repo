using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormValidatorCompare : ActionEditorFormValidatorBase
    {
        public FormValidatorCompare(Field field, Validator validator) : base(field, validator)
        {
        }

        #region Overrides of ActionEditorFormValidatorBase

        protected override Control CreateValidatorControl()
        {
            InputFormCompareValidator control = new InputFormCompareValidator
                                                {
                                                        ID = _validator.GetHashCode().ToString(),
                                                        Operator = ((ValidationCompareOperator)
                                                                    Enum.Parse(typeof(ValidationCompareOperator),
                                                                               _validator.Operation.ToString()))
                                                };

            if (_validator.Value.StartsWith("@"))
            {
                control.ControlToCompare = "c" + _validator.Value;
            }
            else
            {
                control.ValueToCompare = _validator.Value;
            }

            control.ErrorMessage = _validator.ErrorMessage;
            control.ControlToValidate = "c" + _field.Name;
            control.Display = ValidatorDisplay.Dynamic;
            control.SetFocusOnError = true;
                        
            return control;
        }

        #endregion
    }
}