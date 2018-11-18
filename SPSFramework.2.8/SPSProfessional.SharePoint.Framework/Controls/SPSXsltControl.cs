using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// Xslt Control 
    /// </summary>
    public class SPSXsltControl : Control, IDesignTimeHtmlProvider, ISPSErrorControl, INamingContainer
    {
        private string _xsl;
        private string _xml;
        private int? _xslPage;
        private string _xslOrder;
        private int? _xslSelectedRow;
        private SPSKeyValueList _lastRow;
        private bool _debugSource;
        private bool _debugTransformation;

        #region Properties

        /// <summary>
        /// Gets or sets the XSL.
        /// </summary>
        /// <value>The XSL.</value>
        public string Xsl
        {
            get { return _xsl; }
            set { _xsl = value; }
        }

        /// <summary>
        /// Gets or sets the XML data.
        /// </summary>
        /// <value>The XML data.</value>
        public string XmlData
        {
            get { return _xml; }
            set { _xml = value; }
        }

        /// <summary>
        /// Gets or sets the XSL page.
        /// </summary>
        /// <value>The XSL page.</value>
        public int? XslPage
        {
            get { return _xslPage; }
            set { _xslPage = value; }
        }

        /// <summary>
        /// Gets or sets the XSL order.
        /// </summary>
        /// <value>The XSL order.</value>
        public string XslOrder
        {
            get { return _xslOrder; }
            set { _xslOrder = value; }
        }

        /// <summary>
        /// Gets or sets the XSL selected row.
        /// </summary>
        /// <value>The XSL selected row.</value>
        public int? XslSelectedRow
        {
            get { return _xslSelectedRow; }
            set { _xslSelectedRow = value; }
        }

        /// <summary>
        /// Gets or sets the last row.
        /// </summary>
        /// <value>The last row.</value>
        public SPSKeyValueList LastRow
        {
            get { return _lastRow; }
            set { _lastRow = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [debug source].
        /// </summary>
        /// <value><c>true</c> if [debug source]; otherwise, <c>false</c>.</value>
        public bool DebugSource
        {
            get { return _debugSource; }
            set { _debugSource = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [debug transformation].
        /// </summary>
        /// <value><c>true</c> if [debug transformation]; otherwise, <c>false</c>.</value>
        public bool DebugTransformation
        {
            get { return _debugTransformation; }
            set { _debugTransformation = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSXsltControl"/> class.
        /// </summary>
        public SPSXsltControl()
        {            
        }

        #region Control

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> 
        /// object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            Debug.WriteLine("SPSXsltControl RenderControl");
            if (!string.IsNullOrEmpty(XmlData) && !string.IsNullOrEmpty(Xsl))
            {
                try
                {
                    RenderControlInternal(writer);
                }
                catch(Exception ex)
                {
                    TrapError(GetType(),"RenderControl",ex); 
                }
            }
        }

        

        /// <summary>
        /// Internals the render.
        /// Make the Xslt transformation
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void RenderControlInternal(HtmlTextWriter writer)
        {
            Debug.WriteLine("SPSXsltControl RenderControlInternal");
            try
            {
                StringWriter stringWriter = Transform();
                
                if (DebugSource || DebugTransformation)
                {
                    DebugRender(writer, stringWriter.ToString());
                }
                
                writer.Write(stringWriter);
            }
            catch (Exception ex)
            {
                TrapError(GetType(),"RenderControlInternal",ex);
            }
            //string final = sw.ToString();
            //final = final.Replace("&lt;", "<");
            //final = final.Replace("&gt;", ">");
            //return final;
        }

        /// <summary>
        /// Debugs the render.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="transformed">The transformated.</param>
        internal void DebugRender(HtmlTextWriter writer, string transformed)
        {
            SPSEditorPartsTools tools = new SPSEditorPartsTools(writer);

            //if (DebugSource)
            //{
            //    writer.Write("<font color=red>DEBUG XML DATA</font><br><font color=blue>");
            //    writer.Write("<pre>" + SPEncode.HtmlEncode(tools.FormatXml(XmlData)) + "</pre>");
            //    writer.Write("</font><br>");
            //}

            if (DebugTransformation)
            {
                writer.Write("<font color=red>DEBUG XML TRANSFORM</font><br><font color=blue>");
                writer.Write("<pre>" + SPEncode.HtmlEncode(tools.FormatXml(transformed)) + "</pre>");
                writer.Write("</font><br>");
            }
        }

        #endregion

        #region Engine

        internal StringWriter Transform()
        {
            StringWriter stringWriter = new StringWriter();
            try
            {
                XslCompiledTransform xslt = GetCompiled();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(XmlData);

                XmlTextWriter xmlw = new XmlTextWriter(stringWriter);
                XsltArgumentList xal = new XsltArgumentList();

                if (XslPage.HasValue)
                {
                    xal.AddParam("CurrentPage", String.Empty, XslPage);
                }
                if (XslOrder != null)
                {
                    xal.AddParam("CurrentOrder", String.Empty, XslOrder);
                }
                if (XslSelectedRow.HasValue)
                {
                    xal.AddParam("CurrentRow", String.Empty, XslSelectedRow);
                }

                //create custom object
                SPSXsltExtension xsltExtension = new SPSXsltExtension(Parent, Page, LastRow);

                //pass an instance of the custom object
                xal.AddExtensionObject("http://schemas.spsprofessional.com/WebParts/SPSXSLT", xsltExtension);

                xslt.Transform(xmlDocument, xal, xmlw);
            }
            catch (Exception ex)
            {
                TrapError(GetType(),SPSLocalization.GetResourceString("SPSFW_Err_XsltTransformation"), ex);
            }
            return stringWriter;
        }

        /// <summary>
        /// Gets the compiled Xslt
        /// </summary>
        /// <returns>XslCompiledTransform of Xslt</returns>
        private XslCompiledTransform GetCompiled()
        {
            XmlTextReader xsl = new XmlTextReader(new StringReader(Xsl));

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsl);

            return xslt;
        }

        #endregion

        #region IDesignTimeHtmlProvider Members

        ///<summary>
        ///Returns the HTML that is used to represent the control at design time.
        ///</summary>
        ///
        ///<returns>
        ///A string that contains the HTML.
        ///</returns>
        ///
        public string GetDesignTimeHtml()
        {
            Debug.WriteLine("SPSXsltControl GetDesignTimeHtml");
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());
            RenderControlInternal(writer);
            return writer.ToString();
        }

        #endregion

        #region ISPSErrorControl Members

        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        public event SPSControlOnError OnError;

        #endregion

        internal void TrapError(Type type, string message, Exception ex)
        {
            if (OnError != null)
            {
                OnError(this, new SPSErrorArgs(type.ToString(), message, ex));
            }
        }
    }
}