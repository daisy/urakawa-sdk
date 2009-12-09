using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace urakawa.media.data.image
    {
    public class BmpImageMediaData : ImageMediaData
        {
        private DataProvider m_ImageDataProvider;
        
protected override MediaData CopyProtected ()
            {
            ImageMediaData copyImageMediaData = Presentation.MediaDataFactory.Create<BmpImageMediaData> ();
            copyImageMediaData.AddImage ( m_ImageDataProvider, OriginalFileName );
            return copyImageMediaData;
            }

        protected override MediaData ExportProtected ( Presentation destPres )
            {
            PngImageMediaData expImgData = Presentation.MediaDataFactory.Create<PngImageMediaData> ();
            expImgData.AddImage (
                m_ImageDataProvider.Export ( destPres ),
                OriginalFileName);
            return expImgData;
            }

        public override IEnumerable<DataProvider> UsedDataProviders
            {
            get
                {
                List<DataProvider> usedDP = new List<DataProvider> ( 1 );
                usedDP[0] = m_ImageDataProvider;
                return usedDP;
                }
            }

        public override string GetTypeNameFormatted ()
            {
            return xuk.XukStrings.BmpImageMediaData;
            }



        public override void AddImage ( DataProvider dataProv, string imgOriginalName )
            {
            if (dataProv == null)
                {
                throw new exception.MethodParameterIsNullException ( "The data provider of a bmp image can not be null" );
                }


            if (dataProv.MimeType != DataProviderFactory.IMAGE_BMP_MIME_TYPE)
                {
                throw new exception.OperationNotValidException (
                    "The mime type of the given DataProvider is not BMP!" );
                }

            if (string.IsNullOrEmpty ( imgOriginalName ))
                {
                throw new exception.MethodParameterIsEmptyStringException ( "original name of image cannot be null!" );
                }

            m_ImageDataProvider = dataProv;
            base.OriginalFileName = imgOriginalName;
            }



        protected override DataProvider CreateDataProviderFromNewImage ( string path )
            {
            DataProvider imgDataProvider = Presentation.DataProviderFactory.Create ( DataProviderFactory.IMAGE_BMP_MIME_TYPE );
            ((FileDataProvider)imgDataProvider).InitByCopyingExistingFile ( path );

            return imgDataProvider;
            }




        }
    }
