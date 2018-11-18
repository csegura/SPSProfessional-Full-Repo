using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Admin.ListTools
{
    public class AdminViewSelectorMenu : ViewSelectorMenu
    {
        private Dictionary<int, Dictionary<Guid, bool>> _roleProperties;
        private Dictionary<int, Guid> _defaultViews;
        private bool _featureEnabled;

        protected override void OnLoad(EventArgs e)
        {
            if (Visible)
            {
                if (CheckIfFeatureIsEnabled())
                {
                    string key = String.Format("ViewPermission{0}", SPContext.Current.List.ID);

                    if (
                        SPContext.Current.List.ParentWeb.Properties.ContainsKey(key))
                    {
                        _featureEnabled = true;
                        _roleProperties = new Dictionary<int, Dictionary<Guid, bool>>();
                        _defaultViews = new Dictionary<int, Guid>();

                        using (SPWeb web = SPContext.Current.List.ParentWeb)
                        {
                            ViewPermissionUtil.ConvertFromString(ref _roleProperties, ref _defaultViews,
                                                                 web.Properties[key],
                                                                 SPContext.Current.List);

                            if (!UserCanSeeView(RenderContext.ViewContext.View.ID, _roleProperties))
                                SPUtility.Redirect(GoToDefaultView(_defaultViews).ServerRelativeUrl,
                                                   SPRedirectFlags.Default, HttpContext.Current, "redirect=true");
                            else
                            {
                                if (!ComeFromView())
                                    SPUtility.Redirect(GoToDefaultView(_defaultViews).ServerRelativeUrl,
                                                       SPRedirectFlags.Default, HttpContext.Current, "redirect=true");
                            }
                        }
                    }
                }


                base.OnLoad(e);
            }
            else
                base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter output)
        {
            if (Visible)
            {
                if (_featureEnabled)
                {
                    foreach (Control item in MenuTemplateControl.Controls)
                    {
                        try
                        {
                            if ((item is MenuItemTemplate) &&
                                (SPContext.Current.List.Views[((MenuItemTemplate) item).Text]) != null)
                                item.Visible =
                                    UserCanSeeView(SPContext.Current.List.Views[((MenuItemTemplate) item).Text].ID,
                                                   _roleProperties);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                base.Render(output);
            }
            else
                base.Render(output);
        }

        private bool UserCanSeeView(Guid viewId, IDictionary<int, Dictionary<Guid, bool>> roleProperties)
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["redirect"]))
            {
                using (SPWeb webSite = SPContext.Current.Web)
                {
                    SPUser user = webSite.CurrentUser;
                    SPGroupCollection userGroups = user.Groups;

                    if (userGroups.Count > 0)
                    {
                        foreach (SPGroup group in userGroups)
                        {
                            if (roleProperties.ContainsKey(group.ID))
                            {
                                if ((roleProperties[group.ID].ContainsKey(viewId)) &&
                                    roleProperties[group.ID][viewId])
                                    return true;
                            }
                        }
                        return false;
                    }
                    return true;
                }
            }

            return true;
        }

        private static SPView GoToDefaultView(IDictionary<int, Guid> defaultViews)
        {
            using (SPWeb webSite = SPContext.Current.Web)
            {
                SPUser user = webSite.CurrentUser;
                SPGroupCollection userGroups = user.Groups;

                if (userGroups.Count > 0)
                {
                    foreach (SPGroup group in userGroups)
                    {
                        if (defaultViews.ContainsKey(group.ID))
                        {
                            if (SPContext.Current.List.Views[defaultViews[group.ID]] != null)
                                return SPContext.Current.List.Views[defaultViews[group.ID]];
                        }
                    }

                    return SPContext.Current.List.DefaultView;
                }
                
                return SPContext.Current.List.DefaultView;
            }
        }

        private static bool ComeFromView()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["redirect"]))
                return true;
            
            string urlReferer = HttpContext.Current.Request.CurrentExecutionFilePath.ToLower();
            
            foreach (SPView view in SPContext.Current.List.Views)
            {
                if (view.ServerRelativeUrl.ToLower().Equals(urlReferer))
                    return true;
            }

            return false;
        }

        private static bool CheckIfFeatureIsEnabled()
        {
            SPFeature feature = SPContext.Current.Site.Features[new Guid("C509490D-0EAC-46E5-971B-F90093F9D182")];

            if ((feature != null) &&
                (feature.Definition.Status == SPObjectStatus.Online))
                return true;

            return false;
        }
    }
}