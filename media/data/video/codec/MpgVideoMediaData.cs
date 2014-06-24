using urakawa.data;

namespace urakawa.media.data.video.codec
{
    [XukNameUglyPrettyAttribute("mpgVd", "MpgVideoMediaData")]
    public class MpgVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_MPG_MIME_TYPE; }
        }

    }

    [XukNameUglyPrettyAttribute("mp4Vd", "Mp4VideoMediaData")]
    public class Mp4VideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_MP4_MIME_TYPE; }
        }
    }
}


