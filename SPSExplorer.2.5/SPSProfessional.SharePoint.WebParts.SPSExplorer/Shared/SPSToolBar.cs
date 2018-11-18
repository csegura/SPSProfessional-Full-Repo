using System;
using System.Diagnostics;
using System.Web.UI;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.WebParts.SPSExplorer
{
    /// <summary>
    /// Esta clase emula el control ToolBar de SharePoint
    /// El control ToolBar solo se puede inicializar usando LoadControl lo cual es un problema
    /// por temas de seguridad.
    /// 
    /// Para emular el comportamiento de ToolBar, este control implementa los mismos controles 
    /// que tieneel template, al no inicializarse estos mediante el template coreccto el metodo
    /// FixControls, se encarga de hacer las correcciones pertinentes como asignar la Page y 
    /// hackear los controles.
    /// </summary>
    public class SPSToolBar : Control
    {
        private readonly RepeatedControls _rptControls;
        private readonly RepeatedControls _rightRptControls;
        private string _tempTemplateId;

        #region Properties

        public RepeatedControls Buttons
        {
            get { return _rptControls; }
        }

        public RepeatedControls RightButtons
        {
            get { return _rightRptControls; }
        }

        public string CssClass
        {
            get
            {
                object cssClass = ViewState["CssClass"];
                if (cssClass == null)
                {
                    return "ms-toolbar";
                }
                return (string) cssClass;
            }
            set { ViewState["CssClass"] = value; }
        }
        #endregion

        public SPSToolBar()
        {
            _rptControls = new RepeatedControls();
            _rightRptControls = new RepeatedControls();
            _tempTemplateId = string.Empty;
            CssClass = "ms-menutoolbar";
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// Asiganmos las mismas propiedades que tendría el control si se hubiera 
        /// inicializado desde .LoadControl("~/_controltemplates/ToolBar.ascx")
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _rptControls.BeforeControlHtml = "<td class='ms-toolbar' nowrap='true'>";
            _rptControls.AfterControlHtml = "</td>";
            _rptControls.SeparatorHtml = "<td class=ms-separator>|</td>";
            _rptControls.ID = ClientID + ClientIDSeparator + "RptControls";

            _rightRptControls.BeforeControlHtml = _rptControls.BeforeControlHtml;
            _rightRptControls.AfterControlHtml = _rptControls.AfterControlHtml;
            _rightRptControls.SeparatorHtml = _rptControls.SeparatorHtml;
            _rightRptControls.ID = ClientID + ClientIDSeparator + "RightControls";
            ID = "toolBarTbl";
        }


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data. </param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            FixControls(_rptControls.Controls);
            FixControls(_rightRptControls.Controls);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);

            writer.WriteLine(
                    "<TABLE class=\"{0}\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" id=\"{1}\" width=\"100%\">",
                    CssClass,
                    ClientID);
            writer.WriteLine("<TR>");

            _rptControls.RenderControl(writer);

            writer.WriteLine("<TD width=\"99%\" class=\"{0}\" nowrap>", CssClass);
            writer.WriteLine("<IMG src=\"/_layouts/images/blank.gif\" alt=\"\">");
            writer.WriteLine("</TD>");

            _rightRptControls.RenderControl(writer);

            writer.WriteLine("</TR>");
            writer.WriteLine("</TABLE>");
        }

        /// <summary>
        /// Fixes the controls.
        /// </summary>
        /// <param name="controls">The controls.</param>
        private void FixControls(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
#if DEBUG1
                Debug.WriteLine(control.GetType().Name + " [" + control.ClientID + "]");
#endif
                // Asigna la pagina correcta
                // al no haberse inicializado desde el template
                if (control.Page == null)
                {
                    control.Page = Page;
                }

                // Necesitamos el ClientID para poder hacer que el control funcione
                if (control is FeatureMenuTemplate)
                {
                    _tempTemplateId = control.ClientID;
#if DEBUG1
                    Debug.WriteLine(" TemplateID " + _tempTemplateId);
#endif
                }

                // A los menus les asignamos el ClientID de la feature
                if (control is Menu)
                {
#if DEBUG1
                    Debug.WriteLine(control.ID + " is menu " + ClientID);
                    Debug.WriteLine(control.ID + " is menu " + _tempTemplateId);
#endif
                    ((Menu) control).TemplateId = _tempTemplateId;
                }

                FixControls(control.Controls);
            }
        }

        /// <summary>
        /// Finds the control recursive.
        /// </summary>
        /// <param name="control">The control1.</param>
        /// <param name="controlId">The id control.</param>
        /// <returns></returns>
        private Control FindControlRecursive(Control control, string controlId)
        {
            foreach (Control subControl in control.Controls)
            {
#if DEBUG
                Debug.WriteLine("-> " + subControl.GetType());
                Debug.WriteLine("   " + subControl.UniqueID);
                Debug.WriteLine("   " + subControl.ID);
#endif

                Control targetControl = null;

                if (subControl.ID == controlId)
                {
                    targetControl = subControl;
                }
                else
                {
                    targetControl = FindControlRecursive(subControl, controlId);
                }

                if (targetControl != null)
                {
                    return targetControl;
                }
            }

            return null;
        }
    }
}