using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
{
    public class NCXExternalFileData : ExternalFileData
    {

        public override string MimeType
        {
            get
            {
                return DataProviderFactory.STYLE_NCX_MIME_TYPE;
            }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.NCXExternalFileData;
        }

    }
}
