using urakawa.data;

namespace urakawa.media.data.video.codec
{
    public class WmvVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_WMV_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.WmvVideoMediaData;
        }
    }
}


