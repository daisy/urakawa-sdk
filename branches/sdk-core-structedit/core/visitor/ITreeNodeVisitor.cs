namespace urakawa.core.visitor
{
    /// <summary>
    /// Interface for a visitor accepted by <see cref="TreeNode"/>s
    /// </summary>
    public interface ITreeNodeVisitor
    {
        /// <summary>
        /// Called before visiting the child <see cref="TreeNode"/>s
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> currently being visited</param>
        /// <returns>A <see cref="bool"/> indicating if the child <see cref="TreeNode"/>s should be visited:
        /// If <c>true</c> is returned, the children are visited, 
        /// if <c>false</c> is returned, the children are not visited</returns>
        bool PreVisit(TreeNode node);

        /// <summary>
        /// Called after visiting the child <see cref="TreeNode"/>s
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> currently being visited</param>
        void PostVisit(TreeNode node);
    }
}