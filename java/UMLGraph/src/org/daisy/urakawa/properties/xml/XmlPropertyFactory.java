package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
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

	/**
	 * @param parent
	 *            cannot be null
	 * @param xukLocalName
	 *            cannot be null or empty string
	 * @param xukNamespaceUri
	 *            cannot be null but can be empty string
	 * @return cannot be null.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public XmlAttribute createXmlAttribute(XmlProperty parent,
			String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsEmptyStringException,
			MethodParameterIsNullException;
}
