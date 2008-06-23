package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.WithPresentation;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.property.xml.XmlProperty} and
 * {@link org.daisy.urakawa.property.xml.XmlAttribute} instances.
 * </p>
 * 
 * @see org.daisy.urakawa.property.PropertyFactory
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Create - XmlProperty
 * @depend - Create - XmlAttribute
 */
public interface XmlPropertyFactory extends XukAble, WithPresentation {
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
	 * Creates a new XML attribute.
	 * </p>
	 * 
	 * @return cannot be null.
	 */
	public XmlAttribute createXmlAttribute();

	/**
	 * <p>
	 * Creates a new XML attribute.
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
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 */
	public XmlAttribute createXmlAttribute(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
