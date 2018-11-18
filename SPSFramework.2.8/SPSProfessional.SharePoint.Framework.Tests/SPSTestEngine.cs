using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace UnitTests
{
    /// <summary>
    /// 
    /// </summary>
    public class SPSTestEngine : IDisposable
    {
        private const string DefaultWebName = "TestSite";
        private readonly Dictionary<string, SPWeb> _webs;

        private readonly Queue<SPWeb> _websQueue;

        private SPSite _site;
        private string _urlUnitTests;

        public SPSTestEngine()
        {
            _webs = new Dictionary<string, SPWeb>();
            _websQueue = new Queue<SPWeb>();

            UrlTestSite = "http://SetupUrlTestSite";
        }

        public string UrlTestSite
        {
            get { return _urlUnitTests; }
            set { _urlUnitTests = value; }
        }

        public SPSite Site
        {
            get { return _site; }
        }

        public SPWeb GetWeb(string name)
        {
            return _webs[name];
        }

        public void WebProvisioning(string parentUrl)
        {
            _websQueue.Enqueue(CreateWeb(DefaultWebName, parentUrl, string.Empty));
        }

        public void WebProvisioning(string name, string parentUrl)
        {
            _websQueue.Enqueue(CreateWeb(name, parentUrl, string.Empty));
        }

        public void WebProvisioning(string name, string parentUrl, string template)
        {
            _websQueue.Enqueue(CreateWeb(name, parentUrl, template));
        }

        public void Restoring()
        {
            DeleteWebs();
        }

        private SPWeb CreateWeb(string title, string parentUrl, string template)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Web title can't be null or empty.");
            }

            if (string.IsNullOrEmpty(parentUrl))
            {
                throw new ArgumentException("Web parentUrl can't be null or empty.");
            }

            Trace.WriteLine(string.Format("Creating test web {0} at {1}", title, parentUrl));

            _site = new SPSite(UrlTestSite + parentUrl);
            using (SPWeb web = _site.OpenWeb())
            {
                SPWeb newWeb;

                if (web == null)
                {
                    throw new Exception(string.Format("Can´t open SharePoint Site at: {0}", _site.Url));
                }

                // If exist delete
                if (web.Webs[title].Exists)
                {
                    throw new Exception(string.Format("There is already a web in this url.{0}", web.Webs[title].Url));
                    //RecursiveDeleteWebs(web.Webs[title]);
                    //web.Webs[title].Delete();
                }

                // Simple STS site
                if (template.Equals(string.Empty))
                {
                    newWeb = web.Webs.Add(title);
                }
                else
                {
                    // Create a web with custom based template
                    try
                    {
                        SPWebTemplateCollection webTemplateCollection;
                        webTemplateCollection = _site.GetCustomWebTemplates((uint)web.Locale.LCID);

                        SPWebTemplate webTemplate = webTemplateCollection[template];

                        newWeb = web.Webs.Add(title,
                                              title,
                                              string.Empty,
                                              (uint)web.Locale.LCID,
                                              webTemplate,
                                              true,
                                              false);
                    }
                    catch (ArgumentException) // Not a custom template 
                    {
                        newWeb = web.Webs.Add(title,
                                              title,
                                              string.Empty,
                                              (uint)web.Locale.LCID,
                                              template,
                                              true,
                                              false);
                    }
                }

                Trace.WriteLine(string.Format("Created web {0} at {1}", newWeb.Title, newWeb.Url));
                return newWeb;
            }            
        }


        private void DeleteWebs()
        {
            foreach (SPWeb web in _webs.Values)
            {
                RecursiveDeleteWebs(web);
                try
                {
                    Trace.WriteLine(string.Format("Deleting web {0} at {1}", web.Title, web.Url));
                    web.Delete();
                }
                catch (SPException ex)
                {
                    Trace.WriteLine(string.Format("In collection Web {0} at {1} don´t exist.", web.Title, web.Url));
                    Trace.WriteLine(string.Format("Error {0} - {1}", ex.GetType(), ex.Message));
                }
            }
        }

        private void RecursiveDeleteWebs(SPWeb parentWeb)
        {
            if (parentWeb != null)
            {
                Trace.WriteLine(string.Format("Deleteting Webs in {0} at {1} ", parentWeb.Title, parentWeb.Url));
                try
                {
                    foreach (SPWeb web in parentWeb.Webs)
                    {
                        RecursiveDeleteWebs(web);
                        Trace.WriteLine(string.Format("Deleting web {0} at {1}", web.Title, web.Url));
                        web.Delete();
                    }
                }
                catch (ArgumentException)
                {
                    Trace.WriteLine(string.Format("Web {0} at {1} don´t exist.", parentWeb.Title, parentWeb.Url));
                }
            }
        }

        // TODO -  class can´t use IDisposable interface
        public void Ending()
        {
            if (_site != null)
            {
                _site.Dispose();
            }
        }

        ~SPSTestEngine()
        {
            Dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_websQueue != null)
                {
                    while (_websQueue.Count > 0)
                    {
                        SPWeb web = _websQueue.Dequeue();
                        try
                        {
                            Trace.WriteLine(string.Format("Deleting web {0} at {1}", web.Title, web.Url));
                            web.Delete();
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                        }
                    }
                }
            }
        }

        #endregion
    }
}