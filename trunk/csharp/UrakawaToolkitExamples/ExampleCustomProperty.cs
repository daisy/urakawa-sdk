using System;
using System.Xml;
using urakawa.core;

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
			// 
			// TODO: Add constructor logic here
			//
		}
		#region IProperty Members

		/// <summary>
		/// Generates a copy of the instance
		/// </summary>
		/// <returns>The copy</returns>
		public urakawa.core.IProperty copy()
		{
			IPropertyFactory propFact = this.getOwner().getPresentation().getPropertyFactory();
			ExampleCustomProperty theCopy 
				= (ExampleCustomProperty)propFact.createProperty("ExampleCustomProperty", ExampleCustomPropertyFactory.NS);
			theCopy.setOwner(getOwner());
			return theCopy;
		}

		/// <summary>
		/// Gets the owner <see cref="urakawa.core.ICoreNode"/>
		/// </summary>
		/// <returns>The owner</returns>
		public urakawa.core.ICoreNode getOwner()
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
			IPropertyFactory propFact = newOwner.getPresentation().getPropertyFactory();
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
		public bool XUKIn(System.Xml.XmlReader source)
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
		public bool XUKOut(System.Xml.XmlWriter destination)
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

		#endregion
	}
}
