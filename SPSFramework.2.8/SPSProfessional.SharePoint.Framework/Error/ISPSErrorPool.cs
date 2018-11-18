namespace SPSProfessional.SharePoint.Framework.Error
{
    /// <summary>
    /// All clases that can treat SPSErrorArgs
    /// </summary>
    public interface ISPSErrorPool
    {
        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }

        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <param name="reportedError">The reported error.</param>
        void AddError(SPSErrorArgs reportedError);
    }
}