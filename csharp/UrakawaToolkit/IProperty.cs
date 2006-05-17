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
	public interface IProperty:IXUKable
	{
    /// <summary>
    /// Gets the <see cref="PropertyType"/> of the <see cref="IProperty"/>
    /// </summary>
    /// <returns>The <see cref="PropertyType"/></returns>
		PropertyType getPropertyType();
	}


}
