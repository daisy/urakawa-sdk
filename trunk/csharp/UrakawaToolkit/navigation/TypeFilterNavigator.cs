using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
	/// <summary>
	/// The <see cref="TypeFilterNavigator{T}"/> is an concrete implementation of the <see cref="AbstractFilterNavigator"/>
	/// that navigates a virtual forest of trees obtained from a <see cref="CoreNode"/> tree by fintering
	/// on the basis of <see cref="Type"/>, more specifically on sub-type of <see cref="CoreNode"/>
	/// </summary>
	/// <typeparam name="T">The <see cref="CoreNode"/> subclass by which to filter</typeparam>
	public class TypeFilterNavigator<T> : AbstractFilterNavigator where T : CoreNode
	{
		/// <summary>
		/// Determines if a given <see cref="CoreNode"/> is included by the filter of the <see cref="TypeFilterNavigator{T}"/>,
		/// that is if the given <see cref="CoreNode"/> is a <typeparamref name="T"/>
		/// </summary>
		/// <param name="node">The <see cref="CoreNode"/> to test</param>
		/// <returns><c>true</c> if <paramref name="node"/> is a <typeparamref name="T"/>, otherwise <c>false</c></returns>
		public override bool isIncluded(CoreNode node)
		{
			return (node is T);
		}

		/// <summary>
		/// Gets the next <see cref="CoreNode"/> (or rather <typeparamref name="T"/>) of the filtered forest of trees
		/// in depth first order, following a given context <see cref="CoreNode"/>
		/// </summary>
		/// <param name="context">The context <see cref="CoreNode"/></param>
		/// <returns>The next <typeparamref name="T"/> node or null if no next <typeparamref name="T"/> node exists</returns>
		public new T getNext(CoreNode context)
		{
			return (base.getNext(context) as T);
		}

		/// <summary>
		/// Gets the previous <see cref="CoreNode"/> (or rather <typeparamref name="T"/>) of the filtered forest of trees
		/// in depth first order, preceding a given context <see cref="CoreNode"/>
		/// </summary>
		/// <param name="context">The context <see cref="CoreNode"/></param>
		/// <returns>The previuos <typeparamref name="T"/> node or null if no previous <typeparamref name="T"/> node exists</returns>
		public new T getPrevious(CoreNode context)
		{
			return (base.getPrevious(context) as T);
		}

		/// <summary>
		/// Gets the child <see cref="CoreNode"/> (or rather <typeparamref name="T"/>) of the filtered forest of tree
		/// at a given index of a given context <see cref="CoreNode"/>
		/// </summary>
		/// <param name="context">The context <see cref="CoreNode"/></param>
		/// <param name="index">The given index</param>
		/// <returns>
		/// The child <typeparamref name="T"/> node of <paramref name="context"/> at index <paramref name="i ndex"/>,
		/// or <c>null</c> if no such child exists
		/// </returns>
		public new T getChild(CoreNode context, int index)
		{
			return (base.getChild(context, index) as T);
		}

		/// <summary>
		/// Gets the next sibling <see cref="CoreNode"/> (or rather <typeparamref name="T"/>) in the filtered forest of trees
		/// of a given context <see cref="CoreNode"/>
		/// </summary>
		/// <param name="context">The context <see cref="CoreNode"/></param>
		/// <returns>The next sibling <typeparamref name="T"/> node or <c>null</c> if no next sibling <typeparamref name="T"/> exists</returns>
		public new T getNextSibling(CoreNode context)
		{
			return (base.getNextSibling(context) as T);
		}

		/// <summary>
		/// Gets the previous sibling <see cref="CoreNode"/> (or rather <typeparamref name="T"/>) in the filtered forest of trees
		/// of a given context <see cref="CoreNode"/>
		/// </summary>
		/// <param name="context">The context <see cref="CoreNode"/></param>
		/// <returns>The next sibling <typeparamref name="T"/> node or <c>null</c> if no next sibling <typeparamref name="T"/> exists</returns>
		public new T getPreviousSibling(CoreNode context)
		{
			return (base.getPreviousSibling(context) as T);
		}

		/// <summary>
		/// Gets an enumerator enumerating the filtered sub-tree starting at a given start <see cref="CoreNode"/>
		/// </summary>
		/// <param name="context">The given</param>
		/// <returns>The enumerator</returns>
		public new IEnumerator<T> getSubForestIterator(CoreNode context)
		{
			return (base.getSubForestIterator(context) as IEnumerator<T>);
		}
	}
}
