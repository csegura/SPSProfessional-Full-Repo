// File : SPSECBMenu.cs
// Date : 28/07/2008
// User : csegura
// Logs

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.UI;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    /// <summary>
    /// Emulate the ECBItems table
    /// 
    /// <TABLE ID="ECBItems" style="display:none" height="0" width="0">
    /// <TR>
    /// <TD>Copy Item To</TD> {0}
    /// <TD>/_layouts/images/spspro_copy.gif</TD> {1}
    /// <TD>_layouts/SPSProfessional_ListCopyTo.aspx?ItemId={ItemId}&ListId={ListId}</TD> {2}
    /// <TD>0x0</TD> {3}
    /// <TD>0x0</TD> {4}
    /// <TD>ContentType</TD>
    /// <TD>0x010800E43EE242C4F81A44AD3985805CD766A7</TD> {5}
    /// <TD>240</TD> {6}
    /// </TR>
    /// </TABLE>
    /// 
    /// </summary>
    public sealed class SPSECBMenu : Control
    {
        private const string BeginTableTemplate =
                "<TABLE ID=\"ECBItems\" style=\"display:none\" height=\"0\" width=\"0\">";

        private const string RowTemplate = 
                "<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD>{3}</TD><TD>{4}</TD><TD>ContentType</TD>"+
                "<TD>{5}</TD><TD>{6}</TD></TR>";


        private const string EndTableTemplate = "</TABLE>";

        private readonly List<SPSECBMenuItem> _menuItems;

        #region  Public Properties

        public List<SPSECBMenuItem> MenuItems
        {
            get { return _menuItems; }
            //TODO: Clean set { _menuItems = value; }
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.UI.Control" /> class.
        /// </summary>
        public SPSECBMenu()
        {
            _menuItems = new List<SPSECBMenuItem>();
        }


        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HTmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(BeginTableTemplate);

            foreach(SPSECBMenuItem menuItem in MenuItems)
            {
                string highRight;
                string lowRight;

                ConvertULongToInt32HexPair(0x7fffffffffffffffL, out highRight, out lowRight);
                ConvertULongToInt32HexPair((ulong)menuItem.Rights, out highRight, out lowRight);
            
                writer.Write(RowTemplate,
                             menuItem.Title,
                             menuItem.Image,
                             menuItem.UrlAction,
                             highRight,
                             lowRight,
                             menuItem.ListGuid,
                             menuItem.Sequence);
            }

            writer.Write(EndTableTemplate);
        }

        /// <summary>
        /// Converts the U long to int32 hex pair.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="high">The high.</param>
        /// <param name="low">The low.</param>
        private void ConvertULongToInt32HexPair(ulong value, out string high, out string low)
        {
            high = "0x" + ((value >> 0x20)).ToString("X", CultureInfo.InvariantCulture);
            low = "0x" + ((value & 0xffffffffL)).ToString("X", CultureInfo.InvariantCulture);
        }

    }
}