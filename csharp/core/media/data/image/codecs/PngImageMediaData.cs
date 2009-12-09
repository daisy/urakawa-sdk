using urakawa.data;

namespace urakawa.media.data.image.codecs
{
    public class PngImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_PNG_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.PngImageMediaData;
        }
    }
}
