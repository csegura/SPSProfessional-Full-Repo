// File : SPSToolBarControl.cs
// Date : 30/01/2008
// User : csegura
// Logs
// 01/02/2008 (csegura) - Add AddControl method.
// 02/02/2008 (csegura) - AddButton check for image and change if it's null // TODO

using System;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// This class implements a ToolBarControl
    /// </summary>
    public class SPSToolBarControl : RepeatedControls
    {
        // {0} = Button text
        // {1} = onclick script
        // {2} = Tooltip text
        private const string SPSToolbarButtonHtmlFormat =
            @"
        <DIV class=""ms-toolbarItem ms-selectorlink"">
           <A href=""#"" onclick=""{1}"" title=""{2}"">&nbsp;{0}</A>
        </DIV>";

        // {0} = Button text
        // {1} = onclick script
        // {2} = Tooltip text
        // {3} = Button image markup
        private const string SPSToolbarButtonImageHtmlFormatNav =
//            @"
//        <DIV class=""ms-toolbarItem ms-selectorlink"">
//           <A class=""ms-toolbar"" href=""{1}"" title=""{2}"">
//           <IMG align=""absmiddle"" alt=""{2}"" src=""{3}"" border=""0""></A>
//           &nbsp; 
//           <A class=""ms-toolbar"" href=""{1}"" title=""{2}"">{0}</A>
//        </DIV>";

             @"<TABLE cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR>
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">
		            <A class=""ms-toolbar"" href=""{1}"" title=""{2}"">
			          <IMG border=""0"" width=""16"" height=""16"" src=""{3}"" alt=""{2}""/>
		            </A>
	              </TD>	   
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">		
		             <A class=""ms-toolbar"" href=""{1}"" title=""{2}"">{0}</A>				      
	              </TD>
	            </TR>
	          </TABLE>";


        // {0} = Button text
        // {1} = onclick script
        // {2} = Tooltip text
        // {3} = Button image markup
        private const string SPSToolbarButtonHtmlFormatNav =
//            @"
//        <DIV class=""ms-toolbarItem ms-selectorlink"">
//           <A class=""ms-toolbar"" href=""{1}"" title=""{2}"">{0}</A>
//        </DIV>";
            @"<TABLE cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR>
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">		
		             <A class=""ms-toolbar"" href=""{1}"" title=""{2}"">{0}</A>				      
	              </TD>
	            </TR>
	          </TABLE>";

        // {0} = Button text
        // {1} = onclick script
        // {2} = Tooltip text
        // {3} = Button image markup
        private const string SPSToolbarSeparator =
            @"
        <DIV class=""ms-toolbarItem ms-selectorlink"">
           &nbsp;|&nbsp;
        </DIV>";


        // {0} = Button text
        // {1} = onclick script
        // {2} = Tooltip text
        // {3} = Image URL
        private const string SPSToolBarActionButton =
            @"<TABLE cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR>
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">
		            <A class=""ms-toolbar"" href=""#"" onclick=""{1}"" title=""{2}"">
			          <IMG border=""0"" width=""16"" height=""16"" src=""{3}"" alt=""{2}""/>
		            </A>
	              </TD>	   
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">		
		             <A class=""ms-toolbar"" href=""#"" onclick=""{1}"" title=""{2}"">{0}</A>				      
	              </TD>
	            </TR>
	          </TABLE>";


        // {0} = Button text
        // {1} = onclick script
        // {2} = Tooltip text
        // {3} = Image URL
        // {4} = Href URL
        private const string SPSToolBarActionButton2 =
            @"<TABLE cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR>
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">
		            <A class=""ms-toolbar"" href=""{4}"" onclick=""{1}"" title=""{2}"">
			          <IMG border=""0"" width=""16"" height=""16"" src=""{3}"" alt=""{2}""/>
		            </A>
	              </TD>	   
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">		
		             <A class=""ms-toolbar"" href=""{4}"" onclick=""{1}"" title=""{2}"">{0}</A>				      
	              </TD>
	            </TR>
	          </TABLE>";

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSToolBarControl"/> class.
        /// </summary>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public SPSToolBarControl()
        {
            HeaderHtml =
                @"
                <TABLE class=""ms-toolbar"" cellpadding=""2"" cellspacing=""0"" border=""0"" width=""100%"" >
                   <TR>";

            BeforeControlHtml = @"<TD class=""ms-toolbar"" nowrap=""true"">";

            AfterControlHtml = @"</TD>";

            FooterHtml =
                @"<TD width=""99%"" class=""ms-toolbar"" nowrap>
                      <IMG SRC=""/_layouts/images/blank.gif"" width=1 height=18 alt="""">
                   </TD>
                   </TR>
                   </TABLE>";

            SeparatorHtml = "<TD class=ms-separator>|</TD>";
        }

        /// <summary>
        /// Adds the tool bar action button.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="buttonText">The button text.</param>
        /// <param name="clientOnClick">The client on click.</param>
        /// <param name="tooltipText">The tooltip text.</param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void AddToolBarActionButton(
            string buttonId,
            string buttonText,
            string clientOnClick,
            string tooltipText)
        {
            Literal buttonMarkupLiteral = new Literal();

            buttonMarkupLiteral.Text = String.Format(
                SPSToolbarButtonHtmlFormat,
                SPHttpUtility.HtmlEncode(buttonText),
                SPHttpUtility.HtmlEncode(clientOnClick),
                SPHttpUtility.HtmlEncode(tooltipText));
            buttonMarkupLiteral.ID = buttonId;

            Controls.Add(buttonMarkupLiteral);
        }

        /// <summary>
        /// Adds the tool bar action button.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="buttonText">The button text.</param>
        /// <param name="clientOnClick">The client on click.</param>
        /// <param name="tooltipText">The tooltip text.</param>
        /// <param name="buttonImageSrc">The button image SRC.</param>
        /// <example>
        ///  PostBackOptions postBackOptions = new PostBackOptions(_parent);
        ///  postBackOptions.Argument = argument;
        ///  postBackOptions.AutoPostBack = true;
        ///  postBackOptions.PerformValidation = true;
        ///  onClick = _page.ClientScript.GetPostBackEventReference(postBackOptions);
        ///  // Add Action Button
        ///  _toolBarControl.AddToolBarActionButton(
        ///       _parent.ID + "MiId",
        ///        "Button",
        ///       onClick,
        ///       option.Name,
        ///       option.ImageUrl);
        /// </example>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void AddToolBarActionButton(
            string buttonId,
            string buttonText,
            string clientOnClick,
            string tooltipText,
            string buttonImageSrc)
        {
            Literal buttonMarkupLiteral = new Literal();

            buttonMarkupLiteral.Text = String.Format(
                SPSToolBarActionButton,
                SPHttpUtility.HtmlEncode(buttonText),
                SPHttpUtility.HtmlEncode(clientOnClick),
                SPHttpUtility.HtmlEncode(tooltipText),
                SPHttpUtility.HtmlUrlAttributeEncode(buttonImageSrc));
            buttonMarkupLiteral.ID = buttonId;

            Controls.Add(buttonMarkupLiteral);
            //AddWithSeparator(buttonMarkupLiteral);
        }

        /// <summary>
        /// This is an special button that has an onclick event before redirect to an url
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="buttonText">The button text.</param>
        /// <param name="clientOnClick">The client on click.</param>
        /// <param name="urlPostback">The URL postback.</param>
        /// <param name="tooltipText">The tooltip text.</param>
        /// <param name="buttonImageSrc">The button image SRC.</param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        public void AddToolBarActionButton(
            string buttonId,
            string buttonText,
            string clientOnClick,
            string urlPostback,
            string tooltipText,
            string buttonImageSrc)
        {
            Literal buttonMarkupLiteral = new Literal();

            buttonMarkupLiteral.Text = String.Format(
                SPSToolBarActionButton2,
                SPHttpUtility.HtmlEncode(buttonText),
                SPHttpUtility.HtmlEncode(clientOnClick),
                SPHttpUtility.HtmlEncode(tooltipText),
                SPHttpUtility.HtmlUrlAttributeEncode(buttonImageSrc),
                SPHttpUtility.HtmlEncode(urlPostback));
            buttonMarkupLiteral.ID = buttonId;

            Controls.Add(buttonMarkupLiteral);
            //AddWithSeparator(buttonMarkupLiteral);
        }

        /// <summary>
        /// Adds the tool bar action button.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="buttonText">The button text.</param>
        /// <param name="clientOnClick">The client on click.</param>
        /// <param name="urlPostback">The URL postback.</param>
        /// <param name="tooltipText">The tooltip text.</param>
        /// <param name="buttonImageSrc">The button image SRC.</param>
        public void AddToolBarActionButton(
            string buttonId,
            string buttonText,
            string clientOnClick,
            Uri urlPostback,
            string tooltipText,
            string buttonImageSrc)
        {
            AddToolBarActionButton(
                buttonId,
                buttonText,
                clientOnClick,
                urlPostback,
                tooltipText,
                buttonImageSrc);
        }

        /// <summary>
        /// Adds the toolbar button.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="buttonText">The button text.</param>
        /// <param name="navigationUrl">The navigation URL.</param>
        /// <param name="tooltipText">The tooltip text.</param>
        /// <param name="buttonImageSrc">The button image SRC.</param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void AddToolbarButton(
            string buttonId,
            string buttonText,
            string navigationUrl,
            string tooltipText,
            string buttonImageSrc)
        {
            Literal buttonMarkupLiteral = new Literal();

            if (!string.IsNullOrEmpty(buttonImageSrc))
            {
                buttonMarkupLiteral.Text = String.Format(
                    SPSToolbarButtonImageHtmlFormatNav,
                    SPHttpUtility.HtmlEncode(buttonText),
                    SPHttpUtility.HtmlEncode(navigationUrl),
                    SPHttpUtility.HtmlEncode(tooltipText),
                    SPHttpUtility.HtmlUrlAttributeEncode(buttonImageSrc));
            }
            else
            {
                // No image
                buttonMarkupLiteral.Text = String.Format(
                    SPSToolbarButtonHtmlFormatNav,
                    SPHttpUtility.HtmlEncode(buttonText),
                    SPHttpUtility.HtmlEncode(navigationUrl),
                    SPHttpUtility.HtmlEncode(tooltipText));
            }

            buttonMarkupLiteral.ID = buttonId;

            Controls.Add(buttonMarkupLiteral);
        }

        /// <summary>
        /// Adds the toolbar button.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="buttonText">The button text.</param>
        /// <param name="navigationUrl">The navigation URL.</param>
        /// <param name="tooltipText">The tooltip text.</param>
        /// <param name="buttonImageSrc">The button image SRC.</param>
        public void AddToolbarButton(
            string buttonId,
            string buttonText,
            Uri navigationUrl,
            string tooltipText,
            string buttonImageSrc)
        {
            AddToolbarButton(buttonId,
                             buttonText,
                             navigationUrl,
                             tooltipText,
                             buttonImageSrc);
        }

        /// <summary>
        /// Adds the sepparator.
        /// </summary>
        public void AddSepparator()
        {
            Literal buttonMarkupLiteral = new Literal();

            buttonMarkupLiteral.Text = SPSToolbarSeparator;
            buttonMarkupLiteral.ID = "";

            Controls.Add(buttonMarkupLiteral);
        }

        /// <summary>
        /// Adds the control.
        /// </summary>
        /// <param name="control">The control.</param>
        public void AddControl(Control control)
        {
            Controls.Add(control);
        }

        /// <summary>
        /// Adds the control.
        /// </summary>
        /// <param name="labelText">The label text.</param>
        /// <param name="control">The control.</param>
        public void AddControl(string labelText, Control control)
        {
            LabeledControl label = new LabeledControl(labelText, control);
            //label.ID = "l";
            Controls.Add(label);
        }

        //private void AddWithSeparator(Control control)
        //{
        //    if (Controls.Count > 0)
        //    {
        //        AddSepparator();
        //    }

        //    Controls.Add(control);            
        //}


//        private string ButtonHtml()
//        {
//           // {1} onClientClick
//           // {2} title
//           // {3} imgSrc
//           // {4} padding

//           string html =
//            @"<TABLE cellpadding=""1"" cellspacing=""0"" border=""0"" class='ms-toolbar-togglebutton-on' >
//	          <TR>
//	          <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">
//		      <A class=""ms-toolbar"" href=""#"" onclick=""{1}"" title=""{2}"">
//			  <IMG border=""0"" width=""16"" height=""16"" src=""{3}"" alt=""{2}""/>
//		      </A>
//	          </TD>	   
//	          <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">		
//		      <A class=""ms-toolbar"" href=""#"" onclick=""{1}"" title=""{2}"" />				      
//	          </TD>
//	          </TR>
//	          </TABLE>";
//            return "";
//        }

        /// <summary>
        /// Internal class to render a control with a label
        /// </summary>
        private class LabeledControl : CompositeControl
        {
            private readonly string _labelText;
            private readonly Control _childControl;

            private const string BeginLabeledControl =
                @"<TABLE cellpadding=""1"" cellspacing=""0"" border=""0"" >
	            <TR>
	              <TD class=""ms-toolbar"" nowrap style=""padding:3px;"">";

            private const string MiddleLabeledControl = @"</TD><TD>";

            private const string EndLabelControl =
                @"
	              </TD>
	            </TR>
	          </TABLE>";

            /// <summary>
            /// Initializes a new instance of the <see cref="LabeledControl"/> class.
            /// </summary>
            /// <param name="labelText">The label text.</param>
            /// <param name="control">The control.</param>
            public LabeledControl(string labelText, Control control)
            {
                _labelText = labelText;
                _childControl = control;
            }

            protected override void CreateChildControls()
            {
                Controls.Add(_childControl);
            }

            public override void RenderBeginTag(HtmlTextWriter writer)
            {
                writer.WriteLine(BeginLabeledControl);
                writer.WriteLine(_labelText);
                writer.WriteLine("&nbsp;");
                writer.WriteLine(MiddleLabeledControl);
            }

            public override void RenderEndTag(HtmlTextWriter writer)
            {
                writer.WriteLine(EndLabelControl);
            }
        }
    }
}