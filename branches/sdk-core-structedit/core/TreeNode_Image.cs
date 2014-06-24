using urakawa.media;
using urakawa.property.channel;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public AbstractVideoMedia GetVideoMedia()
        {
            Media med = GetMediaInVideoChannel();
            if (med != null)
            {
                return med as AbstractVideoMedia;
            }
            return null;
        }

        public AbstractImageMedia GetImageMedia()
        {
            Media med = GetMediaInImageChannel();
            if (med != null)
            {
                return med as AbstractImageMedia;
            }
            return null;
        }

#if ENABLE_SEQ_MEDIA

        public SequenceMedia GetImageSequenceMedia()
        {
            Media med = GetMediaInImageChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }
        
#endif //ENABLE_SEQ_MEDIA


        public Media GetMediaInImageChannel()
        {
            return GetMediaInChannel<ImageChannel>();
        }


        public Media GetMediaInVideoChannel()
        {
            return GetMediaInChannel<VideoChannel>();
        }
    }
}
