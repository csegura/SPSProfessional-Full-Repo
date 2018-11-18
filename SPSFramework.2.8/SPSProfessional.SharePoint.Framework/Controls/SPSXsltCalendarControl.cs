using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    ///<summary>
    ///</summary>
    public class SPSXsltCalendarControl : SPSXsltControl
    {
        private SPCalendarView _calendarView;
        private string _xmlTransformed;
        private SPSCalendarXML _xmlCalendar;

        #region Control
       

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls 
        /// that use composition-based implementation to create any child 
        /// controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            _calendarView = new SPCalendarView
                                {
                                        Width = new Unit(100, UnitType.Percentage)
                                };
            
            _xmlCalendar = GetCalendarXML();

            if (_xmlCalendar != null)
            {
                _xmlCalendar.Decorate(_calendarView);
            }

            if (Page.Request.QueryString["CalendarPeriod"] != null)
            {
                const string month = "month";
                const string day = "day";
                const string week = "week";
                const string timeline = "timeline";

                switch (Page.Request.QueryString["CalendarPeriod"].ToLower())
                {
                    case month:
                        _calendarView.ViewType = month;
                        break;
                    case day:
                        _calendarView.ViewType = day;
                        break;
                    case week:
                        _calendarView.ViewType = week;
                        break;
                    case timeline:
                        _calendarView.ViewType = timeline;
                        break;
                }
            }

            _calendarView.EnableViewState = true;
            Controls.Add(_calendarView);
            base.CreateChildControls();
        }


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_xmlCalendar != null)
            {
                try
                {
                    _xmlCalendar.Fill(_calendarView);
                    _calendarView.DataBind();
                    DataBind();
                }
                catch(Exception ex)
                {
                    TrapError(GetType(), SPSLocalization.GetResourceString("SPSFW_Err_XmlCalendar"), ex);
                }
            }
        }

        /// <summary>
        /// Internals the render.
        /// Make the Xslt transformation
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void RenderControlInternal(HtmlTextWriter writer)
        {
            if (DebugSource || DebugTransformation)
            {
                DebugRender(writer, _xmlTransformed);
            }
            _calendarView.RenderControl(writer);
        }


        /// <summary>
        /// Gets the XML calendar.
        /// </summary>
        /// <returns>A SPSCalendarXML definition</returns>
        private SPSCalendarXML GetCalendarXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof (SPSCalendarXML));
            
            _xmlTransformed = Transform().ToString();
            
            if (!string.IsNullOrEmpty(_xmlTransformed))
            {
                try
                {
                    TextReader reader = new StringReader(_xmlTransformed);
                    SPSCalendarXML calendar = (SPSCalendarXML) serializer.Deserialize(reader);
                    return calendar;
                }
                catch (Exception ex)
                {
                    TrapError(GetType(), SPSLocalization.GetResourceString("SPSFW_Err_XmlCalendar"), ex);                    
                }
            }
            return null;
        }

        #endregion
       
    }
}