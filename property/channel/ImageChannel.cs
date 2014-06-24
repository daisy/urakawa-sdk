using urakawa.media;
using urakawa.xuk;

namespace urakawa.property.channel
{
    [XukNameUglyPrettyAttribute("imgCh", "ImageChannel")]
    public class ImageChannel : Channel
    {

        /// <summary>
        /// Determines if a given <see cref="Media"/> can be accepted by the channel,
        /// which it can if it implements interface <see cref="AbstractImageMedia"/>
        /// </summary>
        /// <param name="m">The given media</param>
        /// <returns>A <see cref="bool"/> indicating if the given media can be accepted</returns>
        public override bool CanAccept(urakawa.media.Media m)
        {
            if (!base.CanAccept(m)) return false;
            if (m is AbstractImageMedia) return true;

#if ENABLE_SEQ_MEDIA

            if (m is SequenceMedia)
            {
                foreach (Media sm in ((SequenceMedia)m).ChildMedias.ContentsAs_Enumerable)
                {
                    if (!(sm is AbstractImageMedia)) return false;
                }
                return true;
            }
#endif //ENABLE_SEQ_MEDIA

            return false;
        }
    }
}