using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace urakawa.media.data.image
    {
    public class JpgImageMediaData:ImageMediaData
        {
        protected override MediaData CopyProtected ()
            {
            throw new NotImplementedException ();
            }

        protected override MediaData ExportProtected ( Presentation destPres )
            {
            throw new NotImplementedException ();
            }

        public override IEnumerable<DataProvider> UsedDataProviders
            {
            get
                {
                throw new NotImplementedException ();
                }
            }

        public override string GetTypeNameFormatted ()
            {
            return xuk.XukStrings.JpgImageMediaData;
            }



        }
    }
