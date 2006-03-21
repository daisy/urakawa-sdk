using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for DOMNode.
	/// </summary>
	public interface IDOMNode : ILimitedDOMNode
	{
    /// <summary>
    /// Inserts a new <see cref="ICoreNode"/> child before the given child.
    /// </summary>
    /// <param name="newChild">The new <see cref="ICoreNode"/> child node</param>
    /// <param name="anchorNode">The child before which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="newChild"/> and/or <paramref name="anchorNode"/> 
    /// have null values</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="IDOMNode"/></exception>
    void insertBefore(ICoreNode newChild, ICoreNode anchorNode);

    /// <summary>
    /// Inserts a new <see cref="ICoreNode"/> child after the given child.
    /// </summary>
    /// <param name="newNode">The new <see cref="ICoreNode"/> child node</param>
    /// <param name="anchorNode">The child after which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="newNode"/> and/or <paramref name="anchorNode"/> 
    /// have null values</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="IDOMNode"/></exception>
    void insertAfter(ICoreNode newNode, ICoreNode anchorNode);

    /// <summary>
    /// Removes a given <see cref="ICoreNode"/> child. 
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> child to remove</param>
    /// <returns>The removed child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameter <paramref name="node"/> is null</exception>
    /// <exception cref="exception.NodeDoesNotExistException">
    /// Thrown when <paramref name="node"/> is not a child of the instance <see cref="IDOMNode"/></exception>
    ICoreNode removeChild(ICoreNode node);

    /// <summary>
    /// Replaces an existing child <see cref="ICoreNode"/> with i new one
    /// </summary>
    /// <param name="node">The new child with which to replace</param>
    /// <param name="oldNode">The existing child node to replace</param>
    /// <returns>The replaced <see cref="ICoreNode"/> child</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="node"/> and/or <paramref name="oldNode"/> 
    /// have null values</exception>
    /// Thrown when <paramref name="oldNode"/> is not a child of the instance <see cref="IDOMNode"/></exception>
    ICoreNode replaceChild(ICoreNode node, ICoreNode oldNode);
	}
}
