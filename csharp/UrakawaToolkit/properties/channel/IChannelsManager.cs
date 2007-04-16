using System;
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
		/// Adds an existing  <see cref="IChannel"/> to the list.
		/// </summary>
		/// <param name="channel">The <see cref="IChannel"/> to add</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelAlreadyExistsException">
		/// Thrown when <paramref localName="channel"/> is already in the managers list of channels
		/// </exception>
		void addChannel(IChannel channel);

		/// <summary>
		/// Removes an <see cref="IChannel"/> from the list
		/// </summary>
		/// <param name="channel">The <see cref="IChannel"/> to remove</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="channel"/> is null
		/// </exception>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when <paramref localName="channel"/> is not in the managers list of channels
		/// </exception>
		void detachChannel(IChannel channel);

		/// <summary>
		/// Gets a lists of the <see cref="IChannel"/>s managed by the <see cref="IChannelsManager"/>
		/// </summary>
		/// <returns>The list</returns>
		System.Collections.Generic.IList<IChannel> getListOfChannels();

		/// <summary>
		/// Gets the <see cref="IChannel"/> managed by the with a given Xuk id
		/// </summary>
		/// <param name="Uid">The given Xuk id</param>
		/// <returns>The <see cref="IChannel"/> with the given Xuk id or <c>null</c>
		/// if no such <see cref="IChannel"/> exists</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref name="Uid"/> is <c>null</c>
		/// </exception>
		IChannel getChannel(string Uid);

		/// <summary>
		/// Gets the Xuk id of a given channel
		/// </summary>
		/// <param name="ch">The given channel</param>
		/// <returns>The Xuk Uid of the given channel</returns>
		/// <exception cref="exception.ChannelDoesNotExistException">
		/// Thrown when the given channel is not managed by <c>this</c>
		/// </exception>
		string getUidOfChannel(IChannel ch);

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
		/// Removes all <see cref="IChannel"/>s from the manager
		/// </summary>
		void clearChannels();

		/// <summary>
		/// Gets a list of the uids of <see cref="IChannel"/>s managed by the <see cref="IChannelsManager"/>
		/// </summary>
		/// <returns>The list</returns>
		System.Collections.Generic.IList<string> getListOfUids();

	}
}
