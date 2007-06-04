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
 * {@link org.daisy.urakawa.core.TreeNode} changes, via the
 * {@link TreeNodeAddedRemovedListener#treeNodeAdded(TreeNodeAddedEvent)} or
 * {@link TreeNodeAddedRemovedListener#treeNodeRemoved(TreeNodeRemovedEvent)}
 * callback methods which pass the {@link TreeNodeAddedEvent} or
 * {@link TreeNodeRemovedEvent} parameter that specifies the information for the
 * change.
 * </p>
 * <p>
 * Of course, the notification happens only if this listener is known by
 * (registered at) the {@link TreeNodeAdditionRemovalManager}.
 * </p>
 * <p>
 * Application developers typically implement their own listeners to receive
 * change event notifications from the data model.
 * </p>
 * 
 * @see TreeNodeRemovedEvent
 * @see TreeNodeAddedEvent
 * @see TreeNodeAdditionRemovalManager#registerTreeNodeAddedRemovedListener(TreeNodeAddedRemovedListener)
 * @see TreeNodeAdditionRemovalManager#unregisterTreeNodeAddedRemovedListener(TreeNodeAddedRemovedListener)
 */
public interface TreeNodeAddedRemovedListener {
	/**
	 * <p>
	 * This callback method is called when a
	 * {@link org.daisy.urakawa.core.TreeNode} has been added.
	 * </p>
	 * <p>
	 * The {@link TreeNodeAddedEvent} parameter specifies the information for
	 * the change.
	 * </p>
	 * <p>
	 * Typically, this method is called by the {@link TreeNodeAdditionRemovalManager}
	 * when it iterates through all its registered listeners, from the
	 * {@link TreeNodeAdditionRemovalManager#notifyTreeNodeAddedListeners(TreeNodeAddedEvent)}
	 * method which is called by a {@link org.daisy.urakawa.core.TreeNode} when
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
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void treeNodeAdded(TreeNodeAddedEvent changeEvent)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * This callback method is called when a
	 * {@link org.daisy.urakawa.core.TreeNode} has been removed.
	 * </p>
	 * <p>
	 * The {@link TreeNodeRemovedEvent} parameter specifies the information for
	 * the change.
	 * </p>
	 * <p>
	 * Typically, this method is called by the {@link TreeNodeAdditionRemovalManager}
	 * when it iterates through all its registered listeners, from the
	 * {@link TreeNodeAdditionRemovalManager#notifyTreeNodeRemovedListeners(TreeNodeRemovedEvent)}
	 * method which is called by a {@link org.daisy.urakawa.core.TreeNode} when
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
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void treeNodeRemoved(TreeNodeRemovedEvent changeEvent)
			throws MethodParameterIsNullException;
}
