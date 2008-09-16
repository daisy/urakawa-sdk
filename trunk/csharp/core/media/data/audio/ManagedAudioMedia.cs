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
    /// Managed implementation of <see cref="AbstractAudioMedia"/>, that uses <see cref="audio.AudioMediaData"/> to store audio data
    /// </summary>
    public class ManagedAudioMedia : AbstractAudioMedia, IManaged
    {


        private AudioMediaData mAudioMediaData;

        private void Reset()
        {
            mAudioMediaData = null;
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="ManagedAudioMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        public ManagedAudioMedia()
        {
            Reset();
            MediaDataChanged += this_MediaDataChanged;
        }

        /// <summary>
        /// Gets a 'copy' of <c>this</c>, including only the audio after the given clip begin time
        /// </summary>
        /// <param name="clipBegin">The given clip begin time</param>
        /// <returns>The copy</returns>
        public ManagedAudioMedia Copy(Time clipBegin)
        {
            return Copy(clipBegin, Time.Zero.AddTimeDelta(Duration));
        }


        /// <summary>
        /// Gets a 'copy' of <c>this</c>, including only the audio between the given clip begin and end times
        /// </summary>
        /// <param name="clipBegin">The given clip begin time</param>
        /// <param name="clipEnd">The given clip end time</param>
        /// <returns>The copy</returns>
        public ManagedAudioMedia Copy(Time clipBegin, Time clipEnd)
        {

            ManagedAudioMedia copyMAM = MediaFactory.Create<ManagedAudioMedia>();
            Stream pcm = AudioMediaData.GetAudioData(clipBegin, clipEnd);
            try
            {
                AudioMediaData data = MediaDataFactory.Create(
                                          AudioMediaData.XukLocalName, AudioMediaData.XukNamespaceUri) as AudioMediaData;
                data.PCMFormat = AudioMediaData.PCMFormat;
                data.AppendAudioData(pcm, null);
                copyMAM.AudioMediaData = data;
                return copyMAM;
            }
            finally
            {
                pcm.Close();
            }
        }

        #region Media Members

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if <c>this</c> is a continuous <see cref="Media"/>
        /// </summary>
        /// <returns><c>true</c></returns>
        public override bool IsContinuous
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if <c>this</c> is a discrete <see cref="Media"/>
        /// </summary>
        /// <returns><c>false</c></returns>
        public override bool IsDiscrete
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if <c>this</c> is a sequence <see cref="Media"/>
        /// </summary>
        /// <returns><c>false</c></returns>
        public override bool IsSequence
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a copy of <c>this</c>. 
        /// The copy is deep in the sense that the underlying <see cref="audio.AudioMediaData"/> is also copied
        /// </summary>
        /// <returns>The copy</returns>
        public new ManagedAudioMedia Copy()
        {
            return CopyProtected() as ManagedAudioMedia;
        }

        /// <summary>
        /// Gets a copy of the <see cref="Media"/>
        /// </summary>
        /// <returns>The copy</returns>
        protected override Media CopyProtected()
        {
            ManagedAudioMedia cp = (ManagedAudioMedia)base.CopyProtected();

            return cp;
        }

        /// <summary>
        /// Exports the external audio media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external audio media</returns>
        public new ManagedAudioMedia Export(Presentation destPres)
        {
            return Export(destPres) as ManagedAudioMedia;
        }

        /// <summary>
        /// Exports the external audio media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external audio media</returns>
        protected override Media ExportProtected(Presentation destPres)
        {
            ManagedAudioMedia exported = (ManagedAudioMedia)base.ExportProtected(destPres);
            exported.AudioMediaData = AudioMediaData.Export(destPres) as AudioMediaData;
            return exported;
        }


        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="ManagedAudioMedia"/> setting the underlying <see cref="audio.AudioMediaData"/> to <c>null</c>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a ManagedAudioMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string uid = source.GetAttribute("audioMediaDataUid");
            if (uid == null || uid == "")
            {
                throw new exception.XukException("audioMediaDataUid attribute is missing from AudioMediaData");
            }
            if (!MediaDataFactory.MediaDataManager.IsManagerOf(uid))
            {
                throw new exception.XukException(String.Format(
                                                     "The MediaDataManager does not mamage a AudioMediaData with uid {0}",
                                                     uid));
            }
            MediaData md = MediaDataFactory.MediaDataManager.GetMediaData(uid);
            if (!(md is AudioMediaData))
            {
                throw new exception.XukException(String.Format(
                                                     "The AudioMediaData with uid {0} is a {1} which is not a urakawa.media.data.audio.AudioMediaData",
                                                     uid, md.GetType().FullName));
            }
            AudioMediaData = md as AudioMediaData;
            base.XukInAttributes(source);
        }

        /// <summary>
        /// Writes the attributes of a ManagedAudioMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString("audioMediaDataUid", AudioMediaData.Uid);
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IContinuous Members

        /// <summary>
        /// Gets the duration of <c>this</c>, that is the duration of the underlying <see cref="audio.AudioMediaData"/>
        /// </summary>
        /// <returns>The duration</returns>
        public override TimeDelta Duration
        {
            get { return AudioMediaData.AudioDuration; }
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
        public new ManagedAudioMedia Split(urakawa.media.timing.Time splitPoint)
        {
            return SplitProtected(splitPoint) as ManagedAudioMedia;
        }

        /// <summary>
        /// Splits <c>this</c> at a given <see cref="Time"/>
        /// </summary>
        /// <param name="splitPoint">The <see cref="Time"/> at which to split - 
        /// must be between clip begin and clip end <see cref="Time"/>s</param>
        /// <returns>
        /// A newly created <see cref="AbstractAudioMedia"/> containing the audio after <paramref localName="splitPoint"/>,
        /// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="splitPoint"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
        /// </exception>
        protected override AbstractAudioMedia SplitProtected(Time splitPoint)
        {
            AudioMediaData secondPartData = AudioMediaData.Split(splitPoint);
            ManagedAudioMedia secondPartMAM = this.MediaFactory.Create<ManagedAudioMedia>();
            secondPartMAM.AudioMediaData = secondPartData;
            return secondPartMAM;
        }

        #endregion

        #region IManaged members

        /// <summary>
        /// Gets or sets the <see cref="AudioMediaData"/> of the managed audio media
        /// </summary>
        /// <exception cref="exception.MethodParameterIsWrongTypeException">
        /// Then trying t set <see cref="AudioMediaData"/> to a non-<see cref="audio.AudioMediaData"/> value
        /// </exception>
        public MediaData MediaData
        {
            get { return AudioMediaData; }
            set
            {
                if (!(value is AudioMediaData))
                {
                    throw new exception.MethodParameterIsWrongTypeException(
                        "The AudioMediaData of a ManagedAudioMedia must be a AudioMediaData");
                }
                AudioMediaData = value as AudioMediaData;
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaDataFactory"/> creating the <see cref="data.MediaData"/>
        /// used by <c>this</c>.
        /// Convenience for <c>GetMediaData().getMediaDataManager().GetMediaDataFactory()</c>
        /// </summary>
        /// <returns>The media data factory</returns>
        public MediaDataFactory MediaDataFactory
        {
            get { return MediaFactory.Presentation.MediaDataFactory; }
        }


        /// <summary>
        /// Event fired after the <see cref="audio.AudioMediaData"/> of the <see cref="ManagedAudioMedia"/> has changed
        /// </summary>
        public event EventHandler<MediaDataChangedEventArgs> MediaDataChanged;


        /// <summary>
        /// Fires the <see cref="MediaDataChanged"/> event
        /// </summary>
        /// <param name="source">
        /// The source, that is the <see cref="ManagedAudioMedia"/> whoose <see cref="audio.AudioMediaData"/> has changed
        /// </param>
        /// <param name="newData">The new <see cref="audio.AudioMediaData"/></param>
        /// <param name="prevData">The <see cref="audio.AudioMediaData"/> prior to the change</param>
        protected void NotifyMediaDataChanged(ManagedAudioMedia source, AudioMediaData newData, AudioMediaData prevData)
        {
            EventHandler<MediaDataChangedEventArgs> d = MediaDataChanged;
            if (d != null) d(this, new MediaDataChangedEventArgs(source, newData, prevData));
        }


        private void this_MediaDataChanged(object sender, MediaDataChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="audio.AudioMediaData"/> storing the audio of <c>this</c>
        /// </summary>
        /// <returns>The audio media data</returns>
        public AudioMediaData AudioMediaData
        {
            get
            {
                if (mAudioMediaData == null)
                {
                    //Lazy initialization
                    AudioMediaData = MediaDataFactory.CreateAudioMediaData();
                }
                return mAudioMediaData;
            }
            set
            {
                if (mAudioMediaData == value) return;
                if (mAudioMediaData != null) mAudioMediaData.Changed -= AudioMediaData_Changed;
                AudioMediaData prevData = mAudioMediaData;
                mAudioMediaData = value;
                if (mAudioMediaData !=null) mAudioMediaData.Changed += AudioMediaData_Changed;
                NotifyMediaDataChanged(this, mAudioMediaData, prevData);
            }
        }

        private void AudioMediaData_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
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
        public void MergeWith(ManagedAudioMedia other)
        {
            if (other == null)
            {
                throw new exception.MethodParameterIsNullException("Can not merge with a null ManagedAudioMedia");
            }
            AudioMediaData.MergeWith(other.AudioMediaData);
        }

        #region IValueEquatable<Media> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>		
        public override bool ValueEquals(Media other)
        {
            if (!base.ValueEquals(other)) return false;
            ManagedAudioMedia otherMAM = (ManagedAudioMedia) other;
            if (!AudioMediaData.ValueEquals(otherMAM.AudioMediaData)) return false;
            return true;
        }

        #endregion
    }
}