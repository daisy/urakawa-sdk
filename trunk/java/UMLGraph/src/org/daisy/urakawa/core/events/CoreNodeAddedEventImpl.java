package org.daisy.urakawa.core.events;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

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
