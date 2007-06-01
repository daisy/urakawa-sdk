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
 * {@link TreeNodeChangedListener#treeNodeChanged(TreeNodeChangedEvent)}
 * callback method which passes the {@link TreeNodeChangedEvent} parameter that
 * specifies the nature of the change.
 * </p>
 * <p>
 * Of course, the notification happens only if this listener is known by
 * (registered at) the {@link TreeNodeGenericChangeManager}.
 * </p>
 * <p>
 * Application developers typically implement their own listeners to receive
 * change event notifications from the data model.
 * </p>
 * 
 * @see TreeNodeGenericChangeManager#registerTreeNodeChangedListener(TreeNodeChangedListener)
 * @see TreeNodeGenericChangeManager#unregisterTreeNodeChangedListener(TreeNodeChangedListener)
 * @see TreeNodeChangedEvent
 */
public interface TreeNodeChangedListener {
	/**
	 * <p>
	 * This callback method is called when a
	 * {@link org.daisy.urakawa.core.TreeNode} has changed.
	 * </p>
	 * <p>
	 * The {@link TreeNodeChangedEvent} parameter specifies the nature of the
	 * change.
	 * </p>
	 * <p>
	 * Typically, this method is called by the
	 * {@link TreeNodeGenericChangeManager} when it iterates through all its
	 * registered listeners, from the
	 * {@link TreeNodeGenericChangeManager#notifyTreeNodeChangedListeners(TreeNodeChangedEvent)}
	 * method which is called by a {@link org.daisy.urakawa.core.TreeNode} when
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
	public void treeNodeChanged(TreeNodeChangedEvent changeEvent)
			throws MethodParameterIsNullException;
}
