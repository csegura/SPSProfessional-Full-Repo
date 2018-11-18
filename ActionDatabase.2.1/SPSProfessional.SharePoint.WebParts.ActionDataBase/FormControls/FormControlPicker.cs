using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Controls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Relations;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlPicker : ActionEditorFormControlBase
    {
        private readonly SqlConnection _connection;
        private readonly bool _newRecord;
        private ActionEntityPickerControl _control;

        public FormControlPicker(Field field, SqlConnection connection, bool newRecord) : base(field)
        {
            _connection = connection;
            _newRecord = newRecord;
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
           
            _control = new ActionEntityPickerControl
                       {
                               ID = ("c" + _field.Name),
                               MultiSelect = false
                       };

            if (_field.Required)
            {
                _control.AllowEmpty = false;
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
            if (!string.IsNullOrEmpty(value))
            {
                ActionEntity entity = new ActionEntity(_field.Lookup,
                                                       value,
                                                       _connection);

                ActionEntityDataBase actionEntityDataBase = new ActionEntityDataBase(entity);
                
                _control.ExtendedData = entity;

                // Decorate
                if (!_newRecord)
                {
                    Debug.WriteLine("***** Set Entity ****");
                    actionEntityDataBase.BindActionPicker(_control);
                }
            }

        }

        protected override void GetControlValue()
        {
            ArrayList resolvedEntities = _control.ResolvedEntities;

            if (resolvedEntities.Count == 1)
            {
                PickerEntity entity = (PickerEntity) resolvedEntities[0];
                Value = entity.Key;
            }
        }

        #endregion
    }
}