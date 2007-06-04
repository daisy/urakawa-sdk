package org.daisy.urakawa.properties.xml;


/**
 * This interface represents a basic "presentation" with:
 * <ul>
 * <li> a factory for creating xml properties. </li>
 * </li>
 * </ul>
 * This is convenience interface for the design only, in order to organize the
 * data model in smaller modules.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface XmlPresentation extends WithXmlPropertyFactory {
}
