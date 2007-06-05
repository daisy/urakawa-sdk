package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * A convenience interface to isolate the factory methods for xml properties and
 * related object types.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface XmlPropertyFactory {
	/**
	 * 
	 * @return
	 */
	public XmlProperty createXmlProperty();

	/**
	 * @param parent
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @return cannot be null.
	 */
	public XmlAttribute createXmlAttribute(XmlProperty parent)
			throws MethodParameterIsNullException;
}
