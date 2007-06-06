package org.daisy.urakawa.properties.xml;

/**
 * <p>
 * Getting and Setting the xml type.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithXmlType {
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
