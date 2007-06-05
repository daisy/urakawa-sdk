package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.xuk.XukAble;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.properties.xml.XmlAttribute
 * @depend - Aggregation 1 org.daisy.urakawa.properties.xml.XmlProperty
 */
public interface XmlAttribute extends WithXmlProperty, WithQualifiedName,
		WithValue, XukAble {
	/**
	 * @return a distinct copy of the XmlAttribute object.
	 */
	XmlAttribute copy();
}
