using System;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	internal class DOMNode : LimitedDOMNode, IDOMNode
	{
    //TODO: Add exception handling throughout
		internal DOMNode(ICoreNode owner) : base(owner)
		{
    }
    #region IDOMNode Members

    public void insertBefore(ICoreNode newNode, ICoreNode anchorNode)
    {
      insert(newNode, indexOf(anchorNode));
    }

    public void insertAfter(ICoreNode newNode, ICoreNode anchorNode)
    {
      insert(newNode, indexOf(anchorNode));
    }

    public ICoreNode removeChild(ICoreNode node)
    {
      return removeChild(indexOf(node));
    }

    public ICoreNode replaceChild(ICoreNode node, ICoreNode oldNode)
    {
      return replaceChild(node, indexOf(oldNode));
    }

    #endregion
  }
}
