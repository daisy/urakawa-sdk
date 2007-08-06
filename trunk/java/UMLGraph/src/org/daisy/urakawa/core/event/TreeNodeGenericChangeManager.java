package org.daisy.urakawa.core.event;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Please read the top-level comment for {@link TreeNodeChangeManager}, as it
 * contains all the information required to understand and use this application
 * of the Publish/Subscribe event design pattern.
 * </p>
 * 
 * @see TreeNodeChangeManager
 * @see TreeNodeChangedListener#treeNodeChanged(TreeNodeChangedEvent)
 * @see TreeNodeChangedEvent
 */
public interface TreeNodeGenericChangeManager {
	/**
	 * <p>
	 * Adds an event listener.
	 * </p>
	 * <p>
	 * Does nothing if it is already registered.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param listener
	 *            the event listener to add
	 */
	public void registerTreeNodeChangedListener(TreeNodeChangedListener listener)
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
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param listener
	 *            the event listener to remove. Cannot be null.
	 */
	public void unregisterTreeNodeChangedListener(
			TreeNodeChangedListener listener)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Dispatches the change notification to the registered listeners.
	 * </p>
	 * <p>
	 * Typically, this method is called by a
	 * {@link org.daisy.urakawa.core.TreeNode} when its state has changed.
	 * </p>
	 * <p>
	 * This method iterates through all the registered listeners, and forwards
	 * the notification event to each {@link TreeNodeChangedListener} via its
	 * {@link TreeNodeChangedListener#treeNodeChanged(TreeNodeChangedEvent)}
	 * callback method.
	 * </p>
	 * <p>
	 * There can be many listeners, but by design there is <b>no guarantee</b>
	 * that the order of notification will match the order of registration.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param changeEvent
	 *            the change specification to dispatch to all registered
	 *            listeners. Cannot be null.
	 */
	public void notifyTreeNodeChangedListeners(TreeNodeChangedEvent changeEvent)
			throws MethodParameterIsNullException;
}
