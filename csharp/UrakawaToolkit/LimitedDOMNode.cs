using System;
using System.Collections;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for LimitedDOMNode.
	/// </summary>
	internal class LimitedDOMNode : ILimitedDOMNode
	{
    //TODO: Add exception handling throughout
    private ICoreNode mOwner;

    /// <summary>
    /// Contains the children of the node
    /// </summary>
    /// <remarks>All items in <see cref="mChildren"/> MUST be <see cref="ICoreNode"/>s</remarks>
    private IList mChildren;

		internal LimitedDOMNode(ICoreNode owner)
		{
      mOwner = owner;
      mChildren = new ArrayList();
    }
    #region ILimitedDOMNode Members

    public ICoreNode getParent()
    {
      return mOwner.getParent();
    }

    public void appendChild(ICoreNode newChild)
    {
      mChildren.Add(newChild);
    }

    public void insert(ICoreNode newNode, int index)
    {
      mChildren.Insert(index, newNode);
    }

    public ICoreNode getChild(int index)
    {
      return (ICoreNode)mChildren[index];
    }

    public ICoreNode removeChild(int index)
    {
      ICoreNode removedNode = (ICoreNode)mChildren[index];
      mChildren.RemoveAt(index);
      return removedNode;
    }

    public ICoreNode replaceChild(ICoreNode node, int index)
    {
      ICoreNode oldNode = (ICoreNode)mChildren[index];
      mChildren[index] = node;
      return oldNode;
    }

    public int getChildCount()
    {
      return mChildren.Count;
    }

    public int indexOf(ICoreNode node)
    {
      return mChildren.IndexOf(node);
    }

    public ICoreNode detach()
    {
      return getParent().removeChild(indexOf(mOwner));
    }

    #endregion
  }
}
