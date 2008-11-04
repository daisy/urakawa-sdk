package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.DoesNotAcceptMediaException;
import org.daisy.urakawa.media.IMedia;

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
public interface IWithMedia
{
    /**
     * @param iChannel
     *        cannot be null, the channel must exist in the list of current
     *        channel.
     * @return the MediaObject in a given IChannel. returns null if there is no
     *         media object for this channel.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws ChannelDoesNotExistException
     *         when the given channel is not used in this node property
     */
    public IMedia getMedia(IChannel iChannel)
            throws MethodParameterIsNullException, ChannelDoesNotExistException;

    /**
     * Sets the MediaObject for the given IChannel.
     * 
     * @param iChannel
     *        cannot be null, the channel must exist in the list of current
     *        channel.
     * @param iMedia
     *        can be null, or must be of a type acceptable by the channel.
     *        "MethodParameterIsNull-ChannelDoesNotExist-ChannelDoesNotAcceptMedia"
     * @tagvalue Events "ChannelMediaMap"
     * @throws MethodParameterIsNullException
     *         NULL method parameter channel is forbidden
     * @throws ChannelDoesNotExistException
     *         when the given channel is not used in this node property
     * @throws DoesNotAcceptMediaException
     *         if {@link IChannel#canAccept(IMedia)} returns false.
     */
    public void setMedia(IChannel iChannel, IMedia iMedia)
            throws MethodParameterIsNullException,
            ChannelDoesNotExistException, DoesNotAcceptMediaException;
}
