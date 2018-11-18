using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionEditorFormValues
    {
        private readonly ActionEditorControl _actionEditorControl;

        public ActionEditorFormValues(ActionEditorControl actionEditorControl)
        {
            _actionEditorControl = actionEditorControl;
        }

        /// <summary>
        /// Sets the form values.
        /// Call before add the control to parent control with the fieldvalues
        /// </summary>
        public IDictionary<string, string> ResolveFormValues(IDictionary<string, string> fieldValues)
        {
            IDictionary<string, string> newFieldValues = new Dictionary<string, string>();
            DataRow dataRow = _actionEditorControl.CurrentDataBase.DataRow;

            Debug.WriteLine("++++++ SET FORM VALUES ++++++");
            Debug.WriteLine(fieldValues == null ? "NULL VALUES" : "FIELD VALUES");

            foreach (Field field in _actionEditorControl.EditorFields)
            {
                // In new mode, the first time set default values
                if (_actionEditorControl.NewFirstTime)
                {
                    SetFieldValueDefaultNew(newFieldValues, field);
                }
                        // In enter-edit mode get from data row
                else if (dataRow != null &&
                         _actionEditorControl.PreviousDataBaseOperation != ActionEditorFormMode.Edit)
                {
                    SetFieldValueFromDataRow(newFieldValues, field, dataRow);
                }
                        // Finally try to get values from previous loaded values
                        // re-edit
                else if (fieldValues != null &&
                         fieldValues.ContainsKey(field.Name) &&
                         !string.IsNullOrEmpty(fieldValues[field.Name]))
                {
                    SetFieldValueFromForm(newFieldValues, field, fieldValues);
                }
                else
                {
                    SetFieldValueEmpty(newFieldValues, field);
                }
            }
            return newFieldValues;
        }

        private void SetFieldValueEmpty(IDictionary<string, string> newFieldValues, Field field)
        {
            // Finally setup empty
            newFieldValues.Add(field.Name, string.Empty);
            Debug.WriteLine(string.Format("SetFieldValueEmpty - {0,-20}", field.Name));
        }

        private void SetFieldValueFromForm(IDictionary<string, string> newFieldValues,
                                           Field field,
                                           IDictionary<string, string> fieldValues)
        {
            newFieldValues.Add(field.Name, fieldValues[field.Name]);
            Debug.WriteLine(
                    string.Format("SetFieldValueFromForm - {0,-20} -> [{1}]",
                                  field.Name,
                                  fieldValues[field.Name]));
        }

        private void SetFieldValueFromDataRow(IDictionary<string, string> newFieldValues, Field field, DataRow dataRow)
        {
            newFieldValues.Add(field.Name, dataRow[field.Name].ToString());
            Debug.WriteLine(
                    string.Format("SetFieldValueFromDataRow - {0,-20} -> [{1}]", field.Name, dataRow[field.Name]));
        }

        private void SetFieldValueDefaultNew(IDictionary<string, string> newFieldValues, Field field)
        {
            ActionFunctions functions = new ActionFunctions();
            newFieldValues.Add(field.Name, functions.Evaluate(field.DefaultValue));
            Debug.WriteLine(
                    string.Format("SetFieldValueDefaultNew - {0,-20} -> [{1}]",
                                  field.Name,
                                  functions.Evaluate(field.DefaultValue)));
        }

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }
    }
}