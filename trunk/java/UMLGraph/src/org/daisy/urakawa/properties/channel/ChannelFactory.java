package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @depend - Create 1 Channel
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
	 * @tagvalue Exceptions "MethodParameterIsNull,
	 *           MethodParameterIsEmptyString"
	 */
	public Channel createChannel(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	public Channel createChannel(String xukLocalName, String xukNamespaceUri);
}
