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
    public class AdminFormFieldsPage : LayoutsPageBase
    {
        private string _pageRender;
        private SPList _currentList;
        private readonly List<SPField> _displayableFields = new List<SPField>();
        private readonly List<SPGroup> _groups = new List<SPGroup>();
        private readonly StringBuilder _computeFieldsScript = new StringBuilder();

        private readonly Dictionary<string, Dictionary<string, string>> _hiddenFields =
            new Dictionary<string, Dictionary<string, string>>();

        private Dictionary<string, Dictionary<string, string>> _fieldProperties;

        protected Button OK = new Button();
        protected Button Cancel = new Button();

        protected override void OnLoad(EventArgs e)
        {
            Title = "Administrate Form Fields";

            if (
                CurrentList.ParentWeb.Properties.ContainsKey(String.Format("DisplaySetting{0}",
                                                                           CurrentList.ID)))
                _fieldProperties =
                    SPSWebPropertiesHelper.Decode(
                        CurrentList.ParentWeb.Properties[String.Format("DisplaySetting{0}", CurrentList.ID)]);

            // récupération de la liste des champs affichables et des settings dejà défini
            foreach (SPField field in CurrentList.Fields)
            {
                if ((field.Reorderable))
                    _displayableFields.Add(field);
            }

            // Récupération des groups
            foreach (SPGroup group in CurrentList.ParentWeb.Groups)
                _groups.Add(group);

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

            foreach (SPField field in _displayableFields)
            {
                result.Append("<tr><td colspan=\"2\" class=\"ms-sectionline\" style=\"height:1px;\" ></td></tr>");
                result.Append(
                    string.Format("<tr><td valign=\"top\" class=\"ms-sectionheader\" style=\"width:120px\">{0}</td>",
                                  field.Title));
                result.Append(
                    string.Format(
                        "<td class=\"ms-authoringcontrols\">{0}</td></tr><tr><td></td><td class=\"ms-authoringcontrols\" style=\"height:10px;\"></td></tr>",
                        RenderOptions(field)));
            }

            // Fermeture de la table générale
            result.Append("</table>");

            return result.ToString();
        }

        private string RenderOptions(SPField field)
        {
            var result = new StringBuilder();
            bool showWhere = false;

            // New mode
            result.Append("<table style=\"width: 100%\">");
            result.Append("<tr><td style=\"width: 150px;\"  class=\"ms-authoringcontrols\">");
            result.Append("New : ");
            result.Append("</td><td class=\"ms-authoringcontrols\">");
            if (field.Required)
                result.Append("Always (This field is requiered)");
            else
            {
                result.Append(RenderOption(field, "New", ref showWhere));
                result.Append(RenderPanelWhere(field, "New", ref showWhere));
                UpdateGloablScript(field, "New");
            }
            result.Append("</td></tr>");
            result.Append("</table>");

            // Display mode
            result.Append("<table style=\"width: 100%\">");
            result.Append("<tr><td style=\"width: 150px;\" class=\"ms-authoringcontrols\">");
            result.Append("Display : ");
            result.Append("</td><td class=\"ms-authoringcontrols\">");
            result.Append(RenderOption(field, "Display", ref showWhere));
            result.Append(RenderPanelWhere(field, "Display", ref showWhere));
            UpdateGloablScript(field, "Display");
            result.Append("</td></tr>");
            result.Append("</table>");

            // Edit mode
            result.Append("<table style=\"width: 100%\">");
            result.Append("<tr><td style=\"width: 150px;\"  class=\"ms-authoringcontrols\">");
            result.Append("Edit : ");
            result.Append("</td><td class=\"ms-authoringcontrols\">");
            result.Append(RenderOption(field, "Edit", ref showWhere));
            result.Append(RenderPanelWhere(field, "Edit", ref showWhere));
            UpdateGloablScript(field, "Edit");
            result.Append("</td></tr>");
            result.Append("</table>");

            return result.ToString();
        }

        private void UpdateGloablScript(SPField field, string mode)
        {
            _computeFieldsScript.Append(String.Format("ComputeField(\"{0}{1}\");", field.InternalName, mode));
            if (!_hiddenFields.ContainsKey(field.InternalName))
                _hiddenFields.Add(field.InternalName, new Dictionary<string, string>());

            _hiddenFields[field.InternalName].Add(mode, String.Format("Hidden{0}{1}", field.InternalName, mode));
            ClientScript.RegisterHiddenField(String.Format("Hidden{0}{1}", field.InternalName, mode), "");
        }

        protected void SaveCustomDisplay(object sender, EventArgs e)
        {
            _fieldProperties = new Dictionary<string, Dictionary<string, string>>();

            foreach (string field in _hiddenFields.Keys)
            {
                _fieldProperties.Add(field, new Dictionary<string, string>());
                foreach (string mode in _hiddenFields[field].Keys)
                    _fieldProperties[field].Add(mode, HttpContext.Current.Request.Params[_hiddenFields[field][mode]]);
            }

            if (
                !CurrentList.ParentWeb.Properties.ContainsKey(String.Format("DisplaySetting{0}",
                                                                            CurrentList.ID)))
                CurrentList.ParentWeb.Properties.Add(String.Format("DisplaySetting{0}", CurrentList.ID),
                                                     SPSWebPropertiesHelper.Encode(_fieldProperties));
            else
                CurrentList.ParentWeb.Properties[String.Format("DisplaySetting{0}", CurrentList.ID)] =
                    SPSWebPropertiesHelper.Encode(_fieldProperties);

            CurrentList.ParentWeb.Properties.Update();

            SPUtility.Redirect(string.Format("listedit.aspx?List={0}", CurrentList.ID),
                               SPRedirectFlags.RelativeToLayoutsPage, 
                               HttpContext.Current);
            //Server.Transfer(string.Format("~/_layouts/listedit.aspx?List={0}", CurrentList.ID), true);
        }

        private string RenderOption(SPField field, string mode, ref bool showWhere)
        {
            var result = new StringBuilder();

            result.Append(
                String.Format(
                    "<select id=\"Option{0}{1}\" runat=\"server\" onchange=\"javascript:OptionChange('{0}{1}');\" style=\"width: 100px;\">",
                    field.InternalName, mode));

            if ((_fieldProperties != null) && (_fieldProperties.ContainsKey(field.InternalName)) &&
                (_fieldProperties[field.InternalName].ContainsKey(mode)))
            {
                string optionMode = _fieldProperties[field.InternalName][mode];
                string optionValue = optionMode.Split(";".ToCharArray())[0];

                result.Append(String.Format("<option {0}value=\"always\">Always</option>",
                                            optionValue.Equals("always") ? "selected=\"selected\"" : ""));
                result.Append(String.Format("<option {0}value=\"never\">Never</option>",
                                            optionValue.Equals("never") ? "selected=\"selected\"" : ""));
                result.Append(String.Format("<option {0}value=\"where\">Where</option>",
                                            optionValue.Equals("where") ? "selected=\"selected\"" : ""));
                showWhere = optionValue.Equals("where") ? true : false;
            }
            else
            {
                result.Append("<option selected=\"selected\" value=\"always\">Always</option>");
                result.Append("<option value=\"never\">Never</option>");
                result.Append("<option value=\"where\">Where</option>");
            }

            result.Append("</select>");
            result.Append("</td></tr>");

            return result.ToString();
        }

        private string RenderPanelWhere(SPField field, string mode, ref bool showWhere)
        {
            var result = new StringBuilder();

            if (showWhere)
                result.Append(
                    String.Format(
                        "<tr id=\"RowOptionPanel{0}{1}\" style=\"display:inline;\"><td style=\"width: 150px;\" ></td><td class=\"ms-authoringcontrols\">",
                        field.InternalName, mode));
            else
                result.Append(
                    String.Format(
                        "<tr id=\"RowOptionPanel{0}{1}\" style=\"display:none;\"><td style=\"width: 150px;\" ></td><td class=\"ms-authoringcontrols\">",
                        field.InternalName, mode));

            result.Append(RenderWhere(field, mode));
            showWhere = false;

            return result.ToString();
        }

        private string RenderWhere(SPField field, string mode)
        {
            var result = new StringBuilder();

            result.Append("<table cellpadding=\"0px\" cellspacing=\"0px\" style=\"width: 100%;\">");
            result.Append("<tr><td>");

            if ((_fieldProperties != null) && (_fieldProperties.ContainsKey(field.InternalName)) &&
                (_fieldProperties[field.InternalName].ContainsKey(mode)))
            {
                string memMode = _fieldProperties[field.InternalName][mode];
                string memModeValue = memMode.Split(";".ToCharArray())[0];
                string memOptionFieldWhere = memMode.Split(";".ToCharArray())[1];
                string memOptionConditionWhere = memMode.Split(";".ToCharArray())[2];
                string memOptionValueUserWhere = memMode.Split(";".ToCharArray())[3];
                string memOptionValueFieldWhere = memMode.Split(";".ToCharArray())[4];

                //this.resultLabel.Text += string.Format("<BR/>{0}", memMode);

                if (memModeValue.Equals("where"))
                {
                    result.Append(
                        String.Format(
                            "<select id=\"OptionFieldWhere{0}{1}\" runat=\"server\" onchange=\"javascript:OptionFieldWhereChange('{0}{1}');\">",
                            field.InternalName, mode));
                    if (memOptionFieldWhere.Equals("[Me]"))
                        result.Append("<option selected=\"selected\" value=\"[Me]\">[Me]</option>");
                    else
                        result.Append("<option value=\"[Me]\">[Me]</option>");

                    /* Indisponible dans la version 1.0.0.0
                    foreach (SPField fieldItem in _displayableFields)
                    {
                        if (memOptionFieldWhere.Equals(fieldItem.InternalName))
                            result.Append(String.Format("<option selected=\"selected\" value=\"{0}\">{1}</option>", fieldItem.InternalName, fieldItem.Title));
                        else
                            result.Append(String.Format("<option value=\"{0}\">{1}</option>", fieldItem.InternalName, fieldItem.Title));
                    }
                    */
                    result.Append("</select>");


                    result.Append(
                        String.Format(
                            "<select id=\"OptionConditionWhere{0}{1}\" runat=\"server\" style=\"width: 200px\">",
                            field.InternalName, mode));
                    if (memOptionFieldWhere.Equals("[Me]"))
                    {
                        result.Append(String.Format("<option {0}value=\"IsInGroup\">Is in group</option>",
                                                    memOptionConditionWhere.Equals("IsInGroup")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                        result.Append(String.Format("<option {0}value=\"IsNotInGroup\">In not in group</option>",
                                                    memOptionConditionWhere.Equals("IsNotInGroup")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                    }
                    else
                    {
                        result.Append(String.Format("<option {0}value=\"IsEqualTo\">Is equal to</option>",
                                                    memOptionConditionWhere.Equals("IsEqualTo")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                        result.Append(String.Format("<option {0}value=\"IsNotEqualTo\">Is not equal to</option>",
                                                    memOptionConditionWhere.Equals("IsNotEqualTo")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                        result.Append(String.Format("<option {0}value=\"IsGreaterThan\">Is greater than</option>",
                                                    memOptionConditionWhere.Equals("IsGreaterThan")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                        result.Append(String.Format("<option {0}value=\"IsLessThan\">Is less than</option>",
                                                    memOptionConditionWhere.Equals("IsLessThan")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                        result.Append(
                            String.Format(
                                "<option {0}value=\"IsGreaterOrEqualThan\">Is greater or equal than</option>",
                                memOptionConditionWhere.Equals("IsGreaterOrEqualThan") ? "selected=\"selected\" " : ""));
                        result.Append(
                            String.Format("<option {0}value=\"IsLessOrEqualThan\">Is less or equal than</option>",
                                          memOptionConditionWhere.Equals("IsLessOrEqualThan")
                                              ? "selected=\"selected\" "
                                              : ""));
                        result.Append(String.Format("<option {0}value=\"BeginWith\">Begin with</option>",
                                                    memOptionConditionWhere.Equals("BeginWith")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                        result.Append(String.Format("<option {0}value=\"EndWith\">End with</option>",
                                                    memOptionConditionWhere.Equals("EndWith")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                        result.Append(String.Format("<option {0}value=\"Contains\">Contains</option>",
                                                    memOptionConditionWhere.Equals("Contains")
                                                        ? "selected=\"selected\" "
                                                        : ""));
                    }
                    result.Append("</select>");

                    if (memOptionFieldWhere.Equals("[Me]"))
                    {
                        result.Append(
                            String.Format(
                                "<select id=\"OptionValueUserWhere{0}{1}\" runat=\"server\" style=\"width: 200px; display:inline\">",
                                field.InternalName, mode));
                        foreach (SPGroup group in _groups)
                            result.Append(String.Format("<option {0}value=\"{1}\">{2}</option>",
                                                        memOptionValueUserWhere.Equals(group.Name)
                                                            ? "selected=\"selected\" "
                                                            : "", group.Name, group.Name));
                    }
                    else
                    {
                        result.Append(
                            String.Format(
                                "<select id=\"OptionValueUserWhere{0}{1}\" runat=\"server\" style=\"width: 200px; display:none\">",
                                field.InternalName, mode));
                        foreach (SPGroup group in _groups)
                            result.Append(String.Format("<option value=\"{0}\">{1}</option>", group.Name, group.Name));
                    }
                    result.Append("</select>");

                    if (memOptionFieldWhere.Equals("[Me]"))
                        result.Append(
                            String.Format(
                                "<input id=\"OptionValueFieldWhere{0}{1}\" class=\"ms-long\" style=\"width: 200px; display: none\" />",
                                field.InternalName, mode));
                    else
                        result.Append(
                            String.Format(
                                "<input id=\"OptionValueFieldWhere{0}{1}\" class=\"ms-long\" style=\"width: 200px; display: inline\" value=\"{2}\" />",
                                field.InternalName, mode, memOptionValueFieldWhere));
                }
                else
                {
                    result.Append(
                        String.Format(
                            "<select id=\"OptionFieldWhere{0}{1}\" runat=\"server\" onchange=\"javascript:OptionFieldWhereChange('{0}{1}');\">",
                            field.InternalName, mode));
                    result.Append("<option selected=\"selected\" value=\"[Me]\">[Me]</option>");

                    /* Indisponible dans la version 1.0.0.0
                    foreach (SPField fieldItem in _displayableFields)
                        result.Append(String.Format("<option value=\"{0}\">{1}</option>", fieldItem.InternalName, fieldItem.Title));
                    */
                    result.Append("</select>");

                    result.Append(
                        String.Format(
                            "<select id=\"OptionConditionWhere{0}{1}\" runat=\"server\" style=\"width: 200px\">",
                            field.InternalName, mode));
                    result.Append("<option selected=\"selected\" value=\"IsInGroup\">Is in group</option>");
                    result.Append("<option value=\"IsNotInGroup\">In not in group</option>");
                    result.Append("</select>");

                    result.Append(
                        String.Format(
                            "<select id=\"OptionValueUserWhere{0}{1}\" runat=\"server\" style=\"width: 200px\">",
                            field.InternalName, mode));
                    foreach (SPGroup group in _groups)
                        result.Append(String.Format("<option value=\"{0}\">{1}</option>", group.Name, group.Name));
                    result.Append("</select>");

                    result.Append(
                        String.Format(
                            "<input id=\"OptionValueFieldWhere{0}{1}\" class=\"ms-long\" style=\"width: 200px; display: none\" />",
                            field.InternalName, mode));
                }
            }
            else
            {
                result.Append(
                    String.Format(
                        "<select id=\"OptionFieldWhere{0}{1}\" runat=\"server\" onchange=\"javascript:OptionFieldWhereChange('{0}{1}');\">",
                        field.InternalName, mode));
                result.Append("<option selected=\"selected\" value=\"[Me]\">[Me]</option>");

                /* Indisponible dans la version 1.0.0.0
                foreach (SPField fieldItem in _displayableFields)
                    result.Append(String.Format("<option value=\"{0}\">{1}</option>", fieldItem.InternalName, fieldItem.Title));
                */
                result.Append("</select>");

                result.Append(
                    String.Format("<select id=\"OptionConditionWhere{0}{1}\" runat=\"server\" style=\"width: 200px\">",
                                  field.InternalName, mode));
                result.Append("<option selected=\"selected\" value=\"IsInGroup\">Is in group</option>");
                result.Append("<option value=\"IsNotInGroup\">In not in group</option>");
                result.Append("</select>");

                result.Append(
                    String.Format("<select id=\"OptionValueUserWhere{0}{1}\" runat=\"server\" style=\"width: 200px\">",
                                  field.InternalName, mode));
                foreach (SPGroup group in _groups)
                    result.Append(String.Format("<option value=\"{0}\">{1}</option>", group.Name, group.Name));
                result.Append("</select>");

                result.Append(
                    String.Format(
                        "<input id=\"OptionValueFieldWhere{0}{1}\" class=\"ms-long\" style=\"width: 200px; display: none\" />",
                        field.InternalName, mode));
            }

            //result.Append(String.Format("&nbsp<a id=\"OptionAddMoreWhere{0}{1}\" class=\"ms-authoringcontrols\" style=\"cursor: pointer;\" onclick=\"javascript:AddMoreCondition('OptionAddMoreWhere{0}{1}','{0}{1}');\">Add more condition ...</a>", field.InternalName, mode));        
            result.Append(String.Format("<input id=\"Hidden{0}{1}\" type=\"hidden\" />", field.InternalName, mode));

            result.Append("</td></tr>");
            result.Append("</table>");

            return result.ToString();
        }


        private void RegisterScript()
        {
            _computeFieldsScript.Insert(0, "function ComputeFields(){");
            _computeFieldsScript.Append("}");
            Page.ClientScript.RegisterClientScriptBlock(GetType(), 
                                                        "ComputeFields", 
                                                        _computeFieldsScript.ToString(),
                                                        true);

            Page.ClientScript.RegisterClientScriptInclude("ListAdminTools", "/_layouts/SPSProfessional_ListAdminTools.js");

        }
    }
}