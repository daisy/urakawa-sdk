using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
	/// <summary>
	/// An abstract class providing the main functionality of a <see cref="AbstractFilterNavigator"/>,
	/// that is a <see cref="INavigator"/> navigating a forest of <see cref="CoreNode"/> obtained by filtering
	/// an actual <see cref="CoreNode"/> tree
	/// </summary>
	public abstract class AbstractFilterNavigator : INavigator
	{
		/// <summary>
		/// Determines if a given <see cref="ICoreNode"/> is included by the filter of the <see cref="AbstractFilterNavigator"/> instance
		/// </summary>
		/// <param name="node">The given <see cref="ICoreNode"/></param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> is included by the filter of the
		/// <see cref="AbstractFilterNavigator"/> instance</returns>
		/// <remarks>In derived concrete classes the implementation of this abstract method defines the filter</remarks>
		public abstract bool isIncluded(ICoreNode node);

		#region INavigator Members

		/// <summary>
		/// Gets the parent <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/> in the fintered tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The parent <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		public urakawa.core.ICoreNode getParent(urakawa.core.ICoreNode context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the previous sibling of a given context <see cref="ICoreNode"/> in the filtered tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The previous sibling <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		public urakawa.core.ICoreNode getPreviousSibling(urakawa.core.ICoreNode context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the next sibling of a given context <see cref="ICoreNode"/> in the filtered tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The next sibling <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		public urakawa.core.ICoreNode getNextSibling(urakawa.core.ICoreNode context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the number of children of a given context <see cref="ICoreNode"/> in the filtered tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The number of children</returns>
		public int getNumberOfChildren(urakawa.core.ICoreNode context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the child of a given context <see cref="ICoreNode"/> at a given index in the filtered tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="ICoreNode"/> at the given index</returns>
		public urakawa.core.ICoreNode getChild(urakawa.core.ICoreNode context, int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the previous <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in depth first traversal order of the filtered forest
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The previous <see cref="ICoreNode"/></returns>
		public urakawa.core.ICoreNode getPrevious(urakawa.core.ICoreNode context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the next <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in depth first traversal order of the filtered forest
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The next <see cref="ICoreNode"/></returns>
		public urakawa.core.ICoreNode getNext(urakawa.core.ICoreNode context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets an enumerator enumerating the filtered sub-tree starting at a given start <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="startNode">The given</param>
		/// <returns>The enumerator</returns>
		public IEnumerator<urakawa.core.CoreNode> getSubtreeIterator(urakawa.core.ICoreNode startNode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
