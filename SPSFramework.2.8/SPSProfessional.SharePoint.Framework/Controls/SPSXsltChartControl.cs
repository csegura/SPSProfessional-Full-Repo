using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.UI;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// Control to render a Chart
    /// </summary>
    public class SPSXsltChartControl : SPSXsltControl, ISPSXsltChartControl
    {
        private string _graphWidth = "200";
        private string _graphHeight = "150";
        private string _graphType = "Column2D";

        #region Properties ISPSXsltChartControl

        /// <summary>
        /// Gets or sets the width of the graph.
        /// </summary>
        /// <value>The width of the graph.</value>
        public string GraphWidth
        {
            get { return _graphWidth; }
            set { _graphWidth = value; }
        }

        /// <summary>
        /// Gets or sets the height of the graph.
        /// </summary>
        /// <value>The height of the graph.</value>
        public string GraphHeight
        {
            get { return _graphHeight; }
            set { _graphHeight = value; }
        }

        /// <summary>
        /// Gets or sets the type of the graph.
        /// </summary>
        /// <value>The type of the graph.</value>
        public string GraphType
        {
            get { return _graphType; }
            set { _graphType = value; }
        }

        #endregion

        #region Control

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            string scriptName = GetType().ToString();
            
            ClientScriptManager clientScript = Page.ClientScript;
            
            if (!clientScript.IsClientScriptBlockRegistered(scriptName))
            {
                clientScript.RegisterClientScriptInclude(
                        scriptName,
                        "/_layouts/FusionCharts/FusionCharts.js");
            }

            clientScript.RegisterClientScriptBlock(GetType(),
                                                   scriptName,
                                                   GenerateStartupJS(),
                                                   true);

            base.OnLoad(e);
        }

        /// <summary>
        /// Internals the render.
        /// Make the Xslt transformation
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void RenderControlInternal(HtmlTextWriter writer)
        {
            Debug.WriteLine("SPSXsltChartControl RenderControlInternal");
            
            StringWriter stringWriter = Transform();
            string xml = stringWriter.ToString();

            xml = xml.Replace("'", string.Empty);
            xml = xml.Replace('"', '\'');

            DebugRender(writer, xml);

            writer.Write("<div id=\"{0}Xml\" style=\"display:none\">{1}</div>", UniqueID, SPHttpUtility.UrlKeyValueEncode(xml));
            writer.Write("<div id=\"{0}chart\"></div>", UniqueID);
            
            writer.Write("\n<script language=\"javascript\">\n");            
            writer.Write(GetShowChartScriptReference());            
            writer.Write("</script>\n");
        }

        #endregion

        /// <summary>
        /// Generates the startup JS.
        /// </summary>
        /// <returns></returns>
        private string GenerateStartupJS()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("\nfunction SPSShowChart(id)\n{\n");
            sb.AppendFormat(" var FSChart=new FusionCharts(\"{0}\",id+\"chart\",\"{1}\",\"{2}\",\"0\",\"1\");\n",
                            "/_layouts/FusionCharts/FCF_" + GraphType + ".swf",
                            GraphWidth,
                            GraphHeight);
            sb.Append(" var xmldata=document.getElementById(id+\"Xml\").innerHTML;\n");
            sb.Append(" FSChart.setDataXML(unescape(xmldata));\n");
            sb.Append(" FSChart.render(id+\"chart\");\n");
            sb.Append("}\n");            

            return sb.ToString();
        }

        /// <summary>
        /// Return a string with the necesary JavaScript function to 
        /// show the chart
        /// </summary>
        /// <returns>The javascript function necesary to show the chart</returns>
        public string GetShowChartScriptReference()
        {
            return "SPSShowChart('" + UniqueID + "');";
        }
    }
}