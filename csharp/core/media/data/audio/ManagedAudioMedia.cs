using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;
using urakawa.events.media.data;

namespace urakawa.media.data.audio
{
	/// <summary>
	/// Managed implementation of <see cref="IAudioMedia"/>, that uses <see cref="AudioMediaData"/> to store audio data
	/// </summary>
	public class ManagedAudioMedia : AbstractMedia, IAudioMedia, IManagedMedia
	{

		#region Event related members
		/// <summary>
		/// Event fired after the <see cref="AudioMediaData"/> of the <see cref="ManagedAudioMedia"/> has changed
		/// </summary>
		public event EventHandler<MediaDataChangedEventArgs> mediaDataChanged;

		/// <summary>
		/// Fires the <see cref="mediaDataChanged"/> event
		/// </summary>
		/// <param name="source">
		/// The source, that is the <see cref="ManagedAudioMedia"/> whoose <see cref="AudioMediaData"/> has changed
		/// </param>
		/// <param name="newData">The new <see cref="AudioMediaData"/></param>
		/// <param name="prevData">The <see cref="AudioMediaData"/> prior to the change</param>
		protected void notifyMediaDataChanged(ManagedAudioMedia source, AudioMediaData newData, AudioMediaData prevData)
		{
			EventHandler<MediaDataChangedEventArgs> d = mediaDataChanged;
			if (d != null) d(this, new MediaDataChangedEventArgs(source, newData, prevData));
		}

		void AudioMediaData_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion

		internal ManagedAudioMedia()
		{
			this.mediaDataChanged += new EventHandler<MediaDataChangedEventArgs>(ManagedAudioMedia_mediaDataChanged);
		}

		void ManagedAudioMedia_mediaDataChanged(object sender, MediaDataChangedEventArgs e)
		{
			notifyChanged(e);
		}

		private AudioMediaData mAudioMediaData;

		#region IMedia Members

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if <c>this</c> is a continuous <see cref="IMedia"/>
		/// </summary>
		/// <returns><c>true</c></returns>
		public override bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if <c>this</c> is a discrete <see cref="IMedia"/>
		/// </summary>
		/// <returns><c>false</c></returns>
		public override bool isDiscrete()
		{
			return false;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if <c>this</c> is a sequence <see cref="IMedia"/>
		/// </summary>
		/// <returns><c>false</c></returns>
		public override bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Gets a copy of <c>this</c>. 
		/// The copy is deep in the sense that the underlying <see cref="AudioMediaData"/> is also copied
		/// </summary>
		/// <returns>The copy</returns>
		public new ManagedAudioMedia copy()
		{
			return copyProtected() as ManagedAudioMedia;
		}

		/// <summary>
		/// Gets a copy of <c>this</c>. 
		/// The copy is deep in the sense that the underlying <see cref="AudioMediaData"/> is also copied
		/// </summary>
		/// <returns>The copy</returns>
		protected override IMedia copyProtected()
		{
			ManagedAudioMedia copyMAM = base.copyProtected() as ManagedAudioMedia;
			if (copyMAM == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory can not a ManagedAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			copyMAM.setLanguage(getLanguage());
			copyMAM.setMediaData(getMediaData().copy());
			return copyMAM;
		}

		/// <summary>
		/// Exports the external audio media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external audio media</returns>
		public new ManagedAudioMedia export(Presentation destPres)
		{
			return export(destPres) as ManagedAudioMedia;
		}

		/// <summary>
		/// Exports the external audio media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external audio media</returns>
		protected override IMedia exportProtected(Presentation destPres)
		{
			ManagedAudioMedia exported = base.exportProtected(destPres) as ManagedAudioMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a ExternalAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exported.setLanguage(getLanguage());
			exported.setMediaData(getMediaData().export(destPres));
			return exported;
		}



		/// <summary>
		/// Gets a 'copy' of <c>this</c>, including only the audio after the given clip begin time
		/// </summary>
		/// <param name="clipBegin">The given clip begin time</param>
		/// <returns>The copy</returns>
		public ManagedAudioMedia copy(Time clipBegin)
		{
			return copy(clipBegin, Time.Zero.addTimeDelta(getDuration()));
		}


		/// <summary>
		/// Gets a 'copy' of <c>this</c>, including only the audio between the given clip begin and end times
		/// </summary>
		/// <param name="clipBegin">The given clip begin time</param>
		/// <param name="clipEnd">The given clip end time</param>
		/// <returns>The copy</returns>
		public ManagedAudioMedia copy(Time clipBegin, Time clipEnd)
		{
			ManagedAudioMedia copyMAM =
				getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri()) as ManagedAudioMedia;
			if (copyMAM == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory can not a ManagedAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			Stream pcm = getMediaData().getAudioData(clipBegin, clipEnd);
			try
			{
				AudioMediaData data = getMediaDataFactory().createMediaData(
					getMediaData().getXukLocalName(), getMediaData().getXukNamespaceUri()) as AudioMediaData;
				data.setPCMFormat(getMediaData().getPCMFormat());
				data.appendAudioData(pcm, null);
				copyMAM.setMediaData(data);
				return copyMAM;
			}
			finally
			{
				pcm.Close();
			}
		}


		#endregion

		#region IXUKAble members

		/// <summary>
		/// Clears the <see cref="ManagedAudioMedia"/> setting the underlying <see cref="AudioMediaData"/> to <c>null</c>
		/// </summary>
		protected override void clear()
		{
			mAudioMediaData = null;
			base.clear();
		}

		/// <summary>
		/// Reads the attributes of a ManagedAudioMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string uid = source.GetAttribute("audioMediaDataUid");
			if (uid == null || uid == "")
			{
				throw new exception.XukException("audioMediaDataUid attribute is missing from AudioMediaData");
			}
			if (!getMediaDataFactory().getMediaDataManager().isManagerOf(uid))
			{
				throw new exception.XukException(String.Format(
					"The MediaDataManager does not mamage a AudioMediaData with uid {0}", uid));
			}
			MediaData md = getMediaDataFactory().getMediaDataManager().getMediaData(uid);
			if (!(md is AudioMediaData))
			{
				throw new exception.XukException(String.Format(
					"The MediaData with uid {0} is a {1} which is not a urakawa.media.data.audio.AudioMediaData",
					uid, md.GetType().FullName));
			}
			setMediaData(md);
			base.xukInAttributes(source);
		}

		/// <summary>
		/// Writes the attributes of a ManagedAudioMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("audioMediaDataUid", getMediaData().getUid());
			base.xukOutAttributes(destination, baseUri);
		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>		
		public override bool ValueEquals(IMedia other)
		{
			if (!base.ValueEquals(other)) return false;
			ManagedAudioMedia otherMAM = (ManagedAudioMedia)other;
			if (!getMediaData().ValueEquals(otherMAM.getMediaData())) return false;
			return true;
		}

		#endregion

		#region IContinuous Members

		/// <summary>
		/// Gets the duration of <c>this</c>, that is the duration of the underlying <see cref="AudioMediaData"/>
		/// </summary>
		/// <returns>The duration</returns>
		public TimeDelta getDuration()
		{
			return getMediaData().getAudioDuration();
		}

		IContinuous IContinuous.split(urakawa.media.timing.Time splitPoint)
		{
			return split(splitPoint);
		}

		/// <summary>
		/// Splits the managed audio media at a given split point in time,
		/// <c>this</c> retaining the audio before the split point,
		/// creating a new <see cref="ManagedAudioMedia"/> containing the audio after the split point
		/// </summary>
		/// <param name="splitPoint">The given split point</param>
		/// <returns>A managed audio media containing the audio after the split point</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given split point is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given split point is negative or is beyond the duration of <c>this</c>
		/// </exception>
		public virtual ManagedAudioMedia split(urakawa.media.timing.Time splitPoint)
		{
			AudioMediaData secondPartData = getMediaData().split(splitPoint);
			IMedia oSecondPart = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (!(oSecondPart is ManagedAudioMedia))
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory can not create a ManagedAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			ManagedAudioMedia secondPartMAM = (ManagedAudioMedia)oSecondPart;
			secondPartMAM.setMediaData(secondPartData);
			return secondPartMAM;
		}

		#endregion

		MediaData IManagedMedia.getMediaData()
		{
			return getMediaData();
		}

		/// <summary>
		/// Gets the <see cref="AudioMediaData"/> storing the audio of <c>this</c>
		/// </summary>
		/// <returns>The audio media data</returns>
		public AudioMediaData getMediaData()
		{
			if (mAudioMediaData == null)
			{
				//Lazy initialization
				setMediaData(getMediaDataFactory().createAudioMediaData());
			}
			return mAudioMediaData;
		}

		/// <summary>
		/// Sets the <see cref="MediaData"/> of the managed audio media
		/// </summary>
		/// <param name="data">The new media data, must be a <see cref="AudioMediaData"/></param>
		/// <exception cref="exception.MethodParameterIsWrongTypeException">
		/// </exception>
		public void setMediaData(MediaData data)
		{
			if (!(data is AudioMediaData))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The MediaData of a ManagedAudioMedia must be a AudioMediaData");
			}
			if (mAudioMediaData != null) mAudioMediaData.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(AudioMediaData_changed);
			AudioMediaData prevData = mAudioMediaData;
			mAudioMediaData = (AudioMediaData)data;
			mAudioMediaData.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(AudioMediaData_changed);
			if (mAudioMediaData != prevData) notifyMediaDataChanged(this, mAudioMediaData, prevData);
		}

		/// <summary>
		/// Gets the <see cref="MediaDataFactory"/> creating the <see cref="MediaData"/>
		/// used by <c>this</c>.
		/// Convenience for <c>getMediaData().getMediaDataManager().getMediaDataFactory()</c>
		/// </summary>
		/// <returns>The media data factory</returns>
		public MediaDataFactory getMediaDataFactory()
		{
			return getMediaFactory().Presentation.MediaDataFactory;
		}

		/// <summary>
		/// Merges <c>this</c> with a given other <see cref="ManagedAudioMedia"/>,
		/// appending the audio data of the other <see cref="ManagedAudioMedia"/> to <c>this</c>,
		/// leaving the other <see cref="ManagedAudioMedia"/> without audio data
		/// </summary>
		/// <param name="other">The given other managed audio media</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="other"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.InvalidDataFormatException">
		/// Thrown when the PCM format of <c>this</c> is not compatible with that of <paramref name="other"/>
		/// </exception>
		public void mergeWith(ManagedAudioMedia other)
		{
			if (other == null)
			{
				throw new exception.MethodParameterIsNullException("Can not merge with a null ManagedAudioMedia");
			}
			getMediaData().mergeWith(other.getMediaData());
		}

	}
}
