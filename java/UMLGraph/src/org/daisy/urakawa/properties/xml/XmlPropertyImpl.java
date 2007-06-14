package org.daisy.urakawa.properties.xml;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlPropertyImpl implements XmlProperty {
	public XmlProperty copy() {
		return null;
	}

	public TreeNode getTreeNode() {
		return null;
	}

	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
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

	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		return false;
	}

	public XmlType getXMLType() {
		return null;
	}

	public void setXMLType(XmlType newType) {
	}

	public XmlAttribute getAttribute(String localName, String namespace)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<XmlAttribute> getListOfAttributes() {
		return null;
	}

	public boolean removeAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean removeAttribute(String localName, String namespace)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return false;
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
}
