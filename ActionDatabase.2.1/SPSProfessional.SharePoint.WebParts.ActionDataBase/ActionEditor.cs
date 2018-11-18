using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Comms;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Error;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.ActionEditorConfig;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    /*
   Web Part Life Cycle on page load (

   1. OnInit                    - Read XML configuration
   2. OnLoad
   3. Connection Consuming
   4. CreateChildControls
   5. OnPreRender
   6. Render (RenderContents, etc)

   Web Part Life Cycle on button click (handle all modes Edit / Update / New / Create / View)

   1. OnInit                    - Read XML configuration
   2. CreateChildControls       - We cannot create form child controls here, the first time
                                  we need the connection consume value
                                - ToolBar generation
                                - Form geration
   3. OnLoad                   
   4. Click Event Handling      - RaisePostBackEvent (get the new mode)
   5. Connection Consuming      - Now we can get a value
   6. OnPreRender               - CreateChildControls 
   7. RenderContents            - Render 
   */

    [DefaultProperty("CategoryFilter")]
    [ToolboxData("<{0}:ActionEditor runat=server></{0}:ActionEditor>")]
    [XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.ActionDataBase")]
    public class ActionEditor : SPSWebPart
    {
        private SPSErrorBoxControl _actionEditorError;
        private ActionEditorControl _actionEditorControl;
        private SPSActionEditConfig _config;

        private bool _showExtendedErrors;
        private string _xmlConfig;

        private readonly SPSKeyValueList _parameterValues;

        #region Constructor

        public ActionEditor()
        {
            Debug.WriteLine("ActionEditor");
            _parameterValues = new SPSKeyValueList();

            SPSInit("DD434B08-2854-4f29-81A7-44FC86E56886",
                    "ActionDataBase.1.0",
                    "WebParts ActionDataBase",
                    "http://www.spsprofessional.com");

            EditorParts.Add(new ActionEditorEditorPart());
        }

        #endregion

        #region WebPart Properties

        //[Personalizable(PersonalizationScope.Shared)]
        public string XmlConfig
        {
            get { return _xmlConfig; }
            set { _xmlConfig = value; }
        }

        //[Personalizable(PersonalizationScope.Shared)]
        public bool ShowExtendedErrors
        {
            get { return _showExtendedErrors; }
            set { _showExtendedErrors = value; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The config.</value>
        public SPSActionEditConfig Config
        {
            get
            {
                if (_config == null)
                {
                    ReadConfig();
                }
                return _config;
            }
        }

        #endregion

        #region Control Methods

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad " + Title);
            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            Debug.WriteLine("ActionEdior: CreateChildControls - ReadConfig + ToolBar " + Title);

            try
            {
                // The errorbox control
                _actionEditorError = new SPSErrorBoxControl
                                     {
                                             ShowExtendedErrors = _showExtendedErrors,
                                             ConfigErrors = Config.Errors
                                     };
                Controls.Add(_actionEditorError);

                // Create the form
                _actionEditorControl = new ActionEditorControl(Config);
                _actionEditorControl.OnError += TrapSubsystemError;
                Controls.Add(_actionEditorControl);
            }
            catch (Exception ex)
            {
                TrapSubsystemError(this, new SPSErrorArgs("ActionEditor", "CreateChildControls", ex));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Debug.WriteLine("ActionEditor: OnPreRender " + Title);

            try
            {
                base.OnPreRender(e);
                // we can read the database now because we have the
                // filter values 
                // TODO : ver si podemos pasar esto al actioneditorcontrol
                // TODO : y que el AEC lea la BBDD en el prerender
                //_actionEditorControl.ReadDataBase(_parameterValues);
                Debug.WriteLine("******************************** Send Filter Values");
                _actionEditorControl.SetFilterValues(_parameterValues);
            }
            catch (Exception ex)
            {
                TrapSubsystemError(this, new SPSErrorArgs("ActionEditor", "OnPreRender", ex));
            }
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            Debug.WriteLine("ActionEditor: Render " + Title);

            if (Config == null)
            {
                writer.WriteLine(MissingConfiguration);
            }
            else
            {
                try
                {
                    // Force child controls again
                    EnsureChildControls();

                    // finally render the form view
                    _actionEditorControl.RenderControl(writer);

                    // Render error control
                    _actionEditorError.RenderControl(writer);
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs("ActionEditor", "Render", ex));
                }
            }
        }

        #endregion

        #region Engine (see Controller)

        /// <summary>
        /// Deserialize the configuration XML
        /// </summary>
        private void ReadConfig()
        {
            if (!string.IsNullOrEmpty(XmlConfig))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SPSActionEditConfig));
                    StringReader stringReader = new StringReader(XmlConfig);
                    _config = (SPSActionEditConfig) serializer.Deserialize(stringReader);
                    stringReader.Close();
                }
                catch (InvalidOperationException ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs("ActionEditor", ex.Message, ex));
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs("ActionEditor", "Error in XML configuration", ex));
                }
            }
            else
            {
                Debug.WriteLine("ReadConfig - NO XMLCONFIG");
            }
        }    

        /// <summary>
        /// Traps the error messages from subsystems.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void TrapSubsystemError(object sender, SPSErrorArgs args)
        {
            if (_actionEditorError != null)
            {
                _actionEditorError.AddError(args);
            }
            DumpException(args.InternalException.TargetSite.Name, args.InternalException);
        }

        #endregion

        #region Parameters Consumer - Connection Point

        /// <summary>
        /// Connection point for consume parameters
        /// Make a schema of necesary parameters based on IdentityColumnCollection
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        [ConnectionConsumer("Key Parameters", "ActionEditorParametersConsumer", AllowsMultipleConnections = true)]
        public void ConnectionParametersConsumer(IWebPartParameters parameters)
        {
            if (parameters != null && Config != null)
            {
                SPSSchema schemaBuilder = new SPSSchema();

                // Add each necesary parameter 
                foreach (IdentityColumn column in Config.DataBase.Table.IdentityColumnCollection)
                {
                    schemaBuilder.AddParameterSql(column.Name, column.Type.ToString());
                }

                // Set the schema
                parameters.SetConsumerSchema(schemaBuilder.Schema);

                // The get parameters callback
                parameters.GetParametersData(ConsumeParameterValues);
            }
        }

        /// <summary>
        /// Consumes the parameter values.
        /// </summary>
        /// <param name="parametersData">The parameters data.</param>
        public void ConsumeParameterValues(IDictionary parametersData)
        {
            Debug.WriteLine("ConsumeParameterValues");

            if (parametersData != null)
            {
                foreach (Object key in parametersData.Keys)
                {
                    _parameterValues.Add(key.ToString(), parametersData[key].ToString());

                    Debug.WriteLine("Parameter: " + key + " - " + parametersData[key]);
                }
            }
        }

        #endregion

        #region DEBUG

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        #endregion
    }
}