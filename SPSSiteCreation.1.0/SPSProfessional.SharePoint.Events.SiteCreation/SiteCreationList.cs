namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    internal class SiteCreationList
    {
        private readonly string _listName;
        private readonly string _listID;

        #region Public Properties

        public string ListName
        {
            get { return _listName; }
        }

        public string ListID
        {
            get { return _listID; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SiteCreationList(string listName, string listID)
        {
            _listName = listName;
            _listID = listID;
        }
    }
}
