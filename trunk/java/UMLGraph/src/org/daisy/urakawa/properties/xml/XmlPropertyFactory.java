package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.core.property.CorePropertyFactory;

/**
 *
 */
public interface XmlPropertyFactory extends CorePropertyFactory {
    public XmlProperty createXmlProperty();
    public XmlAttribute createXmlAttribute(XmlProperty parent);
    public XmlAttribute createXmlAttribute(XmlProperty parent, String xukLocalName, String xukNamespaceUri);
}
