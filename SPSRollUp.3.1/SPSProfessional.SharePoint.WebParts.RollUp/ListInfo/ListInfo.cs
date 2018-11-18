namespace SPSProfessional.SharePoint.WebParts.RollUp.ListInfo
{
    public sealed class ListInfo
    {
        private readonly string _name;
        private readonly string _url;
        private readonly string _listType;
        private readonly string _description;
        
        public ListInfo(string name, string url, string listType, string description)
        {
            _name = name;
            _listType = listType;
            _url = url;
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }

        public string ListType
        {
            get { return _listType; }
        }

        public string Url
        {
            get { return _url; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}