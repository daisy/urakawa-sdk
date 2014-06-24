using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
{
    [XukNameUglyPrettyAttribute("CovrImgExFl", "CoverImageExternalFileData")]
    public class CoverImageExternalFileData : GenericExternalFileData
    {

    }

    [XukNameUglyPrettyAttribute("NCXExFl", "NCXExternalFileData")]
    public class NCXExternalFileData : ExternalFileData
    {
        public override string MimeType
        {
            get
            {
                return DataProviderFactory.NCX_MIME_TYPE;
            }
        }

    }

    [XukNameUglyPrettyAttribute("NavExFl", "NavDocExternalFileData")]
    public class NavDocExternalFileData : ExternalFileData
    {
        public override string MimeType
        {
            get
            {
                return DataProviderFactory.XHTML_MIME_TYPE;
            }
        }

    }
}
