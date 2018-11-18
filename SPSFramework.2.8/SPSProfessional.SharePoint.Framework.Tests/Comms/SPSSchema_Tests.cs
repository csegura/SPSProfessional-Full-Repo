using System;
using System.Data;
using System.Diagnostics;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Comms;

namespace SPSProfessional.SharePoint.Framework.Tests.Comms
{
    [TestFixture]
    public class SPSSchema_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSSchema schema = new SPSSchema();
            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.Schema);
        }

        [Test]
        public void GetTypeFromSqlDbType_All()
        {
            foreach (object[] type in TestData.SqlDbTratedTypesCastSystemTypes)
            {
                AssertGetTypeFromSqlDbType((SqlDbType) type[0], (Type) type[1]);
            }
        }

        public void AssertGetTypeFromSqlDbType(SqlDbType sqlDbType, Type expectedType)
        {
            SPSSchema schema = new SPSSchema();

            Trace.WriteLine(string.Format("{0},{1}", expectedType, sqlDbType));

            Assert.IsTrue(expectedType == schema.GetSystemTypeFromSqlDbType_Test(sqlDbType.ToString()));
            Assert.IsTrue(expectedType == schema.GetSystemTypeFromSqlDbType_Test(sqlDbType));
        }

        [Test]
        public void GetTypeFromSqlDbType_InvalidTypes_All()
        {
            foreach (object[] type in TestData.SqlDbNoTratedTypes)
            {
                GetTypeFromSqlDbType_InvalidTypes((SqlDbType) type[0]);
            }
        }

        public void GetTypeFromSqlDbType_InvalidTypes(SqlDbType sqlDbType)
        {
            SPSSchema schema = new SPSSchema();
            Assert.IsNull(schema.GetSystemTypeFromSqlDbType_Test(sqlDbType.ToString()));
            Assert.IsNull(schema.GetSystemTypeFromSqlDbType_Test(sqlDbType));
        }


        [Test]
        public void AddParameterSql_All()
        {
            foreach (object[] type in TestData.SqlDbTratedTypes)
            {
                AssertAddParameterSql((SqlDbType) type[0]);
            }
        }

        public void AssertAddParameterSql(SqlDbType sqlDbType)
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameterSql(sqlDbType.ToString(), sqlDbType);
            Assert.IsTrue(schema.Schema.Count == 1);
            Trace.WriteLine(string.Format("{0}", schema.Schema[0].Name));
            Assert.IsTrue(schema.Schema[0].Name == sqlDbType.ToString());
            Assert.IsTrue(schema.Schema[0].PropertyType.Equals(schema.GetSystemTypeFromSqlDbType_Test(sqlDbType)));
        }

        [Test]
        public void AddParameterSql_StringType_All()
        {
            foreach (object[] type in TestData.SqlDbTratedTypes)
            {
                AssertAddParameterSql_StringType((SqlDbType) type[0]);
            }
        }

        public void AssertAddParameterSql_StringType(SqlDbType sqlDbType)
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameterSql(sqlDbType.ToString(), sqlDbType.ToString());
            Assert.IsTrue(schema.Schema.Count == 1);
            Trace.WriteLine(string.Format("{0}", schema.Schema[0].Name));
            Assert.IsTrue(schema.Schema[0].Name == sqlDbType.ToString());
            Assert.IsTrue(schema.Schema[0].PropertyType.Equals(schema.GetSystemTypeFromSqlDbType_Test(sqlDbType)));
        }

        [Test]
        public void AddParameterSql_NullParameter_All()
        {
            foreach (object[] type in TestData.SqlDbTratedTypes)
            {
                AddParameterSql_NullParameter((SqlDbType) type[0]);
            }
        }

        public void AddParameterSql_NullParameter(SqlDbType sqlDbType)
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameterSql(null, sqlDbType);
            Assert.IsTrue(schema.Schema.Count == 1);
            Trace.WriteLine(string.Format("{0}", schema.Schema[0].Name));
            Assert.IsTrue(schema.Schema[0].PropertyType.Equals(schema.GetSystemTypeFromSqlDbType_Test(sqlDbType)));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddParameterSql_BadType()
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameterSql("Test", "BadType");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddParameterSql_NullType()
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameterSql("Test", null);
        }

        [Test]
        //[ExpectedException(typeof(DuplicateNameException))]
        public void AddParameterSql_DuplicateType()
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameterSql("Test", SqlDbType.Int);
            schema.AddParameterSql("Test", SqlDbType.Int);
            Assert.AreEqual(1,schema.Schema.Count);
        }

        [Test]
        public void AddParameter_All()
        {
            foreach (object[] type in TestData.AllSystemTypes)
            {
                AddParameter((Type) type[0]);
            }
        }

        public void AddParameter(Type type)
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameter(type.ToString(), type);
            Assert.IsTrue(schema.Schema.Count == 1);
            Trace.WriteLine(string.Format("{0},{1},{2}", type, schema.Schema[0].Name, schema.Schema[0].PropertyType));
            Assert.IsTrue(schema.Schema[0].Name == type.ToString());
            Assert.IsTrue(schema.Schema[0].PropertyType.Equals(type));
        }

        [Test]
        public void AddParameter_StringType_All()
        {
            foreach (object[] type in TestData.AllSystemTypes)
            {
                AddParameter_StringType((Type) type[0]);
            }
        }

        public void AddParameter_StringType(Type type)
        {
            SPSSchema schema = new SPSSchema();
            schema.AddParameter(type.ToString(), type.ToString());
            Assert.IsTrue(schema.Schema.Count == 1);
            Trace.WriteLine(string.Format("{0},{1},{2}", type, schema.Schema[0].Name, schema.Schema[0].PropertyType));
            Assert.IsTrue(schema.Schema[0].Name == type.ToString());
            Assert.IsTrue(schema.Schema[0].PropertyType.Equals(type));
        }
    }
}