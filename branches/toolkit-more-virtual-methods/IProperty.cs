using System;


namespace urakawa.core
{
//  /// <summary>
//  /// Enumeration of the possible types of <see cref="IProperty"/>s
//  /// </summary>
//	public enum PropertyType
//	{
//    /// <summary>
//    /// <see cref="PropertyType"/> for <see cref="XmlProperty"/>
//    /// </summary>
//		XML,
//    /// <summary>
//    /// <see cref="PropertyType"/> for <see cref="ChannelsProperty"/>
//    /// </summary>
//		CHANNEL,
//    /// <summary>
//    /// <see cref="PropertyType"/> for user-defined <see cref="CustomProperty"/>s
//    /// </summary>
//    CUSTOM
//	}

	/// <summary>
	/// Common interface for properties
	/// </summary>
	public interface IProperty : IXUKAble
	{
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

    /// <summary>
    /// Sets the owner <see cref="ICoreNode"/> of the <see cref="IProperty"/> instance
    /// </summary>
    /// <param name="newOwner">The new owner</param>
    /// <remarks>This function is intended for internal purposes only 
    /// and should not be called by users of the toolkit</remarks>
    void setOwner(ICoreNode newOwner);
	}


}
