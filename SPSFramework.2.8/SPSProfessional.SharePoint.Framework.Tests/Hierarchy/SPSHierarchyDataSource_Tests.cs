using System;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.Framework.Tests.Hierarchy
{
    [TestFixture]
    public class SPSHierarchyDataSource_Tests
    {
        [Test]
        public void Constructor0()
        {
            SPSHierarchyDataSource dataSource = new SPSHierarchyDataSource(null);
            Assert.IsNotNull(dataSource);
        }

        [Test]
        public void Constructor1()
        {
            SPSHierarchyDataSource dataSource = new SPSHierarchyDataSource(null, null);
            Assert.IsNotNull(dataSource);
        }

        [Test]
        public void CacheService()
        {
            SPSHierarchyDataSource dataSource = new SPSHierarchyDataSource(null,null);
            dataSource.CacheService = new SPSCacheService(1,"test");
            Assert.IsNotNull(dataSource.CacheService);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void Root()
        {
            SPSHierarchyDataSource dataSource = new SPSHierarchyDataSource(null, null);            
            Assert.IsNull(dataSource.Root);
        }


        [Test]
        public void GetFilter()
        {
            SPSHierarchyDataSource dataSource = new SPSHierarchyDataSource(null, null);
            Assert.IsNotNull(dataSource.Filter);
        }

        [Test]
        public void SetFilter()
        {
            SPSHierarchyDataSource dataSource = new SPSHierarchyDataSource(null, null);
            dataSource.Filter = new SPSHierarchyFilter();
            Assert.IsNotNull(dataSource.Filter);
        }


    }
}
