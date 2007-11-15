package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.XukAbleObjectFactory;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.property.xml.XmlProperty} and
 * {@link org.daisy.urakawa.property.xml.XmlAttribute} instances.
 * </p>
 * 
 * @see org.daisy.urakawa.PropertyFactory
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface XmlPropertyFactory extends XukAbleObjectFactory, WithPresentation {
	/**
	 * <p>
	 * Creates a new property, not yet associated to a node.
	 * </p>
	 * <p>
	 * This factory method does not take any argument and creates an object of
	 * the default type.
	 * </p>
	 * 
	 * @return cannot be null.
	 */
	public XmlProperty createXmlProperty();

	/**
	 * <p>
	 * Creates a new XML attribute, and associates the given parent.
	 * </p>
	 * 
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
	 * <p>
	 * Creates a new XML attribute, and associates the given parent.
	 * </p>
	 * <p>
	 * This factory method takes arguments to specify the exact type of object
	 * to create, given by the unique QName (XML Qualified Name) used in the XUK
	 * serialization format. This method can be used to generate instances of
	 * subclasses of the base object type.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @param parent
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 */
	public XmlAttribute createXmlAttribute(XmlProperty parent,
			String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
