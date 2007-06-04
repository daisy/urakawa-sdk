package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.core.property.Property;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.properties.xml.XmlType
 * @depend - Composition 0..n org.daisy.urakawa.properties.xml.XmlAttribute
 */
public interface XmlProperty extends Property, WithXmlAttributes,
		WithQualifiedName {
	/**
	 * The type of the structure element described by the XmlProperty, one of
	 * element and text in DAISY this is the type of xml node in the textual
	 * content document. Remark that for a XmlProperty with mType text, mName
	 * and mNamespace and mAttributes has no meaning if the XmlProperty
	 * describes xml.
	 */
	public XmlType getXMLType();

	/**
	 * Should *only* be used at construction/initialization time (using the
	 * Factory). (visibility is "public" because it's mandatory in Interfaces,
	 * but it would rather be "package" so that only the Factory can call this
	 * method, not the end-user).
	 * 
	 * @param newType
	 * @stereotype Initialize
	 */
	public void setXMLType(XmlType newType);
}