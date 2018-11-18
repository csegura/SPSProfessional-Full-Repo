<%@ Assembly Name="SPSProfessional.SharePoint.Events.SiteCreation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4031063ddba1c7c7"%> 
<%@ Page Language="C#"  
         Inherits="SPSProfessional.SharePoint.Events.SiteCreation.SiteCreationPage"                   
         MasterPageFile="~/_layouts/application.master" %>

<%@ Register TagPrefix="wssuc" TagName="ToolBar" src="~/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="~/_controltemplates/ToolBarButton.ascx" %>

<%@ Register TagPrefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" 
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" 
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" 
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="System.Xml" %>

<asp:Content contentplaceholderid="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral ID="EncodedLiteral1" runat="server" text="SPSProfessional SiteCreation" EncodeMethod='HtmlEncode'/>
</asp:content>

<asp:Content ID="Content1" contentplaceholderid="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePoint:EncodedLiteral ID="EncodedLiteral2" runat="server" text="SPSProfessional SiteCreation" EncodeMethod='HtmlEncode'/>
</asp:Content>

<asp:content ID="Content2" contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
</asp:content>

<asp:content ID="Content3" contentplaceholderid="PlaceHolderPageDescription" runat="server">
<SharePoint:EncodedLiteral ID="siteCreationPageDescription" runat="server" text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,siteCreationPageDescription%>" EncodeMethod='HtmlEncode'/>
</asp:content>

<asp:content ID="Content4" contentplaceholderid="PlaceHolderMain" runat="server">
<table width="100%" class="propertysheet" cellspacing="0" cellpadding="0" border="0"> 
    
	<wssuc:ToolBar id="ToolBar" runat="server" CssClass="ms-toolbar">	
		<Template_Buttons>			
			<wssuc:ToolBarButton runat="server" Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,idAddKeyButtonText%>"
				id="idAddKeyButton"
				ToolTip="<%$ Resources:SPSProfessional.SharePoint.Events.SiteCreation,idAddKeyButtonToolTip%>"
				NavigateUrl="../_layouts/SPSProfessional_SiteCreationAdd.aspx"
				ImageUrl="/_layouts/images/newitem.gif"
				Padding="2px" />				
		</Template_Buttons>		
		<Template_RightButtons>			
		</Template_RightButtons>
	</wssuc:ToolBar>
	
  <TABLE border="0" cellspacing="4" cellpadding="0" width="100%">
	<TR>
	 <TD>
	 
	    <asp:Repeater ID="rptSiteCreationLists" Runat="server">
			<HeaderTemplate>
			 <table width=100% cellpadding=0 cellspacing=0 border=0 id="SiteCreationTable">
			  <tr VALIGN=TOP>
			   <th scope="col" class="ms-vh2" width="20%" nowrap>
				 <SharePoint:EncodedLiteral 
				    ID="listName" runat="server" 
				    text="<%$ Resources:SPSProfessional.SharePoint.Events.SiteCreation,listName%>" 
				    EncodeMethod='HtmlEncode'/>
			   </th>
			   <th scope="col" nowrap class="ms-vh2" width="70%">
				 <SharePoint:EncodedLiteral 
				    ID="listId" runat="server" 
				    text="<%$ Resources:SPSProfessional.SharePoint.Events.SiteCreation,listId%>" 
				    EncodeMethod='HtmlEncode'/>
			   </th>		
			  </tr>
			</HeaderTemplate>
			<ItemTemplate>
			  <tr>
			   <td class="ms-vb2">
				 <a href='SPSProfessional_SiteCreationEdit.aspx?List=<%# DataBinder.Eval(Container.DataItem, "ListID") %>'>
				 <span dir=ltr><%# DataBinder.Eval(Container.DataItem, "ListName") %></span>
				 </a>
			   </td>
			   <td class="ms-vb2">
				 <%# DataBinder.Eval(Container.DataItem, "ListID") %>
			   </td>			  
			  </tr>
			</ItemTemplate>
			<FooterTemplate>
			 </table>
			</FooterTemplate>
		 </asp:Repeater>
		   		          	   
	 </TD>
	</TR>
   </table>
</asp:content>

