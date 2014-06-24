using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
{
    [XukNameUglyPrettyAttribute("dtdExFl", "DTDExternalFileData")]
    public class DTDExternalFileData : ExternalFileData
    {

        public override string MimeType
        {
            get
            {
                return DataProviderFactory.DTD_MIME_TYPE;
            }
        }

    }
}
