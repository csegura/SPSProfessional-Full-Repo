using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Admin.ListTools
{
    public class AdminViewsPage : LayoutsPageBase
    {
        private string _pageRender;
        private SPList _currentList;
        private readonly List<SPGroup> _groups = new List<SPGroup>();
        private readonly List<SPView> _views = new List<SPView>();
        private Dictionary<int, Dictionary<Guid, bool>> _roleProperties;
        private Dictionary<int, Guid> _defaultViews;
        private readonly List<int> _hiddenFields = new List<int>();
        private readonly StringBuilder _computeFieldsScript = new StringBuilder();

        protected Button OK = new Button();
        protected Button Cancel = new Button();

        protected override void OnLoad(EventArgs e)
        {
            Title = "View Permission Settings";

            if (
                CurrentList.ParentWeb.Properties.ContainsKey(String.Format("ViewPermission{0}",
                                                                           CurrentList.ID)))
            {
                _roleProperties = new Dictionary<int, Dictionary<Guid, bool>>();
                _defaultViews = new Dictionary<int, Guid>();
                ViewPermissionUtil.ConvertFromStringForPage(ref _roleProperties, ref _defaultViews,
                                                            CurrentList.ParentWeb.Properties[
                                                                String.Format("ViewPermission{0}",
                                                                              CurrentList.ID)], CurrentList);
            }
            else
            {
                _roleProperties = new Dictionary<int, Dictionary<Guid, bool>>();
                _defaultViews = new Dictionary<int, Guid>();

                // Récupération des groups
                foreach (SPGroup group in CurrentList.ParentWeb.Groups)
                    _groups.Add(group);

                // Récupération des vues
                foreach (SPView view in CurrentList.Views)
                {
                    if ((!view.Hidden) && (!view.PersonalView))
                        _views.Add(view);
                }

                foreach (SPGroup group in _groups)
                {
                    _roleProperties.Add(group.ID, new Dictionary<Guid, bool>());
                    _defaultViews.Add(group.ID, CurrentList.DefaultView.ID);
                    foreach (SPView view in _views)
                        _roleProperties[group.ID].Add(view.ID, true);
                }
            }

            _pageRender = PrepareRenderPage();
            RegisterScript();

            Cancel.PostBackUrl = string.Format("~/_layouts/listedit.aspx?List={0}", CurrentList.ID);
        }

        protected string RenderPage()
        {
            return _pageRender;
        }

        protected SPList CurrentList
        {
            get
            {
                if (_currentList == null)
                    _currentList = SPContext.Current.Web.Lists[new Guid(Request.QueryString["List"])];

                return _currentList;
            }
        }

        private string PrepareRenderPage()
        {
            var result = new StringBuilder();

            // Table générale
            result.Append("<table style=\"width:100%\" cellpadding=\"0\" cellspacing=\"0\">");

            foreach (int groupId in _roleProperties.Keys)
            {
                result.Append("<tr><td colspan=\"2\" class=\"ms-sectionline\" style=\"height:1px;\" ></td></tr>");
                result.Append(
                    string.Format(
                        "<tr><td valign=\"top\" class=\"ms-sectionheader\" style=\"width:200px;padding-right:15px;\">{0}</td>",
                        CurrentList.ParentWeb.Groups.GetByID(groupId).Name));
                result.Append(
                    string.Format(
                        "<td class=\"ms-authoringcontrols\">{0}</td></tr><tr><td></td><td class=\"ms-authoringcontrols\" style=\"height:10px;\"></td></tr>",
                        RenderOptions(groupId)));
            }

            // Fermeture de la table générale
            result.Append("</table>");

            // Copyright

            result.Append("<script language=\"javascript\" type=\"text/javascript\">");
            result.Append("ComputeFields();");
            result.Append("</script>");

            return result.ToString();
        }

        private string RenderOptions(int groupId)
        {
            var result = new StringBuilder();

            // Default View
            result.Append("<table style=\"width: 100%\">");
            result.Append("<tr><td style=\"width: 100px;\"  class=\"ms-authoringcontrols\">");
            result.Append("Default view : ");
            result.Append("</td><td class=\"ms-authoringcontrols\">");
            result.Append(RenderDefaultView(groupId));
            result.Append("</td></tr>");
            result.Append("</table>");

            // Available views
            result.Append("<table style=\"width: 100%\">");
            result.Append("<tr><td style=\"width: 100px;vertical-align:top;\" class=\"ms-authoringcontrols\">");
            result.Append("Available views : ");
            result.Append("</td><td class=\"ms-authoringcontrols\">");
            result.Append(RenderAvailableViews(groupId));
            result.Append("</td></tr>");
            result.Append("</table>");

            return result.ToString();
        }

        private string RenderDefaultView(int groupId)
        {
            var result = new StringBuilder();
            var availableViews = new List<Guid>();
            Guid defaultView = _defaultViews[groupId];

            // Récupération de la liste des vues authorisées pour ce groupe
            foreach (Guid viewId in _roleProperties[groupId].Keys)
            {
                if (_roleProperties[groupId][viewId])
                    availableViews.Add(viewId);
            }


            result.Append(
                String.Format(
                    "<select id=\"Option{0}\" runat=\"server\" style=\"width: 150px;\" onchange=\"javascript:ComputeHidden('{0}');\">",
                    groupId));
            foreach (Guid viewId in availableViews)
            {
                if (CurrentList.Views[viewId] != null)
                {
                    string viewName = CurrentList.Views[viewId].Title;
                    result.Append(String.Format("<option {0}value=\"{1}\">{2}</option>",
                                                viewId.Equals(defaultView) ? "selected=\"selected\" " : "", viewId,
                                                viewName));
                }
            }

            result.Append("</select>");
            result.Append("</td></tr>");

            return result.ToString();
        }

        private string RenderAvailableViews(int groupId)
        {
            var result = new StringBuilder();

            result.Append(string.Format("<div id=\"Div{0}\">", groupId));
            // Récupération de la liste des vues
            foreach (SPView view in CurrentList.Views)
            {
                if ((!view.Hidden) && (!view.PersonalView))
                {
                    if ((_roleProperties[groupId].ContainsKey(view.ID)) && (_roleProperties[groupId][view.ID] ))
                        result.Append(
                            string.Format(
                                "<input id=\"Chk{0}{1}\" title=\"{2}\" type=\"checkbox\" value=\"{1}\" onclick=\"javascript:OptionChange('{0}','chk{0}{1}');\" checked=\"checked\"/> {2}<br/>",
                                groupId, view.ID, view.Title));
                    else
                        result.Append(
                            string.Format(
                                "<input id=\"Chk{0}{1}\" title=\"{2}\" type=\"checkbox\" value=\"{1}\" onclick=\"javascript:OptionChange('{0}','chk{0}{1}');\" /> {2}<br/>",
                                groupId, view.ID, view.Title));
                }
            }

            result.Append("</div>");
            result.Append("</td></tr>");
            UpdateGloablScript(groupId);

            return result.ToString();
        }

        private void UpdateGloablScript(int groupId)
        {
            if (!_hiddenFields.Contains(groupId))
            {
                _hiddenFields.Add(groupId);
                _computeFieldsScript.Append(String.Format("ComputeHidden(\"{0}\");", groupId));
            }

            ClientScript.RegisterHiddenField(String.Format("Hidden{0}", groupId), "");
        }

        protected void SaveCustomPermission(object sender, EventArgs e)
        {
            string valueGlobal = string.Empty;

            foreach (int groupId in _hiddenFields)
            {
                string value = HttpContext.Current.Request.Params[string.Format("Hidden{0}", groupId)];
                if (value.Length > 0)
                    valueGlobal += string.Format("{0}|", value.Substring(0, value.Length - 1));
                else
                    valueGlobal += string.Format("{0}|", value.Substring(0, value.Length));
            }
            valueGlobal = valueGlobal.Substring(0, valueGlobal.Length - 1);

            if (
                !CurrentList.ParentWeb.Properties.ContainsKey(String.Format("ViewPermission{0}",
                                                                            CurrentList.ID)))
                CurrentList.ParentWeb.Properties.Add(String.Format("ViewPermission{0}", CurrentList.ID),
                                                     valueGlobal);
            else
                CurrentList.ParentWeb.Properties[String.Format("ViewPermission{0}", CurrentList.ID)] =
                    valueGlobal;

            CurrentList.ParentWeb.Properties.Update();

            SPUtility.Redirect(string.Format("listedit.aspx?List={0}", CurrentList.ID),
                              SPRedirectFlags.RelativeToLayoutsPage,
                              HttpContext.Current);

        }


        private void RegisterScript()
        {
            _computeFieldsScript.Insert(0, "function ComputeFields(){");
            _computeFieldsScript.Append("}");
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "ComputeFields", _computeFieldsScript.ToString(), true);

            string OptionChangeScript =
                @"function OptionChange(id,sender)
                                    {
                                        if(CountChecked(id) == 0)
                                            document.getElementById(sender).checked = true;
                                        else
                                            OptionBind(id);
                                    }";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "OptionChange", OptionChangeScript, true);

            string CountCheckedScript =
                @"function CountChecked(id) 
                                        {
                                            var result = 0;
                                            var panel = document.getElementById('Div' + id);
                                            
                                            for(index=0;index < panel.childNodes.length;index++) 
                                            {
                                                var ctrl = panel.childNodes.item(index);
                                                if((ctrl.name != null) && (ctrl.checked))
                                                    result++;
                                            }
                                            
                                            return result;
                                        }";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "CountChecked", CountCheckedScript, true);

            string OptionBindScript =
                @"function OptionBind(id)
                                        {
                                            var selectCtrl = document.getElementById('Option' + id);
                                            var panel = document.getElementById('Div' + id);
                                            var i = 0;
                                            var lenght = CountChecked(id);
                                            
                                            selectCtrl.options.length = lenght;                                           

                                            for(index=0;index < panel.childNodes.length;index++) 
                                            {
                                                var ctrl = panel.childNodes.item(index);
                                                if((ctrl.name != null) && (ctrl.checked))
                                                {
                                                    selectCtrl.options[i].value = ctrl.value;
                                                    selectCtrl.options[i].text = ctrl.title;
                                                    i++;
                                                }
                                            }
                                            
                                            selectCtrl.selectedIndex = 0;
                                            ComputeHidden(id);
                                        }";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "OptionBind", OptionBindScript, true);

            string ComputeHiddenScript =
                @"function ComputeHidden(id)
                                        {
                                            var selectCtrl = document.getElementById('Option' + id);
                                            var panel = document.getElementById('Div' + id);
                                            var hidden = document.getElementById('Hidden' + id);
                                            var groupId = id;
                                            var defaultView = selectCtrl.options[selectCtrl.selectedIndex].value
                                            var _views = '';
                                            
                                            for(index=0;index < panel.childNodes.length;index++) 
                                            {
                                                var ctrl = panel.childNodes.item(index);
                                                if((ctrl.name != null) && (ctrl.checked))
                                                {
                                                    _views = _views + ctrl.value + ';'
                                                }
                                            }

                                            hidden.value = groupId + '#' + defaultView + '#' + _views;
                                        }";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "ComputeHidden", ComputeHiddenScript, true);
        }
    }
}