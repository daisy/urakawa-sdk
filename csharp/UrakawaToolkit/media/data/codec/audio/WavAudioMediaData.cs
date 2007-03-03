using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using urakawa.media.data;
using urakawa.media.timing;
using urakawa.media.data.utillities;

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
				: this(clipDataProvider)
			{
				if (clipDataProvider == null)
				{
					throw new exception.MethodParameterIsNullException("The data provider of a WavClip can not be null");
				}
				mDataProvider = clipDataProvider;
				setClipBegin(clipBegin);
				setClipEnd(clipEnd);
			}

			private Time mClipBegin;
			/// <summary>
			/// Gets (a copy of) the clip begin <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <returns>
			/// The clip begin <see cref="Time"/> - can not be <c>null</c>
			/// </returns>
			public Time getClipBegin()
			{
				return mClipBegin.copy();
			}

			/// <summary>
			/// Sets the clip begin <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <param name="newClipBegin">The new clip begin <see cref="Time"/> - can not be <c>null</c></param>
			public void setClipBegin(Time newClipBegin)
			{
				if (newClipBegin == null)
				{
					throw new exception.MethodParameterIsNullException("Clip begin of a WavClip can not be null");
				}
				if (newClipBegin.isGreaterThan(getClipEnd()))
				{
					throw new exception.MethodParameterIsOutOfBoundsException(
						"The new clip begin is beyond the current clip end");
				}
				mClipBegin = newClipBegin.copy();
			}

			private Time mClipEnd;
			/// <summary>
			/// Gets (a copy of) the clip end <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <returns>The clip end <see cref="Time"/></returns>
			public Time getClipEnd()
			{
				if (mClipEnd == null) return getClipBegin().addTimeDelta(getAudioDuration());
				return mClipEnd.copy();
			}

			/// <summary>
			/// Determines if clip end is tied to the end of the underlying wave audio
			/// </summary>
			/// <returns>
			/// A <see cref="bool"/> indicating if clip end is tied to the end of the underlying wave audio
			/// </returns>
			public bool isClipEndTiedToEOWA()
			{
				return (mClipEnd == null);
			}


			/// <summary>
			/// Sets the clip end <see cref="Time"/> of <c>this</c>
			/// </summary>
			/// <param name="newClipEnd">
			/// The new clip end <see cref="Time"/> 
			/// - a <c>null</c> ties the clip end to the end of the underlying wave audio
			/// </param>
			/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
			/// Thrown when the new clip end <see cref="Time"/> is less that the current clip begin <see cref="Time"/>
			/// </exception>
			/// <remarks>
			/// There is not check to see if the new clip end <see cref="Time"/> 
			/// is beyond the end of the underlyind wave audio
			/// </remarks>
			public void setClipEnd(Time newClipEnd)
			{
				if (newClipEnd == null)
				{
					mClipEnd = null;
				}
				else
				{
					if (newClipEnd.isLessThan(getClipBegin()))
					{
						throw new exception.MethodParameterIsOutOfBoundsException(
							"The new clip end time is before current clip begin");
					}
					mClipEnd = newClipEnd.copy();
				}
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
				return getClipEnd().getTimeDelta(getClipBegin());
			}

			/// <summary>
			/// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
			/// </summary>
			/// <returns>The raw PCM audio data <see cref="Stream"/></returns>
			public Stream getAudioData()
			{
				return getAudioData(getClipBegin());
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
				Time zero = new Time();
				return getAudioData(subClipBegin, zero.addTimeDelta(getAudioDuration()));
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
					|| Time.Zero.addTimeDelta(getAudioDuration()).isLessThan(subClipEnd))
				{
					throw new exception.MethodParameterIsOutOfBoundsException(
						"The interval [subClipBegin;subClipEnd] must be non-empty and contained in [0;getAudioDuration()]");
				}
				Stream raw = getDataProvider().getInputStream();
				PCMDataInfo pcmInfo = PCMDataInfo.parseRiffWaveHeader(raw);
				Time rawEndTime = new Time(pcmInfo.getDuration());
				if (subClipBegin == null) subClipBegin = new Time();
				if (subClipEnd == null) subClipEnd = new Time(pcmInfo.getDuration());
				if (subClipBegin.isGreaterThan(subClipEnd))
				{
					throw new exception.InvalidDataFormatException(
						"Clip begin of the WavClip is beyond the clip end of the underlying RIFF WAVE PCM data");
				}
				if (subClipBegin.isGreaterThan(rawEndTime))
				{
					throw new exception.InvalidDataFormatException(
						"Clip beginning of the WavClip is beyond the end of the underlying RIFF WAVE PCM data"); 
				}
				long beginPos = raw.Position + (long)((subClipBegin.getTimeAsMillisecondFloat() * pcmInfo.ByteRate) / 1000);
				long endPos = raw.Position + (long)((subClipEnd.getTimeAsMillisecondFloat() * pcmInfo.ByteRate) / 1000);
				utillities.SubStream res = new utillities.SubStream(
					raw,
					beginPos, 
					endPos-beginPos);
				return res;
			}
		}

		/// <summary>
		/// Stores the <see cref="WavClip"/>s of <c>this</c>
		/// </summary>
		private List<WavClip> mWavClips = new List<WavClip>();

		/// <summary>
		/// Constructor associating the newly constructed <see cref="WavAudioMediaData"/> 
		/// with a given <see cref="IMediaDataManager"/> 
		/// </summary>
		/// <param name="mngr">The <see cref="IMediaDataManager"/> with which to associate</param>
		protected internal WavAudioMediaData(IMediaDataManager mngr)
		{
			setMediaDataManager(mngr);
		}

		#region IMediaData

		/// <summary>
		/// Creates a copy of <c>this</c>, including copies of all <see cref="IDataProvider"/>s used by <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public override IMediaData copy()
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
		/// Deletes the <see cref="MediaData"/>, detaching it from it's manager 
		/// and clearing the list of clips making up the wave audio media
		/// </summary>
		public override void delete()
		{
			mWavClips.Clear();
			base.delete();
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


		#endregion

		#region IAudioMediaData

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
			Time timeBeforeStartIndexClip = new Time();
			Time timeBeforeEndIndexClip = new Time();
			Time elapsedTime = new Time();
			int i = 0;
			List<Stream> resStreams = new List<Stream>();
			while (i < mWavClips.Count)
			{
				WavClip curClip = mWavClips[i];
				TimeDelta currentClipDuration = curClip.getAudioDuration();
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
						//(ie. after clipEnd, since newElapsedTime is at the end of the clip)
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


			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Appends 
		/// </summary>
		/// <param name="pcmData"></param>
		/// <param name="duration"></param>
		public override void appendAudioData(Stream pcmData, ITimeDelta duration)
		{
			int PCMLength = getPCMLength();
			IDataProvider dataProv = getMediaDataManager().getDataProviderFactory().createDataProvider();
			Stream dpOutput = dataProv.getOutputStream();
			DataProviderManager.appendDataToProvider(pcmData, PCMLength, dataProv);
			dpOutput.Close();
			mWavClips.Add(new WavClip(dataProv, Time.Zero, Time.Zero.addTimeDelta(duration)));
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

		public override void removeAudio(ITime clipBegin)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void removeAudio(ITime clipBegin, ITime clipEnd)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IValueEquatable<IMediaData> Members

		public override bool ValueEquals(IMediaData other)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IXukAble

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

		#endregion

	}
}
