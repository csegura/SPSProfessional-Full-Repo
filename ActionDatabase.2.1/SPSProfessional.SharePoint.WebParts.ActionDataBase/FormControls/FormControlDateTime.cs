using System;
using System.Web.UI;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlDateTime : ActionEditorFormControlBase
    {
        private DateTimeControl _control;

        public FormControlDateTime(Field field) : base(field)
        {
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
            _control = new DateTimeControl
                       {
                               ID = ("c" + _field.Name),
                               ToolTip = _field.Description
                       };

            FormControlDate.InitializeDatePicker(_control);
            return _control;
        }

        protected override void SetControlValue(string value)
        {
            // http://msdn2.microsoft.com/en-us/library/1k1skd40.aspx
            if (_control!=null && !string.IsNullOrEmpty(value))
            {
                _control.SelectedDate = DateTime.Parse(value);
            }

        }

        protected override void GetControlValue()
        {
            if (_control != null)
                Value = _control.SelectedDate.ToString();
        }

        #endregion
    }
}