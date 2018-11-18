using System.Web.UI;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal abstract class ActionEditorFormValidatorBase
    {
        protected Field _field;
        protected Validator _validator;
        protected Control _validationControl;

        public Control ValidationControl
        {
            get { return _validationControl ?? InitializeValidatorControl(); }
        }

        public Field Field
        {
            get { return _field; }
        }

        protected ActionEditorFormValidatorBase(Field field, Validator validator)
        {
            _field = field;
            _validator = validator;
        }

        protected abstract Control CreateValidatorControl();

        private Control InitializeValidatorControl()
        {
            _validationControl = CreateValidatorControl();
            return _validationControl;
        }

    }
}