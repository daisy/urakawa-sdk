package org.daisy.urakawa.core;

import org.daisy.urakawa.core.events.TreeNodeChangeManager;
import org.daisy.urakawa.core.property.WithCorePropertyFactory;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 1 TreeNodeFactory
 * @depend - Composition 1 CorePropertyFactory
 * @depend 1 Composition 1 TreeNode
 */
public interface TreeNodePresentation extends TreeNodeChangeManager,
		WithTreeNodeFactory, WithCorePropertyFactory, XukAble {
	/**
	 * @return the root TreeNode of the presentation. Can return null (if the
	 *         tree is not allocated yet).
	 */
	public TreeNode getRootNode();

	/**
	 * @param node
	 *            the root TreeNode of the presentation. Can be null.
	 */
	public void setRootNode(TreeNode node);
}
