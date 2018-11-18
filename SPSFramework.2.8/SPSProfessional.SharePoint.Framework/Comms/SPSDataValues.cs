// File : SPSDataValues.cs
// Date : 01/02/2008
// User : csegura
// Logs
// 01/02/2008 (csegura) - Created.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SPSProfessional.SharePoint.Framework.Comms
{
    /// <summary>
    /// Simple class to store Name - Value pairs
    /// </summary>
    [Serializable]
    public class SPSDataValuesCollection : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SPSDataValuesCollection"/> class.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</param>
        public SPSDataValuesCollection(
           SerializationInfo info, 
           StreamingContext context) : base(info,context)
        {            
        }

    }
}