using System;
using System.Diagnostics;
using System.Web.UI;

namespace SPSProfessional.Actions.CopyPaste
{
    /// <summary>
    /// This control handle the paste action    
    /// </summary>
    internal class PasteActionPic : PasteAction
    {
        private string _eventArgs;

        /// <summary>
        /// Register the javascript action
        /// Check for postbacks
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
#if (DEBUG1)
            FileStream fileStream;
            try
            {
                fileStream = new FileStream("C:\\spslog.txt", FileMode.Append);
            }
            catch (Exception)
            {
                fileStream = new FileStream("C:\\spslog.txt", FileMode.OpenOrCreate);
            }
            TextWriterTraceListener traceListener = new TextWriterTraceListener(fileStream);
            Debug.Listeners.Add(traceListener);
            Debug.AutoFlush = true;
#endif
            Debug.WriteLine("PasteAction-OnLoad wef");

            try
            {
                // Register script to handle Copy & Paste
                ClientScriptManager clientScript = Page.ClientScript;
                const string scriptName = "SPSProfessional_Actions_CopyPaste";

                if (!clientScript.IsClientScriptBlockRegistered(scriptName))
                {
                    clientScript.RegisterClientScriptBlock(GetType(),
                                                              scriptName,
                                                              GetScriptForPictureLibraries(),
                                                              true);
                }

               ProcessPostBack();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Debug.Close();
#if (DEBUG1)
                fileStream.Close();
#endif
            }
        }

        /// <summary>
        /// Generate the Copy & Paste Javascript Actions
        /// </summary>
        /// <returns></returns>
        private string GetScriptForPictureLibraries()
        {
            string script = @"                            
                    function SPSProfessionalActionCopy(web,item)
                    {
                        var data = web+'|'+vCurrentListID+'|'+GetUrlKeyValue('RootFolder', true)+'|'+MakeSelectionStr();
                        window.clipboardData.setData('Text','ProCopyPic|'+data);
                    }
                    function SPSProfessionalActionCut(web)
                    {
                        var data = web+'|'+vCurrentListID+'|'+GetUrlKeyValue('RootFolder', true)+'|'+MakeSelectionStr();
                        window.clipboardData.setData('Text','ProCutPic|'+data);                        
                    }
                    function SPSProfessionalActionPaste()
                    {
                        __doPostBack('" + ID + "',window.clipboardData.getData('Text'));" +
                   "}";
            return script;
        }
    }
       
}