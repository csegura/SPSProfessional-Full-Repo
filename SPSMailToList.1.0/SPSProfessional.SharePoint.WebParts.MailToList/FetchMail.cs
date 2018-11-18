using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MailBee;
using MailBee.Mime;
using MailBee.Pop3Mail;
using MailBee.Security;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.MailToList
{
    internal class FetchMail
    {
        private readonly string _login;
        private readonly string _password;
        private readonly string _server;
        private readonly int _port;
        private readonly bool _ssl;
        private readonly SPList _list;

        private string _lastError;

        private object _lock;
        private bool _threadDone;


        public FetchMail(string login,
                         string password,
                         string server,
                         int port,
                         bool ssl,
                         string listGuid)
        {
            Debug.WriteLine("FetchMail");

            _login = login;
            _password = password;
            _server = server;
            _port = port;
            _ssl = ssl;

            if (_port == 0)
            {
                _port = 110;
            }

            _list = SPContext.Current.Web.Lists[new Guid(listGuid)];
        }

        //public void GetAll()
        //{
        //    Debug.WriteLine("GetAll");

        //    Pop3 pop = new Pop3();
        //    Pop3.LicenseKey = "MN200-0BC3C3CDC3C2C322C3470FBBCF38-43DA";

        //    if (_ssl)
        //    {
        //        pop.SslProtocol = SecurityProtocol.Auto;
        //        pop.SslMode = SslStartupMode.OnConnect;
        //    }

        //    try
        //    {
        //        if (pop.Connect(_server, _port, true))
        //        {
        //            Debug.WriteLine("Connected");

        //            if (pop.Login(_login,
        //                          _password,
        //                          AuthenticationMethods.Auto,
        //                          AuthenticationOptions.PreferSimpleMethods,
        //                          null))
        //            {

        //                Debug.WriteLine("Logged in " + pop.InboxMessageCount);
        //                GetAllMessages(pop);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("GetAll");
        //        Debug.WriteLine(ex);
        //        _lastError = ex.Message;
        //    }
        //    finally
        //    {
        //        if (pop.IsConnected)
        //        {
        //            pop.Disconnect();
        //        }
        //        pop.Dispose();
        //    }
        //}

        public void GetMessages()
        {
            Connect(GetAllMessages);  
        }

        public void ThreadGetMessages()
        {
            _lock = new object();
            lock (_lock)
            {
                if (!_threadDone)
                {
                    Connect(GetAllMessages);
                    _threadDone = true;
                }
            }
        }

        public string CheckConnection()
        {
            Connect(Dummy);
            return _lastError;
        }

        private bool Dummy(Pop3 pop)
        {
            return true;
        }

        private void Connect(Func<Pop3,bool> onConnectFunction)
        {
            Debug.WriteLine("GetAll");

            Pop3.LicenseKey = "MN200-539BA4259BD99BD69B32E3AB96B2-6863";
            var pop = new Pop3();
            

            if (_ssl)
            {
                pop.SslProtocol = SecurityProtocol.Auto;
                pop.SslMode = SslStartupMode.OnConnect;
            }

            try
            {
                if (pop.Connect(_server, _port, true))
                {
                    Debug.WriteLine("Connected");

                    if (pop.Login(_login,
                                  _password,
                                  AuthenticationMethods.Auto,
                                  AuthenticationOptions.PreferSimpleMethods,
                                  null))
                    {

                        Debug.WriteLine("Logged in " + pop.InboxMessageCount);
                        onConnectFunction(pop);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetAll");
                Debug.WriteLine(ex);
                _lastError = ex.Message;
            }
            finally
            {
                if (pop.IsConnected)
                {
                    pop.Disconnect();
                }
                pop.Dispose();
            }
        }

        private bool GetAllMessages(Pop3 pop)
        {
            try
            {
                MailMessageCollection messages = pop.DownloadEntireMessages();

                foreach (MailMessage message in messages)
                {
                    if (AddToList(message))
                    {
                        pop.DeleteMessage(message.IndexOnServer);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                _lastError = ex.Message;
            }
            return false;
        }

        private bool AddToList(MailMessage message)
        {
            Debug.WriteLine("AddToList");
            Debug.WriteLine(message.IndexOnServer);
            try
            {
                SPListItem item = _list.Items.Add();

                item["MailFrom"] = message.From.ToString();
                item["MailTo"] = message.To.ToString();
                item["MailCC"] = message.Cc.ToString();
                item["MailBCC"] = message.Bcc.ToString();
                item["Title"] = message.Subject;
                item["MailServerReceived"] = message.DateReceived;
                item["MailSent"] = message.Date;
                item["MailBody"] = message.BodyHtmlText;
                item["MailImportance"] = message.Importance.ToString();
                item["MailSize"] = message.Size;
                
                foreach (Attachment attachment in message.Attachments)
                {
                    Debug.WriteLine(attachment.ContentID);
                    item.Attachments.Add(NormalizeFileName(attachment.Filename), attachment.GetData());
                }

                item.Update();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AddToList");
                Debug.WriteLine(ex);
                _lastError = ex.Message;
            }

            return false;
        }

        public string GetErrorMessage()
        {
            return _lastError;
        }

        // ==========================================================================

        private readonly Regex _invalidCharsRegex =
                new Regex(@"[\*\?\|\\\t/:""'<>#{}%~&]", RegexOptions.Compiled);

        /// <summary>
        /// Returns a folder or file name that
        /// conforms to SharePoint's naming restrictions
        /// </summary>
        /// <param name="original">The original file or folder name.
        /// For files, this should be the file name without the extension.</param>
        /// <returns></returns>
        private string NormalizeFileName(string original)
        {
            // remove invalid characters and some initial replacements
            string friendlyName = _invalidCharsRegex.Replace(original, String.Empty).Trim();

            return friendlyName;
        }
    }
}