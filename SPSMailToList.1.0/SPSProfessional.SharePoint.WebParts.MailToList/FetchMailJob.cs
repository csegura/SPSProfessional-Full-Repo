using System;
using System.Collections;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPSProfessional.SharePoint.WebParts.MailToList
{
    public class FetchMailJob : SPJobDefinition
    {
        public FetchMailJob()
        {
        }


        public FetchMailJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        {
        }

        public FetchMailJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "ListAdding Check";
        }


        public override void Execute(Guid contentDbId)
        {
            // get a reference to the current site collection's content database

            var webApplication = Parent as SPWebApplication;

            if (webApplication != null)
            {
                SPContentDatabase contentDb = webApplication.ContentDatabases[contentDbId];

                SPWeb objWeb = contentDb.Sites[0].RootWeb;

                DateTime objTiSiteCreationtime = objWeb.Created;

                SPList taskList = contentDb.Sites[0].RootWeb.Lists["Tasks"];


                var objQuery = new SPQuery();

                objQuery.Query = "<Where><Gt><FieldRef Name=\"ID\"/><Value Type=\"Counter\">0</Value></Gt></Where>";

                SPListItemCollection objCollection = taskList.GetItems(objQuery);

                DataTable dtAllLists = objCollection.GetDataTable();

                var objArrayList = new ArrayList();

                if (dtAllLists != null)
                {
                    if (dtAllLists.Rows.Count > 0)
                    {
                        for (int iCnt = 0; iCnt <= dtAllLists.Rows.Count - 1; iCnt++)
                        {
                            objArrayList.Add(Convert.ToString(dtAllLists.Rows[iCnt]["Title"]));
                        }
                    }
                }


                for (int iCnt = 0; iCnt <= objWeb.Lists.Count - 1; iCnt++)
                {
                    if (!objArrayList.Contains(objWeb.Lists[iCnt].Title))
                    {
                        if (objWeb.Lists[iCnt].Created.ToShortDateString()
                            != objTiSiteCreationtime.ToShortDateString())
                        {
                            SPListItem newTask = taskList.Items.Add();

                            newTask["Title"] = objWeb.Lists[iCnt].Title;

                            newTask.Update();
                            //Write a logic to send a mail to admin
                        }
                    }
                }
            }
        }
    }
}

}