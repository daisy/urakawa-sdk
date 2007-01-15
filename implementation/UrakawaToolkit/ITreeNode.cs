using System;

namespace urakawa.core
{
	/// <summary>
	/// Tree node providing DOM like navigation and manipulation
	/// </summary>
	public interface ITreeNode : IBasicTreeNode
	{

    /// <summary>
    /// Gets the index of a given child <see cref="ITreeNode"/>
    /// </summary>
    /// <param name="node">The given child <see cref="ITreeNode"/></param>
    /// <returns>The index of the given child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paranref name="node"/> is null</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="node"/> is not a child of the <see cref="ITreeNode"/></exception>
    int indexOf(ITreeNode node);

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
    ITreeNode removeChild(int index);

    /// <summary>
    /// Removes a given <see cref="ITreeNode"/> child. 
    /// </summary>
    /// <param name="node">The <see cref="ITreeNode"/> child to remove</param>
    /// <returns>The removed child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paramref name="node"/> is null</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="node"/> is not a child of the instance <see cref="ITreeNode"/></exception>
    ITreeNode removeChild(ITreeNode node);

    /// <summary>
    /// Inserts a new <see cref="ITreeNode"/> child before the given child.
    /// </summary>
    /// <param name="newChild">The new <see cref="ITreeNode"/> child node</param>
    /// <param name="anchorNode">The child before which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="newChild"/> and/or <paramref name="anchorNode"/> 
    /// have null values</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
    void insertBefore(ITreeNode newChild, ITreeNode anchorNode);

    /// <summary>
    /// Inserts a new <see cref="ITreeNode"/> child after the given child.
    /// </summary>
    /// <param name="newNode">The new <see cref="ITreeNode"/> child node</param>
    /// <param name="anchorNode">The child after which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="newNode"/> and/or <paramref name="anchorNode"/> 
    /// have null values</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
    void insertAfter(ITreeNode newNode, ITreeNode anchorNode);

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
    ITreeNode replaceChild(ITreeNode node, int index);

    /// <summary>
    /// Replaces an existing child <see cref="ITreeNode"/> with i new one
    /// </summary>
    /// <param name="node">The new child with which to replace</param>
    /// <param name="oldNode">The existing child node to replace</param>
    /// <returns>The replaced <see cref="ITreeNode"/> child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="node"/> and/or <paramref name="oldNode"/> 
    /// have null values</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="oldNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
    ITreeNode replaceChild(ITreeNode node, ITreeNode oldNode);

    /// <summary>
    /// Appends a child <see cref="ITreeNode"/> to the end of the list of children
    /// </summary>
    /// <param name="node">The new child to append</param>
    void appendChild(ITreeNode node);

  }
}
