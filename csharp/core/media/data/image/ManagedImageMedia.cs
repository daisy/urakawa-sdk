using System;
using System.Xml;
using urakawa.events.media.data;
using urakawa.media.data.audio;
using urakawa.xuk;

namespace urakawa.media.data.image
{
    [XukNameUglyPrettyAttribute("mIm", "ManagedImageMedia")]
    public class ManagedImageMedia : AbstractImageMedia, IManaged
    {
        public ManagedImageMedia()
        {
            Reset();
            MediaDataChanged += this_MediaDataChanged;
        }

        private void this_MediaDataChanged(object sender, MediaDataChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        protected void NotifyMediaDataChanged(ManagedImageMedia source, ImageMediaData newData, ImageMediaData prevData)
        {
            EventHandler<MediaDataChangedEventArgs> d = MediaDataChanged;
            if (d != null) d(this, new MediaDataChangedEventArgs(source, newData, prevData));
        }
        
        private ImageMediaData m_ImageMediaData;

        private void Reset()
        {
            m_ImageMediaData = null;
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

        public new ManagedImageMedia Copy()
        {
            return CopyProtected() as ManagedImageMedia;
        }

        public new ManagedImageMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ManagedImageMedia;
        }

        protected override Media CopyProtected()
        {
            ManagedImageMedia cp = (ManagedImageMedia)base.CopyProtected();
            cp.ImageMediaData = ImageMediaData.Copy();
            return cp;
        }

        protected override Media ExportProtected(Presentation destPres)
        {
            ManagedImageMedia exported = (ManagedImageMedia)base.ExportProtected(destPres);
            exported.ImageMediaData = ImageMediaData.Export(destPres);
            return exported;
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

        public ImageMediaData ImageMediaData
        {
            get
            {
                if (m_ImageMediaData == null)
                {
                    //Lazy initialization
                    ImageMediaData = Presentation.MediaDataFactory.CreateImageMediaData();
                }
                return m_ImageMediaData;
            }

            set
            {
                if (m_ImageMediaData == value) return;

                if (m_ImageMediaData != null)
                {
                    m_ImageMediaData.Changed -= ImageMediaData_Changed;
                }
                ImageMediaData prevData = m_ImageMediaData;

                m_ImageMediaData = value;

                if (m_ImageMediaData != null)
                {
                    m_ImageMediaData.Changed += ImageMediaData_Changed;
                }

                NotifyMediaDataChanged(this, m_ImageMediaData, prevData);
            }
        }

        private void ImageMediaData_Changed(object sender, events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        public MediaData MediaData
        {
            get
            {
                return ImageMediaData;
            }
            set
            {
                if (!(value is ImageMediaData))
                {
                    throw new exception.MethodParameterIsWrongTypeException(
                        "The MediaData of a ManagedImageMedia must be a ImageMediaData");
                }

                ImageMediaData = value as ImageMediaData;
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

            ManagedImageMedia otherz = other as ManagedImageMedia;
            if (otherz == null)
            {
                return false;
            }

            if (!ImageMediaData.ValueEquals(otherz.ImageMediaData))
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
                throw new exception.XukException("MediaDataUid attribute is missing from ImageMediaData");
            }
            //if (!Presentation.MediaDataManager.IsManagerOf(uid))
            //{
            //    throw new exception.IsNotManagerOfException(String.Format(
            //                                         "The MediaDataManager does not mamage a ImageMediaData with uid {0}",
            //                                         uid));
            //}
            MediaData md = Presentation.MediaDataManager.GetManagedObject(uid);
            if (!(md is ImageMediaData))
            {
                throw new exception.XukException(String.Format(
                                                     "The MediaData with uid {0} is a {1} which is not a ImageMediaData",
                                                     uid, md.GetType().FullName));
            }
            ImageMediaData = md as ImageMediaData;
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.MediaDataUid, ImageMediaData.Uid);
        }
    }
}
