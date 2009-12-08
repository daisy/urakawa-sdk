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
        private string m_ImageOriginalName;

        protected override MediaData CopyProtected ()
            {
            throw new NotImplementedException ();
            }

        protected override MediaData ExportProtected ( Presentation destPres )
            {
            throw new NotImplementedException ();
            }

        public override IEnumerable<DataProvider> UsedDataProviders
            {
            get
                {
                throw new NotImplementedException ();
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
            base.ImageOriginalName = imgOriginalName;
            }



        protected override DataProvider CreateDataProviderFromNewImage ( string path )
            {
            DataProvider imgDataProvider = Presentation.DataProviderFactory.Create ( DataProviderFactory.IMAGE_BMP_MIME_TYPE );
            ((FileDataProvider)imgDataProvider).InitByCopyingExistingFile ( path );

            return imgDataProvider;
            }




        }
    }
