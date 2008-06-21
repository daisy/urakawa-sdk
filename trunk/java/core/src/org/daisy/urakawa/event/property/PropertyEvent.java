package org.daisy.urakawa.event.property;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.property.IProperty;

/**
 * 
 *
 */
public class PropertyEvent extends DataModelChangedEvent {
	/**
	 * @param src
	 */
	public PropertyEvent(IProperty src) {
		super(src);
		mSourceProperty = src;
	}

	private IProperty mSourceProperty;

	/**
	 * @return prop
	 */
	public IProperty getSourceProperty() {
		return mSourceProperty;
	}
}
