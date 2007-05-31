using System;
using System.Collections.Generic;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Manages the list of available channels in the presentation.
	/// Nodes only refer to channels instances contained in this class, via their ChannelsProperty.
	/// </summary>
	public interface IChannelsManager : IXukAble, IValueEquatable<IChannelsManager>
	{
		/// <summary>
		/// Adds an existing  <see cref="Channel"/> to the list.
		/// </summary>
		/// <param name="channel">The <see cref="Channel"/> to add</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelAlreadyExistsException">
		/// Thrown when <paramref localName="channel"/> is already in the managers list of channels
		/// </exception>
		void addChannel(Channel channel);

		/// <summary>
		/// Removes an <see cref="Channel"/> from the list
		/// </summary>
		/// <param name="channel">The <see cref="Channel"/> to remove</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref localName="channel"/> is not in the managers list of channels
		/// </exception>
		void detachChannel(Channel channel);

		/// <summary>
		/// Gets a lists of the <see cref="Channel"/>s managed by the <see cref="IChannelsManager"/>
		/// </summary>
		/// <returns>The list</returns>
		List<Channel> getListOfChannels();

		/// <summary>
		/// Gets the <see cref="Channel"/> managed by the with a given Xuk id
		/// </summary>
		/// <param name="Uid">The given Xuk id</param>
		/// <returns>The <see cref="Channel"/> with the given Xuk id or <c>null</c>
		/// if no such <see cref="Channel"/> exists</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref name="Uid"/> is <c>null</c>
		/// </exception>
		Channel getChannel(string Uid);

		/// <summary>
		/// Gets the Xuk id of a given channel
		/// </summary>
		/// <param name="ch">The given channel</param>
		/// <returns>The Xuk Uid of the given channel</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when the given channel is not managed by <c>this</c>
		/// </exception>
		string getUidOfChannel(Channel ch);

		/// <summary>
		/// Sets the <see cref="IChannelPresentation"/> of the <see cref="ChannelsManager"/>
		/// </summary>
		/// <param name="newPres"></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// The associated <see cref="IChannelPresentation"/> can not be null
		/// </exception>
		void setPresentation(IChannelPresentation newPres);

		/// <summary>
		/// Gets the <see cref="IChannelPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// When no <see cref="IChannelPresentation"/> has been associated with <c>this</c>
		/// </exception>
		IChannelPresentation getPresentation();

		/// <summary>
		/// Removes all <see cref="Channel"/>s from the manager
		/// </summary>
		void clearChannels();

		/// <summary>
		/// Gets a list of the uids of <see cref="Channel"/>s managed by the <see cref="IChannelsManager"/>
		/// </summary>
		/// <returns>The list</returns>
		List<string> getListOfUids();

		/// <summary>
		/// Gets a list of <see cref="Channel"/>s managed by <c>this</c> with a given name
		/// </summary>
		/// <param name="channelName">The given name</param>
		/// <returns>The list</returns>
		List<Channel> getChannelByName(string channelName);

	}
}
