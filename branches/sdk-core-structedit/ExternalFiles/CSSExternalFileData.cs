using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
{
    //typeof(CSSExternalFileData).Name
    [XukNameUglyPrettyAttribute("cssExFl", "CssExternalFileData")]
    public class CSSExternalFileData : ExternalFileData
    {
        public override string MimeType
        {
            get
            {
                return DataProviderFactory.CSS_MIME_TYPE;
            }
        }
    }
}
