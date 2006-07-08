using System;
using urakawa.core;

namespace urakawa.examples
{
	/// <summary>
	/// Example implementation of custom IProperty
	/// </summary>
	public class ExampleCustomProperty : urakawa.core.IProperty
	{
		private CoreNode mOwner;

		public string CustomData = "";

		internal ExampleCustomProperty()
		{
			// 
			// TODO: Add constructor logic here
			//
		}
		#region IProperty Members

		public urakawa.core.IProperty copy()
		{
			IPropertyFactory propFact = this.getOwner().getPresentation().getPropertyFactory();
			ExampleCustomProperty theCopy 
				= (ExampleCustomProperty)propFact.createProperty("ExampleCustomProperty");
			theCopy.setOwner(getOwner());
			return theCopy;
		}

		public urakawa.core.ICoreNode getOwner()
		{
			// TODO:  Add ExampleCustomProperty.getOwner implementation
			return mOwner;
		}

		public void setOwner(urakawa.core.ICoreNode newOwner)
		{
			if (!mOwner.GetType().IsAssignableFrom(newOwner.GetType()))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The owner must be a CoreNode of a subclass of CoreNode");
			}
			IPropertyFactory propFact = this.getOwner().getPresentation().getPropertyFactory();
			if (!propFact.GetType().IsSubclassOf(typeof(ExampleCustomPropertyFactory)))
			{
				throw new exception.OperationNotValidException(
					"The property factory of the presentation of the owner must subclass ExampleCustomPropertyFactory");
			}
			mOwner = (CoreNode)newOwner;
		}

		#endregion

		#region IXUKable Members

		public bool XUKin(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (!(source.Name == "ExampleCustomProperty" &&
				source.NodeType == System.Xml.XmlNodeType.Element))
			{
				return false;
			}

			CustomData = source.GetAttribute("CustomData");
			if (CustomData==null) CustomData = "";
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType==System.Xml.XmlNodeType.Element)
					{
						break;
					}
					if (source.NodeType==System.Xml.XmlNodeType.EndElement) return true;
					if (source.EOF) break;
				}
			}
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement("ExampleCustomProperty");
			destination.WriteAttributeString("CustomData", CustomData);
			destination.WriteEndElement();
			return true;
		}

		#endregion
	}
}
