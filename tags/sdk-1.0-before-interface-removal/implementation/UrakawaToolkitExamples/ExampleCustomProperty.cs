using System;
using System.Xml;
using urakawa.core;
using urakawa.core.property;

namespace urakawa.examples
{
	/// <summary>
	/// Example implementation of a custom <see cref="IProperty"/>
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
		/// <returns>A <see cref="bool"/> indicating if the data was succesfully read</returns>
		protected override bool XukInAttributes(XmlReader source)
		{
			CustomData = source.GetAttribute("CustomData");
			return base.XukInAttributes(source);
		}

		/// <summary>
		/// Writes data to attributes of the ExampleCustomProperty element
		/// </summary>
		/// <param name="destination">The destination xml writer</param>
		/// <returns>A <see cref="bool"/> indicating if the attributes were succesfully written</returns>
		protected override bool XukOutAttributes(XmlWriter destination)
		{
			if (CustomData != null)
			{
				destination.WriteAttributeString("CustomData", CustomData);
			}
			return base.XukOutAttributes(destination);
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

		#region IValueEquatable<IProperty> Members

		/// <summary>
		/// Comapres <c>this</c> with a given other <see cref="IProperty"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IProperty"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public override bool ValueEquals(IProperty other)
		{
			if (!base.ValueEquals(other)) return false;
			if (CustomData != ((ExampleCustomProperty)other).CustomData) return false;
			return true;
		}

		#endregion
	}
}
