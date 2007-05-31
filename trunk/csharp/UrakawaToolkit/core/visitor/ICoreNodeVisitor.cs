using System;
using urakawa.core;

namespace urakawa.core.visitor
{
	/// <summary>
	/// Interface for a visitor accepted by <see cref="CoreNode"/>s
	/// </summary>
	public interface ICoreNodeVisitor
	{
    /// <summary>
    /// Called before visiting the child <see cref="CoreNode"/>s
    /// </summary>
    /// <param name="node">The <see cref="CoreNode"/> currently being visited</param>
    /// <returns>A <see cref="bool"/> indicating if the child <see cref="CoreNode"/>s should be visited:
    /// If <c>true</c> is returned, the children are visited, 
    /// if <c>false</c> is returned, the children are not visited</returns>
    bool preVisit(CoreNode node);
    /// <summary>
    /// Called after visiting the child <see cref="CoreNode"/>s
    /// </summary>
    /// <param name="node">The <see cref="CoreNode"/> currently being visited</param>
    void postVisit(CoreNode node);
	}
}
