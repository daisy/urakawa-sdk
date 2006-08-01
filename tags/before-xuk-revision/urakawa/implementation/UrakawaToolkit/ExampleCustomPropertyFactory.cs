using System;
using urakawa.core;

namespace urakawa.examples
{
	/// <summary>
	/// Custom <see cref="IPropertyFactory"/> that constructs <see cref="ExampleCustomProperty"/>s
	/// in addition to the standard <see cref="IProperty"/>s <see cref="XmlProperty"/> and <see cref="ChannelsProperty"/>
	/// </summary>
	public class ExampleCustomPropertyFactory : urakawa.core.PropertyFactory
	{
		/// <summary>
		/// Constructor setting the <see cref="Presentation"/> to which the instance belongs
		/// </summary>
		/// <param name="pres"></param>
		public ExampleCustomPropertyFactory(Presentation pres) : base(pres)
		{
		}

		/// <summary>
		/// Creates a <see cref="IProperty"/> of <see cref="Type"/> matching a given type string
		/// </summary>
		/// <param name="typeString">The given type string</param>
		/// <returns></returns>
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
