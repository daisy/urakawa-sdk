package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is an XML attribute owned by an XmlProperty.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.properties.xml.XmlAttribute
 * @depend - Aggregation 1 org.daisy.urakawa.properties.xml.XmlProperty
 * @stereotype XukAble
 */
public interface XmlAttribute extends WithXmlProperty, WithQualifiedName,
		WithValue, XukAble {
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	XmlAttribute copy();
}
