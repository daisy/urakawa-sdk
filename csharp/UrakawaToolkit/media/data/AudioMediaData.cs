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
		/// Gets the count in bytes of PCM data of the audio media data of a given duration
		/// </summary>
		/// <param name="duration">The duration</param>
		/// <returns>The count in bytes</returns>
		public int getPCMLength(TimeDelta duration)
		{
			return (int)((duration.getTimeDeltaAsMillisecondFloat() * getByteRate()) / 1000);
		}

		/// <summary>
		/// Gets the count in bytes of the PCM data of the audio media data
		/// </summary>
		/// <returns>The count in bytes</returns>
		public int getPCMLength()
		{
			return getPCMLength(getAudioDuration());
		}


		/// <summary>
		/// Gets the intrinsic duration of the audio data
		/// </summary>
		/// <returns>The duration as an <see cref="TimeDelta"/></returns>
		public abstract TimeDelta getAudioDuration();


		/// <summary>
		/// Gets an input <see cref="Stream"/> giving access to all audio data as raw PCM
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public Stream getAudioData()
		{
			return getAudioData(Time.Zero);
		}

		/// <summary>
		/// Gets an input <see cref="Stream"/> giving access to the audio data after a given <see cref="Time"/> 
		/// as raw PCM
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public Stream getAudioData(Time clipBegin)
		{
			return getAudioData(clipBegin, Time.Zero.addTimeDelta(getAudioDuration()));
		}

		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="Time"/>s
		/// as raw PCM data
		/// </summary>
		/// <param name="clipBegin">The given clip begin <see cref="Time"/></param>
		/// <param name="clipEnd">The given clip end <see cref="Time"/></param>
		/// <returns>The <see cref="Stream"/></returns>
		public abstract Stream getAudioData(Time clipBegin, Time clipEnd);

		/// <summary>
		/// Appends audio of a given duration to <c>this</c>
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
		/// <param name="duration">The duration of the audio to add</param>
		public virtual void appendAudioData(Stream pcmData, TimeDelta duration)
		{
			insertAudioData(pcmData, new Time(getAudioDuration().getTimeDeltaAsMillisecondFloat()), duration);
		}

		private void parseRiffWaveStream(Stream riffWaveStream, out TimeDelta duration)
		{
			utilities.PCMDataInfo pcmInfo = utilities.PCMDataInfo.parseRiffWaveHeader(riffWaveStream);
			if (!pcmInfo.isCompatibleWith(this))
			{
				throw new exception.InvalidDataFormatException(
					String.Format("RIFF WAV file has incompatible PCM format"));
			}
			duration = new TimeDelta(pcmInfo.getDuration());
		}

		/// <summary>
		/// Appends audio data from a RIFF Wave file
		/// </summary>
		/// <param name="riffWaveStream">The RIFF Wave file</param>
		public void appendAudioDataFromRiffWave(Stream riffWaveStream)
		{
			TimeDelta duration;
			parseRiffWaveStream(riffWaveStream, out duration);
			appendAudioData(riffWaveStream, duration);
		}

		/// <summary>
		/// Appends audio data from a RIFF Wave file
		/// </summary>
		/// <param name="path">The path of the RIFF Wave file</param>
		public void appendAudioDataFromRiffWave(string path)
		{
			FileStream rwFS = new FileStream(path, FileMode.Open, FileAccess.Read);
			try
			{
				appendAudioDataFromRiffWave(rwFS);
			}
			finally
			{
				rwFS.Close();
			}
		}


		/// <summary>
		/// Inserts audio data of a given duration at a given insert point
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the audio data as RAW PCM</param>
		/// <param name="insertPoint">The insert point</param>
		/// <param name="duration">The duration</param>
		public abstract void insertAudioData(Stream pcmData, Time insertPoint, TimeDelta duration);

		/// <summary>
		/// Inserts audio data from a RIFF Wave file at a given insert point and of a given duration
		/// </summary>
		/// <param name="riffWaveStream">The RIFF Wave file</param>
		/// <param name="insertPoint">The insert point</param>
		/// <param name="duration">The duration</param>
		public void insertAudioDataFromRiffWave(Stream riffWaveStream, Time insertPoint, TimeDelta duration)
		{
			TimeDelta fileDuration;
			parseRiffWaveStream(riffWaveStream, out fileDuration);
			if (fileDuration.isLessThan(duration))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"Can not insert {0} of audio from RIFF Wave file since the file's duration is only {1}",
					duration, fileDuration));
			}
			insertAudioData(riffWaveStream, insertPoint, duration);
		}

		/// <summary>
		/// Inserts audio data from a RIFF Wave file at a given insert point and of a given duration
		/// </summary>
		/// <param name="path">The path of the RIFF Wave file</param>
		/// <param name="insertPoint">The insert point</param>
		/// <param name="duration">The duration</param>
		public void insertAudioDataFromRiffWave(string path, Time insertPoint, TimeDelta duration)
		{
			FileStream rwFS = new FileStream(path, FileMode.Open, FileAccess.Read);
			try
			{
				insertAudioDataFromRiffWave(rwFS, insertPoint, duration);
			}
			finally
			{
				rwFS.Close();
			}
		}

		/// <summary>
		/// Replaces with audio of a given duration at a given replace point
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
		/// <param name="replacePoint">The given replace point</param>
		/// <param name="duration">The duration of the audio to replace</param>
		public abstract void replaceAudioData(Stream pcmData, Time replacePoint, TimeDelta duration);

		/// <summary>
		/// Replaces with audio from a RIFF Wave file of a given duration at a given replace point
		/// </summary>
		/// <param name="riffWaveStream">The RIFF Wave file</param>
		/// <param name="replacePoint">The given replace point</param>
		/// <param name="duration">The duration of the audio to replace</param>
		public void replaceAudioDataFromRiffWave(Stream riffWaveStream, Time replacePoint, TimeDelta duration)
		{
			TimeDelta fileDuration;
			parseRiffWaveStream(riffWaveStream, out fileDuration);
			if (fileDuration.isLessThan(duration))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"Can not insert {0} of audio from RIFF Wave file since the file's duration is only {1}",
					duration, fileDuration));
			}
			replaceAudioData(riffWaveStream, replacePoint, duration);
		}

		/// <summary>
		/// Replaces with audio from a RIFF Wave file of a given duration at a given replace point
		/// </summary>
		/// <param name="path">The path of the RIFF Wave file</param>
		/// <param name="replacePoint">The given replace point</param>
		/// <param name="duration">The duration of the audio to replace</param>
		public void replaceAudioDataFromRiffWave(string path, Time replacePoint, TimeDelta duration)
		{
			FileStream rwFS = new FileStream(path, FileMode.Open, FileAccess.Read);
			try
			{
				replaceAudioDataFromRiffWave(rwFS, replacePoint, duration);
			}
			finally
			{
				rwFS.Close();
			}
		}
		
		/// <summary>
		/// Removes all audio after a given clip begin <see cref="Time"/>
		/// </summary>
		/// <param name="clipBegin">The clip begin</param>
		public void removeAudio(Time clipBegin)
		{
			removeAudio(clipBegin, Time.Zero.addTimeDelta(getAudioDuration()));
		}

		/// <summary>
		/// Removes all audio between given clip begin and end <see cref="Time"/>
		/// </summary>
		/// <param name="clipBegin">The givne clip begin <see cref="Time"/></param>
		/// <param name="clipEnd">The givne clip end <see cref="Time"/></param>
		public abstract void removeAudio(Time clipBegin, Time clipEnd);

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
		public new IAudioMediaData copy()
		{
			return audioMediaDataCopy();
		}

	}
}
