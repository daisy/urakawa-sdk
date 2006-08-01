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
    /// Constructs a <see cref="PropertyFactory"/> associated with the given
    /// <see cref="Presentation"/>
    /// </summary>
    /// <param name="pres"></param>
		public PropertyFactory(Presentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Presentation cannot be null");
			}
			mPresentation = pres;
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
		public ChannelsProperty createChannelsProperty()
		{
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
    /// Creates a <see cref="IProperty"/> of a given type
    /// </summary>
    /// <param name="typeString">The string representation of the type</param>
    /// <returns>The created <see cref="IProperty"/></returns>
    /// <exception cref="exception.OperationNotValidException">
    /// Thrown when the given type string representation is not recognized as a supported type
    /// </exception>
    public virtual IProperty createProperty(string typeString)
    {
      switch (typeString)
      {
        case "ChannelsProperty":
          return createChannelsProperty();
        case "XmlProperty":
          return createXmlProperty("dummy", "");
        default:
          throw new exception.OperationNotValidException(
            String.Format("Property type string representation {0} in not recognized", typeString));
      }
    }

    #endregion
  }
}
