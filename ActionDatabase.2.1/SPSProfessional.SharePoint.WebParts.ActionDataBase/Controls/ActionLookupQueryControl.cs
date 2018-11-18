using System;
using System.Data;
using System.Diagnostics;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Relations;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.Controls
{
    internal class ActionLookupQueryControl : SimpleQueryControl
    {
        public ActionLookupQueryControl()
        {
            Load += CustomQueryControl_Load;
        }

        private void CustomQueryControl_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                EnsureChildControls();
                ColumnList.Enabled = false;
                ColumnList.Visible = false;
            }
        }
       

        protected override int IssueQuery(string search, string groupName, int pageIndex, int pageSize)
        {
            Debug.WriteLine("%%%%%% IssueQuery ->" + search);
            int rows;

            if ((search == null) || (search.Trim().Length == 0))
            {
                PickerDialog.ErrorMessage = SPResource.GetString("PeoplePickerNoQueryTextError", new object[0]);
                return 0;
            }

            ActionPickerDialog pickerDialog = (ActionPickerDialog) PickerDialog;
            ActionEntityDataBase actionEntityDataBase = new ActionEntityDataBase(pickerDialog.ExtendedData);
            DataTable dataTable = actionEntityDataBase.MatchesTable(search);

            PickerDialog.Results = dataTable;
            rows = dataTable.Rows.Count;
            PickerDialog.ResultControl.PageSize = rows;

            if (rows == 0)
            {
                PickerDialog.ErrorMessage = SPResource.GetString("PeoplePickerNoSearchResultError", new object[0]);
            }

            Debug.WriteLine(rows);

            return rows;
        }

        public override PickerEntity GetEntity(DataRow dr)
        {
            if (dr == null)
            {
                return null;
            }

            PickerEntity entity = new PickerEntity
                                  {
                                          DisplayText = dr["Value"].ToString(),
                                          Key = dr["Key"].ToString(),
                                          IsResolved = true
                                  };

            return entity;
        }
    }
}