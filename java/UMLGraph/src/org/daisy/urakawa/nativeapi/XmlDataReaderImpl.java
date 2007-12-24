package org.daisy.urakawa.nativeapi;

import java.net.URI;

/**
 * Place-holder for a real implementation in Java. It should be replaced with
 * StAX, really.
 */
public class XmlDataReaderImpl implements XmlDataReader {
	/**
	 * @param uri
	 *            bla
	 */
	public XmlDataReaderImpl(@SuppressWarnings("unused")
	URI uri) {
		;
	}

	public void close() {
	}

	public String getLocalName() {
		return null;
	}

	public String getNamespaceURI() {
		return null;
	}

	public int getNodeType() {
		return 0;
	}

	public boolean isEOF() {
		return false;
	}

	public boolean isEmptyElement() {
		return false;
	}

	public boolean read() {
		return false;
	}

	public XmlDataReader readSubtree() {
		return null;
	}

	public boolean readToFollowing(@SuppressWarnings("unused")
	String localName, @SuppressWarnings("unused")
	String namespace) {
		return false;
	}

	public String getAttribute(@SuppressWarnings("unused")
	String name) {
		return null;
	}

	public String getBaseURI() {
		return null;
	}

	public String getName() {
		return null;
	}

	public String getValue() {
		return null;
	}

	public void moveToElement() {
	}

	public boolean moveToFirstAttribute() {
		return false;
	}

	public boolean moveToNextAttribute() {
		return false;
	}

	public String readElementContentAsString() {
		return null;
	}
}
