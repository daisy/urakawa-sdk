using System;
using urakawa.core.property;
using urakawa.media;

namespace urakawa.properties.channel
{
	/// <summary>
  /// This property maintains a mapping from Channel object to Media object.
  /// Channels referenced here are pointers to existing channels in the presentation.
	/// </summary>
	public interface IChannelsProperty : IProperty
	{
    /// <summary>
    /// Retrieves the <see cref="IMedia"/> of a given <see cref="IChannel"/>
    /// </summary>
    /// <param localName="channel">The given <see cref="IChannel"/></param>
    /// <returns>The <see cref="IMedia"/> associated with the given channel, 
    /// <c>null</c> if no <see cref="IMedia"/> is associated</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
    /// </exception>
    IMedia getMedia(IChannel channel);

    /// <summary>
    /// Associates a given <see cref="IMedia"/> with a given <see cref="IChannel"/>
    /// </summary>
    /// <param localName="channel">The given <see cref="IChannel"/></param>
    /// <param localName="media">The given <see cref="IMedia"/></param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref localName="channel"/> or <paramref localName="media"/>
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref localName="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
    /// </exception>
    /// <exception cref="exception.MediaTypeIsIllegalException">
    /// Thrown when <paramref localName="channel"/> does not support the <see cref="MediaType"/> 
    /// of <paramref localName="media"/>
    /// </exception>
    void setMedia(IChannel channel, IMedia media);

    /// <summary>
    /// Gets the list of <see cref="IChannel"/>s used by this instance of <see cref="IChannelsProperty"/>
    /// </summary>
    /// <returns>The list of used <see cref="IChannel"/>s</returns>
    System.Collections.Generic.IList<IChannel> getListOfUsedChannels();
	}
}
