using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NLayer.Decoder
{
    class PushbackInputStream : Stream
    {
        Stream inputStream;
        Stack<byte> pushedBack;

        public PushbackInputStream(Stream inputStream, int bufferSize)
        {
            this.inputStream = inputStream;
            this.pushedBack = new Stack<byte>(bufferSize);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { return inputStream.Length; }
        }

        public override long Position
        {
            get
            {
                return inputStream.Position;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Unread(byte[] buffer, int offset, int count)
        {
            for (int n = count - 1; n >= 0; n--)
                pushedBack.Push(buffer[n + offset]);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while(pushedBack.Count > 0 && read < count)
            {
                buffer[read+offset] = pushedBack.Pop();
                read++;
            }
            if (read < count)
            {
                read += inputStream.Read(buffer, offset+read, count);
            }
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
