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
		/// Namespace of the <see cref="ExampleCustomProperty"/> XUK representation
		/// </summary>
		public static string NS = "http://www.daisy.org/urakawa/example";

		/// <summary>
		/// Constructor setting the <see cref="Presentation"/> to which the instance belongs
		/// </summary>
		/// <param name="pres"></param>
		public ExampleCustomPropertyFactory(Presentation pres) : base(pres)
		{
		}

		/// <summary>
		/// Creates a <see cref="IProperty"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IProperty"/> or <c>null</c> if the given QName is not supported</returns>
		public override IProperty createProperty(string localName, string namespaceUri)
		{
			if (localName == "ExampleCustomProperty" && namespaceUri == NS)
			{
				return new ExampleCustomProperty();
			}
			return base.createProperty(localName, namespaceUri);
		}
	}
}
