<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" MasterPageFile="~/_layouts/application.master" %>
<%@ Register TagPrefix="sptb" TagName="ToolBar" src="~/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="sptb" TagName="ToolBarButton" src="~/_controltemplates/ToolBarButton.ascx" %>
<%@ Register TagPrefix="spwc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="spap" Namespace="Microsoft.SharePoint.ApplicationPages" Assembly="Microsoft.SharePoint.ApplicationPages, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace= "Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace= "System.Data" %>
<%@ Import Namespace= "System.IO" %>
<%@ Import Namespace= "System.Xml" %>
<%@ Import Namespace= "System.Reflection" %>
<%@ Import Namespace= "System.Reflection.Emit" %>
<%@ Import Namespace= "System.Xml.Serialization" %>
<%@ Import Namespace= "System.Xml.Serialization" %>
<%@ Import Namespace= "System.Xml.Serialization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:EncodedLiteral ID="PageTitle" Text="Deploy Item" EncodeMethod="HtmlEncode" runat="server"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <SharePoint:EncodedLiteral ID="TitleArea" Text="Deploy Item" EncodeMethod="HtmlEncode" runat="server"/>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
Use this page to deploy content to subsites, select the sites you wish to deploy the item too and click Deploy to Selected Sites button.
    <sptb:ToolBar id="onetidNavNodesTB" runat="server">
        <Template_Buttons>
            <sptb:ToolBarButton runat="server" id="ltReturn" ToolTip="Return to list" ImageUrl="/_layouts/images/ewr218m.gif" AccessKey="R"/>
            <sptb:ToolBarButton runat="server" Text="Deploy Item" id="idDeployItem" ToolTip="Deploy Item" OnClick="DeployItemEH" ImageUrl="/_layouts/images/ewr217m.gif" AccessKey="I"/>
        </Template_Buttons>
    </sptb:ToolBar>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table border="0" cellpadding="10" cellspacing="10">
        <tr>
            <td>
            <h2 class="ms-standardheader"><SharePoint:EncodedLiteral ID="HeaderText" runat="server" text="Sites" EncodeMethod='HtmlEncode'/> &nbsp;</h2>
            <br />
                <spwc:SPGridView ID="SiteGrid" DataKeyNames="Title" AutoGenerateColumns="false" CssClass="ms-vb2" GridLines="None" HeaderStyle-HorizontalAlign="Left" runat="server" AllowPaging="true" PagerStyle-CssClass="ms-bottompaging" PagerStyle-HorizontalAlign="Center" PagerSettings-Mode="NextPreviousFirstLast" OnSorting="SiteGrid_Sorting" OnDataBound="setCheckStatus">
                    <Columns>
                         <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" Checked="true"/>
                            </ItemTemplate>
                        </asp:TemplateField>                                           
                        <spwc:SPBoundField DataField="Title" HeaderText="Title" SortExpression="Title"/>
                        <spwc:SPBoundField DataField="Description" HeaderText="Description" SortExpression="Description"/>
                        <spwc:SPBoundField DataField="ServerRelativeUrl" HeaderText="URL" Visible="false"/>
                        <spwc:SPBoundField DataField="ID" HeaderText="GUID"/> 
                        <spwc:SPBoundField DataField="ParentWeb" HeaderText="Parent Web" SortExpression="ParentWeb"/>
                        <spwc:SPBoundField DataField="Notes" HeaderText="Item Exists"  SortExpression="Notes" ItemStyle-ForeColor="#0000ff" />
                        <spwc:SPBoundField DataField="Warning" HeaderText="Warning" SortExpression="Warning" ItemStyle-ForeColor="#ff0000" />
                        <asp:TemplateField HeaderText="URL">
                            <ItemTemplate>
                                <asp:HyperLink ID="webListHl" runat="server" NavigateUrl='<%# Eval("ServerRelativeUrl") %>' Text="Link" Target="_blank"/>
                            </ItemTemplate>
                        </asp:TemplateField> 
                    </Columns>
                    <HeaderStyle HorizontalAlign="Left" />
                    <AlternatingRowStyle CssClass="ms-alternating" />
                    <PagerSettings Mode="NumericFirstLast" />
                    <SelectedRowStyle CssClass="ms-selectednav" Font-Bold="True" />
                    <PagerStyle CssClass="ms-bottompaging" />                    
                </spwc:SPGridView>
                <spwc:SPGridViewPager ID="SPGridViewPager1" GridViewId="SiteGrid" runat="server" OnClickNext="SiteGrid_ClickNext" OnClickPrevious="SiteGrid_ClickPrevious" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblMessage" class="ms-info" ForeColor="Red" Visible="true" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<script runat="server"> 
/*
    Copyright (C) 2008  Ray Proffitt

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
	
	quick and nasty datagrid...	feel free to send any changes to rayone@gmail.com
*/
   
    SPSite oSite;
    SPWeb oWeb;
    DataTable oWebsDT = new DataTable();
    DataView oWebsDV;    
    Guid gList = new Guid(HttpContext.Current.Request.QueryString["ListId"]);
    SPList oList;
    SPListItem oItem;
    string sSort = string.Empty;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        oSite = SPContext.Current.Site;
        oWeb = oSite.OpenWeb();
        oList = oWeb.Lists[gList];
        oItem = oList.GetItemById(Convert.ToInt32(HttpContext.Current.Request.QueryString["ItemId"])); 
        if (!Page.IsPostBack)
        {
            FillGrid();
        }
    }

    protected void setCheckStatus(Object sender, EventArgs e)
    {
        foreach (SPGridViewRow oGVR in SiteGrid.Rows)
        {
            try
            {
                System.Web.UI.WebControls.Label olGridNotes = (System.Web.UI.WebControls.Label)oGVR.Controls[7].Controls[0];
                if (olGridNotes.Text.Length > 1)
                {
                    CheckBox chkSelect = (CheckBox)oGVR.FindControl("chkSelect");
                    chkSelect.Checked = false;
                    chkSelect.Enabled = false;
                }
            }
            catch { }           
        }
    }    
        
    protected void DeployItemEH(Object sender, EventArgs e)
    {
        foreach (SPGridViewRow oGVR in SiteGrid.Rows)
        {
            CheckBox chkSelect = (CheckBox)oGVR.FindControl("chkSelect");
            if (chkSelect.Checked)
            {
                string sWebTitle = ((DataKey)SiteGrid.DataKeys[oGVR.RowIndex]).Value.ToString();
                System.Web.UI.WebControls.Label olGridWebGuid = (System.Web.UI.WebControls.Label)oGVR.Controls[4].Controls[0];
                System.Web.UI.WebControls.Label olGridItemExists = (System.Web.UI.WebControls.Label)oGVR.Controls[6].Controls[0];
                System.Web.UI.WebControls.Label olGridNotes = (System.Web.UI.WebControls.Label)oGVR.Controls[7].Controls[0];
                try
                {
                    Guid gWDepl = new Guid(olGridWebGuid.Text);
                    SPWeb oWDepl = oSite.OpenWeb(gWDepl);
                    SPList oWLDepl = oWDepl.Lists[oList.Title];
                    olGridNotes.Text = CopyItem(oItem, oWDepl, oWLDepl);
                    if (olGridNotes.Text.IndexOf("successful") > -1)
                    {
                        olGridItemExists.Text = "Yes";
                    }
                    else
                    {
                       olGridItemExists.Text = "Possibly";   
                    }
                    oWDepl.Dispose();
                }
                catch (Exception ex)
                {
                    olGridNotes.Text = "Error: " + ex.Message;
                }
            }
        }
    }
    
    protected void SiteGrid_ClickNext(Object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void SiteGrid_ClickPrevious(Object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void FillGrid()
    {
        try
        {
            ltReturn.Text = "Return to "+ oList.Title;
            ltReturn.NavigateUrl = oList.DefaultViewUrl;
            HeaderText.Text = string.Format("You are going to deploy {0} from {1} to the select web sites below", oItem.Title, oList.Title);
            SPWebCollection oWebs = oSite.AllWebs;        
            DataColumn oWebsDC;
            DataRow oWebsDR;
            string[] sCols = {"ID","Title","Description","ServerRelativeUrl","ParentWeb","Notes","Warning"};
            foreach(string sC in sCols)
            {
                oWebsDC = new DataColumn();
                oWebsDC.DataType = System.Type.GetType("System.String");
                oWebsDC.ColumnName = sC;
                oWebsDT.Columns.Add(oWebsDC);
            }
            foreach (SPWeb oW in oWebs)
            {
                if (oW.ID != oWeb.ID)
                {
                    oWebsDR = oWebsDT.NewRow();
                    oWebsDR["ID"] = oW.ID;
                    oWebsDR["Title"] = oW.Title;
                    oWebsDR["Description"] = oW.Description;
                    oWebsDR["ParentWeb"] = oW.ParentWeb;
                    try
                    {
                        SPList oWL = oW.Lists[oList.Title];
                        oWebsDR["ServerRelativeUrl"] = oWL.DefaultViewUrl;
                        try
                        {
                            SPListItem oWLI = oWL.GetItemById(oItem.ID);
                            oWebsDR["Notes"] = string.Format("An item:{0}, '{1}' - exists and will be updated!", oItem.ID.ToString(), oWLI.Title);
                        }
                        catch
                        {
                            oWebsDR["Notes"] = "No";
                        }
                    }
                    catch
                    {
                        oWebsDR["ServerRelativeUrl"] = oW.ServerRelativeUrl;
                        oWebsDR["Warning"] = "List Does Not Exists";
                    }
                    oWebsDT.Rows.Add(oWebsDR);
                    oW.Dispose();
                }
            }

            oWebsDV = new DataView(oWebsDT);
            oWebsDV.Sort = sSort;
            SiteGrid.DataSource = oWebsDV;
            SiteGrid.AllowSorting = true;
            SiteGrid.ShowFooter = true;
            SiteGrid.Sorting +=new GridViewSortEventHandler(SiteGrid_Sorting);
            SiteGrid.PageSize = 20;
            SiteGrid.DataBind();
        }
        catch (Exception ex)
        {
            lblMessage.Text += ex.StackTrace + " " + ex.Message;
        }
    }
    void SiteGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        //throw new NotImplementedException();        
        string lastExpression = "";
        if (ViewState["SortExpression"] != null)
        {
            lastExpression = ViewState["SortExpression"].ToString();
        }
        string lastDirection = "asc";
        if (ViewState["SortDirection"] != null)
        {
            lastDirection = ViewState["SortDirection"].ToString();
        }

        string newDirection = "asc";
        if (e.SortExpression == lastExpression)
        {
            newDirection = (lastDirection == "asc") ? "desc" : "asc";
        }
        ViewState["SortExpression"] = e.SortExpression;
        ViewState["SortDirection"] = newDirection;
        sSort = e.SortExpression + " " + newDirection;
        FillGrid();
    }

    public static string CopyItem(SPListItem oItem, SPWeb oWDepl, SPList oWLDepl)
    {
        string sRtn = String.Empty;
        try
        {
            oWDepl.AllowUnsafeUpdates = true;
            SPListItem oItemDepl;
            try
            {
                oItemDepl = oWLDepl.GetItemById(oItem.ID);
                sRtn += "Updating item - ";
            }
            catch
            {
                oItemDepl = oWLDepl.Items.Add();
                sRtn += "Inserting item - ";
            }

            foreach (SPField oFld in oItem.Fields)
            {
                if (oFld.InternalName != SPFieldType.Attachments.ToString() && !oFld.ReadOnlyField)
                {
                    try
                    {
                        if (!oWLDepl.Fields.ContainsField(oFld.InternalName))
                        {
                            try
                            {
                                oWLDepl.Fields.Add(oFld);
                                oWLDepl.Update();
                                sRtn += string.Format(" '{0}' added to destination list;", oFld.InternalName);
                            }
                            catch 
                            {
                                sRtn += string.Format(" '{0}' doesn't exists in destination list and couldn't create.", oFld.InternalName);
                            }
                            
                        }
                        oItemDepl[oFld.InternalName] = oItem[oFld.InternalName]; 
                    }
                    catch (Exception ex1)
                    {
                        sRtn += string.Format(" '{0}' failed;", oFld.InternalName);
                        //sRtn += " failed with: "+ ex1.Message;
                    }
                }
            }
            //attachments
            foreach (string sFileName in oItem.Attachments)
            {
                try
                {
                    SPFile oIDFile = oItem.ParentList.ParentWeb.GetFile(oItem.Attachments.UrlPrefix + sFileName);
                    byte[] imageData = oIDFile.OpenBinary();
                    oItemDepl.Attachments.Add(sFileName, imageData);
                }
                catch (Exception ex1)
                {
                    sRtn += " and attachments failed with: " + ex1.Message;
                }
            }
            oItemDepl.Update();
            oWDepl.AllowUnsafeUpdates = false;
            if (sRtn.IndexOf("failed")==-1)
            {
                sRtn += " successful";
            }
        }
        catch (Exception ex)
        {
            sRtn += ex.Message;
        }
        return sRtn;
    }
</script>

