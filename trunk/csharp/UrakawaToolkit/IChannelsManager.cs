using System;

namespace urakawa.core
{
	/// <summary>
  /// Manages the list of available channels in the presentation.
  /// Nodes only refer to channels instances contained in this class, via their ChannelsProperty.
	/// </summary>
	public interface IChannelsManager:IXUKable
	{
    /// <summary>
    /// Adds an existing  <see cref="IChannel"/> to the list.
    /// </summary>
    /// <param name="channel">The <see cref="IChannel"/> to add</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelAlreadyExistsException">
    /// Thrown when <paramref name="channel"/> is already in the managers list of channels
    /// </exception>
		void addChannel(IChannel channel);

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
    void removeChannel(IChannel channel);

    /// <summary>
    /// Gets a lists of the <see cref="IChannel"/>s managed by the <see cref="IChannelsManager"/>
    /// </summary>
    /// <returns>The list</returns>
    System.Collections.IList getListOfChannels();

		/// <summary>
		/// Removes all <see cref="IChannel"/>s 
		/// from the <see cref="IChannelsManager"/>
		/// </summary>
		void removeAllChannels();
	}
}
