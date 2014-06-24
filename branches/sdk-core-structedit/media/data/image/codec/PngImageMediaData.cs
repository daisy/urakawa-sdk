using urakawa.data;

namespace urakawa.media.data.image.codec
{
    [XukNameUglyPrettyAttribute("pngIm", "PngImageMediaData")]
    public class PngImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_PNG_MIME_TYPE; }
        }

    }
}


