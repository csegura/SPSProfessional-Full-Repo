using System;
using System.Reflection;
using System.Security;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.WebParts.SPSExplorer.Controls
{
    public class SPSListView : WebControl, INamingContainer
    {
        // Fields
        private SPList _list;
        private SPView _view;
        private string _viewHtml = string.Empty;

        private string _rootFolder;
        private ListViewWebPart _listViewWebPart;
        private char[] _viewHtml2;

        #region Public Properties

        public string RootFolder
        {
            set { _rootFolder = value; }
        }

        public SPList List
        {
            get
            {
                if (_list == null)
                {
                    SPControl.GetContextSite(Context);
                    SPWeb contextWeb = SPControl.GetContextWeb(Context);
                    if (!(ListId == "UserInfo"))
                    {
                        _list = contextWeb.Lists.GetList(new Guid(ListId), true);
                    }
                    else
                    {
                        _list = contextWeb.SiteUserInfoList;
                    }
                }
                return _list;
            }
        }

        public string ListId
        {
            get
            {
                string str = ViewState["ListId"] as string;
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set { ViewState["ListId"] = value; }
        }

        public string ViewId
        {
            get
            {
                string str = ViewState["ViewId"] as string;
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set { ViewState["ViewId"] = value; }
        }

        #endregion

        #region Control overrides

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!string.IsNullOrEmpty(ViewId))
            {
                try
                {
                    Guid guid = new Guid(ViewId);
                    _view = List.Views[guid];
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (FormatException)
                {
                }
            }
            if (_view == null)
            {
                _view = List.DefaultView;
            }
        }


        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            _listViewWebPart = new ListViewWebPart();
            _listViewWebPart.ListName = _list.ID.ToString("B").ToUpper();
            // _listViewWebPart.ViewGuid = _view.ID.ToString("B").ToUpper();             
            _listViewWebPart.ListViewXml = _view.HtmlSchemaXml;
            //_viewHtml = _listViewWebPart.GetDesignTimeHtml();
            _listViewWebPart.ChromeType = PartChromeType.None;

            //Find the ToolBar control and set visible to False
            foreach (Control ctrl in _listViewWebPart.Controls)
            {
                if (ctrl.GetType() == typeof(ViewToolBar))
                {
                    ctrl.Visible = false;
                    break;
                }
            }

            //DisableToolbar();
            Controls.Add(_listViewWebPart);
        }


        protected override void OnPreRender(EventArgs e)
        {
            if (Visible)
            {
                if (!string.IsNullOrEmpty(_rootFolder))
                {
                    SetRootFolder();
                }
                //_viewHtml = _view.RenderAsHtml();
                //_viewHtml = _view.RenderAsHtml(true, false, _view.Url);

                if (_listViewWebPart.ViewType == ViewType.Html)
                {
                    _viewHtml2 = (char[])typeof(ListViewWebPart).InvokeMember("RenderView",
                                                                               BindingFlags.NonPublic |
                                                                               BindingFlags.Instance |
                                                                               BindingFlags.InvokeMethod,
                                                                               null,
                                                                               _listViewWebPart,
                                                                               null);
                }
                else
                {
                    //TextWriter wr = new StringWriter();
                    //HtmlTextWriter hwr = new HtmlTextWriter(wr);
                    //typeof(ListViewWebPart).InvokeMember("RenderWebPart",
                    //                                                           BindingFlags.NonPublic |
                    //                                                           BindingFlags.Instance |
                    //                                                           BindingFlags.InvokeMethod,
                    //                                                           null,
                    //                                                           _listViewWebPart,
                    //                                                           new object[] { hwr });


                    _viewHtml2 = SPSResources.GetResourceString("SPSPE_NotSupportedView").ToCharArray();
                }

            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            writer.WriteLine("<!-- ListView -->");
            //_listViewWebPart.RenderControl(writer);

           

            writer.WriteLine(_viewHtml2);
            writer.WriteLine("<!-- EndListView -->");
        }

        #endregion

        // Properties

        private void SetRootFolder()
        {
            SetViewRenderQueryParameters(_list,
                                         Context,
                                         SPAlternateUrl.ContextUri.OriginalString,
                                         SPContext.Current.Web,
                                         _list.ID.ToString("B").ToUpper(),
                                         _view.ID.ToString("B").ToUpper(),
                                         _view,
                                         false,
                                         null,
                                         null,
                                         null,
                                         _rootFolder,
                                         null,
                                         null,
                                         null,
                                         0);
        }

        private void SetViewRenderQueryParameters(SPList list,
                                                  HttpContext context,
                                                  string currentUrl,
                                                  SPWeb web,
                                                  string listName,
                                                  string viewGuid,
                                                  SPView designerView,
                                                  bool gridview,
                                                  string instanceID,
                                                  string selectedID,
                                                  string filterString,
                                                  string rootFolder,
                                                  string folderCtId,
                                                  string paged,
                                                  StringBuilder pagingTokens,
                                                  int lastFilterIndex)
        {
            try
            {
                list.GetType().InvokeMember("SetViewRenderQueryParameters",
                                            BindingFlags.Public
                                            | BindingFlags.NonPublic
                                            | BindingFlags.InvokeMethod
                                            | BindingFlags.Instance,
                                            null,
                                            list,
                                            new object[]
                                            {
                                                    context, currentUrl, web, listName, viewGuid,
                                                    designerView, gridview, instanceID, selectedID, filterString,
                                                    rootFolder, folderCtId, paged, pagingTokens, lastFilterIndex
                                            });
            }
            catch (SecurityException ex)
            {
                throw new SPSException(ex.TargetSite.Name, ex);
            }
        }

        private void DisableToolbar2()
        {
            //SPList list = web.Lists[new Guid(listWebPart.ListName)];
            //SPView webpartView = list.Views[new Guid(listWebPart.ViewGuid)];

            _view.GetType().InvokeMember("EnsureFullBlownXmlDocument",
                                         BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                         null,
                                         _view,
                                         null);

            PropertyInfo nodeProp = _view.GetType().GetProperty("Node",
                                                                BindingFlags.NonPublic | BindingFlags.Instance);
            XmlNode node = nodeProp.GetValue(_view, null) as XmlNode;

            if (node != null)
            {
                XmlNode toolbarNode = node.SelectSingleNode("Toolbar");
                if (toolbarNode != null)
                {
                    toolbarNode.Attributes["Type"].Value = "None";
                    _view.Update();
                }
            }
        }

        private void DisableToolbar()
        {
            ////  Extract view
            //PropertyInfo ViewProp = lv.GetType().GetProperty("View",
            //                                                 BindingFlags.NonPublic | BindingFlags.Instance);


            //SPView _view = ViewProp.GetValue(lv, null) as SPView;


            //if (_view != null)
            //{
            //    string txt = _view.SchemaXml;
            //}

            if (_view != null)
            {
                PropertyInfo NodeProp = _view.GetType().GetProperty("Node",
                                                                    BindingFlags.NonPublic | BindingFlags.Instance);


                XmlNode node = NodeProp.GetValue(_view, null) as XmlNode;

                if (node != null)
                {
                    XmlNode tBarNode = node.SelectSingleNode("Toolbar");


                    if (tBarNode != null)
                    {
                        XmlAttribute typeNode = tBarNode.Attributes["Type"];

                        // make the contents empty so we realy remove the toolbar .....

                        // otherwise you might get a different type of toolbar popup when we have a
                        // Migrated site from 2.0

                        tBarNode.RemoveAll();

                        // re-add the type attribute

                        tBarNode.Attributes.Append(typeNode);

                        // finally set the toolbar to not show....

                        typeNode.Value = "None";
                    }
                }
            }

            //This forces a refresh of the views internal xml or the node's cild nodes are not populated

            if (_view != null)
            {
                _list.ParentWeb.AllowUnsafeUpdates = true;
                _view.Update();
                _list.ParentWeb.AllowUnsafeUpdates = false;
            }
        }
    }
}