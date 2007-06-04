package org.daisy.urakawa.properties.xml;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Adding and Removing xml attributes. Please take notice of the aggregation or
 * composition relationship described here.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend 1 Composition 0..n XmlAttribute
 */
public interface WithXmlAttributes {
	/**
	 * @return The list of attributes the XML element has.
	 */
	public List<XmlAttribute> getListOfAttributes();

	/**
	 * @param attr
	 *            cannot be null
	 * @return true if the attribute was already existing, which means after
	 *         method is executed the attribute has been overridden by the new
	 *         value.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean setAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException;

	/**
	 * @param localName
	 *            cannot be null, cannot be empty.
	 * @param namespace
	 *            cannot be null, but can be empty.
	 * @param value
	 *            cannot be null, but can be empty.
	 * @return true if the attribute was already existing, which means after
	 *         method is executed the attribute has been overridden by the new
	 *         value.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean setAttribute(String localName, String namespace, String value)
			throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * @param attr
	 *            cannot be null
	 * @return true if the attribute was removed, false if it did not exist
	 * @throws MethodParameterIsNullException
	 */
	public boolean removeAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException;

	/**
	 * @param localName
	 *            cannot be null, cannot be empty.
	 * @param namespace
	 *            cannot be null, but can be empty.
	 * @return true if the attribute was removed, false if it did not exist
	 * @throws MethodParameterIsNullException
	 */
	public boolean removeAttribute(String localName, String namespace)
			throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * @param localName
	 *            cannot be null, cannot be empty.
	 * @param namespace
	 *            cannot be null, but can be empty.
	 * @return returns the attribute for the given namespace and local name. can
	 *         return NULL.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public XmlAttribute getAttribute(String localName, String namespace)
			throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
