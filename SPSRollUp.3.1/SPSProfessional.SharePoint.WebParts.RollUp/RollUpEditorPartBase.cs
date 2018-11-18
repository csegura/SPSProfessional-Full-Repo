using System;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    internal class RollUpEditorPartBase : EditorPart
    {
        protected TextBox _topSite;
        protected TextBox _lists;
        protected TextBox _fields;
        protected CheckBox _camlQueryRecursive;
        protected LinkButton _btnFieldInfo;
        protected LinkButton _btnListInfo;
        protected TextBox _sortFields;
        protected TextBox _camlQuery;
        protected TextBox _xsl;
        protected TextBox _maxResults;
        protected CheckBox _dateTimeISO;
        protected CheckBox _fixLookUp;
        protected CheckBox _showExtendedErrors;
        
        protected override void OnLoad(EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "FieldsSelector",
                GenJavaScript_InfoPopUps(),
                true);

            base.OnLoad(e);
        }

        protected string GenJavaScript_InfoPopUps()
        {
            string url = "/_layouts/SPSProfessional_FieldInfo.aspx?" +
                         "TopUrl='+topSite+'"+
                         "&Lists='+lists.value+'"+
                         "&Recursive='+recursive+'";

            string js = string.Empty;

            js += "function FieldInfo() {";
            js += " var topSite=document.getElementById('" + _topSite.ClientID + "').value;";
            js += " if (topSite == '') { topSite = '"+ SPContext.Current.Web.Url +"'; }";
            js += " var lists=document.getElementById('" + _lists.ClientID + "');";
            js += " var recursive=document.getElementById('" + _camlQueryRecursive.UniqueID + "').value;";
            //js += " var fields=document.getElementById('" + _fields.ClientID + "');";
            js += " " + GenJavaScriptDialog(url, "null");   
            js += " return false;";
            js += "}\n";


            url = "/_layouts/SPSProfessional_ListInfo.aspx?" +
                  "TopUrl='+topSite+'" +
                  "&Lists='+lists.value+'" +
                  //"&Fields='+fields.value+'"+
                  "&Recursive='+recursive+'";

            js += "function ListInfo() {";
            js += " var topSite=document.getElementById('" + _topSite.ClientID + "').value;";
            js += " if (topSite == '') { topSite = '" + SPContext.Current.Web.Url + "'; }";
            js += " var recursive=document.getElementById('" + _camlQueryRecursive.UniqueID + "').value;";
            js += " var lists=document.getElementById('" + _lists.ClientID + "');";
            //js += " var fields=document.getElementById('" + _fields.ClientID + "');";
            js += " " + GenJavaScriptDialog(url, "null");
            js += " return false;";
            js += "}\n";

            //js += "function FieldSelectorReturn(value) {";
            //js += " alert(value);";
            //js += " var fields=document.getElementById('" + _fields.ClientID + "');";
            //js += " fields.value = value; ";
            //js += "}";
            return js;
        }

        /// <summary>
        /// Gens the java script dialog.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="callBack">The call back.</param>
        /// <returns></returns>
        private string GenJavaScriptDialog(string url,string callBack)
        {
            string features = "resizable: yes; status: no; scroll: no; help: no; center: yes; " +
                              "dialogWidth : 500px; dialogHeight : 550px;";

            if (!SPSTools.IsIE55Plus(Context))
            {
                features = "resizable=yes,status=no,scrollbars=no,menubar=no," +
                           "directories=no,location=no,width=500px,height=550px";
            }

            return string.Format("commonShowModalDialog('{0}','{1}',{2},null);",
                                 url,
                                 features,
                                 callBack);
        }

        #region Overrides of EditorPart

        /// <summary>
        /// Saves the values in an <see cref="T:System.Web.UI.WebControls.WebParts.EditorPart" /> control to the corresponding properties in the associated <see cref="T:System.Web.UI.WebControls.WebParts.WebPart" /> control.
        /// </summary>
        /// <returns>
        /// true if the action of saving values from the <see cref="T:System.Web.UI.WebControls.WebParts.EditorPart" /> control to the <see cref="T:System.Web.UI.WebControls.WebParts.WebPart" /> control is successful; otherwise (if an error occurs), false. 
        /// </returns>
        public override bool ApplyChanges()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Retrieves the property values from a <see cref="T:System.Web.UI.WebControls.WebParts.WebPart" /> control for its associated <see cref="T:System.Web.UI.WebControls.WebParts.EditorPart" /> control.
        /// </summary>
        public override void SyncChanges()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}