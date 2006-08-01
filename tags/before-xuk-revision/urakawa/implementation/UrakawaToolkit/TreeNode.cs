using System;

namespace urakawa.core
{
	/// <summary>
	/// Implementation of the tree node <see cref="ITreeNode"/>
	/// For internal use only
	/// </summary>
	public class TreeNode : BasicTreeNode, ITreeNode
	{
		internal TreeNode() : base()
		{
			//
			// TODO: Add constructor logic here
			//
    }
    #region ITreeNode Members

    /// <summary>
    /// Gets the index of a given child
    /// </summary>
    /// <param name="node">The given child</param>
    /// <returns>The index of the given child or -1 if <paramref name="node"/> is not a child</returns>
    /// <remarks>Hides <see cref="BasicTreeNode.indexOfChild"/> 
    /// from any decendants of <see cref="TreeNode"/></remarks>
    private new int indexOfChild(IBasicTreeNode node)
    {
      return base.indexOfChild(node);
    }


    /// <summary>
    /// Gets the index of a given child <see cref="ITreeNode"/>
    /// </summary>
    /// <param name="node">The given child <see cref="ITreeNode"/></param>
    /// <returns>The index of the given child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paranref name="node"/> is null</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="node"/> is not a child of the <see cref="ITreeNode"/></exception>
    public int indexOf(ITreeNode node)
    {
      if (node==null)
      {
        throw new exception.MethodParameterIsNullException("The given node is null");
      }
      int index = indexOfChild(node);
      if (index==-1)
      {
        throw new exception.NodeDoesNotExistException("The given node is not a child");
      }
      return index;
    }

    /// <summary>
    /// Removes the child at a given index. 
    /// </summary>
    /// <param name="index">The given index</param>
    /// <returns>The removed child</returns>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when <paramref name="index"/> is out of bounds, 
    /// that is not the index of a child 
    /// (child indexes range from 0 to <c><see cref="IBasicTreeNode.getChildCount"/>()-1</c>)
    /// </exception>
    public ITreeNode removeChild(int index)
    {
      IBasicTreeNode removedChild = getChild(index);
      removedChild.detach();
      return (ITreeNode)removedChild;
    }

    /// <summary>
    /// Removes a given <see cref="ITreeNode"/> child. 
    /// </summary>
    /// <param name="node">The <see cref="ITreeNode"/> child to remove</param>
    /// <returns>The removed child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paramref name="node"/> is null</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="node"/> is not a child of the instance <see cref="ITreeNode"/></exception>
    public ITreeNode removeChild(ITreeNode node)
    {
      int index = indexOf(node);
      return removeChild(index);
    }

    /// <summary>
    /// Inserts a new <see cref="ITreeNode"/> child before the given child.
    /// </summary>
    /// <param name="node">The new <see cref="ITreeNode"/> child node</param>
    /// <param name="anchorNode">The child before which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="node"/> and/or <paramref name="anchorNode"/> 
    /// have null values</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref name="node"/> is already attached as a child of a parent 
		/// </exception>
		public void insertBefore(ITreeNode node, ITreeNode anchorNode)
    {
      int index = indexOf(anchorNode);
      insert(node, index);
    }

    /// <summary>
    /// Inserts a new <see cref="ITreeNode"/> child after the given child.
    /// </summary>
    /// <param name="node">The new <see cref="ITreeNode"/> child node</param>
    /// <param name="anchorNode">The child after which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="node"/> and/or <paramref name="anchorNode"/> 
    /// have null values</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref name="node"/> is already attached as a child of a parent 
		/// </exception>
		public void insertAfter(ITreeNode node, ITreeNode anchorNode)
    {
      int index = indexOf(anchorNode)+1;
      insert(node, index);
    }

    /// <summary>
    /// Replaces the child <see cref="ITreeNode"/> at a given index with a new <see cref="ITreeNode"/>
    /// </summary>
    /// <param name="node">The new <see cref="ITreeNode"/> with which to replace</param>
    /// <param name="index">The index of the child <see cref="ITreeNode"/> to replace</param>
    /// <returns>The replaced child <see cref="ITreeNode"/></returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paranref name="node"/> is null</exception>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when index is out if range, 
    /// that is when <paramref name="index"/> is not between 0 
    /// and <c><see cref="IBasicTreeNode.getChildCount"/>()-1</c>c></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref name="node"/> is already attached as a child of a parent 
		/// </exception>
		public ITreeNode replaceChild(ITreeNode node, int index)
    {
      IBasicTreeNode replacedChild = getChild(index);
      insert(node, index);
      replacedChild.detach();
      return (ITreeNode)replacedChild;
    }

    /// <summary>
    /// Replaces an existing child <see cref="ITreeNode"/> with i new one
    /// </summary>
    /// <param name="node">The new child with which to replace</param>
    /// <param name="oldNode">The existing child node to replace</param>
    /// <returns>The replaced <see cref="ITreeNode"/> child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="node"/> and/or <paramref name="oldNode"/> 
    /// have null values
		/// </exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="oldNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref name="node"/> is already attached as a child of a parent 
		/// </exception>
		public ITreeNode replaceChild(ITreeNode node, ITreeNode oldNode)
    {
      return replaceChild(node, indexOf(oldNode));
    }

    /// <summary>
    /// Appends a child <see cref="ITreeNode"/> to the end of the list of children
    /// </summary>
    /// <param name="node">The new child to append</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref name="node"/> and/or <paramref name="oldNode"/> 
		/// have null values
		/// </exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref name="node"/> is already attached as a child of a parent 
		/// </exception>
		public void appendChild(ITreeNode node)
    {
      insert(node, getChildCount());
    }

    #endregion
  }
}
