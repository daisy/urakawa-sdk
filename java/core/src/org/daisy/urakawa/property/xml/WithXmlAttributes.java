package org.daisy.urakawa.property.xml;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Adding and Removing xml attributes.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
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
	 * @tagvalue Events "XmlAttributeSet"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
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
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden:
	 *             <b>localName, value</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean setAttribute(String localName, String namespace, String value)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param attr
	 *            cannot be null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @tagvalue Events "XmlAttributeSet"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XmlAttributeDoesNotExistException
	 *             The specified attribute does not exist for this is
	 *             XmlProperty
	 */
	public void removeAttribute(XmlAttribute attr)
			throws MethodParameterIsNullException,
			XmlAttributeDoesNotExistException;

	/**
	 * @param localName
	 *            cannot be null, cannot be empty.
	 * @param namespace
	 *            cannot be null, but can be empty.
	 * @return the removed attribute after the operation succeeds (otherwise an
	 *         exception is thrown)
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString,XmlAttributeDoesNotExist"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden: <b>localName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XmlAttributeDoesNotExistException
	 *             The specified attribute does not exist for this is
	 *             XmlProperty
	 */
	public XmlAttribute removeAttribute(String localName, String namespace)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			XmlAttributeDoesNotExistException;

	/**
	 * @param localName
	 *            cannot be null, cannot be empty.
	 * @param namespace
	 *            cannot be null, but can be empty.
	 * @return returns the attribute for the given namespace and local name. can
	 *         return NULL.
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden: <b>localName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public XmlAttribute getAttribute(String localName, String namespace)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
