using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
{
    public class PLSExternalFileData : ExternalFileData
    {

        public override string MimeType
        {
            get
            {
                return DataProviderFactory.STYLE_PLS_MIME_TYPE;
            }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.PLSExternalFileData;
        }

    }
}
