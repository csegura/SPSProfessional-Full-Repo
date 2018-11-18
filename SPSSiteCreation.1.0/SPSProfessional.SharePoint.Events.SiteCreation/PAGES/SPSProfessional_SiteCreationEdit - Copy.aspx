<%@ Assembly Name="SPSProfessional.SharePoint.Events.SiteCreation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4031063ddba1c7c7"%> 
<%@ Page Language="C#"  
         Inherits="SPSProfessional.SharePoint.Events.SiteCreation.SiteCreationEditPage"                   
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

<asp:Content contentplaceholderid="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral ID="EncodedLiteral1" runat="server" text="SPSProfessional SiteCreation" EncodeMethod='HtmlEncode'/>
</asp:content>

<asp:Content ID="Content1" contentplaceholderid="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePoint:EncodedLiteral ID="EncodedLiteral2" runat="server" text="SPSProfessional SiteCreation" EncodeMethod='HtmlEncode'/>
</asp:Content>

<asp:content ID="Content2" contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
</asp:content>

<asp:content ID="Content3" contentplaceholderid="PlaceHolderPageDescription" runat="server">
    <SharePoint:EncodedLiteral ID="encDescription" runat="server" text="" EncodeMethod='HtmlEncode'/>
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
                
                    <wssuc:InputFormSection runat="server" id="iform1" 
                        Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,Fields%>" 
                        Description="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,FieldsDescription%>">
                        <template_inputformcontrols>                        
                             <wssuc:InputFormControl LabelText="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,SiteNameField%>" runat="server">
				                <Template_Control>
				                    <asp:DropDownList ID="ddlSiteTitleField"  				               
				                    Runat="server"/> 
				                </Template_Control>
				                <Template_ExampleText>
                                    <asp:Label ID="lblSiteTitleField" runat="server" forecolor="red" text="" />
                                </Template_ExampleText>
			                </wssuc:InputFormControl>
			                <wssuc:InputFormControl LabelText="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,SiteUrlField%>" runat="server">
			                    <Template_Control>
			                        <asp:DropDownList ID="ddlSiteUrlField"  
				                    Runat="server"/>
				                </Template_Control>
				                <Template_ExampleText>
                                    <asp:Label ID="lblSiteUrlField" runat="server" forecolor="red" text="" />
                                </Template_ExampleText>
			                </wssuc:InputFormControl>	
			                <wssuc:InputFormControl LabelText="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,TemplateField%>" runat="server">
			                    <Template_Control>
			                        <asp:DropDownList ID="ddlTemplateField" 
			                        OnSelectedIndexChanged="ddlTemplateField_SelectedIndexChanged"  
			                        AutoPostBack="True"
				                    Runat="server"/>
				                </Template_Control>
				                <Template_ExampleText>
                                    <asp:Label ID="lblTemplateField" runat="server" forecolor="red" text="" />
                                </Template_ExampleText>
			                </wssuc:InputFormControl>	
                        </template_inputformcontrols>
                    </wssuc:InputFormSection>
                    
                    <wssuc:InputFormSection id="iform2" runat="server"
                        Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,Templates%>" 
                        Description="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,TemplatesDescription%>">                        
                        <template_inputformcontrols> 
                         <asp:Repeater ID="rptTemplateMap" Runat="server">
			                <HeaderTemplate>			                 		                  
			                </HeaderTemplate>
			                <ItemTemplate>
    		                    <span dir=ltr><%# DataBinder.Eval(Container.DataItem, "Key") %></span>
    		                    <br />			                    
			                    <IMG SRC="/_layouts/images/blank.gif" width=11 height=3 alt=""><asp:DropDownList ID="ddlTemplates" runat="server" />
			                    <br />
			                </ItemTemplate>
			                <FooterTemplate>			                 
			                    <br />
			                </FooterTemplate>
		                 </asp:Repeater>
                        </template_inputformcontrols>
                     </wssuc:InputFormSection>
                     
                     <wssuc:InputFormSection runat="server" id="iform3a" 
                        Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,ErrorManagement%>"                         
                        Description="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,ErrorManagementDescription%>" >
                        <template_inputformcontrols> 
                         <wssuc:InputFormControl runat="server" LabelText="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,LogError%>">
                            <Template_Control>
                            <asp:CheckBox id="chkOptLogError" 
                                Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,chkOptLogErrorText%>" runat="server" /> 
                            </Template_Control>
                        </wssuc:InputFormControl>
                         <wssuc:InputFormControl runat="server" LabelText="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,ForceCreation%>">
                            <Template_Control>
                            <asp:CheckBox id="chkOptForceCreation" 
                            Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,chkOptForceCreationText%>" runat="server" />                                                                        
                            </Template_Control>
                        </wssuc:InputFormControl>
                        </template_inputformcontrols>
                     </wssuc:InputFormSection>
                     
                     
                     <wssuc:InputFormSection id="iform3"  runat="server"
                        Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,NewSiteNavigation%>"                         
                        Description="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,NewSiteNavigationDescription%>">
                        <template_inputformcontrols> 
                         <wssuc:InputFormControl runat="server" LabelText="">
                            <Template_Control>
                            <asp:CheckBox id="chkOptOnQuickLaunch" 
                                Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,chkOptOnQuickLaunchText%>" runat="server" /> 
                            </Template_Control>
                        </wssuc:InputFormControl>
                         <wssuc:InputFormControl runat="server" LabelText="">
                            <Template_Control>
                            <asp:CheckBox id="chkOptUseSharedNavBar" 
                            Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,chkOptUseSharedNavBarText%>" runat="server" />                                                                        
                            </Template_Control>
                        </wssuc:InputFormControl>
                        </template_inputformcontrols>
                     </wssuc:InputFormSection>

                     <wssuc:InputFormSection 
                            Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,NewSitePermissions%>" id="iform4" 
                            Description="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,NewSitePermissionsDescription%>" runat="server">                     
                        <template_inputformcontrols> 
                            <wssuc:InputFormControl runat="server" LabelText="">
                                <Template_Control>
                                <asp:CheckBox id="chkOptUniquePermissons" 
                                    Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,chkOptUniquePermissonsText%>" 
                                    runat="server" />
                                </Template_Control>
                            </wssuc:InputFormControl>
                            <wssuc:InputFormControl runat="server" 
                                LabelText="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,Permissions%>">
			                    <Template_Control>				                		
                                    <wssawc:InputFormTextBox 
                                    Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,txtOptNewPermissionsTitle%>" 
                                    class="ms-input"  
                                    ID="txtOptNewPermissions" 
                                    Runat="server" 
                                    TextMode="MultiLine" 
                                    Columns="40" 
                                    Rows="10"/> 
			                    </Template_Control>
                  		    </wssuc:InputFormControl>
                        </template_inputformcontrols>                        
                     </wssuc:InputFormSection>

                    <wssuc:InputFormSection id="iform5"  runat="server"
                        Title="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,DeleteSite%>" 
                        Description="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,DeleteSiteDescription%>">
                        <template_inputformcontrols> 
                        <asp:CheckBox id="chkOptAttachDelete" Text="<%$Resources:SPSProfessional.SharePoint.Events.SiteCreation,chkOptAttachDeleteText%>" runat="server" />                                
                        </template_inputformcontrols>
                     </wssuc:InputFormSection>
                    
                    <wssuc:ButtonSection runat="server">
		                <Template_Buttons>
		                  <asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" 
			              OnClick="BtnDelete_Click" 
			              Text="<%$Resources:wss,multipages_deletebutton_text%>" 
			              id="BtnDelete" /> 
			              <asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" 
			              OnClick="BtnSave_Click" 
			              Text="<%$Resources:wss,multipages_okbutton_text%>" 
			              id="BtnSave" 
			              accesskey="<%$Resources:wss,okbutton_accesskey%>"/>                        
		                </Template_Buttons>
	                </wssuc:ButtonSection>	                
                   
                </table>
            </td>
        </tr>
    </table>
</asp:Content>


