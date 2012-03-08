using urakawa.data;

namespace urakawa.media.data.image.codec
{
    public class GifImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_GIF_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.GifImageMediaData;
        }
    }
}


