// File : SPSErrorArgs.cs
// Date : 30/01/2008
// User : csegura
// Logs

using System;

namespace SPSProfessional.SharePoint.Framework.Error
{
    /// <summary>
    /// Delegate to pass errors
    /// </summary>
    public delegate void SPSControlOnError(object sender, SPSErrorArgs args);

    ///<summary>
    /// SPSErrorArgs store error information well
    ///</summary>
    public class SPSErrorArgs
    {
        private readonly Exception _exception;
        private readonly string _subsystem;
        private readonly string _userMessage;
        private readonly bool _onlyDevelopper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSErrorArgs"/> class.
        /// </summary>
        /// <param name="subsystem">The subsystem.</param>
        /// <param name="userMessage">The user message.</param>
        /// <param name="exception">The exception.</param>
        public SPSErrorArgs(string subsystem, string userMessage, Exception exception)
        {
            _subsystem = subsystem;
            _userMessage = userMessage;
            _exception = exception;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSErrorArgs"/> class.
        /// </summary>
        /// <param name="subsystem">The subsystem.</param>
        /// <param name="userMessage">The user message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="developper">if set to <c>true</c> [developper].</param>
        public SPSErrorArgs(string subsystem, string userMessage, Exception exception,bool developper)
        {
            _subsystem = subsystem;
            _userMessage = userMessage;
            _exception = exception;
            _onlyDevelopper = developper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSErrorArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public SPSErrorArgs(Exception exception)
        {
            _subsystem = exception.TargetSite.Name;
            _userMessage = exception.Message;
            _exception = exception;
        }

        /// <summary>
        /// Gets the internal exception.
        /// </summary>
        /// <value>The internal exception.</value>
        public Exception InternalException
        {
            get { return _exception; }        
        }

        /// <summary>
        /// Gets the subsystem.
        /// </summary>
        /// <value>The subsystem.</value>
        public string Subsystem
        {
            get { return _subsystem; }
        }

        /// <summary>
        /// Gets the user message.
        /// </summary>
        /// <value>The user message.</value>
        public string UserMessage
        {
            get { return _userMessage; }
        }

        /// <summary>
        /// Gets a value indicating whether [only developper].
        /// </summary>
        /// <value><c>true</c> if [only developper]; otherwise, <c>false</c>.</value>
        public bool OnlyDevelopper
        {
            get { return _onlyDevelopper; }
        }
    }
}