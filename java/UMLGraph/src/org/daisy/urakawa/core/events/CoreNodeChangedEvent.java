package org.daisy.urakawa.core.events;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * <i>This is a part of the Event Listener design pattern (variation of the <a
 * href="http://en.wikipedia.org/wiki/Observer_pattern">Observer</a> pattern,
 * also known as Publish / Subscribe), as implemented in the API design of the
 * Urakawa SDK, to provide an event mechanism for listening to changes in the
 * data model.</i>
 * </p>
 * <p>
 * This interface should be extended to specify concrete change-event types. The
 * basic function of this interface is to provide access to the
 * {@link org.daisy.urakawa.core.CoreNode} for which the state has changed.
 * </p>
 * 
 * @see CoreNodeChangeManager
 * @see CoreNodeGenericChangeManager
 * @see CoreNodeGenericChangeManager#notifyCoreNodeChangedListeners(CoreNodeChangedEvent)
 * @see CoreNodeChangedListener#coreNodeChanged(CoreNodeChangedEvent)
 * @depend - Aggregation 1 CoreNode
 */
public interface CoreNodeChangedEvent {
	/**
	 * Sets the core node for this change event.
	 * 
	 * @stereotype Initialize
	 * @throws MethodParameterIsNullException
	 *             if node is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param node
	 *            the node. Cannot be null.
	 */
	public void setCoreNode(CoreNode node)
			throws MethodParameterIsNullException;

	/**
	 * Gets the core node for this change event.
	 * 
	 * @return the node which has changed. Cannot return null.
	 */
	public CoreNode getCoreNode() throws MethodParameterIsNullException;
}
