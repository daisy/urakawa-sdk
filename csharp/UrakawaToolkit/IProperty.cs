using System;
using urakawa.xuk;

namespace urakawa.core
{
  /// <summary>
  /// Enumeration of the possible types of <see cref="IProperty"/>s
  /// </summary>
	public enum PropertyType
	{
    /// <summary>
    /// <see cref="PropertyType"/> for <see cref="XmlProperty"/>
    /// </summary>
		XML,
    /// <summary>
    /// <see cref="PropertyType"/> for <see cref="ChannelsProperty"/>
    /// </summary>
		CHANNEL
	}

	/// <summary>
	/// Common interface for properties
	/// </summary>
	public interface IProperty : IXUKable
	{
    /// <summary>
    /// Gets the <see cref="PropertyType"/> of the <see cref="IProperty"/>
    /// </summary>
    /// <returns>The <see cref="PropertyType"/></returns>
		PropertyType getPropertyType();

    /// <summary>
    /// The actual Property object implementations must define the semantics of such copy,
    /// as it has critical implications in terms of memory management, shared object pools, etc.
    /// e.g.: a ChannelsProperty that has Media objects pointing to actual files (like MP3 audio files)
    /// => Media should be sufficiently abstract and well-managed via the some sort of MediaAssetManager
    /// to guarantee that sharing conflicts are resolved transparently.
    /// </summary>
    /// <returns>A copy/clone of the current instance</returns>
    IProperty copy();

    /// <summary>
    /// Gets the owner <see cref="ICoreNode"/> of the <see cref="IProperty"/> instance
    /// </summary>
    /// <returns>The owner</returns>
		ICoreNode getOwner();
	}


}
