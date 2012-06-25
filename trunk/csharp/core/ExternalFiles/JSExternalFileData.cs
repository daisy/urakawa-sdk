using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using urakawa.data;
using urakawa.xuk;

namespace urakawa.ExternalFiles
{
    public class JSExternalFileData:ExternalFileData
    {

        public override string MimeType
        {
            get
            {
                return DataProviderFactory.STYLE_JS_MIME_TYPE;
            }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.JSExternalFileData;
        }


    }
}
