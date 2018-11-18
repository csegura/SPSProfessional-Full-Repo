using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.WebParts.MailToList.Resources;

namespace SPSProfessional.SharePoint.WebParts.MailToList
{
    public class SPSMailToListEditorPart : EditorPart
    {

        private TextBox txtLogin;
        private TextBox txtPassword;
        private TextBox txtMailServer;
        private TextBox txtMailPort;
        private CheckBox chkUseSSL;
        private DropDownList ddlLists;
        private CheckBox chkManual;

        private SPSEditorPartsTools tools;

        public SPSMailToListEditorPart()
        {
            ID = "SPSMailToListEditorPart";
            Title = "SPSMailToList";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            SPSMailToList webpart = WebPartToEdit as SPSMailToList;

            if (webpart != null)
            {
                webpart.Login = txtLogin.Text;

                if (!string.IsNullOrEmpty(txtPassword.Text))
                    webpart.Password = txtPassword.Text;
                
                webpart.MailServer = txtMailServer.Text;
                webpart.MailPort = txtMailPort.Text;
                webpart.ListGuid = ddlLists.SelectedValue;
                webpart.UseSsl = chkUseSSL.Checked;
                webpart.ManualConnection = chkManual.Checked;
                webpart.ClearControlState();
                webpart.ClearCache();

                return true;
            }
            return false;
        }

        
        public override void SyncChanges()
        {
            EnsureChildControls();
            SPSMailToList webpart = WebPartToEdit as SPSMailToList;

            if (webpart != null)
            {
                Debug.WriteLine("*ListGuid:" + ddlLists.SelectedValue);

                if (!string.IsNullOrEmpty(webpart.ListGuid))
                {
                    txtLogin.Text = webpart.Login;
                    txtPassword.Attributes.Add("value", webpart.Password);
                    txtMailServer.Text = webpart.MailServer;
                    txtMailPort.Text = webpart.MailPort;
                    ddlLists.SelectedValue = webpart.ListGuid;
                    chkUseSSL.Checked = webpart.UseSsl;
                    chkManual.Checked = webpart.ManualConnection;
                }                               
            }
        }

        protected override void CreateChildControls()
        {
            ddlLists = new DropDownList
                       {
                               Width = new Unit("100%")
                       };

            FillLists(ddlLists);

            Controls.Add(ddlLists);

            txtLogin = new TextBox
                       {
                               ID = "t1",
                               Text = string.Empty
                       };
            Controls.Add(txtLogin);

            txtPassword = new TextBox
                          {
                                  ID = "t2",
                                  TextMode = TextBoxMode.Password,
                                  Text = string.Empty
                          };
            Controls.Add(txtPassword);

            txtMailServer = new TextBox
                            {
                                    ID = "t3",
                                    Text = string.Empty
                            };
            Controls.Add(txtMailServer);

            txtMailPort = new TextBox
                          {
                                  ID = "t4",
                                  Text = string.Empty
                          };
            Controls.Add(txtMailPort);

            chkUseSSL = new CheckBox
                        {
                                Text = SPSResources.GetResourceString("SPS_SSL"),
                                ID = "c1"
                        };
            Controls.Add(chkUseSSL);

            chkManual = new CheckBox
                        {
                                Text = SPSResources.GetResourceString("SPS_ManualGet"),
                                ID = "c2"
                        };
            Controls.Add(chkManual);

        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            tools = new SPSEditorPartsTools(writer);

            tools.DecorateControls(Controls);
            tools.SectionBeginTag();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            tools.SectionHeaderTag(SPSResources.GetResourceString("SPS_Login"));
            txtLogin.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPS_Password"));
            txtPassword.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPS_MailServer"));
            txtMailServer.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPS_MailPort"));
            txtMailPort.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkUseSSL.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPS_List"));
            ddlLists.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkManual.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            tools.SectionFooterTag();
            tools.SectionEndTag();
        }

        /// <summary>
        /// Used by EditorParts to fill in a dropdown control with all site lists
        /// </summary>
        /// <param name="dropDownList">Control to fill</param>
        public void FillLists(ListControl dropDownList)
        {
            SPWeb web = SPContext.Current.Web;

            SPContentTypeId contentTypeId = new SPContentTypeId("0x010004A0644CE4CBDF42B050F3D0A7C611A400283DA0EF7F48A642BD1BE0CFDCF1CCE4");
            
            foreach (SPList list in web.Lists)
            {
                if (!list.Hidden && CheckListContentType(list, contentTypeId))
                {
                    dropDownList.Items.Add(new ListItem(list.Title, list.ID.ToString()));
                }
            }
        }

        private bool CheckListContentType(SPList list, SPContentTypeId contentTypeId)
        {
            foreach (SPContentType contentType in list.ContentTypes)
            {
                if (contentType.Id.IsChildOf(contentTypeId))
                {
                    return true;
                }
            }
            return false;
        }
    }
}