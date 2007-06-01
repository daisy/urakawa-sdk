package org.daisy.urakawa.metadata;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MetadataImpl implements Metadata {
	/**
	 * @hidden
	 */
	public String getContent() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<String> getOptionalAttributeNames() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getOptionalAttributeValue(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setContent(String content)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void setName(String name) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	/**
	 * @hidden
	 */
	public void setOptionalAttributeValue(String name, String content)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination) {
		return false;
	}

	/**
	 * @hidden
	 */
	public String getXukLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(Metadata other)
			throws MethodParameterIsNullException {
		return false;
	}
}
