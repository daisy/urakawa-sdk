package org.daisy.urakawa.property;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.channel.IChannelsPropertyFactory;
import org.daisy.urakawa.property.xml.IXmlPropertyFactory;

/**
 * <p>
 * This is the factory that creates all types of built-in
 * {@link org.daisy.urakawa.property.IProperty} instances.
 * </p>
 * <p>
 * Note: this interface assembles a set of other interfaces, but does not
 * introduce new methods itself.
 * </p>
 * 
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 * @depend - Create - org.daisy.urakawa.property.IProperty
 * @depend - Create - org.daisy.urakawa.property.xml.IXmlAttribute
 * @depend - Create - org.daisy.urakawa.property.xml.IXmlProperty
 * @depend - Create - org.daisy.urakawa.property.channel.IChannelsProperty
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface IPropertyFactory extends IXmlPropertyFactory,
		IChannelsPropertyFactory {
	/**
	 * @param xukLocalName
	 * @param xukNamespaceURI
	 * @return a potentially null Property subclass, if the factory could not
	 *         create the object.
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public IProperty createProperty(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
