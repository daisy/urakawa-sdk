using urakawa.data;

namespace urakawa.media.data.video.codec
{
    public class WebmVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_WEBM_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.WebmVideoMediaData;
        }
    }
}


