using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.core
{
	/// <summary>
	/// Provides the read-only tree methods of a <see cref="CoreNode"/>
	/// </summary>
	public interface ICoreNodeReadOnlyMethods
	{

		/// <summary>
		/// Gets the child <see cref="CoreNode"/> at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="CoreNode"/> at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out if range, 
		/// that is not between 0 and <c><see cref="getChildCount"/>()-1</c></exception>
		CoreNode getChild(int index);

		/// <summary>
		/// Gets the index of a given child <see cref="CoreNode"/>
		/// </summary>
		/// <param name="node">The given child <see cref="CoreNode"/></param>
		/// <returns>The index of the given child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paranref localName="node"/> is null</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="node"/> is not a child of the <see cref="CoreNode"/></exception>
		int indexOf(CoreNode node);

		/// <summary>
		/// Gets the parent <see cref="CoreNode"/> of the instance,
		/// null if the instance is detached from a tree or is the root node of a tree
		/// </summary>
		/// <returns>The parent</returns>
		CoreNode getParent();

		/// <summary>
		/// Gets the number of children
		/// </summary>
		/// <returns>The number of children</returns>
		int getChildCount();

		/// <summary>
		/// Make a copy of the node
		/// </summary>
		/// <param name="deep">If true, then include the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <param name="copyProperties">If true, then include the node's properties.</param>
		/// <returns>A <see cref="CoreNode"/> containing the copied data.</returns>
		CoreNode copy(bool deep, bool copyProperties);

		/// <summary>
		/// Make a copy of the node
		/// </summary>
		/// <param name="deep">If true, then include the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <returns>A <see cref="CoreNode"/> containing the copied data.</returns>
		CoreNode copy(bool deep);

		/// <summary>
		/// Make a deep copy of the node including properties
		/// </summary>
		/// <returns>A <see cref="CoreNode"/> containing the copied data.</returns>
		CoreNode copy();

		/// <summary>
		/// Gets the next sibling of <c>this</c>
		/// </summary>
		/// <returns>The next sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
		CoreNode getNextSibling();

		/// <summary>
		/// Gets the previous sibling of <c>this</c>
		/// </summary>
		/// <returns>The previous sibling of <c>this</c> or <c>null</c> if no previous sibling exists</returns>
		CoreNode getPreviousSibling();

		/// <summary>
		/// Tests if a given <see cref="CoreNode"/> is a sibling of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="CoreNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a sibling of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool isSiblingOf(CoreNode node);

		/// <summary>
		/// Tests if a given <see cref="CoreNode"/> is an ancestor of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="CoreNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is an ancestor of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool isAncestorOf(CoreNode node);

		/// <summary>
		/// Tests if a given <see cref="CoreNode"/> is a descendant of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="CoreNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a descendant of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool isDescendantOf(CoreNode node);
	}
}
