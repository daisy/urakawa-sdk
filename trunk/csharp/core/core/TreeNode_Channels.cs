using urakawa.media;
using urakawa.property.channel;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public Media GetMediaInChannel<T>() where T : Channel
        {
            ChannelsProperty chProp = GetProperty<ChannelsProperty>();
            if (chProp != null)
            {
                T channel = null;

                foreach (Channel ch in Presentation.ChannelsManager.ManagedObjects.ContentsAs_YieldEnumerable)
                {
                    if (ch is T)
                    {
                        channel = ch as T;
                        break;
                    }
                }
                if (channel != null)
                {
                    Media med = chProp.GetMedia(channel);
                    return med;
                }
            }
            return null;
        }

        public ChannelsProperty GetChannelsProperty()
        {
            return GetProperty<ChannelsProperty>();
        }

        public bool HasChannelsProperty
        {
            get { return GetProperty<ChannelsProperty>() != null; }
        }

        public ChannelsProperty GetOrCreateChannelsProperty()
        {
            return GetOrCreateProperty<ChannelsProperty>();
        }
    }
}
