using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Common;


namespace SPSProfessional.SharePoint.Framework.Tests.Common
{
    [TestFixture]
    public class SPSKeyvalueList_Tests
    {

        [Test]
        public void Constructor()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();
            Assert.IsTrue(keyValues.Count == 0);
        }

        [Test]
        public void Add()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();
            keyValues.Add("Key", "Value");
            Assert.IsTrue(keyValues.Count == 1);
            Assert.IsTrue(keyValues[0].Key == "Key");
            Assert.IsTrue(keyValues[0].Value == "Value");
        }

        [Test]
        public void AddKeyValuePair()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();

            keyValues.Add(new SPSKeyValuePair("Key", "Value"));
            keyValues.Add(new SPSKeyValuePair("Key", "Value"));


            Assert.IsTrue(keyValues.Count == 2);

            Assert.IsTrue(keyValues[0].Key == "Key");
            Assert.IsTrue(keyValues[0].Value == "Value");
            Assert.IsTrue(keyValues[1].Key == "Key");
            Assert.IsTrue(keyValues[1].Value == "Value");
        }

        [Test]
        public void ReplaceValue()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();

            keyValues.Add(new SPSKeyValuePair("Key", "Value"));
            keyValues.Add(new SPSKeyValuePair("Key", "Value"));

            Assert.IsTrue(keyValues.Count == 2);

            keyValues.ReplaceValue("Key", "Value1");

            Assert.IsTrue(keyValues[0].Key == "Key");
            Assert.IsTrue(keyValues[0].Value == "Value1");
            Assert.IsTrue(keyValues[1].Key == "Key");
            Assert.IsTrue(keyValues[1].Value == "Value");
        }
 
        [Test]
        public void Contains()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();

            keyValues.Add(new SPSKeyValuePair("Key", "Value"));
            keyValues.Add(new SPSKeyValuePair("Key", "Value"));

            Assert.IsTrue(keyValues.Contains("Key"));
            Assert.IsFalse(keyValues.Contains("OtherKey"));
        }

        [Test]
        public void Index()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();

            keyValues.Add(new SPSKeyValuePair("Key", "Value"));
            keyValues.Add(new SPSKeyValuePair("Key1", "Value1"));

            Assert.IsTrue(keyValues["Key"] == "Value");
            Assert.IsTrue(keyValues["Key1"] == "Value1");
        }

        [Test]
        public void ReplaceAllValues()
        {
            SPSKeyValueList keyValues = new SPSKeyValueList();

            keyValues.Add(new SPSKeyValuePair("Key", "Value"));
            keyValues.Add(new SPSKeyValuePair("Key", "Value"));

            keyValues.ReplaceAllValues("Key","NewValue");

            Assert.IsTrue(keyValues[0].Value == "NewValue");
            Assert.IsTrue(keyValues[1].Value == "NewValue");
        }

        [Test]
        public void SerializeDeserialize()
        {
            SPSKeyValueList keyValuesBefore = new SPSKeyValueList();

            SPSKeyValuePair keyValue0 = new SPSKeyValuePair("KeyA", "Value1");
            SPSKeyValuePair keyValue1 = new SPSKeyValuePair("KeyB", "Value2");

            keyValuesBefore.Add(keyValue0);
            keyValuesBefore.Add(keyValue1);

            string serialized = SPSKeyValueList.Serialize(keyValuesBefore);

            SPSKeyValueList keyValuesAfter = SPSKeyValueList.Deserialize(serialized);

            Assert.IsTrue(keyValuesAfter.Count == keyValuesBefore.Count);

            Assert.IsTrue(keyValuesAfter.Count == keyValuesBefore.Count);

            Assert.IsTrue(keyValuesAfter.Contains("KeyA"));
            Assert.IsTrue(keyValuesAfter.Contains("KeyB"));

            Assert.IsTrue(keyValuesAfter[0].Value == "Value1");
            Assert.IsTrue(keyValuesAfter[1].Value == "Value2");
        }

        public static string SerializeToBase64(object data)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            stream.Position = 0;
            return Convert.ToBase64String(stream.ToArray());
        }

        private object DeserializeFromBase64(string data)
        {
            MemoryStream stream =
                    new MemoryStream(Convert.FromBase64String(data));
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;

            stream.Position = 0;
            return formatter.Deserialize(stream);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Looking for:" + args.Name);
            return null;
        }
    }
}