using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.Framework.Tests.Hierarchy
{
    [TestFixture]
    public class SPSTreeNode_Tests
    {
        [Test]
        public void Constructor()
        {
            SPSTreeNode<string> rootNode = new SPSTreeNode<string>("root");
            Assert.IsNotNull(rootNode);
            Assert.IsTrue(rootNode.Node == "root");
        }

        [Test]
        public void Add()
        {
            SPSTreeNode<string> rootNode = new SPSTreeNode<string>("root");
            ISPSTreeNode<string> child1 = rootNode.Add("child1");
            ISPSTreeNode<string> child2 = rootNode.Add("child2");
            
            Assert.AreSame(rootNode,child1.Parent);
            Assert.AreSame(rootNode,child2.Parent);
            Assert.IsTrue(rootNode.Children.Count == 2);
        }

        [Test]
        public void AddNull()
        {
            SPSTreeNode<string> rootNode = new SPSTreeNode<string>("root");
            ISPSTreeNode<string> child1 = rootNode.Add(null);

            Assert.IsNull(child1.Node);
        }

        [Test]
        public void Deep()
        {
            SPSTreeNode<string> rootNode = new SPSTreeNode<string>("root");
            ISPSTreeNode<string> child1 = rootNode.Add("child1");            
            ISPSTreeNode<string> child2 = child1.Add("child2");
            ISPSTreeNode<string> child11 = rootNode.Add("child11");

            Assert.IsTrue(rootNode.Deep == 0);
            Assert.IsTrue(child1.Deep == 1);
            Assert.IsTrue(child11.Deep == 1);
            Assert.IsTrue(child2.Deep == 2);
        }


    }
}
