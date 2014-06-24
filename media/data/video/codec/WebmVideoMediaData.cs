using urakawa.data;

namespace urakawa.media.data.video.codec
{
    [XukNameUglyPrettyAttribute("webmVd", "WebmVideoMediaData")]
    public class WebmVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_WEBM_MIME_TYPE; }
        }
    }
}


