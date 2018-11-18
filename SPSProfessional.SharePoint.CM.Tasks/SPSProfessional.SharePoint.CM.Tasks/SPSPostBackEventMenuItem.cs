using System;
using System.Web.UI;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.CM.Tasks
{
    public class SPSPostBackEventMenuItem : MenuItemTemplate, IPostBackEventHandler
    {
        public SPSPostBackEventMenuItem()
            : base()
        {
        }

        public SPSPostBackEventMenuItem(string text)
            : base(text)
        {
        }

        public SPSPostBackEventMenuItem(string text, string imageUrl)
            : base(text, imageUrl)
        {
        }

        public SPSPostBackEventMenuItem(string text, string imageUrl, string clientOnClickScript)
            : base(text, imageUrl, clientOnClickScript)
        {
        }

        protected override void EnsureChildControls()
        {
            if (!ChildControlsCreated)
            {
                base.EnsureChildControls();
                if (string.IsNullOrEmpty(ClientOnClickUsingPostBackEvent))
                {
                    ClientOnClickUsingPostBackEventFromControl(this);
                }
            }
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            EventHandler<EventArgs> handler = OnPostBackEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        #endregion

        public event EventHandler<EventArgs> OnPostBackEvent;
    }
}