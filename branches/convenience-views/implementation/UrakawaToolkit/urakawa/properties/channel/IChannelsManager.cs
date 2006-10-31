using System;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Manages the list of available channels in the presentation.
	/// Nodes only refer to channels instances contained in this class, via their ChannelsProperty.
	/// </summary>
	public interface IChannelsManager : IXukAble
	{
		/// <summary>
		/// Gets the <see cref="IChannelFactory"/> creating <see cref="IChannel"/>s 
		/// for the <see cref="IChannelsManager"/>
		/// </summary>
		/// <returns>The <see cref="IChannelFactory"/></returns>
		IChannelFactory getChannelFactory();

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
		System.Collections.Generic.IList<IChannel> getListOfChannels();

		///// <summary>
		///// Removes all <see cref="IChannel"/>s 
		///// from the <see cref="IChannelsManager"/>
		///// </summary>
		//void removeAllChannels();

		/// <summary>
		/// Gets the <see cref="IChannel"/> managed by the with a given Xuk id
		/// </summary>
		/// <param name="Id">The given Xuk id</param>
		/// <returns>The <see cref="IChannel"/> with the given Xuk id or <c>null</c>
		/// if no such <see cref="IChannel"/> exists</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref name="Id"/> is <c>null</c>
		/// </exception>
		IChannel getChannelByXukId(string Id);

		/// <summary>
		/// Gets the Xuk id of a given channel
		/// </summary>
		/// <param name="ch">The given channel</param>
		/// <returns>The Xuk Id of the given channel</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when the given channel is not managed by <c>this</c>
		/// </exception>
		string getXukIdOfChannel(IChannel ch);
	}
}
