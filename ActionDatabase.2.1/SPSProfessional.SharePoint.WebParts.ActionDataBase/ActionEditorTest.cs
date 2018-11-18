using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
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

    [DefaultProperty("CategoryFilter"),
     ToolboxData("<{0}:ActionEditor runat=server></{0}:ActionEditor>"),
     XmlRoot(Namespace = "SPSProfessional.SharePoint.WebParts.ActionDataBase")]
    public class ActionEditorTest : SPSWebPart, IPostBackEventHandler, IPostBackDataHandler
    {
        private SPSErrorBoxControl _actionEditorError;
        //private ActionEditorControl _actionEditorControl;
        private SPSActionEditConfig _config;
        private LinkButton _button;

        private bool _showExtendedErrors;
        private string _xmlConfig;

        private readonly SPSKeyValueList _parameterValues;

        #region Constructor

        public ActionEditorTest()
        {
            Debug.WriteLine("TEST Contructor");
            _parameterValues = new SPSKeyValueList();

            SPSInit("DD434B08-2854-4f29-81A7-44FC86E56886",
                    "ActionDataBase.1.0",
                    "WebParts ActionDataBase",
                    "http://www.spsprofessional.com");
            EditorParts.Add(new ActionEditorTestEditorPart());
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

        public override void DataBind()
        {
            Debug.WriteLine("TEST DataBind");
            base.DataBind();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            Debug.WriteLine("TEST CreateEditorParts");
            return base.CreateEditorParts();
        }

        protected override void OnInit(EventArgs e)
        {
            Debug.WriteLine("TEST OnInit");  
            base.OnInit(e);
        }

        protected override void LoadControlState(object savedState)
        {
            Debug.WriteLine("TEST LoadControlState");    
            base.LoadControlState(savedState);
        }

        protected override void LoadViewState(object savedState)
        {
            Debug.WriteLine("TEST LoadViewState");
            base.LoadViewState(savedState);
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("TEST OnLoad " + Title);
            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        protected override void CreateChildControls()
        {
            Debug.WriteLine("TEST: CreateChildControls - ReadConfig + ToolBar " + Title);

            try
            {
                _button = new LinkButton();
                _button.Text = "TEST";
                _button.ID = "button";
                
                //_button.Attributes.Add("onclick", "javascript:" + Page.ClientScript.GetPostBackEventReference(this, "TEST$TEST"));
                _button.OnClientClick = Page.ClientScript.GetPostBackEventReference(this, "TEST$TEST");
                _button.Attributes.Add("href", "#");
                Controls.Add(_button);

                // Is only necesary for Edit and New but send it always
                if (Page != null)
                {
                    Page.RegisterRequiresPostBack(this);
                }

                // The errorbox control
                _actionEditorError = new SPSErrorBoxControl
                                     {
                                             ShowExtendedErrors = _showExtendedErrors
                                             //ConfigErrors = Config.Errors
                                     };
                Controls.Add(_actionEditorError);

                // Create the form
                //_actionEditorControl = new ActionEditorControl(Config);
                //_actionEditorControl.OnError += TrapSubsystemError;
                //Controls.Add(_actionEditorControl);
            }
            catch (Exception ex)
            {
                TrapSubsystemError(this, new SPSErrorArgs("TEST", "CreateChildControls", ex));
                DumpException("CreateChildControls", ex);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Debug.WriteLine("TEST: OnPreRender - Create childs controls " + Title);

            foreach (SPSKeyValuePair pair in _parameterValues)
            {
                Debug.WriteLine(string.Format("{0} - {1}", pair.Key, pair.Value));
            }

            

            try
            {
                base.OnPreRender(e);
                //_actionEditorControl.ReadDataBase(_parameterValues);
            }
            catch (Exception ex)
            {
                TrapSubsystemError(this, new SPSErrorArgs("TEST", "OnPreRender", ex));
                DumpException("OnPreRender", ex);
            }
        }

        protected override object SaveControlState()
        {
            Debug.WriteLine("TEST SaveControlState");
            return base.SaveControlState();
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            Debug.WriteLine("TEST RenderBeginTag");
            base.RenderBeginTag(writer);
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            Debug.WriteLine("TEST RenderEndTag");
            base.RenderEndTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            Debug.WriteLine("TEST RenderContents");
            base.RenderContents(writer);
        }

        protected override object SaveViewState()
        {
            Debug.WriteLine("TEST SaveViewState");
            return base.SaveViewState();
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            Debug.WriteLine("TEST: Render " + Title);

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
                    // _actionEditorControl.RenderControl(writer);
                    _button.RenderControl(writer);
                    // Render error control
                    _actionEditorError.RenderControl(writer);
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs("TEST", "Render", ex));
                    DumpException("Render", ex);
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
            Debug.WriteLine("TEST ReadConfig");
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
                    DumpException("ReadConfig", ex);
                }
                catch (Exception ex)
                {
                    TrapSubsystemError(this, new SPSErrorArgs("ActionEditor", "Error in XML configuration", ex));
                    DumpException("ReadConfig", ex);
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
            _actionEditorError.AddError(args);
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
            Debug.WriteLine("TEST ConnectionParametersConsumer");

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
            Debug.WriteLine("TEST ConsumeParameterValues");

            if (parametersData != null)
            {
                Debug.WriteLine("Get Parameter Values");
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

        protected override void OnClosing(EventArgs e)
        {
            Debug.WriteLine("TEST OnClosing");
            base.OnClosing(e);
        }

        protected override void OnConnectModeChanged(EventArgs e)
        {
            Debug.WriteLine("TEST OnConnectModeChanged");
            base.OnConnectModeChanged(e);
        }

        protected override void OnDeleting(EventArgs e)
        {
            Debug.WriteLine("TEST OnDeleting");
            base.OnDeleting(e);
        }

        protected override void OnEditModeChanged(EventArgs e)
        {
            Debug.WriteLine("TEST OnEditModeChanged");
            base.OnEditModeChanged(e);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            Debug.WriteLine("TEST RaisePostBackEvent");
        }

        #region Implementation of IPostBackDataHandler

        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Debug.WriteLine("TEST LoadPostData");
            Debug.WriteLine("PostDataKey:" + postDataKey);
            Debug.WriteLine("postCollection:");
            foreach (string s in postCollection)
            {
                Debug.Write(s);
            }
            return true;
        }

        public void RaisePostDataChangedEvent()
        {
            Debug.WriteLine("TEST RaisePostDataChangedEvent");
        }

        #endregion
    }
}