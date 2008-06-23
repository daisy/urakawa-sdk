package org.daisy.urakawa.event.core;

import org.daisy.urakawa.core.TreeNode;

/**
 *
 *
 */
public class ChildAddedEvent extends TreeNodeEvent {
	private TreeNode mAddedChild;

	/**
	 * @param notfr
	 * @param child
	 */
	public ChildAddedEvent(TreeNode notfr, TreeNode child) {
		super(notfr);
		mAddedChild = child;
	}

	/**
	 * @return node
	 */
	public TreeNode getAddedChild() {
		return mAddedChild;
	}
}
