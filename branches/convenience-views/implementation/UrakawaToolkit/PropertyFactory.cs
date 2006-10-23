using System;
using urakawa.core;
//using urakawa.properties.channel;
//using urakawa.properties.xml;

namespace urakawa.core.property
{
	/// <summary>
	/// Default implementation of <see cref="ICorePropertyFactory"/>.
	/// Creates only <see cref="ChannelsProperty"/>s and <see cref="XmlProperty"/>s 
	/// </summary>
	public class CorePropertyFactory : ICorePropertyFactory
	{

		private ICorePresentation mPresentation;

    /// <summary>
    /// Constructs a <see cref="CorePropertyFactory"/>
    /// </summary>
		public CorePropertyFactory()
		{
		}

    /// <summary>
    /// Gets the <see cref="Presentation"/> associated with 
    /// the <see cref="CorePropertyFactory"/>
    /// </summary>
		/// <returns>The associated <see cref="Presentation"/></returns>
		public ICorePresentation getPresentation()
		{
			return mPresentation;
		}

		/// <summary>
		/// Sets the <see cref="Presentation"/> associated with 
		/// the <see cref="CorePropertyFactory"/>
		/// </summary>
		/// <param name="pres">The associated <see cref="Presentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="pres"/> is null
		/// </exception>
		public void setPresentation(ICorePresentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Presentation cannot be null");
			}
			mPresentation = pres;
		}

		///// <summary>
		///// Creates a <see cref="ChannelsProperty"/>
		///// </summary>
		///// <returns>The newly created <see cref="ChannelsProperty"/></returns>
		///// <exception cref="urakawa.exception.OperationNotValidException">
		///// Thrown when trying to create a <see cref="ChannelsProperty"/> before the <see cref="Presentation"/>
		///// associated with the factory has been set
		///// </exception>
		//public ChannelsProperty createChannelsProperty()
		//{
		//  if (mPresentation == null)
		//  {
		//    throw new urakawa.exception.OperationNotValidException(
		//      "Can not create ChannelsProperty when the Presentation has not been set");
		//  }
		//  return new ChannelsProperty(mPresentation);
		//}

		///// <summary>
		///// Creates a <see cref="XmlProperty"/> with given name and namespace.
		///// Shorthand for <c></c>
		///// </summary>
		///// <param name="name">The given name (may not be null or <see cref="String.Empty"/></param>
		///// <param name="ns">The given namespace (may not be null)</param>
		///// <returns></returns>
		//public XmlProperty createXmlProperty(string name, string ns)
		//{
		//  return new XmlProperty(name, ns);
		//}

    #region ICorePropertyFactory Members

		//IChannelsProperty ICorePropertyFactory.createChannelsProperty()
		//{
		//  return createChannelsProperty();
		//}

		//IXmlProperty ICorePropertyFactory.createXmlProperty(string name, string ns)
		//{
		//  return createXmlProperty(name, ns);;
		//}

    /// <summary>
    /// Creates a <see cref="IProperty"/> matching a given QName
    /// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IProperty"/> or <c>null</c> if the given QName is not supported</returns>
    public virtual IProperty createProperty(string localName, string namespaceUri)
    {
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					//case "ChannelsProperty":
					//  return createChannelsProperty();
					//case "XmlProperty":
					//  return createXmlProperty("dummy", "");
				}
			}
			return null;
    }

    #endregion
  }
}
