using System;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Common;

namespace SPSProfessional.SharePoint.Framework.Tests.Common
{
    [TestFixture]
    public class SPSKeyValuePair_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSKeyValuePair keyValue = new SPSKeyValuePair("Key", "Value");
            Assert.IsTrue(keyValue.Key == "Key");
            Assert.IsTrue(keyValue.Value == "Value");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullKey()
        {
            SPSKeyValuePair pair = new SPSKeyValuePair(null, "Value");                          
        }

        [Test]
        public void ConstructorWithNullValue()
        {
            SPSKeyValuePair keyValue = new SPSKeyValuePair("Key", null);
            Assert.IsTrue(keyValue.Key == "Key");
            Assert.IsTrue(keyValue.Value == null);
        }

        [Test]
        public void Use()
        {
            SPSKeyValuePair keyValue = new SPSKeyValuePair("Key", null);

            keyValue.Key = "KeyA";
            keyValue.Value = "Value";

            Assert.IsTrue(keyValue.Key == "KeyA");
            Assert.IsTrue(keyValue.Value == "Value");
        }
    }
}