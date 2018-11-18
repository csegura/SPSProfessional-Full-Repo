// File : ISPSErrorControl.cs
// Date : 30/01/2008
// User : csegura
// Logs

using SPSProfessional.SharePoint.Framework.Controls;

namespace SPSProfessional.SharePoint.Framework.Error
{
    /// <summary>
    /// Ensure that a class has support for <see cref="SPSErrorBoxControl"/>
    /// </summary>
    public interface ISPSErrorControl
    {
        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        event SPSControlOnError OnError;
    }
}