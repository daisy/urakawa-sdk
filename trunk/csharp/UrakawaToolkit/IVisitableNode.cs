using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for VisitableNode.
	/// </summary>
	public interface IVisitableNode
	{
    void AcceptDepthFirst(ICoreTreeVisitor visitor);
    void AcceptBreadthFirst(ICoreTreeVisitor visitor);
  }
}
