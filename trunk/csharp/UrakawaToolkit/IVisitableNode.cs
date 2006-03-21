using System;

namespace urakawa.core
{
	/// <summary>
	/// Provides methods for accepting <see cref="ICoreNodeVisitor"/>s
	/// </summary>
	public interface IVisitableNode
	{
    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    void AcceptDepthFirst(ICoreTreeVisitor visitor);
  }
}
