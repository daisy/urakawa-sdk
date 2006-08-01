using System;

namespace urakawa.core
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
	/// </summary>
	public class ChannelFactory : IChannelFactory
	{
		/// <summary>
		/// The namespace uri of the XUK files
		/// </summary>
		public static string XUK_NS = PropertyFactory.XUK_NS;

    /// <summary>
    /// Default constructor
    /// </summary>
		public ChannelFactory()
		{
    }



    #region IChannelFactory Members

    IChannel IChannelFactory.createChannel(string name)
    {
      return createChannel(name);
    }

    /// <summary>
    /// Creates a new <see cref="IChannel"/> with the given name
    /// </summary>
    /// <param name="name">The name of the <see cref="Channel"/> to create</param>
    /// <returns></returns>
    public Channel createChannel(string name)
    {
      return new Channel(name);
    }

		/// <summary>
		/// Creates a new <see cref="IChannel"/> matching a given QName.
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IChannel"/> or <c>null</c> is the given QName is not supported</returns>
		/// <remarks>
		/// The only supported QName is <c><see cref="PropertyFactory.XUK_NS"/>:Channel</c> which matches <see cref="Channel"/>
		/// </remarks>
		public virtual IChannel createChannel(string localName, string namespaceUri)
		{
			if (localName == "Channel" && namespaceUri == XUK_NS)
			{
				return new Channel("");
			}
			return null;
		}

    #endregion
  }
}
