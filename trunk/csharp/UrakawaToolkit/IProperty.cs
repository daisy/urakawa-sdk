using System;

namespace urakawa.core.property
{
	public enum PropertyType
	{
		None,
		//TODO: Decide if we need the type None, and if so: why?
		StructureProperty,
		SMILProperty,
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
