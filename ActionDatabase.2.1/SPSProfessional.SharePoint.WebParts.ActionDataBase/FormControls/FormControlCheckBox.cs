using System.Web.UI;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;
using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlCheckBox : ActionEditorFormControlBase
    {
        private CheckBox _control;

        public FormControlCheckBox(Field field) : base(field)
        {
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
            _control = new CheckBox
                       {
                               ID = ("c" + _field.Name),
                               ToolTip = _field.Description
                       };
            return _control;
        }

        protected override string GetProcessedValue()
        {
            string valueOut;
            if (!string.IsNullOrEmpty(_field.CheckBox.TextChecked) 
                && !string.IsNullOrEmpty(_field.CheckBox.TextUnChecked))
            {

                if (Value == "1" ||
                    Value == "on" ||
                    Value == "True")
                {
                    valueOut = _field.CheckBox.TextChecked;
                }
                else
                {
                    valueOut = _field.CheckBox.TextUnChecked;
                }
            }
            else
            {
                valueOut = Value;
            }

            return valueOut;
        }

        protected override void SetControlValue(string value)
        {
            if (_control!=null && !string.IsNullOrEmpty(value))
            {
                _control.Checked = (value.Trim() == "on") ||
                                  (value == "1") ||
                                  (value == "True");
            }
        }

        protected override void GetControlValue()
        {
            if (_control != null)
                if (_control.Checked)
                    Value = _field.CheckBox.TextChecked;
                else
                    Value = _field.CheckBox.TextUnChecked;
        }

        #endregion
    }
}