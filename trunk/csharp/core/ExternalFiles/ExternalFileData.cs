using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using urakawa.data;

namespace urakawa.ExternalFiles
    {
    public abstract class ExternalFileData : WithPresentation
        {

        DataProvider m_DataProvider;
        //public ExternalFiles ()
            //{

            //}
        public override string GetTypeNameFormatted ()
            {
            throw new NotImplementedException ();
            }

        public IEnumerable<DataProvider> UsedDataProviders
            {
            get
                {
                yield return m_DataProvider;
                yield break;
                }
            }


        
        }
    }
