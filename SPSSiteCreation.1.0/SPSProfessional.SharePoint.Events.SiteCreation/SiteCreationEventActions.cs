using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class SiteCreationEventActions
    {
        /// <summary>
        /// Creates the web.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="name">The name.</param>
        /// <param name="template">The template.</param>
        /// <param name="uniquePermissions"><c>true</c> to create a subsite that does not inherit permissions from another site; otherwise, false</param>
        /// <param name="alternativeIfExist">if set to <c>true</c> [alternative if exist].</param>
        /// <returns>The new</returns>
        public SPWeb CreateWeb(string url, string name, string template, bool uniquePermissions, bool alternativeIfExist)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Web url can't be null or empty.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Web name can't be null or empty.");
            }

            if (string.IsNullOrEmpty(template))
            {
                throw new ArgumentException("Template name can't be null or empty.");
            }

            string namedUrl = RemoveDiacritics((RemoveInvalidCharacters(name)));

            using(SPSite _site = new SPSite(url))
            {
                using(SPWeb web = _site.OpenWeb())
                {
                    SPWeb newWeb;

                    if (web == null)
                    {
                        throw new SPSException(string.Format("Can´t open parent web at '{0}'",
                                                             url));
                    }

                    Debug.WriteLine(string.Format("Trying create test web '{0}' - at '{1}/{2}'",
                                                  name,
                                                  url,
                                                  namedUrl));

                    
                    // check if new site exist and lo
                    if (ExistUrl(web,namedUrl))
                    {
                        if (alternativeIfExist)
                        {
                            namedUrl = GetNewValidUrl(web, namedUrl);
                        }
                        else
                        {
                            throw new SPSException(string.Format("Unable to create web. Already exist at '{0}/{1}'", url,namedUrl));
                        }
                    }

                    SPWebTemplate webTemplate = GetCustomWebTemplate(template, _site, web);

                    #region Custom Template

                    if (webTemplate != null)
                    {
                        try
                        {
                            newWeb = web.Webs.Add(namedUrl,
                                                  name,
                                                  string.Empty,
                                                  (uint) web.Locale.LCID,
                                                  webTemplate,
                                                  uniquePermissions,
                                                  false);

                            Debug.WriteLine(string.Format("Created web '{0}' at '{1}'",
                                                          newWeb.Title,
                                                          newWeb.Url));


                            return newWeb;
                        }
                        catch (SPException ex)
                        {
                            throw new SPSException(string.Format("Unable create web at '{0}' named '{1}/{2}'",
                                                                 url,
                                                                 name,
                                                                 namedUrl),
                                                   ex);
                        }
                    }

                    #endregion

                    // not a custom template, search here
                    webTemplate = GetWebTemplate(template, _site, web);

                    #region Web Template

                    if (webTemplate != null)
                    {
                        try
                        {
                            newWeb = web.Webs.Add(namedUrl,
                                                  name,
                                                  string.Empty,
                                                  (uint) web.Locale.LCID,
                                                  webTemplate,
                                                  uniquePermissions,
                                                  false);

                            Debug.WriteLine(string.Format("Created web '{0}' at '{1}'",
                                                          newWeb.Title,
                                                          newWeb.Url));
                            return newWeb;
                        }
                        catch (Exception ex)
                        {
                            throw new SPSException(string.Format("Unable create web at '{0}' named '{1}/{2}'",
                                                                 url,
                                                                 name,
                                                                 namedUrl),
                                                   ex);
                        }
                    }

                    #endregion

                    throw new SPSException(string.Format("Template '{0}' not found.", template));
                }
            }
        }
    

        /// <summary>
        /// Adds the on quick launch bar.
        /// </summary>
        /// <param name="web">The web.</param>
        public void AddOnQuickLaunchBar(SPWeb web)
        {
            using(SPWeb rootWeb = web.Site.OpenWeb())
            {
                SPNavigationNodeCollection nodes = rootWeb.Navigation.QuickLaunch;
                SPNavigationNode navNode = new SPNavigationNode(web.Title, web.ServerRelativeUrl, false);

                foreach (SPNavigationNode node in nodes)
                {
                    if (node.Id == 1026)
                    {
                        node.Children.AddAsLast(navNode);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the use shared navbar.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="useSharedNavbar">if set to <c>true</c> [use shared navbar].</param>
        public void SetUseSharedNavbar(SPWeb web, bool useSharedNavbar)
        {
            web.Navigation.UseShared = useSharedNavbar;
        }

        /// <summary>
        /// Deletes the web.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void DeleteWeb(string url)
        {
            Debug.WriteLine(string.Format("Try delete web at '{0}'", url));

            try
            {
                using(SPSite site = new SPSite(url))
                {
                    using(SPWeb web = site.OpenWeb())
                    {
                        web.Delete();
                        Debug.WriteLine("Deleted");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SPSException(string.Format("Unable delete web at '{0}'",
                                                     url),
                                       ex);
            }
        }

        private string GetNewValidUrl(SPWeb web, string url)
        {
            while( ExistUrl(web, url) )
            {
                int i = 0;

                // kick #
                while ((i<url.Length && char.IsNumber(url[url.Length-(i+1)]) && i++ > 0))
                {
                }

                if (i >= 1)
                {
                    int version = Int16.Parse(url.Substring(url.Length - i));
                    url = url.Substring(0,url.Length - i) + (++version);
                }
                else
                {
                    url = url + "0";
                }

            }

            return url;
        }

        private bool ExistUrl(SPWeb web, string url)
        {
            url = url.Substring(url.LastIndexOf('/') + 1);

            foreach(string name in web.Webs.Names)
            {
                if (url == name)
                {
                    return true;
                }
            }
            return false;
        }

        #region Private Methods

        private SPWebTemplate GetWebTemplate(string template, SPSite site, SPWeb web)
        {
            SPWebTemplate webTemplate;
            SPWebTemplateCollection templates = site.GetWebTemplates((uint)web.Locale.LCID);

            webTemplate = LocateTemplateByName(templates, template);

            return webTemplate;
        }

        private SPWebTemplate GetCustomWebTemplate(string template, SPSite site, SPWeb web)
        {
            SPWebTemplateCollection templates;
            SPWebTemplate webTemplate;

            // Create a web with custom based template
            templates = site.GetCustomWebTemplates((uint)web.Locale.LCID);

            webTemplate = LocateTemplateByName(templates, template);

            return webTemplate;
        }

        private SPWebTemplate LocateTemplateByName(SPWebTemplateCollection templates, string templateTitle)
        {
            foreach (SPWebTemplate template in templates)
            {
                if (template.Title == templateTitle)
                {
                    return template;
                }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Removes the invalid characters.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Valid Url name</returns>
        private static string RemoveInvalidCharacters(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            //We should trim the input before we do length check.
            //This way, we could be able to fit in more characters in
            //our output if there are spaces in the beginning.

            name = name.Trim();

            string[] invalidCharacters =
                    new[]
                        {
                                "#", "%", "&", "*", ":", "<", ">", "?", "\\", "/", "{", "}", "~",
                                "+", "-", ",", "(", ")", "|",
                                "."
                        };

            Regex cleanUpRegex = GetCharacterRemovalRegex(invalidCharacters);

            string cleanName = cleanUpRegex.Replace(name, string.Empty);

            //cleanName = cleanName.Replace(" ", "%20");                        

            cleanName = cleanName.Replace(" ", string.Empty);

            if (cleanName.StartsWith("_"))
            {
                cleanName = cleanName.Substring(1);
            }

            if (cleanName.Length > 50)
            {
                cleanName = cleanName.Substring(0, 50);
            }

            return cleanName;
        }

        /// <summary>
        /// Gets the character removal regex.
        /// </summary>
        /// <param name="invalidCharacters">The invalid characters.</param>
        /// <returns>Regular expression</returns>
        private static Regex GetCharacterRemovalRegex(ICollection<string> invalidCharacters)
        {
            if (invalidCharacters == null)
            {
                throw new ArgumentNullException("invalidCharacters");
            }

            if (invalidCharacters.Count == 0)
            {
                throw new ArgumentException("invalidCharacters can not be empty.",
                                            "invalidCharacters");
            }

            string[] escapedCharacters = new string[invalidCharacters.Count];

            int index = 0;

            foreach (string input in invalidCharacters)
            {
                escapedCharacters[index] = Regex.Escape(input);

                index++;
            }

            return new Regex(string.Join("|", escapedCharacters));
        }

        private  string RemoveDiacritics(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int ich = 0; ich < normalized.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(normalized[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalized[ich]);
                }
            }

            return (stringBuilder.ToString());
        }

        public void SetPermissions(SPWeb web, string permissions)
        {
            if (!string.IsNullOrEmpty(permissions))
            {
                string[] groupDescriptionPairs = permissions.Split(';');

                foreach(string groupDescription in groupDescriptionPairs)
                {
                    string group = groupDescription.Split(':')[0];
                    string description = groupDescription.Split(':')[1];

                    SiteCreationPermissions.AddGroup(web, group, description, null, null, null, false);
                }
            }
        }
    }
}