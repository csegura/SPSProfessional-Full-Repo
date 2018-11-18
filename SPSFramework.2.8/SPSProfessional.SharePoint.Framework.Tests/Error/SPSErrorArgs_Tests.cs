using System;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Framework.Tests.Error
{
    [TestFixture]
    public class SPSErrorArgs_Tests
    {
        [Test]
        public void Constructor()
        {
            ArgumentException ex = new ArgumentException("ArgumentExcptionMessage", "test");
            SPSErrorArgs args = new SPSErrorArgs("Subsystem", "User Message", ex);

            Assert.IsFalse(args.OnlyDevelopper);
        }

        [Test]
        public void ConstructorBadParams()
        {
            ArgumentException ex = new ArgumentException("ArgumentExcptionMessage", "Test");
            SPSErrorArgs args = new SPSErrorArgs(null, null, null);

            Assert.IsNull(args.InternalException);
        }

        [Test]
        public void ConstructorDevelopper()
        {
            ArgumentException ex = new ArgumentException("ArgumentExcptionMessage", "Test");
            SPSErrorArgs args = new SPSErrorArgs("Subsystem", "User Message", ex, true);

            Assert.IsTrue(args.OnlyDevelopper);
        }

        [Test]
        public void Use()
        {
            ArgumentException ex = new ArgumentException("ArgumentExcptionMessage", "Test");
            SPSErrorArgs args = new SPSErrorArgs("Subsystem", "User Message", ex, true);

            Assert.IsTrue(args.OnlyDevelopper);
            Assert.IsTrue(args.InternalException.Equals(ex));
            Assert.IsTrue(args.Subsystem.Equals("Subsystem"));
            Assert.IsTrue(args.UserMessage.Equals("User Message"));
            Assert.AreEqual(typeof(ArgumentException), args.InternalException.GetType());
        }
    }
}