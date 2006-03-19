using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for CoreTreeVisitor.
	/// </summary>
	public interface ICoreTreeVisitor
	{
    bool preVisit(ICoreNode node);
    void postVisit(ICoreNode node);
	}
}
