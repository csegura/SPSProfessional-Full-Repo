using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Relations;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlLookup : ActionEditorFormControlBase
    {
        private readonly SqlConnection _connection;
        private DropDownList _control;

        public FormControlLookup(Field field, SqlConnection connection) : base(field)
        {
            _connection = connection;
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
            Debug.WriteLine("%%%%%% GenLookup");
            _control = new DropDownList
                       {
                               ID = ("c" + _field.Name),
                               ToolTip = _field.Description
                       };

            if (!string.IsNullOrEmpty(_field.Lookup.DisplayFormat))
            {
                _control.DataTextFormatString = _field.Lookup.DisplayFormat;
            }
            
            return _control;
        }

        protected override string GetProcessedValue()
        {
            ActionEntity actionEntity = new ActionEntity(_field.Lookup, Value, _connection);
            ActionEntityDataBase actionEntityDataBase = new ActionEntityDataBase(actionEntity);

            return (actionEntityDataBase.ResolveActionEntity(Value));    
        }

        protected override void SetControlValue(string value)
        {
            if (_control != null)
            {
                ActionEntity entity = new ActionEntity(_field.Lookup,
                                                       value,
                                                       _connection);

                ActionEntityDataBase actionEntityDataBase = new ActionEntityDataBase(entity);

                // Decorate
                actionEntityDataBase.BindDropDown(_control);

                if (!string.IsNullOrEmpty(value))
                {
                    _control.SelectedValue = value;
                }
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