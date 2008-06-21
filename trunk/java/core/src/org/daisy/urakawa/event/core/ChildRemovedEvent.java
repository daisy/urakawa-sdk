package org.daisy.urakawa.event.core;

import org.daisy.urakawa.core.ITreeNode;

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
	public ChildRemovedEvent(ITreeNode notfr, ITreeNode child, int pos) {
		super(notfr);
		mRemovedChild = child;
		mRemovedPosition = pos;
	}

	private ITreeNode mRemovedChild;
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
	public ITreeNode getRemovedChild() {
		return mRemovedChild;
	}
}
