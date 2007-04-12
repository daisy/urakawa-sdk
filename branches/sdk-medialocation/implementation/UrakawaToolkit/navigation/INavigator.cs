using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
	/// <summary>
	/// Generic interface for a <see cref="ICoreNode"/> forest navigator providing methods for navigation 
	/// but not for manipulation of virtual trees in a virtual forest
	/// </summary>
	public interface INavigator
	{
		/// <summary>
		/// Gets the parent <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The parent <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		ICoreNode getParent(ICoreNode context);
		/// <summary>
		/// Gets the previous sibling of a given context <see cref="ICoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The previous sibling <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		ICoreNode getPreviousSibling(ICoreNode context);
		/// <summary>
		/// Gets the next sibling of a given context <see cref="ICoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The next sibling <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		ICoreNode getNextSibling(ICoreNode context);
		/// <summary>
		/// Gets the number of children of a given context <see cref="ICoreNode"/> in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The number of children</returns>
		int getChildCount(ICoreNode context);
		/// <summary>
		/// Gets the child of a given context <see cref="ICoreNode"/> at a given index in the virtual tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="ICoreNode"/> at the given index</returns>
		ICoreNode getChild(ICoreNode context, int index);
		/// <summary>
		/// Gets the previous <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in depth first traversal order of the virtual forest
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The previous <see cref="ICoreNode"/></returns>
		ICoreNode getPrevious(ICoreNode context);
		/// <summary>
		/// Gets the next <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in depth first traversal order of the virtual forest
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The next <see cref="ICoreNode"/></returns>
		ICoreNode getNext(ICoreNode context);
		/// <summary>
		/// Gets an enumerator enumerating the virtual sub-forest starting at a given start <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="startNode">The given</param>
		/// <returns>The enumerator</returns>
		IEnumerator<ICoreNode> getSubForestIterator(ICoreNode startNode);
	}
}
