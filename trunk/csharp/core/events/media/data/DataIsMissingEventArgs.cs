using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.events.media.data
{
    public class DataIsMissingEventArgs : EventArgs
    {
        public DataIsMissingEventArgs(MediaData md, exception.DataMissingException ex)
        {
            Exception = ex;
            SourceMediaData = md;
        }
        public readonly exception.DataMissingException Exception;
        public readonly MediaData SourceMediaData;
    }
}
