using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data.utilities
{
	/// <summary>
	/// Represents information describing a RAW PCM format
	/// </summary>
	public class PCMFormatInfo : IValueEquatable<PCMFormatInfo>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public PCMFormatInfo()
		{
			setNumberOfChannels(1);
			setSampleRate(44100);
			setBitDepth(16);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other">The PCMFormatInfo to copy</param>
		public PCMFormatInfo(PCMFormatInfo other)
		{
			setNumberOfChannels(other.getNumberOfChannels());
			setSampleRate(other.getSampleRate());
			setBitDepth(other.getBitDepth());
		}

		private ushort mNumberOfChannels = 1;

		/// <summary>
		/// Gets the number of channels of audio
		/// </summary>
		public ushort getNumberOfChannels()
		{
			return mNumberOfChannels;
		}

		/// <summary>
		/// Sets the number of channels of audio
		/// </summary>
		public virtual void setNumberOfChannels(ushort newValue)
		{
			if (newValue < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Minimum number of channels is 1");
			}
			mNumberOfChannels = newValue;
		}

		private uint mSampleRate = 44100;

		/// <summary>
		/// Gets the sample rate in Hz of the audio
		/// </summary>
		public uint getSampleRate()
		{
			return mSampleRate;
		}

		/// <summary>
		/// Sets the sample rate in Hz of the audio
		/// </summary>
		public virtual void setSampleRate(uint newValue)
		{
			if (mSampleRate < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Sample rate must be positive");
			}
		}

		private ushort mBitDepth = 16;
		/// <summary>
		/// Gets the depth in bits of the audio, ie. the size in bits of each sample of audio
		/// </summary>
		public ushort getBitDepth()
		{
			return mBitDepth;
		}

		/// <summary>
		/// Sets the depth in bits of the audio, ie. the size in bits of each sample of audio
		/// </summary>
		public virtual void setBitDepth(ushort newValue)
		{
			if ((newValue % 8) != 0)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Bit depth must be a multiple of 8");
			}
			if (newValue < 8)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Bit depth must be a least 8");
			}
			mBitDepth = newValue;
		}


		/// <summary>
		/// Gets the byte rate of the raw PCM data
		/// </summary>
		public uint getByteRate()
		{
			return getNumberOfChannels() * getSampleRate() * getBitDepth() / 8U;
		}

		/// <summary>
		/// Gets the size in bytes of a single block (i.e. a sample from each channel)
		/// </summary>
		public ushort getBlockAlign()
		{
			return (ushort)(getNumberOfChannels() * (getBitDepth() / 8));
		}

		/// <summary>
		/// Determines if the <see cref="PCMDataInfo"/> is compatible with a given other <see cref="PCMDataInfo"/>
		/// </summary>
		/// <param name="pcmInfo">The other PCMDataInfo</param>
		/// <returns>A <see cref="bool"/> indicating the compatebility</returns>
		public bool isCompatibleWith(PCMFormatInfo pcmInfo)
		{
			return ValueEquals(pcmInfo);
		}

		#region IValueEquatable<PCMFormatInfo> Members

		/// <summary>
		/// Determines if <c>this</c> has the same value as a given other <see cref="PCMFormatInfo"/>
		/// </summary>
		/// <param name="other">The given other PCMFormatInfo with which to compare</param>
		/// <returns>A <see cref="bool"/> indicating value equality</returns>
		public bool ValueEquals(PCMFormatInfo other)
		{
			if (other.GetType() != GetType()) return false;
			if (getNumberOfChannels() != other.getNumberOfChannels()) return false;
			if (getSampleRate() != other.getSampleRate()) return false;
			if (getBitDepth() != other.getBitDepth()) return false;
			return true;
		}

		#endregion
	}
}
