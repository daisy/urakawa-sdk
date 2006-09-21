using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for a <see cref="ICoreNode"/> factory
	/// </summary>
	public interface ICoreNodeFactory
	{
    /// <summary>
    /// Creates a new <see cref="ICoreNode"/> instance of <see cref="Type"/> matching a given QName
    /// </summary>
		/// <param name="localName">The local name part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
    /// <returns>The new <see cref="ICoreNode"/></returns>
    ICoreNode createNode(string localName, string namespaceUri);
	}
}
