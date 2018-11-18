using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal abstract class ActionEditorFormControlBase
    {
        protected Field _field;
        private string _value;
        protected Control _fieldControl;
        private bool _required;
        private List<ActionEditorFormValidatorBase> _validators;
        private bool _internalControlCreated;

        #region Public Properties

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>The field control.</value>
        public Control FieldControl
        {
            get { return _fieldControl ?? InitializeFieldControl(); }
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>The field.</value>
        public Field Field
        {
            get { return _field; }
        }

        /// <summary>
        /// Gets or sets the control value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                if (_internalControlCreated)
                    GetControlValue();
                return _value;
            }
            set
            {
                _value = value;
                if (_internalControlCreated)
                    SetControlValue(_value);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEditorFormControlBase"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        protected ActionEditorFormControlBase(Field field)
        {
            _field = field;
        }

        /// <summary>
        /// Creates the control.
        /// </summary>
        /// <returns></returns>
        protected abstract Control CreateControl();

        /// <summary>
        /// Gets the value for render formatted.
        /// </summary>
        /// <returns></returns>
        public string GetControlValueForRender()
        {
            string value = GetProcessedValue();

            if (!string.IsNullOrEmpty(_field.DisplayFormat))
            {
                value = string.Format(value, _field.DisplayFormat);
            }

            return string.IsNullOrEmpty(value) ? "&nbsp;" : value;
        }

        /// <summary>
        /// Instantiates the in.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        public void InstantiateIn(Control parentControl)
        {
            parentControl.Controls.Add(FieldControl);
            _validators.ForEach(validator => parentControl.Controls.Add(validator.ValidationControl));
            Debug.WriteLine("InstantiateIn:" + FieldControl.ID);
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void RenderControl(HtmlTextWriter writer)
        {
            FieldControl.RenderControl(writer);
            _validators.ForEach(validator => validator.ValidationControl.RenderControl(writer));
        }

        protected virtual string GetProcessedValue()
        {
            return _value;
        }

        protected abstract void SetControlValue(string value);
        protected abstract void GetControlValue();

        #region Private Methods

        private Control InitializeFieldControl()
        {
            _fieldControl = CreateControl();
            DecorateControl();
            CreateValidators();
            Debug.WriteLine("InitializeFieldControl:" + _fieldControl.ID);
            SetControlValue(_value);
            _internalControlCreated = true;
            return _fieldControl;
        }

        private void DecorateControl()
        {
            const string READONLY = "readonly";
            _required = _field.Required;

            if (_field.Edit == ActionEditorFieldVisibility.Disabled)
            {
                ((WebControl) _fieldControl).Attributes.Add(READONLY, READONLY);

                if (_field.Control == ActionEditorControlsType.Lookup)
                {
                    ((WebControl) _fieldControl).Enabled = false;
                }

                _required = false;
            }
        }

        private void CreateValidators()
        {
            FormValidatorFactory validatorFactory = new FormValidatorFactory();
            _validators = new List<ActionEditorFormValidatorBase>();

            if (_required
                && (_field.Control != ActionEditorControlsType.Lookup)
                && (_field.Control != ActionEditorControlsType.CheckBox))
            {
                ActionEditorFormValidatorBase requiredValidator = validatorFactory.CreateRequiredValidator(_field,
                                                                                                           _fieldControl);
                _validators.Add(requiredValidator);
            }

            foreach (Validator validator in _field.Validators)
            {
                ActionEditorFormValidatorBase formValidator = validatorFactory.CreateValidator(_field, validator);
                _validators.Add(formValidator);
            }
        }

        #endregion
    }
}