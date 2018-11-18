using System;
using System.Collections.ObjectModel;
using Microsoft.SharePoint.WebPartPages;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    [Serializable]
    internal class ActionFilterValues : IFilterValues
    {
        private readonly string _parameterName;
        private readonly ReadOnlyCollection<string> _parameterValues;

        public ActionFilterValues(string parameterName, ReadOnlyCollection<string> parameterValues)
        {
            _parameterName = parameterName;
            _parameterValues = parameterValues;
        }

        public ActionFilterValues(string parameterName, string parameterValue)
        {
            _parameterName = parameterName;
            _parameterValues = new ReadOnlyCollection<string>(new[] {parameterValue});
        }

        #region IFilterValues Members

        public void SetConsumerParameters(ReadOnlyCollection<ConsumerParameter> parameters)
        {
        }

        public string ParameterName
        {
            get { return _parameterName; }
        }

        public ReadOnlyCollection<string> ParameterValues
        {
            get { return _parameterValues; }
        }

        #endregion
    }

   

}