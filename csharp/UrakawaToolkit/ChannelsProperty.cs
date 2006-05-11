using System;
using System.Collections;

using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of 
	/// </summary>
	public class ChannelsProperty : IChannelsProperty
  {
		private IDictionary mMapChannelToMediaObject;

    private ChannelsManager mChannelsManager;

    internal ChannelsProperty(ChannelsManager manager)
    {
      mChannelsManager = manager;
      mChannelsManager.Removed += new ChannelsManagerRemovedEventDelegate(mChannelsManager_Removed);
      mMapChannelToMediaObject = new System.Collections.Specialized.ListDictionary();
    }

    #region IProperty Members

    /// <summary>
    /// Gets the <see cref="PropertyType"/> of the <see cref="ChannelsProperty"/>
    /// </summary>
    /// <returns>Always <see cref="PropertyType.ChannelsProperty"/></returns>
    public PropertyType getPropertyType()
    {
      return PropertyType.ChannelsProperty;
    }

    #endregion

    #region IChannelsProperty Members

    /// <summary>
    /// Retrieves the <see cref="IMedia"/> of a given <see cref="IChannel"/>
    /// </summary>
    /// <param name="channel">The given <see cref="IChannel"/></param>
    /// <returns>The <see cref="IMedia"/> associated with the given channel, 
    /// <c>null</c> if no <see cref="IMedia"/> is associated</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref name="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
    /// </exception>
    public IMedia getMedia(IChannel channel)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
      if (!mChannelsManager.getListOfChannels().Contains(channel))
      {
        throw new exception.ChannelDoesNotExistException(
          "The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
      }
      return (IMedia)mMapChannelToMediaObject[channel];
    }

    /// <summary>
    /// Associates a given <see cref="IMedia"/> with a given <see cref="IChannel"/>
    /// </summary>
    /// <param name="channel">The given <see cref="IChannel"/></param>
    /// <param name="media">The given <see cref="IMedia"/></param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="channel"/> or <paramref name="media"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref name="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
    /// </exception>
    /// <exception cref="exception.MediaTypeIsIllegalException">
    /// Thrown when <paramref name="channel"/> does not support the <see cref="MediaType"/> 
    /// of <paramref name="media"/>
    /// </exception>
    public void setMedia(IChannel channel, IMedia media)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
      if (media==null)
      {
        throw new exception.MethodParameterIsNullException(
          "media parameter is null");
      }
      if (!mChannelsManager.getListOfChannels().Contains(channel))
      {
        throw new exception.ChannelDoesNotExistException(
          "The given channel is not managed by the ChannelManager associated with the ChannelsProperty");
      }
      if (!channel.isMediaTypeSupported(media.getType()))
      {
        throw new exception.MediaTypeIsIllegalException(
          "The given media type is not supported by the given channel");
      }
      mMapChannelToMediaObject[channel] = media;
    }

    public ArrayList getListOfUsedChannels()
    {
      // TODO:  Add ChannelsProperty.getListOfUsedChannels implementation
      return null;
    }
    #endregion

    private void mChannelsManager_Removed(ChannelsManager o, ChannelsManagerRemovedEventArgs e)
    {
      mMapChannelToMediaObject.Remove(e.RemovedChannel);
    }
  }
}
