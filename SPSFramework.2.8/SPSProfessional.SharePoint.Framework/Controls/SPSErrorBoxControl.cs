// File : SPSErrorBoxControl.cs
// Date : 30/01/2008
// User : csegura
// Logs

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// SPSErrorBoxContros display errors
    /// Internally maintains a Stack with the error
    /// </summary>
    /// <see cref="SPSErrorArgs"/>
    public class SPSErrorBoxControl : Control, ISPSErrorPool
    {
        #region CONSTS

        // {0} = Title style
        // {1} = Title
        // {2} = Content style
        // {3} = Content
        private const string SPSErrorExpandBox =
            @"
              <TABLE style=""width:100%;"" cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR ID=""group0"">
	              <TD style=""{0}"">
                   <span class=""ms-announcementtitle"">
                   <a href=""javascript:"" onclick=""javascript:ExpGroupBy(this);return false;"">
                   <img src=""/_layouts/images/plus.gif"" border=""0"" />
                   </a>
                   {1}
                   </span>
                  </TD>
                </TR>
                <TR style=""display:none"">	   
	              <TD style=""{2}"">		
		             {3}				      
	              </TD>
	            </TR>
	          </TABLE>";

        // {0} = Error Subsystem
        // {1} = User message
        // {2} = Exception 
        private const string SPSErrorBoxEntry =
            @"
              <TABLE style=""width:100%;"" cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR ID=""group0"">
	              <TD style=""background-color: red; border-color:white; color:white;"">
                   <span class=""ms-announcementtitle"">
                   <a href=""javascript:"" onclick=""javascript:ExpGroupBy(this);return false;"">
                   <img src=""/_layouts/images/plus.gif"" border=""0"" />
                   </a>
                   {0}
                   </span>
                  </TD>
                </TR>
                <TR style=""display:none"">	   
	              <TD style=""border-color:red; color:red;"">		
		             {1}<BR/>{2}				      
	              </TD>
	            </TR>
	          </TABLE>";

        private const string SPSErrorBeginTableTag =
            @"
              <TABLE style=""width:100%;"" cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR>
	              <TD style=""background-color: red; border-color:white; color:white;"">
		            Errors
	              </TD>
                </TR><TR><TD>";

        private const string SPSErrorEndTableTag = "</TD></TR></TABLE>";



        #endregion

        // Internal Stack
        private readonly Stack<SPSErrorArgs> _errors;
        private bool _showExtendedErrors;
        private SPSConfigErrors _configErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSErrorBoxControl"/> class.
        /// </summary>
        public SPSErrorBoxControl()
        {
            _errors = new Stack<SPSErrorArgs>();
            _configErrors = new SPSConfigErrors();
        }
       
        /// <summary>
        /// Logs display user mode or developper messages
        /// </summary>
        /// <value><c>true</c> if [user mode]; otherwise, <c>false</c>.</value>
        public bool ShowExtendedErrors
        {
            set { _showExtendedErrors = value; }
            get { return _showExtendedErrors; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        /// <summary>
        /// Gets or sets the config errors.
        /// </summary>
        /// <value>The config errors.</value>
        public SPSConfigErrors ConfigErrors
        {
            get { return _configErrors; }
            set { _configErrors = value; }
        }

        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <param name="reportedError">The reported error.</param>
        public void AddError(SPSErrorArgs reportedError)
        {
            _errors.Push(reportedError);
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>        
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (_errors.Count > 0)
            {
                writer.WriteLine(SPSErrorBeginTableTag);

                foreach (SPSErrorArgs error in _errors)
                {
                    if (_showExtendedErrors || error.OnlyDevelopper)
                    {
                        // developer messages
                        writer.WriteLine(SPSErrorBoxEntry,
                                         error.Subsystem,
                                         error.UserMessage,
                                         ExceptionToHtml(error.InternalException));
                    }
                    else
                    {
                        // User messages
                        string userMessage = string.Empty;

                        if (error.InternalException is FormatException)
                            userMessage += error.InternalException.Message;

                        if (error.InternalException is SqlException)
                            userMessage += error.InternalException.Message;

                        if (!CustomError(userMessage, writer))
                        {
                            writer.WriteLine(SPSErrorBoxEntry,
                                             error.Subsystem,
                                             error.UserMessage,
                                             userMessage);
                        }
                    }
                }
                writer.WriteLine(SPSErrorEndTableTag);
            }
            base.RenderControl(writer);
        }

        /// <summary>
        /// Indent the exception message and the stack trace
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The html formated text</returns>
        private string ExceptionToHtml(Exception ex)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (ex != null)
            {
                stringBuilder.AppendFormat("M:{0}<br/>", ex.Message);
                stringBuilder.AppendFormat("S:{0}<br/>", ex.Source);
                stringBuilder.Append(ex.StackTrace.Replace("\r\n", "<br/>"));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Check if there is an user specified message for the exception
        /// </summary>
        /// <param name="userMessage">The user message.</param>
        /// <param name="writer">The writer.</param>
        /// <returns>If there is a custom message for the exception</returns>
        private bool CustomError(string userMessage, HtmlTextWriter writer)
        {
            foreach(SPSConfigError error in ConfigErrors)
            {
                Regex exp = new Regex(error.Match,RegexOptions.Multiline);
                if (exp.IsMatch(userMessage))
                {
                    writer.Write(string.Format("<font color='{0}'>{1}</font></br>",
                                               error.Color, 
                                               error.Message));
                    return true;
                }

            }
            return false;
        }
    }
}