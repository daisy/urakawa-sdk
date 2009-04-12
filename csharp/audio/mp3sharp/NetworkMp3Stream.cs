using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Mp3Sharp
{
    /// <summary>
    /// An Extension of the ExtendedMP3Stream to handle playing of a networked stream
    /// </summary>
	public class NetworkMp3Stream:ExtendedMp3Stream
	{
        /// <summary>
        /// Constructor - initialises the object and passes the stream to the base class
        /// </summary>
        /// <param name="stream"></param>
        public NetworkMp3Stream(NetworkStream stream)
            : base(stream)
        {
        }
        /// <summary>
        /// Used to detect whether there the stream has finished
        /// </summary>
        public override bool EndOfStream
        {
            get
            {
                bool atEndOfFile=true;
                NetworkStream stream = SourceStream as NetworkStream;
                if (stream != null)
                    atEndOfFile = stream.CanRead && !stream.DataAvailable;

                return atEndOfFile;
            }
        }
        /// <summary>
        /// The length of the stream - we can't do this with a network stream - we can only say it hasn't finished
        /// </summary>
        public override long Length
        {
            get
            {
                long length = 1;
                System.Net.Sockets.NetworkStream networkStream = this.SourceStream as System.Net.Sockets.NetworkStream;
                if (networkStream == null)
                    length = SourceStream.Length;
                return length;
            }
        }
        /// <summary>
        /// The current position of the stream - we can't do this with a network stream - we can only say we haven't reached
        /// the end
        /// </summary>
        public override long Position
        {
            get
            {
                long position = 0;
                System.Net.Sockets.NetworkStream nwstream = SourceStream as System.Net.Sockets.NetworkStream;
                if (nwstream == null)
                    position = SourceStream.Position;
                else if (!nwstream.DataAvailable)
                    position = 1;
                return position;
            }
            set 
            { 
                Exception e=new Exception("Unable to set the position of a NetworkStream object");
                throw e;
            }
        }
	}
}
