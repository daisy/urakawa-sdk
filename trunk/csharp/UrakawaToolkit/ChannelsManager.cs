using System;
using System.Collections;

namespace urakawa.core
{
  /// <summary>
  /// Arguments for the <see cref="ChannelsManager.Removed"/> event
  /// </summary>
  internal class ChannelsManagerRemovedEventArgs : EventArgs
  {
    /// <summary>
    /// The removed <see cref="IChannel"/>
    /// </summary>
    public IChannel RemovedChannel;

    /// <summary>
    /// Constructor - sets member 
    /// </summary>
    /// <param name="removedCh">The value for member <see cref="RemovedChannel"/></param>
    public ChannelsManagerRemovedEventArgs(IChannel removedCh)
    {
      RemovedChannel = removedCh;
    }
  }

  /// <summary>
  /// Delegate for the <see cref="ChannelsManager.Removed"/> event
  /// </summary>
  internal delegate void ChannelsManagerRemovedEventDelegate(
    ChannelsManager o, ChannelsManagerRemovedEventArgs e);

	/// <summary>
	/// Default implementation of <see cref="IChannelsManager"/>
	/// </summary>
	public class ChannelsManager : IChannelsManager
	{
    /// <summary>
    /// The list of channels managed by the manager
    /// </summary>
    private IList mChannels;

    /// <summary>
    /// Event fired when a <see cref="IChannel"/> is removed from the list of <see cref="IChannel"/> 
    /// mamaged by the <see cref="ChannelsManager"/>
    /// </summary>
    internal event ChannelsManagerRemovedEventDelegate Removed;

    private void FireRemoved(IChannel removedChannel)
    {
      if (Removed!=null) Removed(this, new ChannelsManagerRemovedEventArgs(removedChannel));
    }

    /// <summary>
    /// Default constructor
    /// </summary>
		public ChannelsManager()
		{
			mChannels = new ArrayList();
    }
    #region IChannelsManager Members

    /// <summary>
    /// Adds an existing  <see cref="IChannel"/> to the list of <see cref="IChannel"/>s 
    /// managed by the <see cref="ChannelsManager"/>
    /// </summary>
    /// <param name="channel">The <see cref="IChannel"/> to add</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelAlreadyExistsException">
    /// Thrown when <paramref name="channel"/> is already in the managers list of channels
    /// </exception>
    public void addChannel(IChannel channel)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
      if (mChannels.IndexOf(channel)!=-1)
      {
        throw new exception.ChannelAlreadyExistsException(
          "The given channel is already managed by the ChannelsManager");
      }
      mChannels.Add(channel);
    }

    /// <summary>
    /// Removes an <see cref="IChannel"/> from the list
    /// </summary>
    /// <param name="channel">The <see cref="IChannel"/> to remove</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref name="channel"/> is not in the managers list of channels
    /// </exception>
    public void removeChannel(IChannel channel)
    {
      if (channel==null)
      {
        throw new exception.MethodParameterIsNullException(
          "channel parameter is null");
      }
      int index = mChannels.IndexOf(channel);
      if (index==-1)
      {
        throw new exception.ChannelDoesNotExistException(
          "The given channel is not managed by the ChannelsManager");
      }
      mChannels.RemoveAt(index);
    }

    /// <summary>
    /// Gets a lists of the <see cref="IChannel"/>s managed by the <see cref="IChannelsManager"/>
    /// </summary>
    /// <returns>Teh list</returns>
    public System.Collections.IList getListOfChannels()
    {
      // ArrayList(ICollection c) constructs a new ArrayList with the items of the given ICollection,
      // items are not cloned
      return new ArrayList(mChannels);
    }
    #endregion
  }
}
