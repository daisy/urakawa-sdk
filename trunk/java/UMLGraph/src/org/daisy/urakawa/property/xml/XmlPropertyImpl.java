package org.daisy.urakawa.property.xml;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyImpl;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlPropertyImpl extends PropertyImpl implements XmlProperty {
	public XmlProperty copy() {
		return null;
	}

	public TreeNode getTreeNode() {
		return null;
	}

	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		return false;
	}

	public XmlAttribute getAttribute(String localName, String namespace)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<XmlAttribute> getListOfAttributes() {
		return null;
	}

	public void removeAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException {
	}

	public XmlAttribute removeAttribute(String localName, String namespace)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public boolean setAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean setAttribute(String localName, String namespace, String value)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return false;
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

	public Property export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		return null;
	}

	public XmlProperty copyXmlProperty() {
		return null;
	}
}
