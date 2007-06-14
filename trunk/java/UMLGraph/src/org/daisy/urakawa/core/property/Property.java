package org.daisy.urakawa.core.property;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.core.TreeNode
 * @depend - Clone - org.daisy.urakawa.core.property.Property
 * @stereotype XukAble
 */
public interface Property extends WithTreeNode, XukAble,
		ValueEquatable<Property> {
	/**
	 * @return a clone (or "copy") of this property.
	 */
	public Property copy();
}
