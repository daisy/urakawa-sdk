package org.daisy.urakawa.property;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.TreeNode;
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
public class PropertyImpl implements Property {
	public Property copy() {
		return null;
	}

	public TreeNode getTreeNode() {
		return null;
	}

	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
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

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public Property exportProperty(Presentation destPres)
			throws FactoryCannotCreateTypeException {
		Property prop;
		try {
			prop = destPres.getPropertyFactory().createProperty(
					this.getXukLocalName(), this.getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			e.printStackTrace();
			return null;
		} catch (MethodParameterIsEmptyStringException e) {
			e.printStackTrace();
			return null;
		}
		if (prop == null) {
			throw new FactoryCannotCreateTypeException();
		}
		return prop;
	}
}
