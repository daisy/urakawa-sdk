using urakawa.data;

namespace urakawa.media.data.video.codec
{
    [XukNameUglyPrettyAttribute("aviVd", "AviVideoMediaData")]
    public class AviVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_AVI_MIME_TYPE; }
        }
    }
}


