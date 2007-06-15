package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.properties.channel.Channel} instances.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - org.daisy.urakawa.properties.channel.Channel
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface ChannelFactory extends WithPresentation {
	/**
	 * <p>
	 * Creates a new named, unmanaged channel.
	 * </p>
	 * <p>
	 * This factory method does not take any argument to specify the object
	 * type, and therefore creates an object of the default type.
	 * </p>
	 * <p>
	 * This factory method takes arguments to specify the exact type of object
	 * to create, given by the unique QName (XML Qualified Name) used in the XUK
	 * serialization format. This method can be used to generate instances of
	 * subclasses of the base object type.
	 * </p>
	 * 
	 * @param name
	 *            cannot be null, cannot be empty String
	 * @return cannot return null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public Channel createChannel(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a new named, unmanaged channel.
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
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 */
	public Channel createChannel(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
