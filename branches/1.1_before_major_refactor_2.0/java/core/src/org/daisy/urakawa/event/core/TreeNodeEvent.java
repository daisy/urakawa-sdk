package org.daisy.urakawa.event.core;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.event.DataModelChangedEvent;

/**
 * 
 *
 */
public class TreeNodeEvent extends DataModelChangedEvent {
	/**
	 * @param src
	 */
	public TreeNodeEvent(TreeNode src) {
		super(src);
		mSourceTreeNode = src;
	}

	private TreeNode mSourceTreeNode;

	/**
	 * @return node
	 */
	public TreeNode getSourceTreeNode() {
		return mSourceTreeNode;
	}
}
