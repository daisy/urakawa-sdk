package org.daisy.urakawa.event.presentation;

import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.core.ITreeNode;

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
	public RootNodeChangedEvent(IPresentation source, ITreeNode newRoot,
			ITreeNode prevRoot) {
		super(source);
		mNewRootNode = newRoot;
		mPreviousRootNode = prevRoot;
	}

	private ITreeNode mNewRootNode;
	private ITreeNode mPreviousRootNode;

	/**
	 * @return node
	 */
	public ITreeNode getPreviousRootNode() {
		return mPreviousRootNode;
	}

	/**
	 * @return node
	 */
	public ITreeNode getNewRootNode() {
		return mNewRootNode;
	}
}
