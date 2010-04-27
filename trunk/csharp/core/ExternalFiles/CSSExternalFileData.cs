using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
{
    public class CSSExternalFileData : ExternalFileData
    {

        public override string MimeType
        {
            get
            {
                return DataProviderFactory.STYLE_CSS_MIME_TYPE;
            }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.CSSExternalFileData;
        }

    }
}
