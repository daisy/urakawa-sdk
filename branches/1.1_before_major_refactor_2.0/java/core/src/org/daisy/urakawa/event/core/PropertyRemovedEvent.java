package org.daisy.urakawa.event.core;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.property.Property;

/**
 * 
 *
 */
public class PropertyRemovedEvent extends TreeNodeEvent {
	private Property mRemovedProperty;

	/**
	 * @param notfr
	 * @param removee
	 */
	public PropertyRemovedEvent(TreeNode notfr, Property removee) {
		super(notfr);
		mRemovedProperty = removee;
	}

	/**
	 * @return prop
	 */
	public Property getRemovedProperty() {
		return mRemovedProperty;
	}
}
