package org.daisy.urakawa.core.property;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Aggregation 1 TreeNode
 * @depend - - - PropertyType
 */
public interface Property extends WithTreeNode, XukAble,
		ValueEquatable<Property> {
	/**
	 * @return a clone (or "copy") of this property.
	 */
	public Property copy();
}
