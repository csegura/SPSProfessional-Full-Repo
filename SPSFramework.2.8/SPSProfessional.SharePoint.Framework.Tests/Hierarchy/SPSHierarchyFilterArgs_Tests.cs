using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.Framework.Tests.Hierarchy
{
    [TestFixture]
    public class SPSHierarchyFilterArgs_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSHierarchyFilterArgs filterArgs = new SPSHierarchyFilterArgs(null,null,null);
            Assert.IsNotNull(filterArgs);
            Assert.IsNull(filterArgs.Web);
            Assert.IsNull(filterArgs.List);
            Assert.IsNull(filterArgs.Folder);
        }
    }
}
