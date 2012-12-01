using System;
using System.Xml;
using urakawa.events.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.media.data.video
{
    [XukNameUglyPrettyAttribute("mVd", "ManagedVideoMedia")]
    public class ManagedVideoMedia : AbstractVideoMedia, IManaged
    {
        public override Time Duration
        {
            get { throw new NotImplementedException(); }
        }

        protected override AbstractVideoMedia SplitProtected(Time splitPoint)
        {
            throw new NotImplementedException();
        }

        public override int Width
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public override int Height
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }



        public ManagedVideoMedia()
        {
            Reset();
            MediaDataChanged += this_MediaDataChanged;
        }

        private void this_MediaDataChanged(object sender, MediaDataChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        protected void NotifyMediaDataChanged(ManagedVideoMedia source, VideoMediaData newData, VideoMediaData prevData)
        {
            EventHandler<MediaDataChangedEventArgs> d = MediaDataChanged;
            if (d != null) d(this, new MediaDataChangedEventArgs(source, newData, prevData));
        }
        
        private VideoMediaData m_VideoMediaData;

        private void Reset()
        {
            m_VideoMediaData = null;
        }

        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        public override bool IsContinuous
        {
            get { return false; }
        }
        public override bool IsDiscrete
        {
            get { return true; }
        }
        public override bool IsSequence
        {
            get { return false; }
        }

        public new ManagedVideoMedia Copy()
        {
            return CopyProtected() as ManagedVideoMedia;
        }

        public new ManagedVideoMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ManagedVideoMedia;
        }

        protected override Media CopyProtected()
        {
            ManagedVideoMedia cp = (ManagedVideoMedia)base.CopyProtected();
            cp.VideoMediaData = VideoMediaData.Copy();
            return cp;
        }

        protected override Media ExportProtected(Presentation destPres)
        {
            ManagedVideoMedia exported = (ManagedVideoMedia)base.ExportProtected(destPres);
            exported.VideoMediaData = VideoMediaData.Export(destPres);
            return exported;
        }

        public VideoMediaData VideoMediaData
        {
            get
            {
                if (m_VideoMediaData == null)
                {
                    //Lazy initialization
                    VideoMediaData = Presentation.MediaDataFactory.CreateVideoMediaData();
                }
                return m_VideoMediaData;
            }

            set
            {
                if (m_VideoMediaData == value) return;

                if (m_VideoMediaData != null)
                {
                    m_VideoMediaData.Changed -= VideoMediaData_Changed;
                }
                VideoMediaData prevData = m_VideoMediaData;

                m_VideoMediaData = value;

                if (m_VideoMediaData != null)
                {
                    m_VideoMediaData.Changed += VideoMediaData_Changed;
                }

                NotifyMediaDataChanged(this, m_VideoMediaData, prevData);
            }
        }

        private void VideoMediaData_Changed(object sender, events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        public MediaData MediaData
        {
            get
            {
                return VideoMediaData;
            }
            set
            {
                if (!(value is VideoMediaData))
                {
                    throw new exception.MethodParameterIsWrongTypeException(
                        "The MediaData of a ManagedVideoMedia must be a VideoMediaData");
                }

                VideoMediaData = value as VideoMediaData;
            }
        }

        public event EventHandler<MediaDataChangedEventArgs> MediaDataChanged;


        #region IValueEquatable<Media> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ManagedVideoMedia otherz = other as ManagedVideoMedia;
            if (otherz == null)
            {
                return false;
            }

            if (!VideoMediaData.ValueEquals(otherz.VideoMediaData))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            return true;
        }

        #endregion


        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string uid = source.GetAttribute(XukStrings.MediaDataUid);
            if (string.IsNullOrEmpty(uid))
            {
                throw new exception.XukException("MediaDataUid attribute is missing from VideoMediaData");
            }
            //if (!Presentation.MediaDataManager.IsManagerOf(uid))
            //{
            //    throw new exception.IsNotManagerOfException(String.Format(
            //                                         "The MediaDataManager does not mamage a VideoMediaData with uid {0}",
            //                                         uid));
            //}
            MediaData md = Presentation.MediaDataManager.GetManagedObject(uid);
            if (!(md is VideoMediaData))
            {
                throw new exception.XukException(String.Format(
                                                     "The MediaData with uid {0} is a {1} which is not a VideoMediaData",
                                                     uid, md.GetType().FullName));
            }
            VideoMediaData = md as VideoMediaData;
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.MediaDataUid, VideoMediaData.Uid);
        }
    }
}
