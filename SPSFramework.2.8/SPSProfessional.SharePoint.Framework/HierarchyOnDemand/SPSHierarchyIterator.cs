using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.HierarchyOnDemand;

namespace SPSProfessional.SharePoint.Framework.HierarchyOnDemand
{
    /// <summary>
    /// Walk delegate
    /// </summary>
    //public delegate void SPSHierarchyIteratorFunc(ISPSTreeNode<ISPSHierarchyNode> node, int deep);
    public delegate void SPSHierarchyIteratorFunc(ISPSHierarchyNode node, int deep);
}