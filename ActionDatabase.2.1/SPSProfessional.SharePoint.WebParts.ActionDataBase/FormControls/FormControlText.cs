using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlText : ActionEditorFormControlBase
    {
        private InputFormTextBox _control;

        public FormControlText(Field field) : base(field)
        {
        }

        protected override Control CreateControl()
        {
            _control = new InputFormTextBox
                       {
                               ID = ("c" + _field.Name),
                               ToolTip = _field.Description,
                               EnableViewState = true
                       };

            if (_field.TextBox != null)
            {
                _control.Columns = _field.TextBox.Columns;

                if (_field.TextBox.MaxLength != 0)
                {
                    _control.MaxLength = _field.TextBox.MaxLength;
                }

                if (_field.TextBox.RightToLeft)
                {
                    _control.Direction = ContentDirection.RightToLeft;
                }
            }


            return _control;
        }

        protected override void SetControlValue(string value)
        {
            _control.Text = value;
        }

        protected override void GetControlValue()
        {
            Value = _control.Text;
        }
    }
}