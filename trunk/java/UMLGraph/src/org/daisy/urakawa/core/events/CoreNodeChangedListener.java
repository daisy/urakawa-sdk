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
 * {@link CoreNodeChangedListener#coreNodeChanged(CoreNodeChangedEvent)}
 * callback method which passes the {@link CoreNodeChangedEvent} parameter that
 * specifies the nature of the change.
 * </p>
 * <p>
 * Of course, the notification happens only if this listener is known by
 * (registered at) the {@link CoreNodeGenericChangeManager}.
 * </p>
 * <p>
 * Application developers typically implement their own listeners to receive
 * change event notifications from the data model.
 * </p>
 * 
 * @see CoreNodeGenericChangeManager#registerCoreNodeChangedListener(CoreNodeChangedListener)
 * @see CoreNodeGenericChangeManager#unregisterCoreNodeChangedListener(CoreNodeChangedListener)
 * @see CoreNodeChangedEvent
 */
public interface CoreNodeChangedListener {
	/**
	 * <p>
	 * This callback method is called when a
	 * {@link org.daisy.urakawa.core.CoreNode} has changed.
	 * </p>
	 * <p>
	 * The {@link CoreNodeChangedEvent} parameter specifies the nature of the
	 * change.
	 * </p>
	 * <p>
	 * Typically, this method is called by the
	 * {@link CoreNodeGenericChangeManager} when it iterates through all its
	 * registered listeners, from the
	 * {@link CoreNodeGenericChangeManager#notifyCoreNodeChangedListeners(CoreNodeChangedEvent)}
	 * method which is called by a {@link org.daisy.urakawa.core.CoreNode} when
	 * its state has changed.
	 * </p>
	 * <p>
	 * There can be many listeners, but by design there is <b>no guarantee</b>
	 * that the order of notification will match the order of registration.
	 * </p>
	 * 
	 * @param changeEvent
	 *            Specifies the nature of the change. Cannot be null.
	 * @throws MethodParameterIsNullException
	 *             if changeEvent is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void coreNodeChanged(CoreNodeChangedEvent changeEvent)
			throws MethodParameterIsNullException;
}
