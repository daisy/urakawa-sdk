package org.daisy.urakawa.core;

import org.daisy.urakawa.core.events.CoreNodeChangeManager;
import org.daisy.urakawa.core.property.WithCorePropertyFactory;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 1 CoreNodeFactory
 * @depend - Composition 1 CorePropertyFactory
 * @depend 1 Composition 1 CoreNode
 */
public interface CorePresentation extends CoreNodeChangeManager,
		WithCoreNodeFactory, WithCorePropertyFactory, XukAble {
	/**
	 * @return the root CoreNode of the presentation. Can return null (if the
	 *         tree is not allocated yet).
	 */
	public CoreNode getRootNode();

	/**
	 * @param node
	 *            the root CoreNode of the presentation. Can be null.
	 */
	public void setRootNode(CoreNode node);
}
