using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
	/// <summary>
	/// Generic interface for a <see cref="CoreNode"/> forest navigator providing methods for navigation 
	/// but not for manipulation of virtual trees in a virtual forest
	/// </summary>
	public interface INavigator
	{
		/// <summary>
		/// Gets the parent <see cref="CoreNode"/> of a given context <see cref="CoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="CoreNode"/></param>
		/// <returns>The parent <see cref="CoreNode"/> or <c>null</c> if no such <see cref="CoreNode"/> exists.</returns>
		CoreNode getParent(CoreNode context);
		/// <summary>
		/// Gets the previous sibling of a given context <see cref="CoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="CoreNode"/></param>
		/// <returns>The previous sibling <see cref="CoreNode"/> or <c>null</c> if no such <see cref="CoreNode"/> exists.</returns>
		CoreNode getPreviousSibling(CoreNode context);
		/// <summary>
		/// Gets the next sibling of a given context <see cref="CoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="CoreNode"/></param>
		/// <returns>The next sibling <see cref="CoreNode"/> or <c>null</c> if no such <see cref="CoreNode"/> exists.</returns>
		CoreNode getNextSibling(CoreNode context);
		/// <summary>
		/// Gets the number of children of a given context <see cref="CoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="CoreNode"/></param>
		/// <returns>The number of children</returns>
		int getChildCount(CoreNode context);
		/// <summary>
		/// Gets the child of a given context <see cref="CoreNode"/> at a given index in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="CoreNode"/></param>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="CoreNode"/> at the given index</returns>
		CoreNode getChild(CoreNode context, int index);
		/// <summary>
		/// Gets the previous <see cref="CoreNode"/> of a given context <see cref="CoreNode"/>
		/// in depth first traversal order of the virtual forest
		/// </summary>
		/// <param name="context">The given context <see cref="CoreNode"/></param>
		/// <returns>The previous <see cref="CoreNode"/></returns>
		CoreNode getPrevious(CoreNode context);
		/// <summary>
		/// Gets the next <see cref="CoreNode"/> of a given context <see cref="CoreNode"/>
		/// in depth first traversal order of the virtual forest
		/// </summary>
		/// <param name="context">The given context <see cref="CoreNode"/></param>
		/// <returns>The next <see cref="CoreNode"/></returns>
		CoreNode getNext(CoreNode context);
		/// <summary>
		/// Gets an enumerator enumerating the virtual sub-forest starting at a given start <see cref="CoreNode"/>
		/// </summary>
		/// <param name="startNode">The given</param>
		/// <returns>The enumerator</returns>
		IEnumerator<CoreNode> getSubForestIterator(CoreNode startNode);
	}
}
