package org.daisy.urakawa.metadata;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MetadataImpl implements Metadata {
	public String getContent() {
		return null;
	}

	public List<String> getListOfOptionalAttributeNames() {
		return null;
	}

	public String getName() {
		return null;
	}

	public String getOptionalAttributeValue(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public void setContent(String content)
			throws MethodParameterIsNullException {
	}

	public void setName(String name) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void setOptionalAttributeValue(String name, String content)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(Metadata other)
			throws MethodParameterIsNullException {
		return false;
	}
}
