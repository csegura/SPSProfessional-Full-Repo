
<%@ Assembly Name="SPSProfessional.SharePoint.WebParts.RollUp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4031063ddba1c7c7"%> 
<%@ Page Language="C#" Inherits="SPSProfessional.SharePoint.WebParts.RollUp.ListInfo.SPSProfessional_ListInfo" MasterPageFile="/_layouts/dialog.master" %>

<%@ Import Namespace="Microsoft.SharePoint.WebControls" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Import Namespace="Microsoft.SharePoint" %>

<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>


<asp:Content ID="Content5" contentplaceholderid="PlaceHolderDialogHeaderPageTitle" runat="server">
		<asp:Literal runat="server" ID="_dialogTitle" />
</asp:Content>

<asp:Content ID="Content6" contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
	<SharePoint:FormDigest ID="FormDigest1" runat=server/>
</asp:Content>


<asp:Content ID="Content7" contentplaceholderid="PlaceHolderDialogImage" runat="server">
	<asp:Image ImageUrl="/_layouts/images/VWCNTNT.GIF" ID="_dialogImage" width="32" height="32" runat="server" />
</asp:Content>


<asp:Content ID="Content8" contentplaceholderid="PlaceHolderDialogDescription" runat="server">
	<asp:Literal runat="server" ID="_dialogDescription" />
</asp:Content>


<asp:Content ID="Content9" contentplaceholderid="PlaceHolderHelpLink" runat="server">
</asp:Content>

<asp:Content ID="Content10" contentplaceholderid="PlaceHolderDialogBodyMainSection" runat="server">
    <div class="ms-WPBody">
    <asp:Repeater ID="_rptListsInfo" runat="server" >
    <HeaderTemplate>
        <table cellpadding="3" cellspacing="0">
            <tbody>
            <tr style="background-color:#f2f2f2">
                <th style="background-color:#f2f2f2">Url/Title/Type</th>                
                <th style="background-color:#f2f2f2">Description</th>
            </tr>
    </HeaderTemplate> 
    <AlternatingItemTemplate>
            <tr class="ms-alternating">                     
                <td><%# Eval("Url")%><br />
                <%# Eval("Name")%> <br />
                <%# Eval("ListType")%> </td>
                <td><%# Eval("Description")%> </td>
            </tr>
    </AlternatingItemTemplate>
    <ItemTemplate>
            <tr>                     
                <td><%# Eval("Url")%><br />
                <%# Eval("Name")%> <br />
                <%# Eval("ListType")%> </td>
                <td><%# Eval("Description")%> </td>
            </tr>
    </ItemTemplate>  
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    </asp:Repeater>
    </div>
</asp:Content>
