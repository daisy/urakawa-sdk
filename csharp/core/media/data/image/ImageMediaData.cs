using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data.image
{
    public abstract class ImageMediaData: MediaData
    {
        protected abstract override MediaData CopyProtected();
        DataProvider imgDataProvider;

        protected DataProvider CreateDataProviderFromNewImage(string path)
        {            
            FileInfo mImgFile = new FileInfo(path);

            // We are not using derived class but switch case to avoid complexities.We will be implementing the classes
           //  in future if needed.
            switch (mImgFile.Extension)
            {
                case ".jpg":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_JPG_MIME_TYPE);
                        return imgDataProvider;
                        break;
                    }
                case ".png":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_PNG_MIME_TYPE);
                        return imgDataProvider;
                        break;
                    }
                case ".svg":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_SVG_MIME_TYPE);
                        return imgDataProvider;
                        break;
                    }
                default:
                    return imgDataProvider;
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
          }
    }//Class
}//namespace
