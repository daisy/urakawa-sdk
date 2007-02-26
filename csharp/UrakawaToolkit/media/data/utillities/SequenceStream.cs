using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data.utillities
{
	/// <summary>
	/// A <see cref="Stream"/> that supports reading from a sequence of source <see cref="Stream"/>s
	/// as if they were one.
	/// </summary>
	public class SequenceStream : Stream
	{
		private List<Stream> mSources;
		private int mCurrentIndex;

		/// <summary>
		/// Constructor supplying the sequence of source <see cref="Stream"/>s
		/// </summary>
		/// <param name="ss">
		/// The sequence of source <see cref="Stream"/>s. 
		/// Must contain at least one source <see cref="Stream"/>
		/// </param>
		public SequenceStream(IEnumerable<Stream> ss)
		{
			mSources = new List<Stream>(ss);
			if (mSources.Count == 0)
			{
				throw new exception.MethodParameterHasNoItemsException(
					"A SequenceStream must have at least one source Stream in it's sequence");
			}
			mCurrentIndex = 0;
			mSources[0].Seek(0, SeekOrigin.Begin);
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading. 
		/// </summary>
		public override bool CanRead
		{
			get
			{
				foreach (Stream subS in mSources)
				{
					if (!subS.CanRead)
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking. 
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				foreach (Stream subS in mSources)
				{
					if (!subS.CanSeek)
					{
						return false;
					}
				}
				return true;
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
		/// Gets the length in <see cref="byte"/>s of the current <see cref="SequenceStream"/>
		/// </summary>
		public override long Length
		{
			get
			{
				long len = 0;
				foreach (Stream subS in mSources)
				{
					len += subS.Length;
				}
				return len;
			}
		}

		/// <summary>
		/// Gets or sets the position within the current <see cref="SequenceStream"/>. 
		/// </summary>
		public override long Position
		{
			get
			{
				return getBytesBeforeIndex(mCurrentIndex) + mSources[mCurrentIndex].Position;
			}
			set
			{
				mCurrentIndex = 0;
				long bytesBefore = 0;
				while (mCurrentIndex < mSources.Count)
				{
					if (value<bytesBefore+mSources[mCurrentIndex].Length)
					{
						mSources[mCurrentIndex].Position = value-bytesBefore;
						return;
					}
					bytesBefore += mSources[mCurrentIndex].Length;
					mCurrentIndex++;
				}
				mCurrentIndex = mSources.Count-1;
				mSources[mCurrentIndex].Position = mSources[mCurrentIndex].Length;
			}
		}

		private long getBytesBeforeIndex(int index)
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
				if (mCurrentIndex+1 < mSources.Count)
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
			throw new NotSupportedException("A SubStream is read-only so it's length can not be set");
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
			foreach (Stream subS in mSources)
			{
				subS.Close();
			}
		}
	}
}
