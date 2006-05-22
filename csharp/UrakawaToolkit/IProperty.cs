using System;

namespace urakawa.core
{
  /// <summary>
  /// Enumeration of the possible types of <see cref="IProperty"/>s
  /// </summary>
	public enum PropertyType
	{
		None,
		//TODO: Decide if we need the type None, and if so: why?
    /// <summary>
    /// <see cref="PropertyType"/> for <see cref="StructureProperty"/>
    /// </summary>
		StructureProperty,
    /// <summary>
    /// <see cref="PropertyType"/> for <see cref="SMILProperty"/>
    /// </summary>
		SMILProperty,
    /// <summary>
    /// <see cref="PropertyType"/> for <see cref="ChannelsProperty"/>
    /// </summary>
    ChannelsProperty
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
	}


}
