using System;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Framework.Tests.Error
{
    [TestFixture]
    public class SPSException_Test
    {

        [Test]
        [ExpectedException(typeof(SPSException))]
        public void Constructor()
        {
            throw new SPSException();
        }

        [Test]
        public void Constructor2()
        {
            SPSException ex = new SPSException("Message");
            Assert.AreEqual(ex.Message, "Message");
        }

        [Test]
        public void Constructor3()
        {
            SPSException ex = new SPSException("Message", new ArgumentNullException());
            Assert.AreEqual(ex.InnerException.GetType(), typeof(ArgumentNullException));
        }
    }
}