package org.daisy.urakawa.event.core;

import org.daisy.urakawa.core.TreeNode;

/**
 *
 *
 */
public class ChildRemovedEvent extends TreeNodeEvent {
	/**
	 * @param notfr
	 * @param child
	 * @param pos
	 */
	public ChildRemovedEvent(TreeNode notfr, TreeNode child, int pos) {
		super(notfr);
		mRemovedChild = child;
		mRemovedPosition = pos;
	}

	private TreeNode mRemovedChild;
	private int mRemovedPosition;

	/**
	 * @return pos
	 */
	public int getRemovedPosition() {
		return mRemovedPosition;
	}

	/**
	 * @return node
	 */
	public TreeNode getRemovedChild() {
		return mRemovedChild;
	}
}
