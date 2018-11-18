using System.Collections;
using System.Collections.Generic;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.Framework.HierarchyOnDemand
{
    /// <summary>
    /// Hierarchy Iterator
    /// </summary>
    public class SPSHierarchyODIterator : IEnumerable<ISPSHierarchyNode>
    {
        private readonly SPSHierarchyODDataSource _dataSource;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SPSHierarchyIterator"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public SPSHierarchyODIterator(SPSHierarchyODDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ISPSHierarchyNode> GetEnumerator()
        {
            List<ISPSHierarchyNode> listNodes = new List<ISPSHierarchyNode>();

            if (_dataSource.Root != null)
            {
                foreach (ISPSTreeNode<ISPSHierarchyNode> node in _dataSource.Root.Children)
                {
                    listNodes.Add(node.Node);
                }
            }
            return listNodes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}