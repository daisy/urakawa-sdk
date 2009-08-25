using System;
using System.Collections.Generic;
using System.Text;
using urakawa.events.media.data;

namespace urakawa.media.data.image
{

   public class ManagedImageMedia : AbstractImageMedia, IManaged
    {
        private ImageMediaData m_ImageMediaData;
        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.ManagedImageMedia;
        }
        private void Reset()
        {
           m_ImageMediaData = null;
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

        protected override Media CopyProtected()
        {
            throw new NotImplementedException();
        }
        protected override Media ExportProtected(Presentation destPres)
        {
            throw new NotImplementedException();
        }
        protected override void Clear()
        {
            throw new NotImplementedException();
        }
        public override void SetSize(int newHeight, int newWidth)
        {
            Height = newHeight;
            Width = newWidth;
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

       public MediaData MediaData
       {
           get
           { return m_ImageMediaData; }
           set
           {
               if (value is ImageMediaData)
                   m_ImageMediaData = value as ImageMediaData;
           }
       }
  
       public event EventHandler<MediaDataChangedEventArgs> MediaDataChanged;
     }
}
