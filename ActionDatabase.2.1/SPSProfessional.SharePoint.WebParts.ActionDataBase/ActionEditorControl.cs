using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionEditorControl : CompositeControl, IPostBackEventHandler, ISPSErrorControl
    {
        private ActionEditorFormControl _actionEditorForm;
        private ActionEditorToolBarControl _actionToolBar;
        private ActionDataBase _actionDataBase;
        private readonly SPSActionEditConfig _config;

        // To expansion 
        public event EventHandler CommandEvent;

        #region Constructor

        public ActionEditorControl(SPSActionEditConfig config)
        {
            Debug.WriteLine("ActionEditorControl: Constructor ");
            _config = config;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The current form mode 
        /// ActionEditorForm (the view) gets the form mode from here
        /// </summary>
        public ActionEditorFormMode FormMode
        {
            get
            {
                return ViewState["FormMode"] != null
                               ? (ActionEditorFormMode) ViewState["FormMode"]
                               : ActionEditorFormMode.View;
            }
            set { ViewState["FormMode"] = value; }
        }

        /// <summary>
        /// Database operation pending
        /// </summary>
        public ActionDataBaseOperation DataBaseOperation
        {
            get
            {
                return ViewState["DataBaseOperaion"] != null
                               ? (ActionDataBaseOperation) ViewState["DataBaseOperaion"]
                               : ActionDataBaseOperation.Read;
            }
            set
            {
                ViewState["PreviousDataBaseOperation"] = ViewState["DataBaseOperaion"];
                ViewState["DataBaseOperaion"] = value;
            }
        }

        /// <summary>
        /// Gets the previous data base operation.
        /// </summary>
        /// <value>The previous data base operation.</value>
        public ActionEditorFormMode PreviousDataBaseOperation
        {
            get
            {
                return ViewState["PreviousDataBaseOperation"] != null
                               ? (ActionEditorFormMode) ViewState["PreviousDataBaseOperation"]
                               : ActionEditorFormMode.View;
            }
        }

        public bool NewFirstTime
        {
            get
            {
                return (FormMode == ActionEditorFormMode.New &&
                        PreviousDataBaseOperation != ActionEditorFormMode.New);
            }
        }

        private SPSKeyValueList _filterValues;

        /// <exception cref="ArgumentNullException"><c>value</c> is null.</exception>
        public SPSKeyValueList FilterValues
        {
            get
            {
                if (_filterValues == null)
                {
                    try
                    {
                        if (ViewState["FilterValues"] != null)
                        {
                            //TODO
                            _filterValues = SPSKeyValueList.Deserialize((string) ViewState["FilterValues"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                return _filterValues;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                //TODO
                ViewState["FilterValues"] = SPSKeyValueList.Serialize(value);
            }
        }

        public ActionDataBase CurrentDataBase
        {
            get { return _actionDataBase; }
        }

        public Fields EditorFields
        {
            get { return _config.Fields; }
        }

        public bool InEditMode
        {
            get
            {
                return (FormMode == ActionEditorFormMode.Edit ||
                        FormMode == ActionEditorFormMode.New);
            }
        }

        #endregion

        #region Control Methods

        ///<summary>
        ///Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        ///</summary>
        ///
        ///<param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data. </param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _actionDataBase = new ActionDataBase(_config);
            _actionDataBase.OnError += OnError;
            _actionDataBase.InitializeConnection();
        }

        ///<summary>
        ///Called by the ASP.NET page framework to notify server controls that use composition-based 
        ///implementation to create any child controls they contain in preparation for posting back or rendering.
        ///</summary>
        protected override void CreateChildControls()
        {
            Debug.WriteLine("ActionEditorControl CreateChildControls ");
            Debug.WriteLine("FormMode:" + FormMode);
            Debug.WriteLine("DataBaseOperation:" + DataBaseOperation);

            try
            {              
                // Create the toolbar control
                _actionToolBar = new ActionEditorToolBarControl(Page, this, _config.ActionToolBars)
                                 {
                                         ID = "tb"
                                 };
                _actionToolBar.OnError += OnError;
                Controls.Add(_actionToolBar);

                // Create the form control
                _actionEditorForm = new ActionEditorFormControl(this, _actionDataBase, _config.Fields)
                                    {
                                            ID = "editor"
                                    };
                _actionEditorForm.OnError += OnError;
                Controls.Add(_actionEditorForm);
            }
            catch (Exception ex)
            {
                SendError(ex);
            }
            Debug.WriteLine("End ActionEditorControl CreateChildControls ");
        }

        protected override void OnPreRender(EventArgs e)
        {
            Debug.WriteLine("ActionEditorControl: OnPreRender - Create childs controls ");
            Debug.WriteLine("FormMode:" + FormMode);

            try
            {
                // Force creation of child controls at this 
                // moment we can consume a value (IMPORTANT)
                //if (FormMode == ActionEditorFormMode.Edit ||
                //    FormMode == ActionEditorFormMode.New)
                //if (InEditMode)
                //{
                //    ChildControlsCreated = false; // Discard state
                //    EnsureChildControls(); // Force                              
                //}

                // Read the database with the filter values 
                ReadDataBase(FilterValues);

                // pass values to editor
                _actionEditorForm.SetFormValues();
                
                // Send current form values to toolbar
                _actionToolBar.GetFormValues(_actionEditorForm.GetFormValues());

                // Is only necesary for Edit and New but send it always
                //if (Page != null)
                //{
                //    Page.RegisterRequiresPostBack(_actionToolBar);
                //}
            }
            catch (Exception ex)
            {
                SendError(ex);
            }
        }

        ///<summary>
        ///Restores view-state information from a previous request that was saved with the <see cref="M:System.Web.UI.WebControls.WebControl.SaveViewState" /> method. 
        ///</summary>
        ///
        ///<param name="savedState">An object that represents the control state to restore. </param>
        protected override void LoadViewState(object savedState)
        {
            Debug.WriteLine("LOAD VIEW STATE");
            base.LoadViewState(savedState);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Debug.WriteLine("ActionEdiorControl Render");

            try
            {
                // Force child controls again
                EnsureChildControls();

                // Here we add the buttons to the toolbar
                // it's necesary because action commands 
                // require GetPostBackEventReference and
                // we can call it, only here  
                if (NoRecordFound())
                {
                    writer.WriteLine("No record found.");
                    FormMode = ActionEditorFormMode.Blank;
                    _actionToolBar.AddButtons(ActionEditorFormMode.View);
                }
                else
                {
                    _actionToolBar.AddButtons(FormMode);
                }


                // Render the toolbar control
                _actionToolBar.RenderControl(writer);

                // finally render the form view
                if (_actionEditorForm != null)
                {
                    _actionEditorForm.RenderControl(writer);
                }
            }
            catch (Exception ex)
            {
                SendError(ex);
            }
        }

      
        private bool NoRecordFound()
        {
            return _actionDataBase.DataRow == null && DataBaseOperation == ActionDataBaseOperation.Read;
        }

        ///<summary>
        ///Saves any state that was modified after the <see cref="M:System.Web.UI.WebControls.Style.TrackViewState" /> method was invoked.
        ///</summary>
        ///
        ///<returns>
        ///An object that contains the current view state of the control; otherwise, if there is no view state associated with the control, null.
        ///</returns>
        ///
        protected override object SaveViewState()
        {
            Debug.WriteLine("SAVE VIEW STATE");
            return base.SaveViewState();
        }

        #endregion

        //#region IPostBackDataHandler

        //public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        //{
        //    Debug.WriteLine("ActionEditorControl LoadPostData");
        //    DumpPostCollection(postCollection);

        //    if (_config != null)
        //    {
        //        HandlePostData(postCollection);
        //    }

        //    return true;
        //}

        //public void RaisePostDataChangedEvent()
        //{        
        //}

        //#endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            Debug.WriteLine("ActionEdiorControl: RaisePostBackEvent - Handle Events ");
            Debug.WriteLine("Mode: " + FormMode);
            Debug.WriteLine("Argument: " + eventArgument);

            // Send action to controller
            if (eventArgument.StartsWith("Mode$"))
            {
                HandleEditorMode(eventArgument.Substring(5));

                if (CommandEvent != null)
                {
                    CommandEvent(this, new EventArgs());
                }
            }
        }

        #endregion

        #region ISPSErrorControl Members

        public event SPSControlOnError OnError;

        #endregion

        #region Engine

        /// <summary>
        /// Change the current state
        /// </summary>
        /// <param name="userOperation">Selected user operation</param>
        private void HandleEditorMode(string userOperation)
        {
            switch (userOperation)
            {
                case "Edit":
                    FormMode = ActionEditorFormMode.Edit;
                    DataBaseOperation = ActionDataBaseOperation.Read;
                    break;

                case "Back":
                case "View":
                    FormMode = ActionEditorFormMode.View;
                    DataBaseOperation = ActionDataBaseOperation.Read;
                    break;

                case "New":
                    FormMode = ActionEditorFormMode.New;
                    DataBaseOperation = ActionDataBaseOperation.None;
                    break;

                case "Update":
                    DataBaseUpdate();
                    break;

                case "Create":
                    DataBaseCreate();
                    break;

                case "Delete":
                    DataBaseDelete();
                    break;
            }
        }

        private IDictionary<string,string> GetFieldValuesFromControls()
        {
            Debug.WriteLine("GetFieldValuesFromControls");
            //EnsureChildControls();

            return _actionEditorForm.GetFormValues();

            // Dictionary to store field values
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();

            foreach (KeyValuePair<string,string> pair in _actionEditorForm.GetFormValues())
            {
                Debug.WriteLine("GetValue:" + pair.Key);
                fieldValues.Add(pair.Key,pair.Value);
            }

            return fieldValues;                             
        }

        ///// <summary>
        ///// Handle the postback from form, get values from form fields
        ///// </summary>
        ///// <param name="postCollection">The post collection.</param>
        //private void HandlePostData(NameValueCollection postCollection)
        //{
        //    return;
        //    // Dictionary to store field values
        //    _fieldValues = new SPSKeyValueList();

        //    // Get field values from postCollection
        //    foreach (Field field in _config.Fields)
        //    {
        //        try
        //        {
        //            string value = GetPostDataValues(field, postCollection);

        //            Debug.WriteLine(string.Format("Load Field [{0,-20}] -> [{1}]", field.Name, value));

        //            _fieldValues.Add(field.Name, value);
        //        }
        //        catch (Exception ex) // ArgumentOutOfRangeException
        //        {
        //            SendError(ex);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets the form value for an espcified field.
        ///// </summary>
        ///// <param name="field">The field.</param>
        ///// <param name="postCollection">The post collection.</param>
        ///// <returns>The post data value of field</returns>
        //private string GetPostDataValues(Field field, NameValueCollection postCollection)
        //{
        //    string value;
        //    string baseId = UniqueID + IdSeparator + "ctl01$c" + field.Name;

        //    //Debug.WriteLine(baseId);

        //    switch (field.Control)
        //    {
        //        case ActionEditorControlsType.Lookup:
        //            if (field.Lookup.ControlEditor == ActionEditorLookupControl.PickerDataBase)
        //            {
        //                value = postCollection[baseId + IdSeparator + "hiddenSpanData"];
        //            }
        //            else
        //            {
        //                value = postCollection[baseId];
        //            }
        //            break;
        //        case ActionEditorControlsType.Date:
        //            value = postCollection[baseId + IdSeparator + "c" + field.Name + "Date"];
        //            break;
        //        case ActionEditorControlsType.DateTime:
        //            value = postCollection[baseId + IdSeparator + "c" + field.Name + "Date"] + " " +
        //                    postCollection[baseId + IdSeparator + "c" + field.Name + "DateHours"] +
        //                    postCollection[baseId + IdSeparator + "c" + field.Name + "DateMinutes"];
        //            break;
        //        case ActionEditorControlsType.Memo:
        //            if (field.Memo.RichText == "No" || field.Memo.RichText == "false")
        //            {
        //                value = postCollection[baseId];
        //            }
        //            else
        //            {
        //                value = postCollection[baseId + "_spSave"];
        //            }
        //            break;
        //        default:
        //            value = postCollection[baseId];
        //            break;
        //    }

        //    return value;
        //}

        public void SetFilterValues(SPSKeyValueList filterValues)
        {
            if (filterValues != null)
            {
                _filterValues = filterValues;
                //FilterValues = filterValues;
            }
        }

        /// <summary>
        /// Reads the data base.
        /// </summary>
        /// <param name="filterValues">The filter values.</param>
        public void ReadDataBase(SPSKeyValueList filterValues)
        {
            // Read
            if (DataBaseOperation == ActionDataBaseOperation.Read)
            {
                Debug.WriteLine("** READ_DATABASE **");

                // Try read
                _actionDataBase.InitializeDataSet();

                if (_actionDataBase.SelectRecord(filterValues, false) == 0)
                {
                    // Record not found
                    FormMode = ActionEditorFormMode.Blank;
                }
                else
                {
                    // Record found 
                    if (FormMode == ActionEditorFormMode.Blank)
                    {
                        FormMode = ActionEditorFormMode.View;
                    }
                }
            }

            // Retain filter values 
            FilterValues = filterValues;

            // If skip-read put read
            if (DataBaseOperation == ActionDataBaseOperation.NextTimeRead)
            {
                DataBaseOperation = ActionDataBaseOperation.Read;
            }
        }

        /// <summary>
        /// Do the insert in the DataBase
        /// </summary>
        private void DataBaseCreate()
        {
            // Insert error
            FormMode = ActionEditorFormMode.New;
            DataBaseOperation = ActionDataBaseOperation.None;

            Page.Validate();
            if (Page.IsValid)
            {
                if (_actionDataBase.CreateRecord(GetFieldValuesFromControls(), FilterValues) > 0)
                {
                    // Ok
                    FormMode = ActionEditorFormMode.View;
                    DataBaseOperation = ActionDataBaseOperation.NextTimeRead;
                    ViewState.SetItemDirty("FilterValues", true);
                }
            }
        }

        /// <summary>
        /// Do the Update in the DataBase
        /// </summary>
        private void DataBaseUpdate()
        {
            // Assume Error 
            FormMode = ActionEditorFormMode.Edit;
            DataBaseOperation = ActionDataBaseOperation.None;

            Page.Validate();
            if (Page.IsValid)
            {
                if (_actionDataBase.UpdateRecord(GetFieldValuesFromControls(), FilterValues) > 0)
                {
                    // Ok
                    FormMode = ActionEditorFormMode.View;
                    DataBaseOperation = ActionDataBaseOperation.Read;                    
                }
            }
        }

        /// <summary>
        /// Do Delete in DataBase 
        /// </summary>
        private void DataBaseDelete()
        {
            _actionDataBase.DeleteRecord(FilterValues);
            FormMode = ActionEditorFormMode.View;
            DataBaseOperation = ActionDataBaseOperation.Read;
        }

        #endregion

        private void SendError(Exception ex)
        {
            if (OnError != null)
            {
                OnError(this, new SPSErrorArgs(GetType().Name, ex.TargetSite.Name, ex));
            }

            DumpException(ex.TargetSite.Name,ex);
        }

        #region DEBUG

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        [Conditional("DEBUG")]
        private void DumpPostCollection(NameValueCollection postCollection)
        {
            Debug.WriteLine("** Dump postCollection Values **");
            foreach (string key in postCollection.AllKeys)
            {
                Debug.WriteLine(string.Format("{0} -> {1}", key, postCollection.Get(key)));
            }
            Debug.WriteLine("** End Dump postCollection Values **");
        }

        #endregion
    }
}