using System.Diagnostics;
using System.IO;
using System.Web.UI;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Controls;
using TypeMock;

namespace SPSProfessional.SharePoint.Framework.Tests.Controls
{
    [TestFixture]    
    public class SPSXsltControl_Tests
    {
        [Test]
        public void Constructors()
        {
            SPSXsltControl control = new SPSXsltControl();
            Assert.IsNotNull(control);
            Assert.IsFalse(control.DebugSource);
            Assert.IsFalse(control.DebugTransformation);
        }

        [Test]
        [Category("TypeMock")]
        public void Transform()
        {
            SPSKeyValueList list = new SPSKeyValueList();
            list.Add("key0", "value0");
            list.Add("key1", "value1");
            list.Add("key2", "value2");

            Mock<Page> mockPage = MockManager.MockObject<Page>(Constructor.Mocked);
            Mock<Control> mockControl = MockManager.MockObject<Control>(Constructor.Mocked);
            mockControl.MockedInstance.ID = "TestID";

            Mock<SPSXsltControl> xsltMockControl = MockManager.MockObject<SPSXsltControl>(Constructor.Mocked);
            xsltMockControl.ExpectGet("Page", mockPage.MockedInstance);
            xsltMockControl.ExpectGet("Parent", mockControl.MockedInstance);

            SPSXsltControl xsltControl = xsltMockControl.MockedInstance;
            Assert.IsNotNull(xsltControl);
            xsltControl.XmlData = TestData.GetResource("TestData.xml");
            xsltControl.Xsl = TestData.GetResource("Test.xslt");
            StringWriter transform = xsltControl.Transform();
            string output = transform.ToString();

            Debug.WriteLine(output);

            Assert.IsTrue(output.Contains("TEST"));
            Assert.IsTrue(output.Contains("1"));
            Assert.IsTrue(output.Contains("2"));

            MockManager.Verify();
        }
    }
}