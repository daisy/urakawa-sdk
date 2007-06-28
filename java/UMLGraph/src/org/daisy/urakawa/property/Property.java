package org.daisy.urakawa.property;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the baseline for a Property object. It is recommended to extend this
 * basic type, in order to provide more specific behaviors.
 * </p>
 * <p>
 * The Urakawa data model provides 2 built-in concrete property types: see
 * {@link org.daisy.urakawa.property.xml.XmlProperty} and
 * {@link org.daisy.urakawa.property.channel.ChannelsProperty}.
 * </p>
 * 
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
	 * <p>
	 * Clone method.
	 * </p>
	 * 
	 * @return cannot be null.
	 */
	public Property copy();
}
