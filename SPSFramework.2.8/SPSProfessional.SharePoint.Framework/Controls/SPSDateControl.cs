// File : SPSDateControl.cs
// Date : 24/06/2008
// User : csegura
// Logs

using System;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// Subclass sharepoint DateTimeControl
    /// </summary>
    public class SPSDateControl : DateTimeControl
    {
        /// <summary>
        /// Raises the <see cref="E:PreRender"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            TextBox input = (TextBox) FindControl(ID + "Date");
            
            if (input != null)
            {
                input.Columns = 10;
            }
            
            base.OnPreRender(e);
        }
    }
}