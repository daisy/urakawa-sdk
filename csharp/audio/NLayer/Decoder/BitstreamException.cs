using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLayer.Decoder
{
    class BitstreamException : Exception
    {
        public Errors Error { get; private set; }

        public BitstreamException(Errors error)
            : this(error, null)
        {
        }

        public BitstreamException(Errors error, Exception e) :
            this(error.ToString(), e)
        {
            Error = error;
        }

        public BitstreamException(string message, Exception e) :
            base (message, e)
        {
        }
    }
}
