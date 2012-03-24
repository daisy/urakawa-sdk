using urakawa.data;

namespace urakawa.media.data.video.codec
{
    public class MpgVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_MPG_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.MpgVideoMediaData;
        }
    }
    public class Mp4VideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_MP4_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.Mp4VideoMediaData;
        }
    }
}


