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
		private IChannelPresentation mChannelPresentation = null;

    /// <summary>
    /// Default constructor
    /// </summary>
		public ChannelFactory()
		{
    }



    #region IChannelFactory Members
		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> assigned the <see cref="IChannel"/>s created
		/// by the <see cref="ChannelFactory"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		public IChannelsManager getChannelsManager()
		{
			return getPresentation().getChannelsManager();
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
				return new Channel(getChannelsManager());
			}
			return null;
		}
 
		/// <summary>
		/// Gets the <see cref="IChannelPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IChannelPresentation"/> has been associated with <c>this</c>
		/// </exception>
		public IChannelPresentation getPresentation()
		{
			if (mChannelPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"No IChannelPresentation has been asociated with the ChannelFactory");
			}
			return mChannelPresentation;
		}

		/// <summary>
		/// Associates a <see cref="IChannelPresentation"/> with <c>this</c>
		/// </summary>
		/// <param name="pres">The <see cref="IChannelPresentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when trying to associate <c>this</c> with a <c>null</c> <see cref="IChannelPresentation"/>
		/// </exception>
		public void setPresentation(IChannelPresentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException("The IChannelPresentation is null");
			}
			mChannelPresentation = pres;
		}

		#endregion
	}
}
