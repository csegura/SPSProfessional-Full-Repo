using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Framework.Tests.Error
{
    [TestFixture]
    public class SPSConfigErrorCollection_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSConfigErrorCollection configErrors = new SPSConfigErrorCollection();
            Assert.IsTrue(configErrors.Count == 0);
        }

        [Test]
        public void Add()
        {
            SPSConfigErrorCollection configErrors = new SPSConfigErrorCollection();
            SPSConfigError configError = new SPSConfigError();
            configError.Message = "Message";
            configError.Match = "Error*";
            configError.Color = "#ff0000";
            configErrors.Add(configError);
            configErrors.Add();

            Assert.IsTrue(configErrors.Count == 2);
            Assert.AreEqual(configErrors[0], configError);
        }

        [Test]
        public void Remove()
        {
            SPSConfigErrorCollection configErrors = new SPSConfigErrorCollection();
            SPSConfigError configError = new SPSConfigError();
            configError.Message = "Message";
            configError.Match = "Error*";
            configError.Color = "#ff0000";

            configErrors.Add(configError);
            configErrors.Remove(configError);

            Assert.IsTrue(configErrors.Count == 0);
        }

        [Test]
        public void Insert()
        {
            SPSConfigErrorCollection configErrors = new SPSConfigErrorCollection();
            SPSConfigError configError = new SPSConfigError();
            configError.Message = "Message";
            configError.Match = "Error*";
            configError.Color = "#ff0000";

            configErrors.Add(null);
            configErrors.Insert(0, configError);

            Assert.IsTrue(configErrors.Count == 2);
            Assert.IsTrue(configErrors[0].Message == "Message");
        }
    }
}