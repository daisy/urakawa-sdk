using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for PropertyFactory.
	/// </summary>
	public class PropertyFactory : IPropertyFactory 
	{
		private Presentation mPresentation;

		public PropertyFactory(Presentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Presentation cannot be null");
			}
				
			mPresentation = pres;
		}

		public Presentation getPresentation()
		{
			return mPresentation;
		}

		#region IPropertyFactory Members
		
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
