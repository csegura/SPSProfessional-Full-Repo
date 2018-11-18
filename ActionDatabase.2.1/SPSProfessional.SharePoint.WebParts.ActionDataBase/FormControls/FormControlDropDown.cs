using System.Web.UI;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlDropDown : ActionEditorFormControlBase
    {
        private DropDownList _control;
        
        public FormControlDropDown(Field field) : base(field)
        {
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
            _control = new DropDownList
                       {
                               ID = ("c" + _field.Name),
                               ToolTip = _field.Description,
                               EnableViewState = true
                       };

            foreach (Item item in _field.ListItems.ItemCollection)
            {
                ListItem listItem = new ListItem(item.Text, item.Value)
                                    {
                                            Selected = item.Selected
                                    };
                _control.Items.Add(listItem);
            }
           
            return _control;
        }

        protected override string GetProcessedValue()
        {
            string valueOut = Value;
            foreach (Item item in _field.ListItems)
            {
                if (item.Value == Value)
                {
                    valueOut = item.Text;
                }
            }

            return valueOut;
        }

        protected override void SetControlValue(string value)
        {
            if (_control!=null && !string.IsNullOrEmpty(value))
            {
                _control.SelectedValue = value;
            }
        }

        protected override void GetControlValue()
        {
            if (_control != null)
            {
                Value = _control.SelectedValue;
            }
        }

        #endregion
    }
}