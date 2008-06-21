package org.daisy.urakawa.property.xml;


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
public interface IWithXmlProperty {
	/**
	 * @return the factory object. Cannot be null.
	 */
	public IXmlProperty getParent();

	/**
	 * @param prop
	 *            can be null
	 * @stereotype Initialize
	 */
	public void setParent(IXmlProperty prop);
}
