package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlAttributeImpl implements XmlAttribute {
	public XmlAttribute copy() {
		return null;
	}

	public XmlProperty getXmlProperty() {
		return null;
	}

	public void setXmlProperty(XmlProperty prop)
			throws MethodParameterIsNullException {
	}

	public String getLocalName() {
		return null;
	}

	public String getNamespace() {
		return null;
	}

	public void setLocalName(String newName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void setNamespace(String newNS)
			throws MethodParameterIsNullException {
	}

	public String getValue() {
		return null;
	}

	public void setValue(String newValue)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		return false;
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}
}
