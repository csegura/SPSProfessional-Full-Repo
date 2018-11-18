using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace SPSJobConfigurator
{
    public class JobSettings : SPPersistedObject
    {
        // Fields
        [Persisted]
        public List<Guid> SiteCollectionsEnabled;

        // Methods
        public JobSettings()
        {
            this.SiteCollectionsEnabled = new List<Guid>();
        }

        public JobSettings(string name, SPPersistedObject parent, Guid id)
            : base(name, parent, id)
        {
            this.SiteCollectionsEnabled = new List<Guid>();
        }
    }
}