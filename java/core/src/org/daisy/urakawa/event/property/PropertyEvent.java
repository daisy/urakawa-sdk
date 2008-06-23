package org.daisy.urakawa.event.property;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.property.Property;

/**
 * 
 *
 */
public class PropertyEvent extends DataModelChangedEvent {
	/**
	 * @param src
	 */
	public PropertyEvent(Property src) {
		super(src);
		mSourceProperty = src;
	}

	private Property mSourceProperty;

	/**
	 * @return prop
	 */
	public Property getSourceProperty() {
		return mSourceProperty;
	}
}
