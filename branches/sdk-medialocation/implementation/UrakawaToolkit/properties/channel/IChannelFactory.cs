using System;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Interface for an abstract factory creating <see cref="IChannel"/>s
	/// </summary>
	public interface IChannelFactory
	{
		/// <summary>
		/// Gets the <see cref="IChannelPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IChannelPresentation"/> has been associated with <c>this</c>
		/// </exception>
		IChannelPresentation getPresentation();

		/// <summary>
		/// Associates a <see cref="IChannelPresentation"/> with <c>this</c>
		/// </summary>
		/// <param name="pres">The <see cref="IChannelPresentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when trying to associate a <c>null</c> <see cref="IChannelPresentation"/> with <c>this</c>
		/// </exception>
		void setPresentation(IChannelPresentation pres);

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> associated with <see cref="IChannel"/>s created
		/// by the <see cref="IChannelFactory"/>.
		/// Convenience for <c>getPresentation().getChannelsManager()</c>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		IChannelsManager getChannelsManager();

		/// <summary>
		/// Creates a new <see cref="IChannel"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IChannel"/> or <c>null</c> is the given QName is not supported</returns>
		IChannel createChannel(string localName, string namespaceUri);
	}
}
