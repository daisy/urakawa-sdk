using System;
using System.Xml;
using urakawa.core;
using urakawa.property;

namespace urakawa.examples
{
	/// <summary>
	/// Example implementation of a custom <see cref="Property"/>
	/// </summary>
	public class ExampleCustomProperty : Property
	{

		/// <summary>
		/// The data of the custom property
		/// </summary>
		public string CustomData = "";

		internal ExampleCustomProperty() : base()
		{
		}

		#region IXUKAble Members

		/// <summary>
		/// Reads data from the attributes of the ExampleCustomProperty element
		/// </summary>
		/// <param name="source">The source xml reader</param>
		protected override void XukInAttributes(XmlReader source)
		{
			CustomData = source.GetAttribute("CustomData");
			base.XukInAttributes(source);
		}

		/// <summary>
		/// Writes data to attributes of the ExampleCustomProperty element
		/// </summary>
		/// <param name="destination">The destination xml writer</param>
		protected override void XukOutAttributes(XmlWriter destination)
		{
			if (CustomData != null)
			{
				destination.WriteAttributeString("CustomData", CustomData);
			}
			base.XukOutAttributes(destination);
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ExampleCustomProperty"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public override string getXukNamespaceUri()
		{
			return ExampleCustomPropertyFactory.NS;
		}


		#endregion

		#region IValueEquatable<Property> Members

		/// <summary>
		/// Comapres <c>this</c> with a given other <see cref="Property"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="Property"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public override bool valueEquals(Property other)
		{
			if (!base.valueEquals(other)) return false;
			if (CustomData != ((ExampleCustomProperty)other).CustomData) return false;
			return true;
		}

		#endregion
	}
}
