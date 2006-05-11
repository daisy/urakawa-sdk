using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for ChannelFactory.
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
    /// Default constructor
    /// </summary>
		public ChannelFactory()
		{
    }

    IChannel IChannelFactory.createChannel(string name)
    {
      return createChannel(name);
    }
    #region IChannelFactory Members

    /// <summary>
    /// Creates a new <see cref="IChannel"/> with the given name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Channel createChannel(string name)
    {
      return new Channel(name);
    }

    #endregion
  }
}
