namespace SPSProfessional.SharePoint.Framework.ConfigurationManager
{
    /// <summary>
    /// Represents a config item in the config store list.
    /// </summary>
    public class SPSConfigurationManagerIdentifier
    {
        private string _category = null;
        private string _key = null;

        /// <summary>
        /// Category of the config item.
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
        
        /// <summary>
        /// Key (name) of the config item.
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSConfigurationManagerIdentifier"/> class.
        /// </summary>
        /// <param name="Category">The category.</param>
        /// <param name="Key">The key.</param>
        public SPSConfigurationManagerIdentifier(string Category, string Key)
        {
            _category = Category;
            _key = Key;
        }
    }
}