using System;
using urakawa.core;
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
	public class PropertyFactory : CorePropertyFactory, IPropertyFactory
	{

		/// <summary>
		/// Gets the <see cref="IPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <c>this</c> has not been initialized with a <see cref="IPresentation"/></exception>
		public new IPresentation getPresentation()
		{
			ICorePresentation pres = base.getPresentation();
			if (!(pres is IPresentation))
			{
				throw new exception.IsNotInitializedException(
					"The PropertyFactory has not been initialized with a IPresentation");
			}
			return (IPresentation)pres;
		}

		/// <summary>
		/// Sets the <see cref="IPresentation"/> of <c>this</c> - initializer
		/// </summary>
		/// <param name="newPres">The presentation</param>
		public virtual void setPresentation(IPresentation newPres)
		{
			base.setPresentation(newPres);
		}

		/// <summary>
		/// Sets the <see cref="IPresentation"/> of <c>this</c> - initializer
		/// </summary>
		/// <param name="newPres">The presentation - must be a <see cref="IPresentation"/></param>
		public override void setPresentation(ICorePresentation newPres)
		{
			if (!(newPres is IPresentation))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The presentation no a PropertyFactory must be an IPresentation");
			}
			base.setPresentation(newPres);
		}

		#region IChannelsPropertyFactory Members

		/// <summary>
		/// Creates a <see cref="ChannelsProperty"/>
		/// </summary>
		/// <returns>The created <see cref="ChannelsProperty"/></returns>
		public ChannelsProperty createChannelsProperty()
		{
			return new ChannelsProperty(getPresentation());
		}

		#endregion

		#region ICorePropertyFactory Members

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
		/// Creates an <see cref="XmlProperty"/> instance
		/// </summary>
		/// <returns>The created instance</returns>
		public XmlProperty createXmlProperty()
		{
			return new XmlProperty();
		}

		/// <summary>
		/// Creates an <see cref="XmlAttribute"/> instance with a given <see cref="XmlProperty"/> parent
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <returns>The created instance</returns>
		public XmlAttribute createXmlAttribute(XmlProperty parent)
		{
			return new XmlAttribute(parent);
		}

		/// <summary>
		/// Creates a <see cref="XmlAttribute"/> of type matching a given QName with a given parent <see cref="XmlProperty"/>
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created instance or <c>null</c> if the QName is not recognized</returns>
		public XmlAttribute createXmlAttribute(XmlProperty parent, string localName, string namespaceUri)
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
