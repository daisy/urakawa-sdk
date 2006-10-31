using System;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Interface for an abstract factory creating <see cref="IChannel"/>s
	/// </summary>
	public interface IChannelFactory
	{
		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> assigned the <see cref="IChannel"/>s created
		/// by the <see cref="IChannelFactory"/>
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
