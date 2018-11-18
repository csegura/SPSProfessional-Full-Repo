using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SPSProfessional.SharePoint.Framework.Error
{
    /// <summary>
    /// Each line of error
    /// </summary>
    [XmlType(TypeName = "SPSConfigError"), Serializable]
    public class SPSConfigError
    {
        [XmlAttribute(AttributeName = "Match", Form = XmlSchemaForm.Unqualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __Match;

        /// <summary>
        /// Gets or sets the match.
        /// </summary>
        /// <value>The match.</value>
        [XmlIgnore]
        public string Match
        {
            get { return __Match; }
            set { __Match = value; }
        }

        [XmlAttribute(AttributeName = "Message", Form = XmlSchemaForm.Unqualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __Message;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        [XmlIgnore]
        public string Message
        {
            get { return __Message; }
            set { __Message = value; }
        }

        [XmlAttribute(AttributeName = "Color", Form = XmlSchemaForm.Unqualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __Color;

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [XmlIgnore]
        public string Color
        {
            get { return __Color; }
            set { __Color = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPSConfigError"/> class.
        /// </summary>
        public SPSConfigError()
        {
            Match = string.Empty;
            Message = string.Empty;
            Color = "#ff0000";
        }
    }
}