package org.daisy.urakawa.event.property.xml;

import org.daisy.urakawa.property.xml.XmlAttribute;
import org.daisy.urakawa.property.xml.XmlProperty;

/**
 * 
 *
 */
public class XmlAttributeSetEvent extends XmlPropertyEvent {
	/**
	 * @param src
	 * @param oldAttr
	 * @param newAttr
	 */
	public XmlAttributeSetEvent(XmlProperty src, XmlAttribute oldAttr,
			XmlAttribute newAttr) {
		super(src);
		mNewAttribute = newAttr;
		mPreviousAttribute = oldAttr;
	}

	private XmlAttribute mNewAttribute;
	private XmlAttribute mPreviousAttribute;

	/**
	 * @return attr
	 */
	public XmlAttribute getPreviousAttribute() {
		return mPreviousAttribute;
	}

	/**
	 * @return attr
	 */
	public XmlAttribute getNewAttribute() {
		return mNewAttribute;
	}
}
