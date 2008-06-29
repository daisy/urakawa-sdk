using System;
using urakawa.core;

namespace urakawa.core.visitor
{
	/// <summary>
	/// Provides methods for accepting <see cref="ITreeNodeVisitor"/>s
	/// </summary>
	public interface IVisitableTreeNode
	{
    /// <summary>
    /// Accept a <see cref="ITreeNodeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ITreeNodeVisitor"/></param>
    void AcceptDepthFirst(ITreeNodeVisitor visitor);

    /// <summary>
    /// Accept a <see cref="ITreeNodeVisitor"/> in breadth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ITreeNodeVisitor"/></param>
    void AcceptBreadthFirst(ITreeNodeVisitor visitor);
	
		/// <summary>
		/// Visits the <see cref="IVisitableTreeNode"/> depth first
		/// </summary>
		/// <param name="preVisit">The pre-visit delegate</param>
		/// <param name="postVisit">The post visit delegate</param>
		void AcceptDepthFirst(PreVisitDelegate preVisit, PostVisitDelegate postVisit);
	}

	/// <summary>
	/// Delegate for pre-visit
	/// </summary>
	/// <param name="node">The <see cref="TreeNode"/> being visited</param>
	/// <returns>A <see cref="bool"/> indicating if the children of <paramref localName="node"/>
	/// should be visited</returns>
	public delegate bool PreVisitDelegate(TreeNode node);

	/// <summary>
	/// Delegate for post-visit
	/// </summary>
	/// <param name="node">The <see cref="TreeNode"/> being visited</param>
	public delegate void PostVisitDelegate(TreeNode node);
}
