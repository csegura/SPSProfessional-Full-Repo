using System;
using System.Diagnostics;
using System.Web.UI;


namespace SPSProfessional.SharePoint.Framework.Controls
{
    public class SPSXsltCommandControl : Control, INamingContainer, ISPSViewCommandParameters, IPostBackEventHandler
    {
        protected const string CMD_Order = "Order:";
        protected const string CMD_Page = "Page:";
        protected const string CMD_Select = "Select:";

        private const string VS_XslOrder = "XslOrder";
        private const string VS_XslPage = "XslPage";
        private const string VS_XslSelectedRow = "XslSelectedRow";

        public event EventHandler<EventArgs> OnCommand;

        public SPSXsltCommandControl()
        {
            XslPage = 1;
        }

        public int? XslPage
        {
            get { return (int?)ViewState[VS_XslPage]; }
            set { ViewState[VS_XslPage] = value; }
        }

        public int? XslSelectedRow
        {
            get {
                int? xslSelectedRow = (int?) ViewState[VS_XslSelectedRow];
                return xslSelectedRow ?? 0;
            }
            set { ViewState[VS_XslSelectedRow] = value; }
        }

        public string XslOrder
        {
            get { return (string)ViewState[VS_XslOrder]; }
            set { ViewState[VS_XslOrder] = value; }
        }


        #region Implementation of IPostBackEventHandler

        /// <summary>
        /// When implemented by a class, enables a server control to process an event raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A <see cref="T:System.String" /> that represents an optional event argument to be passed to the event handler. </param>
        public void RaisePostBackEvent(string eventArgument)
        {
            // Process our postbacks
            Debug.WriteLine("RaisePostBackEvent " + ClientID);
            Debug.WriteLine("RaisePostBackEvent " + eventArgument);

            bool command = false;

            if (eventArgument.StartsWith(CMD_Select))
            {
                // Get the selected row
                XslSelectedRow = Int32.Parse(eventArgument.Substring(7));
                command = true;
            }

            if (eventArgument.StartsWith(CMD_Page))
            {
                // Get the page
                XslPage = Int32.Parse(eventArgument.Substring(5));
                command = true;
            }

            if (eventArgument.StartsWith(CMD_Order))
            {
                // Get the sort order
                XslOrder = eventArgument.Substring(6);
                command = true;
            }
            
            if (command)
            {
                FireOnCommand();
            }
        }

        private void FireOnCommand()
        {
            if (OnCommand != null)
            {
                OnCommand(this, new EventArgs());
            }
        }

        #endregion

    }
}
