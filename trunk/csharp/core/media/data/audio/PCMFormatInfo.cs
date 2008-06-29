using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.xuk;
using urakawa.media.timing;

namespace urakawa.media.data.audio
{
	/// <summary>
	/// Represents information describing a RAW PCM format
	/// </summary>
	public class PCMFormatInfo : XukAble, IValueEquatable<PCMFormatInfo>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public PCMFormatInfo() : this(1, 44100, 16)
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other">The PCMFormatInfo to copy</param>
		public PCMFormatInfo(PCMFormatInfo other) : this(other.NumberOfChannels, other.SampleRate, other.BitDepth)
		{
		}

		/// <summary>
		/// Constructor initializing the <see cref="PCMFormatInfo"/> with given number of channels, sample rate and bit depth value
		/// </summary>
		/// <param name="noc">The given number of channels value</param>
		/// <param name="sr">The given sample rate value</param>
		/// <param name="bd">The given bit depth value</param>
		public PCMFormatInfo(ushort noc, uint sr, ushort bd)
		{
			NumberOfChannels = noc;
			SampleRate = sr;
			BitDepth = bd;
		}

		/// <summary>
		/// Create a copy of the <see cref="PCMFormatInfo"/>
		/// </summary>
		/// <returns>The copy</returns>
		public PCMFormatInfo Copy()
		{
			return new PCMFormatInfo(this);
		}

		private ushort mNumberOfChannels = 1;

	    /// <summary>
	    /// Gets or sets the number of channels of audio
	    /// </summary>
	    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
	    /// Thrown when <paramref name="value"/> is less than <c>1</c>
	    /// </exception>
	    public virtual ushort NumberOfChannels
	    {
	        get { return mNumberOfChannels; }
	        set
	        {
	            if (value < 1)
	            {
	                throw new exception.MethodParameterIsOutOfBoundsException(
	                    "Minimum number of channels is 1");
	            }
	            mNumberOfChannels = value;
	        }
	    }

	    private uint mSampleRate = 44100;

	    /// <summary>
	    /// Gets the sample rate in Hz of the audio
	    /// </summary>
	    public virtual uint SampleRate
	    {
	        get { return mSampleRate; }
	        set
	        {
	            if (mSampleRate < 1)
	            {
	                throw new exception.MethodParameterIsOutOfBoundsException(
	                    "Sample rate must be positive");
	            }
	            mSampleRate = value;
	        }
	    }

	    private ushort mBitDepth = 16;

	    /// <summary>
	    /// Gets the depth in bits of the audio, ie. the size in bits of each sample of audio
	    /// </summary>
	    public virtual ushort BitDepth
	    {
	        get { return mBitDepth; }
	        set
	        {
	            if ((value%8) != 0)
	            {
	                throw new exception.MethodParameterIsOutOfBoundsException(
	                    "Bit depth must be a multiple of 8");
	            }
	            if (value < 8)
	            {
	                throw new exception.MethodParameterIsOutOfBoundsException(
	                    "Bit depth must be a least 8");
	            }
	            mBitDepth = value;
	        }
	    }


	    /// <summary>
	    /// Gets the byte rate of the raw PCM data
	    /// </summary>
	    public uint ByteRate
	    {
	        get { return NumberOfChannels*SampleRate*BitDepth/8U; }
	    }

	    /// <summary>
	    /// Gets the size in bytes of a single block (i.e. a sample from each channel)
	    /// </summary>
	    public ushort BlockAlign
	    {
	        get { return (ushort) (NumberOfChannels*(BitDepth/8)); }
	    }

	    /// <summary>
		/// Determines if the <see cref="PCMFormatInfo"/> is compatible with a given other <see cref="PCMDataInfo"/>
		/// </summary>
		/// <param name="pcmInfo">The other PCMDataInfo</param>
		/// <returns>A <see cref="bool"/> indicating the compatebility</returns>
		public bool IsCompatibleWith(PCMFormatInfo pcmInfo)
		{
			if (pcmInfo == null) return false;
			if (NumberOfChannels != pcmInfo.NumberOfChannels) return false;
			if (SampleRate != pcmInfo.SampleRate) return false;
			if (BitDepth != pcmInfo.BitDepth) return false;
			return true;
		}

		/// <summary>
		/// Gets the duration of PCM data in the format of a given length
		/// </summary>
		/// <param name="dataLen">The length</param>
		/// <returns>The duration</returns>
		public TimeDelta GetDuration(uint dataLen)
		{
			if (ByteRate == 0)
			{
				throw new exception.InvalidDataFormatException("The PCM data has byte rate 0");
			}
			double blockCount = dataLen / BlockAlign;
			return new TimeDelta(TimeSpan.FromTicks((long)(Math.Round(getTicksPerBlock() * blockCount))));
		}

		private double getTicksPerBlock()
		{
			return ((double)TimeSpan.TicksPerSecond) / SampleRate;
		}

		/// <summary>
		/// Gets the PCM data length corresponding to a given duration
		/// </summary>
		/// <param name="duration">The given duration</param>
		/// <returns>The PCM data length</returns>
		public uint GetDataLength(TimeDelta duration)
		{
			uint blockCount = (uint)Math.Round(((double)duration.getTimeDeltaAsTimeSpan().Ticks) / getTicksPerBlock());
			uint res = blockCount * BlockAlign;
			return res;
		}

		
		#region IXUKAble members


		/// <summary>
		/// Reads the attributes of a PCMFormatInfo xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string attr = source.GetAttribute("numberOfChannels");
			if (attr==null)
			{
				throw new exception.XukException("Attribute NumberOfChannels is missing");
			}
			ushort noc;
			if (!UInt16.TryParse(attr, out noc))
			{
				throw new exception.XukException(String.Format(
					"Attribute NumberOfChannels value {0} is not an unsigned short integer",
					attr));
			}
			NumberOfChannels = noc;
			uint sr;
			attr = source.GetAttribute("sampleRate");
			if (attr == null)
			{
				throw new exception.XukException("Attribute SampleRate is missing");
			}
			if (!UInt32.TryParse(attr, out sr))
			{
				throw new exception.XukException(String.Format(
					"Attribute SampleRate value {0} is not an unsigned integer",
					attr));

			}
			SampleRate = sr;
			ushort bd;
			attr = source.GetAttribute("bitDepth");
			if (attr == null)
			{
				throw new exception.XukException("Attribute BitDepth is missing");
			}
			if (!UInt16.TryParse(attr, out bd))
			{
				throw new exception.XukException(String.Format(
					"Attribute BitDepth value {0} is not an unsigned short integer",
					attr));

			}
			BitDepth = bd;
            base.xukInAttributes(source);
		}

		/// <summary>
		/// Writes the attributes of a PCMFormatInfo element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("numberOfChannels", NumberOfChannels.ToString());
			destination.WriteAttributeString("sampleRate", SampleRate.ToString());
			destination.WriteAttributeString("bitDepth", BitDepth.ToString());
            base.xukOutAttributes(destination, baseUri);
		}
		#endregion


		#region IValueEquatable<PCMFormatInfo> Members

		/// <summary>
		/// Determines if <c>this</c> has the same value as a given other <see cref="PCMFormatInfo"/>
		/// </summary>
		/// <param name="other">The given other PCMFormatInfo with which to compare</param>
		/// <returns>A <see cref="bool"/> indicating value equality</returns>
		public bool ValueEquals(PCMFormatInfo other)
		{
			if (other == null) return false;
			if (other.GetType() != GetType()) return false;
			if (!IsCompatibleWith(other)) return false;
			return true;
		}

		#endregion

		
	}

}
