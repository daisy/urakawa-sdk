package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.property.IProperty;

/**
 * <p>
 * This is a specific type of node IProperty that implements an XML element
 * (with support for attributes).
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 0..n org.daisy.urakawa.property.xml.IXmlAttribute
 * @depend - Clone - org.daisy.urakawa.property.xml.IXmlProperty
 */
public interface IXmlProperty extends IProperty, IWithXmlAttributes,
		IWithQualifiedName {
	/**
	 * The definition of this interface is split into several parts, see the
	 * "extends" statement.
	 */
}