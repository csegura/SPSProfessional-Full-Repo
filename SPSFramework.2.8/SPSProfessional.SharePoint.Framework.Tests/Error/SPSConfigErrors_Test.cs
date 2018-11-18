using System.Collections.Generic;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Error;

namespace SPSProfessional.SharePoint.Framework.Tests.Error
{
    [TestFixture]
    public class SPSConfigErrors_Test
    {
        [Test]
        public void Constructor()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();
            Assert.IsTrue(configErrors.Count == 0);            
        }

        [Test]
        public void Add()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();
            SPSConfigError configError = new SPSConfigError();
            configError.Message = "Message";
            configError.Match = "Error*";
            configError.Color = "#ff0000";
            configErrors.Add(configError);

            Assert.IsTrue(configErrors.Count == 1);
            Assert.AreEqual(configErrors[0], configError);
        }

        [Test]
        public void AddNull()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();
            configErrors.Add(null);

            Assert.IsTrue(configErrors.Count == 1);            
        }


        [Test]
        public void AddNull_AddsEmpty()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();
            configErrors.Add(null);

            Assert.IsTrue(string.IsNullOrEmpty(configErrors[0].Message));
            Assert.IsTrue(string.IsNullOrEmpty(configErrors[0].Match));
            Assert.IsTrue(configErrors[0].Color == "#ff0000");
        }

        [Test]
        public void Iterator()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();
            SPSConfigError configError = new SPSConfigError();
            configError.Message = "Message";
            configErrors.Add(configError);
            configErrors.Add(configError);
            configErrors.Add(configError);

            List<SPSConfigError> errors = new List<SPSConfigError>();

            foreach (SPSConfigError error in configErrors)
            {
                errors.Add(error);
            }

            Assert.IsTrue(errors.Count == 3);
        }

        [Test]
        public void Remove()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();
            configErrors.Add(null);
            configErrors.Add(null);
            configErrors.Add(null);

            configErrors.Remove(0);
            configErrors.Remove(0);
            configErrors.Remove(0);

            Assert.IsTrue(configErrors.Count == 0);
        }

        [Test]
        public void Clear()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();
            configErrors.Add(null);
            configErrors.Add(null);
            configErrors.Add(null);

            configErrors.Clear();

            Assert.IsTrue(configErrors.Count == 0);
        }

        [Test]
        public void ToStringTest()
        {
            SPSConfigErrors configErrors = new SPSConfigErrors();

            Assert.IsFalse(string.IsNullOrEmpty(configErrors.ToString()));
        }
    }
}
