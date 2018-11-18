using System;
using System.Globalization;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Common;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Tools;
using TypeMock;

namespace SPSProfessional.SharePoint.Framework.Tests.Tools
{

    [TestFixture]
    [Category("TypeMock")]
    public class SPSXsltExtension_Test
    {
        private readonly SPSXsltExtension _extension;
        
       
        public SPSXsltExtension_Test()
        {            
            SPSKeyValueList list = new SPSKeyValueList();
            list.Add("key0", "value0");
            list.Add("key1", "value1");
            list.Add("key2", "value2");

            Mock<Page> mockPage = MockManager.MockObject<Page>(Constructor.Mocked);
            SPSXsltCommandControl xsltCommandControl = new SPSXsltCommandControl
                                                           {
                                                                   ID = "TestID"
                                                           };
            _extension = new SPSXsltExtension(xsltCommandControl, mockPage.MockedInstance, list);
                        
            MockManager.Verify();
        }

        

        [Test]        
        public void EventTest()
        {
            string result = _extension.Event("name", "parameters");
            Assert.IsTrue(result.Contains("name:parameters"));
        }

        [Test]
        public void EventControlIDTest()
        {
            string result = _extension.Event("name", "parameters");
            Assert.IsTrue(result.Contains("TestID"));
        }

        [Test]
        public void EventJSTest()
        {
            string result = _extension.EventJS("name", "parameters");
            Assert.IsTrue(result.Contains("'name:'+parameters"));
        }

        [Test]
        public void EcmaScriptEncodeTest()
        {
            string expected = "http://test";
            string result = _extension.EcmaScriptEncode(expected);
            Assert.AreEqual(result, SPHttpUtility.EcmaScriptStringLiteralEncode(expected));
        }

        [Test]
        public void LastRowTest()
        {
            string result = _extension.LastRow("key1");
            Assert.IsTrue(result == "value1");
        }

        [Test]
        public void LastRowNoKeyTest()
        {
            string result = _extension.LastRow("key5");
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [Test]
        public void iifTrueTest()
        {
            string result = _extension.iif(1 == 1, "true", "false");
            Assert.IsTrue(result == "true");
        }

        [Test]
        public void iifFalseTest()
        {
            string result = _extension.iif(1 == 0, "true", "false");
            Assert.IsTrue(result == "false");
        }


        //[Test]
        //public void MapToIconTest()
        //{
        //    using (RecordExpectations rec = new RecordExpectations())
        //    {                
        //        SPWeb web = RecorderManager.CreateMockedObject<SPWeb>(Constructor.Mocked);
        //        rec.ExpectAndReturn(SPContext.Current.Web, web).RepeatAlways();
        //    }

        //    string result = _extension.MapToIcon("test.xls", "xls");
        //    Assert.AreEqual("xls", result);
        //}

        [Test]
        public void DateDiffTest()
        {
            DateTime dateNow = DateTime.Now;
            DateTime testYear = dateNow.AddYears(1);
            DateTime testMonth = dateNow.AddMonths(1);
            DateTime testDay = dateNow.AddDays(1);
            DateTime testHour = dateNow.AddHours(1);
            DateTime testMinute = dateNow.AddMinutes(1);
            DateTime testSeconds = dateNow.AddSeconds(1);

            string result;
            
            result = _extension.DateDiff("Y", dateNow, testYear);
            Assert.AreEqual("1",result);

            result = _extension.DateDiff("MM", dateNow, testMonth);
            Assert.AreEqual("1", result);

            result = _extension.DateDiff("D", dateNow, testDay);
            Assert.AreEqual("1", result);

            result = _extension.DateDiff("H", dateNow, testHour);
            Assert.AreEqual("1", result);

            result = _extension.DateDiff("M", dateNow, testHour);
            Assert.AreEqual("60", result);

            result = _extension.DateDiff("S", dateNow, testHour);
            Assert.AreEqual("3600", result);

            result = _extension.DateDiff("M", dateNow, testMinute);
            Assert.AreEqual("1", result);

            result = _extension.DateDiff("S", dateNow, testMinute);
            Assert.AreEqual("60", result);

            result = _extension.DateDiff("S", dateNow, testSeconds);
            Assert.AreEqual("1", result);


        }

        [Test]
        public void GetFcColorTest()
        {
            Assert.IsTrue(_extension.GetFcColor() != _extension.GetFcColor());
        }

        [Test]
        public void GetCalColorTest()
        {
            Assert.IsTrue(_extension.GetCalColor() != _extension.GetCalColor());
        }

        [Test]
        public void CounterTest()
        {
            _extension.Counter();
            string counter = _extension.Counter();
            Assert.IsTrue(counter == "2");
        }

        [Test]
        public void FormatDateTimeTest()
        {
            DateTime dateTime = DateTime.Now;
            string result = _extension.FormatDateTime(dateTime.ToString(), 1033, "s");
            Assert.IsTrue(result == dateTime.ToString("s", CultureInfo.GetCultureInfo(1033)));

            result = _extension.FormatDateTime(dateTime.ToString(), 3082, "s");
            Assert.IsTrue(result == dateTime.ToString("s", CultureInfo.GetCultureInfo(3082)));

        }

        [Test]
        public void FormatDateTime_EnglishDatesTest()
        {
            DateTime dateTime = DateTime.Parse(DateTime.Now.ToString(CultureInfo.GetCultureInfo(2057)));
            string result = _extension.FormatDateTime(dateTime.ToString(), 2057, "s");
            Assert.IsTrue(result == dateTime.ToString("s", CultureInfo.GetCultureInfo(2057)));

            result = _extension.FormatDateTime(dateTime.ToString(), 3082, "s");
            Assert.IsTrue(result == dateTime.ToString("s", CultureInfo.GetCultureInfo(3082)));
        }

        [Test]
        public void FormatIsoDateTimeTest()
        {
            DateTime dateTime = DateTime.Now;
            string result = _extension.FormatIsoDateTime(dateTime.ToString("s"), 1033, "s");
            Assert.IsTrue(result == dateTime.ToString("s", CultureInfo.GetCultureInfo(1033)));

            result = _extension.FormatIsoDateTime(dateTime.ToString("s"), 3082, "s");
            Assert.IsTrue(result == dateTime.ToString("s", CultureInfo.GetCultureInfo(3082)));
        }

        [Test]
        public void GetFileExtensionTest()
        {
            string result;

            result = _extension.GetFileExtension("Test.gif");
            Assert.IsTrue(result == "gif");
            result = _extension.GetFileExtension("Test.xlsl");
            Assert.IsTrue(result == "xlsl");
            result = _extension.GetFileExtension("c:\\test\\test test\\Test.xlsl");
            Assert.IsTrue(result == "xlsl");        
        }

        [Test]
        public void GetStringBeforeSeparatorTest()
        {
            string result;

            result = _extension.GetStringBeforeSeparator("one;#dos");
            Assert.IsTrue(result == "one");
        }

        [Test]
        public void IfNewTest()
        {
            Assert.IsTrue(_extension.IfNew(DateTime.Now.ToString()));
            Assert.IsFalse(_extension.IfNew(DateTime.Now.AddDays(-3).ToString()));
        }

        [Test]
        public void LimitTest()
        {
            string result = _extension.Limit(new String('A', 50), 25, "end");
            string expected = new String('A', 25) + "end";
            Assert.IsTrue(result == expected);
        }

        //[Test]
        //public void MaxTest()
        //{
        //}

        //[Test]
        //public void MinTest()
        //{
        //}

        //[Test]
        //public void TodayTest()
        //{
        //}

        //[Test]
        //public void TodayIsoTest()
        //{
        //}

        //[Test]
        //public void UrlBaseNameTest()
        //{
        //}

        //[Test]
        //public void UrlDirNameTest()
        //{
        //}

        //[Test]
        //public void UrlEncodeTest()
        //{
        //}

        //[Test]
        //void GetTypeTest()
        //{
        //}

        //[Test]
        //public void ToStringTest()
        //{
        //}

        //[Test]
        //public void EqualsTest()
        //{
        //}

        //[Test]
        //public void GetHashCodeTest()
        //{
        //}
    }
}