using System;

namespace urakawa.core
{
	/// <summary>
	/// Basic tree node, providing the minimal set of methods needed for a tree node
	/// </summary>
	public interface IBasicTreeNode
	{
    /// <summary>
    /// Gets the child <see cref="IBasicTreeNode"/> at a given index
    /// </summary>
    /// <param name="index">The given index</param>
    /// <returns>The child <see cref="IBasicTreeNode"/> at the given index</returns>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when <paramref name="index"/> is out if range, 
    /// that is not between 0 and <c><see cref="getChildCount"/>()-1</c></exception>
    IBasicTreeNode getChild(int index);

    /// <summary>
    /// Inserts a <see cref="IBasicTreeNode"/> child at a given index. 
    /// The index of any children at or after the given index are increased by one
    /// </summary>
    /// <param name="node">The new child <see cref="IBasicTreeNode"/> to insert,
    /// must be between 0 and the number of children as returned by member method 
    /// <see cref="getChildCount"/></param>
    /// <param name="insertIndex">The index at which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when <paramref name="insertIndex"/> is out if range, 
    /// that is not between 0 and <c><see cref="getChildCount"/>()</c></exception>
    /// <exception cref="exception.MethodParameterIsNull">
    /// Thrown when <paramref name="node"/> is null</exception>
    void insert(IBasicTreeNode node, int insertIndex);

    /// <summary>
    /// Detaches the instance <see cref="IBasicTreeNode"/> from it's parent's children
    /// </summary>
    /// <returns>The detached <see cref="IBasicTreeNode"/> (i.e. <c>this</c>)</returns>
    IBasicTreeNode detach();

    /// <summary>
    /// Gets the parent <see cref="IBasicTreeNode"/> of the instance
    /// </summary>
    /// <returns>The parent</returns>
    IBasicTreeNode getParent();

    /// <summary>
    /// Gets the number of children
    /// </summary>
    /// <returns>The number of children</returns>
    int getChildCount();
  }
}
