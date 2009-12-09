using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data.image
{
    public abstract class ImageMediaData: MediaData
    {
        private string m_ImageOriginalName;

        protected abstract override MediaData CopyProtected();

        public string OriginalFileName
            {
            get
                {
                return m_ImageOriginalName;
                }
            protected set
                {
                m_ImageOriginalName = value;
                }
            }


        public void AddImage ( string path )
            {
            if (string.IsNullOrEmpty ( path ))
                {
                throw new exception.MethodParameterIsNullException ( "The path of a png image can not be null" );
                }

            if (!File.Exists(path ))
                {
                throw new FileNotFoundException( "File not found at " + path);
                }

            AddImage ( 
                CreateDataProviderFromNewImage ( path),
                Path.GetFileName(path)  );
            }


        public abstract void AddImage ( DataProvider dataProv, string imageOriginalName );


        protected abstract DataProvider CreateDataProviderFromNewImage(string path) ;
        /*
        {            
            FileInfo mImgFile = new FileInfo(path);

            // We are not using derived class but switch case to avoid complexities.We will be implementing the classes
           //  in future if needed.
            switch (mImgFile.Extension)
            {
                case ".jpg":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_JPG_MIME_TYPE);
                        break;
                    }
                case ".png":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_PNG_MIME_TYPE);
                        break;
                    }
                case ".svg":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_SVG_MIME_TYPE);
                        break;
                    }
                default:
                        break;
            }
          
          FileStream originalImageStream = new FileStream ( path, FileMode.Open, FileAccess.Read );

            try
            {
               imgDataProvider.AppendData ( originalImageStream, originalImageStream.Length );
            }
            finally
            {
                originalImageStream.Close ();
                originalImageStream = null;
            }
            return imgDataProvider;
          }
         */ 
    }//Class
}//namespace
