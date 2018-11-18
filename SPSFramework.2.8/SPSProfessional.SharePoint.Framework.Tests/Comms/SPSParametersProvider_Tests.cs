using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Comms;

namespace SPSProfessional.SharePoint.Framework.Tests.Comms
{
    [TestFixture]
    public class SPSParametersProvider_Tests
    {
        private Dictionary<object,object> _dictionary;
        private bool _callbackInvoked;
        private readonly SPSSchema schema;

        public SPSParametersProvider_Tests()
        {
            schema = new SPSSchema();
            schema.AddParameter("Test1", typeof(string));
            schema.AddParameter("Test2", typeof(int));  
        }

        [Test]
        public void SetConsumerSchemaNullArgs()
        {
            SPSParametersProvider p = new SPSParametersProvider();
            p.SetConsumerSchema(null);
            p.GetParametersData(GetParameterDataCallback);
            Assert.IsTrue(_callbackInvoked);
            Assert.IsTrue(_dictionary.Count == 0);
        }

        [Test]
        public void SetConsumerSchema()
        {
            SPSParametersProvider p = new SPSParametersProvider();

            p.SetConsumerSchema(schema.Schema);
            Assert.AreEqual(typeof(PropertyDescriptorCollection),p.Schema.GetType());
        }

        [Test]
        public void GetParametersData()
        {
            // Prepair the provider
            SPSParametersProvider p = new SPSParametersProvider();            
            p.AddParameter("Test1", typeof(string));
            p.AddParameter("Test2", typeof(int));
          
            p.SetConsumerSchema(schema.Schema);

            _callbackInvoked = false;
            p.GetParametersData(GetParameterDataCallback);
            
            Assert.IsTrue(_callbackInvoked);

            Assert.IsTrue(_dictionary.ContainsKey("Test1"));
            Assert.IsTrue(_dictionary.ContainsKey("Test2"));
        }

        private void GetParameterDataCallback(IDictionary parametersData)
        {
            _callbackInvoked = true;
            _dictionary = new Dictionary<object, object>();

            foreach (DictionaryEntry entry in parametersData)
            {
                _dictionary.Add(entry.Key,entry.Value);
            }
        }

        [Test]
        public void ToStringTest()
        {
            SPSParametersProvider p = new SPSParametersProvider();

            p.SetConsumerSchema(schema.Schema);

            Assert.IsNotNull(p.ToString());
        }
       
    }
}
