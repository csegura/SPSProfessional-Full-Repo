using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp.FieldInfo
{
    internal sealed class FieldCollector
    {
        private readonly ISPSCrawler _crawler;
        private Dictionary<string, FieldInfo> _fieldsInfo;

        public FieldCollector(SPSRollUpOptions options)
        {
            _crawler = new SPSCrawler(options.TopSite,
                                      options.ListsList,
                                      options.CamlQueryRecursive);
            _crawler.Collector += CollectData;
        }

        public IEnumerable<FieldInfo> FieldsInfo
        {
            get { return _fieldsInfo.Values; }
        }

        public void Collect()
        {
            _fieldsInfo = new Dictionary<string, FieldInfo>();
            _crawler.Crawl();
        }

        /// <summary>
        /// Collectors the specified web.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        /// <exception cref="SPSDataCollectorException"><c>SPSDataCollectorException</c>.</exception>
        private void CollectData(SPWeb web, SPList list)
        {
            Debug.WriteLine(string.Format("CollectData {0} - {1}", web.Title, list.Title));
            Debug.Assert(list != null);

            foreach (SPField field in list.Fields)
            {
                if (!_fieldsInfo.ContainsKey(field.InternalName))
                {
                    _fieldsInfo.Add(field.InternalName,
                                    new FieldInfo(field.Title,
                                                  field.InternalName,
                                                  field.TypeAsString,
                                                  field.Description));
                }
            }
        }
    }
}