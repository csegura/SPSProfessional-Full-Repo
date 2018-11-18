using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.WebParts.RollUp.Engine
{
    public delegate void CrawlerCollector(SPWeb web, SPList list);

    internal class SPSCrawler : ISPSCrawler
    {
        private readonly string _topUrl;
        private readonly IList<string> _lists;
        private readonly bool _recursive;

        public bool Processed
        {
            get;
            private set;
        }

        public CrawlerCollector Collector
        {
            get;
            set;
        }

        public SPSCrawler(string topUrl, IList<string> lists, bool recursive)
        {
            _recursive = recursive;
            _lists = lists;
            _topUrl = topUrl;
        }

        /// <summary>
        /// Crawls this instance.
        /// </summary>
        /// <exception cref="SPSCrawlerException"><c>SPSCrawlerException</c>.</exception>
        public void Crawl()
        {
            // Select top web or current
            using(SPWeb web = GetTopWeb())
            {
                try
                {
                    // Filter we only need lists
                    var dataFilter = new SPSHierarchyFilter
                                     {
                                             SortHierarchy = false,
                                             IncludeLists = true,
                                             IncludeWebs = _recursive,
                                             IncludeFolders = false
                                     };

                    dataFilter.OnFilter += InvokeCollector;

                    // DataSource
                    using(var dataSource = new SPSHierarchyDataSource(web))
                    {
                        dataSource.Filter = dataFilter;
                        // get all elements . on filter does the rollup
                        dataSource.GetAll();
                    }

                    Processed = true;
                }
                catch (SPException ex)
                {
                    throw new SPSCrawlerException(ex);
                }
            }
        }

        /// <summary>
        /// Gets the top web.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SPSCrawlerException"><c>SPSCrawlerException</c>.</exception>
        private SPWeb GetTopWeb()
        {
            SPWeb web;
            // TODO
            if (!string.IsNullOrEmpty(_topUrl))
            {
                try
                {
                    using(var site = new SPSite(_topUrl))
                    {
                        var uri = new Uri(_topUrl, UriKind.RelativeOrAbsolute);
                        web = site.OpenWeb(uri.LocalPath);
                    }
                }
                catch (Exception ex)
                {
                    throw new SPSCrawlerException(ex);
                }                
            }
            else
            {
                web = SPContext.Current.Web.Site.OpenWeb();
            }

            return web;
        }

        /// <summary>
        /// Data Source Filter
        /// Check if the list needed matches our selection and is added to the data source
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <returns>True if pass the filter</returns>
        private bool InvokeCollector(object sender, SPSHierarchyFilterArgs args)
        {
            if (args.List != null)
            {
                bool include = FilterListTitle(args.List.Title);

                if (include)
                {
                    Collector(args.Web, args.List);
                }

                return include;
            }
            return true;
        }

        /// <summary>
        /// Filters the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        private bool FilterListTitle(string title)
        {
            foreach (string list in _lists)
            {
                if (title.Contains(list))
                {
                    return true;
                }
            }
            return false;
        }
    }
}