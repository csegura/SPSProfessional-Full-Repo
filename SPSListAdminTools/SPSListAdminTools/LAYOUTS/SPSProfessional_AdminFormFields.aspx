<%@ Assembly Name="SPSProfessional.SharePoint.Admin.ListTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4031063ddba1c7c7" %>

<%@ Page Language="C#" Inherits="SPSProfessional.SharePoint.Admin.ListTools.AdminFormFieldsPage"
    MasterPageFile="~/_layouts/application.master" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    <SharePoint:FormattedStringWithListType ID="FormattedStringWithListType1" runat="server"
        String="List Display Settings:" LowerCase="false" />
    <a id="onetidListHlink" href="<% SPHttpUtility.AddQuote(SPHttpUtility.UrlPathEncode(CurrentList.DefaultViewUrl,true),Response.Output);%>">
        <%SPHttpUtility.HtmlEncode(CurrentList.Title, Response.Output);%></a>
</asp:Content>
<asp:Content ID="ContentMain" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <%= RenderPage() %>
    <div style="float: right; margin-top: 10px;">
        <asp:Button ID="OK" runat="server" OnClientClick="javascript:ComputeFields();" Text="OK"
            OnClick="SaveCustomDisplay" CssClass="ms-ButtonHeightWidth" />
        <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth" />
    </div>
</asp:Content>
