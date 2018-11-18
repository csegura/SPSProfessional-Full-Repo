namespace SPSProfessional.SharePoint.Framework.Hierarchy
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISPSNode<T>
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        T Item { get; }
    }
}
