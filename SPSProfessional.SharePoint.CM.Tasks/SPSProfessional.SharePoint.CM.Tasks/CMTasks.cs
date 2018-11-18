using System;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.CM.Tasks
{
    public class CMTasks : LayoutsPageBase
    {
        private string _operation;
        private int _itemId;
        private Guid _listId;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            try
            {
                _operation = Page.Request["OP"];
                _itemId = Int32.Parse(Page.Request["ID"]);
                _listId = new Guid(Page.Request["LIST"]);

                switch (_operation)
                {
                    case "Complete":
                        CompleteTask();
                        break;
                }

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

            RegisterGoBack();
        }

        private void RegisterGoBack()
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page),
                                       "goBack",
                                       @"<script type=""text/javascript"" language=""javascript"">window.history.go(-1);</script>");
        }

        private void CompleteTask()
        {
            SPList list = SPContext.Current.Web.Lists[_listId];
            SPItem item = list.GetItemById(_itemId);

            try
            {
                item["PercentComplete"] = "100";
                item["Status"] = "3";
                item.Update();
                UpdateLits(list);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
        }

        private void UpdateLits(SPList list)
        {
            list.ParentWeb.AllowUnsafeUpdates = true;
            list.Update();
            list.ParentWeb.AllowUnsafeUpdates = false;
        }

        /// <summary>
        /// After paste we need a refres in order to show changes
        /// </summary>
        private void RefreshPage()
        {
            Debug.WriteLine("Refresh");
            string referrer = Page.Request.ServerVariables["HTTP_REFERER"];
            base.Page.Response.Redirect(referrer, false);
        }
    }
}