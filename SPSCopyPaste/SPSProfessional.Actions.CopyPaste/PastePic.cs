using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.Actions.CopyPaste
{
    /// <summary>
    /// Este control crea el menu con la opción PASTE en el menu actions   
    /// </summary>
    public class PastePic : WebControl
    {
        private SPSControlar _controlador;

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                Debug.WriteLine("OnLoad PASTE");
                _controlador = new SPSControlar("60D3AF22-BB0B-4ac0-956B-AF60FC9402E5",
                                               "CutCopyPaste.2.5");
                Debug.WriteLine(_controlador.Aceptado());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            EnsureChildControls();
            base.OnLoad(e);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            Trace.WriteLine("Render PASTE");
            try
            {
                SPWeb web = SPContext.Current.Web;
                Debug.WriteLine(web.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            base.RenderControl(writer);
        }


        /// <summary>
        /// Crea la entrada en el menu
        /// A la vez se crea un control-oculto que es el que se encargará 
        /// de realizar la acción de pegar - es el truco del invento
        /// </summary>
        protected override void CreateChildControls()
        {
            Debug.WriteLine("CreateChildControls");
           
            if (_controlador.Aceptado())
            {
                try
                {

                    // Control que realizará la acción (oculto)
                    var pasteAction = new PasteActionPic
                                          {
                                              ID = "PAP"+ClientID
                                          };
                    Controls.Add(pasteAction);

                    // Menu con la opción de pegar
                    var customAction = new MenuItemTemplate
                                           {
                                               Text = GetResourceString("PasteMenuActionText"),
                                               ID = "SPSProfessional.Paste",
                                               Description = GetResourceString("PasteImageMenuActionDescription"),
                                               ImageUrl = "/_layouts/images/spspro_paste.png",
                                               Sequence = 1000,
                                               ClientOnClickScript = "javascript:SPSProfessionalActionPaste();"
                                           };

                    // Añadimos la opción
                    Controls.Add(customAction);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debug.Flush();
                }
            }
            Debug.WriteLine("CreateChildControls End");
        }

        public static string GetResourceString(string key)
        {
            Debug.WriteLine("GetResourceString " + key);
            const string resourceClass = "SPSProfessional.Actions.CopyPaste";
            uint lang = SPContext.Current.Web.Language;                  
            string value = SPUtility.GetLocalizedString("$Resources:"+key, resourceClass, lang);
            Debug.WriteLine("GetResourceString End");
            return value;
        }
    }
}