using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormValidatorRequired : ActionEditorFormValidatorBase
    {
        private readonly Control _targetControl;

        public FormValidatorRequired(Field field, Control control) : base(field, null)
        {
            _targetControl = control;
        }

        #region Overrides of ActionEditorFormValidatorBase

        protected override Control CreateValidatorControl()
        {
            InputFormRequiredFieldValidator validator = new InputFormRequiredFieldValidator
                                                        {
                                                                FriendlyNameOfControlToValidate = _field.Title,
                                                                ID = ("vr" + _field.Name)
                                                        };

            if (_field.Control == ActionEditorControlsType.Date || _field.Control == ActionEditorControlsType.DateTime)
            {
                validator.ControlToValidate = _targetControl.Controls[0].ID;
            }
            else
            {
                validator.ControlToValidate = _targetControl.ID;
            }

            // TODO: Localization
            validator.Text = "Required!!";
            validator.Display = ValidatorDisplay.Dynamic;
            validator.SetFocusOnError = true;

            return validator;
        }

        #endregion
    }
}