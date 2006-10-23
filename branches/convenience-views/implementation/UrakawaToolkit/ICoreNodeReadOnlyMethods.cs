using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.core
{
	/// <summary>
	/// Provides the read-only tree methods of a <see cref="ICoreNode"/>
	/// </summary>
	public interface ICoreNodeReadOnlyMethods
	{
		/// <summary>
		/// Gets the index of a given child <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="node">The given child <see cref="ICoreNode"/></param>
		/// <returns>The index of the given child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paranref name="node"/> is null</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref name="node"/> is not a child of the <see cref="ICoreNode"/></exception>
		int indexOf(ICoreNode node);

		/// <summary>
		/// Gets the parent <see cref="ICoreNode"/> of the instance,
		/// null if the instance is detached from a tree or is the root node of a tree
		/// </summary>
		/// <returns>The parent</returns>
		ICoreNode getParent();

		/// <summary>
		/// Gets the number of children
		/// </summary>
		/// <returns>The number of children</returns>
		int getChildCount();

		/// <summary>
		/// Gets the child <see cref="ICoreNode"/> at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="ICoreNode"/> at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="index"/> is out if range, 
		/// that is not between 0 and <c><see cref="getChildCount"/>()-1</c></exception>
		ICoreNode getChild(int index);

		/// <summary>
		/// Make a copy of the node
		/// </summary>
		/// <param name="deep">If true, then include the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <param name="copyProperties">If true, then include the node's properties.</param>
		/// <returns>A <see cref="ICoreNode"/> containing the copied data.</returns>
		ICoreNode copy(bool deep, bool copyProperties);

		/// <summary>
		/// Make a copy of the node
		/// </summary>
		/// <param name="deep">If true, then include the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <returns>A <see cref="ICoreNode"/> containing the copied data.</returns>
		ICoreNode copy(bool deep);

		/// <summary>
		/// Make a deep copy of the node including properties
		/// </summary>
		/// <returns>A <see cref="ICoreNode"/> containing the copied data.</returns>
		ICoreNode copy();

	}
}
