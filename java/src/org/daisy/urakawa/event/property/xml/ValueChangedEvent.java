package org.daisy.urakawa.event.property.xml;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.property.xml.XmlAttribute;

/**
 *
 *
 */
public class ValueChangedEvent extends DataModelChangedEvent {
	/**
	 * @param src
	 * @param newVal
	 * @param prevVal
	 */
	public ValueChangedEvent(XmlAttribute src, String newVal, String prevVal) {
		super(src);
		mSourceXmlAttribute = src;
		mNewValue = newVal;
		mPreviousValue = prevVal;
	}

	private XmlAttribute mSourceXmlAttribute;
	private String mNewValue;
	private String mPreviousValue;

	/**
	 * @return attr
	 */
	public XmlAttribute getSourceXmlAttribute() {
		return mSourceXmlAttribute;
	}

	/**
	 * @return tr
	 */
	public String getNewValue() {
		return mNewValue;
	}

	/**
	 * @return tr
	 */
	public String getPreviousValue() {
		return mPreviousValue;
	}
}
