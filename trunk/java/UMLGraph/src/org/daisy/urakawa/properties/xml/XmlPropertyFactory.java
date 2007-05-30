package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.core.property.CorePropertyFactory;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Create 1 XmlAttribute
 * @depend - Create 1 XmlProperty
 */
public interface XmlPropertyFactory extends CorePropertyFactory {
	public XmlProperty createXmlProperty();

	/**
	 * 
	 * @param parent
	 *            cannot be null
	 * 
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @return cannot be null.
	 */
	public XmlAttribute createXmlAttribute(XmlProperty parent)
			throws MethodParameterIsNullException;

	/**
	 * 
	 * @param parent
	 *            cannot be null
	 * @param xukLocalName
	 *            cannot be null or empty string
	 * @param xukNamespaceUri
	 *            cannot be null but can be empty string
	 * @return cannot be null.
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public XmlAttribute createXmlAttribute(XmlProperty parent,
			String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsEmptyStringException,
			MethodParameterIsNullException;
}
