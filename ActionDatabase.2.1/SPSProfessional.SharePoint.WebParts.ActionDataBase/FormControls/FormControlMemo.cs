using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlMemo : ActionEditorFormControlBase
    {
        private InputFormTextBox _control;

        public FormControlMemo(Field field) : base(field)
        {
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
            _control = new InputFormTextBox
                      {
                              ID = ("c" + _field.Name),
                              ToolTip = _field.Description,
                              TextMode = TextBoxMode.MultiLine,
                              Rows = 10
                      };

            if (_field.Memo.RichText == "No" || _field.Memo.RichText == "false")
            {
                _control.RichText = false;
            }
            else
            {
                _control.RichText = true;

                if (_field.Memo.RichText == "Full")
                {
                    _control.RichTextMode = SPRichTextMode.FullHtml;
                }
                else
                {
                    _control.RichTextMode = SPRichTextMode.Compatible;
                }
            }

            if (_field.Memo != null)
            {
                _control.Columns = _field.Memo.Columns;
                _control.Rows = _field.Memo.Rows;
                _control.MaxLength = _field.Memo.MaxLength;
            }

            return _control;
        }

        protected override string GetProcessedValue()
        {
            return SPEncode.HtmlDecode(Value);
        }

        protected override void SetControlValue(string value)
        {
            if (_control != null)
            _control.Text = value;
        }

        protected override void GetControlValue()
        {
            if (_control != null)
            {
                Value = _control.Text;
            }
        }

        #endregion
    }
}