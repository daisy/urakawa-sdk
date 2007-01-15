using System;
using urakawa.core.property;
using urakawa.properties.channel;
using urakawa.properties.xml;

namespace urakawa
{
	/// <summary>
	/// <para>Default implementation of <see cref="IPropertyFactory"/>.</para>
	/// <para>
	/// Supports creation of <see cref="ChannelsProperty"/> matching 
	/// QName <see cref="ToolkitSettings.XUK_NS"/>:ChannelsProperty
	/// and <see cref="XmlProperty"/> matching </para>
	/// QName <see cref="ToolkitSettings.XUK_NS"/>:XmlProperty
	/// </summary>
	public class PropertyFactory : IPropertyFactory
	{
		private IPresentation mPresentation;

		/// <summary>
		/// Gets the <see cref="IPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <c>this</c> has not been initialized with a <see cref="IPresentation"/></exception>
		public IPresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"No presentation has been associated with this yet");
			}
			return mPresentation;
		}

		/// <summary>
		/// Sets the <see cref="IPresentation"/> of <c>this</c>
		/// </summary>
		/// <param localName="newPres"></param>
		public void setPresentation(IPresentation newPres)
		{
			if (newPres == null)
			{
				throw new exception.MethodParameterIsNullException("The presentation can not be null");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException("This has not been initialized with a presentation");
			}
			mPresentation = newPres;
		}

		#region IChannelsPropertyFactory Members

		/// <summary>
		/// Creates a <see cref="ChannelsProperty"/>
		/// </summary>
		/// <returns>The created <see cref="ChannelsProperty"/></returns>
		public IChannelsProperty createChannelsProperty()
		{
			return new ChannelsProperty(getPresentation());
		}

		#endregion

		#region ICorePropertyFactory Members

		/// <summary>
		/// Creates a <see cref="IProperty"/> of type matching a given QName
		/// </summary>
		/// <param localName="localName">The local localName part of the QName</param>
		/// <param localName="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IProperty"/> or <c>null</c> is the given QName is not recognized</returns>
		public virtual IProperty createProperty(string localName, string namespaceUri)
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
			return null;
		}

		#endregion

		#region IXmlPropertyFactory Members

		/// <summary>
		/// Creates an <see cref="XmlProperty"/> instance
		/// </summary>
		/// <returns>The created instance</returns>
		public IXmlProperty createXmlProperty()
		{
			return new XmlProperty();
		}

		/// <summary>
		/// Creates an <see cref="XmlAttribute"/> instance with a given <see cref="IXmlProperty"/> parent
		/// </summary>
		/// <param localName="parent">The parent</param>
		/// <returns>The created instance</returns>
		public IXmlAttribute createXmlAttribute(IXmlProperty parent)
		{
			return new XmlAttribute(parent);
		}

		/// <summary>
		/// Creates a <see cref="IXmlAttribute"/> of type matching a given QName with a given parent <see cref="IXmlProperty"/>
		/// </summary>
		/// <param localName="parent">The parent</param>
		/// <param localName="localName">The local localName part of the QName</param>
		/// <param localName="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created instance or <c>null</c> if the QName is not recognized</returns>
		public IXmlAttribute createXmlAttribute(IXmlProperty parent, string localName, string namespaceUri)
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

		//#region IXukAble Members

		///// <summary>
		///// Reads the <see cref="PropertyFactory"/> from an xuk xml element
		///// </summary>
		///// <param localName="source">The source <see cref="XmlReader"/></param>
		///// <returns>A <see cref="bool"/> indicating if the <see cref="PropertyFactory"/> was succesfully read</returns>
		//public bool XukIn(System.Xml.XmlReader source)
		//{
		//  if (source == null)
		//  {
		//    throw new exception.MethodParameterIsNullException("The source xml reader is null");
		//  }
		//  if (!source.NodeType == XmlNodeType.Element) return false;
		//  if (!source.IsEmptyElement)
		//  {
		//    //Read past element subtree
		//    source.ReadSubtree().Close();
		//  }
		//}

		///// <summary>
		///// Write a xuk xml element representing the <see cref="PropertyFactory"/>
		///// </summary>
		///// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		///// <returns>A <see cref="bool"/> indicating if the element was succesfully written</returns>
		//public bool XukOut(System.Xml.XmlWriter destination)
		//{
		//  destination.WriteElementString(getXukLocalName(), getXukNamespaceUri());
		//}

		
		///// <summary>
		///// Gets the local localName part of the QName representing a <see cref="IPropertyFactory"/> in Xuk
		///// </summary>
		///// <returns>The local localName part</returns>
		//public string getXukLocalName()
		//{
		//  return this.GetType().Name;
		//}

		///// <summary>
		///// Gets the namespace uri part of the QName representing a <see cref="IPropertyFactory"/> in Xuk
		///// </summary>
		///// <returns>The namespace uri part</returns>
		//public string getXukNamespaceUri()
		//{
		//  return urakawa.ToolkitSettings.XUK_NS;
		//}

		//#endregion

		#region ICorePropertyFactory Members


		urakawa.core.ICorePresentation ICorePropertyFactory.getPresentation()
		{
			return getPresentation();
		}

		void ICorePropertyFactory.setPresentation(urakawa.core.ICorePresentation pres)
		{
			if (!(pres is IPresentation))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The presentation associated with a IPropertyFactory must an IPresentation");
			}
			setPresentation((IPresentation)pres);
		}

		#endregion
	}
}
