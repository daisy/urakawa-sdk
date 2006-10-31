using System;
//using urakawa.properties.channel;
//using urakawa.properties.xml;

namespace urakawa.core.property
{
	/// <summary>
	/// Interface for factories creating <see cref="IProperty"/>s
	/// </summary>
	public interface ICorePropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="IProperty"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IProperty"/> or <c>null</c> if the given QName is not supported</returns>
		IProperty createProperty(string localName, string namespaceUri);
	}
}
