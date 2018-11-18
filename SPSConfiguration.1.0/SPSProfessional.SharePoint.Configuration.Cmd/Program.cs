using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPSProfessional.SharePoint.Configuration.Cmd
{
    internal class Program
    {
        private static int _argCount;

        private static string _url;
        private static string _key;
        private static SPSite _site;

        private static uint _secuence = 1000;

        private static SPWebConfigModification siteEntry;
        private static SPWebConfigModification listEntry;

        private static bool _error;

        private static void Main(string[] args)
        {
            _argCount = 0;

            Console.WriteLine("Usage: [-a url] | [-d url]");

            foreach (string arg in args)
            {
                if (arg == "-a")
                {
                    GetUrl(args, arg);
                    Console.WriteLine("Activated ... in {0}", _url);
                    DoIt(false);
                }

                if (arg == "-d")
                {
                    GetUrl(args, arg);
                    Console.WriteLine("Deactivated ... in {0}", _url);
                    DoIt(true);
                }

                if (arg == "-c")
                {
                    GetUrl(args, arg);
                    GetKey(args, arg);
                    Console.WriteLine("Clean ... in {0}", _url);
                    Clean();
                }

                if (arg == "-s")
                {
                    GetUrl(args, arg);
                    Console.WriteLine("Show ... in {0}", _url);
                    Dump();
                }


                _argCount++;
            }
            Console.WriteLine("Done.");
        }

        private static void GetUrl(string[] args, string arg)
        {
            if (arg.Length > _argCount)
            {
                Console.WriteLine(args[_argCount + 1]);
                _url = args[_argCount + 1];
                _argCount++;
            }
        }

        private static void GetKey(string[] args, string arg)
        {
            if (arg.Length > _argCount)
            {
                Console.WriteLine(args[_argCount + 1]);
                _key = args[_argCount + 1];
                _argCount++;
            }
        }

        private static void Clean()
        {
            using (_site = new SPSite(_url))
            {
                Console.WriteLine("Open Web at Url.: {0}", _site.Url);
                Console.WriteLine("Application Name: {0}", _site.WebApplication.Name);

                Initialize();

                try
                {
                    SPWebApplication webApplication = _site.WebApplication;
                    List<SPWebConfigModification> toDelete = new List<SPWebConfigModification>();

                    foreach (SPWebConfigModification m in webApplication.WebConfigModifications)
                    {
                        if (m.Owner == "SPSWC" || (!string.IsNullOrEmpty(_key) && m.Owner == _key))
                        {
                            ShowModification(m);
                            toDelete.Add(m);                           
                        }
                    }

                    foreach (SPWebConfigModification m in toDelete)
                    {
                        string name = m.Name;
                        if (webApplication.WebConfigModifications.Remove(m))
                            Console.WriteLine("Removed ..." + name);
                    }

                    UpdateService(webApplication);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void DoIt(bool remove)
        {
            try
            {
                using(_site = new SPSite(_url))
                {
                    Console.WriteLine("Open Web at Url.: {0}", _site.Url);
                    Console.WriteLine("Application Name: {0}", _site.WebApplication.Name);

                    Initialize();

                    try
                    {
                        SPWebApplication webApplication = _site.WebApplication;

                        ModifyWebApplication(webApplication, listEntry, remove);
                        ModifyWebApplication(webApplication, siteEntry, remove);

                        if (!_error)
                        {
                            UpdateService(webApplication);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        Debug.WriteLine(ex.TargetSite);
                        Debug.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static void Initialize()
        {
            siteEntry = Modification("/configuration/appSettings",
                                      "SPSSiteUrl",
                                      _site.Url);

            listEntry = Modification("/configuration/appSettings",
                                      "SPSListName",
                                      "SPSProfessional Configuration Manager");
        }



        private static void Dump()
        {
            
            using(_site = new SPSite(_url))
            {
                Console.WriteLine("Open Web at Url.: {0}", _site.Url);
                Console.WriteLine("Application Name: {0}", _site.WebApplication.Name);

                Initialize();

                try
                {
                    SPWebApplication webApplication = _site.WebApplication;

                    foreach (SPWebConfigModification m in webApplication.WebConfigModifications)
                    {
                        ShowModification(m);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
        }

        private static void ShowModification(SPWebConfigModification m)
        {
            Console.WriteLine(string.Format("\n============"+
                                            "\nName: {0}"+
                                            "\nValue: {1}"+
                                            "\nOwner: {2}"+
                                            "\nSequence: {3}"+
                                            "\nType: {4}"+
                                            "\n------------",
                                            m.Name,
                                            m.Value,
                                            m.Owner,
                                            m.Sequence,
                                            m.Type));
        }

        private static void ModifyWebApplication(SPWebApplication app,
                                                 SPWebConfigModification modification,
                                                 bool removeModification)
        {
            foreach (SPWebConfigModification m in app.WebConfigModifications)
            {
                Debug.WriteLine(string.Format("{0}\n{1} - {2}\n{3}\n{4}\n\n",
                                              m.Name,
                                              m.Value,
                                              m.Owner,
                                              m.Sequence,
                                              m.Type));
            }

            if (app.WebConfigModifications.Contains(modification))
            {
                if (removeModification)
                {
                    //Console.WriteLine("Remove " + modification.Name);
                    //modification.Name = modification.Name.Replace("add", "remove");
                    Console.WriteLine("To Remove " + modification.Name);
                    if (app.WebConfigModifications.Remove(modification))
                        Console.WriteLine(" - Removed.");
                }
                else
                {
                    Console.WriteLine("Add [Error] Already Exist " + modification.Name);
                    _error = true;
                }
            }
            else
            {
                if (removeModification)
                {
                    Console.WriteLine("Remove [Error] No key name " + modification.Name);
                    _error = true;
                }
                else
                {
                    Console.WriteLine("Add " + modification.Name);
                    app.WebConfigModifications.Add(modification);
                }
            }
        }

        private static void UpdateService(SPWebApplication app)
        {
            app.Update(true);
            SPWebService service = app.Farm.Services.GetValue<SPWebService>();
            //service.Update();
            service.ApplyWebConfigModifications();
        }

        private static SPWebConfigModification Modification(string xpath, string key, string value)
        {
            string modification = string.Format("add[@key='{0}']", key);

            SPWebConfigModification configMod = new SPWebConfigModification(modification, xpath);

            configMod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

            configMod.Owner = "SPSCM";
            configMod.Value = string.Format(CultureInfo.InvariantCulture,
                                            string.Format("<add key=\"{0}\" value=\"{1}\"/>", key, value));
            
            configMod.Sequence = _secuence++;
            Debug.WriteLine(string.Format("{0} : {1}", key, value));
            return configMod;
        }
    }
}