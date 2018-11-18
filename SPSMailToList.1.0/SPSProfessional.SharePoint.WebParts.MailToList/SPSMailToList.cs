using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.WebParts.MailToList.Resources;


namespace SPSProfessional.SharePoint.WebParts.MailToList
{
    [DefaultProperty("CategoryFilter")]
    [ToolboxData("<{0}:SPSMailToList runat=server></{0}:SPSMailToList>")]
    [XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.MailToList")]
    public class SPSMailToList : SPSWebPart
    {
        private string _login;
        private string _password;
        private string _mailServer;
        private string _mailPort;
        private string _listGuid;
        private bool _ssl;
        private bool _manualConnection;
        private Thread _thread;

        private Button _btnGetMail;

        private string _error;

        //private SPSErrorBoxControl _errorBox;

        #region WebPart Properties

        [Personalizable(PersonalizationScope.Shared)]
        public string ListGuid
        {
            get { return _listGuid; }
            set { _listGuid = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string MailServer
        {
            get { return _mailServer; }
            set { _mailServer = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string MailPort
        {
            get { return _mailPort; }
            set { _mailPort = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ManualConnection
        {
            get { return _manualConnection; }
            set { _manualConnection = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool UseSsl
        {
            get { return _ssl; }
            set { _ssl = value; }
        }

        #endregion

        public SPSMailToList()
        {
            SPSInit("e2d3eda6-c5fa-4e63-a412-68485bfabcd0",
                    "SPSMailToList.1.0",
                    "SPSMailToList WebPart",
                    "http://www.spsprofessional.com/");

            EditorParts.Add(new SPSMailToListEditorPart());
        }

        #region Control

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ExportMode = WebPartExportMode.All;
        }

        protected override void CreateChildControls()
        {            
            base.CreateChildControls();

            if (_manualConnection)
            {
                _btnGetMail = new Button
                                  {
                                      Text = SPSResources.GetResourceString("SPS_GetMail")
                                  };
                _btnGetMail.Click += BtnGetMailClick;
                Controls.Add(_btnGetMail);
            }
        }

        void BtnGetMailClick(object sender, EventArgs e)
        {
            using (var longOperation = new SPLongOperation(Page))
            {
                longOperation.LeadingHTML = SPSResources.GetResourceString("SPS_FetchMail_Message");
                longOperation.TrailingHTML = SPSResources.GetResourceString("SPS_FetchMail_Message2"); 
                longOperation.Begin();

                int port;
                Int32.TryParse(_mailPort, out port);

                var fetchMail = new FetchMail(_login, 
                    _password, 
                    _mailServer, 
                    port, 
                    _ssl, 
                    _listGuid);

                fetchMail.GetMessages();

                _error = fetchMail.GetErrorMessage();

                if (!string.IsNullOrEmpty(_error))
                {
                    SPUtility.TransferToErrorPage(_error);
                }
                else
                {
                    longOperation.End(Page.Request.Url.ToString());
                }
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            EnsureChildControls();
        }
        
        protected override void SPSRender(HtmlTextWriter writer)
        {
            try
            {
                if (CheckParameters())
                {
                    if (!_manualConnection)
                        GetMailUsingThread();
                    else
                        _btnGetMail.RenderControl(writer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _error = ex.ToString();
            }

            if (!string.IsNullOrEmpty(_error))
                writer.Write(_error);
        }

        private void GetMailUsingThread()
        {
            int port;
            Int32.TryParse(_mailPort, out port);

            var fetchMail = new FetchMail(_login, _password, _mailServer, port, _ssl, _listGuid);
            _thread = new Thread(fetchMail.ThreadGetMessages);
            _thread.Start();
        }

        #endregion

        /// <summary>
        /// Checks the parameters.
        /// </summary>
        /// <returns></returns>
        internal bool CheckParameters()
        {
            return !string.IsNullOrEmpty(_listGuid)
                   && !string.IsNullOrEmpty(_login)
                   && !string.IsNullOrEmpty(_password)
                   && !string.IsNullOrEmpty(_mailServer);
        }       
    }
}