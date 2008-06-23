package org.daisy.urakawa.nativeapi;

/**
 * Place-holder for a real implementation in Java. It should be replaced with
 * StAX, really.
 */
public class XmlDataReaderImpl implements XmlDataReader {
	/**
	 * @param stream
	 */
	public XmlDataReaderImpl(@SuppressWarnings("unused") Stream stream) {
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

	public boolean readToFollowing(
			@SuppressWarnings("unused") String localName,
			@SuppressWarnings("unused") String namespace) {
		return false;
	}

	public String getAttribute(@SuppressWarnings("unused") String name) {
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
