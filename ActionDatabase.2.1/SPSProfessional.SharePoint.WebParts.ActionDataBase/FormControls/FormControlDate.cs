using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.FormControls
{
    internal class FormControlDate : ActionEditorFormControlBase
    {
        private DateTimeControl _control;

        public FormControlDate(Field field) : base(field)
        {
        }

        #region Overrides of ActionEditorFormControlBase

        protected override Control CreateControl()
        {
            _control = new DateTimeControl
                       {
                               ID = ("c" + _field.Name),
                               ToolTip = _field.Description,
                               DateOnly = true
                       };

            InitializeDatePicker(_control);

            return _control;
        }

        protected override void SetControlValue(string value)
        {
            if (_control!= null && !string.IsNullOrEmpty(value))
            {
                _control.SelectedDate = DateTime.Parse(value);
            }
        }

        protected override void GetControlValue()
        {
            if (_control != null)
                Value = _control.SelectedDate.ToString();
        }

        #endregion

        internal static void InitializeDatePicker(DateTimeControl datePicker)
        {
            SPWeb web = SPContext.Current.Web;
            SPRegionalSettings regionalSettings = web.CurrentUser.RegionalSettings;
            if (regionalSettings == null)
            {
                regionalSettings = web.RegionalSettings;
            }
            datePicker.LocaleId = (int)regionalSettings.LocaleId;
            datePicker.TimeZoneID = regionalSettings.TimeZone.ID;
            datePicker.UseTimeZoneAdjustment = false;
            datePicker.HijriAdjustment = regionalSettings.AdjustHijriDays;
            datePicker.HoursMode24 = true; //regionalSettings.Time24;
            datePicker.Calendar = (SPCalendarType)regionalSettings.CalendarType;
            datePicker.DatePickerFrameUrl = SPContext.Current.Web.ServerRelativeUrl + "/_layouts/iframe.aspx";
        }
    }
}