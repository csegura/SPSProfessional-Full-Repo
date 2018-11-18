using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp.FieldInfo
{
    public class SPSProfessional_FieldInfo : Page
    {
        protected Literal _dialogTitle;
        protected Literal _dialogDescription;
        protected Image _dialogImage;
        protected Repeater _rptFieldsInfo;
        protected Literal _message;

        private FieldCollector _fieldCollector;

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

            _dialogTitle.Text = "SPSRollUp Field Information";
            _dialogDescription.Text = "Fields Crawled by SPSRollUp from " + TopUrl;

            base.OnInit(e);
        }

        void OkButton_Click(object sender, EventArgs e)
        {
            string strScript;
            //string fieldsList = string.Empty;

            //List<string> fields = GetSelectedItems(_rptFieldsInfo, "chkBox");

            //if (fields != null)
            //{
            //    fields.ForEach(x => fieldsList += x + ",");
            //}            

            strScript = "<script>";
            //strScript += "window.returnValue='" + fieldsList.Trim(',') + "';";
            //strScript += "window.opener=self;";
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
            
            try
            {
                _fieldCollector = new FieldCollector(options);
                _fieldCollector.Collect();
                _rptFieldsInfo.DataSource = _fieldCollector.FieldsInfo;
                _rptFieldsInfo.DataBind();

                // SelectedItems(_rptFieldsInfo,Fields.Split(','),"chkBox");
            }
            catch (Exception ex)
            {
                //_message.Text = ex.ToString();
                Debug.WriteLine(ex);
            }
        }

        private void SelectedItems(Repeater rpt, ICollection<string> ids, string chkBoxId)
        {
            for (int i = 0; i < rpt.Items.Count; i++)
            {
                var chkBox = rpt.Items[i].FindControl(chkBoxId) as HtmlInputCheckBox;

                if (chkBox != null && ids.Contains(chkBox.Value))
                {
                    chkBox.Checked = true;
                }
            }
        }

        public static List<string> GetSelectedItems(Repeater rpt, string chkBoxId)
        {
            var selectedValues = new List<string>();

            for (int i = 0; i < rpt.Items.Count; i++)
            {
                var chkBox = rpt.Items[i].FindControl(chkBoxId) as HtmlInputCheckBox;

                if (chkBox != null && chkBox.Checked)
                {
                    selectedValues.Add(chkBox.Value);
                }
            }

            return selectedValues;
        }
       
    }
}