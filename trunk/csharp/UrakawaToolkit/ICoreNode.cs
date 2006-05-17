using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for the core node of the Urakawa model
	/// </summary>
	public interface ICoreNode : ITreeNode, IVisitableCoreNode, IXUKable
	{
    #region new IBasicTreeNode members
//    /// <summary>
//    /// Gets the child <see cref="ICoreNode"/> at a given index
//    /// </summary>
//    /// <param name="index">The given index</param>
//    /// <returns>The child <see cref="ICoreNode"/> at the given index</returns>
//    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
//    /// Thrown when index is out if range, 
//    /// that is when<c><paramref name="index"/> &lt; 0</c> 
//    /// or <c><see cref="IBasicTreeNode.getChildCount"/>() &lt; <paramref name="index"/></c></exception>
//    new ICoreNode getChild(int index);
//
//    /// <summary>
//    /// Removes the child <see cref="ICoreNode"/> at a given index
//    /// </summary>
//    /// <param name="index">The given index</param>
//    /// <returns>The removes child <see cref="ICoreNode"/></returns>
//    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
//    /// Thrown when index is out if range, 
//    /// that is when<c><paramref name="index"/> &lt; 0</c> 
//    /// or <c><see cref="IBasicTreeNode.getChildCount"/>() &lt; <paramref name="index"/></c></exception>
//    new ICoreNode removeChild(int index);
//
//    /// <summary>
//    /// Detaches the instance <see cref="ICoreNode"/> from it's parent's children
//    /// </summary>
//    /// <returns>The detached <see cref="ICoreNode"/> (i.e. <c>this</c>)</returns>
//    new ICoreNode detach();
//
//    /// <summary>
//    /// Gets the parent <see cref="ICoreNode"/> of the instance
//    /// </summary>
//    /// <returns>The parent</returns>
//    new ICoreNode getParent();
//    #endregion
//
//    #region new ITreeNode members
//    /// <summary>
//    /// Removes a given <see cref="ITreeNode"/> child. 
//    /// </summary>
//    /// <param name="node">The <see cref="ITreeNode"/> child to remove</param>
//    /// <returns>The removed child</returns>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameter <paramref name="node"/> is null</exception>
//    /// <exception cref="exception.NodeDoesNotExistException">
//    /// Thrown when <paramref name="node"/> is not a child of the instance <see cref="ICoreNode"/></exception>
//    new ICoreNode removeChild(ITreeNode node);
//
//    /// <summary>
//    /// Replaces an existing child <see cref="ITreeNode"/> with i new one
//    /// </summary>
//    /// <param name="node">The new child with which to replace</param>
//    /// <param name="oldNode">The existing child node to replace</param>
//    /// <returns>The replaced <see cref="ITreeNode"/> child</returns>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameters <paramref name="node"/> and/or <paramref name="oldNode"/> 
//    /// have null values</exception>
//    /// <exception cref="exception.NodeDoesNotExistException">
//    /// Thrown when <paramref name="oldNode"/> is not a child of the instance <see cref="ICoreNode"/></exception>
//    new ICoreNode replaceChild(ITreeNode node, ITreeNode oldNode);
//
//    /// <summary>
//    /// Replaces the child <see cref="ITreeNode"/> at a given index with a new <see cref="ITreeNode"/>
//    /// </summary>
//    /// <param name="node">The new <see cref="ITreeNode"/> with which to replace</param>
//    /// <param name="index">The index of the child <see cref="ITreeNode"/> to replace</param>
//    /// <returns>The replaced child <see cref="ITreeNode"/></returns>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameter <paranref name="node"/> is null</exception>
//    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
//    /// Thrown when index is out if range, 
//    /// that is when<c><paramref name="index"/> &lt; 0</c> 
//    /// or <c><see cref="IBasicTreeNode.getChildCount"/>() &lt; <paramref name="index"/></c></exception>
//    new ICoreNode replaceChild(ITreeNode node, int index);
    #endregion

    /// <summary>
    /// Gets the <see cref="Presentation"/> that owns the core node
    /// </summary>
    /// <returns>The owner</returns>
    IPresentation getPresentation();

    /// <summary>
    /// Gets the <see cref="IProperty"/> of the given <see cref="PropertyType"/>
    /// </summary>
    /// <param name="type">The given <see cref="PropertyType"/></param>
    /// <returns>The <see cref="IProperty"/> of the given <see cref="PropertyType"/>,
    /// <c>null</c> if no property of the given <see cref="PropertyType"/> has been set</returns>
    IProperty getProperty(PropertyType type);

    /// <summary>
    /// Sets a <see cref="IProperty"/>, possible overwriting previously set <see cref="IProperty"/>
    /// of the same <see cref="PropertyType"/>
    /// </summary>
    /// <param name="prop">The <see cref="IProperty"/> to set. 
    /// If <c>null</c> is passed, an <see cref="exception.MethodParameterIsNullException"/> is thrown</param>
    /// <returns>A <see cref="bool"/> indicating if a previously set <see cref="IProperty"/>
    /// was overwritten
    /// </returns>
    bool setProperty(IProperty prop);
	}
}
