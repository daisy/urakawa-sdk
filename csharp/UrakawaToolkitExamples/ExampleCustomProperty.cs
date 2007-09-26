using System;
using System.Xml;
using urakawa.core;
using urakawa.property;
using urakawa.property.xml;

namespace urakawa.examples
{
	/// <summary>
	/// Example implementation of a custom <see cref="Property"/>
	/// </summary>
	public class ExampleCustomProperty : XmlProperty
	{

		/// <summary>
		/// The data of the custom property
		/// </summary>
		public string CustomData = "";

		internal ExampleCustomProperty() : base()
		{
		}

		/// <summary>
		/// Creates a copy of the <see cref="ExampleCustomProperty"/>
		/// </summary>
		/// <returns>The copy</returns>
		public new ExampleCustomProperty copy()
		{
			return copyProtected() as ExampleCustomProperty;
		}

		/// <summary>
		/// Protected version of <see cref="copy"/> - in place as part of a technicality to have <see cref="copy"/>
		/// return <see cref="ExampleCustomProperty"/> instead of <see cref="XmlProperty"/>
		/// </summary>
		/// <returns>The copy</returns>
		protected override Property copyProtected()
		{
			ExampleCustomProperty exProp = base.copyProtected() as ExampleCustomProperty;
			if (exProp == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The property factory can not create a ExampleCustomProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			exProp.CustomData = CustomData;
			return exProp;
		}

		/// <summary>
		/// Exports the <see cref="ExampleCustomProperty"/> to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported <see cref="ExampleCustomProperty"/></returns>
		public new ExampleCustomProperty export(Presentation destPres)
		{
			return exportProtected(destPres) as ExampleCustomProperty;
		}

		/// <summary>
		/// Protected version of <see cref="export"/> - in place as part of a technicality to have <see cref="export"/>
		/// return <see cref="ExampleCustomProperty"/> instead of <see cref="XmlProperty"/>
		/// </summary>
		/// <returns>The export</returns>
		protected override Property exportProtected(Presentation destPres)
		{
			ExampleCustomProperty exProp = base.exportProtected(destPres) as ExampleCustomProperty;
			if (exProp == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The property factory can not create a ExampleCustomProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			exProp.CustomData = CustomData;
			return exProp;
		}

		#region IXUKAble Members

		/// <summary>
		/// Reads data from the attributes of the ExampleCustomProperty element
		/// </summary>
		/// <param name="source">The source xml reader</param>
		protected override void XukInAttributes(XmlReader source)
		{
			base.XukInAttributes(source);
			CustomData = source.GetAttribute("customData");
		}

		/// <summary>
		/// Writes data to attributes of the ExampleCustomProperty element
		/// </summary>
		/// <param name="destination">The destination xml writer</param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			base.XukOutAttributes(destination, baseUri);
			if (CustomData != null)
			{
				destination.WriteAttributeString("customData", CustomData);
			}
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
