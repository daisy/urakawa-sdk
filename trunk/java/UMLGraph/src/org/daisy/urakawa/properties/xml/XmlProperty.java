package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.core.property.Property;

/**
 * <p>
 * This is a specific type of node Property that implements an XML element
 * (potentially with attributes) or XML text node.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.properties.xml.XmlType
 * @depend - Composition 0..n org.daisy.urakawa.properties.xml.XmlAttribute
 * @depend - Clone - org.daisy.urakawa.properties.xml.XmlProperty
 */
public interface XmlProperty extends Property, WithXmlType, WithXmlAttributes,
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