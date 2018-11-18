using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// XML Calendar definition
    /// </summary>
    [Serializable]
    [XmlRoot("SPSCalendar")]
    public class SPSCalendarXML
    {
        private SPSCalendarItem[] _items;
        private string _viewType;

        #region Properties

        [XmlElement("SPSCalendarItem")]
        public SPSCalendarItem[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        [XmlAttribute]
        public string ViewType
        {
            get { return _viewType; }
            set { _viewType = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSCalendarXML"/> class.
        /// </summary>
        public SPSCalendarXML()
        {
            _items = new SPSCalendarItem[]
                     {
                     };
            ViewType = "month";
        }

        /// <summary>
        /// Decorates the specified calendar view.
        /// </summary>
        /// <param name="calendarView">The calendar view.</param>
        public void Decorate(SPCalendarView calendarView)
        {
            //calendarView.SelectedDate = DateTime.Now.ToString();
            calendarView.ViewType = ViewType;
        }

        /// <summary>
        /// Fills the specified calendar view.
        /// </summary>
        /// <param name="calendarView">The calendar view.</param>
        public void Fill(SPCalendarView calendarView)
        {
            calendarView.DataSource = DataSource();
        }

        /// <summary>
        /// Generate Data
        /// </summary>
        /// <returns></returns>
        private SPCalendarItemCollection DataSource()
        {
            SPCalendarItemCollection items = new SPCalendarItemCollection();

            Array.Sort(Items);

            foreach (SPSCalendarItem item in Items)
            {
                SPCalendarItem calItem = new SPCalendarItem
                                         {
                                                 CalendarType = item.CalendarType,
                                                 StartDate = item.StartDate,
                                                 EndDate = item.EndDate,
                                                 IsAllDayEvent = item.IsAllDayEvent,
                                                 Title = item.Title,
                                                 Description = item.Description,
                                                 Location = item.Location,
                                                 IsRecurrence = item.IsRecurrence,
                                                 DisplayFormUrl = item.DisplayFormUrl,
                                                 ItemID = item.ItemID,
                                         };

                if (item.BackgroundColorClassName != null)
                {
                    calItem.BackgroundColorClassName = item.BackgroundColorClassName;
                }

#if (DEBUG)
                XmlSerializer s = new XmlSerializer(typeof(SPSCalendarItem));
                StringWriter sw = new StringWriter();
                s.Serialize(sw, item);
                Debug.WriteLine("*" + sw);
                sw.Close();
#endif
                items.Add(calItem);
            }

#if (DEBUG)
            foreach (SPCalendarItem item in items)
            {
                SPSDebug.DumpObject(item);
            }

            //SPCalendarItem calItem1 = new SPCalendarItem
            //                          {
            //                                  StartDate = new DateTime(2009, 1, 14, 11, 00, 00),
            //                                  EndDate = new DateTime(2009, 1, 21, 11, 00, 00),
            //                                  Title = "TEST"
            //                          };

            //items.Add(calItem1);
#endif

            return items;
        }
    }
}