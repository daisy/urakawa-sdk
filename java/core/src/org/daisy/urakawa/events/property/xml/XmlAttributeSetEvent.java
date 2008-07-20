package org.daisy.urakawa.events.property.xml;

import org.daisy.urakawa.property.xml.IXmlAttribute;
import org.daisy.urakawa.property.xml.IXmlProperty;

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
	public XmlAttributeSetEvent(IXmlProperty src, IXmlAttribute oldAttr,
			IXmlAttribute newAttr) {
		super(src);
		mNewAttribute = newAttr;
		mPreviousAttribute = oldAttr;
	}

	private IXmlAttribute mNewAttribute;
	private IXmlAttribute mPreviousAttribute;

	/**
	 * @return attr
	 */
	public IXmlAttribute getPreviousAttribute() {
		return mPreviousAttribute;
	}

	/**
	 * @return attr
	 */
	public IXmlAttribute getNewAttribute() {
		return mNewAttribute;
	}
}
