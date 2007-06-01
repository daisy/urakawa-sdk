package org.daisy.urakawa.properties.xml;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface, based on the default code from the
 * base class.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlPropertyImpl implements XmlProperty {
	/**
	 * @hidden
	 */
	public XmlType getXMLType() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setXMLType(XmlType newType) {
	}

	/**
	 * @hidden
	 */
	public String getNamespace() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setNamespace(String newNS)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public List<XmlAttribute> getListOfAttributes() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean setAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean setAttribute(String localName, String namespace, String value)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean removeAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean removeAttribute(String localName, String namespace)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public XmlAttribute getAttribute(String localName, String namespace)
			throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public XmlProperty copy() {
		return null;
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
	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public TreeNode getTreeNode() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public String getLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setLocalName(String newName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}
}
