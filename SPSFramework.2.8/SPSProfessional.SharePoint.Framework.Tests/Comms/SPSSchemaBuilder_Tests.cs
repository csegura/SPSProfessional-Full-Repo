using NUnit.Framework;

namespace SPSProfessional.SharePoint.Framework.Tests.Comms
{
    [TestFixture]
    public class SPSSchemaBuilder_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSSchemaBuilderImplementation schemaBuilder;

            using (schemaBuilder = new SPSSchemaBuilderImplementation())
            {
                Assert.IsNotNull(schemaBuilder);
                Assert.IsNotNull(schemaBuilder.Schema);
                Assert.IsNotNull(schemaBuilder.GetDataView());
            }
        }       
    }
}