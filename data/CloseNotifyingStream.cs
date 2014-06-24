using System;
using System.IO;

namespace urakawa.data
{
    /// <summary>
    /// <see cref="Stream"/> wrapper that has an event <see cref="CloseNotifyingStream.StreamClosed"/>
    /// that is fired when the <see cref="CloseNotifyingStream"/> has closed.
    /// Method calls are delegated to the base <see cref="Stream"/> passed with the constructor.
    /// </summary>
    public class CloseNotifyingStream : Stream
    {
        /// <summary>
        /// Fires when the <see cref="CloseNotifyingStream"/> has closed
        /// </summary>
        public event EventHandler StreamClosed;

        private void FireStreamClosed()
        {
            EventHandler d = StreamClosed;
            if (d != null) d(this, EventArgs.Empty);
        }

        private Stream mBaseStream;

        /// <summary>
        /// Constructor specifying the base <see cref="Stream"/> to be wrapped
        /// </summary>
        /// <param name="baseStm">The base <see cref="Stream"/></param>
        public CloseNotifyingStream(Stream baseStm)
        {
            if (baseStm == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "The base Stream cannot be null");
            }
            mBaseStream = baseStm;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return mBaseStream.CanRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return mBaseStream.CanSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return mBaseStream.CanWrite; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device. 
        /// </summary>
        public override void Flush()
        {
            mBaseStream.Flush();
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return mBaseStream.Length; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get { return mBaseStream.Position; }
            set { mBaseStream.Position = value; }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream 
        /// and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. 
        /// When this method returns, the buffer contains the specified byte array 
        /// with the values between <c><paramref name="offset"/></c> and <c>(<paramref name="offset"/> + <paramref name="count"/> - 1)</c> 
        /// replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> 
        /// at which to begin storing the data read from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. 
        /// This can be less than the number of bytes requested if that many bytes are not currently available, 
        /// or zero (0) if the end of the stream has been reached. 
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return mBaseStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position. 
        /// </param>
        /// <returns>The new position within the current stream. </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return mBaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            mBaseStream.SetLength(value);
            ;
        }

        /// <summary>
        /// writes a sequence of bytes to the current stream 
        /// and advances the current position within this stream by the number of bytes written. 
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. 
        /// This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream. 
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            mBaseStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Closes the current stream 
        /// and releases any resources (such as sockets and file handles) associated with the current stream. 
        /// </summary>
        public override void Close()
        {
            mBaseStream.Close();
            FireStreamClosed();
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte,
        /// or returns <c>-1</c> if at the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an <see cref="Int32"/>, or <c>-1</c> if at the end of the stream. </returns>
        public override int ReadByte()
        {
            return mBaseStream.ReadByte();
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte. 
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            mBaseStream.WriteByte(value);
        }
    }
}