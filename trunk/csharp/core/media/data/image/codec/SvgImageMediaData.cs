using urakawa.data;

namespace urakawa.media.data.image.codec
{
    public class SvgImageMediaData : ImageMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.IMAGE_SVG_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.SvgImageMediaData;
        }
    }
}


