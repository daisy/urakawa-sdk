package org.daisy.urakawa.core.event.impl;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.core.event.CoreNodeRemovedEvent;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

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
