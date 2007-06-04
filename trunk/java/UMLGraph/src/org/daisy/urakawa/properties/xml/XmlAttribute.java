package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface XmlAttribute extends WithQualifiedName, XukAble {
	/**
	 * @return the container element for this attribute.
	 */
	public XmlProperty getParent();

	/**
	 * The attribute value.
	 * 
	 * @return Cannot return NULL and cannot return an empty string.
	 */
	public String getValue();

	/**
	 * The attribute value.
	 * 
	 * @param newValue
	 *            cannot be null, cannot be empty String
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             if new Value is empty string
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void setValue(String newValue)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @return a distinct copy of the XmlAttribute object.
	 */
	XmlAttribute copy();
}
