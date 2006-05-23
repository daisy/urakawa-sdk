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

    #region IPropertyFactory Members

    /// <summary>
    /// Creates a <see cref="IProperty"/> of a given <see cref="PropertyType"/>
    /// </summary>
    /// <param name="type">The <see cref="PropertyType"/></param>
    /// <returns>The newly created <see cref="IProperty"/></returns>
		public IProperty createProperty(PropertyType type)
		{
			if (type == PropertyType.CHANNEL)
			{
				return new ChannelsProperty(mPresentation);
			}
			else if (type == PropertyType.XML)
			{
				return new XmlProperty();
			}
			else
			{
				throw new exception.PropertyTypeIsIllegalException
					("The property type " + type.ToString() + " is not supported.");

			}
		}
		

		#endregion


	}
}
