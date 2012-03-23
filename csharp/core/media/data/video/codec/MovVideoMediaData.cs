using urakawa.data;

namespace urakawa.media.data.video.codec
{
    public class MovVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_MOV_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.MovVideoMediaData;
        }
    }
}


