package org.daisy.urakawa.events.property.xml;

import org.daisy.urakawa.property.xml.IXmlProperty;

/**

 *
 */
public class QNameChangedEvent extends XmlPropertyEvent {
	/**
	 * @param src
	 * @param newLN
	 * @param newNS
	 * @param prevLN
	 * @param prevNS
	 */
	public QNameChangedEvent(IXmlProperty src, String newLN, String newNS,
			String prevLN, String prevNS) {
		super(src);
		mNewLocalName = newLN;
		mNewNamespaceUri = newNS;
		mPreviousLocalName = prevLN;
		mPreviousNamespaceUri = prevNS;
	}

	private String mNewLocalName;
	private String mNewNamespaceUri;
	private String mPreviousLocalName;
	private String mPreviousNamespaceUri;

	/**
	 * @return str
	 */
	public String getNewLocalName() {
		return mNewLocalName;
	}

	/**
	 * @return str
	 */
	public String getPreviousNamespaceUri() {
		return mPreviousNamespaceUri;
	}

	/**
	 * @return str
	 */
	public String getPreviousLocalName() {
		return mPreviousLocalName;
	}

	/**
	 * @return str
	 */
	public String getNewNamespaceUri() {
		return mNewNamespaceUri;
	}
}
