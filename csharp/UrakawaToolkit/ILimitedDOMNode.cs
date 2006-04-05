using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for LimitedDOMNode.
	/// </summary>
	public interface ILimitedDOMNode
	{
    /// <summary>
    /// Gets the parent <see cref="ICoreNode"/> of the instance
    /// </summary>
    /// <returns>The parent</returns>
    ICoreNode getParent();

    /// <summary>
    /// Sets the parent <see cref="ICoreNode"/> of the instance
    /// </summary>
    /// <param name="parent">The parent</param>
    /// <remarks>
    /// For internal use only, using this method may corrupt the core tree</remarks>
    void setParent(ICoreNode parent);

    /// <summary>
    /// Appends a <see cref="ICoreNode"/> child to the end of instance's list of child nodes
    /// </summary>
    /// <param name="newChild">The new child <see cref="ICoreNode"/> to append</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paranref name="newChild"/> is null</exception>
    void appendChild(ICoreNode newChild);

    /// <summary>
    /// Inserts a new child <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="newChild">The new child <see cref="ICoreNode"/> to insert</param>
    /// <param name="index">The index at which to insert the new node</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paranref name="newChild"/> is null</exception>
    /// <exception cref="exception.MethodParameterIsValueOutOfBoundsException">
    /// Thrown when index is out if range, 
    /// that is when<c><paramref name="index"/> &lt; 0</c> 
    /// or <c><see cref="getChildCount"/>() &lt; <paramref name="index"/></c></exception>
    void insertChild(ICoreNode newChild, int index);

    /// <summary>
    /// Gets the child <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="index">The given index</param>
    /// <returns>The child <see cref="ICoreNode"/> at the given index</returns>
    /// <exception cref="exception.MethodParameterIsValueOutOfBoundsException">
    /// Thrown when index is out if range, 
    /// that is when<c><paramref name="index"/> &lt; 0</c> 
    /// or <c><see cref="getChildCount"/>() &lt; <paramref name="index"/></c></exception>
    ICoreNode getChild(int index);

    /// <summary>
    /// Removes the child <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="index">The given index</param>
    /// <returns>The removed child <see cref="ICoreNode"/></returns>
    /// <exception cref="exception.MethodParameterIsValueOutOfBoundsException">
    /// Thrown when index is out if range, 
    /// that is when<c><paramref name="index"/> &lt; 0</c> 
    /// or <c><see cref="getChildCount"/>() &lt; <paramref name="index"/></c></exception>
    ICoreNode removeChild(int index);

    /// <summary>
    /// Replaces the child <see cref="ICoreNode"/> at a given index with a new <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The new <see cref="ICoreNode"/> with which to replace</param>
    /// <param name="index">The index of the child <see cref="ICoreNode"/> to replace</param>
    /// <returns>The replaced child <see cref="ICoreNode"/></returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paranref name="node"/> is null</exception>
    /// <exception cref="exception.MethodParameterIsValueOutOfBoundsException">
    /// Thrown when index is out if range, 
    /// that is when<c><paramref name="index"/> &lt; 0</c> 
    /// or <c><see cref="getChildCount"/>() &lt; <paramref name="index"/></c></exception>
    ICoreNode replaceChild(ICoreNode node, int index);

    /// <summary>
    /// Gets the number of children
    /// </summary>
    /// <returns>The number of children</returns>
    int getChildCount();

    /// <summary>
    /// Gets the index of a given child <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The given child <see cref="ICoreNode"/></param>
    /// <returns>The index of the given child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paranref name="node"/> is null</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="node"/> is not a child of the <see cref="ICoreNode"/></exception>
    int indexOfChild(ICoreNode node);

    /// <summary>
    /// Detaches the instance <see cref="ICoreNode"/> from it's parent's children
    /// </summary>
    /// <returns>The detached <see cref="ICoreNode"/> (i.e. <c>this</c>)</returns>
    ICoreNode detach();
  }
}
