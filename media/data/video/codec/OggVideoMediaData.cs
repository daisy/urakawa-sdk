using urakawa.data;

namespace urakawa.media.data.video.codec
{
    [XukNameUglyPrettyAttribute("oggVd", "OggVideoMediaData")]
    public class OggVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_OGG_MIME_TYPE; }
        }
    }
}


