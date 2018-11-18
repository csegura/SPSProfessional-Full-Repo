using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Xml.XPath;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Common;

namespace SPSProfessional.SharePoint.WebParts.RollUp.XsltProcessor
{
    public class SPSXsltExtension
    {
        private readonly Control _control;
        private readonly Page _page;

        private string _scriptjs;
        private string _script;

        private string[] _fcColors;
        private int _fcColorCounter;

        private string[] _calColors;
        private int _calColorCounter;

        private int _counter;
        private static Hashtable _cultureInfoTable;
        private readonly SPSKeyValueList _keyValues;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSXsltExtension"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="page">The page.</param>
        /// <param name="keyValues">The key values.</param>
        public SPSXsltExtension(Control control, Page page, SPSKeyValueList keyValues)
        {
            _control = control;
            _page = page;
            _keyValues = keyValues;
            Initialize();
        }

        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _scriptjs = "__doPostBack('" + _control.UniqueID + "','{0}:'+{1});";
            _script = "javascript:" + _page.ClientScript.GetPostBackEventReference(_control, "{0}:{1}");

            #region InitChartsColors

            _fcColorCounter = 0;
            _fcColors = new string[20];
            _fcColors[0] = "1941A5"; //Dark Blue
            _fcColors[1] = "AFD8F8";
            _fcColors[2] = "F6BD0F";
            _fcColors[3] = "8BBA00";
            _fcColors[4] = "A66EDD";
            _fcColors[5] = "F984A1";
            _fcColors[6] = "CCCC00"; //Chrome Yellow+Green
            _fcColors[7] = "999999"; //Grey
            _fcColors[8] = "0099CC"; //Blue Shade
            _fcColors[9] = "FF0000"; //Bright Red 
            _fcColors[10] = "006F00"; //Dark Green
            _fcColors[11] = "0099FF"; //Blue (Light)
            _fcColors[12] = "FF66CC"; //Dark Pink
            _fcColors[13] = "669966"; //Dirty green
            _fcColors[14] = "7C7CB4"; //Violet shade of blue
            _fcColors[15] = "FF9933"; //Orange
            _fcColors[16] = "9900FF"; //Violet
            _fcColors[17] = "99FFCC"; //Blue+Green Light
            _fcColors[18] = "CCCCFF"; //Light violet
            _fcColors[19] = "669900"; //Shade of green            

            _calColorCounter = 0;
            _calColors = new string[32];
            //_calColors[0] = "SPSCal_Azure";
            //_calColors[1] = "SPSCal_Beige";
            //_calColors[2] = "SPSCal_Bisque";
            //_calColors[3] = "SPSCal_Chartreuse";
            //_calColors[4] = "SPSCal_Brown";
            //_calColors[5] = "SPSCal_BurlyWood";
            //_calColors[6] = "SPSCal_CadetBlue";
            //_calColors[7] = "SPSCal_Chocolate";
            //_calColors[8] = "SPSCal_CornflowerBlue";
            //_calColors[9] = "SPSCal_Crimson";
            //_calColors[10] = "SPSCal_DarkBlue";
            //_calColors[11] = "SPSCal_DarkCyan";
            //_calColors[12] = "SPSCal_DarkGreen";
            //_calColors[13] = "SPSCal_DarkRed";
            //_calColors[14] = "SPSCal_DarkSalmon";
            //_calColors[15] = "SPSCal_DarkSlateBlue";
            //_calColors[16] = "SPSCal_DarkSlateGray";
            //_calColors[17] = "SPSCal_FireBrick";

            _calColors[0] = "SPSCal_Ivory";
            _calColors[1] = "SPSCal_Beige";
            _calColors[2] = "SPSCal_Wheat";
            _calColors[3] = "SPSCal_Tan";
            _calColors[4] = "SPSCal_Khaki";
            _calColors[5] = "SPSCal_Silver";
            _calColors[6] = "SPSCal_Gray";
            _calColors[7] = "SPSCal_Charcoal";
            _calColors[8] = "SPSCal_NavyBlue";
            _calColors[9] = "SPSCal_RoyalBlue";
            _calColors[10] = "SPSCal_MediumBlue";
            _calColors[11] = "SPSCal_Azure";
            _calColors[12] = "SPSCal_Cyan";
            _calColors[13] = "SPSCal_Aquamarine";
            _calColors[14] = "SPSCal_Teal";
            _calColors[15] = "SPSCal_ForestGreen";
            _calColors[16] = "SPSCal_Olive";
            _calColors[17] = "SPSCal_Chartreuse";
            _calColors[18] = "SPSCal_Lime";
            _calColors[19] = "SPSCal_Golden";
            _calColors[20] = "SPSCal_Goldenrod";
            _calColors[21] = "SPSCal_Coral";
            _calColors[22] = "SPSCal_Salmon";
            _calColors[23] = "SPSCal_HotPink";
            _calColors[24] = "SPSCal_Fuchsia";
            _calColors[25] = "SPSCal_Puce";
            _calColors[26] = "SPSCal_Mauve";
            _calColors[27] = "SPSCal_Lavender";
            _calColors[28] = "SPSCal_Plum";
            _calColors[29] = "SPSCal_Indigo";
            _calColors[30] = "SPSCal_Maroon";
            _calColors[31] = "SPSCal_Crimson";


            #endregion

            _cultureInfoTable = new Hashtable();
        }

        #region Events

        /// <summary>
        /// Fire the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The script to fire event</returns>
        public string Event(string eventName, string parameters)
        {
            return string.Format(_script, eventName, parameters);
        }

        /// <summary>
        /// Fire the specified event name, for use inside scripts
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The script to fire event</returns>
        public string EventJS(string eventName, string parameters)
        {
            return string.Format(_scriptjs, eventName, parameters);
        }

        /// <summary>
        /// ECMAs the script encode.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <returns>The encoded string</returns>
        public string EcmaScriptEncode(string stringToEncode)
        {
            return SPHttpUtility.EcmaScriptStringLiteralEncode(stringToEncode);
        }

        #endregion

        #region SPS

        /// <summary>
        /// Lasts the row.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value for the last row key (Row[Column])</returns>
        public string LastRow(string key)
        {
            return _keyValues[key];
        }

        /// <summary>
        /// Iifs the specified expr.
        /// </summary>
        /// <param name="expr">if set to <c>true</c> [expr].</param>
        /// <param name="isTrue">The is true value</param>
        /// <param name="isFalse">The is false value</param>
        /// <returns>The selected expression</returns>
        public string iif(bool expr, string isTrue, string isFalse)
        {
            return expr ? isTrue : isFalse;
        }

        /// <summary>
        /// Maps to icon.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="programId">The program id.</param>
        /// <returns>The icon file</returns>
        public string MapToIcon(string fileName, string programId)
        {
            try
            {
                return SPUtility.MapToIcon(SPContext.Current.Web, fileName, programId);
            }
            catch (Exception ex)
            {
                SPSDebug.DumpException("MapToIcon", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// Dates diff.
        /// </summary>
        /// <param name="Interval">The interval.</param>
        /// <param name="Date1">The date1.</param>
        /// <param name="Date2">The date2.</param>
        /// <returns>The date diff</returns>
        public static long DateDiff(string Interval, DateTime Date1, DateTime Date2)
        {
            try
            {
                TimeSpan span = Date2.Subtract(Date1);
                switch (Interval.ToLowerInvariant())
                {
                    case "y":
                        return Date2.Year - Date1.Year;
                    case "mm":
                        return ((Date2.Year * 12) + Date2.Month) - ((Date1.Year * 12) + Date1.Month);
                    case "d":
                        return (long)span.TotalDays;
                    case "w":
                        return (long)span.TotalDays / 7;
                    case "h":
                        return (long)span.TotalHours;
                    case "m":
                        return (long)span.TotalMinutes;
                    case "s":
                        return (long)span.TotalSeconds;
                }
            }
            catch (Exception ex)
            {
                SPSDebug.DumpException("DateDiff", ex);
            }
            return 0;
        }

        #endregion

        #region Charts

        /// <summary>
        /// Gets the color of the fc.
        /// </summary>
        /// <returns>Cycle color for Fusion Charts</returns>
        public string GetFcColor()
        {
            //Update index
            _fcColorCounter++;
            //Return color
            return _fcColors[_fcColorCounter % _fcColors.Length];
        }


        /// <summary>
        /// Gets the color of the fc.
        /// </summary>
        /// <returns>Cycle color for Fusion Charts</returns>
        public string GetCalColor()
        {
            //Update index
            _calColorCounter++;
            //int rnd = new Random(DateTime.Now.Millisecond + _calColorCounter).Next(_calColors.Length);
            //Return color
            return _calColors[_calColorCounter % _calColors.Length];
            //return _calColors[rnd];
        }

        #endregion

        #region Cloned from MS

        /// <summary>
        /// Counters this instance.
        /// </summary>
        /// <returns>The counter value</returns>
        public string Counter()
        {
            _counter++;
            return _counter.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="theDate">The date.</param>
        /// <returns>The DateTime</returns>
        protected static DateTime GetDateTime(string theDate)
        {
            return DateTime.Parse(theDate, CultureInfo.InvariantCulture).ToUniversalTime().ToLocalTime();
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="stringDate">The string date.</param>
        /// <param name="lcid">The lcid.</param>
        /// <param name="formatString">The format string.</param>
        /// <returns>The formated date</returns>
        public string FormatDateTime(string stringDate, long lcid, string formatString)
        {
            string str = string.Empty;
            try
            {
                DateTime dateTime = GetDateTime(stringDate);

                if (formatString.Length != 0)
                {
                    CultureInfo info = new CultureInfo((int)lcid);
                    _cultureInfoTable[lcid] = info;
                    return dateTime.ToString(formatString, info);
                }
            }
            catch (Exception)
            {
                str = string.Empty;
            }
            return str;
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="stringDate">The string date.</param>
        /// <param name="formatString">The format string.</param>
        /// <returns>The formated date</returns>
        public string FormatDateTime(string stringDate, string formatString)
        {
            string str = string.Empty;
            int lcid = SPContext.Current.Web.CurrencyLocaleID;
            
            Debug.WriteLine("LCID:"+lcid);
            
            try
            {
                DateTime dateTime = GetDateTime(stringDate);

                if (formatString.Length != 0)
                {
                    CultureInfo info = new CultureInfo(lcid);
                    _cultureInfoTable[lcid] = info;
                    return dateTime.ToString(formatString, info);
                }
            }
            catch (Exception)
            {
                str = string.Empty;
            }
            return str;
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <param name="targetUrl">The target URL.</param>
        /// <returns>The file extension</returns>
        public string GetFileExtension(string targetUrl)
        {
            int num = targetUrl.LastIndexOf('.');
            if (num < 0)
            {
                return string.Empty;
            }
            return targetUrl.Substring(num + 1, targetUrl.Length - (num + 1));
        }

        /// <summary>
        /// Gets the string before separator.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>The string before</returns>
        public static string GetStringBeforeSeparator(string inputString)
        {
            int index = inputString.IndexOf(";#");
            if (index != -1)
            {
                return inputString.Substring(0, index);
            }
            return string.Empty;
        }

        /// <summary>
        /// Ifs the new.
        /// </summary>
        /// <param name="createdTime">The created time.</param>
        /// <returns>If is a new item</returns>
        public bool IfNew(string createdTime)
        {
            try
            {
                return (GetDateTime(createdTime).AddDays(2.0) >= DateTime.Now);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Limits the specified input text.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="additionalText">The additional text.</param>
        /// <returns>Trimed text</returns>
        public string Limit(string inputText, int maxLength, string additionalText)
        {
            if (inputText.Length > maxLength)
            {
                return (inputText.Substring(0, maxLength) + additionalText);
            }
            return inputText;
        }

        /// <summary>
        /// Maxes the specified node iter.
        /// </summary>
        /// <param name="nodeIter">The node iter.</param>
        /// <returns>The max value</returns>
        public int Max(XPathNodeIterator nodeIter)
        {
            int num = -2147483648;
            int num2 = 0;
            while (true)
            {
                while (!nodeIter.MoveNext())
                {
                    if (num2 <= 0)
                    {
                        return 0x7fffffff;
                    }
                    return num;
                }
                try
                {
                    int num3 = int.Parse(nodeIter.Current.Value);
                    if (num3 > num)
                    {
                        num = num3;
                    }
                    num2++;
                }
                catch (FormatException)
                {
                }
            }
        }

        /// <summary>
        /// Mins the specified node iter.
        /// </summary>
        /// <param name="nodeIter">The node iter.</param>
        /// <returns>The min value</returns>
        public int Min(XPathNodeIterator nodeIter)
        {
            int num = 0x7fffffff;
            int num2 = 0;
            while (true)
            {
                while (!nodeIter.MoveNext())
                {
                    if (num2 <= 0)
                    {
                        return -2147483648;
                    }
                    return num;
                }
                try
                {
                    int num3 = int.Parse(nodeIter.Current.Value);
                    if (num3 < num)
                    {
                        num = num3;
                    }
                    num2++;
                }
                catch (FormatException)
                {
                }
            }
        }

        /// <summary>
        /// Todays this instance.
        /// </summary>
        /// <returns>Today date</returns>
        public string Today()
        {
            return DateTime.Now.ToString("G", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Todays the iso.
        /// </summary>
        /// <returns>Today date in iso format</returns>
        public string TodayIso()
        {
            return DateTime.Now.ToString("s", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// URLs the name of the base.
        /// </summary>
        /// <param name="targetUrl">The target URL.</param>
        /// <returns>The Url</returns>
        public string UrlBaseName(string targetUrl)
        {
            int num = targetUrl.LastIndexOf('/');
            int num2 = targetUrl.LastIndexOf('.');
            if (num >= (num2 - 1))
            {
                return string.Empty;
            }
            return targetUrl.Substring(num + 1, (num2 - 1) - num);
        }

        /// <summary>
        /// URLs the name of the dir.
        /// </summary>
        /// <param name="targetUrl">The target URL.</param>
        /// <returns>The Url</returns>
        public string UrlDirName(string targetUrl)
        {
            try
            {
                int index = targetUrl.IndexOf('/');
                int num2 = targetUrl.LastIndexOf('/');
                if (index < num2)
                {
                    return targetUrl.Substring(index, num2 - index);
                }
            }
            catch (Exception ex)
            {
                SPSDebug.DumpException("UrlDirName", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// URLs the encode.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <returns>The encoded Url</returns>
        public string UrlEncode(string stringToEncode)
        {
            return HttpUtility.UrlEncode(stringToEncode);
        }

        #endregion
    }

    internal static class SPSDebug
    {
        [Conditional("DEBUG")]
        internal static void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        [Conditional("DEBUG")]
        internal static void DumpException(Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}