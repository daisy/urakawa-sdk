using urakawa.data;

namespace urakawa.media.data.image.codec
{
    [XukNameUglyPrettyAttribute("bmpIm", "BmpImageMediaData")]
    public class BmpImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_BMP_MIME_TYPE; }
        }
    }
}


