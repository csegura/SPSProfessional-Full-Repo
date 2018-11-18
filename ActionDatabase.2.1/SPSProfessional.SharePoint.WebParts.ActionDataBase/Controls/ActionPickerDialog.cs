using System.Collections;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Relations;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.Controls
{
    internal class ActionPickerDialog : PickerDialog
    {
        public ActionPickerDialog()
                : base(new ActionLookupQueryControl(),
                       new TableResultControl(),
                       new ActionEntityPickerControl())
        {
            ArrayList columnDisplayNames = ((TableResultControl) ResultControl).ColumnDisplayNames;
            columnDisplayNames.Clear();
            columnDisplayNames.Add("Key");
            columnDisplayNames.Add("Value");
            ArrayList columnNames = ((TableResultControl) ResultControl).ColumnNames;
            columnNames.Clear();
            columnNames.Add("Key");
            columnNames.Add("Value");
            ArrayList columnWidths = ((TableResultControl) ResultControl).ColumnWidths;
            columnWidths.Clear();
            columnWidths.Add("20%");
            columnWidths.Add("80%");
        }

        public ActionEntity ExtendedData
        {
            get { return ((ActionEntityPickerControl) EditorControl).ExtendedData; }
        }
    }
}