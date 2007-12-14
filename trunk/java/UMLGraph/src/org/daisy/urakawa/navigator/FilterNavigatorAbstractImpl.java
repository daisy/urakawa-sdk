package org.daisy.urakawa.navigator;

import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * Partial reference implementation of the interface, to let isIncluded() by
 * implemented by a derived class. An extension of Navigator to determine what
 * TreeNodes are part of the tree based on filtering/selection criteria
 * implemented by isIncluded(node).
 * 
 * @stereotype Abstract
 */
public abstract class FilterNavigatorAbstractImpl implements Navigator {
	/**
	 * This method makes the decision about whether or not the given node
	 * belongs to the virtual tree for this navigator. Determines if a given
	 * TreeNode is included by the filter of the AbstractFilterNavigator
	 * instance.Concrete classes must implement this method to determine the
	 * behaviour of the filter navigator
	 * 
	 * @param node
	 *            the node to check
	 * @return true if the node is included in the resulting virtual tree, based
	 *         on the filtering/selection criteria implemented by this method.
	 * @throws MethodParameterIsNullException
	 * @stereotype Abstract
	 */
	public abstract boolean isIncluded(TreeNode node)
			throws MethodParameterIsNullException;

	public TreeNode getParent(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		TreeNode parent = node.getParent();
		if (parent == null)
			return null;
		if (isIncluded(parent))
			return parent;
		return getParent(parent);
	}

	private void findChildren(TreeNode context, List<TreeNode> childList)
			throws MethodParameterIsNullException {
		if (context == null || childList == null) {
			throw new MethodParameterIsNullException();
		}
		for (int i = 0; i < context.getChildCount(); i++) {
			TreeNode child;
			try {
				child = context.getChild(i);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (isIncluded(child)) {
				childList.add(child);
			} else {
				findChildren(child, childList);
			}
		}
	}

	public int getChildCount(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		List<TreeNode> childList = new LinkedList<TreeNode>();
		findChildren(node, childList);
		return childList.size();
	}

	private TreeNode findChildAtIndex(TreeNode context, int index,
			IntWrapper acumIndex) throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
		if (context == null || acumIndex == null) {
			throw new MethodParameterIsNullException();
		}
		for (int i = 0; i < context.getChildCount(); i++) {
			TreeNode child;
			try {
				child = context.getChild(i);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (isIncluded(child)) {
				if (index == acumIndex.value)
					return child;
				acumIndex.value++;
			} else {
				TreeNode retCh = findChildAtIndex(child, index, acumIndex);
				if (retCh != null)
					return retCh;
			}
		}
		throw new MethodParameterIsOutOfBoundsException();
	}

	private boolean findIndexOf(TreeNode context, TreeNode childToFind,
			IntWrapper index) throws MethodParameterIsNullException {
		if (context == null || index == null || childToFind == null) {
			throw new MethodParameterIsNullException();
		}
		for (int i = 0; i < context.getChildCount(); i++) {
			TreeNode child;
			try {
				child = context.getChild(i);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (isIncluded(child)) {
				if (child == childToFind) {
					return true;
				}
				index.value++;
			} else if (findIndexOf(child, childToFind, index)) {
				return true;
			}
		}
		return false;
	}

	public int indexOf(TreeNode context) throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(context)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		TreeNode parent = getParent(context);
		if (parent == null)
			return -1;
		IntWrapper index = new IntWrapper();
		index.value = 0;
		if (!findIndexOf(parent, context, index)) {
			return -1;
		}
		return index.value;
	}

	public TreeNode getChild(TreeNode node, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		IntWrapper acumIndex = new IntWrapper();
		acumIndex.value = 0;
		TreeNode res = findChildAtIndex(node, index, acumIndex);
		return res;
	}

	private TreeNode getLastChild(TreeNode context)
			throws MethodParameterIsNullException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		int index = context.getChildCount() - 1;
		while (index >= 0) {
			TreeNode child;
			try {
				child = context.getChild(index);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (isIncluded(child)) {
				return child;
			} else {
				child = getLastChild(child);
				if (child != null)
					return child;
			}
		}
		return null;
	}

	private TreeNode getPreviousSibling(TreeNode context, boolean checkParent)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(context)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		if (checkParent) {
			if (getParent(context) == null)
				return null;
		}
		TreeNode parent = context.getParent();
		TreeNode tmpNode = context;
		while (parent != null) {
			TreeNode prevUnfiltSib = tmpNode.getPreviousSibling();
			while (prevUnfiltSib != null) {
				if (isIncluded(prevUnfiltSib)) {
					return prevUnfiltSib;
				} else {
					TreeNode lastChild = getLastChild(prevUnfiltSib);
					if (lastChild != null)
						return lastChild;
				}
				prevUnfiltSib = prevUnfiltSib.getPreviousSibling();
			}
			if (isIncluded(parent))
				break;
			tmpNode = parent;
			parent = tmpNode.getParent();
		}
		return null;
	}

	public TreeNode getPreviousSibling(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		return getPreviousSibling(node, true);
	}

	private TreeNode getFirstChild(TreeNode context)
			throws MethodParameterIsNullException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		IntWrapper acumIndex = new IntWrapper();
		acumIndex.value = 0;
		try {
			return findChildAtIndex(context, 0, acumIndex);
		} catch (MethodParameterIsOutOfBoundsException e) {
			e.printStackTrace();
			return null;
		}
	}

	private TreeNode getNextSibling(TreeNode context, boolean checkParent)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(context)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		if (checkParent) {
			if (getParent(context) == null)
				return null;
		}
		TreeNode parent = context.getParent();
		while (parent != null) {
			TreeNode tmpNode = context;
			TreeNode nextUnfiltSib = tmpNode.getNextSibling();
			while (nextUnfiltSib != null) {
				if (isIncluded(nextUnfiltSib)) {
					return nextUnfiltSib;
				} else {
					TreeNode firstChild = getFirstChild(nextUnfiltSib);
					if (firstChild != null)
						return firstChild;
				}
				nextUnfiltSib = nextUnfiltSib.getNextSibling();
			}
			if (isIncluded(parent))
				break;
			tmpNode = parent;
			parent = tmpNode.getParent();
		}
		return null;
	}

	public TreeNode getNextSibling(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		return getNextSibling(node, true);
	}

	private TreeNode getUnfilteredPrevious(TreeNode context)
			throws MethodParameterIsNullException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		TreeNode prev = context.getPreviousSibling();
		if (prev != null) {
			while (prev.getChildCount() > 0) {
				try {
					prev = prev.getChild(prev.getChildCount() - 1);
				} catch (MethodParameterIsOutOfBoundsException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
		if (prev == null) {
			prev = context.getParent();
		}
		return prev;
	}

	public TreeNode getPrevious(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		TreeNode prev = getUnfilteredPrevious(node);
		while (prev != null) {
			if (isIncluded(prev))
				return prev;
			prev = getUnfilteredPrevious(prev);
		}
		return prev;
	}

	public TreeNode getNext(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		IntWrapper acumIndex = new IntWrapper();
		acumIndex.value = 0;
		TreeNode next;
		try {
			next = findChildAtIndex(node, 0, acumIndex);
		} catch (MethodParameterIsOutOfBoundsException e) {
			next = null;
			e.printStackTrace();
		}
		if (next != null)
			return next;
		TreeNode tmpNode = node;
		while (tmpNode != null) {
			next = getNextSibling(tmpNode, false);
			if (next != null)
				return next;
			tmpNode = getParent(tmpNode);
		}
		return null;
	}

	@SuppressWarnings("unused")
	private TreeNode getUnfilteredNext(TreeNode context)
			throws MethodParameterIsNullException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		TreeNode prev = context.getNextSibling();
		if (prev == null) {
			TreeNode contextParent = context.getParent();
			if (contextParent != null) {
				prev = getUnfilteredNext(contextParent);
			}
		}
		return prev;
	}

	private void generateSubtree(TreeNode context, List<TreeNode> subtree)
			throws MethodParameterIsNullException {
		if (context == null || subtree == null) {
			throw new MethodParameterIsNullException();
		}
		if (isIncluded(context))
			subtree.add(context);
		for (int i = 0; i < context.getChildCount(); i++) {
			try {
				generateSubtree(context.getChild(i), subtree);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public Iterator<TreeNode> getSubForestIterator(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		List<TreeNode> subtree = new LinkedList<TreeNode>();
		generateSubtree(node, subtree);
		return subtree.listIterator();
	}
}
