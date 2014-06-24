using urakawa.data;

namespace urakawa.media.data.video.codec
{
    [XukNameUglyPrettyAttribute("wmvVd", "WmvVideoMediaData")]
    public class WmvVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_WMV_MIME_TYPE; }
        }
    }
}


