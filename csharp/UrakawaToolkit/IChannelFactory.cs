using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for an abstract factory creating <see cref="IChannel"/>s
	/// </summary>
	public interface IChannelFactory
	{
    /// <summary>
    /// Creates a new Channel with a given name, which is not linked to the channels list yet. 
    /// </summary>
    /// <param name="name">The name of the new <see cref="Channel"/></param>
    /// <returns>The new <see cref="Channel"/></returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="name"/> is null
    /// </exception>
    /// <exception cref="exception.MethodParameterIsEmptyStringException">
    /// Thrown when <paramref name="name"/> is an empty string
    /// </exception>
		IChannel createChannel(string name);

		/// <summary>
		/// Creates a new <see cref="IChannel"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IChannel"/> or <c>null</c> is the given QName is not supported</returns>
		IChannel createChannel(string localName, string namespaceUri);
	}
}
