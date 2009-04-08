package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.property.channel.Channel} instances.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - org.daisy.urakawa.property.channel.Channel
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface ChannelFactory extends XukAble, WithPresentation {
	/**
	 * Convenience method that delegates to the Presentation
	 * 
	 * @see Presentation#getChannelsManager()
	 * @return the ChannelsManager
	 * @throws IsNotInitializedException 
	 */
	public ChannelsManager getChannelsManager()
			throws IsNotInitializedException;

	/**
	 * <p>
	 * Creates a default channel.
	 * </p>
	 * <p>
	 * The returned object is managed by its associated manager.
	 * </p>
	 * 
	 * @return cannot return null
	 */
	public Channel createChannel();

	/**
	 * <p>
	 * Creates a channel.
	 * </p>
	 * <p>
	 * The returned object is managed by its associated manager.
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
	public Channel createChannel(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
