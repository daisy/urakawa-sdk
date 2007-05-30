package org.daisy.urakawa.core.events;

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
 * Classes that implement this interface are notified of
 * {@link org.daisy.urakawa.core.CoreNode} changes, via the
 * {@link CoreNodeAddedRemovedListener#coreNodeAdded(CoreNodeAddedEvent)} or
 * {@link CoreNodeAddedRemovedListener#coreNodeRemoved(CoreNodeRemovedEvent)}
 * callback methods which pass the {@link CoreNodeAddedEvent} or
 * {@link CoreNodeRemovedEvent} parameter that specifies the information for the
 * change.
 * </p>
 * <p>
 * Of course, the notification happens only if this listener is known by
 * (registered at) the {@link CoreNodeAdditionRemovalManager}.
 * </p>
 * <p>
 * Application developers typically implement their own listeners to receive
 * change event notifications from the data model.
 * </p>
 * 
 * @see CoreNodeRemovedEvent
 * @see CoreNodeAddedEvent
 * @see CoreNodeAdditionRemovalManager#registerCoreNodeAddedRemovedListener(CoreNodeAddedRemovedListener)
 * @see CoreNodeAdditionRemovalManager#unregisterCoreNodeAddedRemovedListener(CoreNodeAddedRemovedListener)
 */
public interface CoreNodeAddedRemovedListener {
	/**
	 * <p>
	 * This callback method is called when a
	 * {@link org.daisy.urakawa.core.CoreNode} has been added.
	 * </p>
	 * <p>
	 * The {@link CoreNodeAddedEvent} parameter specifies the information for
	 * the change.
	 * </p>
	 * <p>
	 * Typically, this method is called by the {@link CoreNodeAdditionRemovalManager}
	 * when it iterates through all its registered listeners, from the
	 * {@link CoreNodeAdditionRemovalManager#notifyCoreNodeAddedListeners(CoreNodeAddedEvent)}
	 * method which is called by a {@link org.daisy.urakawa.core.CoreNode} when
	 * its state has changed.
	 * </p>
	 * <p>
	 * There can be many listeners, but by design there is <b>no guarantee</b>
	 * that the order of notification will match the order of registration.
	 * </p>
	 * 
	 * @param changeEvent
	 *            Specifies the information for the change. Cannot be null.
	 * @throws MethodParameterIsNullException
	 *             if changeEvent is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void coreNodeAdded(CoreNodeAddedEvent changeEvent)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * This callback method is called when a
	 * {@link org.daisy.urakawa.core.CoreNode} has been removed.
	 * </p>
	 * <p>
	 * The {@link CoreNodeRemovedEvent} parameter specifies the information for
	 * the change.
	 * </p>
	 * <p>
	 * Typically, this method is called by the {@link CoreNodeAdditionRemovalManager}
	 * when it iterates through all its registered listeners, from the
	 * {@link CoreNodeAdditionRemovalManager#notifyCoreNodeRemovedListeners(CoreNodeRemovedEvent)}
	 * method which is called by a {@link org.daisy.urakawa.core.CoreNode} when
	 * its state has changed.
	 * </p>
	 * <p>
	 * There can be many listeners, but by design there is <b>no guarantee</b>
	 * that the order of notification will match the order of registration.
	 * </p>
	 * 
	 * @param changeEvent
	 *            Specifies the information for the change. Cannot be null.
	 * @throws MethodParameterIsNullException
	 *             if changeEvent is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void coreNodeRemoved(CoreNodeRemovedEvent changeEvent)
			throws MethodParameterIsNullException;
}
