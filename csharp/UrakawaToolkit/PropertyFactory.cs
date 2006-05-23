using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for PropertyFactory.
	/// </summary>
	public class PropertyFactory : IPropertyFactory 
	{
		private ChannelsManager mChannelsManager;

		public PropertyFactory(ChannelsManager channelsManager)
		{
			if (channelsManager == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Channels Manager cannot be null");
			}
				
			mChannelsManager = channelsManager;
		}

		public ChannelsManager getChannelsManager()
		{
			return mChannelsManager;
		}

		#region IPropertyFactory Members
		
		public IProperty createProperty(PropertyType type)
		{
			if (type == PropertyType.ChannelsProperty)
			{
				return new ChannelsProperty(mChannelsManager);
			}
			else if (type == PropertyType.XmlProperty)
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
