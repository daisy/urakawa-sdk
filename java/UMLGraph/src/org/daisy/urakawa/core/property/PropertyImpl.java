package org.daisy.urakawa.core.property;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

public class PropertyImpl implements Property {
	/**
	 * @hidden
	 */
	public Property copy() {
		return null;
	};

	/**
	 * @hidden
	 */
	public CoreNode getOwner() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setOwner(CoreNode newOwner) {
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
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
}
