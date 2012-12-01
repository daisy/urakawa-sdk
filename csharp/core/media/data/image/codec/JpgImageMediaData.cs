using urakawa.data;

namespace urakawa.media.data.image.codec
{
    [XukNameUglyPrettyAttribute("jpgIm", "JpgImageMediaData")]
    public class JpgImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_JPG_MIME_TYPE; }
        }
    }
}


