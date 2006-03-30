using System;
using System.Collections;

namespace urakawa.core
{
	/// <summary>
	/// Implements interface <see cref="ILimitedDOMNode"/>. 
	/// Must be owner by a <see cref="ICoreNode"/>implementation
	/// </summary>
	internal class LimitedDOMNode : ILimitedDOMNode
	{
    /// <summary>
    /// The <see cref="ICoreNode"/> that owns the <see cref="LimitedDOMNode"/>
    /// </summary>
    private ICoreNode mOwner;
   
    /// <summary>
    /// The parent <see cref="ICoreNode"/>
    /// </summary>
    private ICoreNode mParent;

    /// <summary>
    /// Contains the children of the node
    /// </summary>
    /// <remarks>All items in <see cref="mChildren"/> MUST be <see cref="ICoreNode"/>s</remarks>
    private IList mChildren;


    /// <summary>
    /// Constructor that sets the owner <see cref="ICoreNode"/> and parent <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="owner">The owner</param>
    /// <param name="parent">The parent</param>
    internal LimitedDOMNode(ICoreNode owner, ICoreNode parent)
		{
      mOwner = owner;
      mParent = parent;
      mChildren = new ArrayList();
    }
    #region ILimitedDOMNode Members

    /// <summary>
    /// See <see cref="ILimitedDOMNode.getParent"/>
    /// </summary>
    public ICoreNode getParent()
    {
      return mParent;
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.appendChild"/>
    /// </summary>
    public void appendChild(ICoreNode newChild)
    {
      if (newChild==null) 
      {
        throw new exception.MethodParameterIsNullException("Parameter 'newChild' is null");
      }
      mChildren.Add(newChild);
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.insertChild"/>
    /// </summary>
    public void insertChild(ICoreNode newChild, int index)
    {
      if (newChild==null) 
      {
        throw new exception.MethodParameterIsNullException("Parameter 'newChild' is null");
      }
      mChildren.Insert(index, newChild);
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.getChild"/>
    /// </summary>
    public ICoreNode getChild(int index)
    {
      if (index<0 || getChildCount()<index) 
      {
        throw new exception.MethodParameterValueIsOutOfBoundsException(
          "Parameter 'index' is out of bounds");
      }
      return (ICoreNode)mChildren[index];
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.removeChild"/>
    /// </summary>
    public ICoreNode removeChild(int index)
    {
      if (index<0 || getChildCount()<index) 
      {
        throw new exception.MethodParameterValueIsOutOfBoundsException(
          "Parameter 'index' is out of bounds");
      }
      ICoreNode removedNode = (ICoreNode)mChildren[index];
      mChildren.RemoveAt(index);
      return removedNode;
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.replaceChild"/>
    /// </summary>
    public ICoreNode replaceChild(ICoreNode node, int index)
    {
      if (node==null) 
      {
        throw new exception.MethodParameterIsNullException("Parameter 'node' is null");
      }
      if (index<0 || getChildCount()<index) 
      {
        throw new exception.MethodParameterValueIsOutOfBoundsException(
          "Parameter 'index' out of bounds");
      }
      ICoreNode oldNode = (ICoreNode)mChildren[index];
      mChildren[index] = node;
      return oldNode;
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.getChildCount"/>
    /// </summary>
    public int getChildCount()
    {
      return mChildren.Count;
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.indexOfChild"/>
    /// </summary>
    public int indexOfChild(ICoreNode node)
    {
      if (node==null) 
      {
        throw new exception.MethodParameterIsNullException("Parameter 'node' is null");
      }
      int index = mChildren.IndexOf(node);
      if (index==-1) 
      {
        throw new exception.NodeDoesNotExistException("The specified node is not a child node");
      }
      return index;
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.detach"/>
    /// </summary>
    public ICoreNode detach()
    {
      return getParent().removeChild(indexOfChild(mOwner));
    }

    #endregion
  }
}
