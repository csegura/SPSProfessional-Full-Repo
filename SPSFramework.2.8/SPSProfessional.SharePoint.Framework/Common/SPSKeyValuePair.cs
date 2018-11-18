using System;

namespace SPSProfessional.SharePoint.Framework.Common
{
    /// <summary>
    /// Simple class to store a Key value Pair   
    /// </summary>
    [Serializable]
    public sealed class SPSKeyValuePair
    {
        private string _key;
        private string _value;

        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSKeyValuePair"/> class.
        /// </summary>
        private SPSKeyValuePair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSKeyValuePair"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">Argument is null.</exception>
        public SPSKeyValuePair(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            _key = key;
            _value = value;
        }

        #endregion
    }
}