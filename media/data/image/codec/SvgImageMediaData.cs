using urakawa.data;

namespace urakawa.media.data.image.codec
{
    [XukNameUglyPrettyAttribute("svgIm", "SvgImageMediaData")]
    public class SvgImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_SVG_MIME_TYPE; }
        }

    }
}


