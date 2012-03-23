using urakawa.data;

namespace urakawa.media.data.video.codec
{
    public class AviVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_AVI_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.AviVideoMediaData;
        }
    }
}


