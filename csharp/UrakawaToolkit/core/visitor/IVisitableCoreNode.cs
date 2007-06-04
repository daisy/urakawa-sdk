using System;
using urakawa.core;

namespace urakawa.core.visitor
{
	/// <summary>
	/// Delegate for pre-visit
	/// </summary>
	/// <param name="node">The <see cref="CoreNode"/> being visited</param>
	/// <returns>A <see cref="bool"/> indicating if the children of <paramref localName="node"/>
	/// should be visited</returns>
	public delegate bool PreVisitDelegate(CoreNode node);

	/// <summary>
	/// Delegate for post-visit
	/// </summary>
	/// <param name="node">The <see cref="CoreNode"/> being visited</param>
	public delegate void PostVisitDelegate(CoreNode node);

	/// <summary>
	/// Provides methods for accepting <see cref="ICoreNodeVisitor"/>s
	/// </summary>
	public interface IVisitableCoreNode
	{
    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    void acceptDepthFirst(ICoreNodeVisitor visitor);

    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in breadth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    void acceptBreadthFirst(ICoreNodeVisitor visitor);
	
		/// <summary>
		/// Visits the <see cref="IVisitableCoreNode"/> depth first
		/// </summary>
		/// <param name="preVisit">The pre-visit delegate</param>
		/// <param name="postVisit">The post visit delegate</param>
		void acceptDepthFirst(PreVisitDelegate preVisit, PostVisitDelegate postVisit);
	}
}
