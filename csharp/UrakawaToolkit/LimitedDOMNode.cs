using System;
using System.Collections;

namespace urakawa.core
{
	/// <summary>
	/// Implements interface <see cref="ILimitedDOMNode"/>. 
	/// Must be owner by a <see cref="ICoreNode"/>implementation
	/// </summary>
	public class LimitedDOMNode : ILimitedDOMNode
	{
    /// <summary>
    /// The parent <see cref="ICoreNode"/>
    /// </summary>
    private ICoreNode mParent;

    /// <summary>
    /// The owner <see cref="ICoreNode"/> of the <see cref="LimitedDOMNode"/>
    /// </summary>
    protected ICoreNode mOwner;

    /// <summary>
    /// Default constructor
    /// </summary>
    internal LimitedDOMNode()
    {
    }

    /// <summary>
    /// Contains the children of the node
    /// </summary>
    /// <remarks>All items in <see cref="mChildren"/> MUST be <see cref="ICoreNode"/>s</remarks>
    private IList mChildren = new ArrayList();

    #region ILimitedDOMNode Members

    /// <summary>
    /// See <see cref="ILimitedDOMNode.getParent"/>
    /// </summary>
    public ICoreNode getParent()
    {
      return mParent;
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.setParent"/>
    /// </summary>
    public void setParent(ICoreNode parent)
    {
      mParent = parent;
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
      newChild.setParent(mOwner);
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
      newChild.appendChild(mOwner);
    }

    /// <summary>
    /// See <see cref="ILimitedDOMNode.getChild"/>
    /// </summary>
    public ICoreNode getChild(int index)
    {
      if (index<0 || getChildCount()<index) 
      {
        throw new exception.MethodParameterIsOutOfBoundsException(
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
        throw new exception.MethodParameterIsOutOfBoundsException(
          "Parameter 'index' is out of bounds");
      }
      ICoreNode removedNode = (ICoreNode)mChildren[index];
      mChildren.RemoveAt(index);
      removedNode.setParent(null);
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
        throw new exception.MethodParameterIsOutOfBoundsException(
          "Parameter 'index' out of bounds");
      }
      ICoreNode oldNode = (ICoreNode)mChildren[index];
      mChildren[index] = node;
      oldNode.setParent(null);
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
      ICoreNode detNode = getParent().removeChild(indexOfChild(mOwner));
      detNode.setParent(null);
      return detNode;
    }

    #endregion
  }
}
