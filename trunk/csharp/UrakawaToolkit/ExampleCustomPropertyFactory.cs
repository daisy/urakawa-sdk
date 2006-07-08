using System;
using urakawa.core;

namespace urakawa.examples
{
	/// <summary>
	/// 
	/// </summary>
	public class ExampleCustomPropertyFactory : urakawa.core.PropertyFactory
	{
		public ExampleCustomPropertyFactory(Presentation pres) : base(pres)
		{
			// 
			// TODO: Add constructor logic here
			//
		}

		public override IProperty createProperty(string typeString)
		{
			if (typeString=="ExampleCustomProperty")
			{
				return new ExampleCustomProperty();
			}
			return base.createProperty(typeString);
		}
	}
}
