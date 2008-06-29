using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.core
{
	/// <summary>
	/// Provides the read-only tree methods of a <see cref="TreeNode"/>
	/// </summary>
	public interface ITreeNodeReadOnlyMethods
	{

		/// <summary>
		/// Gets the child <see cref="TreeNode"/> at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="TreeNode"/> at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out if range, 
		/// that is not between 0 and <c><see cref="ChildCount"/>()-1</c></exception>
		TreeNode GetChild(int index);

		/// <summary>
		/// Gets the index of a given child <see cref="TreeNode"/>
		/// </summary>
		/// <param name="node">The given child <see cref="TreeNode"/></param>
		/// <returns>The index of the given child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paranref localName="node"/> is null</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="node"/> is not a child of the <see cref="TreeNode"/></exception>
		int IndexOf(TreeNode node);

	    /// <summary>
	    /// Gets the parent <see cref="TreeNode"/> of the instance,
	    /// null if the instance is detached from a tree or is the root node of a tree
	    /// </summary>
	    /// <returns>The parent</returns>
	    TreeNode Parent { get; }

	    /// <summary>
	    /// Gets the number of children
	    /// </summary>
	    /// <returns>The number of children</returns>
	    int ChildCount { get; }

	    /// <summary>
		/// Make a copy of the node
		/// </summary>
		/// <param name="deep">If true, then include the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <param name="copyProperties">If true, then include the node's property.</param>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		TreeNode Copy(bool deep, bool copyProperties);

		/// <summary>
		/// Make a copy of the node
		/// </summary>
		/// <param name="deep">If true, then include the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		TreeNode Copy(bool deep);

		/// <summary>
		/// Make a deep copy of the node including property
		/// </summary>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		TreeNode Copy();

	    /// <summary>
	    /// Gets the next sibling of <c>this</c>
	    /// </summary>
	    /// <returns>The next sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
	    TreeNode NextSibling { get; }

	    /// <summary>
	    /// Gets the previous sibling of <c>this</c>
	    /// </summary>
	    /// <returns>The previous sibling of <c>this</c> or <c>null</c> if no previous sibling exists</returns>
	    TreeNode PreviousSibling { get; }

	    /// <summary>
		/// Tests if a given <see cref="TreeNode"/> is a sibling of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a sibling of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool IsSiblingOf(TreeNode node);

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is an ancestor of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is an ancestor of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool IsAncestorOf(TreeNode node);

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is a descendant of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a descendant of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		bool IsDescendantOf(TreeNode node);
	}
}
