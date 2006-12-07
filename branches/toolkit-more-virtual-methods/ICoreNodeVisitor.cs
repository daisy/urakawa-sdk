using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for a visitor accepted by <see cref="ICoreNode"/>s
	/// </summary>
	public interface ICoreNodeVisitor
	{
    /// <summary>
    /// Called before visiting the child <see cref="ICoreNode"/>s
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> currently being visited</param>
    /// <returns>A <see cref="bool"/> indicating if the child <see cref="ICoreNode"/>s should be visited:
    /// If <c>true</c> is returned, the children are visited, 
    /// if <c>false</c> is returned, the children are not visited</returns>
    bool preVisit(ICoreNode node);
    /// <summary>
    /// Called after visiting the child <see cref="ICoreNode"/>s
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> currently being visited</param>
    void postVisit(ICoreNode node);
	}
}
