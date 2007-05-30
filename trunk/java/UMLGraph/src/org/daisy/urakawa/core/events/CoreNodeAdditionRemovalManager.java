package org.daisy.urakawa.core.events;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Please read the top-level comment for {@link CoreNodeChangeManager}, as it
 * contains all the information required to understand and use this application
 * of the Publish/Subscribe event design pattern.
 * 
 * @see CoreNodeChangeManager
 * @see CoreNodeAddedRemovedListener#coreNodeAdded(CoreNodeAddedEvent)
 * @see CoreNodeAddedRemovedListener#coreNodeRemoved(CoreNodeRemovedEvent)
 * @see CoreNodeRemovedEvent
 * @see CoreNodeAddedEvent
 * @depend - Aggregation 0..n CoreNodeAddedRemovedListener
 */
public interface CoreNodeAdditionRemovalManager {
	/**
	 * <p>
	 * Adds an event listener.
	 * </p>
	 * <p>
	 * Does nothing if it is already registered.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             if listener is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param listener
	 *            the event listener to add
	 */
	public void registerCoreNodeAddedRemovedListener(
			CoreNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Removes an event listener.
	 * </p>
	 * <p>
	 * Silently ignores if it is not actually registered.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             if listener is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param listener
	 *            the event listener to remove. Cannot be null.
	 */
	public void unregisterCoreNodeAddedRemovedListener(
			CoreNodeAddedRemovedListener listener)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Dispatches the change notification to the registered listeners.
	 * </p>
	 * <p>
	 * Typically, this method is called by a
	 * {@link org.daisy.urakawa.core.CoreNode} when its state has changed.
	 * </p>
	 * <p>
	 * This method iterates through all the registered listeners, and forwards
	 * the notification event to each {@link CoreNodeAddedRemovedListener} via
	 * its
	 * {@link CoreNodeAddedRemovedListener#coreNodeAdded(CoreNodeAddedEvent)}
	 * callback method.
	 * </p>
	 * <p>
	 * There can be many listeners, but by design there is <b>no guarantee</b>
	 * that the order of notification will match the order of registration.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             if changeEvent is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param changeEvent
	 *            the change specification to dispatch to all registered
	 *            listeners. Cannot be null.
	 */
	public void notifyCoreNodeAddedListeners(CoreNodeAddedEvent changeEvent)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Dispatches the change notification to the registered listeners.
	 * </p>
	 * <p>
	 * Typically, this method is called by a
	 * {@link org.daisy.urakawa.core.CoreNode} when its state has changed.
	 * </p>
	 * <p>
	 * This method iterates through all the registered listeners, and forwards
	 * the notification event to each {@link CoreNodeRemovedListener} via its
	 * {@link CoreNodeRemovedListener#coreNodeAdded(CoreNodeRemovedEvent)}
	 * callback method.
	 * </p>
	 * <p>
	 * There can be many listeners, but by design there is <b>no guarantee</b>
	 * that the order of notification will match the order of registration.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             if changeEvent is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param changeEvent
	 *            the change specification to dispatch to all registered
	 *            listeners. Cannot be null.
	 */
	public void notifyCoreNodeRemovedListeners(CoreNodeRemovedEvent changeEvent)
			throws MethodParameterIsNullException;
}
