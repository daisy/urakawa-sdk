package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the xml property.
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
public interface WithXmlProperty {
	/**
	 * @return the factory object. Cannot be null.
	 */
	public XmlProperty getParent();

	/**
	 * @param prop
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setParent(XmlProperty prop)
			throws MethodParameterIsNullException;
}
