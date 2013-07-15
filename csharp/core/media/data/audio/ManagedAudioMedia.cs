using System;
using System.Xml;
using System.IO;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.events.media.data;
using urakawa.xuk;

namespace urakawa.media.data.audio
{
    [XukNameUglyPrettyAttribute("mAu", "ManagedAudioMedia")]
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
        /// Exports the external audio media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external audio media</returns>
        public new ManagedAudioMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ManagedAudioMedia;
        }

        /// <summary>
        /// Gets a copy of the <see cref="Media"/>
        /// </summary>
        /// <returns>The copy</returns>
        protected override Media CopyProtected()
        {
            ManagedAudioMedia cp = (ManagedAudioMedia)base.CopyProtected();
            if (HasActualAudioMediaData)
            {
                cp.AudioMediaData = AudioMediaData.Copy();
            }
            return cp;
        }

        /// <summary>
        /// Exports the external audio media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external audio media</returns>
        protected override Media ExportProtected(Presentation destPres)
        {
            ManagedAudioMedia exported = (ManagedAudioMedia)base.ExportProtected(destPres);

            if (HasActualAudioMediaData)
            {
                exported.AudioMediaData = AudioMediaData.Export(destPres) as AudioMediaData;
            }
            
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
            base.XukInAttributes(source);

            string uid = source.GetAttribute(XukStrings.MediaDataUid);
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.XukException("MediaDataUid attribute is missing from AudioMediaData");
            }
            //if (!Presentation.MediaDataManager.IsManagerOf(uid))
            //{
            //    throw new exception.XukException(String.Format(
            //                                         "The MediaDataManager does not manage a AudioMediaData with uid {0}",
            //                                         uid));
            //}
            
            MediaData md = Presentation.MediaDataManager.GetManagedObject(uid);
            if (!(md is AudioMediaData))
            {
                throw new exception.XukException(String.Format(
                                                     "The AudioMediaData with uid {0} is a {1} which is not a urakawa.media.data.audio.AudioMediaData",
                                                     uid, md.GetType().FullName));
            }
            AudioMediaData = md as AudioMediaData;
            
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
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.MediaDataUid, AudioMediaData.Uid);
            
        }

        #endregion

        #region IContinuous Members

        /// <summary>
        /// Gets the duration of <c>this</c>, that is the duration of the underlying <see cref="audio.AudioMediaData"/>
        /// </summary>
        /// <returns>The duration</returns>
        public override Time Duration
        {
            get
            {
                if (HasActualAudioMediaData)
                {
                    return AudioMediaData.AudioDuration;
                }
                return new Time();
            }
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
            ManagedAudioMedia secondPartMAM = Presentation.MediaFactory.Create<ManagedAudioMedia>();

            if (HasActualAudioMediaData)
            {
                AudioMediaData secondPartData = AudioMediaData.Split(splitPoint);
                secondPartMAM.AudioMediaData = secondPartData;
            }

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
            get
            {
                if (HasActualAudioMediaData)
                {
                    return AudioMediaData;
                }
                return null;
            }
            set
            {
                if (!(value is AudioMediaData))
                {
                    throw new exception.MethodParameterIsWrongTypeException(
                        "The MediaData of a ManagedAudioMedia must be a AudioMediaData");
                }

                AudioMediaData = value as AudioMediaData;
            }
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
        /// The AudioMediaData getter lazily instanciates an AudioMediaData object,
        /// so this method can be used to test whether this ManagedAudioMedia is actually empty.
        /// </summary>
        /// <returns></returns>
        public bool HasActualAudioMediaData
        {
            get
            {
                return mAudioMediaData != null;
            }
        }

        public bool IsWavAudioMediaData
        {
            get
            {
                return HasActualAudioMediaData && mAudioMediaData is WavAudioMediaData;
            }
        }

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
                    AudioMediaData = Presentation.MediaDataFactory.CreateAudioMediaData();
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

        #region IValueEquatable<Media> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ManagedAudioMedia otherz = other as ManagedAudioMedia;
            if (otherz == null)
            {
                return false;
            }

            if (otherz.HasActualAudioMediaData != HasActualAudioMediaData)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (HasActualAudioMediaData && !AudioMediaData.ValueEquals(otherz.AudioMediaData))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            return true;
        }

        #endregion
    }
}