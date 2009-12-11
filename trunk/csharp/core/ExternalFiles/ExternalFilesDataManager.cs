using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace urakawa.ExternalFiles
    {
    public sealed class ExternalFilesDataManager:XukAbleManager<ExternalFileData>
        {
        public ExternalFilesDataManager ( Presentation pres )
            : base ( pres, "MEF" )
            {
            }
        public override string GetTypeNameFormatted ()
            {
            throw new NotImplementedException ();
            }

        public override bool CanAddManagedObject (ExternalFileData fileDataObject)
            {
            return true;
            }

        public void AddManagedObject ( ExternalFileData instance )
            {

            }
        }
    }
