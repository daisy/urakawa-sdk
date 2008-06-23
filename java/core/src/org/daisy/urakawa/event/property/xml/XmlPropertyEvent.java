package org.daisy.urakawa.event.property.xml;

import org.daisy.urakawa.event.property.PropertyEvent;
import org.daisy.urakawa.property.xml.XmlProperty;

/**
 * 
 *
 */
public class XmlPropertyEvent extends PropertyEvent {
	/**
	 * @param src
	 */
	public XmlPropertyEvent(XmlProperty src) {
		super(src);
		mSourceXmlProperty = src;
	}

	private XmlProperty mSourceXmlProperty;

	/**
	 * @return prop
	 */
	public XmlProperty getSourceXmlProperty() {
		return mSourceXmlProperty;
	}
}
