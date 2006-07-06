using System;

namespace urakawa.project
{
	/// <summary>
	/// Interface of factory for constructing <see cref="IMetadata"/>
	/// </summary>
	public interface IMetadataFactory
	{
    /// <summary>
    /// Creates an <see cref="IMetadata"/> instance of a given type
    /// </summary>
    /// <param name="typeString">The string representation of the type</param>
    /// <returns>The created <see cref="IMetadata"/> instance</returns>
    IMetadata createMetadata(string typeString);
	}
}
