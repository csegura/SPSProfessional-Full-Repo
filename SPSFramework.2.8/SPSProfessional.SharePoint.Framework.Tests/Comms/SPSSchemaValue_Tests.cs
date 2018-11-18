using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Comms;


namespace SPSProfessional.SharePoint.Framework.Tests.Comms
{
    [TestFixture]
    public class SPSSchemaValue_Tests
    {
        
        //public static IEnumerable<object[]> AllSystemTypesDefaultValues
        //{
        //    get { return TestData.AllSystemTypesDefaultValues; }
        //}

        [Test]
        public void AddData_Value()
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();
            schemaValue.AddParameter("Parameter", "System.Int32");
            schemaValue.AddDataValue("Parameter", 10);

            DataRowView drv = schemaValue.GetDataView();

            Assert.IsTrue((int) drv.Row["Parameter"] == 10);
        }

        [Test]
        public void AddDataValue()
        {
            foreach (KeyValuePair<Type, object> pair in TestData.AllSystemTypesDefaultValues())
            {
                AddDataValue_AllTypes(pair.Key,pair.Value);
            }
        }

        public void AddDataValue_AllTypes(Type type, object value)
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();
            schemaValue.AddParameter(type.Name, type);
            schemaValue.AddDataValue(type.Name, value);

            DataRowView drv = schemaValue.GetDataView();
            object o = drv.Row[type.Name];

            Assert.AreEqual(value, o);
            Assert.AreEqual(type, o.GetType());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddDataValue_NullParameter()
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();

            schemaValue.AddParameter("Parameter", "System.Int32");
            schemaValue.AddDataValue(null, 10);
        }

        [Test]
        public void AddDataValue_NullValue()
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();
            schemaValue.AddParameter("Parameter", "System.Int32");
            schemaValue.AddDataValue("Parameter", null);

            DataRowView drv = schemaValue.GetDataView();

            Assert.AreEqual(typeof(DBNull), drv.Row["Parameter"].GetType());
            Assert.IsTrue(drv.Row["Parameter"] == DBNull.Value);
        }

        [Test]
        public void AddDataValue_NullValueObject()
        {
            SPSSchemaValue schemaValue = new SPSSchemaValue();
            schemaValue.AddParameter("Parameter", "System.Int32");
            schemaValue.AddDataValue("Parameter", (object)null);

            DataRowView drv = schemaValue.GetDataView();

            Assert.AreEqual(typeof(DBNull), drv.Row["Parameter"].GetType());
            Assert.IsTrue(drv.Row["Parameter"] == DBNull.Value);
        }      
    }
}