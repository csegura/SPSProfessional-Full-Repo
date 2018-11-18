using System;
using System.Xml.Serialization;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    [Serializable]
    public class SPSLeafItem : SPSBranchItem
    {        
    }

    [Serializable]
    public class SPSBranchItem
    {
        private string _fieldName;
        private string _imageUrl;

        #region Properties

        [XmlAttribute]
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        [XmlAttribute]
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        #endregion

        public SPSBranchItem()
        {
            FieldName = string.Empty;
            ImageUrl = string.Empty;
        }
    }
}