using System;

namespace urakawa.metadata
{
	/// <summary>
	/// Interface of factory for constructing <see cref="IMetadata"/>
	/// </summary>
	public interface IMetadataFactory
	{
    /// <summary>
    /// Creates an <see cref="IMetadata"/> matching a given QName
    /// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IMetadata"/> instance or <c>null</c> if the given QName is not supported</returns>
    IMetadata createMetadata(string localName, string namespaceUri);

		/// <summary>
		/// Creates a <see cref="IMetadata"/> instance of default type
		/// </summary>
		/// <returns>The instance</returns>
		IMetadata createMetadata();
	}
}
