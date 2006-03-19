using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for LimitedDOMNode.
	/// </summary>
	public interface ILimitedDOMNode
	{
    ICoreNode getParent();
    void appendChild(ICoreNode newChild);
    void insert(ICoreNode newNode, int index);
    ICoreNode getChild(int index);
    ICoreNode removeChild(int index);
    ICoreNode replaceChild(ICoreNode node, int index);
    int getChildCount();
    int indexOf(ICoreNode node);
    ICoreNode detach();
  }
}
