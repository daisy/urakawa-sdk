using System;

namespace urakawa.core.property
{
  /// <summary>
  /// Enumeration of the possible types of properties
  /// </summary>
	public enum PropertyType
	{
		None,
		//TODO: Decide if we need the type None, and if so: why?
    /// <summary>
    /// <see cref="PropertyType"/> for <see cref="PropertyType.StructureProperty"/>
    /// </summary>
		StructureProperty,
    /// <summary>
    /// <see cref="PropertyType"/> for <see cref="PropertyType.StructureProperty"/>
    /// </summary>
		SMILProperty,
    /// <summary>
    /// <see cref="ChannelsProperty"/> for <see cref="PropertyType.ChannelsProperty"/>
    /// </summary>
    ChannelsProperty
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IProperty
	{
		PropertyType getPropertyType();
	}


}
