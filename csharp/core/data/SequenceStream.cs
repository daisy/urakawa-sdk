using System;
using System.Collections.Generic;
using System.IO;

namespace urakawa.data
{
    public sealed class LightLinkedList<T> where T : class //, new() NO NEED FOR PUBLIC DEFAULT CONSTRUCTOR
    {
        public Item m_First;

        public class Item
        {
            public T m_data;
            public Item m_previousItem;
            public Item m_nextItem;
        }

        public void AddRange(LightLinkedList<T> list)
        {
            Item current = list.m_First;
            while (current != null)
            {
                T o = current.m_data;
                Add(o);

                current = current.m_nextItem;
            }
        }

        public void Add(T data)
        {
            if (m_First == null)
            {
                m_size = 1;
                m_First = new Item();
                m_First.m_data = data;
                m_First.m_nextItem = null;
                m_First.m_previousItem = null;
                return;
            }

            Item last = m_First;
            while (last.m_nextItem != null)
            {
                last = last.m_nextItem;
            }

            m_size++;
            last.m_nextItem = new Item();
            last.m_nextItem.m_data = data;
            last.m_nextItem.m_nextItem = null;
            last.m_nextItem.m_previousItem = last;
        }

        public void Clear()
        {
            if (m_First == null) return;

            Item current = m_First;
            do
            {
                //current.m_data = default(T);
                current.m_data = null;

                current = current.m_nextItem;
                if (current != null)
                {
                    current.m_nextItem = null;
                    current.m_previousItem = null;
                }
            } while (current != null);

            m_size = 0;
            m_First = null;
        }

        private int m_size = 0;
        public int Count
        {
            get { return m_size; }
        }

        public bool IsEmpty
        {
            get
            {
                return m_First == null;
            }
        }
    }

    /// <summary>
    /// A <see cref="Stream"/> that supports reading from a sequence of source <see cref="Stream"/>s
    /// as if they were one.
    /// </summary>
    public class SequenceStream : Stream
    {
        private
#if USE_NORMAL_LIST
            List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<Stream> mSources;

#if USE_NORMAL_LIST
        private int mCurrentIndex;
#else
        private LightLinkedList<Stream>.Item mCurrentStreamItem;
#endif //USE_NORMAL_LIST



        //public IEnumerable<Stream> ChildStreams
        //{
        //    get
        //    {
        //        foreach (Stream stream in mSources)
        //        {
        //            yield return stream;
        //        }
        //    }
        //}


        /// <summary>
        /// Constructor supplying the sequence of source <see cref="Stream"/>s
        /// </summary>
        /// <param name="ss">
        /// The sequence of source <see cref="Stream"/>s. 
        /// Must contain at least one source <see cref="Stream"/>
        /// </param>
        public SequenceStream(
#if USE_NORMAL_LIST
IEnumerable
#else
LightLinkedList
#endif //USE_NORMAL_LIST
<Stream> ss
            )
        {
            mSources = new
#if USE_NORMAL_LIST
List<Stream>(ss);
#else
 LightLinkedList<Stream>();
#endif //USE_NORMAL_LIST

#if USE_NORMAL_LIST
            if (mSources.Count == 0)
            {
                throw new exception.MethodParameterHasNoItemsException(
                    "A SequenceStream must have at least one source Stream in it's sequence");
            }
            mCurrentIndex = 0;
            mSources[mCurrentIndex].Seek(0, SeekOrigin.Begin);
#else
            LightLinkedList<Stream>.Item current = ss.m_First;
            while (current != null)
            {
                Stream subS = current.m_data;
                mSources.Add(subS);

                current = current.m_nextItem;
            }

            if (mSources.IsEmpty)
            {
                mSources = null;
                throw new exception.MethodParameterHasNoItemsException(
                    "A SequenceStream must have at least one source Stream in it's sequence");
            }
            mCurrentStreamItem = mSources.m_First;
            mCurrentStreamItem.m_data.Seek(0, SeekOrigin.Begin);
#endif //USE_NORMAL_LIST
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading. 
        /// </summary>
        public override bool CanRead
        {
            get
            {
#if USE_NORMAL_LIST
                foreach (Stream subS in mSources)
                {
                    if (!subS.CanRead)
                    {
                        return false;
                    }
                }
                return true;
#else
                LightLinkedList<Stream>.Item current = mSources.m_First;
                while (current != null)
                {
                    Stream subS = current.m_data;
                    if (!subS.CanRead)
                    {
                        return false;
                    }
                    current = current.m_nextItem;
                }
                return true;
#endif //USE_NORMAL_LIST
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking. 
        /// </summary>
        public override bool CanSeek
        {
            get
            {
#if USE_NORMAL_LIST
                foreach (Stream subS in mSources)
                {
                    if (!subS.CanSeek)
                    {
                        return false;
                    }
                }
                return true;
#else
                LightLinkedList<Stream>.Item current = mSources.m_First;
                while (current != null)
                {
                    Stream subS = current.m_data;
                    if (!subS.CanSeek)
                    {
                        return false;
                    }
                    current = current.m_nextItem;
                }
                return true;
#endif //USE_NORMAL_LIST
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing - always returns <c>false</c>
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Does nothing since a <see cref="SubStream"/> is read-only
        /// </summary>
        public override void Flush()
        {
            //Does nothing since there is no writing
        }

        /// <summary>
        /// Gets the count in <see cref="byte"/>s of the current <see cref="SequenceStream"/>
        /// </summary>
        public override long Length
        {
            get
            {
#if USE_NORMAL_LIST
                long len = 0;
                foreach (Stream subS in mSources)
                {
                    len += subS.Length;
                }
                return len;
#else
                long len = 0;
                LightLinkedList<Stream>.Item current = mSources.m_First;
                while (current != null)
                {
                    Stream subS = current.m_data;
                    len += subS.Length;
                    current = current.m_nextItem;
                }
                return len;
#endif //USE_NORMAL_LIST
            }
        }

        /// <summary>
        /// Gets or sets the position within the current <see cref="SequenceStream"/>. 
        /// </summary>
        public override long Position
        {
            get
            {

#if USE_NORMAL_LIST
                return GetBytesBeforeIndex(mCurrentIndex) + mSources[mCurrentIndex].Position;
#else
                return GetBytesBeforeStream(mCurrentStreamItem) + mCurrentStreamItem.m_data.Position;
#endif //USE_NORMAL_LIST
            }
            set
            {
#if USE_NORMAL_LIST
                mCurrentIndex = 0;
                long bytesBefore = 0;
                while (mCurrentIndex < mSources.Count)
                {
                    long length = mSources[mCurrentIndex].Length;
                    if (value < bytesBefore + length)
                    {
                        mSources[mCurrentIndex].Position = value - bytesBefore;
                        return;
                    }
                    bytesBefore += length;
                    mCurrentIndex++;
                }
                mCurrentIndex = mSources.Count - 1;
                mSources[mCurrentIndex].Position = mSources[mCurrentIndex].Length;
#else
                mCurrentStreamItem = mSources.m_First;
                long bytesBefore = 0;

                LightLinkedList<Stream>.Item current = mSources.m_First;
                while (current != null)
                {
                    mCurrentStreamItem = current;

                    long length = mCurrentStreamItem.m_data.Length;

                    if (value < bytesBefore + length)
                    {
                        mCurrentStreamItem.m_data.Position = value - bytesBefore;
                        return;
                    }

                    bytesBefore += length;

                    current = current.m_nextItem;
                }

                mCurrentStreamItem.m_data.Position = mCurrentStreamItem.m_data.Length;
#endif //USE_NORMAL_LIST
            }
        }


#if USE_NORMAL_LIST
        private long GetBytesBeforeIndex(int index)
        {
            int i = 0;
            if (index >= mSources.Count) index = mSources.Count - 1;
            long bytesBefore = 0;
            while (i < index)
            {
                bytesBefore += mSources[i].Length;
                i++;
            }
            return bytesBefore;
        }
#else
        private long GetBytesBeforeStream(LightLinkedList<Stream>.Item streamItem)
        {
            long bytesBefore = 0;

            LightLinkedList<Stream>.Item current = mSources.m_First;
            while (current != null && current != streamItem)
            {
                Stream subS = current.m_data;
                bytesBefore += subS.Length;

                current = current.m_nextItem;
            }

            return bytesBefore;
        }
#endif //USE_NORMAL_LIST


        /// <summary>
        /// Reads a sequence of bytes from the current <see cref="SequenceStream"/> and 
        /// advances the position within the stream by the number of bytes read
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, 
        /// the buffer contains the specified byte array with the values between 
        /// <c><paramref name="offset"/></c> and <c>(<paramref name="offset"/> + <paramref name="count"/> - 1)</c> 
        /// replaced by the bytes read from the current <see cref="SequenceStream"/>.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> 
        /// at which to begin storing the data read from the current <see cref="SequenceStream"/>.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current <see cref="SequenceStream"/>.</param>
        /// <returns>The number of <see cref="byte"/>s read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count == 0) return 0;

#if USE_NORMAL_LIST
            int totalBytesRead = 0;
            int bytesRead = 0;
            while (true)
            {
                if (mSources[mCurrentIndex].Position < mSources[mCurrentIndex].Length)
                {
                    bytesRead = mSources[mCurrentIndex].Read(buffer, offset, count);
                }
                else
                {
                    bytesRead = 0;
                }
                totalBytesRead += bytesRead;
                count -= bytesRead;
                offset += bytesRead;
                if (count == 0) break;
                if (mCurrentIndex + 1 < mSources.Count)
                {
                    mCurrentIndex++;
                    mSources[mCurrentIndex].Position = 0;
                }
                else
                {
                    break;
                }
            }
            return totalBytesRead;
#else
            int totalBytesRead = 0;
            int bytesRead = 0;
            while (true)
            {
                if (mCurrentStreamItem.m_data.Position < mCurrentStreamItem.m_data.Length)
                {
                    bytesRead = mCurrentStreamItem.m_data.Read(buffer, offset, count);
                }
                else
                {
                    bytesRead = 0;
                }
                totalBytesRead += bytesRead;
                count -= bytesRead;
                offset += bytesRead;
                if (count == 0) break;
                if (mCurrentStreamItem.m_nextItem != null)
                {
                    mCurrentStreamItem = mCurrentStreamItem.m_nextItem;
                    mCurrentStreamItem.m_data.Position = 0;
                }
                else
                {
                    break;
                }
            }
            return totalBytesRead;
#endif //USE_NORMAL_LIST
        }


        /// <summary>
        /// Sets the <see cref="Position"/> within the current stream.
        /// </summary>
        /// <param name="offset">
        /// A byte <paramref name="offset"/> relative to the origin parameter.
        /// </param>
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.
        /// </param>
        /// <returns>The new <see cref="Position"/> within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPos = Position;
            switch (origin)
            {
                case SeekOrigin.Current:
                    newPos += offset;
                    break;
                case SeekOrigin.Begin:
                    newPos = offset;
                    break;
                case SeekOrigin.End:
                    newPos = Length + offset;
                    break;
            }
            Position = newPos;
            return Position;
        }

        /// <summary>
        /// Sets the <see cref="Length"/> of the <see cref="SequenceStream"/>.
        /// Since a <see cref="SequenceStream"/> is read-only, 
        /// calling this method will thorw an <see cref="NotSupportedException"/>
        /// </summary>
        /// <param name="value">The new <see cref="Length"/> </param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("A SubStream is read-only so it's count can not be set");
        }

        /// <summary>
        /// 
        /// Since a <see cref="SequenceStream"/> is read only, 
        /// calling this method will thorw an <see cref="NotSupportedException"/>
        /// </summary>
        /// <param name="buffer">
        /// An array of <see cref="byte"/>s. 
        /// This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying <see cref="byte"/>s 
        /// to the current stream.
        /// </param>
        /// <param name="count">The number of <see cref="byte"/>s to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("A SubStream is read-only so it can not be written to");
        }

        /// <summary>
        /// Closes the <see cref="SequenceStream"/> including all it's source <see cref="Stream"/>s
        /// </summary>
        public override void Close()
        {
            base.Close();

#if USE_NORMAL_LIST
            foreach (Stream subS in mSources)
            {
                subS.Close();
            }
#else
            LightLinkedList<Stream>.Item current = mSources.m_First;
            while (current != null)
            {
                Stream subS = current.m_data;
                subS.Close();

                current = current.m_nextItem;
            }
#endif //USE_NORMAL_LIST
        }
    }
}