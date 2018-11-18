using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Comms;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.RollUp
{
    /// <summary>
    /// CamlPreprocessor
    /// Evaluate CAML queries using functions defined in SPSEvaluator using the next syntax
    /// [Variable:Function(Parameter)]
    /// _parameterFormulas      - contains the variables and the functions (all variables are here)
    /// _variableValues     - contains the variables and the values from consummer connection
    /// </summary>
    internal class CamlPreprocessor
    {

        #region Regex to parse syntax

        private const string CAMLVariable = "Variable";
        private const string CAMLFunction = "Function";
        /// <summary>
        ///  A description of the regular expression:
        ///  
        ///  Literal [
        ///  [Variable]: A named capture group. [\w+]
        ///      Alphanumeric, one or more repetitions
        ///  :
        ///  [Function]: A named capture group. [\w+]
        ///      Alphanumeric, one or more repetitions
        ///  Literal (
        ///  [Parameter]: A named capture group. [\w+]
        ///      Alphanumeric, one or more repetitions
        ///  \)\]
        ///      Literal )
        ///      Literal ]
        /// </summary>
        private const string CAMLVariableRegex = @"\[(?<Variable>\w+):(?:(?<Function>\w+(\(.*\)))|(?<Function>\w*))\]";
        #endregion

        private readonly string _camlQuery;
        private readonly SPSKeyValueList _variableValues;
        private readonly SPSKeyValueList _parameterFormulas;
        private readonly Regex _regex;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CamlPreprocessor"/> class.
        /// </summary>
        /// <param name="camlQuery">The caml query.</param>
        public CamlPreprocessor(string camlQuery)
        {
            _regex = new Regex(CAMLVariableRegex,
                              RegexOptions.Multiline
                              | RegexOptions.IgnoreCase
                              | RegexOptions.CultureInvariant
                              | RegexOptions.IgnorePatternWhitespace
                              | RegexOptions.Compiled);

            _camlQuery = camlQuery;
            _variableValues = new SPSKeyValueList();
            _parameterFormulas = new SPSKeyValueList();
            ParseParametersFormulas();
        }

        #endregion

        //#region Public properties

        ///// <summary>
        ///// Gets the variables values.
        ///// </summary>
        ///// <value>The variables values.</value>
        //public SPSKeyValueList VariablesValues
        //{
        //    get { return _variableValues; }
        //}

        //#endregion

        #region Public methods

        /// <summary>
        /// Gets the Schema.
        /// </summary>
        /// <returns>
        /// Describing the variables in the query
        /// </returns>
        public PropertyDescriptorCollection GetSchema()
        {
            ParseParametersFormulas();

            SPSSchema schema = new SPSSchema();

            foreach (SPSKeyValuePair keyValuePair in _parameterFormulas)
            {
                schema.AddParameter(keyValuePair.Key, typeof (string));
            }

            return schema.Schema;
        }

        /// <summary>
        /// The query processed
        /// </summary>
        /// <returns></returns>
        public string Evaluate()
        {
            if (!string.IsNullOrEmpty(_camlQuery))
            {
                EvaluateParameters();
                return ReplaceParameters();
            }
            
            return null;
        }

        /// <summary>
        /// Clears the variable values.
        /// </summary>
        public void ClearVariableValues()
        {
            _variableValues.Clear();
        }

        /// <summary>
        /// Initializes the variable values.
        /// </summary>
        /// <param name="keyValueList">The key value list.</param>
        public void InitializeVariableValues(SPSKeyValueList keyValueList)
        {
            ClearVariableValues();
            keyValueList.ForEach(pair => _variableValues.Add(pair));
        }

        /// <summary>
        /// Adds the variable.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="value">The value.</param>
        public void AddVariable(string variable, string value)
        {
            _variableValues.Add(variable, value);
        }

        /// <summary>
        /// Adds the variable.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        public void AddVariable(SPSKeyValuePair keyValuePair)
        {
            _variableValues.Add(keyValuePair);
        }

        /// <summary>
        /// Copies the variable values.
        /// </summary>
        /// <param name="keyValueList">The key value list.</param>
        public void CopyVariableValues(SPSKeyValueList keyValueList)
        {
            keyValueList.Clear();
            _variableValues.ForEach(pair => keyValueList.Add(pair));
        }

        public void AddValues(IDictionary parametersData)
        {
            foreach (string key in parametersData.Keys)
            {
                if (!string.IsNullOrEmpty(parametersData[key].ToString()))
                {
                    _variableValues.Add(key, parametersData[key].ToString());
                }

                Debug.WriteLine("*Parameter: " + key + " - " + parametersData[key]);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>A list with parameters and values or functions</returns>
        private void ParseParametersFormulas()
        {
            if (_camlQuery != null)
            {
                _parameterFormulas.Clear();

                MatchCollection ms = _regex.Matches(_camlQuery);

                foreach (Match match in ms)
                {
                    Debug.WriteLine(string.Format("Parameter - {0} - {1}",
                                                  match.Groups[CAMLVariable].Value,
                                                  match.Groups[CAMLFunction].Value));

                    _parameterFormulas.Add(match.Groups[CAMLVariable].Value,
                                           match.Groups[CAMLFunction].Value);
                }
            }
        }

        /// <summary>
        /// Evaluates the parameters.
        /// 1.- Process _parameterFormulas list evaluating each function
        /// 2.- Process _parameterFormulas list replacing the evaluated 
        /// formula with a value thats come from _variableValues list
        /// </summary>
        private void EvaluateParameters()
        {
            EvaluateFormulas();
            ReplaceValues();
        }

        /// <summary>
        /// Replaces the values.
        /// </summary>
        private void ReplaceValues()
        {
            // Replace with Consumer
            foreach (SPSKeyValuePair keyValue in _variableValues)
            {
                Debug.WriteLine("3-CAML Provider - " + keyValue.Key + " " + keyValue.Value);
                _parameterFormulas.ReplaceValue(keyValue.Key, keyValue.Value);
            }
        }

        /// <summary>
        /// Evaluates the formulas.
        /// </summary>
        private void EvaluateFormulas()
        {
            SPSEvaluator evaluator = new SPSEvaluator();

            // Evaluate all
            foreach (SPSKeyValuePair keyValue in _parameterFormulas)
            {
                Debug.WriteLine("1-CAML Original - " + keyValue.Key + " " + keyValue.Value);
                keyValue.Value = evaluator.Evaluate(keyValue.Value);
                Debug.WriteLine("2-CAML Evaluated - " + keyValue.Key + " " + keyValue.Value);
            }
        }

        /// <summary>
        /// Replaces the parameters with the values
        /// </summary>
        /// <returns>The final caml query</returns>
        private string ReplaceParameters()
        {
            return Regex.Replace(_camlQuery, 
                                 CAMLVariableRegex,
                                 new MatchEvaluator(InternalReplace));
        }

        /// <summary>
        /// Internals the replace each variable with your value
        /// </summary>
        /// <param name="m">The variable that match</param>
        /// <returns>The value</returns>
        private string InternalReplace(Match m)
        {
            string variable = m.Groups[CAMLVariable].Value;
            return _parameterFormulas[variable].Trim();
        }

        #endregion
    }
}