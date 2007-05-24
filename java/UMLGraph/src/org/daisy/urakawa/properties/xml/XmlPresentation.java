package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.core.CorePresentation;

/**
 * @depend - Aggregation 1 XmlPropertyFactory
 */
public interface XmlPresentation extends CorePresentation {
    /**
     * @return Cannot return null. This is a convenience method for CorePresentation.getPropertyFactory() to avoid explicit cast when writing applications. The returned object instance is the same for both method calls.
     * @see CorePresentation#getPropertyFactory()
     */
    public XmlPropertyFactory getXmlPropertyFactory();
}
