using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using urakawa.events.media.data;
using urakawa.xuk;


namespace urakawa.media.data.Image
{
    public class ManagedImageMedia : AbstractImageMedia, IManaged
    {
        public override string GetTypeNameFormatted()
        {
            throw new NotImplementedException();           
        }
        public override bool IsContinuous
        {
            get
            {
                throw new NotImplementedException();
            }            
        }
        public override bool IsDiscrete
        {
            get
            {
                throw new NotImplementedException();
            }            
        }
        public override bool IsSequence
        {
            get
            {
                throw new NotImplementedException();
            }            
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
            get { 
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
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<MediaDataChangedEventArgs> MediaDataChanged;

   }//class
}//namespace