using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    /// <summary>
    /// Evaluator 
    /// </summary>
    public class SPSEvaluator
    {
        /// <summary>
        /// Evaluates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string Evaluate(string value)
        {
            return InternalEvaluator(value);
        }

        /// <summary>
        /// Internals the evaluator.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string InternalEvaluator(string value)
        {
            string defaultValue = value;

            //if (value.Contains("()"))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                string loginName = string.Empty;

                if (identity != null)
                {
                    loginName = identity.Name;
                }

                SPUser user = null;
                SPWeb web = null;

                if (value.Contains("User"))
                {
                    user = SPContext.Current.Web.CurrentUser;
                }

                if (value.Contains("Web"))
                {
                    web = SPContext.Current.Web;
                }

                switch (value)
                {
                    case "DateNow()":
                        defaultValue = DateTime.Now.ToString("d");
                        break;
                    case "DateTimeNow()":
                        defaultValue = DateTime.Now.ToString("G");
                        break;
                    case "MonthNumber()":
                        defaultValue = DateTime.Now.Month.ToString();
                        break;
                    case "DayNumber()":
                        defaultValue = DateTime.Now.Day.ToString();
                        break;
                    case "YearNumber()":
                        defaultValue = DateTime.Now.Year.ToString();
                        break;
                    case "Guid()":
                        defaultValue = Guid.NewGuid().ToString("B");
                        break;
                    case "UserLogin()":
                        defaultValue = loginName;
                        break;
                    case "UserName()":
                        if (user != null)
                        {
                            defaultValue = user.Name;
                        }
                        break;
                    case "UserId()":
                        if (user != null)
                        {
                            defaultValue = user.ID.ToString();
                        }
                        break;
                    case "UserEmail()":
                        if (user != null)
                        {
                            defaultValue = user.Email;
                        }
                        break;
                    case "WebName()":
                        if (web != null)
                        {
                            defaultValue = web.Name;
                        }
                        break;
                    case "WebTitle()":
                        if (web != null)
                        {
                            defaultValue = web.Title;
                        }
                        break;
                    case "WebUrl()":
                        if (web != null)
                        {
                            defaultValue = web.Url;
                        }
                        break;
                    case "Empty()":
                        defaultValue = string.Empty;
                        break;

                    default:
                        if (value.StartsWith("$Now"))
                        {
                            // Parse culture & format
                            // http://msdn2.microsoft.com/es-es/library/az4se3k1(VS.80).aspx
                        }
                        if (value.StartsWith("QueryStringNull"))
                        {
                            defaultValue = GetQueryStringValiableNull(value);
                        }
                        else if (value.StartsWith("QueryString"))
                        {
                            defaultValue = GetQueryStringValiable(value);
                        }
                        else if (value.StartsWith("DateFormat"))
                        {
                            defaultValue = GetDateFormat(value);
                        }
                        else if (value.StartsWith("DateCalcFormat"))
                        {
                            defaultValue = GetDateCalcFormat(value);
                        }
                        else if (value.StartsWith("DateCalc"))
                        {
                            defaultValue = GetDateCalc(value);
                        }
                        break;
                }
            }

            //Debug.WriteLineIf(defaultValue != value, "Default [" + value + "]");

            return defaultValue;
        }

        /// <summary>
        /// Gets the query string valiable.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        private string GetQueryStringValiable(string formula)
        {
            return Regex.Replace(formula,
                                 @"^QueryString\('([\s\w]+?)'\)",
                                 new MatchEvaluator(GetQueryStringVariableEvaluator));
        }

        /// <summary>
        /// Gets the query string variable evaluator.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private string GetQueryStringVariableEvaluator(Match m)
        {
            string variable = m.Groups[1].ToString();
            return HttpContext.Current.Request.QueryString.Get(variable);
        }

        /// <summary>
        /// Gets the query string valiable.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        private string GetQueryStringValiableNull(string formula)
        {
            return Regex.Replace(formula,
                                 @"^QueryStringNull\('([\s\w]+?)','(.*?)'\)",
                                 new MatchEvaluator(GetQueryStringVariableNullEvaluator));
        }

        /// <summary>
        /// Gets the query string variable evaluator.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private string GetQueryStringVariableNullEvaluator(Match m)
        {
            string variable = m.Groups[1].ToString();
            string variable2 = m.Groups[2].ToString();
            string value = HttpContext.Current.Request.QueryString.Get(variable);
            if (string.IsNullOrEmpty(value))
            {
                value = variable2;
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Gets the query string valiable.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        private string GetDateFormat(string formula)
        {
            return Regex.Replace(formula,
                                 @"^DateFormat\('([\s\w]+?)'\)",
                                 new MatchEvaluator(GetDateFormatEvaluator));
        }

        /// <summary>
        /// Gets the query string variable evaluator.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        /// <exception cref="SPSEvaluatorException"><c>SPSEvaluatorException</c>.</exception>
        private string GetDateFormatEvaluator(Match m)
        {
            try
            {
                string variable = m.Groups[1].ToString();
                return DateTime.Now.ToString(variable);
            }
            catch (FormatException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
        }


        /// <summary>
        /// Gets the date diff format.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        private string GetDateCalcFormat(string formula)
        {
            return Regex.Replace(formula,
                                 @"^DateCalcFormat\('([\s\w]+?)',([MDY][+-]\d+)\)",
                                 new MatchEvaluator(GetDateCalcFormatEvaluator));
        }


        /// <summary>
        /// Gets the date diff format evaluator.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        /// <exception cref="SPSEvaluatorException"><c>SPSEvaluatorException</c>.</exception>
        private string GetDateCalcFormatEvaluator(Match m)
        {
            try
            {
                string format = m.Groups[1].ToString();
                string days = m.Groups[2].ToString();

                string value = string.Empty;
                int diff = Int32.Parse(days.Substring(1));

                switch (days.Substring(0, 1))
                {
                    case "M":
                        value = DateTime.Now.AddMonths(diff).ToString(format);
                        break;
                    case "D":
                        value = DateTime.Now.AddDays(diff).ToString(format);
                        break;
                    case "Y":
                        value = DateTime.Now.AddYears(diff).ToString(format);
                        break;
                }

                return value;
            }
            catch (OverflowException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
            catch (FormatException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
        }


        /// <summary>
        /// Gets the date diff format.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        private string GetDateCalc(string formula)
        {
            return Regex.Replace(formula,
                                 @"^DateCalc\(([MDY][+-]\d+)\)",
                                 new MatchEvaluator(GetDateCalcEvaluator));
        }


        /// <summary>
        /// Gets the date diff format evaluator.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        /// <exception cref="SPSEvaluatorException"><c>SPSEvaluatorException</c>.</exception>
        private string GetDateCalcEvaluator(Match m)
        {
            try
            {
                string match = m.Groups[1].ToString();

                DateTime value = DateTime.Now;

                int diff = Int32.Parse(match.Substring(1));

                switch (match.Substring(0, 1))
                {
                    case "M":
                        value = value.AddMonths(diff);
                        break;
                    case "D":
                        value = value.AddDays(diff);
                        break;
                    case "Y":
                        value = value.AddYears(diff);
                        break;
                }

                return SPUtility.CreateISO8601DateTimeFromSystemDateTime(value);
            }
            catch (OverflowException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
            catch (FormatException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new SPSEvaluatorException(ex);
            }
        }

        ///// <summary>
        ///// Test the evaluator
        ///// </summary>
        ///// <returns>An string with samples</returns>
        //public static string Sample()
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    string[] functions = new string[]
        //        {
        //            "DateNow()",
        //            "DateTimeNow()",
        //            "MonthNumber()",
        //            "DayNumber()",
        //            "YearNumber()",
        //            "Guid()",
        //            "UserLogin()",
        //            "UserName()",
        //            "UserId()",
        //            "UserEmail()",
        //            "WebName()",
        //            "WebTitle()",
        //            "WebUrl()",
        //            "Empty()",
        //            "DateFormat('d')",
        //            "DateFormat('s')",
        //            "DateFormat('T')",
        //            "DateCalcFormat('s',M+1)",
        //            "DateCalc('T',D+1)",
        //            "DateCalc('d',Y-1)",
        //            "QueryString('Test')"
        //        };

        //    SPSEvaluator evaluator = new SPSEvaluator();
        //    foreach (string function in functions)
        //    {
        //        stringBuilder.AppendFormat("<font color=blue>{0}</font> - {1}</br>",
        //                                   function,
        //                                   evaluator.Evaluate(function));
        //    }

        //    return stringBuilder.ToString();
        //}
       
    }
}