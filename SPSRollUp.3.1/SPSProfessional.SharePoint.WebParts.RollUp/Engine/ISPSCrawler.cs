namespace SPSProfessional.SharePoint.WebParts.RollUp.Engine
{
    internal interface ISPSCrawler
    {
        CrawlerCollector Collector
        {
            get;
            set;
        }

        /// <summary>
        /// Crawls this instance.
        /// </summary>
        /// <exception cref="SPSCrawlerException"><c>SPSCrawlerException</c>.</exception>
        void Crawl();
    }
}