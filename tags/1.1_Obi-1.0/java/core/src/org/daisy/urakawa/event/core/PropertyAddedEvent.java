package org.daisy.urakawa.event.core;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.property.Property;

/**
 * 
 *
 */
public class PropertyAddedEvent extends TreeNodeEvent {
	/**
	 * @param src
	 * @param addee
	 */
	public PropertyAddedEvent(TreeNode src, Property addee) {
		super(src);
		mAddedProperty = addee;
	}

	private Property mAddedProperty;

	/**
	 * @return prop
	 */
	public Property getAddedProperty() {
		return mAddedProperty;
	}
}
