using urakawa.data;

namespace urakawa.media.data.image.codec
{
    public class JpgImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_JPG_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.JpgImageMediaData;
        }
    }
}


