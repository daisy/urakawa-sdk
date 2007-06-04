using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.core
{
	/// <summary>
	/// Provides the read-only tree methods of a <see cref="TreeNode"/>
	/// </summary>
	public interface ICoreNodeReadOnlyMethods
	{

		/// <summary>
		/// Gets the child <see cref="TreeNode"/> at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="TreeNode"/> at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out if range, 
		/// that is not between 0 and <c><see cref="getChildCount"/>()-1</c></exception>
		TreeNode getChild(int index);

		/// <summary>
		/// Gets the index of a given child <see cref="TreeNode"/>
		/// </summary>
		/// <param name="node">The given child <see cref="TreeNode"/></param>
		/// <returns>The index of the given child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paranref localName="node"/> is null</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="node"/> is not a child of the <see cref="TreeNode"/></exception>
		int indexOf(TreeNode node);

		/// <summary>
		/// Gets the parent <see cref="TreeNode"/> of the instance,
		/// null if the instance is detached from a tree or is the root node of a tree
		/// </summary>
		/// <returns>The parent</returns>
		TreeNode getParent();

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
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		TreeNode copy(bool deep, bool copyProperties);

		/// <summary>
		/// Make a copy of the node
		/// </summary>
		/// <param name="deep">If true, then include the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		TreeNode copy(bool deep);

		/// <summary>
		/// Make a deep copy of the node including properties
		/// </summary>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		TreeNode copy();

		/// <summary>
		/// Gets the next sibling of <c>this</c>
		/// </summary>
		/// <returns>The next sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
		TreeNode getNextSibling();

		/// <summary>
		/// Gets the previous sibling of <c>this</c>
		/// </summary>
		/// <returns>The previous sibling of <c>this</c> or <c>null</c> if no previous sibling exists</returns>
		TreeNode getPreviousSibling();

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is a sibling of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a sibling of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool isSiblingOf(TreeNode node);

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is an ancestor of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is an ancestor of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool isAncestorOf(TreeNode node);

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is a descendant of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a descendant of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool isDescendantOf(TreeNode node);
	}
}
