using urakawa.data;

namespace urakawa.media.data.image.codec
{
    public class BmpImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_BMP_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.BmpImageMediaData;
        }
    }
}


