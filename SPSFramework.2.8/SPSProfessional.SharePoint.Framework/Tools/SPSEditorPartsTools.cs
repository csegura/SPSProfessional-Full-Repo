using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using WebPart=System.Web.UI.WebControls.WebParts.WebPart;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    /// <summary>
    /// This class provide a set of tools to configurate the SharePoint EditorParts
    /// </summary>
    public sealed class SPSEditorPartsTools
    {
        private readonly HtmlTextWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSEditorPartsTools"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public SPSEditorPartsTools(HtmlTextWriter writer)
        {
            _writer = writer;
        }

        #region Cosmetic

        /// <summary>
        /// Sections the begin tag.
        /// </summary>
        /// <example>
        /// PSEditorPartsTools partsTools = new SPSEditorPartsTools(writer);
        ///
        /// partsTools.SectionBeginTag();
        ///
        /// partsTools.SectionHeaderTag("Configuration:");
        ///
        /// partsTools.CreateTextBoxAndBuilder(txtConfig, writer);
        /// partsTools.SectionFooterTag();
        ///
        /// partsTools.SectionHeaderTag();
        /// chkDevErrors.RenderControl(writer);
        /// partsTools.SectionFooterTag();
        ///
        /// partsTools.SectionEndTag();
        /// </example>
        public void SectionBeginTag()
        {
            _writer.Write("<table width=100%>");
        }

        /// <summary>
        /// Sections the end tag.
        /// </summary>
        public void SectionEndTag()
        {
            _writer.Write("</table>");
        }

        /// <summary>
        /// Sections the next line tag.
        /// </summary>
        public void SectionNextLineTag()
        {
            _writer.Write("<br/>");
        }

        /// <summary>
        /// Sections the header tag.
        /// </summary>
        /// <param name="head">The head.</param>
        public void SectionHeaderTag(string head)
        {
            _writer.Write("<tr><td><div class=UserSectionHead>{0}</div>", head);
        }

        /// <summary>
        /// Sections the header tag.
        /// </summary>
        public void SectionHeaderTag()
        {
            _writer.Write("<tr><td><div class=UserSectionBody>");
        }



        /// <summary>
        /// Sections the footer tag.
        /// </summary>
        public void SectionFooterTag()
        {
            _writer.Write("</div><div class=UserDottedLine></div></td></tr>");
        }

        /// <summary>
        /// Sections the footer tag.
        /// </summary>
        public void SectionFooterWithoutLineTag()
        {
            _writer.Write("</div><div class=UserDottedLine></div></td></tr>");
        }

        /// <summary>
        /// Decorates the controls.
        /// </summary>
        /// <param name="controls">The controls.</param>
        public void DecorateControls(ControlCollection controls)
        {
            foreach (WebControl control in controls)
            {
                control.CssClass = "UserSectionGroup";
                control.Width = new Unit("95%");
            }
        }

        /// <summary>
        /// Creates the text box with zoom builder.
        /// </summary>
        /// <param name="editor">The editor.</param>
        public void CreateTextBoxAndBuilder(TextBox editor)
        {
            _writer.RenderBeginTag(HtmlTextWriterTag.Nobr);
            editor.CssClass = "UserInput";
            editor.Width = new Unit("176px");
            editor.RenderControl(_writer);
            ButtonTagTextBuilder(editor);
            _writer.RenderEndTag();
        }

        /// <summary>
        /// Creates the text box with zoom builder for XML based texts
        /// </summary>
        /// <param name="editor">The editor.</param>
        public void CreateTextBoxAndBuilderXml(TextBox editor)
        {
            _writer.RenderBeginTag(HtmlTextWriterTag.Nobr);
            editor.CssClass = "UserInput";
            editor.TextMode = TextBoxMode.MultiLine;
            editor.Rows = 1;
            editor.Width = new Unit("176px");
            editor.Text = FormatXml(editor.Text);
            editor.RenderControl(_writer);
            ButtonTagTextBuilder(editor);
            _writer.RenderEndTag();
        }

        /// <summary>
        /// Tag to create the button for ZoomBuilder.
        /// </summary>
        /// <param name="editor">The editor.</param>
        private void ButtonTagTextBuilder(Control editor)
        {
            _writer.WriteLine("&nbsp;<input type=\"button\" value=\"...\" id=\"" +
                              editor.ClientID +
                              "Builder\"  title=\"" +
                              "\" class=\"ms-PropGridBuilderButton\" " +
                              "style=\"display:inline\" onclick=\"javascript:MSOPGrid_doBuilder('" +
                              ZoomBuilderRelativeUrl() + "'," + editor.ClientID +
                              ",'dialogHeight:500px;dialogWidth:600px;help:no;status:no;resizable:yes')" +
                              ";\" />");
        }

        /// <summary>
        /// Relative url for zoom builder
        /// </summary>
        /// <returns>The relative Url</returns>
        private static string ZoomBuilderRelativeUrl()
        {
            HttpContext current = HttpContext.Current;
            SPWeb web = SPControl.GetContextWeb(current);
            string relative = web.ServerRelativeUrl;
            if (relative == "/")
            {
                relative = "/_layouts/zoombldr.aspx";
            }
            else
            {
                relative += "/_layouts/zoombldr.aspx";
            }

            return SPEncode.UrlEncodeAsUrl(relative);
        }

        /// <summary>
        /// Indents the XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public string FormatXml(string xml)
        {
            return IndentXml(xml);
        }

        /// <summary>
        /// Indents the XML.
        /// </summary>
        /// <param name="xmlText">The XML text.</param>
        /// <returns></returns>
        private static string IndentXml(string xmlText)
        {
            StringBuilder stringBuilder;            
            try
            {
                if (!string.IsNullOrEmpty(xmlText))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlText);
                    stringBuilder = new StringBuilder();
                    XmlTextWriter xtw = new XmlTextWriter(new StringWriter(stringBuilder))
                                        {
                                                Formatting = Formatting.Indented
                                        };
                    xmlDocument.WriteTo(xtw);
                    xtw.Close();
                    return stringBuilder.ToString();
                }
            }
            catch (XmlException ex)
            {
                SPSDebug.DumpException(ex);
            }

            return xmlText;
        }

        #endregion

        #region DropDown

        /// <summary>
        /// Used by EditorParts to fill in a dropdown control with all site lists
        /// </summary>
        /// <param name="dropDownList">Control to fill</param>
        public static void FillLists(ListControl dropDownList)
        {
            SPWeb web = SPContext.Current.Web;

            foreach (SPList list in web.Lists)
            {
                if (!list.Hidden)
                {
                    dropDownList.Items.Add(new ListItem(list.Title, list.ID.ToString()));
                }
            }
        }


        /// <summary>
        /// Used by EditorParts to fill in a dropdown control with all site lists
        /// </summary>
        /// <param name="dropDownList">Control to fill</param>
        /// <param name="baseType">Type of the base.</param>
        public static void FillLists(ListControl dropDownList, SPBaseType baseType)
        {
            SPWeb web = SPContext.Current.Web;

            foreach (SPList list in web.Lists)
            {
                if (!list.Hidden && list.BaseType == baseType)
                {
                    dropDownList.Items.Add(new ListItem(list.Title, list.ID.ToString()));
                }
            }
        }


        /// <summary>
        /// Used by EditorParts to fill in a dropdown control with all site lists
        /// excluded all of baseType
        /// </summary>
        /// <param name="dropDownList">Control to fill</param>
        /// <param name="baseType">Type of the base.</param>
        public static void FillListsExclude(ListControl dropDownList, SPBaseType baseType)
        {
            SPWeb web = SPContext.Current.Web;

            foreach (SPList list in web.Lists)
            {
                if (!list.Hidden && list.BaseType != baseType)
                {
                    dropDownList.Items.Add(new ListItem(list.Title, list.ID.ToString()));
                }
            }
        }

        /// <summary>
        /// Fills the list views.
        /// </summary>
        /// <param name="dropDownList">The drop down list.</param>
        /// <param name="listGuid">The list GUID.</param>
        public static void FillListViews(ListControl dropDownList, string listGuid)
        {
            SPWeb web = SPContext.Current.Web;

            if (!string.IsNullOrEmpty(listGuid))
            {
                try
                {
                    SPList list = web.Lists[new Guid(listGuid)];

                    dropDownList.Items.Clear();

                    foreach (SPView view in list.Views)
                    {
                        if (!view.Hidden)
                        {
                            dropDownList.Items.Add(new ListItem(view.Title, view.ID.ToString()));
                        }
                    }
                }
                catch(SPException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }


        /// <summary>
        /// Used by EditorParts to fill a ListBox or a DropDownList whith the names of
        /// ListViewWebParts currently loaded in the same page
        /// </summary>
        /// <param name="context">HttpContext, we need the request url</param>
        /// <param name="listControl">Control to fill</param>
        public static void FillWebParts(HttpContext context, ListControl listControl)
        {
            try
            {                
                SPWeb curWeb = SPContext.Current.Web;

                using (SPLimitedWebPartManager webpartManager =
                      curWeb.GetLimitedWebPartManager(context.Request.Url.ToString(),
                                                      PersonalizationScope.Shared))
                {
                    foreach (WebPart webpart in webpartManager.WebParts)
                    {
                        ListViewWebPart listViewWebPart = webpart as ListViewWebPart;

                        // if the list is a ListView WebPart
                        if (listViewWebPart != null)
                        {
                            listControl.Items.Add(new ListItem(webpart.Title, listViewWebPart.ViewGuid));
                        }
                        webpart.Dispose();
                    }
                    webpartManager.Web.Dispose();
                }
            }
            catch (Exception)
            {
                listControl.Items.Add(new ListItem(SPSLocalization.GetResourceString("SPSFW_UnavailableListItem"), string.Empty));
            }
        }

        /// <summary>
        /// Used by EditorParts to fill a ListBox or a DropDownList whith the names of
        /// ListViewWebParts currently loaded in the same page and are using a determinated list 
        /// </summary>
        /// <param name="context">HttpContext, we need the request url</param>
        /// <param name="listID">Sharepoint list ID related to the view</param>
        /// <param name="listControl">Control to fill</param>
        public static void FillWebPartsForList(HttpContext context, string listID, ListControl listControl)
        {
            try
            {
              
                listControl.Items.Clear();

                SPWeb curWeb = SPContext.Current.Web;

                using (SPLimitedWebPartManager webpartManager =
                      curWeb.GetLimitedWebPartManager(context.Request.Url.ToString(),
                                                      PersonalizationScope.Shared))
                {
                    listID = new Guid(listID).ToString("B").ToUpper();

                    foreach (WebPart webpart in webpartManager.WebParts)
                    {
                        ListViewWebPart listViewWebPart = webpart as ListViewWebPart;

                        // if the list is a ListView WebPart
                        if (listViewWebPart != null)
                        {
                            // if the view reference the list
                            if (listViewWebPart.ListName == listID)
                            {
                                listControl.Items.Add(new ListItem(webpart.Title, listViewWebPart.ViewGuid));
                            }
                        }
                        webpart.Dispose();
                    }
                    webpartManager.Web.Dispose();
                }
            }
            catch (Exception)
            {
                listControl.Items.Add(new ListItem(SPSLocalization.GetResourceString("SPSFW_UnavailableListItem"), string.Empty));
            }
        }



        /// <summary>
        /// Get the ListGuid from a ListViewWebPart that contains a specified ViewGuid
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="viewGuid">View Guid</param>
        /// <returns>The Guid of the List, null if not found</returns>
        public static string GetListGuidFromListViewGuid(HttpContext context, string viewGuid)
        {
            SPWeb curWeb = SPContext.Current.Web;

            using (SPLimitedWebPartManager webpartManager =
                curWeb.GetLimitedWebPartManager(context.Request.Url.ToString(),
                                                PersonalizationScope.Shared))
            {
                foreach (WebPart webpart in webpartManager.WebParts)
                {
                    ListViewWebPart listViewWebPart = webpart as ListViewWebPart;

                    // if the list is a ListView WebPart
                    if (listViewWebPart != null)
                    {
                        if (listViewWebPart.ViewGuid == viewGuid)
                        {
                            return listViewWebPart.ListName;
                        }
                    }
                    webpart.Dispose();
                }
                webpartManager.Web.Dispose();
            }

            return null;
        }
        #endregion       
    }
}