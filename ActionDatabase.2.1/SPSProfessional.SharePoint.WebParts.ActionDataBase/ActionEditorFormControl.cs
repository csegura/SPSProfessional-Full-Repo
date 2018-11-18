using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionEditorFormControl : CompositeControl, ISPSErrorControl
    {
        private const string SPSSubSystemName = "ActionEditorFormControl";
        private readonly SqlConnection _connection;

        private readonly Fields _fields;
        private readonly ActionEditorControl _actionEditorControl;
        private readonly ActionEditorFormValues _actionEditorFormValues;
       
        private readonly List<ActionEditorFormControlBase> _formControls;

        #region Properties

        //public DataRow DataRow
        //{
        //    get { return _actionDataBase.DataRow; }
        //}

        public ActionEditorControl EditorControl
        {
            get { return _actionEditorControl; }
        }

        public Fields Fields
        {
            get { return _fields; }
        }
     
        #endregion

        public ActionEditorFormControl(ActionEditorControl actionEditor, ActionDataBase actionDataBase, Fields fields)
        {
            _fields = fields;
            _connection = actionDataBase.Connection;
            _formControls = new List<ActionEditorFormControlBase>();
            _actionEditorControl = actionEditor;
            _actionEditorFormValues = new ActionEditorFormValues(actionEditor);
        }

        #region ISPSErrorControl Members

        public event SPSControlOnError OnError;

        #endregion

        #region Control Overrides   
   
        protected override void OnPreRender(EventArgs e)
        {
            // if we are not in edit mode we can clear controls            
            if (!_actionEditorControl.InEditMode)
            {
                //ClearChildState();
                Controls.Clear();                
            }
            //base.OnPreRender(e);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            Debug.WriteLine("ActionEditorFormControl RenderControl" + _actionEditorControl.FormMode);

            EnsureChildControls();

            switch (_actionEditorControl.FormMode)
            {
                case ActionEditorFormMode.Blank:
                    RenderBlank(writer);                    
                    break;
                case ActionEditorFormMode.View:
                    RenderView(writer);
                    break;
                case ActionEditorFormMode.New:
                    RenderEdit(writer, true);
                    break;
                case ActionEditorFormMode.Edit:
                    RenderEdit(writer, false);
                    break;
            }

            Debug.WriteLine("End ActionEditorFormControl RenderControl");
        }

        protected override void CreateChildControls()
        {
            Debug.WriteLine("ActionEditorFormControl CreateChildControls");

            // create controls
            FormControlFactory controlFactory = new FormControlFactory(_connection, _actionEditorControl.NewFirstTime);

            foreach (Field field in _fields)
            {
                _formControls.Add(controlFactory.CreateFromControl(field));
            }

            // instantiate if edit mode
            if (_actionEditorControl.InEditMode)
            {
                _formControls.ForEach(control => control.InstantiateIn(this));
            }

            Debug.WriteLine("End ActionEditorFormControl CreateChildControls");
        }

        #endregion

        #region Private Methods

        private void RenderBlank(TextWriter writer)
        {
            StringBuilder output = new StringBuilder();

            output.Append("<br/><table border='0' cellspacing='0' width='100%' class='ms-formtable'>");

            foreach (Field filed in _fields)
            {
                output.Append("<tr>");
                output.Append("<td width='190px' valign='top' class='ms-formlabel'>");
                output.Append("<h4 class='ms-standardheader'>");
                output.Append("<nobr>");
                output.Append(filed.Title);
                output.Append("</nobr>");
                output.Append("</h4>");
                output.Append("</td>");

                output.Append("<td width='400px' valign='top' class='ms-formbody'>");
                output.Append("&nbsp;");
                output.Append("</td>");

                output.Append("</tr>");
            }

            output.Append("</table>");

            writer.Write(output.ToString());
        }

        private void RenderView(TextWriter writer)
        {
            StringBuilder output = new StringBuilder();

            output.Append(
                    "<br/><span><table border='0' cellspacing='0' width='100%' class='ms-formtable' style='margin-top: 8px'>");

            foreach (ActionEditorFormControlBase formControl in _formControls)
            {
                if (formControl.Field.View == ActionEditorFieldVisibility.Enabled)
                {
                    output.Append("<tr>");
                    output.Append("<td width='190px' valign='top' class='ms-formlabel'>");
                    output.Append("<h4 class='ms-standardheader'>");
                    output.Append("<nobr>");
                    output.Append(formControl.Field.Title);
                    output.Append("</nobr>");
                    output.Append("</h4>");
                    output.Append("</td>");

                    output.Append("<td width='400px' valign='top' class='ms-formbody'>");
                    output.Append(formControl.GetControlValueForRender());
                    output.Append("</td>");

                    output.Append("</tr>");
                }
            }

            output.Append("</table></span>");
            writer.Write(output.ToString());
        }

        private void RenderEdit(HtmlTextWriter writer, bool isNew)
        {
            try
            {
                writer.WriteLine("<br/><table border='0' cellspacing='0' width='100%' class='ms-formtable'>");

                foreach (ActionEditorFormControlBase formControl in _formControls)
                {
                    Debug.WriteLine(string.Format("RenderEdit: {0}", formControl.Field.Name));

                    ActionEditorFieldVisibility visibilityMode = (isNew ? formControl.Field.New : formControl.Field.Edit);

                    string tableRowStyle = (visibilityMode == ActionEditorFieldVisibility.Hidden)
                                                   ? " style='visibility:hidden;display:none'"
                                                   : string.Empty;

                    writer.WriteLine("<tr {0}>", tableRowStyle);
                    writer.WriteLine("<td width='190px' valign='top' class='ms-formlabel'>");
                    writer.WriteLine("<h4 class='ms-standardheader'>");
                    writer.WriteLine("<nobr>");
                    writer.WriteLine(formControl.Field.Title);

                    if (formControl.Field.Required)
                    {
                        writer.WriteLine("<span class='ms-formvalidation'> *</span>");
                    }

                    writer.WriteLine("</nobr>");
                    writer.WriteLine("</h4>");
                    writer.WriteLine("</td>");

                    writer.WriteLine("<td width='400px' valign='top' class='ms-formbody'>");

                    formControl.RenderControl(writer);

                    if (!string.IsNullOrEmpty(formControl.Field.Description))
                    {
                        writer.WriteLine("<br/>" + formControl.Field.Description);
                    }

                    writer.WriteLine("</td>");
                    writer.WriteLine("</tr>");
                }
                writer.WriteLine("</table>");
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new SPSErrorArgs(SPSSubSystemName, ex.Message, ex));
                }

                DumpException("RenderEdit", ex);
            }
        }

        /// <summary>
        /// Sets the form values.
        /// Call before add the control to parent control with the fieldvalues
        /// </summary>
        public void SetFormValues()
        {
            Debug.WriteLine("SetFormValues");
            EnsureChildControls();
            IDictionary<string, string> _fieldValues;
            _fieldValues = _actionEditorFormValues.ResolveFormValues(GetFormValues());
            _formControls.ForEach(control => control.Value = _fieldValues[control.Field.Name]);
            _formControls.ForEach(control => Debug.WriteLine("- Set " + control.Field.Name));
        }   

        public IDictionary<string, string> GetFormValues()
        {
            Debug.WriteLine("GetFormValues");
            Dictionary<string, string> formValues = new Dictionary<string, string>();
            _formControls.ForEach(control => formValues.Add(control.Field.Name, control.Value));
            _formControls.ForEach(control => Debug.WriteLine("- Get "+control.Field.Name));
            return formValues;
        }

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        #endregion
    }
}