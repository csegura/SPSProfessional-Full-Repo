namespace SPSProfessional.SharePoint.Framework.Controls
{
    public interface ISPSViewCommandParameters
    {
        int? XslPage
        {
            get;
            set;
        }

        int? XslSelectedRow
        {
            get;
            set;
        }

        string XslOrder
        {
            get;
            set;
        }
    }   
}