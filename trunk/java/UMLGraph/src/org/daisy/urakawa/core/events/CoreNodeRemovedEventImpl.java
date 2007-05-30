package org.daisy.urakawa.core.events;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 */
public class CoreNodeRemovedEventImpl implements CoreNodeRemovedEvent {
	/**
	 * @hidden
	 */
	public CoreNode getFormerParent() {
		return null;
	}

	/**
	 * @hidden
	 */
	public int getFormerPosition() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public CoreNode getCoreNode() throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setCoreNode(CoreNode node)
			throws MethodParameterIsNullException {
	}
}
