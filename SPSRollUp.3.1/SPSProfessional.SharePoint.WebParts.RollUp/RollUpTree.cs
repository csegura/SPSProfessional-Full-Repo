using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    [DefaultProperty("CategoryFilter")]
    [Category("SPSProfessional WebParts")]
    [ToolboxData("<{0}:RollUpTree runat=server></{0}:RollUpTree>")]
    [XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.RollUpTree")]
    public class RollUpTree : RollUpBase3
    {
        private TreeView _treeView;

        #region Constructor

        public RollUpTree()
        {
            SPSInit("9DA7AF31-81EB-4dbe-AF29-026FABB97223",
                    "SPSRollUp.2.0",
                    "RollUp WebPart",
                    "http://www.spsprofessional.com/page/SPSRollUp-WebPart.aspx");

            ExportMode = WebPartExportMode.All;
            EditorParts.Add(new RollUpTreeEditorPart());

            _topSite = string.Empty;
            _lists = string.Empty;
            _fields = string.Empty;
            _camlQuery = string.Empty;
            _xsl = string.Empty;
            _dateTimeISO = false;
            _fixLookUp = false;
            XslPage = 1;
            _errorBox = new SPSErrorControl();
        }

        #endregion

        #region Control Methods

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _treeView = new TreeView();
            _treeView.EnableClientScript = true;
            _treeView.EnableViewState = true; 
            
            //_treeView.ImageSet = TreeViewImageSet.Arrows;
            _treeView.NodeStyle.CssClass = "ms-navitem";
            _treeView.NodeStyle.HorizontalPadding = 2;
            _treeView.SelectedNodeStyle.CssClass = "ms-tvselected";
            //_treeView.SkipLinkText = "";
            
            _treeView.SelectedNodeChanged += _treeView_SelectedNodeChanged;
            Controls.Add(_treeView);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Page.IsPostBack || EditorParts[0].Display)
            {
                SPSTreeXML treeXML = GetTreeXML();
                if (treeXML != null)
                {
                    try
                    {
                        treeXML.Fill(_treeView, _collector.GetView());
                        treeXML.Decorate(_treeView);
                    }
                    catch(Exception ex)
                    {
                        TrapSubsystemError(this, 
                            new SPSErrorArgs("Render", SPSResources.GetString("SPS_Err_TreeDefinition"), ex));
                    }
                }
                else
                {
                    TrapSubsystemError(this, 
                        new SPSErrorArgs("Render", SPSResources.GetString("SPS_Err_EmptyTreeDefinition"), null));
                }
            }
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            Debug.WriteLine("RollUpTree: Render " + Title);

            if (!ValidProperties())
            {
                writer.WriteLine(MissingConfiguration);
            }
            else
            {
                EnsureChildControls();

                DebugRender(writer);
                
                if (!string.IsNullOrEmpty(Xsl))
                {
                    _treeView.RenderControl(writer);
                }
                
                //_errorBox.ShowExtendedErrors = ShowExtendedErrors;
                _errorBox.RenderControl(writer);
            }
        }

        void _treeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            int rowNumber;
            if (Int32.TryParse(_treeView.SelectedNode.Value, out rowNumber))
            {
                XslSelectedRow = rowNumber;
                InternalCollect();
                //_collector.
                //_rollupEngine.SelectedRow = XslSelectedRow;
                //ForceCrawl();
            }
        }

        #endregion

        private bool ValidProperties()
        {
            if (Lists.Length > 0 && Fields.Length > 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Gets the calendar items.
        /// </summary>
        /// <returns>CalendarItemCollection</returns>
        private SPSTreeXML GetTreeXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SPSTreeXML));

            if (!string.IsNullOrEmpty(Xsl))
            {               
                try
                {
                    TextReader reader = new StringReader(Xsl);
                    SPSTreeXML collection = (SPSTreeXML)serializer.Deserialize(reader);
                    return collection;
                }
                catch (InvalidOperationException ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs(ex.TargetSite.Name,
                                                 SPSResources.GetString("SPS_Err_InvalidXMLTree"),
                                                 ex));
                }
            }
            return null;
        }
 
        /// <summary>
        /// Gets the calendar items.
        /// </summary>
        /// <returns>CalendarItemCollection</returns>
        //private void SetupTree()
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(SPSTreeXML));
        //    SPSTreeXML collection;
            
        //    if (!string.IsNullOrEmpty(Xsl))
        //    {
        //        try
        //        {
        //            TextReader reader = new StringReader(Xsl);
        //            collection = (SPSTreeXML)serializer.Deserialize(reader);
        //            collection.Fill(_treeView, _rollupEngine.Data.Tables[0].DefaultView);
        //        }
        //        catch (Exception ex)
        //        {
        //            TrapSubsystemError(this, new SPSErrorArgs(GetType().Name,
        //                                         "Invalid XML for calendar",
        //                                         ex));                    
        //        }
        //    }
        //}
    }
}