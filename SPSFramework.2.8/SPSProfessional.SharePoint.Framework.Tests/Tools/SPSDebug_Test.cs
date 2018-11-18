using System;
using System.Diagnostics;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Tools;

namespace SPSProfessional.SharePoint.Framework.Tests.Tools
{
    public class SPSDebug_Test
    {
        [Test]
        [Conditional("DEBUG")]
        public void Dummy()
        {
            //SPSDebug.DumpException(new ArgumentNullException());
            //SPSDebug.DumpException("Null", new InvalidOperationException());
            //Assert.IsTrue(true);
        }
    }
}