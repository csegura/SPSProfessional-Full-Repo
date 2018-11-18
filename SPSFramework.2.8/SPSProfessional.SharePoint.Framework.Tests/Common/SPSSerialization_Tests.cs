using System.Collections.Generic;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Common;


namespace SPSProfessional.SharePoint.Framework.Tests.Common
{
    [TestFixture]
    public class SPSSerialization_Tests
    {
        [Test]
        public void SerializeDeserialize()
        {
            Dictionary<string, int> beforeDictionary = new Dictionary<string, int>();
            beforeDictionary.Add("Key1", 10);
            beforeDictionary.Add("Key2", 20);

            string serialized = SPSSerialization.Serialize(beforeDictionary);

            Dictionary<string, int> afterDictionary = (Dictionary<string, int>) SPSSerialization.Deserialize(serialized);

            Assert.IsTrue(beforeDictionary.Count == afterDictionary.Count);

            Assert.IsTrue(afterDictionary.ContainsKey("Key1"));
            Assert.IsTrue(afterDictionary.ContainsKey("Key2"));

            Assert.IsTrue(afterDictionary["Key1"] == 10);
            Assert.IsTrue(afterDictionary["Key2"] == 20);           
        }

        [Test]
        public void SerializeDeserializeGenerics()
        {
            Dictionary<string, int> beforeDictionary = new Dictionary<string, int>();
            beforeDictionary.Add("Key1", 10);
            beforeDictionary.Add("Key2", 20);

            string serialized = SPSSerialization.Serialize(beforeDictionary);

            Dictionary<string, int> afterDictionary = SPSSerialization.Deserialize<Dictionary<string, int>>(serialized);

            Assert.IsTrue(beforeDictionary.Count == afterDictionary.Count);

            Assert.IsTrue(afterDictionary.ContainsKey("Key1"));
            Assert.IsTrue(afterDictionary.ContainsKey("Key2"));

            Assert.IsTrue(afterDictionary["Key1"] == 10);
            Assert.IsTrue(afterDictionary["Key2"] == 20);
        }
    }
}