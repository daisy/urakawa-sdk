using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLayer.Decoder
{
    class DecoderException : Exception
    {
        public Errors Error { get; private set; }

        public DecoderException(Errors error)
            : base(error.ToString())
        {
            this.Error = error;
        }
    }
}
