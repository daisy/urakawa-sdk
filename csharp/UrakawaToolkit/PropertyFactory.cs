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
    /// <returns>The assocuated <see cref="Presentation"/></returns>
		public Presentation getPresentation()
		{
			return mPresentation;
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
    /// Creates a <see cref="XmlProperty"/> with given name and namespace
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
      // TODO:  Add PropertyFactory.urakawa.core.IPropertyFactory.createChannelsProperty implementation
      return null;
    }

    IXmlProperty IPropertyFactory.createXmlProperty(string name, string ns)
    {
      // TODO:  Add PropertyFactory.createXmlProperty implementation
      return null;
    }

    #endregion
  }
}
