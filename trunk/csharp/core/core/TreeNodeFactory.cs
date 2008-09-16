namespace urakawa.core
{
    /// <summary>
    /// Default implementation of <see cref="TreeNodeFactory"/>.
    /// Creates <see cref="TreeNode"/>s belonging to a specific <see cref="Presentation"/>
    /// </summary>
    /// <remarks>
    /// A <see cref="TreeNodeFactory"/> can not create <see cref="TreeNode"/>s
    /// until it has been associated with a <see cref="Presentation"/> using the
    /// <see cref="WithPresentation.Presentation"/> method
    /// </remarks>
    public sealed class TreeNodeFactory : GenericFactory<TreeNode>
    {
        /// <summary>
        /// Creates a new <see cref="TreeNode"/>
        /// </summary>
        /// <returns>The new <see cref="TreeNode"/></returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> of the 
        /// </exception>
        public TreeNode Create()
        {
            return Create<TreeNode>();
        }
    }
}