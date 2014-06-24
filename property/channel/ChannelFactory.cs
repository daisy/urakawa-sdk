using System;
using urakawa.xuk;

namespace urakawa.property.channel
{
    [XukNameUglyPrettyAttribute("chFct", "ChannelFactory")]
    public sealed class ChannelFactory : GenericWithPresentationFactory<Channel>
    {
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
        public AudioXChannel CreateAudioXChannel()
        {
            return Create<AudioXChannel>();
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