using urakawa.data;

namespace urakawa.media.data.image.codec
{
    [XukNameUglyPrettyAttribute("gifIm", "GifImageMediaData")]
    public class GifImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_GIF_MIME_TYPE; }
        }
    }
}


