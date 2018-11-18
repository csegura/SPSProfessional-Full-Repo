using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls.WebParts;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Comms;

namespace SPSProfessional.SharePoint.Framework.Tests.Comms
{
    [TestFixture]
    public class SPSRowProvider_Tests
    {
        private bool testCallBackTwoInvoked;

        public string TestParameter
        {
            get { return "Test"; }
        }

        [Test]
        public void Constructor_FromSPSSchemaValueBuilder()
        {
            SPSSchemaValue schema = new SPSSchemaValue();

            schema.AddParameter("TestParameter", "System.String");
            schema.AddDataValue("TestParameter", "Test");

            SPSRowProvider rowProvider = new SPSRowProvider(schema.GetDataView());
            IWebPartRow provider = rowProvider;

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(SPSRowProvider_Tests));

            Assert.IsTrue(provider.Schema[0].Name == pdc[0].Name);
            Assert.IsTrue(provider.Schema[0].PropertyType.Equals(pdc[0].PropertyType));
        }

        [Test]
        public void Constructor_FromDataTable()
        {
           
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("TestParameter", typeof(string));
            dataTable.Rows.Add(dataTable.NewRow());
           
            SPSRowProvider rowProvider = new SPSRowProvider(dataTable);
            IWebPartRow provider = rowProvider;

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(SPSRowProvider_Tests));

            Assert.IsTrue(provider.Schema[0].Name == pdc[0].Name);
            Assert.IsTrue(provider.Schema[0].PropertyType.Equals(pdc[0].PropertyType));
        }

        [Test]
        public void Callback()
        {
            testCallBackTwoInvoked = false;

            SPSSchemaValue schema = new SPSSchemaValue();

            schema.AddParameter("TestParameter", "System.String");
            schema.AddDataValue("TestParameter", "TestModified");

            SPSRowProvider rowProvider = new SPSRowProvider(schema.GetDataView());
            IWebPartRow provider = rowProvider;
            
            provider.GetRowData(TestCallback);
            rowProvider.SendRow();

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(SPSRowProvider_Tests));

            Assert.IsTrue(schema.Schema[0].Name == pdc[0].Name);
            Assert.IsTrue(schema.Schema[0].PropertyType.Equals(pdc[0].PropertyType));
            Assert.IsTrue(testCallBackTwoInvoked);
        }

        private void TestCallback(object rowData)
        {
            DataRowView rowView = (DataRowView)rowData;
            Assert.IsTrue(rowView["TestParameter"].ToString() == "TestModified");
            testCallBackTwoInvoked = true;
        }

        [Test]
        public void Callback_WhitNullRowView()
        {
            testCallBackTwoInvoked = false;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("TestParameter", typeof(string));
            dataTable.Rows.Add(dataTable.NewRow());

            SPSRowProvider rowProvider = new SPSRowProvider();
            IWebPartRow provider = rowProvider;

            provider.GetRowData(TestCallback);

            dataTable.Rows[0]["TestParameter"] = "TestModified";
            
            rowProvider.RowView = dataTable.DefaultView[0];

            rowProvider.SendRow();

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(SPSRowProvider_Tests));

            Assert.IsTrue(provider.Schema[0].Name == pdc[0].Name);
            Assert.IsTrue(provider.Schema[0].PropertyType.Equals(pdc[0].PropertyType));
            Assert.IsTrue(testCallBackTwoInvoked);
        }
      

        private void TestCallbackTwo(object rowData)
        {            
            DataRowView rowView = (DataRowView)rowData;

            if (testCallBackTwoInvoked)
            {
                Assert.IsTrue(rowView["TestParameter"].Equals("TestModified"));
            }
            else
            {
                Assert.AreEqual(typeof(string), rowView["TestParameter"].GetType());
                Assert.IsTrue(rowView["TestParameter"].Equals(DBNull.Value));
                testCallBackTwoInvoked = true;
            }
        }
    }
}