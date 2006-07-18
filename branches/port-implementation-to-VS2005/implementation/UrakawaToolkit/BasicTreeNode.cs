using System;
using System.Collections;

namespace urakawa.core
{
	/// <summary>
	/// Implementation of the minimal tree node <see cref="IBasicTreeNode"/>
	/// For internal use only
	/// </summary>
	public class BasicTreeNode : IBasicTreeNode
	{
    
    /// <summary>
    /// Contains the children of the node
    /// </summary>
    /// <remarks>All items in <see cref="mChildren"/> MUST be <see cref="ICoreNode"/>s</remarks>
    private IList mChildren;

    /// <summary>
    /// Gets the index of a given child
    /// </summary>
    /// <param name="node">The given child</param>
    /// <returns>The index of the given child or -1 if <paramref name="node"/> is not a child</returns>
    protected int indexOfChild(IBasicTreeNode node)
    {
      return mChildren.IndexOf(node);
    }

    /// <summary>
    /// The parent <see cref="BasicTreeNode"/>
    /// </summary>
    private BasicTreeNode mParent;


    internal BasicTreeNode()
		{
			mChildren = new ArrayList();
    }

    #region IBasicTreeNode Members

    /// <summary>
    /// Gets the child <see cref="IBasicTreeNode"/> at a given index
    /// </summary>
    /// <param name="index">The given index</param>
    /// <returns>The child <see cref="IBasicTreeNode"/> at the given index</returns>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when <paramref name="index"/> is out if range, 
    /// that is not between 0 and <c><see cref="getChildCount"/>()-1</c></exception>
    public IBasicTreeNode getChild(int index)
    {
      if (index<0 || mChildren.Count<=index)
      {
        throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
          "Could not get child at index {0:0} - index is out of bounds", index));
      }
      return (IBasicTreeNode)mChildren[index];
    }

    /// <summary>
    /// Inserts a <see cref="IBasicTreeNode"/> child at a given index. 
    /// The index of any children at or after the given index are increased by one
    /// </summary>
    /// <param name="node">The new child <see cref="IBasicTreeNode"/> to insert,
    /// must be between 0 and the number of children as returned by member method.
    /// Must be an instance of 
    /// <see cref="getChildCount"/></param>
    /// <param name="insertIndex">The index at which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when <paramref name="insertIndex"/> is out if range, 
    /// that is not between 0 and <c><see cref="getChildCount"/>()</c></exception>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="node"/> is null</exception>
    /// <exception cref="exception.MethodParameterIsInvalidException">
    /// Thrown when <paramref name="node"/> is not an instance of <see cref="BasicTreeNode"/>
    /// </exception>
    public void insert(IBasicTreeNode node, int insertIndex)
    {
      if (node==null)
      {
        throw new exception.MethodParameterIsNullException(String.Format(
          "Can not insert null child at index {0:0}", insertIndex));
      }
      BasicTreeNode newNode;
      try
      {
        newNode = (BasicTreeNode)node;
      }
      catch (Exception e)
      {
        throw new exception.MethodParameterIsWrongTypeException(
          "The new node is not of type BasicTreeNode", e);
      }
      if (insertIndex<0 || mChildren.Count<insertIndex)
      {
        throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
          "Could not insert a new child at index {0:0} - index is out of bounds", insertIndex));
      }
      mChildren.Insert(insertIndex, newNode);
      newNode.mParent = this;
    }

    /// <summary>
    /// Detaches the instance <see cref="IBasicTreeNode"/> from it's parent's children
    /// </summary>
    /// <returns>The detached <see cref="IBasicTreeNode"/> (i.e. <c>this</c>)</returns>
    public IBasicTreeNode detach()
    {
      mParent.mChildren.Remove(this);
      mParent = null;
      return this;
    }

    /// <summary>
    /// Gets the parent <see cref="IBasicTreeNode"/> of the instance
    /// </summary>
    /// <returns>The parent</returns>
    public IBasicTreeNode getParent()
    {
      return mParent;
    }

    /// <summary>
    /// Gets the number of children
    /// </summary>
    /// <returns>The number of children</returns>
    public int getChildCount()
    {
      return mChildren.Count;
    }

    #endregion
  }
}
