package org.daisy.urakawa.core;

import org.daisy.urakawa.core.events.TreeNodeChangeManager;
import org.daisy.urakawa.core.property.WithGenericPropertyFactory;

/**
 * This interface represents a basic "presentation"
 * with:
 * <ul>
 * <li> its root document tree node. </li>
 * <li> a factory for creating tree nodes. </li>
 * <li> a factory for creating QNamed properties. </li>
 * <li> an event manager for handling structural changes in the document tree.
 * </li>
 * </ul>
 * This is convenience interface for the design only, in order to organize the data
 * model in smaller modules. 
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface TreeNodePresentation extends TreeNodeChangeManager,
		WithTreeNode, WithTreeNodeFactory, WithGenericPropertyFactory {
}
