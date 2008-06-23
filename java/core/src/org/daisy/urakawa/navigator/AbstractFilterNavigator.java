package org.daisy.urakawa.navigator;

import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * Partial reference implementation of the interface, to let isIncluded() by
 * implemented by a derived class. An extension of INavigator to determine what
 * TreeNodes are part of the tree based on filtering/selection criteria
 * implemented by isIncluded(node).
 * 
 * @stereotype Abstract
 */
public abstract class AbstractFilterNavigator implements INavigator {
	/**
	 * This method makes the decision about whether or not the given node
	 * belongs to the virtual tree for this navigator. Determines if a given
	 * ITreeNode is included by the filter of the AbstractFilterNavigator
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
	public abstract boolean isIncluded(ITreeNode node)
			throws MethodParameterIsNullException;

	public ITreeNode getParent(ITreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		ITreeNode parent = node.getParent();
		if (parent == null)
			return null;
		if (isIncluded(parent))
			return parent;
		return getParent(parent);
	}

	private void findChildren(ITreeNode context, List<ITreeNode> childList)
			throws MethodParameterIsNullException {
		if (context == null || childList == null) {
			throw new MethodParameterIsNullException();
		}
		for (int i = 0; i < context.getChildCount(); i++) {
			ITreeNode child;
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

	public int getChildCount(ITreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		List<ITreeNode> childList = new LinkedList<ITreeNode>();
		findChildren(node, childList);
		return childList.size();
	}

	private ITreeNode findChildAtIndex(ITreeNode context, int index,
			IntWrapper acumIndex) throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
		if (context == null || acumIndex == null) {
			throw new MethodParameterIsNullException();
		}
		for (int i = 0; i < context.getChildCount(); i++) {
			ITreeNode child;
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
				ITreeNode retCh = findChildAtIndex(child, index, acumIndex);
				if (retCh != null)
					return retCh;
			}
		}
		throw new MethodParameterIsOutOfBoundsException();
	}

	private boolean findIndexOf(ITreeNode context, ITreeNode childToFind,
			IntWrapper index) throws MethodParameterIsNullException {
		if (context == null || index == null || childToFind == null) {
			throw new MethodParameterIsNullException();
		}
		for (int i = 0; i < context.getChildCount(); i++) {
			ITreeNode child;
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

	public int indexOf(ITreeNode context) throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(context)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		ITreeNode parent = getParent(context);
		if (parent == null)
			return -1;
		IntWrapper index = new IntWrapper();
		index.value = 0;
		if (!findIndexOf(parent, context, index)) {
			return -1;
		}
		return index.value;
	}

	public ITreeNode getChild(ITreeNode node, int index)
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
		ITreeNode res = findChildAtIndex(node, index, acumIndex);
		return res;
	}

	private ITreeNode getLastChild(ITreeNode context)
			throws MethodParameterIsNullException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		int index = context.getChildCount() - 1;
		while (index >= 0) {
			ITreeNode child;
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

	private ITreeNode getPreviousSibling(ITreeNode context, boolean checkParent)
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
		ITreeNode parent = context.getParent();
		ITreeNode tmpNode = context;
		while (parent != null) {
			ITreeNode prevUnfiltSib = tmpNode.getPreviousSibling();
			while (prevUnfiltSib != null) {
				if (isIncluded(prevUnfiltSib)) {
					return prevUnfiltSib;
				} else {
					ITreeNode lastChild = getLastChild(prevUnfiltSib);
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

	public ITreeNode getPreviousSibling(ITreeNode node)
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

	private ITreeNode getFirstChild(ITreeNode context)
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

	private ITreeNode getNextSibling(ITreeNode context, boolean checkParent)
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
		ITreeNode parent = context.getParent();
		while (parent != null) {
			ITreeNode tmpNode = context;
			ITreeNode nextUnfiltSib = tmpNode.getNextSibling();
			while (nextUnfiltSib != null) {
				if (isIncluded(nextUnfiltSib)) {
					return nextUnfiltSib;
				} else {
					ITreeNode firstChild = getFirstChild(nextUnfiltSib);
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

	public ITreeNode getNextSibling(ITreeNode node)
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

	private ITreeNode getUnfilteredPrevious(ITreeNode context)
			throws MethodParameterIsNullException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		ITreeNode prev = context.getPreviousSibling();
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

	public ITreeNode getPrevious(ITreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		ITreeNode prev = getUnfilteredPrevious(node);
		while (prev != null) {
			if (isIncluded(prev))
				return prev;
			prev = getUnfilteredPrevious(prev);
		}
		return prev;
	}

	public ITreeNode getNext(ITreeNode node)
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
		ITreeNode next;
		try {
			next = findChildAtIndex(node, 0, acumIndex);
		} catch (MethodParameterIsOutOfBoundsException e) {
			next = null;
			e.printStackTrace();
		}
		if (next != null)
			return next;
		ITreeNode tmpNode = node;
		while (tmpNode != null) {
			next = getNextSibling(tmpNode, false);
			if (next != null)
				return next;
			tmpNode = getParent(tmpNode);
		}
		return null;
	}

	@SuppressWarnings("unused")
	private ITreeNode getUnfilteredNext(ITreeNode context)
			throws MethodParameterIsNullException {
		if (context == null) {
			throw new MethodParameterIsNullException();
		}
		ITreeNode prev = context.getNextSibling();
		if (prev == null) {
			ITreeNode contextParent = context.getParent();
			if (contextParent != null) {
				prev = getUnfilteredNext(contextParent);
			}
		}
		return prev;
	}

	private void generateSubtree(ITreeNode context, List<ITreeNode> subtree)
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

	public Iterator<ITreeNode> getSubForestIterator(ITreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!isIncluded(node)) {
			throw new TreeNodeNotIncludedByNavigatorException();
		}
		List<ITreeNode> subtree = new LinkedList<ITreeNode>();
		generateSubtree(node, subtree);
		return subtree.listIterator();
	}
}
