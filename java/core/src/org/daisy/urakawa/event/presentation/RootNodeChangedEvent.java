package org.daisy.urakawa.event.presentation;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.core.TreeNode;

/**
 * 
 *
 */
public class RootNodeChangedEvent extends PresentationEvent {
	/**
	 * @param source
	 * @param newRoot
	 * @param prevRoot
	 */
	public RootNodeChangedEvent(Presentation source, TreeNode newRoot,
			TreeNode prevRoot) {
		super(source);
		mNewRootNode = newRoot;
		mPreviousRootNode = prevRoot;
	}

	private TreeNode mNewRootNode;
	private TreeNode mPreviousRootNode;

	/**
	 * @return node
	 */
	public TreeNode getPreviousRootNode() {
		return mPreviousRootNode;
	}

	/**
	 * @return node
	 */
	public TreeNode getNewRootNode() {
		return mNewRootNode;
	}
}
