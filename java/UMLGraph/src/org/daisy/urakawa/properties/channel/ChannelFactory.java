package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - Channel
 * @depend - Aggregation 1 Presentation
 * @depend - Aggregation 1 ChannelsManager
 */
public interface ChannelFactory extends WithPresentation,
		WithChannelsManager {
	/**
	 * Creates a new Channel with a given name, which is not linked to the
	 * channel list yet.
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
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 */
	public Channel createChannel(String xukLocalName, String xukNamespaceUri) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
