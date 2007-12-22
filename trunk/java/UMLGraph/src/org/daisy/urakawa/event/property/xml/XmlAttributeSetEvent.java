package org.daisy.urakawa.event.property.xml;

import org.daisy.urakawa.property.xml.XmlProperty;

/**
 * 
 *
 */
public class XmlAttributeSetEvent extends XmlPropertyEvent {
	/**
	 * @param src
	 * @param attrLN
	 * @param attrNS
	 * @param newVal
	 * @param prevVal
	 */
	public XmlAttributeSetEvent(XmlProperty src, String attrLN, String attrNS,
			String newVal, String prevVal) {
		super(src);
		mAttributeLocalName = attrLN;
		mAttributeNamespaceUri = attrNS;
		mNewValue = newVal;
		mPreviousValue = prevVal;
	}

	private String mAttributeLocalName;
	private String mAttributeNamespaceUri;
	private String mNewValue;
	private String mPreviousValue;

	/**
	 * @return str
	 */
	public String getPreviousValue() {
		return mPreviousValue;
	}

	/**
	 * @return str
	 */
	public String getAttributeLocalName() {
		return mAttributeLocalName;
	}

	/**
	 * @return str
	 */
	public String getAttributeNamespaceUri() {
		return mAttributeNamespaceUri;
	}

	/**
	 * @return str
	 */
	public String getNewValue() {
		return mNewValue;
	}
}
