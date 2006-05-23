using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IPropertyFactory.
	/// </summary>
	public interface IPropertyFactory
	{
		IProperty createProperty(PropertyType type);
	}
}
