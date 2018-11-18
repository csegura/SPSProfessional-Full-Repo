using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionEditorToolBarControl : Control, ISPSErrorControl
    {
        private readonly ActionToolBars _actionToolBars;
        private readonly Page _page;
        private readonly Control _parent;
        private SPSToolBarControl _toolBarControl;
        private IDictionary<string, string> _fieldValues;

        public ActionEditorToolBarControl(Page page,
                                          Control parent,
                                          ActionToolBars actionToolBars)
        {
            _page = page;
            _parent = parent;
            _actionToolBars = actionToolBars;
        }

        protected override void CreateChildControls()
        {
            _toolBarControl = new SPSToolBarControl
                              {
                                      ID = ("toolBar" + _parent.ID)
                              };
            //_parent.Controls.Add(_toolBarControl);
        }

        public void GetFormValues(IDictionary<string, string> fieldValues)
        {
            _fieldValues = fieldValues;
        }

        /// <summary>
        /// Adds the buttons.
        /// Must be called from a Render method we are using GetPostBackEventReference
        /// </summary>
        /// <param name="editorMode">The editor mode.</param>
        public void AddButtons(ActionEditorFormMode editorMode)
        {
            EnsureChildControls();
            try
            {
                AddButtonsInternal(editorMode);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this,
                            new SPSErrorArgs(
                                    "ActionEditorToolBarControl",
                                    "Error adding buttons.",
                                    ex));
                }
            }
        }

        #region Control Overrides

        public override void RenderControl(HtmlTextWriter writer)
        {
            EnsureChildControls();
            _toolBarControl.RenderControl(writer);
        }

        #endregion

        #region Private Methods

        private void AddButtonsInternal(ActionEditorFormMode editorMode)
        {
            foreach (ActionToolBar actionToolBar in _actionToolBars)
            {
                // Is our bar
                if (actionToolBar.Name.ToString() == editorMode.ToString())
                {
                    foreach (Option option in actionToolBar)
                    {
                        if (ActionNotDefined(option))
                        {
                            AddUserButton(option);
                        }
                        else
                        {
                            // Action commands need postback
                            string argument = "Mode$" + option.Action;
                            string onClick;

                            // Generate postback options
                            onClick = ActionRequireValidation(option)
                                              ? GetOnClickWithValidation(argument)
                                              : GetOnClickWithoutValidation(argument);

                            Debug.WriteLine(argument + " " + onClick);

                            if (ActionDeleteRecord(option))
                            {
                                AddDeleteRecordButton(option, onClick);
                            }
                            else
                            {
                                AddOtherActionButton(option, onClick);
                            }
                        }
                    }
                }
            }
        }

        private bool ActionDeleteRecord(Option option)
        {
            return option.Action == ActionEditorToolBarsActions.Delete;
        }

        private bool ActionNotDefined(Option option)
        {
            return option.Action == ActionEditorToolBarsActions.None;
        }

        private string GetOnClickWithoutValidation(string argument)
        {
            return _page.ClientScript.GetPostBackEventReference(_parent, argument);
        }

        private bool ActionRequireValidation(Option option)
        {
            // Update & Create commands require server validation
            return (option.Action == ActionEditorToolBarsActions.Update)
                   || (option.Action == ActionEditorToolBarsActions.Create);
        }

        private string GetOnClickWithValidation(string argument)
        {
            PostBackOptions postBackOptions = new PostBackOptions(_parent)
                                              {
                                                      Argument = argument,
                                                      AutoPostBack = true,
                                                      PerformValidation = true
                                              };

            return _page.ClientScript.GetPostBackEventReference(postBackOptions);
        }

        private void AddOtherActionButton(Option option, string onClick)
        {
            // Add Action Button
            _toolBarControl.AddToolBarActionButton(
                    _parent.ID + option.Name,
                    option.Name,
                    onClick,
                    option.Name,
                    option.ImageUrl);
        }

        private void AddDeleteRecordButton(Option option, string onClick)
        {
            // Add Special Action Button that require confirmation
            _toolBarControl.AddToolBarActionButton(
                    _parent.ID + option.Name,
                    option.Name,
                    "return DeleteItemConfirmation();",
                    "javascript:" + onClick + ";",
                    option.Name,
                    option.ImageUrl);
        }

        private void AddUserButton(Option option)
        {
            string processedNavigateUrl = ProcessUserButton(option.NavigateUrl);

            //TODO: Conditional display

            // User configurable Button
            _toolBarControl.AddToolbarButton(
                    _parent.ID + option.Name,
                    option.Name,
                    processedNavigateUrl,
                    option.Name,
                    option.ImageUrl);
        }

        private string ProcessUserButton(string navigateUrl)
        {
            if (_fieldValues != null)
            {
                foreach (KeyValuePair<string, string> pair in _fieldValues)
                {
                    string look = "%" + pair.Key.Trim() + "%";
                    navigateUrl = navigateUrl.Replace(look, SPHttpUtility.UrlKeyValueEncode(pair.Value));
                    return navigateUrl;
                }
            }
            return "#";
        }

        #endregion

        #region ISPSErrorControl Members

        public event SPSControlOnError OnError;

        #endregion
    }
}