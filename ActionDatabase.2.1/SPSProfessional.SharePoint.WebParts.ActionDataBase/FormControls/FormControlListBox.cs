using System.Web.UI;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlListBox : ActionEditorFormControlBase
    {
        private ListBox _control;

        public FormControlListBox(Field field) : base(field)
        {
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
            _control = new ListBox();
            _control.ID = "c" + _field.Name;
            _control.ToolTip = _field.Description;
            _control.EnableViewState = true;
            foreach (Item item in _field.ListItems.ItemCollection)
            {
                ListItem listItem = new ListItem(item.Text, item.Value);
                listItem.Selected = item.Selected;
                _control.Items.Add(listItem);
            }
           
            if (_field.ListItems.Multiple)
            {
                _control.SelectionMode = ListSelectionMode.Multiple;
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
            if (_control != null && !string.IsNullOrEmpty(value))
            {
                _control.SelectedValue = value;
            }
        }

        protected override void GetControlValue()
        {
            if (_control != null)
                Value = _control.SelectedValue;
        }

        #endregion
    }
}