using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.data;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa.media.data.video
{
    public abstract class VideoMediaData : MediaData
    {
        public abstract string MimeType { get; }

        private DataProvider m_DataProvider;
        public DataProvider DataProvider
        {
            get
            {
                return m_DataProvider;
            }
            private set
            {
                m_DataProvider = value;
            }
        }

        public override IEnumerable<DataProvider> UsedDataProviders
        {
            get
            {
                yield return m_DataProvider;
                yield break;
            }
        }

        private string m_OriginalRelativePath;
        public string OriginalRelativePath
        {
            get
            {
                return m_OriginalRelativePath;
            }
            private set
            {
                m_OriginalRelativePath = value;
                if (m_OriginalRelativePath != null)
                {
                    m_OriginalRelativePath = m_OriginalRelativePath.Replace('\\', '/');
                }
            }
        }

        public new VideoMediaData Copy()
        {
            return CopyProtected() as VideoMediaData;
        }

        public new VideoMediaData Export(Presentation destPres)
        {
            return ExportProtected(destPres) as VideoMediaData;
        }

        protected override MediaData CopyProtected()
        {
            VideoMediaData copyVideoMediaData = (VideoMediaData)Presentation.MediaDataFactory.Create(GetType());
            // We do not Copy the FileDataProvider,
            // it is shared amongst VideoMediaData instances without concurrent access problems
            // because the file stream is read-only by design.
            copyVideoMediaData.InitializeVideo(m_DataProvider, OriginalRelativePath);
            return copyVideoMediaData;
        }

        protected override MediaData ExportProtected(Presentation destPres)
        {
            VideoMediaData expImgData = (VideoMediaData)destPres.MediaDataFactory.Create(GetType());
            expImgData.InitializeVideo(m_DataProvider.Export(destPres), OriginalRelativePath);
            return expImgData;
        }


        public void InitializeVideo(string path, string originalRelativePath)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new MethodParameterIsNullException("The path of a Video can not be null");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found at " + path);
            }

            DataProvider imgDataProvider = Presentation.DataProviderFactory.Create(MimeType);
            ((FileDataProvider)imgDataProvider).InitByCopyingExistingFile(path);

            InitializeVideo(imgDataProvider, originalRelativePath);
        }

        public void InitializeVideo(DataProvider dataProv, string imgOriginalName)
        {
            if (dataProv == null)
            {
                throw new MethodParameterIsNullException("The data provider of a Video can not be null");
            }

            if (dataProv.MimeType != MimeType)
            {
                throw new OperationNotValidException("The mime type of the given DataProvider is not correct !");
            }

            if (string.IsNullOrEmpty(imgOriginalName))
            {
                throw new MethodParameterIsEmptyStringException("original name of Video cannot be null!");
            }

            DataProvider = dataProv;
            OriginalRelativePath = imgOriginalName;
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            VideoMediaData otherz = other as VideoMediaData;
            if (otherz == null)
            {
                return false;
            }

            if (OriginalRelativePath != otherz.OriginalRelativePath)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            if (!DataProvider.ValueEquals(otherz.DataProvider))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            return true;
        }

        public Stream OpenInputStream()
        {
            if (m_DataProvider == null)
            {
                throw new exception.IsNotInitializedException("VideoMediaData is not initialized with an Video!");
            }


            return m_DataProvider.OpenInputStream();
        }



        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (String.IsNullOrEmpty(OriginalRelativePath))
            {
                throw new XukException("The OriginalRelativePath of an VideoMediaData cannot be null or empty !");
            }
            destination.WriteAttributeString(XukStrings.OriginalRelativePath, OriginalRelativePath);

            if (DataProvider == null || String.IsNullOrEmpty(DataProvider.Uid))
            {
                throw new XukException("The DataProvider of an VideoMediaData cannot be null or empty !");
            }
            destination.WriteAttributeString(DataProvider.DataProvider_NAME.z(PrettyFormat), DataProvider.Uid);

            //if (!Presentation.Project.PrettyFormat)
            //{
            //    //destination.WriteAttributeString(XukStrings.Uid, Uid);
            //}
        }

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string path = source.GetAttribute(XukStrings.OriginalRelativePath);
            if (String.IsNullOrEmpty(path))
            {
                throw new XukException("The OriginalRelativePath of an VideoMediaData cannot be null or empty !");
            }

            string uid = XukAble.ReadXukAttribute(source, DataProvider.DataProvider_NAME);
            if (String.IsNullOrEmpty(uid))
            {
                throw new XukException("The DataProvider of an VideoMediaData cannot be null or empty !");
            }

            //if (!Presentation.DataProviderManager.IsManagerOf(uid))
            //{
            //    throw new IsNotManagerOfException(
            //            String.Format("DataProvider cannot be found {0}", uid));
            //}
            DataProvider prov = Presentation.DataProviderManager.GetManagedObject(uid);

            InitializeVideo(prov, path);
        }

    }//Class
}//namespace
