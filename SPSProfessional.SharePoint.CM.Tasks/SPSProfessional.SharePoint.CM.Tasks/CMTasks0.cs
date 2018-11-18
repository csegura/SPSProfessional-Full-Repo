using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.CM.Tasks
{
    public  class CMTasks0 : WebControl
    {
        private int _itemId;

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();
            base.OnLoad(e);            
        }

        protected override void CreateChildControls()
        {
            var completeTask = new SPSPostBackEventMenuItem
                                   {
                                       ID = "CompleteTasks",
                                       Title = "Complete the Task",
                                       ImageUrl = "/_layouts/thismeet.gif",
                                       Sequence = 401
                                   };

            completeTask.OnPostBackEvent += CompleteTaskOnPostBackEvent;
            
            Controls.Add(completeTask);
            Page.Controls.Add(completeTask);
            
            base.CreateChildControls();
        }

        private void CompleteTaskOnPostBackEvent(object sender, EventArgs e)
        {
            SPList list = SPContext.Current.List;
            SPListItem item = SPContext.Current.ListItem;

            item["PercentCompletd"] = "100%";
            item["Status"] = "Completed";
            list.Update();

            RefreshPage();
        }

        public static string GetResourceString(string key)
        {
            Debug.WriteLine("GetResourceString " + key);
            const string resourceClass = "SPSProfessional.Actions.CopyPaste";
            uint lang = SPContext.Current.Web.Language;
            string value = SPUtility.GetLocalizedString("$Resources:" + key, resourceClass, lang);
            Debug.WriteLine("GetResourceString End");
            return value;
        }

        /// <summary>
        /// After paste we need a refres in order to show changes
        /// </summary>
        private void RefreshPage()
        {
            Debug.WriteLine("Refresh");
            //base.Page.Response.Redirect(Page.Request.Url.ToString());
            base.Page.Response.Redirect(Page.Request.Url.ToString(), false);
        }
    }
}