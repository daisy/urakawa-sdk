using System;
using System.Xml;
using urakawa.core;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.property
{
	/// <summary>
	/// Factory for creating <see cref="Property"/>s
	/// </summary>
	public class PropertyFactory 
		: GenericPropertyFactory, IChannelsPropertyFactory, urakawa.property.xml.IXmlPropertyFactory, IXukAble
	{
		/// <summary>
		/// Defautl constructor
		/// </summary>
		protected internal PropertyFactory()
		{
		}

		#region IChannelsPropertyFactory Members

		/// <summary>
		/// Creates a <see cref="ChannelsProperty"/>
		/// </summary>
		/// <returns>The created <see cref="ChannelsProperty"/></returns>
		public ChannelsProperty createChannelsProperty()
		{
			ChannelsProperty newProp = new ChannelsProperty();
			newProp.setPresentation(getPresentation());
			return newProp;
		}

		#endregion

		#region IGenericPropertyFactory Members

		/// <summary>
		/// Creates a <see cref="Property"/> of type matching a given QName
		/// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Property"/> or <c>null</c> is the given QName is not recognized</returns>
		public override Property createProperty(string localName, string namespaceUri)
		{
			if (namespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "XmlProperty":
						return createXmlProperty();
					case "ChannelsProperty":
						return createChannelsProperty();
				}
			}
			return base.createProperty(localName, namespaceUri);;
		}

		#endregion

		#region IXmlPropertyFactory Members

		/// <summary>
		/// Creates an <see cref="urakawa.property.xml.XmlProperty"/> instance
		/// </summary>
		/// <returns>The created instance</returns>
		public urakawa.property.xml.XmlProperty createXmlProperty()
		{
			urakawa.property.xml.XmlProperty newProp = new urakawa.property.xml.XmlProperty();
			newProp.setPresentation(getPresentation());
			return newProp;
		}

		/// <summary>
		/// Creates an <see cref="urakawa.property.xml.XmlAttribute"/> instance 
		/// with a given <see cref="urakawa.property.xml.XmlProperty"/> parent
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <returns>The created instance</returns>
		public urakawa.property.xml.XmlAttribute createXmlAttribute(urakawa.property.xml.XmlProperty parent)
		{
			return new urakawa.property.xml.XmlAttribute(parent);
		}

		/// <summary>
		/// Creates a <see cref="urakawa.property.xml.XmlAttribute"/> of type 
		/// matching a given QName with a given parent <see cref="urakawa.property.xml.XmlProperty"/>
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created instance or <c>null</c> if the QName is not recognized</returns>
		public urakawa.property.xml.XmlAttribute createXmlAttribute(
			urakawa.property.xml.XmlProperty parent, string localName, string namespaceUri)
		{
			if (namespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "XmlAttribute":
						return createXmlAttribute(parent);
				}
			}
			return null;
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="PropertyFactory"/> from a PropertyFactory xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read PropertyFactory from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of PropertyFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a PropertyFactory xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a PropertyFactory xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a PropertyFactory element to a XUK file representing the <see cref="PropertyFactory"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void XukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination, baseUri);
				XukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of PropertyFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a PropertyFactory element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void XukOutAttributes(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Write the child elements of a PropertyFactory element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void XukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="PropertyFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="PropertyFactory"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
