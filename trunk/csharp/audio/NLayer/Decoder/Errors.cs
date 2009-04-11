using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLayer.Decoder
{
    enum Errors
    {	
        /// <summary>
        /// The first bitstream error code. 
        /// interface for other bitstream error codes.
        /// </summary>
	    BITSTREAM_ERROR = 0x100,

        	/**
	 * An undeterminable error occurred. 
	 */
	UNKNOWN_ERROR = BITSTREAM_ERROR + 0,
	
	/**
	 * The header describes an unknown sample rate.
	 */
	UNKNOWN_SAMPLE_RATE = BITSTREAM_ERROR + 1,

	/**
	 * A problem occurred reading from the stream.
	 */
	STREAM_ERROR = BITSTREAM_ERROR + 2,
	
	/**
	 * The end of the stream was reached prematurely. 
	 */
	UNEXPECTED_EOF = BITSTREAM_ERROR + 3,
	
	/**
	 * The end of the stream was reached. 
	 */
	STREAM_EOF = BITSTREAM_ERROR + 4,
	
	/**
	 * Frame data are missing. 
	 */
	INVALIDFRAME = BITSTREAM_ERROR + 5,

	/**
	 * 
	 */
	BITSTREAM_LAST = 0x1ff,

        /// <summary>
        /// The first decoder error code.
        /// </summary>
        DECODER_ERROR = 0x200,
	
    	/// <summary>
    	/// Layer not supported by the decoder.
    	/// </summary>
        UNSUPPORTED_LAYER = DECODER_ERROR + 1,

	    /// <summary>
        /// Illegal allocation in subband layer. Indicates a corrupt stream.
	    /// </summary>
        ILLEGAL_SUBBAND_ALLOCATION = DECODER_ERROR + 2,
    }
}
