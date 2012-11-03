using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
{
    public class CoverImageExternalFileData : GenericExternalFileData
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.CoverImageExternalFileData;
        }

    }

    public class NCXExternalFileData : ExternalFileData
    {
        public override string MimeType
        {
            get
            {
                return DataProviderFactory.NCX_MIME_TYPE;
            }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.NCXExternalFileData;
        }

    }

    public class NavDocExternalFileData : ExternalFileData
    {
        public override string MimeType
        {
            get
            {
                return DataProviderFactory.XHTML_MIME_TYPE;
            }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.NavDocExternalFileData;
        }

    }
}
