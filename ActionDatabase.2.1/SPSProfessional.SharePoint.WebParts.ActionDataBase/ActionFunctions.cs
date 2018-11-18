using System;
using System.Diagnostics;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    internal class ActionFunctions
    {
        public string Evaluate(string value)
        {
            SPSEvaluator evaluator = new SPSEvaluator();
            string result = string.Empty;
            try
            {
                result = evaluator.Evaluate(value);
            }
            catch(SPSEvaluatorException ex)
            {
                DumpException("Evaluator",ex);
            }
            return result;
        }

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }
    }
}