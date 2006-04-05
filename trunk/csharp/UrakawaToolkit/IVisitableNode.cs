using System;

namespace urakawa.core
{
	/// <summary>
	/// Provides methods for accepting <see cref="ICoreTreeVisitor"/>s
	/// </summary>
	public interface IVisitableNode
	{
    /// <summary>
    /// Accept a <see cref="ICoreTreeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreTreeVisitor"/></param>
    void AcceptDepthFirst(ICoreTreeVisitor visitor);
  }
}
