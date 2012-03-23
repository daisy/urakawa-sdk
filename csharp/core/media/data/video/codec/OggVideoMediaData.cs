using urakawa.data;

namespace urakawa.media.data.video.codec
{
    public class OggVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_OGG_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.OggVideoMediaData;
        }
    }
}


