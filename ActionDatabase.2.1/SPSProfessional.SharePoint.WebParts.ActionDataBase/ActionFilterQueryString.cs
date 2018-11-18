using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Comms;
using SPSProfessional.SharePoint.Framework.Controls;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    public class ActionFilterQueryString : SPSWebPart
    {
        private string _queryParameters;
        private string _defaultParameters;
        private SPSRowProvider _rowProvider;

        #region Properties

        //[Personalizable(PersonalizationScope.Shared)]
        public string QueryParameters
        {
            get { return _queryParameters; }
            set { _queryParameters = value; }
        }

        //[Personalizable(PersonalizationScope.Shared)]
        public string DefaultParameters
        {
            get { return _defaultParameters; }
            set { _defaultParameters = value; }
        }

        #endregion

        #region Constructor

        public ActionFilterQueryString()
        {
            _queryParameters = string.Empty;
            _defaultParameters = string.Empty;

            SPSInit("DD434B08-2854-4f29-81A7-44FC86E56886",
                    "ActionDataBase.1.0",
                    "WebParts ActionDataBase",
                    "http://www.spsprofessional.com");

            EditorParts.Add(new ActionFilterQueryStringEditorPart());
        }

        #endregion

        #region Row Provider - Connection Point

        /// <summary>
        /// Gets the connection interface. (Row Provider)
        /// </summary>
        /// <returns></returns>
        [ConnectionProvider("Parameters Row", "ActionFilterQueryStringRowProvider", AllowsMultipleConnections = true)]
        public IWebPartRow ConnectionRowProvider()
        {
            // Using our special SPSRowProvider class
            _rowProvider = new SPSRowProvider(GetRowViewForProvider());
            return _rowProvider;
        }

        /// <summary>
        /// Gets the row view for provider.
        /// </summary>
        /// <returns></returns>
        private DataRowView GetRowViewForProvider()
        {
            SPSSchemaValue schemaValueBuilder = new SPSSchemaValue();

            string[] parameters = QueryParameters.Split(',');
            string[] values = DefaultParameters.Split(',');

            if (parameters.Length > 0 && !string.IsNullOrEmpty(QueryParameters))
            {
                // Generate Schema
                foreach (string parameter in parameters)
                {
                    schemaValueBuilder.AddParameter(parameter, typeof (string));
                }

                // Generate Data
                if (Page != null)
                {
                    int parameterCounter = 0;
                    foreach (string parameter in parameters)
                    {
                        string value = Page.Request.QueryString.Get(parameter);

                        if (!string.IsNullOrEmpty(value))
                        {
                            schemaValueBuilder.AddDataValue(parameter, value);
                        }
                        else if (values.Length >= parameterCounter)
                        {
                            schemaValueBuilder.AddDataValue(parameter, values[parameterCounter]);
                        }
                        parameterCounter++;
                    }
                }
            }

            return schemaValueBuilder.GetDataView();
        }

        #endregion

        #region SPSWebPart

        protected override void SPSRender(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(QueryParameters))
            {
                writer.WriteLine(MissingConfiguration);
            }
        }

        #endregion
    }
}