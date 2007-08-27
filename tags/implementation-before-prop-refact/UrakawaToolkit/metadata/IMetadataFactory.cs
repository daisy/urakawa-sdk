using System;

namespace urakawa.metadata
{
	/// <summary>
	/// Interface of factory for constructing <see cref="Metadata"/>
	/// </summary>
	public interface MetadataFactory
	{
    /// <summary>
    /// Creates an <see cref="Metadata"/> matching a given QName
    /// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Metadata"/> instance or <c>null</c> if the given QName is not supported</returns>
    Metadata createMetadata(string localName, string namespaceUri);

		/// <summary>
		/// Creates a <see cref="Metadata"/> instance of default type
		/// </summary>
		/// <returns>The instance</returns>
		Metadata createMetadata();
	}
}
