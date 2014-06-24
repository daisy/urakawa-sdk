using urakawa.media;
using urakawa.xuk;

namespace urakawa.property.channel
{
    [XukNameUglyPrettyAttribute("vidCh", "VideoChannel")]
    public class VideoChannel : Channel
    {
        public override bool CanAccept(urakawa.media.Media m)
        {
            if (!base.CanAccept(m)) return false;
            if (m is AbstractVideoMedia) return true;

#if ENABLE_SEQ_MEDIA

            if (m is SequenceMedia)
            {
                foreach (Media sm in ((SequenceMedia)m).ChildMedias.ContentsAs_Enumerable)
                {
                    if (!(sm is AbstractVideoMedia)) return false;
                }
                return true;
            }
#endif //ENABLE_SEQ_MEDIA

            return false;
        }
    }
}
