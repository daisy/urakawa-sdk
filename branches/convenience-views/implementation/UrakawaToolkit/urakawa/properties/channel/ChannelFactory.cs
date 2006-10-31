using System;
using urakawa.core;

namespace urakawa.properties.channel
{
	/// <summary>
	/// The actual implementation to be implemented by the implementation team ;)
	/// All method bodies must be completed for realizing the required business logic.
	/// -
	/// This is the DEFAULT implementation for the API/Toolkit:
	/// end-users should feel free to use this class as such,
	/// or they can sub-class it in order to specialize the instance creation process.
	/// -
	/// In addition, an end-user has the possibility to implement the
	/// singleton factory pattern, so that only one instance of the factory
	/// is used throughout the application life
	/// (by adding a method like "static Factory getFactory()").
	/// <seealso cref="IChannel"/> 
	/// <seealso cref="Channel"/>
	/// <seealso cref="IChannelsManager"/>
	/// </summary>
	public class ChannelFactory : IChannelFactory
	{
		private IChannelsManager mChannelsManager;

    /// <summary>
    /// Default constructor seting the associated <see cref="IChannelsManager"/>
    /// </summary>
		/// <param name="chMgr">
		/// The <see cref="IChannelsManager"/> associated with <see cref="IChannelFactory"/>
		/// </param>
		public ChannelFactory(IChannelsManager chMgr)
		{
			mChannelsManager = chMgr;
    }



    #region IChannelFactory Members
		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> assigned the <see cref="IChannel"/>s created
		/// by the <see cref="ChannelFactory"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		public IChannelsManager getChannelsManager()
		{
			return mChannelsManager;
		}

		/// <summary>
		/// Creates a new <see cref="IChannel"/> matching a given QName.
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IChannel"/> or <c>null</c> is the given QName is not supported</returns>
		/// <remarks>
		/// The only supported QName is <c><see cref="urakawa.ToolkitSettings.XUK_NS"/>:Channel</c> which matches <see cref="Channel"/>
		/// </remarks>
		public virtual IChannel createChannel(string localName, string namespaceUri)
		{
			if (localName == "Channel" && namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				return new Channel(mChannelsManager);
			}
			return null;
		}
    #endregion
  }
}
