using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="IPropertyFactory"/>.
	/// Creates only <see cref="ChannelsProperty"/>s and <see cref="XmlProperty"/>s 
	/// </summary>
	public class PropertyFactory : IPropertyFactory
	{

		private Presentation mPresentation;

    /// <summary>
    /// Constructs a <see cref="PropertyFactory"/>
    /// </summary>
		public PropertyFactory()
		{
		}

    /// <summary>
    /// Gets the <see cref="Presentation"/> associated with 
    /// the <see cref="PropertyFactory"/>
    /// </summary>
    /// <returns>The assocaated <see cref="Presentation"/></returns>
		public Presentation getPresentation()
		{
			return mPresentation;
		}

		/// <summary>
		/// Gets the <see cref="Presentation"/> associated with 
		/// the <see cref="PropertyFactory"/>
		/// </summary>
		/// <param name="pres">The assocuated <see cref="Presentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="pres"/> is null
		/// </exception>
		internal void setPresentation(Presentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Presentation cannot be null");
			}
			mPresentation = pres;
		}

    /// <summary>
    /// Creates a <see cref="ChannelsProperty"/>
    /// </summary>
    /// <returns>The newly created <see cref="ChannelsProperty"/></returns>
		/// <exception cref="urakawa.exception.OperationNotValidException">
		/// Thrown when trying to create a <see cref="ChannelsProperty"/> before the <see cref="Presentation"/>
		/// associated with the factory has been set
		/// </exception>
		public ChannelsProperty createChannelsProperty()
		{
			if (mPresentation == null)
			{
				throw new urakawa.exception.OperationNotValidException(
					"Can not create ChannelsProperty when the Presentation has not been set");
			}
			return new ChannelsProperty(mPresentation);
		}

    /// <summary>
    /// Creates a <see cref="XmlProperty"/> with given name and namespace.
    /// Shorthand for <c></c>
    /// </summary>
    /// <param name="name">The given name (may not be null or <see cref="String.Empty"/></param>
    /// <param name="ns">The given namespace (may not be null)</param>
    /// <returns></returns>
    public XmlProperty createXmlProperty(string name, string ns)
    {
      return new XmlProperty(name, ns);
    }

    #region IPropertyFactory Members

    IChannelsProperty IPropertyFactory.createChannelsProperty()
    {
      return createChannelsProperty();
    }

    IXmlProperty IPropertyFactory.createXmlProperty(string name, string ns)
    {
      return createXmlProperty(name, ns);;
    }

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
					case "ChannelsProperty":
						return createChannelsProperty();
					case "XmlProperty":
						return createXmlProperty("dummy", "");
				}
			}
			return null;
    }

    #endregion
  }
}
