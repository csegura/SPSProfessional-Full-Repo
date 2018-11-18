using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.Framework.Tests.Hierarchy
{
    [TestFixture]
    public class SPSHierarchyFilter_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSHierarchyFilter filter = new SPSHierarchyFilter();

            Assert.IsNotNull(filter);

            Assert.IsTrue(filter.MaxDeepth == 99);
            Assert.IsTrue(filter.HideUnderscoreFolders);
            Assert.IsTrue(filter.IncludeLists);
            Assert.IsTrue(filter.Recursive);
            Assert.IsFalse(filter.SortHierarchy);
            Assert.IsTrue(filter.IncludeWebs);
            Assert.IsFalse(filter.IncludeItems);
            Assert.IsFalse(filter.IncludeFolders);
            Assert.IsFalse(filter.IncludeHiddenLists);
            Assert.IsFalse(filter.IncludeHiddenFolders);
            Assert.IsFalse(filter.IncludeNumberOfFiles);
        }


        [Test]
        public void Properties()
        {
            SPSHierarchyFilter filter = new SPSHierarchyFilter();

            Assert.IsNotNull(filter);

            filter.HideUnderscoreFolders = false;
            filter.IncludeLists = false;
            filter.Recursive = false;
            filter.SortHierarchy = true;
            filter.IncludeWebs = true;
            filter.IncludeItems = true;
            filter.IncludeFolders = true;
            filter.IncludeHiddenLists = true;
            filter.IncludeHiddenFolders = true;
            filter.IncludeNumberOfFiles = true;
            filter.MaxDeepth = 0;

            Assert.IsFalse(filter.HideUnderscoreFolders);
            Assert.IsFalse(filter.IncludeLists);
            Assert.IsFalse(filter.Recursive);
            Assert.IsTrue(filter.SortHierarchy);
            Assert.IsTrue(filter.IncludeWebs);
            Assert.IsTrue(filter.IncludeItems);
            Assert.IsTrue(filter.IncludeFolders);
            Assert.IsTrue(filter.IncludeHiddenLists);
            Assert.IsTrue(filter.IncludeHiddenFolders);
            Assert.IsTrue(filter.IncludeHiddenFolders);
            Assert.IsTrue(filter.IncludeNumberOfFiles);
            Assert.IsTrue(filter.MaxDeepth == 0);
        }
    }
}
