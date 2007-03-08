using System;
using System.Xml;
using urakawa.core;
using urakawa.core.property;

namespace urakawa.examples
{
	/// <summary>
	/// Example implementation of a custom <see cref="IProperty"/>
	/// </summary>
	public class ExampleCustomProperty : IProperty
	{

		private CoreNode mOwner;

		/// <summary>
		/// The data of the custom property
		/// </summary>
		public string CustomData = "";

		internal ExampleCustomProperty()
		{
		}
		#region IProperty Members

		/// <summary>
		/// Generates a copy of the instance
		/// </summary>
		/// <returns>The copy</returns>
		public IProperty copy()
		{
			ICorePropertyFactory propFact = this.getOwner().getPresentation().getPropertyFactory();
			ExampleCustomProperty theCopy 
				= (ExampleCustomProperty)propFact.createProperty("ExampleCustomProperty", ExampleCustomPropertyFactory.NS);
			theCopy.setOwner(getOwner());
			return theCopy;
		}

		/// <summary>
		/// Gets the owner <see cref="urakawa.core.ICoreNode"/>
		/// </summary>
		/// <returns>The owner</returns>
		public ICoreNode getOwner()
		{
			return mOwner;
		}

		/// <summary>
		/// Sets the owner <see cref="urakawa.core.ICoreNode"/>
		/// </summary>
		/// <param name="newOwner">The new owner</param>
		public void setOwner(urakawa.core.ICoreNode newOwner)
		{
			if (!typeof(CoreNode).IsAssignableFrom(newOwner.GetType()))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The owner must be a CoreNode of a subclass of CoreNode");
			}
			ICorePropertyFactory propFact = newOwner.getPresentation().getPropertyFactory();
			if (!typeof(ExampleCustomPropertyFactory).IsAssignableFrom(propFact.GetType()))
			{
				throw new exception.OperationNotValidException(
					"The property factory of the presentation of the owner must assignable to a ExampleCustomPropertyFactory");
			}
			mOwner = (CoreNode)newOwner;
		}

		#endregion

		#region IXUKAble Members

		/// <summary>
		/// Reads the instance from a ExampleCustomProperty element in a XUK document
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the instance was succesfully read</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "ExampleCustomProperty") return false;
			if (source.NamespaceURI != ExampleCustomPropertyFactory.NS) return false;

			CustomData = source.GetAttribute("CustomData");
			if (CustomData==null) CustomData = "";
			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType==System.Xml.XmlNodeType.Element)
				{
					return false;
				}
				else if (source.NodeType==System.Xml.XmlNodeType.EndElement) break;
				if (source.EOF) break;
			}
			return true;
		}

		/// <summary>
		/// Writes an ExampleCustomProperty element to a XUK file representing the instance.
		/// </summary>
		/// <param name="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the element was succesfully written</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement("ExampleCustomProperty", ExampleCustomPropertyFactory.NS);
			destination.WriteAttributeString("CustomData", CustomData);
			destination.WriteEndElement();
			return true;
		}


		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="ExampleCustomProperty"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ExampleCustomProperty"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}


		#endregion

		#region IValueEquatable<IProperty> Members

		/// <summary>
		/// Comapres <c>this</c> with a given other <see cref="IProperty"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IProperty"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public bool ValueEquals(IProperty other)
		{
			if (!(other is ExampleCustomProperty)) return false;
			if (CustomData != ((ExampleCustomProperty)other).CustomData) return false;
			return true;
		}

		#endregion
	}
}
