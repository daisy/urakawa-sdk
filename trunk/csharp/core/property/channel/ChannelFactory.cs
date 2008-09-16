namespace urakawa.property.channel
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
    /// <seealso cref="Channel"/>
    /// <seealso cref="channel.ChannelsManager"/>
    /// </summary>
    public sealed class ChannelFactory : GenericFactory<Channel>
    {

        /// <summary>
        /// Gets the <see cref="ChannelsManager"/> assigned the <see cref="Channel"/>s created
        /// by the <see cref="ChannelFactory"/>
        /// </summary>
        /// <returns>The <see cref="ChannelsManager"/></returns>
        public ChannelsManager ChannelsManager
        {
            get { return Presentation.ChannelsManager; }
        }

        /// <summary>
        /// Creates a <see cref="Channel"/> instance
        /// </summary>
        /// <returns>The instance</returns>
        public Channel CreateChannel()
        {
            return Create<Channel>();
        }
    }
}