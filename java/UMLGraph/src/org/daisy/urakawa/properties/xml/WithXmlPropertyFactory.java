package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 XmlPropertyFactory
 */
public interface WithXmlPropertyFactory {
	/**
	 * @return the factory object
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
