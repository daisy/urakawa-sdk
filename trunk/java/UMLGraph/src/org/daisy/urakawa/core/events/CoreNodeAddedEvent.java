package org.daisy.urakawa.core.events;

/**
 * <p>
 * This extends the basic event type to specify that the
 * {@link org.daisy.urakawa.core.CoreNode} has been added to the tree. No need
 * for extra methods, as the node is attached to the tree and therefore provides
 * all required information for further processing by listeners.
 * </p>
 * 
 * @see CoreNodeRemovedEvent
 */
public interface CoreNodeAddedEvent extends CoreNodeChangedEvent {
}
