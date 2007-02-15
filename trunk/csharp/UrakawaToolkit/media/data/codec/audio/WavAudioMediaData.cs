using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using urakawa.media.data;
using urakawa.media.timing;

namespace urakawa.media.data.codec.audio
{
	/// <summary>
	/// Implementation of <see cref="AudioMediaData"/> that supports sequences of RIFF WAVE PCM audio data clips
	/// </summary>
	public class WavAudioMediaData : AudioMediaData
	{
		/// <summary>
		/// Represents a RIFF WAVE PCM audio data clip
		/// </summary>
		protected class WavClip
		{
			/// <summary>
			/// Constructor setting the <see cref="IDataProvider"/>, 
			/// clip begin and clip end will in this case be initialized to <c>null</c>,
			/// which means beginning/end if the RIFF WAVE PCM data
			/// </summary>
			/// <param name="clipDataProvider">The <see cref="IDataProvider"/></param>
			public WavClip(IDataProvider clipDataProvider)
			{
				mDataProvider = clipDataProvider;
			}
			/// <summary>
			/// Constructor setting the <see cref="IDataProvider"/> and clip begin/end values
			/// </summary>
			/// <param name="clipDataProvider">The <see cref="IDataProvider"/></param>
			/// <param name="clipBegin">The clip begin <see cref="Time"/></param>
			/// <param name="clipEnd">The clip end <see cref="Time"/></param>
			public WavClip(IDataProvider clipDataProvider, Time clipBegin, Time clipEnd)
				: this(clipDataProvider)
			{
				setClipBegin(clipBegin);
				setClipEnd(clipEnd);
			}
			private Time mClipBegin;
			/// <summary>
			/// Gets the clip begin <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <returns>The clip begin <see cref="Time"/></returns>
			public Time getClipBegin()
			{
				return mClipBegin;
			}
			/// <summary>
			/// Sets the clip begin <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <param name="newClipBegin">The new clip begin <see cref="Time"/></param>
			public void setClipBegin(Time newClipBegin)
			{
				if (newClipBegin != null && getClipEnd() != null)
				{
					if (newClipBegin.isGreaterThan(getClipEnd()))
					{
						throw new exception.MethodParameterIsOutOfBoundsException(
							"The new clip begin is beyond the current clip end");
					}
				}
				mClipBegin = newClipBegin;
			}
			private Time mClipEnd;
			/// <summary>
			/// Gets the clip end <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <returns>The clip end <see cref="Time"/></returns>
			public Time getClipEnd()
			{
				return mClipEnd;
			}
			/// <summary>
			/// Sets the clip end <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <param name="newClipEnd">The new clip end <see cref="Time"/></param>
			public void setClipEnd(Time newClipEnd)
			{
				if (newClipEnd != null && getClipBegin() != null)
				{
					if (newClipEnd.isLessThan(getClipBegin()))
					{
						throw new exception.MethodParameterIsOutOfBoundsException(
							"The new clip end time is before current clip begin");
					}
				}
				mClipEnd = newClipEnd;
			}
			private IDataProvider mDataProvider;
			/// <summary>
			/// Gets the <see cref="IDataProvider"/> storing the RIFF WAVE PCM audio data of <c>this</c>
			/// </summary>
			/// <returns>The <see cref="IDataProvider"/></returns>
			public IDataProvider getDataProvider()
			{
				return mDataProvider;
			}
			/// <summary>
			/// Gets the duration of audio that <c>this</c> is representing
			/// </summary>
			/// <returns>The duration of as a <see cref="TimeDelta"/></returns>
			public TimeDelta getAudioDuration()
			{
				Time clipEnd = getClipEnd();
				if (clipEnd == null)
				{
					PCMDataInfo pcmInfo;
					parseWavData(getDataProvider(), out pcmInfo);
					clipEnd = new Time(pcmInfo.getDuration());
				}
				Time clipBegin = getClipBegin();
				if (clipBegin == null) clipBegin = new Time();
				return getClipEnd().getTimeDelta(getClipBegin());
			}

			/// <summary>
			/// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
			/// </summary>
			/// <returns>The <see cref="Stream"/></returns>
			public Stream getAudioData()
			{
				Stream raw = getDataProvider().getInputStream();
				PCMDataInfo pcmInfo;
				parseWavData(raw, out pcmInfo);
				Time rawEndTime = new Time(pcmInfo.getDuration());
				Time clipBegin = getClipBegin();
				if (clipBegin==null) clipBegin = new Time();
				Time clipEnd = getClipEnd();
				if (clipEnd==null) clipEnd = new Time(pcmInfo.getDuration());
				if (clipBegin.isGreaterThan(clipEnd))
				{
					throw new exception.InvalidDataFormatException(
						"Clip begin of the WavClip is beyond the clip end of the underlying RIFF WAVE PCM data");
				}
				if (clipEnd.isGreaterThan(rawEndTime))
				{
					throw new exception.InvalidDataFormatException(
						"Clip end of the WavClip is beyond the end of the underlying RIFF WAVE PCM data"); 
				}
				utillities.SubStream res = new utillities.SubStream(
					raw, 
					raw.Position+(long)(clipBegin.getTimeAsMillisecondFloat()*pcmInfo.ByteRate), 
					pcmInfo.DataLength);
				return res;
			}
		}

		/// <summary>
		/// Stores the <see cref="WavClip"/>s of <c>this</c>
		/// </summary>
		private List<WavClip> mWavClips = new List<WavClip>();

		/// <summary>
		/// Parses a RIFF WAVE PCM header of a given input <see cref="Stream"/>
		/// </summary>
		/// <param name="input">The input <see cref="Stream"/> - must be positioned at the start of the RIFF chunk</param>
		/// <param name="pcmInfo">A <see cref="AudioMediaData.PCMDataInfo"/> in which to return the parsed data</param>
		protected static void parseWavData(IDataProvider input, out PCMDataInfo pcmInfo)
		{
			Stream inputStream = input.getInputStream();
			parseWavData(inputStream, out pcmInfo);
			inputStream.Close();
		}

		/// <summary>
		/// Parses a RIFF WAVE PCM header of a given input <see cref="Stream"/>
		/// </summary>
		/// <remarks>
		/// Upon succesful parsing the <paramref name="input"/> <see cref="Stream"/> is positioned at the beginning of the actual PCM data,
		/// that is at the beginning of the data field of the data sub-chunk
		/// </remarks>
		/// <param name="input">The input <see cref="Stream"/> - must be positioned at the start of the RIFF chunk</param>
		/// <param name="pcmInfo">A <see cref="AudioMediaData.PCMDataInfo"/> in which to return the parsed data</param>
		/// <exception cref="exception.InvalidDataFormatException">
		/// Thrown when RIFF WAVE header is invalid or is not PCM data
		/// </exception>
		protected static void parseWavData(Stream input, out PCMDataInfo pcmInfo)
		{
			BinaryReader rd = new BinaryReader(input);
			if (input.Length-input.Position<12)
			{
				throw new exception.InvalidDataFormatException("The RIFF chunk descriptor does not fit in the input stream");
			}
			string chunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
			if (chunkId != "RIFF")
			{
				throw new exception.InvalidDataFormatException("ChunkId is not RIFF");
			}
			uint chunkSize = rd.ReadUInt32();
			long chunkEndPos = input.Position + chunkSize;
			if (chunkEndPos > input.Length)
			{
				throw new exception.InvalidDataFormatException(String.Format(
					"The WAVE PCM chunk does not fit in the input Stream (expected chunk end position is {0:0}, Stream length is {1:0})",
					chunkEndPos, input.Length));
			}
			string format = Encoding.ASCII.GetString(rd.ReadBytes(4));
			if (format != "WAVE")
			{
				throw new exception.InvalidDataFormatException(String.Format(
					"RIFF format {0} is not supported. The only supported RIFF format is WAVE",
					format));
			}
			bool foundFormatSubChunk = false;
			pcmInfo = new PCMDataInfo();
			// Search for format subchunk
			while (input.Position+8 < chunkEndPos)
			{
				string formatSubChunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
				uint formatSubChunkSize = rd.ReadUInt32();
				if (input.Position + formatSubChunkSize > chunkEndPos)
				{
					throw new exception.InvalidDataFormatException(String.Format(
						"ChunkId {0} does not fit in RIFF chunk",
						formatSubChunkId));
				}
				if (formatSubChunkId == "fmt ")
				{
					foundFormatSubChunk = true;
					if (formatSubChunkSize < 2)
					{
						throw new exception.InvalidDataFormatException("No room for AudioFormat field in format sub-chunk");
					}
					ushort audioFormat = rd.ReadUInt16();
					if (audioFormat != 1)
					{
						throw new exception.InvalidDataFormatException(String.Format(
							"AudioFormat is not PCM (AudioFormat is {0:0})",
							audioFormat));
					}
					if (formatSubChunkSize != 16)
					{
						throw new exception.InvalidDataFormatException(String.Format(
							"Invalid format sub-chink size {0:0} for PCM - must be 16 bytes"));
					}
					ushort numChannels = rd.ReadUInt16();
					if (numChannels == 0)
					{
						throw new exception.InvalidDataFormatException("0 channels of audio is not supported");
					}
					uint sampleRate = rd.ReadUInt32();
					uint byteRate = rd.ReadUInt32();
					ushort blockAlign = rd.ReadUInt16();
					ushort bitsPerSample = rd.ReadUInt16();
					if ((bitsPerSample % 8) != 0)
					{
						throw new exception.InvalidDataFormatException(String.Format(
							"Invalid number of bits per sample {0:0} - must be a mulitpla of 8",
							bitsPerSample));
					}
					if (blockAlign!=(numChannels*bitsPerSample/8))
					{
						throw new exception.InvalidDataFormatException(String.Format(
							"Invalid block align {0:0} - expected {1:0}",
							blockAlign, numChannels*bitsPerSample/8));
					}
					if (byteRate!=sampleRate*blockAlign)
					{
						throw new exception.InvalidDataFormatException(String.Format(
							"Invalid byte rate {0:0} - expected {1:0}",
							byteRate, sampleRate*blockAlign));
					}
					pcmInfo.BitDepth = bitsPerSample;
					pcmInfo.NumberOfChannels = numChannels;
					pcmInfo.SampleRate = sampleRate;
				}
				else
				{
					input.Seek(formatSubChunkSize, SeekOrigin.Current);
				}
			}
			if (!foundFormatSubChunk)
			{
				throw new exception.InvalidDataFormatException("Found no format sub-chunk");
			}
			bool foundDataSubChunk = false;
			while (input.Position + 8 < chunkEndPos)
			{
				string dataSubChunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
				uint dataSubChunkSize = rd.ReadUInt32();
				if (input.Position + dataSubChunkSize > chunkEndPos)
				{
					throw new exception.InvalidDataFormatException(String.Format(
						"ChunkId {0} does not fit in RIFF chunk",
						dataSubChunkId));
				}
				if (dataSubChunkId == "data")
				{
					foundDataSubChunk = true;
					pcmInfo.DataLength = dataSubChunkSize;
				}
				else
				{
					input.Seek(dataSubChunkSize, SeekOrigin.Current);
				}
			}
			if (!foundDataSubChunk)
			{
				throw new exception.InvalidDataFormatException("Found no data sub-chunk");
			}
		}

		/// <summary>
		/// Constructor associating the newly constructed <see cref="WavAudioMediaData"/> 
		/// with a given <see cref="IMediaDataManager"/> 
		/// </summary>
		/// <param name="mngr"></param>
		protected internal WavAudioMediaData(IMediaDataManager mngr)
		{
			setMediaDataManager(mngr);
		}

		protected override IMediaData copyL()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of <c>this</c>, including copies of all <see cref="IDataProvider"/>s used by <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public WavAudioMediaData copy()
		{
			IMediaData oCopy = getMediaDataFactory().createMediaData(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is WavAudioMediaData))
			{
				throw new exception.FactoryCanNotCreateTypeException(
					"The MediaDataFactory can not create a WavAudioMediaData");
			}
			WavAudioMediaData copy = (WavAudioMediaData)oCopy;
			foreach (WavClip clip in mWavClips)
			{
				copy.mWavClips.Add(new WavClip(
					clip.getDataProvider().copy(),
					clip.getClipBegin() == null ? null : clip.getClipBegin().copy(),
					clip.getClipEnd() == null ? null : clip.getClipEnd().copy()));
			}
			return copy;
		}

		
		/// <summary>
		/// Reads the <see cref="WavAudioMediaData"/> from a WavAudioMediaData xuk element
		/// </summary>
		/// <param localName="source">The source <see cref="System.Xml.XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public override bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		
		/// <summary>
		/// Write a WavAudioMediaData element to a XUK file representing the <see cref="WavAudioMediaData"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public override bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="ITime"/>s
		/// as raw PCM data
		/// </summary>
		/// <param name="clipBegin">The given clip begin <see cref="ITime"/></param>
		/// <param name="clipEnd">The given clip end <see cref="ITime"/></param>
		/// <returns>The <see cref="Stream"/></returns>
		public override Stream getAudioData(ITime clipBegin, ITime clipEnd)
		{
			if (clipBegin.isLessThan(new Time()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The clip begin value can not be a negative time");
			}
			if (clipEnd.isLessThan(clipBegin))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The clip end can not be before clip begin");
			}
			int startIndex = 0;
			Time timeBeforeStartIndexClip = new Time();
			int endIndex = mWavClips.Count - 1;
			Time timeBeforeEndIndexClip = new Time();
			Time elapsedTime = new Time();
			int i = 0;
			while (i < mWavClips.Count)
			{
				TimeDelta currentClipDuration = mWavClips[i].getAudioDuration();
				Time newElapsedTime = elapsedTime;
				newElapsedTime.addTimeDelta(currentClipDuration);
				//if (clipBegin.getTimeDelta(elapsedTime)<
				//elapsedTime.Add(mWavClips[i].getAudioDuration().getTimeDeltaAsTimeSpan());
				i++;
			}


			throw new Exception("The method or operation is not implemented.");
		}

		public override void appendAudioData(Stream pcmData, ITimeDelta duration)
		{
			//TODO: Implement method
		}

		public override void insertAudioData(Stream pcmData, ITime insertPoint, ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override void replaceAudioData(Stream pcmData, ITime replacePoint, ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		/// <summary>
		/// Gets the intrinsic duration of the audio data
		/// </summary>
		/// <returns>The duration as an <see cref="ITimeDelta"/></returns>
		public override ITimeDelta getAudioDuration()
		{
			TimeDelta dur = new TimeDelta();
			foreach (WavClip clip in mWavClips)
			{
				dur.addTimeDelta(clip.getAudioDuration());
			}
			return dur;
		}

		public override void delete()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets a <see cref="IList{IDataProvider}"/> of the <see cref="IDataProvider"/>s
		/// used to store the Wav audio data
		/// </summary>
		/// <returns>The <see cref="List{IDataProvider}"/></returns>
		protected override IList<IDataProvider> getUsedDataProviders()
		{
			List<IDataProvider> usedDP = new List<IDataProvider>(mWavClips.Count);
			foreach (WavClip clip in mWavClips)
			{
				usedDP.Add(clip.getDataProvider());
			}
			return usedDP;
		}

		public override void removeAudio(ITime clipBegin)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void removeAudio(ITime clipBegin, ITime clipEnd)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool ValueEquals(IMediaData other)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
