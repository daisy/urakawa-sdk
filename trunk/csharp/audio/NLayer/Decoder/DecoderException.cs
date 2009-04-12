using System;

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
