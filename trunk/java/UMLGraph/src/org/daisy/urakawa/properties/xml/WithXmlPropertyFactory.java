package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting a factory. Please take notice of the aggregation
 * or composition relationship for the object attribute described here, and also
 * be aware that this relationship may be explicitly overriden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @depend - Aggregation 1 XmlPropertyFactory
 */
public interface WithXmlPropertyFactory {
	/**
	 * @return the factory object. Cannot be null.
	 */
	public XmlPropertyFactory getXmlPropertyFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setXmlPropertyFactory(XmlPropertyFactory factory)
			throws MethodParameterIsNullException;
}
