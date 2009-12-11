using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
    {
    public class DTDExternalFileData:ExternalFileData
        {

        public override string MimeType
            {
            get
                {
                return DataProviderFactory.DTD_MIME_TYPE;
                }
            }

        public override string GetTypeNameFormatted ()
            {
            return XukStrings.DTDExternalFileData;
            }
        }
    }
