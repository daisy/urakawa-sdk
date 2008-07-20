package org.daisy.urakawa.nativeapi;

/**
 * Place-holder for a real implementation in Java. It should be replaced with
 * StAX, really.
 */
public class XmlDataReader implements IXmlDataReader {
	/**
	 * @param iStream
	 */
	public XmlDataReader(@SuppressWarnings("unused") IStream iStream) {
		// Needs implementing !
	}

	public void close() {
		/**
		 * To implement.
		 */
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

	public IXmlDataReader readSubtree() {
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
		/**
		 * To implement.
		 */
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
