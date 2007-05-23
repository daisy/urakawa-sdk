package org.daisy.urakawa.core.event.impl;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.core.event.CoreNodeAddedEvent;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

public class CoreNodeAddedEventImpl implements CoreNodeAddedEvent {
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
