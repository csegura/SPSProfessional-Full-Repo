using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Framework.Tests.Error
{
    [TestFixture]
    public class SPSStackErrors_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSStackErrors stackError = new SPSStackErrors();
            Assert.IsTrue(stackError.Count == 0);
        }
    }
}