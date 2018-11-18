using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.WebParts.RollUp.Engine;

namespace SPSProfessional.SharePoint.WebParts.RollUp.ListInfo
{
    internal sealed class ListCollector
    {
        private readonly SPSCrawler _crawler;
        private Dictionary<string, ListInfo> _listsInfo;

        public ListCollector(SPSRollUpOptions options)
        {
            _crawler = new SPSCrawler(options.TopSite,
                                      options.ListsList,
                                      options.CamlQueryRecursive);
            _crawler.Collector += CollectData;
        }

        public IEnumerable<ListInfo> FieldsInfo
        {
            get { return _listsInfo.Values; }
        }

        public void Collect()
        {
            _listsInfo = new Dictionary<string, ListInfo>();
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
            
            if (!_listsInfo.ContainsKey(list.ParentWebUrl+"/"+list.Title))
            {
                _listsInfo.Add(list.ParentWebUrl + "/" + list.Title,
                                new ListInfo(list.Title,
                                            list.ParentWebUrl,
                                            list.BaseTemplate.ToString(),
                                            list.Description));
            }            
        }
    }
}