using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.media.timing;

namespace urakawa.media.data
{
	/// <summary>
	/// Abstract implementation of interface <see cref="IAudioMediaData"/>.
	/// Implements PCM format accessors (number of channels, bit depth, sample rate) 
	/// and leaves all other methods abstract
	/// </summary>
	public abstract class AudioMediaData : MediaData, IAudioMediaData
	{


		/// <summary>
		/// Gets the <see cref="IMediaDataFactory"/>
		/// </summary>
		/// <returns></returns>
		protected IMediaDataFactory getMediaDataFactory()
		{
			return getMediaDataManager().getMediaDataFactory();
		}

		#region IAudioMediaData Members

		private int mNumberOfChannels;

		/// <summary>
		/// Gets the number of channels of audio
		/// </summary>
		/// <returns>The number of channels</returns>
		public int getNumberOfChannels()
		{
			return mNumberOfChannels;
		}

		/// <summary>
		/// Sets the number of channels of audio
		/// </summary>
		/// <param name="newNumberOfChannels">The new number of channels</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new number of channels is not positive or in derived classes when otherwise out of bounds
		/// </exception>
		public virtual void setNumberOfChannels(int newNumberOfChannels)
		{
			if (newNumberOfChannels < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The number of channels must be positive");
			}
			mNumberOfChannels = newNumberOfChannels;
		}

		private int mBitDepth;

		/// <summary>
		/// Gets the number of bits used to store each sample of audio data
		/// </summary>
		/// <returns>The bit depth</returns>
		public int getBitDepth()
		{
			return mBitDepth;
		}

		/// <summary>
		/// Sets the number of bits used to store each sample of audio data
		/// </summary>
		/// <param name="newBitDepth">The new bit depth</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new bit depth is not positive or in derived classes when otherwise out of bounds
		/// </exception>
		public virtual void setBitDepth(int newBitDepth)
		{
			if (newBitDepth < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The bit depth must be positive");
			}
			mBitDepth = newBitDepth;
		}

		private int mSampleRate;

		/// <summary>
		/// Gets the sample rate of the audio data
		/// </summary>
		/// <returns>The sample rate in Hz</returns>
		public int getSampleRate()
		{
			return mSampleRate;
		}

		/// <summary>
		/// Sets the sample rate of the audio data
		/// </summary>
		/// <param name="newSampleRate">The new sample rate</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new sample rate is not positive or in derived classes when otherwise out of bounds
		/// </exception>
		public void setSampleRate(int newSampleRate)
		{
			if (newSampleRate < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException("The sample rate must be positive");
			}
			mSampleRate = newSampleRate;
		}

		/// <summary>
		/// Gets the byte rate of the audio media data
		/// </summary>
		/// <returns>The byte rate in bytes/sec</returns>
		public int getByteRate()
		{
			return (getBitDepth() * getNumberOfChannels() * getSampleRate()) / 8;
		}

		/// <summary>
		/// Gets the count in bytes of the PCM data of the audio media data
		/// </summary>
		/// <returns>The count in bytes</returns>
		public int getPCMLength()
		{
			return (int)((getAudioDuration().getTimeDeltaAsMillisecondFloat() * getByteRate()) / 1000);
		}


		/// <summary>
		/// Gets the intrinsic duration of the audio data
		/// </summary>
		/// <returns>The duration as an <see cref="ITimeDelta"/></returns>
		public abstract ITimeDelta getAudioDuration();


		/// <summary>
		/// Gets an input <see cref="Stream"/> giving access to all audio data as raw PCM
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public Stream getAudioData()
		{
			return getAudioData(Time.Zero);
		}

		/// <summary>
		/// Gets an input <see cref="Stream"/> giving access to the audio data after a given <see cref="ITime"/> 
		/// as raw PCM
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public Stream getAudioData(ITime clipBegin)
		{
			return getAudioData(clipBegin, Time.Zero.addTimeDelta(getAudioDuration()));
		}

		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="ITime"/>s
		/// as raw PCM data
		/// </summary>
		/// <param name="clipBegin">The given clip begin <see cref="ITime"/></param>
		/// <param name="clipEnd">The given clip end <see cref="ITime"/></param>
		/// <returns>The <see cref="Stream"/></returns>
		public abstract Stream getAudioData(ITime clipBegin, ITime clipEnd);

		/// <summary>
		/// Appends audio of a given duration to <c>this</c>
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
		/// <param name="duration">The duration of the audio to add</param>
		public virtual void appendAudioData(Stream pcmData, ITimeDelta duration)
		{
			insertAudioData(pcmData, new Time(getAudioDuration().getTimeDeltaAsMillisecondFloat()), duration);
		}

		/// <summary>
		/// Inserts audio data of a given duration at a given insert point
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the audio data as RAW PCM</param>
		/// <param name="insertPoint"></param>
		/// <param name="duration"></param>
		public abstract void insertAudioData(Stream pcmData, ITime insertPoint, ITimeDelta duration);

		/// <summary>
		/// Replaces audio with a given duration at a given replace point in <see cref="ITime"/>
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
		/// <param name="replacePoint">The given replkace point in <see cref="ITime"/></param>
		/// <param name="duration">The duration of the audio to replace</param>
		public abstract void replaceAudioData(Stream pcmData, ITime replacePoint, ITimeDelta duration);
		
		/// <summary>
		/// Removes all audio after a given clip begin <see cref="ITime"/>
		/// </summary>
		/// <param name="clipBegin">The clip begin</param>
		public void removeAudio(ITime clipBegin)
		{
			removeAudio(clipBegin, Time.Zero.addTimeDelta(getAudioDuration()));
		}

		/// <summary>
		/// Removes all audio between given clip begin and end <see cref="ITime"/>
		/// </summary>
		/// <param name="clipBegin">The givne clip begin <see cref="ITime"/></param>
		/// <param name="clipEnd">The givne clip end <see cref="ITime"/></param>
		public abstract void removeAudio(ITime clipBegin, ITime clipEnd);

		/// <summary>
		/// Part of technical solution to make copy method return correct type. 
		/// In implementing classes this method should return a copy of the class instances
		/// </summary>
		/// <returns>The copy</returns>
		protected abstract AudioMediaData audioMediaDataCopy();

		/// <summary>
		/// Part of technical solution to make copy method return correct type. 
		/// In implementing classes this method should return a copy of the class instances
		/// </summary>
		/// <returns>The copy</returns>
		protected override MediaData mediaDataCopy()
		{
			return audioMediaDataCopy();
		}

		/// <summary>
		/// Gets a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public IAudioMediaData copy()
		{
			return audioMediaDataCopy();
		}

		#endregion
	}
}
