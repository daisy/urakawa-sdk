using System;
using urakawa.xuk;

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
    public sealed class ChannelFactory : GenericWithPresentationFactory<Channel>
    {

        public override string GetTypeNameFormatted()
        {
            return XukStrings.ChannelFactory;
        }
        public ChannelFactory(Presentation pres) : base(pres)
        {
        }

        protected override void InitializeInstance(Channel instance)
        {
            base.InitializeInstance(instance);
            if (m_skipManagerInitialization)
            {
                m_skipManagerInitialization = false;
                return;
            }

            Presentation.ChannelsManager.AddManagedObject(instance);
        }

        private bool m_skipManagerInitialization = false;
        public Channel Create_SkipManagerInitialization(string xukLN, string xukNS)
        {
            m_skipManagerInitialization = true;
            return Create(xukLN, xukNS);
        }

        /// <summary>
        /// Creates a <see cref="Channel"/> instance
        /// </summary>
        /// <returns>The instance</returns>
        public Channel Create()
        {
            return Create<Channel>();
        }

        public AudioChannel CreateAudioChannel()
        {
            return Create<AudioChannel>();
        }
        public TextChannel CreateTextChannel()
        {
            return Create<TextChannel>();
        }
        public ImageChannel CreateImageChannel()
        {
            return Create<ImageChannel>();
        }
        public VideoChannel CreateVideoChannel()
        {
            return Create<VideoChannel>();
        }
    }
}