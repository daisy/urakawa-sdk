package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * Set of read-only methods for navigating (accessing the nodes of) a forest of
 * trees (actually an ordered list). The forest has zero or more trees made of
 * TreeNodes.
 * 
 * @depend - - - org.daisy.urakawa.navigator.TreeNodeIterator
 */
public interface Navigator {
	/**
	 * @param node
	 * @return
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-TreeNodeNotIncludedByNavigator"
	 */
	public TreeNode getPreviousInDepthFirstOrder(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException,
			MethodParameterIsNullException;

	/**
	 * @param node
	 * @return
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-TreeNodeNotIncludedByNavigator"
	 */
	public TreeNode getNextInDepthFirstOrder(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException,
			MethodParameterIsNullException;

	/**
	 * @param node
	 * @return
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-TreeNodeNotIncludedByNavigator"
	 */
	public TreeNodeIterator getDepthFirstOrderIterator(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException,
			MethodParameterIsNullException;

	/**
	 * @param node
	 *            the base node for which to return the parent. Cannot be NULL,
	 *            and must be part of the forest (must be included by the
	 *            navigator).
	 * @return the parent of the given node, in the context of this navigator.
	 *         If NULL, then node is the root of a tree. To access another tree
	 *         of the forest, Next/prev sibling() can be used.
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-TreeNodeNotIncludedByNavigator"
	 */
	public TreeNode getParent(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException;

	/**
	 * @param node
	 *            the base node for which to return the children. Cannot be
	 *            NULL, and must be part of the forest (must be included by the
	 *            navigator).
	 * @return the number (zero or more) of children for the given node, in the
	 *         context of this navigator.
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions
	 *           "MethodParameterIsNull-TreeNodeNotIncludedByNavigator"
	 */
	public int getChildCount(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException;

	/**
	 * @param node
	 *            the base node for which to return the child. Cannot be NULL,
	 *            and must be part of the forest (must be included by the
	 *            navigator).
	 * @param index
	 *            must be a value between 0 and getNumberOfChildren(node)-1, in
	 *            the context of this navigator.
	 * @return the non-NULL child at position index for the given node, in the
	 *         context of this navigator.
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 * @tagvalue Exceptions
	 *           "MethodParameterIsNull-TreeNodeNotIncludedByNavigator-MethodParameterIsOutOfBounds"
	 */
	public TreeNode getChild(TreeNode node, int index)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException,
			MethodParameterIsOutOfBoundsException;

	/**
	 * @param node
	 *            the base node for which to return the sibling. Cannot be NULL,
	 *            and must be part of the forest (must be included by the
	 *            navigator).
	 * @return the previous sibling for the given node, in the context of this
	 *         navigator (can be NULL).
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-TreeNodeNotIncludedByNavigator"
	 */
	public TreeNode getPreviousSibling(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException;

	/**
	 * @param node
	 *            the base node for which to return the sibling. Cannot be NULL,
	 *            and must be part of the forest (must be included by the
	 *            navigator).
	 * @return the next sibling for the given node, in the context of this
	 *         navigator (can be NULL).
	 * @throws TreeNodeNotIncludedByNavigatorException
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-TreeNodeNotIncludedByNavigator"
	 */
	public TreeNode getNextSibling(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException;

	/**
	 * @param node
	 *            the base node for which to return the previous node. Cannot be
	 *            NULL, but is not necesseraly part of the forest (not included
	 *            by the navigator).
	 * @return previous node in the Depth First Traversal Order (can be NULL),
	 *         in the context of this navigator (is included by the navigator,
	 *         is part of the forest) .
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public TreeNode getPrevious(TreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * @param node
	 *            the base node for which to return the next node. Cannot be
	 *            NULL, but is not necesseraly part of the forest (not included
	 *            by the navigator).
	 * @return next node in the Depth First Traversal Order (can be NULL), in
	 *         the context of this navigator (is included by the navigator, is
	 *         part of the forest).
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public TreeNode getNext(TreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * @param node
	 *            the base node for which to return the corresponding iterator.
	 *            Cannot be NULL, but is not necesseraly part of the forest (not
	 *            included by the navigator).
	 * @return an iterator of TreeNodes (included by the navigator, part of the
	 *         forest), within the boundaries of the subtree which root is the
	 *         given node. Can be empty, but not NULL.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public TreeNodeIterator getSubTreeIterator(TreeNode node)
			throws MethodParameterIsNullException;
}
