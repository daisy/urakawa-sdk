using urakawa.media;
using urakawa.property.channel;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public AbstractImageMedia GetImageMedia()
        {
            Media med = GetMediaInImageChannel();
            if (med != null)
            {
                return med as AbstractImageMedia;
            }
            return null;
        }

        public SequenceMedia GetImageSequenceMedia()
        {
            Media med = GetMediaInImageChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }

        public Media GetMediaInImageChannel()
        {
            return GetMediaInChannel<ImageChannel>();
        }
    }
}
