using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp.ListInfo
{
    public class SPSProfessional_ListInfo : Page
    {
        protected Literal _dialogTitle;
        protected Literal _dialogDescription;
        protected Image _dialogImage;
        protected Repeater _rptListsInfo;

        private ListCollector _listCollector;

        #region Dialog Properties

        public string TopUrl
        {
            get { return SPHttpUtility.UrlKeyValueDecode(Page.Request.QueryString["TopUrl"]); }
        }

        public string Lists
        {
            get { return SPHttpUtility.UrlKeyValueDecode(Page.Request.QueryString["Lists"]); }
        }

        public string Fields
        {
            get { return SPHttpUtility.UrlKeyValueDecode(Page.Request.QueryString["Fields"]); }
        }

        public bool Recursive
        {
            get
            {
                return SPHttpUtility.UrlKeyValueDecode(Page.Request.QueryString["Recursive"]).ToUpper() == "ON"
                               ? true
                               : false;
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            ((DialogMaster)Page.Master).OkButton.Click += OkButton_Click;

            _dialogTitle.Text = "SPSRollUp List Information";
            _dialogDescription.Text = "Lists Crawled by SPSRollUp from " + TopUrl;

            base.OnInit(e);
        }

        void OkButton_Click(object sender, EventArgs e)
        {
            string strScript;
                 
            strScript = "<script>";
            strScript += "window.close();";
            strScript += "</script>";

            Response.Write(strScript);
        }

        protected override void OnPreRender(EventArgs e)
        {
            FillRepeater();
        }
        
        protected void FillRepeater()
        {
            SPSRollUpOptions options = new SPSRollUpOptions(TopUrl, Lists, Recursive); 
           
            _listCollector = new ListCollector(options);

            try
            {
                _listCollector.Collect();
                _rptListsInfo.DataSource = _listCollector.FieldsInfo;
                _rptListsInfo.DataBind();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }       
       
    }
}