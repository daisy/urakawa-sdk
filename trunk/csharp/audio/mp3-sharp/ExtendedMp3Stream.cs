using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Mp3Sharp
{
    /// <summary>
    /// An Extension to MP3Stream that allows additions to be made to the class without changing its code
    /// </summary>
	public class ExtendedMp3Stream:Mp3Stream
	{
        /// <summary>
        /// Class constructor - takes a stream and passes it to the base class
        /// </summary>
        /// <param name="stream"></param>
        public ExtendedMp3Stream(Stream stream)
            : base(stream)
        { }
        public ExtendedMp3Stream(string fileName,int chunkSize)
            : base(fileName,chunkSize)
        { }

        public ExtendedMp3Stream(Stream musicStream, int chunkSize)
            : base(musicStream, chunkSize)
        { }
        /// <summary>
        /// Used to detect if we're at the end of the stream.  This property is overridable
        /// </summary>
        public virtual bool EndOfStream
        {
            get
            {
                return this.Position == this.Length;
            }
        }
	}
}
