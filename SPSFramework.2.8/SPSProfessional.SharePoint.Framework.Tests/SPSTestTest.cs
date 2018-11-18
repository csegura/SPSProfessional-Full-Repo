using System;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.SharePoint;
using NUnit.Framework;


namespace UnitTests
{
    [TestFixture]
    [Category("Tests")]
    public class SPSTestTest : IDisposable
    {
        private readonly SPSTestEngine testEngine;

        public SPSTestTest()
        {
            testEngine = new SPSTestEngine();
            testEngine.UrlTestSite = "http://w2k3/Test/";
            testEngine.WebProvisioning("TestP1", "P1");
        }

        [Test]
        //[AssumeIdentity("Users")]
        public void GetCurrentUserName()
        {
            WindowsIdentity actualUser = WindowsIdentity.GetCurrent();
            WindowsIdentity newUser = new WindowsIdentity("W2K3\\DemoUser");


            WindowsImpersonationContext wic = newUser.Impersonate();
            try
            {
                Trace.WriteLine(WindowsIdentity.GetCurrent().Name);
            }
            finally
            {
                wic.Undo();
            }

            string expectedUser = newUser.Name; //WindowsIdentity.GetCurrent().Name;

            SPUser user = testEngine.GetWeb("TestP1").CurrentUser;
            string userName = string.Empty;

            //SPSecurity.RunWithElevatedPrivileges(delegate { userName = user.LoginName; }) ;

            userName = user.LoginName;

            Trace.WriteLine(string.Format("Expected: {0} User: {1}", expectedUser, userName));

            Assert.IsTrue(expectedUser.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public void Dispose()
        {
            testEngine.Dispose();
        }
    }
}