using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.Framework.Tests.Hierarchy
{
    [TestFixture]
    public class SPSHierarchyNode_Tests
    {
        [Test]
        public void Contructor0()
        {
            Guid testGuid = Guid.NewGuid();
            SPSHierarchyNode node = new SPSHierarchyNode(SPSHierarchyNodeType.Web,
                                                         testGuid,
                                                         testGuid,
                                                         "name",
                                                         "openUrl",
                                                         "urlSegment",
                                                         "navigate",
                                                         "path",
                                                         "image",
                                                         false);
            Assert.IsNotNull(node);
            Assert.AreEqual(testGuid,node.WebID);
            Assert.AreEqual(testGuid,node.SiteID);
            Assert.AreEqual("name",node.Name);
            Assert.AreEqual("openUrl", node.OpenUrl);
            Assert.AreEqual("urlSegment", node.UrlSegment);
            Assert.AreEqual("navigate", node.NavigateUrl);
            Assert.AreEqual("path", node.Path);
            Assert.AreEqual("image", node.ImageUrl);
            Assert.AreEqual(false,node.HasChilds);
        }

        [Test]
        public void Contructor1()
        {
            Guid testGuid = Guid.NewGuid();
            SPSHierarchyNode node = new SPSHierarchyNode(SPSHierarchyNodeType.Web,
                                                         testGuid,
                                                         testGuid,
                                                         testGuid,
                                                         "name",
                                                         "openUrl",
                                                         "urlSegment",
                                                         "navigate",
                                                         "path",
                                                         "image",
                                                         false);
            Assert.IsNotNull(node);
            Assert.AreEqual(testGuid, node.WebID);
            Assert.AreEqual(testGuid, node.SiteID);
            Assert.AreEqual(testGuid, node.ListID);
            Assert.AreEqual("name", node.Name);
            Assert.AreEqual("openUrl", node.OpenUrl);
            Assert.AreEqual("urlSegment", node.UrlSegment);
            Assert.AreEqual("navigate", node.NavigateUrl);
            Assert.AreEqual("path", node.Path);
            Assert.AreEqual("image", node.ImageUrl);
            Assert.AreEqual(false, node.HasChilds);
        }

        [Test]
        public void Contructor3()
        {
            Guid testGuid = Guid.NewGuid();
            SPSHierarchyNode node = new SPSHierarchyNode(SPSHierarchyNodeType.Web,
                                                         testGuid,
                                                         testGuid,
                                                         testGuid,
                                                         testGuid,
                                                         "name",
                                                         "openUrl",
                                                         "urlSegment",
                                                         "navigate",
                                                         "path",
                                                         "image",
                                                         false);
            Assert.IsNotNull(node);
            Assert.AreEqual(testGuid, node.WebID);
            Assert.AreEqual(testGuid, node.SiteID);
            Assert.AreEqual(testGuid, node.ListID);
            Assert.AreEqual(testGuid, node.FolderID);
            Assert.AreEqual("name", node.Name);
            Assert.AreEqual("openUrl", node.OpenUrl);
            Assert.AreEqual("urlSegment", node.UrlSegment);
            Assert.AreEqual("navigate", node.NavigateUrl);
            Assert.AreEqual("path", node.Path);
            Assert.AreEqual("image", node.ImageUrl);
            Assert.AreEqual(false, node.HasChilds);
        }

    }
}
