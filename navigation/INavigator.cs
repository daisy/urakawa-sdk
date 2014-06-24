using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
    /// <summary>
    /// Generic interface for a <see cref="TreeNode"/> forest navigator providing methods for navigation 
    /// but not for manipulation of virtual trees in a virtual forest
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Gets the parent <see cref="TreeNode"/> of a given context <see cref="TreeNode"/> in the virtual tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The parent <see cref="TreeNode"/> or <c>null</c> if no such <see cref="TreeNode"/> exists.</returns>
        TreeNode GetParent(TreeNode context);

        /// <summary>
        /// Gets the previous sibling of a given context <see cref="TreeNode"/> in the virtual tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The previous sibling <see cref="TreeNode"/> or <c>null</c> if no such <see cref="TreeNode"/> exists.</returns>
        TreeNode GetPreviousSibling(TreeNode context);

        /// <summary>
        /// Gets the next sibling of a given context <see cref="TreeNode"/> in the virtual tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The next sibling <see cref="TreeNode"/> or <c>null</c> if no such <see cref="TreeNode"/> exists.</returns>
        TreeNode GetNextSibling(TreeNode context);

        /// <summary>
        /// Gets the number of children of a given context <see cref="TreeNode"/> in the virtual tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The number of children</returns>
        int GetChildCount(TreeNode context);

        /// <summary>
        /// Gets the child of a given context <see cref="TreeNode"/> at a given index in the virtual tree
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <param name="index">The given index</param>
        /// <returns>The child <see cref="TreeNode"/> at the given index</returns>
        TreeNode GetChild(TreeNode context, int index);

        /// <summary>
        /// Gets the previous <see cref="TreeNode"/> of a given context <see cref="TreeNode"/>
        /// in depth first traversal order of the virtual forest
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The previous <see cref="TreeNode"/></returns>
        TreeNode GetPrevious(TreeNode context);

        /// <summary>
        /// Gets the next <see cref="TreeNode"/> of a given context <see cref="TreeNode"/>
        /// in depth first traversal order of the virtual forest
        /// </summary>
        /// <param name="context">The given context <see cref="TreeNode"/></param>
        /// <returns>The next <see cref="TreeNode"/></returns>
        TreeNode GetNext(TreeNode context);

        /// <summary>
        /// Gets an list enumerating the virtual sub-forest starting at a given start <see cref="TreeNode"/>
        /// </summary>
        /// <param name="startNode">The given</param>
        /// <returns>The enumerator</returns>
        List<TreeNode> GetSubForestIterator(TreeNode startNode);
    }
}