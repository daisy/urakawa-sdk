package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaTypeIsIllegalException;

/**
 * <p>
 * Getting and Setting a factory.
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
public interface WithMedia {
	/**
	 * @param channel
	 *            cannot be null, the channel must exist in the list of current
	 *            channel.
	 * @return the MediaObject in a given Channel. returns null if there is no
	 *         media object for this channel.
	 * @tagvalue Exceptions "MethodParameterIsNull-ChannelDoesNotExist"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelDoesNotExistException
	 *             when the given channel is not used in this node property
	 */
	public Media getMedia(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException;

	/**
	 * Sets the MediaObject for the given Channel.
	 * 
	 * @param channel
	 *            cannot be null, the channel must exist in the list of current
	 *            channel.
	 * @param media
	 *            cannot be null, and must be of a type acceptable by the
	 *            channel.
	 * @tagvalue Exceptions
	 *           "MethodParameterIsNull-ChannelDoesNotExist-MediaTypeIsIllegal"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelDoesNotExistException
	 *             when the given channel is not used in this node property
	 * @throws MediaTypeIsIllegalException
	 */
	public void setMedia(Channel channel, Media media)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MediaTypeIsIllegalException;
}
