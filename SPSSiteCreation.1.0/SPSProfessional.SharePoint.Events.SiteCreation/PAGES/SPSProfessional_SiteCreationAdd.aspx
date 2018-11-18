<%@ Assembly Name="SPSProfessional.SharePoint.Events.SiteCreation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4031063ddba1c7c7"%> 
<%@ Page Language="C#"  
         Inherits="SPSProfessional.SharePoint.Events.SiteCreation.SiteCreationAddPage"                   
         MasterPageFile="~/_layouts/application.master" %>

<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="~/_controltemplates/InputFormControl.ascx" %>

<%@ Register TagPrefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" 
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" 
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" 
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

    
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="SPSProfessional.SharePoint.Events.SiteCreation" %>

<script runat="server">    
   
</script>


<asp:Content contentplaceholderid="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral ID="EncodedLiteral1" runat="server" text="SPSProfessional SiteCreation" EncodeMethod='HtmlEncode'/>
</asp:content>

<asp:Content ID="Content1" contentplaceholderid="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePoint:EncodedLiteral ID="EncodedLiteral2" runat="server" text="SPSProfessional SiteCreation" EncodeMethod='HtmlEncode'/>
</asp:Content>

<asp:content ID="Content2" contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
</asp:content>

<asp:content ID="Content3" contentplaceholderid="PlaceHolderPageDescription" runat="server">
</asp:content>

<asp:Content ID="contentMain" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <style type="text/css">
        table.ms-propertysheet {
            height: 100%;
        }
    </style>
    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; height: 100%" class="ms-settingsframe">

        <tr>
            <td valign="top" style="padding: 4px 0px 4px 0px;" height="100%">
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ms-propertysheet">
                
                    <wssuc:InputFormSection 
                        Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,List%>" 
                        id="iform1" 
                        Description="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,ListDescription%>" runat="server">
                        <template_inputformcontrols>                        
                         <wssuc:InputFormControl 
                         LabelText="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,SelectList%>" 
                         runat="server">
				            <Template_Control>
				                <asp:DropDownList ID="ddlLists"  				               
				                Runat="server"/>                                
				            </Template_Control>
			            </wssuc:InputFormControl>	
                        </template_inputformcontrols>
                    </wssuc:InputFormSection>                   
                   
                    <wssuc:ButtonSection runat="server">
		                <Template_Buttons>		                  
			              <asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" 
			              OnClick="BtnNext_Click" 
			              Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,BtnNextTitle%>" 
			              id="BtnNext" 
			              accesskey="<%$Resources:wss,okbutton_accesskey%>"/>                        
		                </Template_Buttons>
	                </wssuc:ButtonSection>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>


