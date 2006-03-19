using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for DOMNode.
	/// </summary>
	public interface IDOMNode : ILimitedDOMNode
	{
    void insertBefore(ICoreNode newNode, ICoreNode anchorNode);
    void insertAfter(ICoreNode newNode, ICoreNode anchorNode);
    ICoreNode removeChild(ICoreNode node);
    ICoreNode replaceChild(ICoreNode node, ICoreNode oldNode);
	}
}
