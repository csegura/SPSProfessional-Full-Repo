using System.Data.SqlClient;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    /// <summary>
    /// Generate the web controls for the editor
    /// </summary>
    internal sealed class FormControlFactory
    {
        private readonly SqlConnection _connection;
        private readonly bool _newRecord;

        public FormControlFactory(SqlConnection connection, bool newRecord)
        {
            _connection = connection;
            _newRecord = newRecord;
        }

        #region Factory Method

        /// <summary>
        /// Generates the specified control
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public ActionEditorFormControlBase CreateFromControl(Field field)
        {
            ActionEditorFormControlBase control = null;
            ActionEditorControlsType controlType = field.Control;

            switch (controlType)
            {
                case ActionEditorControlsType.TextBox:
                    control = new FormControlText(field);                        
                    break;
                case ActionEditorControlsType.Memo:
                    control = new FormControlMemo(field);
                    break;
                case ActionEditorControlsType.Date:
                    control = new FormControlDate(field);
                    break;
                case ActionEditorControlsType.DateTime:
                    control = new FormControlDateTime(field);
                    break;
                case ActionEditorControlsType.Lookup:
                    if (field.Lookup.ControlEditor == ActionEditorLookupControl.PickerDataBase)
                    {
                        control = new FormControlPicker(field, _connection, _newRecord);
                    }
                    else
                    {
                        control = new FormControlLookup(field, _connection);
                    }
                    break;
                case ActionEditorControlsType.CheckBox:
                    control = new FormControlCheckBox(field);
                    break;
                case ActionEditorControlsType.ListBox:
                    control = new FormControlListBox(field);
                    break;
                case ActionEditorControlsType.DropDownList:
                    control = new FormControlDropDown(field);
                    break;
            }

            return control;
        }

        #endregion
      
    }
}