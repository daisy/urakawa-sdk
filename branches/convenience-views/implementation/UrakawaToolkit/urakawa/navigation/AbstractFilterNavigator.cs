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
		/// Determines if a given <see cref="ICoreNode"/> is included by the filter of the <see cref="AbstractFilterNavigator"/> instance.
		/// Concrete classes must implement this method to determine the behaviour of the filter navigator
		/// </summary>
		/// <param localName="node">The given <see cref="ICoreNode"/></param>
		/// <returns>A <see cref="bool"/> indicating if <paramref localName="node"/> is included by the filter of the
		/// <see cref="AbstractFilterNavigator"/> instance</returns>
		/// <remarks>In derived concrete classes the implementation of this abstract method defines the filter</remarks>
		public abstract bool isIncluded(ICoreNode node);

		#region INavigator Members

		/// <summary>
		/// Gets the parent <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/> in the filtered tree
		/// </summary>
		/// <param localName="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The parent <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="context"/> is <c>null</c>
		/// </exception>
		public ICoreNode getParent(ICoreNode context)
		{
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			ICoreNode parent = context.getParent();
			if (parent == null) return null;
			if (isIncluded(parent)) return parent;
			return getParent(parent);
		}


		/// <summary>
		/// Gets the previous sibling of a given context <see cref="ICoreNode"/> in the filtered tree
		/// </summary>
		/// <param localName="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The previous sibling <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="context"/> is <c>null</c>
		/// </exception>
		public ICoreNode getPreviousSibling(ICoreNode context)
		{
			return getPreviousSibling(context, true);
		}

		private ICoreNode getPreviousSibling(ICoreNode context, bool checkParent)
		{
			#region Works but is considered ugly
			//if (context == null)
			//{
			//  throw new exception.MethodParameterIsNullException("The context core node can not be null");
			//}
			//if (checkParent)
			//{
			//  if (getParent(context) == null) return null;
			//}
			//ICoreNode parent = context.getParent();
			//while (parent != null)
			//{
			//  int index = parent.indexOf(context)-1;
			//  while (index >= 0)
			//  {
			//    ICoreNode child = parent.getChild(index);
			//    if (isIncluded(child))
			//    {
			//      return child;
			//    }
			//    else
			//    {
			//      ICoreNode lastChild = getLastChild(child);
			//      if (lastChild != null) return lastChild;
			//    }
			//    index--;
			//  }
			//  if (isIncluded(parent)) break;
			//  context = parent;
			//  parent = context.getParent();
			//}
			//return null;
			#endregion

			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			if (checkParent)
			{
				if (getParent(context) == null) return null;
			}
			ICoreNode parent = context.getParent();
			while (parent != null)
			{
				ICoreNode prevUnfiltSib = context.getPreviousSibling();
				while (prevUnfiltSib != null)
				{
					if (isIncluded(prevUnfiltSib))
					{
						return prevUnfiltSib;
					}
					else
					{
						ICoreNode lastChild = getLastChild(prevUnfiltSib);
						if (lastChild != null) return lastChild;
					}
					prevUnfiltSib = prevUnfiltSib.getPreviousSibling();
				}
				if (isIncluded(parent)) break;
				context = parent;
				parent = context.getParent();
			}
			return null;
		}

		/// <summary>
		/// Finds the last child <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="context">The context <see cref="ICoreNode"/></param>
		/// <returns>The last child or <c>null</c> if the context <see cref="ICoreNode"/> has no children</returns>
		private ICoreNode getLastChild(ICoreNode context)
		{
			int index = context.getChildCount() - 1;
			while (index >= 0)
			{
				ICoreNode child = context.getChild(index);
				if (isIncluded(child))
				{
					return child;
				}
				else
				{
					child = getLastChild(child);
					if (child != null) return child;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the next sibling of a given context <see cref="ICoreNode"/> in the filtered tree
		/// </summary>
		/// <param localName="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The next sibling <see cref="ICoreNode"/> or <c>null</c> if no such <see cref="ICoreNode"/> exists.</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="context"/> is <c>null</c>
		/// </exception>
		public ICoreNode getNextSibling(ICoreNode context)
		{
			return getNextSibling(context, true);
		}
		private ICoreNode getNextSibling(ICoreNode context, bool checkParent)
		{
			#region Works but is ugly
			//if (context == null)
			//{
			//  throw new exception.MethodParameterIsNullException("The context core node can not be null");
			//}
			//if (checkParent)
			//{
			//  if (getParent(context) == null) return null;
			//}
			//ICoreNode parent = context.getParent();
			//while (parent != null)
			//{
			//  int index = parent.indexOf(context)+1;
			//  while (index < parent.getChildCount())
			//  {
			//    ICoreNode child = parent.getChild(index);
			//    if (isIncluded(child))
			//    {
			//      return child;
			//    }
			//    else
			//    {
			//      ICoreNode firstChild = getFirstChild(child);
			//      if (firstChild != null) return firstChild;
			//    }
			//    index++;
			//  }
			//  if (isIncluded(parent))
			//  {
			//    parent = null;
			//  }
			//  else
			//  {
			//    context = parent;
			//    parent = context.getParent();
			//  }
			//}
			//return null;
			#endregion
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			if (checkParent)
			{
				if (getParent(context) == null) return null;
			}
			ICoreNode parent = context.getParent();
			while (parent != null)
			{
				ICoreNode nextUnfiltSib = context.getNextSibling();
				while (nextUnfiltSib != null)
				{
					if (isIncluded(nextUnfiltSib))
					{
						return nextUnfiltSib;
					}
					else
					{
						ICoreNode firstChild = getFirstChild(nextUnfiltSib);
						if (firstChild != null) return firstChild;
					}
					nextUnfiltSib = nextUnfiltSib.getNextSibling();
				}

				if (isIncluded(parent)) break;
				context = parent;
				parent = context.getParent();
			}
			return null;
		}

		/// <summary>
		/// Finds the first
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private ICoreNode getFirstChild(ICoreNode context)
		{
			int acumIndex = 0;
			return findChildAtIndex(context, 0, ref acumIndex);
		}

		/// <summary>
		/// Gets the number of children of a given context <see cref="ICoreNode"/> in the filtered tree
		/// </summary>
		/// <param localName="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The number of children</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="context"/> is <c>null</c>
		/// </exception>
		public int getChildCount(ICoreNode context)
		{
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			List<ICoreNode> childList = new List<ICoreNode>();
			findChildren(context, childList);
			return childList.Count;
		}

		/// <summary>
		/// Recursively finds the children of a given context <see cref="ICoreNode"/> and adds 
		/// then to a given child list
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <param name="childList">The given child <see cref="List{ICoreNode}"/></param>
		private void findChildren(ICoreNode context, List<ICoreNode> childList)
		{
			for (int i = 0; i < context.getChildCount(); i++)
			{
				ICoreNode child = context.getChild(i);
				if (isIncluded(child))
				{
					childList.Add(child);
				}
				else
				{
					findChildren(child, childList);
				}
			}
		}

		/// <summary>
		/// Gets the index of a given context <see cref="ICoreNode"/> as a child of it's parent <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="context">The context <see cref="ICoreNode"/></param>
		/// <returns>
		/// The index or <c>-1</c> if <paramref name="context"/> does not have a parent
		/// </returns>
		public int indexOf(ICoreNode context)
		{
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			ICoreNode parent = getParent(context);
			if (parent == null) return -1;
			int index = 0;
			if (!findIndexOf(parent, context, ref index))
			{
				throw new exception.NodeDoesNotExistException(
					"The context core node is not a child of it's own parent");
			}
			return index;
		}

		/// <summary>
		/// Finds the index of a given <see cref="ICoreNode"/> as the child of a given context <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <param name="childToFind">The given <see cref="ICoreNode"/> child</param>
		/// <param name="index">Reference holding the index</param>
		/// <returns>A <see cref="bool"/> indicating if the index was found,
		/// that is if the child <see cref="ICoreNode"/> is in fact a child 
		/// of the given context <see cref="ICoreNode"/>
		/// </returns>
		private bool findIndexOf(ICoreNode context, ICoreNode childToFind, ref int index)
		{
			for (int i = 0; i < context.getChildCount(); i++)
			{
				ICoreNode child = context.getChild(i);
				if (isIncluded(child))
				{
					if (child == childToFind)
					{
						return true;
					}
					index++;
				}
				else if (findIndexOf(child, childToFind, ref index)) 
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Recursively finds the child <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/> 
		/// at a given index
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <param name="index">The given index</param>
		/// <param name="acumIndex">The accumulated index</param>
		/// <returns>The child <see cref="ICoreNode"/> at the given index 
		/// - <c>null</c> if there is no child at the given index</returns>
		private ICoreNode findChildAtIndex(ICoreNode context, int index, ref int acumIndex)
		{
			for (int i = 0; i < context.getChildCount(); i++)
			{
				ICoreNode child = context.getChild(i);
				if (isIncluded(child))
				{
					if (index == acumIndex) return child;
					acumIndex++;
				}
				else
				{
					ICoreNode retCh = findChildAtIndex(child, index, ref acumIndex);
					if (retCh != null) return retCh;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the child of a given context <see cref="ICoreNode"/> at a given index in the filtered tree
		/// </summary>
		/// <param localName="context">The given context <see cref="ICoreNode"/></param>
		/// <param localName="index">The given index</param>
		/// <returns>The child <see cref="ICoreNode"/> at the given index</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="context"/> is <c>null</c>
		/// </exception>
		public ICoreNode getChild(ICoreNode context, int index)
		{
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			int acumIndex = 0;
			ICoreNode res = findChildAtIndex(context, index, ref acumIndex);
			if (res == null)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The index of the child to get is out of bounds");
			}
			return res;
		}

		/// <summary>
		/// Gets the previous <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in depth first traversal order of the filtered forest
		/// </summary>
		/// <param localName="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The previous <see cref="ICoreNode"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="context"/> is <c>null</c>
		/// </exception>
		public ICoreNode getPrevious(ICoreNode context)
		{
			ICoreNode prev = getUnfilteredPrevious(context);
			while (prev != null)
			{
				if (isIncluded(prev)) return prev;
				prev = getUnfilteredPrevious(prev);
			}
			return prev;
		}

		/// <summary>
		/// Finds the previous <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in the unfiltered source tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The previous <see cref="ICoreNode"/></returns>
		private ICoreNode getUnfilteredPrevious(ICoreNode context)
		{
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			ICoreNode prev = context.getPreviousSibling();
			if (prev != null)
			{
				while (prev.getChildCount() > 0)
				{
					prev = prev.getChild(prev.getChildCount() - 1);
				}
			}
			if (prev == null)
			{
				prev = context.getParent();
			}
			return prev;
		}

		/// <summary>
		/// Gets the next <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in depth first traversal order of the filtered forest
		/// </summary>
		/// <param localName="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The next <see cref="ICoreNode"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="context"/> is <c>null</c>
		/// </exception>
		public ICoreNode getNext(ICoreNode context)
		{
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			int acumIndex = 0;
			ICoreNode next = findChildAtIndex(context, 0, ref acumIndex);
			if (next != null) return next;
			while (context!=null)
			{
				next = getNextSibling(context, false);
				if (next!=null) return next;
				context = getParent(context);
			}
			return null;
		}

		/// <summary>
		/// Finds the next <see cref="ICoreNode"/> of a given context <see cref="ICoreNode"/>
		/// in the unfiltered source tree
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <returns>The next <see cref="ICoreNode"/></returns>
		private ICoreNode getUnfilteredNext(ICoreNode context)
		{
			if (context == null)
			{
				throw new exception.MethodParameterIsNullException("The context core node can not be null");
			}
			ICoreNode prev = context.getNextSibling();
			if (prev == null)
			{
				ICoreNode contextParent = context.getParent();
				if (contextParent != null)
				{
					prev = getUnfilteredNext(contextParent);
				}
			}
			return prev;
		}



		/// <summary>
		/// Gets an enumerator enumerating the filtered sub-tree starting at a given start <see cref="ICoreNode"/>
		/// </summary>
		/// <param localName="startNode">The given</param>
		/// <returns>The enumerator</returns>
		public IEnumerator<ICoreNode> getSubForestIterator(ICoreNode startNode)
		{
			List<ICoreNode> subtree = new List<ICoreNode>();
			generateSubtree(startNode, subtree);
			return (IEnumerator<ICoreNode>)subtree.ToArray().GetEnumerator();
		}

		/// <summary>
		/// Adds any included <see cref="ICoreNode"/>s of the subtree starting at a given context <see cref="ICoreNode"/>
		/// to a given <see cref="List{ICoreNode}"/>
		/// </summary>
		/// <param name="context">The given context <see cref="ICoreNode"/></param>
		/// <param name="subtree">The given <see cref="List{ICoreNode}"/></param>
		private void generateSubtree(ICoreNode context, List<ICoreNode> subtree)
		{
			if (isIncluded(context)) subtree.Add(context);
			for (int i = 0; i < context.getChildCount(); i++)
			{
				generateSubtree(context.getChild(i), subtree);
			}
		}

		#endregion
	}
}
