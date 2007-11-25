using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.media.data;
using urakawa.media.timing;
using urakawa.media.data.utilities;

namespace urakawa.media.data.audio.codec
{
	/// <summary>
	/// Implementation of <see cref="AudioMediaData"/> that supports sequences of RIFF WAVE PCM audio data clips
	/// </summary>
	public class WavAudioMediaData : AudioMediaData
	{

		/// <summary>
		/// Represents a RIFF WAVE PCM audio data clip
		/// </summary>
		protected class WavClip : Clip, IValueEquatable<WavClip>
		{
			/// <summary>
			/// Constructor setting the <see cref="IDataProvider"/>, 
			/// clip begin and clip end will in this case be initialized to <c>null</c>,
			/// which means beginning/end if the RIFF WAVE PCM data
			/// </summary>
			/// <param name="clipDataProvider">The <see cref="IDataProvider"/></param>
			public WavClip(IDataProvider clipDataProvider) : this(clipDataProvider, new Time(), null)
			{
			}

			/// <summary>
			/// Constructor setting the <see cref="IDataProvider"/> and clip begin/end values
			/// </summary>
			/// <param name="clipDataProvider">The <see cref="IDataProvider"/> - can not be <c>null</c></param>
			/// <param name="clipBegin">The clip begin <see cref="Time"/> - can not be <c>null</c></param>
			/// <param name="clipEnd">
			/// The clip end <see cref="Time"/>
			/// - a <c>null</c> value ties clip end to the end of the underlying wave audio</param>
			public WavClip(IDataProvider clipDataProvider, Time clipBegin, Time clipEnd)
			{
				if (clipDataProvider == null)
				{
					throw new exception.MethodParameterIsNullException("The data provider of a WavClip can not be null");
				}
				mDataProvider = clipDataProvider;
				setClipBegin(clipBegin);
				setClipEnd(clipEnd);
			}

			/// <summary>
			/// Gets the duration of the underlying RIFF wav file 
			/// </summary>
			/// <returns>The duration</returns>
			public override TimeDelta getMediaDuration()
			{
				Stream raw = getDataProvider().getInputStream();
				PCMDataInfo pcmInfo;
				try
				{
					pcmInfo = PCMDataInfo.parseRiffWaveHeader(raw);
				}
				finally
				{
					raw.Close();
				}
				return new TimeDelta(pcmInfo.getDuration());
			}


			/// <summary>
			/// Creates a copy of the wav clip
			/// </summary>
			/// <returns>The copy</returns>
			public WavClip copy()
			{
				Time clipEnd = null;
				if (!isClipEndTiedToEOM()) clipEnd = getClipEnd().copy();
				return new WavClip(getDataProvider().copy(), getClipBegin().copy(), clipEnd);
			}

			/// <summary>
			/// Exports the clip to a destination <see cref="Presentation"/>
			/// </summary>
			/// <param name="destPres">The destination <see cref="Presentation"/></param>
			/// <returns>The exported clip</returns>
			public WavClip export(Presentation destPres)
			{
				Time clipEnd = null;
				if (!isClipEndTiedToEOM()) clipEnd = getClipEnd().copy();
				return new WavClip(getDataProvider().export(destPres), getClipBegin().copy(), clipEnd);
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
			/// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
			/// </summary>
			/// <returns>The raw PCM audio data <see cref="Stream"/></returns>
			public Stream getAudioData()
			{
				return getAudioData(Time.Zero);
			}

			/// <summary>
			/// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
			/// after a given sub-clip begin time
			/// </summary>
			/// <param name="subClipBegin"></param>
			/// <returns>The raw PCM audio data <see cref="Stream"/></returns>
			/// <seealso cref="getAudioData(Time,Time)"/>
			public Stream getAudioData(Time subClipBegin)
			{
				return getAudioData(subClipBegin, Time.Zero.addTimeDelta(getDuration()));
			}

			/// <summary>
			/// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
			/// between given sub-clip begin and end times
			/// </summary>
			/// <param name="subClipBegin">The beginning of the sub-clip</param>
			/// <param name="subClipEnd">The end of the sub-clip</param>
			/// <returns>The raw PCM audio data <see cref="Stream"/></returns>
			/// <remarks>
			/// <para>Sub-clip times must be in the interval <c>[0;this.getAudioDuration()]</c>.</para>
			/// <para>
			/// The sub-clip is
			/// relative to clip begin of the WavClip, that if <c>this.getClipBegin()</c>
			/// returns <c>00:00:10</c>, <c>this.getClipEnd()</c> returns <c>00:00:50</c>, 
			/// <c>x</c> and <c>y</c> is <c>00:00:05</c> and <c>00:00:30</c> respectively, 
			/// then <c>this.getAudioData(x, y)</c> will get the audio in the underlying wave audio between
			/// <c>00:00:15</c> and <c>00:00:40</c>
			/// </para>
			/// </remarks>
			public Stream getAudioData(Time subClipBegin, Time subClipEnd)
			{
				if (subClipBegin == null)
				{
					throw new exception.MethodParameterIsNullException("subClipBegin must not be null");
				}
				if (subClipEnd == null)
				{
					throw new exception.MethodParameterIsNullException("subClipEnd must not be null");
				}
				if (
					subClipBegin.isLessThan(Time.Zero) 
					|| subClipEnd.isLessThan(subClipBegin)
					|| subClipBegin.addTimeDelta(getDuration()).isLessThan(subClipEnd))
				{
					throw new exception.MethodParameterIsOutOfBoundsException(
						"The interval [subClipBegin;subClipEnd] must be non-empty and contained in [0;getDuration()]");
				}
				Stream raw = getDataProvider().getInputStream();
				PCMDataInfo pcmInfo = PCMDataInfo.parseRiffWaveHeader(raw);
				Time rawEndTime = Time.Zero.addTimeDelta(pcmInfo.getDuration());
				if (
					getClipBegin().isLessThan(Time.Zero)
					|| getClipBegin().isGreaterThan(getClipEnd())
					|| getClipEnd().isGreaterThan(rawEndTime))
				{
					throw new exception.InvalidDataFormatException(String.Format(
						"WavClip [{0};{1}] is empty or not within the underlying wave data stream ([0;{2}])",
						getClipBegin().ToString(), getClipEnd().ToString(), rawEndTime.ToString()));
				}
				Time rawClipBegin = getClipBegin().addTime(subClipBegin);
				Time rawClipEnd = getClipBegin().addTime(subClipEnd);
				long offset;
				long beginPos = raw.Position + (long)((rawClipBegin.getTimeAsMillisecondFloat() * pcmInfo.getByteRate()) / 1000);
				offset = (beginPos - raw.Position) % pcmInfo.getBlockAlign();
				beginPos -= offset;
				long endPos = raw.Position + (long)((rawClipEnd.getTimeAsMillisecondFloat() * pcmInfo.getByteRate()) / 1000);
				offset = (endPos - raw.Position) % pcmInfo.getBlockAlign();
				endPos -= offset;
				utilities.SubStream res = new utilities.SubStream(
					raw,
					beginPos, 
					endPos-beginPos);
				return res;
			}

			#region IValueEquatable<WavClip> Members


			/// <summary>
			/// Determines of <c>this</c> has the same value as a given other instance
			/// </summary>
			/// <param name="other">The other instance</param>
			/// <returns>A <see cref="bool"/> indicating the result</returns>
			public bool valueEquals(WavClip other)
			{
				if (other == null) return false;
				if (!getClipBegin().isEqualTo(other.getClipBegin())) return false;
				if (isClipEndTiedToEOM() != other.isClipEndTiedToEOM()) return false;
				if (!getClipEnd().isEqualTo(other.getClipEnd())) return false;
				if (!getDataProvider().valueEquals(other.getDataProvider())) return false;
				return true;
			}

			#endregion

		}

		/// <summary>
		/// Stores the <see cref="WavClip"/>s of <c>this</c>
		/// </summary>
		private List<WavClip> mWavClips = new List<WavClip>();

		/// <summary>
		/// Constructor associating the newly constructed <see cref="WavAudioMediaData"/> 
		/// with a given <see cref="MediaDataManager"/> 
		/// </summary>
		protected internal WavAudioMediaData()
		{
		}

		private void PCMFormat_FormatChanged(object sender, EventArgs e)
		{
			if (mWavClips.Count > 0)
			{
				throw new exception.InvalidDataFormatException(
					"Can not change PCMFormat of the WavAudioMediaData "
					+"since it already contains audio data of another format"); 
			}
			if (getMediaDataManager().getEnforceSinglePCMFormat())
			{
				if (!getMediaDataManager().getDefaultPCMFormat().valueEquals(getPCMFormat()))
				{
					throw new exception.InvalidDataFormatException(
						"The PCM format change is invalid because the MediaDataManager enforces single PCM format");
				}
			}
		}

		private PCMFormatInfo mPCMFormat;

		/// <summary>
		/// Gets the <see cref="PCMFormatInfo"/> of <c>this</c>.
		/// </summary>
		/// <returns>The PCMFormatInfo</returns>
		/// <remarks>The <see cref="PCMFormatInfo"/> is returned by reference, so any changes to the returned instance</remarks>
		public override PCMFormatInfo getPCMFormat()
		{
			if (mPCMFormat == null)
			{
				mPCMFormat = new PCMFormatInfo(getMediaDataManager().getDefaultPCMFormat());
				mPCMFormat.FormatChanged += new EventHandler(PCMFormat_FormatChanged);
			}
			return mPCMFormat;
		}


		/// <summary>
		/// Gets a <see cref="WavClip"/> from a RAW PCM audio <see cref="Stream"/>, 
		/// reading all data from the current position in the stream till it's end
		/// </summary>
		/// <param name="pcmData">The raw PCM stream</param>
		/// <returns>The <see cref="WavClip"/></returns>
		protected WavClip createWavClipFromRawPCMStream(Stream pcmData)
		{
			return createWavClipFromRawPCMStream(pcmData, null);
		}

		/// <summary>
		/// Gets a <see cref="WavClip"/> from a RAW PCM audio <see cref="Stream"/> of a given duration
		/// </summary>
		/// <param name="pcmData">The raw PCM data stream</param>
		/// <param name="duration">The duration</param>
		/// <returns>The <see cref="WavClip"/></returns>
		protected WavClip createWavClipFromRawPCMStream(Stream pcmData, TimeDelta duration)
		{
			IDataProvider newSingleDataProvider = getMediaDataManager().getDataProviderFactory().createDataProvider(
				FileDataProviderFactory.AUDIO_WAV_MIME_TYPE);
			PCMDataInfo pcmInfo = new PCMDataInfo(getPCMFormat());
			if (duration == null)
			{
				pcmInfo.setDataLength((uint)(pcmData.Length - pcmData.Position));
			}
			else
			{
				pcmInfo.setDataLength(pcmInfo.getDataLength(duration));
			}
			Stream nsdps = newSingleDataProvider.getOutputStream();
			try
			{
				pcmInfo.writeRiffWaveHeader(nsdps);
			}
			finally
			{
				nsdps.Close();
			}
			FileDataProviderManager.appendDataToProvider(pcmData, (int)pcmInfo.getDataLength(), newSingleDataProvider);
			WavClip newSingleWavClip = new WavClip(newSingleDataProvider);
			return newSingleWavClip;
		}

		/// <summary>
		/// Forces the PCM data to be stored in a single <see cref="IDataProvider"/>.
		/// </summary>
		/// <remarks>
		/// This effectively copies the data, 
		/// since the <see cref="IDataProvider"/>(s) previously used to store the PCM data are left untouched
		/// </remarks>
		public void forceSingleDataProvider()
		{
			Stream audioData = getAudioData();
			WavClip newSingleClip;
			try
			{
				newSingleClip = createWavClipFromRawPCMStream(audioData);
			}
			finally
			{
				audioData.Close();
			}
			mWavClips.Clear();
			mWavClips.Add(newSingleClip);
		}

		#region MediaData

		/// <summary>
		/// Creates a copy of <c>this</c>, including copies of all <see cref="IDataProvider"/>s used by <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		protected override AudioMediaData audioMediaDataCopy()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of <c>this</c>, including copies of all <see cref="IDataProvider"/>s used by <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public new WavAudioMediaData copy()
		{
			MediaData oCopy = getMediaDataFactory().createMediaData(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is WavAudioMediaData))
			{
				throw new exception.FactoryCannotCreateTypeException(
					"The MediaDataFactory can not create a WavAudioMediaData");
			}
			WavAudioMediaData copy = (WavAudioMediaData)oCopy;
			foreach (WavClip clip in mWavClips)
			{
				copy.mWavClips.Add(clip.copy());
			}
			return copy;
		}

		/// <summary>
		/// Exports <c>this</c> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The given destination presentation</param>
		/// <returns>The exported wav audio media data</returns>
		protected override MediaData protectedExport(Presentation destPres)
		{
			return export(destPres);
		}

		/// <summary>
		/// Exports <c>this</c> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The given destination presentation</param>
		/// <returns>The exported wav audio media data</returns>
		public new WavAudioMediaData export(Presentation destPres)
		{
			WavAudioMediaData expWAMD = destPres.getMediaDataFactory().createMediaData(
				getXukLocalName(), getXukNamespaceUri()) as WavAudioMediaData;
			if (expWAMD == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaDataFactory of the destination Presentation cannot create a WavAudioMediaData matching QName {0}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			expWAMD.setPCMFormat(getPCMFormat());
			foreach (WavClip clip in mWavClips)
			{
				expWAMD.mWavClips.Add(clip.export(destPres));
			}
			return expWAMD;
		}

		/// <summary>
		/// Deletes the <see cref="MediaData"/>, detaching it from it's manager 
		/// and clearing the list of clips making up the wave audio media
		/// </summary>
		public override void delete()
		{
			mWavClips.Clear();
			base.delete();
		}

		/// <summary>
		/// Gets a <see cref="List{IDataProvider}"/> of the <see cref="IDataProvider"/>s
		/// used to store the Wav audio data
		/// </summary>
		/// <returns>The <see cref="List{IDataProvider}"/></returns>
		public override List<IDataProvider> getListOfUsedDataProviders()
		{
			List<IDataProvider> usedDP = new List<IDataProvider>(mWavClips.Count);
			foreach (WavClip clip in mWavClips)
			{
				if (!usedDP.Contains(clip.getDataProvider())) usedDP.Add(clip.getDataProvider());
			}
			return usedDP;
		}


		#endregion

		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="Time"/>s
		/// as raw PCM data
		/// </summary>
		/// <param name="clipBegin">The given clip begin <see cref="Time"/></param>
		/// <param name="clipEnd">The given clip end <see cref="Time"/></param>
		/// <returns>The <see cref="Stream"/></returns>
		public override Stream getAudioData(Time clipBegin, Time clipEnd)
		{
			if (clipBegin.isNegativeTimeOffset())
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The clip begin value can not be a negative time");
			}
			if (clipEnd.isLessThan(clipBegin))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The clip end can not be before clip begin");
			}
			if (clipEnd.isGreaterThan(Time.Zero.addTimeDelta(getAudioDuration())))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The clip end can not beyond the end of the audio content");
			}
			Time timeBeforeStartIndexClip = new Time();
			Time timeBeforeEndIndexClip = new Time();
			Time elapsedTime = new Time();
			int i = 0;
			List<Stream> resStreams = new List<Stream>();
			while (i < mWavClips.Count)
			{
				WavClip curClip = mWavClips[i];
				TimeDelta currentClipDuration = curClip.getDuration();
				Time newElapsedTime = elapsedTime.addTimeDelta(currentClipDuration);
				if (newElapsedTime.isLessThan(clipBegin))
				{
					//Do nothing - the current clip and the [clipBegin;clipEnd] are disjunkt
				}
				else if (elapsedTime.isLessThan(clipBegin))
				{
					if (newElapsedTime.isLessThan(clipEnd))
					{
						//Add part of current clip between clipBegin and newElapsedTime 
						//(ie. after clipBegin, since newElapsedTime is at the end of the clip)
						resStreams.Add(curClip.getAudioData(
							Time.Zero.addTimeDelta(clipBegin.getTimeDelta(elapsedTime))));
					}
					else
					{
						//Add part of current clip between clipBegin and clipEnd
						resStreams.Add(curClip.getAudioData(
							Time.Zero.addTimeDelta(clipBegin.getTimeDelta(elapsedTime)),
							Time.Zero.addTimeDelta(clipEnd.getTimeDelta(elapsedTime))));
					}
				}
				else if (elapsedTime.isLessThan(clipEnd))
				{
					if (newElapsedTime.isLessThan(clipEnd))
					{
						//Add part of current clip between elapsedTime and newElapsedTime
						//(ie. entire clip since elapsedTime and newElapsedTime is at
						//the beginning and end of the clip respectively)
						resStreams.Add(curClip.getAudioData());
					}
					else
					{
						//Add part of current clip between elapsedTime and clipEnd
						//(ie. before clipEnd since elapsedTime is at the beginning of the clip)
						resStreams.Add(curClip.getAudioData(
							Time.Zero,
							Time.Zero.addTimeDelta(clipEnd.getTimeDelta(elapsedTime))));
					}
				}
				else
				{
					//The current clip and all remaining clips are beyond clipEnd
					break;
				}
				elapsedTime = newElapsedTime;
				i++;
			}
			if (resStreams.Count == 0)
			{
				return new MemoryStream(0);
			}
			return new SequenceStream(resStreams);
		}

		/// <summary>
		/// Appends audio of a given duration from a given source PCM data <see cref="Stream"/> to the wav audio media data
		/// </summary>
		/// <param name="pcmData">The source PCM data stream</param>
		/// <param name="duration">The duration of the audio to append</param>
		public override void appendAudioData(Stream pcmData, TimeDelta duration)
		{
			WavClip newAppClip = createWavClipFromRawPCMStream(pcmData, duration);
			mWavClips.Add(newAppClip);
		}


		/// <summary>
		/// Inserts audio of a given duration from a given source PCM data <see cref="Stream"/> to the wav audio media data
		/// at a given point
		/// </summary>
		/// <param name="pcmData">The source PCM data stream</param>
		/// <param name="insertPoint">The insert point</param>
		/// <param name="duration">The duration of the aduio to append</param>
		public override void insertAudioData(Stream pcmData, Time insertPoint, TimeDelta duration)
		{
			Time insPt = insertPoint.copy();
			if (insPt.isLessThan(Time.Zero))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The given insert point is negative");
			}
			WavClip newInsClip = createWavClipFromRawPCMStream(pcmData, duration);
			Time endTime = Time.Zero.addTimeDelta(getAudioDuration());
			if (insertPoint.isGreaterThan(endTime))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The given insert point is beyond the end of the WavAudioMediaData");
			}
			if (insertPoint.isEqualTo(endTime))
			{
				mWavClips.Add(newInsClip);
				return;
			}
			Time elapsedTime = Time.Zero;
			int clipIndex = 0;
			while (clipIndex < mWavClips.Count)
			{
				WavClip curClip = mWavClips[clipIndex];
				if (insPt.isEqualTo(elapsedTime))
				{
					//If the insert point at the beginning of the current clip, insert the new clip and break
					mWavClips.Insert(clipIndex, newInsClip);
					break;
				}
				else if (insPt.isLessThan(elapsedTime.addTimeDelta(curClip.getDuration())))
				{
					//If the insert point is between the beginning and end of the current clip, 
					//Replace the current clip with three clips containing 
					//the audio in the current clip before the insert point,
					//the audio to be inserted and the audio in the current clip after the insert point respectively
					Time insPtInCurClip = Time.Zero.addTimeDelta(insPt.getTimeDelta(elapsedTime));
					Stream audioDataStream;
					audioDataStream = curClip.getAudioData(Time.Zero, insPtInCurClip);
					WavClip curClipBeforeIns, curClipAfterIns;
					try
					{
						curClipBeforeIns = createWavClipFromRawPCMStream(audioDataStream);
					}
					finally
					{
						audioDataStream.Close();
					}
					audioDataStream = curClip.getAudioData(insPtInCurClip);
					try
					{
						curClipAfterIns = createWavClipFromRawPCMStream(audioDataStream);
					}
					finally
					{
						audioDataStream.Close();
					}
					mWavClips.RemoveAt(clipIndex);
					mWavClips.InsertRange(clipIndex, new WavClip[] { curClipBeforeIns, newInsClip, curClipAfterIns });
					break;
				}
				elapsedTime = elapsedTime.addTimeDelta(curClip.getDuration());
				clipIndex++;
			}
		}

		/// <summary>
		/// Replaces audio in the wave audio media data of a given duration at a given replace point with
		/// audio from a given source PCM data <see cref="Stream"/>
		/// </summary>
		/// <param name="pcmData">The given audio data stream</param>
		/// <param name="replacePoint">The given replace point</param>
		/// <param name="duration">The given duration</param>
		public override void replaceAudioData(Stream pcmData, Time replacePoint, TimeDelta duration)
		{
			removeAudioData(replacePoint, replacePoint.addTimeDelta(duration));
			insertAudioData(pcmData, replacePoint, duration);
		}

		/// <summary>
		/// Gets the intrinsic duration of the audio data
		/// </summary>
		/// <returns>The duration as an <see cref="TimeDelta"/></returns>
		public override TimeDelta getAudioDuration()
		{
			TimeDelta dur = new TimeDelta();
			foreach (WavClip clip in mWavClips)
			{
				dur.addTimeDelta(clip.getDuration());
			}
			return dur;
		}

		/// <summary>
		/// Removes the audio between given clip begin and end points
		/// </summary>
		/// <param name="clipBegin">The given clip begin point</param>
		/// <param name="clipEnd">The given clip end point</param>
		public override void removeAudioData(Time clipBegin, Time clipEnd)
		{
			if (clipBegin == null || clipEnd == null)
			{
				throw new exception.MethodParameterIsNullException("Clip begin and clip end can not be null");
			}
			if (
				clipBegin.isLessThan(Time.Zero) 
				|| clipBegin.isGreaterThan(clipEnd) 
				|| clipEnd.isGreaterThan(Time.Zero.addTimeDelta(getAudioDuration())))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					String.Format("The given clip times are not valid, must be between 00:00:00.000 and {0}", getAudioDuration()));
			}
			Time curBeginTime = Time.Zero;

			List<WavClip> newClipList = new List<WavClip>();
			foreach (WavClip curClip in mWavClips)
			{
				Time curEndTime = curBeginTime.addTimeDelta(curClip.getDuration());
				if ((!curEndTime.isGreaterThan(clipBegin)) || (!curBeginTime.isLessThan(clipEnd)))
				{
					//The current clip is before or beyond the range to remove - 
					//so the clip is added unaltered to the new list of clips
					newClipList.Add(curClip);
				}
				else if (curBeginTime.isLessThan(clipBegin) && curEndTime.isGreaterThan(clipEnd))
				{
					//Some of the current clip is before the range and some is after
					TimeDelta beforePartDur = curBeginTime.getTimeDelta(clipBegin);
					TimeDelta beyondPartDur = curEndTime.getTimeDelta(clipEnd);
					Stream beyondAS = curClip.getAudioData(curClip.getClipEnd().subtractTimeDelta(beyondPartDur));
					WavClip beyondPartClip;
					try
					{
						beyondPartClip = createWavClipFromRawPCMStream(beyondAS);
					}
					finally
					{
						beyondAS.Close();
					}
						
					curClip.setClipEnd(curClip.getClipBegin().addTimeDelta(beforePartDur));
					newClipList.Add(curClip);
					newClipList.Add(beyondPartClip);
				}
				else if (curBeginTime.isLessThan(clipBegin) && curEndTime.isGreaterThan(clipBegin))
				{
					//Some of the current clip is before the range to remove, none is beyond
					TimeDelta beforePartDur = curBeginTime.getTimeDelta(clipBegin);
					curClip.setClipEnd(curClip.getClipBegin().addTimeDelta(beforePartDur));
					newClipList.Add(curClip);
				}
				else if (curBeginTime.isLessThan(clipEnd) && curEndTime.isGreaterThan(clipEnd))
				{ 
					//Some of the current clip is beyond the range to remove, none is before
					TimeDelta beyondPartDur = curEndTime.getTimeDelta(clipEnd);
					curClip.setClipBegin(curClip.getClipEnd().subtractTimeDelta(beyondPartDur));
					newClipList.Add(curClip);
				}
				else
				{
					//All of the current clip is within the range to remove, 
					//so this clip is not added to the new list of WavClips
				}
				curBeginTime = curEndTime;
			}
			mWavClips = newClipList;
		}

		#region IXukAble

		/// <summary>
		/// Clears the <see cref="WavAudioMediaData"/>, removing all <see cref="WavClip"/>s
		/// </summary>
		protected override void clear()
		{
			mWavClips.Clear();
			base.clear();
		}

		/// <summary>
		/// Reads a child of a WavAudioMediaData xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mWavClips":
						xukInWavClips(source);
						break;
					case "mPCMFormat":
						xukInPCMFormat(source);
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!readItem) base.xukInChild(source);
		}

		private void xukInPCMFormat(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "PCMFormatInfo" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							getPCMFormat().xukIn(source);
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		private void xukInWavClips(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "WavClip" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							xukInWavClip(source);
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		private void xukInWavClip(XmlReader source)
		{
			string clipBeginAttr = source.GetAttribute("clipBegin");
			Time cb = Time.Zero;
			if (clipBeginAttr != null)
			{
				try
				{
					cb = new Time(clipBeginAttr);
				}
				catch (Exception e)
				{
					throw new exception.XukException(
						String.Format("Invalid clip begin time {0}", clipBeginAttr),
						e);
				}
			}
			string clipEndAttr = source.GetAttribute("clipEnd");
			Time ce = null;
			if (clipEndAttr != null)
			{
				try
				{
					ce = new Time(clipEndAttr);
				}
				catch (Exception e)
				{
					throw new exception.XukException(
						String.Format("Invalid clip end time {0}", clipEndAttr),
						e);
				}

			}
			string dataProviderUid = source.GetAttribute("dataProvider");
			if (dataProviderUid == null)
			{
				throw new exception.XukException("dataProvider attribute is missing from WavClip element");
			}
			IDataProvider prov;
			prov = getMediaDataManager().getPresentation().getDataProviderManager().getDataProvider(dataProviderUid);
			mWavClips.Add(new WavClip(prov, cb, ce));
			if (!source.IsEmptyElement)
			{
				source.ReadSubtree().Close();
			}
		}

		/// <summary>
		/// Write the child elements of a WavAudioMediaData element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			base.xukOutChildren(destination, baseUri);
			destination.WriteStartElement("mPCMFormat");
			getPCMFormat().xukOut(destination, baseUri);
			destination.WriteEndElement();
			destination.WriteStartElement("mWavClips", ToolkitSettings.XUK_NS);
			foreach (WavClip clip in mWavClips)
			{
				destination.WriteStartElement("WavClip", ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("dataProvider", clip.getDataProvider().getUid());
				destination.WriteAttributeString("clipBegin", clip.getClipBegin().ToString());
				if (!clip.isClipEndTiedToEOM()) destination.WriteAttributeString("clipEnd", clip.getClipEnd().ToString());
				destination.WriteEndElement();
			}
			destination.WriteEndElement();
		}
		#endregion

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
		public override void mergeWith(AudioMediaData other)
		{
			if (other is WavAudioMediaData)
			{
				if (!getPCMFormat().isCompatibleWith(other.getPCMFormat()))
				{
					throw new exception.InvalidDataFormatException(
						"Can not merge this with a WavAudioMediaData with incompatible audio data");
				}
				WavAudioMediaData otherWav = (WavAudioMediaData)other;
				mWavClips.AddRange(otherWav.mWavClips);
				otherWav.mWavClips.Clear();
			}
			else
			{
				base.mergeWith(other);
			}
		}

		/// <summary>
		/// Splits the audio media data at a given split point in time,
		/// <c>this</c> retaining the audio before the split point,
		/// creating a new <see cref="WavAudioMediaData"/> containing the audio after the split point
		/// </summary>
		/// <param name="splitPoint">The given split point</param>
		/// <returns>A audio media data containing the audio after the split point</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given split point is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given split point is negative or is beyond the duration of <c>this</c>
		/// </exception>
		public override AudioMediaData split(Time splitPoint)
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
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"Split point {0} is beyond the WavAudioMediaData end - audio length is {1}",
					splitPoint.ToString(), getAudioDuration().ToString()));
			}
			WavAudioMediaData oWAMD = getMediaDataFactory().createMediaData(getXukLocalName(), getXukNamespaceUri()) as WavAudioMediaData;
			if (oWAMD == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"Thrown if the MediaDataFactory can not create a WacAudioMediaData matching Xuk QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			oWAMD.setPCMFormat(getPCMFormat());

			Time elapsed = Time.Zero;
			List<WavClip> clips = new List<WavClip>(mWavClips);
			mWavClips.Clear();
			oWAMD.mWavClips.Clear();
			for (int i = 0; i < clips.Count; i++)
			{
				WavClip curClip = clips[i];
				Time endCurClip = elapsed.addTimeDelta(curClip.getDuration());
				if (splitPoint.isLessThanOrEqualTo(elapsed))
				{
					oWAMD.mWavClips.Add(curClip);
				}
				else if (splitPoint.isLessThan(endCurClip))
				{
					WavClip secondPartClip = new WavClip(
						curClip.getDataProvider(), 
						curClip.getClipBegin(), 
						curClip.isClipEndTiedToEOM()?null as Time:curClip.getClipEnd());
					curClip.setClipEnd(curClip.getClipBegin().addTime(splitPoint.subtractTime(elapsed)));
					secondPartClip.setClipBegin(curClip.getClipEnd());
					mWavClips.Add(curClip);
					oWAMD.mWavClips.Add(secondPartClip);
				}
				else
				{
					mWavClips.Add(curClip);
				}
				elapsed = elapsed.addTimeDelta(curClip.getDuration());
			}
			return oWAMD;
		}
	}
}
