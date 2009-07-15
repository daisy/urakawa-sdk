using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
    /// <summary>
    /// An abstract class providing the main functionality of a <see cref="AbstractFilterNavigator"/>,
    /// that is a <see cref="INavigator"/> navigating a forest of <see cref="TreeNode"/> obtained by filtering
    /// an actual <see cref="TreeNode"/> tree
    /// </summary>
    public abstract class AbstractFilterNavigator : INavigator
    {
        /// <summary>
        /// Determines if a given <see cref="TreeNode"/> is included by the filter of the <see cref="AbstractFilterNavigator"/> instance.
        /// Concrete classes must implement this method to determine the behaviour of the filter navigator
        /// </summary>
        /// <param name="node">The given <see cref="TreeNode"/></param>
        /// <returns>A <see cref="bool"/> indicating if <paramref localName="node"/> is included by the filter of the
        /// <see cref="AbstractFilterNavigator"/> instance</returns>
        /// <remarks>In derived concrete classes the implementation of this abstract method defines the filter</remarks>
        public abstract bool IsIncluded(TreeNode node);

        #region INavigator Members

        /// <summary>
        /// Gets the parent <see cref="TreeNode"/> of a given context <see cref="TreeNode"/> in the filtered tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The parent <see cref="TreeNode"/> or <c>null</c> if no such <see cref="TreeNode"/> exists.</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="context"/> is <c>null</c>
        /// </exception>
        public TreeNode GetParent(TreeNode context)
        {
            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            TreeNode parent = context.Parent;
            if (parent == null) return null;
            if (IsIncluded(parent)) return parent;
            return GetParent(parent);
        }


        /// <summary>
        /// Gets the previous sibling of a given context <see cref="TreeNode"/> in the filtered tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The previous sibling <see cref="TreeNode"/> or <c>null</c> if no such <see cref="TreeNode"/> exists.</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="context"/> is <c>null</c>
        /// </exception>
        public TreeNode GetPreviousSibling(TreeNode context)
        {
            return GetPreviousSibling(context, true);
        }

        private TreeNode GetPreviousSibling(TreeNode context, bool checkParent)
        {
            #region Works but is considered ugly

            //if (context == null)
            //{
            //  throw new exception.MethodParameterIsNullException("The context core node can not be null");
            //}
            //if (checkParent)
            //{
            //  if (GetParent(context) == null) return null;
            //}
            //TreeNode parent = context.GetParent();
            //while (parent != null)
            //{
            //  int index = parent.IndexOf(context)-1;
            //  while (index >= 0)
            //  {
            //    TreeNode child = parent.GetChild(index);
            //    if (IsIncluded(child))
            //    {
            //      return child;
            //    }
            //    else
            //    {
            //      TreeNode lastChild = GetLastChild(child);
            //      if (lastChild != null) return lastChild;
            //    }
            //    index--;
            //  }
            //  if (IsIncluded(parent)) break;
            //  context = parent;
            //  parent = context.GetParent();
            //}
            //return null;

            #endregion

            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            if (checkParent)
            {
                if (GetParent(context) == null) return null;
            }
            TreeNode parent = context.Parent;
            while (parent != null)
            {
                TreeNode prevUnfiltSib = context.PreviousSibling;
                while (prevUnfiltSib != null)
                {
                    if (IsIncluded(prevUnfiltSib))
                    {
                        return prevUnfiltSib;
                    }
                    else
                    {
                        TreeNode lastChild = GetLastChild(prevUnfiltSib);
                        if (lastChild != null) return lastChild;
                    }
                    prevUnfiltSib = prevUnfiltSib.PreviousSibling;
                }
                if (IsIncluded(parent)) break;
                context = parent;
                parent = context.Parent;
            }
            return null;
        }

        /// <summary>
        /// Finds the last child <see cref="TreeNode"/> of a given context <see cref="TreeNode"/>
        /// </summary>
        /// <param name="context">The context <see cref="TreeNode"/></param>
        /// <returns>The last child or <c>null</c> if the context <see cref="TreeNode"/> has no children</returns>
        private TreeNode GetLastChild(TreeNode context)
        {
            int index = context.Children.Count - 1;
            while (index >= 0)
            {
                TreeNode child = context.Children.Get(index);
                if (IsIncluded(child))
                {
                    return child;
                }
                child = GetLastChild(child);
                if (child != null) return child;
            }
            return null;
        }

        /// <summary>
        /// Gets the next sibling of a given context <see cref="TreeNode"/> in the filtered tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The next sibling <see cref="TreeNode"/> or <c>null</c> if no such <see cref="TreeNode"/> exists.</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="context"/> is <c>null</c>
        /// </exception>
        public TreeNode GetNextSibling(TreeNode context)
        {
            return GetNextSibling(context, true);
        }

        private TreeNode GetNextSibling(TreeNode context, bool checkParent)
        {
            #region Works but is ugly

            //if (context == null)
            //{
            //  throw new exception.MethodParameterIsNullException("The context core node can not be null");
            //}
            //if (checkParent)
            //{
            //  if (GetParent(context) == null) return null;
            //}
            //TreeNode parent = context.GetParent();
            //while (parent != null)
            //{
            //  int index = parent.IndexOf(context)+1;
            //  while (index < parent.GetChildCount())
            //  {
            //    TreeNode child = parent.GetChild(index);
            //    if (IsIncluded(child))
            //    {
            //      return child;
            //    }
            //    else
            //    {
            //      TreeNode firstChild = GetFirstChild(child);
            //      if (firstChild != null) return firstChild;
            //    }
            //    index++;
            //  }
            //  if (IsIncluded(parent))
            //  {
            //    parent = null;
            //  }
            //  else
            //  {
            //    context = parent;
            //    parent = context.GetParent();
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
                if (GetParent(context) == null) return null;
            }
            TreeNode parent = context.Parent;
            while (parent != null)
            {
                TreeNode nextUnfiltSib = context.NextSibling;
                while (nextUnfiltSib != null)
                {
                    if (IsIncluded(nextUnfiltSib))
                    {
                        return nextUnfiltSib;
                    }
                    TreeNode firstChild = GetFirstChild(nextUnfiltSib);
                    if (firstChild != null) return firstChild;
                    nextUnfiltSib = nextUnfiltSib.NextSibling;
                }

                if (IsIncluded(parent)) break;
                context = parent;
                parent = context.Parent;
            }
            return null;
        }

        /// <summary>
        /// Finds the first
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private TreeNode GetFirstChild(TreeNode context)
        {
            int acumIndex = 0;
            return FindChildAtIndex(context, 0, ref acumIndex);
        }

        /// <summary>
        /// Gets the number of children of a given context <see cref="TreeNode"/> in the filtered tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The number of children</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="context"/> is <c>null</c>
        /// </exception>
        public int GetChildCount(TreeNode context)
        {
            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            List<TreeNode> childList = new List<TreeNode>();
            FindChildren(context, childList);
            return childList.Count;
        }

        /// <summary>
        /// Recursively finds the children of a given context <see cref="TreeNode"/> and adds 
        /// then to a given child list
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <param name="childList">The given child <see cref="List{TreeNode}"/></param>
        private void FindChildren(TreeNode context, List<TreeNode> childList)
        {
            for (int i = 0; i < context.Children.Count; i++)
            {
                TreeNode child = context.Children.Get(i);
                if (IsIncluded(child))
                {
                    childList.Add(child);
                }
                else
                {
                    FindChildren(child, childList);
                }
            }
        }

        /// <summary>
        /// Gets the index of a given context <see cref="TreeNode"/> as a child of it's parent <see cref="TreeNode"/>
        /// </summary>
        /// <param name="context">The context <see cref="TreeNode"/></param>
        /// <returns>
        /// The index or <c>-1</c> if <paramref name="context"/> does not have a parent
        /// </returns>
        public int IndexOf(TreeNode context)
        {
            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            TreeNode parent = GetParent(context);
            if (parent == null) return -1;
            int index = 0;
            if (!FindIndexOf(parent, context, ref index))
            {
                throw new exception.NodeDoesNotExistException(
                    "The context core node is not a child of it's own parent");
            }
            return index;
        }

        /// <summary>
        /// Finds the index of a given <see cref="TreeNode"/> as the child of a given context <see cref="TreeNode"/>
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <param name="childToFind">The given <see cref="TreeNode"/> child</param>
        /// <param name="index">Reference holding the index</param>
        /// <returns>A <see cref="bool"/> indicating if the index was found,
        /// that is if the child <see cref="TreeNode"/> is in fact a child 
        /// of the given context <see cref="TreeNode"/>
        /// </returns>
        private bool FindIndexOf(TreeNode context, TreeNode childToFind, ref int index)
        {
            for (int i = 0; i < context.Children.Count; i++)
            {
                TreeNode child = context.Children.Get(i);
                if (IsIncluded(child))
                {
                    if (child == childToFind)
                    {
                        return true;
                    }
                    index++;
                }
                else if (FindIndexOf(child, childToFind, ref index))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Recursively finds the child <see cref="TreeNode"/> of a given context <see cref="TreeNode"/> 
        /// at a given index
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <param name="index">The given index</param>
        /// <param name="acumIndex">The accumulated index</param>
        /// <returns>The child <see cref="TreeNode"/> at the given index 
        /// - <c>null</c> if there is no child at the given index</returns>
        private TreeNode FindChildAtIndex(TreeNode context, int index, ref int acumIndex)
        {
            for (int i = 0; i < context.Children.Count; i++)
            {
                TreeNode child = context.Children.Get(i);
                if (IsIncluded(child))
                {
                    if (index == acumIndex) return child;
                    acumIndex++;
                }
                else
                {
                    TreeNode retCh = FindChildAtIndex(child, index, ref acumIndex);
                    if (retCh != null) return retCh;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the child of a given context <see cref="TreeNode"/> at a given index in the filtered tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <param name="index">The given index</param>
        /// <returns>The child <see cref="TreeNode"/> at the given index</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="context"/> is <c>null</c>
        /// </exception>
        public TreeNode GetChild(TreeNode context, int index)
        {
            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            int acumIndex = 0;
            TreeNode res = FindChildAtIndex(context, index, ref acumIndex);
            if (res == null)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The index of the child to get is out of bounds");
            }
            return res;
        }

        /// <summary>
        /// Gets the previous <see cref="TreeNode"/> of a given context <see cref="TreeNode"/>
        /// in depth first traversal order of the filtered forest
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The previous <see cref="TreeNode"/></returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="context"/> is <c>null</c>
        /// </exception>
        public TreeNode GetPrevious(TreeNode context)
        {
            TreeNode prev = GetUnfilteredPrevious(context);
            while (prev != null)
            {
                if (IsIncluded(prev)) return prev;
                prev = GetUnfilteredPrevious(prev);
            }
            return prev;
        }

        /// <summary>
        /// Finds the previous <see cref="TreeNode"/> of a given context <see cref="TreeNode"/>
        /// in the unfiltered source tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The previous <see cref="TreeNode"/></returns>
        private TreeNode GetUnfilteredPrevious(TreeNode context)
        {
            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            TreeNode prev = context.PreviousSibling;
            if (prev != null)
            {
                while (prev.Children.Count > 0)
                {
                    prev = prev.Children.Get(prev.Children.Count - 1);
                }
            }
            if (prev == null)
            {
                prev = context.Parent;
            }
            return prev;
        }

        /// <summary>
        /// Gets the next <see cref="TreeNode"/> of a given context <see cref="TreeNode"/>
        /// in depth first traversal order of the filtered forest
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The next <see cref="TreeNode"/></returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="context"/> is <c>null</c>
        /// </exception>
        public TreeNode GetNext(TreeNode context)
        {
            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            int acumIndex = 0;
            TreeNode next = FindChildAtIndex(context, 0, ref acumIndex);
            if (next != null) return next;
            while (context != null)
            {
                next = GetNextSibling(context, false);
                if (next != null) return next;
                context = GetParent(context);
            }
            return null;
        }

        /// <summary>
        /// Finds the next <see cref="TreeNode"/> of a given context <see cref="TreeNode"/>
        /// in the unfiltered source tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The next <see cref="TreeNode"/></returns>
        private TreeNode GetUnfilteredNext(TreeNode context)
        {
            if (context == null)
            {
                throw new exception.MethodParameterIsNullException("The context core node can not be null");
            }
            TreeNode next = context.NextSibling;
            if (next == null)
            {
                TreeNode contextParent = context.Parent;
                if (contextParent != null)
                {
                    next = GetUnfilteredNext(contextParent);
                }
            }
            return next;
        }

        /// <summary>
        /// Gets an list enumerating the filtered sub-tree starting at a given start <see cref="TreeNode"/>
        /// </summary>
        /// <param name="startNode">The given</param>
        /// <returns>The enumerator</returns>
        public List<TreeNode> GetSubForestIterator(TreeNode startNode)
        {
            List<TreeNode> subtree = new List<TreeNode>();
            GenerateSubtree(startNode, ref subtree);
            //return (IEnumerator<TreeNode>) subtree.ToArray().GetEnumerator();
            return subtree;
        }

        /// <summary>
        /// Adds any included <see cref="TreeNode"/>s of the subtree starting at a given context <see cref="TreeNode"/>
        /// to a given <see cref="List{TreeNode}"/>
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <param name="subtree">The given <see cref="List{TreeNode}"/></param>
        private void GenerateSubtree(TreeNode context, ref List<TreeNode> subtree)
        {
            if (IsIncluded(context)) subtree.Add(context);
            for (int i = 0; i < context.Children.Count; i++)
            {
                GenerateSubtree(context.Children.Get(i), ref subtree);
            }
        }

        #endregion
    }
}