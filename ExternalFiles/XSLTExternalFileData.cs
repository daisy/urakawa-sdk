using urakawa.data;
using urakawa.xuk;
namespace urakawa.ExternalFiles
{
    //typeof(XSLTExternalFileData).Name
    [XukNameUglyPrettyAttribute("XsltExFl", "XsltExternalFileData")]
    public class XSLTExternalFileData : ExternalFileData
    {
        public override string MimeType
        {
            get
            {
                return DataProviderFactory.XSLT_MIME_TYPE;
            }
        }
    }
}
