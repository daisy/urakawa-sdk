package org.daisy.urakawa.core.event;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

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
 * {@link CoreNodeChangeListener#coreNodeChanged(CoreNodeChangeEvent)} callback
 * method which passes the {@link CoreNodeChangeEvent} parameter that specifies
 * the nature of the change.
 * </p>
 * <p>
 * Of course, the notification happens only if this listener is known by
 * (registered at) the {@link CoreNodeChangeManager}.
 * </p>
 * 
 * @see CoreNodeChangeManager#registerCoreNodeChangeListener(CoreNodeChangeListener)
 * @see CoreNodeChangeManager#unregisterCoreNodeChangeListener(CoreNodeChangeListener)
 * @see CoreNodeChangeEvent
 */
public interface CoreNodeChangeListener {
	/**
	 * <p>
	 * This callback method is called when a
	 * {@link org.daisy.urakawa.core.CoreNode} has changed.
	 * </p>
	 * <p>
	 * The {@link CoreNodeChangeEvent} parameter specifies the nature of the
	 * change.
	 * </p>
	 * <p>
	 * Typically, this method is called by the {@link CoreNodeChangeManager}
	 * when it iterates through all its registered listeners, from the
	 * {@link CoreNodeChangeManager#notifyCoreNodeChangeListeners(CoreNodeChangeEvent)}
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
	 * 
	 */
	public void coreNodeChanged(CoreNodeChangeEvent changeEvent)
			throws MethodParameterIsNullException;
}
