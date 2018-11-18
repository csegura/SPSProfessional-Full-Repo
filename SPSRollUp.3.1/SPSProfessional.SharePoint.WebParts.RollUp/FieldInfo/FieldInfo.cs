namespace SPSProfessional.SharePoint.WebParts.RollUp.FieldInfo
{
    public sealed class FieldInfo
    {
        private readonly string _name;
        private readonly string _internalName;
        private readonly string _dataType;
        private readonly string _description;
        
        public FieldInfo(string name, string internalName, string dataType, string description)
        {
            _name = name;
            _dataType = dataType;
            _internalName = internalName;
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }

        public string DataType
        {
            get { return _dataType; }
        }

        public string InternalName
        {
            get { return _internalName; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}