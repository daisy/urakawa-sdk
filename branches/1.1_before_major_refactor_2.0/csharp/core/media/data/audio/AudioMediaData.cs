using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.media.timing;
using urakawa.media.data.utilities;

namespace urakawa.media.data.audio
{
	/// <summary>
	/// Abstract base class for audio <see cref="MediaData"/>.
	/// Implements PCM format accessors (number of channels, bit depth, sample rate) 
	/// and leaves all other methods abstract
	/// </summary>
	public abstract class AudioMediaData : MediaData
	{
		#region Event related members
		/// <summary>
		/// Event fired after the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/> has changed
		/// </summary>
		public event EventHandler<events.media.data.audio.PCMFormatChangedEventArgs> pcmFormatChanged;
		/// <summary>
		/// Fires the <see cref="pcmFormatChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="AudioMediaData"/> whoose <see cref="PCMFormatInfo"/> has changed</param>
		/// <param name="newFormat">The new value</param>
		/// <param name="prevFormat">The value prior to the change</param>
		protected void notifyPCMFormatChanged(AudioMediaData source, PCMFormatInfo newFormat, PCMFormatInfo prevFormat)
		{
			EventHandler<events.media.data.audio.PCMFormatChangedEventArgs> d = pcmFormatChanged;
			if (d != null) d(this, new urakawa.events.media.data.audio.PCMFormatChangedEventArgs(source, newFormat, prevFormat));
		}

		void this_pcmFormatChanged(object sender, urakawa.events.media.data.audio.PCMFormatChangedEventArgs e)
		{
			notifyChanged(e);
		}
		/// <summary>
		/// Event fired after audio data has been inserted into the <see cref="AudioMediaData"/>
		/// </summary>
		public event EventHandler<events.media.data.audio.AudioDataInsertedEventArgs> audioDataInserted;
		/// <summary>
		/// Fires the <see cref="audioDataInserted"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="AudioMediaData"/> into which audio data was inserted</param>
		/// <param name="insertPoint">The insert point at which audio data was inserted</param>
		/// <param name="duration">The duration of the inserted audio data</param>
		protected void notifyAudioDataInserted(AudioMediaData source, Time insertPoint, TimeDelta duration)
		{
			EventHandler<events.media.data.audio.AudioDataInsertedEventArgs> d = audioDataInserted;
			if (d != null) d(this, new urakawa.events.media.data.audio.AudioDataInsertedEventArgs(source, insertPoint, duration));
		}

		void this_audioDataInserted(object sender, urakawa.events.media.data.audio.AudioDataInsertedEventArgs e)
		{
			notifyChanged(e);
		}

		/// <summary>
		/// Event fired after audio data has been removed from the <see cref="AudioMediaData"/>
		/// </summary>
		public event EventHandler<events.media.data.audio.AudioDataRemovedEventArgs> audioDataRemoved;
		/// <summary>
		/// Fires the <see cref="audioDataRemoved"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="AudioMediaData"/> from which audio data was removed</param>
		/// <param name="fromPoint">The point at which audio data was removed</param>
		/// <param name="duration">The duration of the removed audio data</param>
		protected void notifyAudioDataRemoved(AudioMediaData source, Time fromPoint, TimeDelta duration)
		{
			EventHandler<events.media.data.audio.AudioDataRemovedEventArgs> d = audioDataRemoved;
			if (d != null) d(this, new urakawa.events.media.data.audio.AudioDataRemovedEventArgs(source, fromPoint, duration));
		}

		void this_audioDataRemoved(object sender, urakawa.events.media.data.audio.AudioDataRemovedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion

		internal AudioMediaData()
		{
			this.pcmFormatChanged += new EventHandler<urakawa.events.media.data.audio.PCMFormatChangedEventArgs>(this_pcmFormatChanged);
			this.audioDataInserted += new EventHandler<urakawa.events.media.data.audio.AudioDataInsertedEventArgs>(this_audioDataInserted);
			this.audioDataRemoved += new EventHandler<urakawa.events.media.data.audio.AudioDataRemovedEventArgs>(this_audioDataRemoved);
		}

		private PCMFormatInfo mPCMFormat;

		/// <summary>
		/// Determines if a PCM Format change is ok
		/// </summary>
		/// <param name="newFormat">The new PCM Format value - assumed not to be <c>null</c></param>
		/// <param name="failReason">The <see cref="string"/> to which a failure reason must be written in case the change is not ok</param>
		/// <returns>A <see cref="bool"/> indicating if the change is ok</returns>
		protected virtual bool isPCMFormatChangeOk(PCMFormatInfo newFormat, out string failReason)
		{
			failReason = "";
			if (getMediaDataManager().getEnforceSinglePCMFormat())
			{
				if (!getMediaDataManager().getDefaultPCMFormat().valueEquals(newFormat))
				{
					failReason =
						"When the MediaDataManager enforces a single PCM Format, "
						+ "the PCM Format of the AudioMediaData must match the default defined by the manager";
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Gets the <see cref="MediaDataFactory"/>
		/// </summary>
		/// <returns></returns>
		protected MediaDataFactory getMediaDataFactory()
		{
			return getMediaDataManager().getMediaDataFactory();
		}

		/// <summary>
		/// Gets (a copy of) the <see cref="PCMFormatInfo"/> of the audio media data 
		/// </summary>
		/// <returns>The PCMFormatInfo</returns>
		public PCMFormatInfo getPCMFormat()
		{
			if (mPCMFormat == null)
			{
				mPCMFormat = new PCMFormatInfo(getMediaDataManager().getDefaultPCMFormat());
			}
			return mPCMFormat.copy();
		}

		/// <summary>
		/// Sets the PCMFormat of <c>this</c>
		/// </summary>
		/// <param name="newFormat">The new PCM format</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newFormat"/> is null
		/// </exception>
		/// <exception cref="exception.InvalidDataFormatException">
		/// Thrown when the <see cref="MediaDataManager"/> enforces a single PCM Format different from the new value
		/// or when audio data in a different format has already been added to the <see cref="AudioMediaData"/>
		/// </exception>
		public void setPCMFormat(PCMFormatInfo newFormat)
		{
			if (newFormat == null)
			{
				throw new exception.MethodParameterIsNullException("The new PCMFormatInfo can not be null");
			}
			string failReason;
			if (!isPCMFormatChangeOk(newFormat, out failReason))
			{
				throw new exception.InvalidDataFormatException(failReason);
			}
			if (!newFormat.valueEquals(mPCMFormat))
			{
				PCMFormatInfo prevFormat = mPCMFormat;
				mPCMFormat = newFormat.copy();
				notifyPCMFormatChanged(this, mPCMFormat.copy(), prevFormat);
			}
		}

		/// <summary>
		/// Sets the number of channels of the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/>
		/// </summary>
		/// <param name="numberOfChannels">The new number of channels</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="numberOfChannels"/> is less than <c>1</c>
		/// </exception>
		/// <exception cref="exception.InvalidDataFormatException">
		/// Thrown when the <see cref="MediaDataManager"/> enforces a single PCM Format with a different number of channels from the new value
		/// or when audio data with a different number of channels has already been added to the <see cref="AudioMediaData"/>
		/// </exception>
		public void setNumberOfChannels(ushort numberOfChannels)
		{
			PCMFormatInfo newFormat = getPCMFormat();
			newFormat.setNumberOfChannels(numberOfChannels);
			setPCMFormat(newFormat);
		}

		/// <summary>
		/// Sets the sample rate of the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/>
		/// </summary>
		/// <param name="sampleRate">The new  sample rate</param>
		/// <exception cref="exception.InvalidDataFormatException">
		/// Thrown when the <see cref="MediaDataManager"/> enforces a single PCM Format with a different sample rate from the new value
		/// or when audio data with a different sample rate has already been added to the <see cref="AudioMediaData"/>
		/// </exception>
		public void setSampleRate(uint sampleRate)
		{
			PCMFormatInfo newFormat = getPCMFormat();
			newFormat.setSampleRate(sampleRate);
			setPCMFormat(newFormat);
		}

		/// <summary>
		/// Sets the number of channels of the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/>
		/// </summary>
		/// <param name="bitDepth">The new bit depth</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="bitDepth"/> is less than <c>1</c>
		/// </exception>
		/// <exception cref="exception.InvalidDataFormatException">
		/// Thrown when the <see cref="MediaDataManager"/> enforces a single PCM Format with a different bit depth from the new value
		/// or when audio data with a different bit depth has already been added to the <see cref="AudioMediaData"/>
		/// </exception>
		public void setBitDepth(ushort bitDepth)
		{
			PCMFormatInfo newFormat = getPCMFormat();
			newFormat.setBitDepth(bitDepth);
			setPCMFormat(newFormat);
		}

		/// <summary>
		/// Gets the count in bytes of PCM data of the audio media data of a given duration
		/// </summary>
		/// <param name="duration">The duration</param>
		/// <returns>The count in bytes</returns>
		public int getPCMLength(TimeDelta duration)
		{
			return (int)getPCMFormat().getDataLength(duration);
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
		/// <remarks>
		/// Make sure to close any audio <see cref="Stream"/> returned by this method when no longer needed.
		/// Failing to do so may cause <see cref="exception.InputStreamsOpenException"/> when trying to added data to or delete
		/// the <see cref="IDataProvider"/>s used by the <see cref="AudioMediaData"/> instance
		/// </remarks>
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
		/// <remarks>
		/// Make sure to close any audio <see cref="Stream"/> returned by this method when no longer needed.
		/// Failing to do so may cause <see cref="exception.InputStreamsOpenException"/> when trying to added data to or delete
		/// the <see cref="IDataProvider"/>s used by the <see cref="AudioMediaData"/> instance
		/// </remarks>
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
			PCMDataInfo pcmInfo = PCMDataInfo.parseRiffWaveHeader(riffWaveStream);
			if (!pcmInfo.isCompatibleWith(getPCMFormat()))
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

		private Stream openFileStream(string path)
		{
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		/// <summary>
		/// Appends audio data from a RIFF Wave file
		/// </summary>
		/// <param name="path">The path of the RIFF Wave file</param>
		public void appendAudioDataFromRiffWave(string path)
		{
			Stream rwFS = openFileStream(path); 
			try
			{
				appendAudioDataFromRiffWave(rwFS);
			}
			finally
			{
				rwFS.Close();
			}
		}

        public abstract void AppendPcmData ( IDataProvider fileDataProvider );

        public abstract void AppendPcmData ( IDataProvider fileDataProvider, Time clipBegin, Time clipEnd );


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
			Stream rwFS = openFileStream(path); 
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
		public void replaceAudioData(Stream pcmData, Time replacePoint, TimeDelta duration)
		{
			removeAudioData(replacePoint, replacePoint.addTimeDelta(duration));
			insertAudioData(pcmData, replacePoint, duration);
		}

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
			Stream rwFS = openFileStream(path); 
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
		public virtual void removeAudioData(Time clipBegin)
		{
			removeAudioData(clipBegin, Time.Zero.addTimeDelta(getAudioDuration()));
		}

		/// <summary>
		/// Removes all audio between given clip begin and end <see cref="Time"/>
		/// </summary>
		/// <param name="clipBegin">The givne clip begin <see cref="Time"/></param>
		/// <param name="clipEnd">The givne clip end <see cref="Time"/></param>
		public abstract void removeAudioData(Time clipBegin, Time clipEnd);

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
		protected override MediaData protectedCopy()
		{
			return audioMediaDataCopy();
		}

		/// <summary>
		/// Gets a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public new AudioMediaData copy()
		{
			return audioMediaDataCopy();
		}

		/// <summary>
		/// Splits the audio media data at a given split point in time,
		/// <c>this</c> retaining the audio before the split point,
		/// creating a new <see cref="AudioMediaData"/> containing the audio after the split point
		/// </summary>
		/// <param name="splitPoint">The given split point</param>
		/// <returns>A audio media data containing the audio after the split point</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given split point is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given split point is negative or is beyond the duration of <c>this</c>
		/// </exception>
		public virtual AudioMediaData split(Time splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The split point can not be null");
			}
			if (splitPoint.isNegativeTimeOffset())
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split point can not be negative");
			}
			if (splitPoint.isGreaterThan(Time.Zero.addTimeDelta(getAudioDuration())))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split point can not be beyond the end of the AudioMediaData");
			}
			MediaData md = getMediaDataFactory().createMediaData(getXukLocalName(), getXukNamespaceUri());
			if (!(md is AudioMediaData))
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaDataFactory can not create a AudioMediaData matching Xuk QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			AudioMediaData secondPartAMD = (AudioMediaData)md;
			TimeDelta spDur = Time.Zero.addTimeDelta(getAudioDuration()).getTimeDelta(splitPoint);
			Stream secondPartAudioStream = getAudioData(splitPoint);
			try
			{
				secondPartAMD.appendAudioData(secondPartAudioStream, spDur);
			}
			finally
			{
				secondPartAudioStream.Close();
			}
			removeAudioData(splitPoint);
			return secondPartAMD;
		}

		/// <summary>
		/// Merges <c>this</c> with a given other <see cref="AudioMediaData"/>,
		/// appending the audio data of the other <see cref="AudioMediaData"/> to <c>this</c>,
		/// leaving the other <see cref="AudioMediaData"/> without audio data
		/// </summary>
		/// <param name="other">The given other AudioMediaData</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="other"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.InvalidDataFormatException">
		/// Thrown when the PCM format of <c>this</c> is not compatible with that of <paramref name="other"/>
		/// </exception>
		public virtual void mergeWith(AudioMediaData other)
		{
			if (other == null)
			{
				throw new exception.MethodParameterIsNullException("Can not merge with a null AudioMediaData");
			}
			if (!getPCMFormat().isCompatibleWith(other.getPCMFormat()))
			{
				throw new exception.InvalidDataFormatException(
					"Can not merge this with a AudioMediaData with incompatible audio data");
			}
			System.IO.Stream otherData = other.getAudioData();
			try
			{
				appendAudioData(otherData, other.getAudioDuration());
			}
			finally
			{
				otherData.Close();
			}
			other.removeAudioData(Time.Zero);
		}

		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>		
		public override bool valueEquals(MediaData other)
		{
			if (!base.valueEquals(other)) return false;
			AudioMediaData amdOther = (AudioMediaData)other;
			if (!getPCMFormat().valueEquals(amdOther.getPCMFormat())) return false;
			if (getPCMLength() != amdOther.getPCMLength()) return false;
			Stream thisData = getAudioData();
			try
			{
				Stream otherdata = amdOther.getAudioData();
				try
				{
					if (!PCMDataInfo.compareStreamData(thisData, otherdata, (int)thisData.Length)) return false;
				}
				finally
				{
					otherdata.Close();
				}
			}
			finally
			{
				thisData.Close();
			}
			return true;
		}

	}
}
