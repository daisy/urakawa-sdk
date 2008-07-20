package org.daisy.urakawa.events.property.xml;

import org.daisy.urakawa.events.property.PropertyEvent;
import org.daisy.urakawa.property.xml.IXmlProperty;

/**
 * 
 *
 */
public class XmlPropertyEvent extends PropertyEvent {
	/**
	 * @param src
	 */
	public XmlPropertyEvent(IXmlProperty src) {
		super(src);
		mSourceXmlProperty = src;
	}

	private IXmlProperty mSourceXmlProperty;

	/**
	 * @return prop
	 */
	public IXmlProperty getSourceXmlProperty() {
		return mSourceXmlProperty;
	}
}
