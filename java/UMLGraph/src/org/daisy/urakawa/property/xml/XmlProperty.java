package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.property.Property;

/**
 * <p>
 * This is a specific type of node Property that implements an XML element (with
 * support for attributes).
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 0..n org.daisy.urakawa.properties.xml.XmlAttribute
 * @depend - Clone - org.daisy.urakawa.properties.xml.XmlProperty
 */
public interface XmlProperty extends Property, WithXmlAttributes,
		WithQualifiedName {
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public XmlProperty copy();
}