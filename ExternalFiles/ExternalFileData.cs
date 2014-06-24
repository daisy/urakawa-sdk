using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.data;
using urakawa.exception;
using urakawa.media.data;
using urakawa.xuk;

namespace urakawa.ExternalFiles
{
    [XukNameUglyPrettyAttribute("GenExFl", "GenericExternalFileData")]
    public class GenericExternalFileData : ExternalFileData
    {
        public override string MimeType
        {
            get
            {
                string mime = null;
                if (!string.IsNullOrEmpty(OriginalRelativePath))
                {
                    mime = DataProviderFactory.GetMimeTypeFromExtension(Path.GetExtension(OriginalRelativePath));
                }
                if (string.IsNullOrEmpty(mime))
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    return "N/A";
                }
                return mime;
            }
        }

    

    }

    public abstract class ExternalFileData : WithPresentation
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


        private bool m_PreserveForOutputFile;
        public bool IsPreservedForOutputFile
        {
            get
            {
                return m_PreserveForOutputFile;
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
                    //if (m_OriginalRelativePath.StartsWith(@"./"))
                    //{
                    //    m_OriginalRelativePath = m_OriginalRelativePath.Substring(2);
                    //}
                }
            }
        }

        private string m_OptionalInfo;
        public string OptionalInfo
        {
            get
            {
                return m_OptionalInfo;
            }
            private set
            {
                m_OptionalInfo = value;
            }
        }

        public IEnumerable<DataProvider> UsedDataProviders
        {
            get
            {
                yield return m_DataProvider;
                yield break;
            }
        }



        public new ExternalFileData Copy()
        {
            return CopyProtected() as ExternalFileData;
        }

        public new ExternalFileData Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ExternalFileData;
        }

        protected ExternalFileData CopyProtected()
        {
            ExternalFileData fileDataCopy = (ExternalFileData)Presentation.ExternalFilesDataFactory.Create(GetType());
            // We do not Copy the FileDataProvider,
            // because the file stream is read-only by design.
            fileDataCopy.InitializeWithData(m_DataProvider, OriginalRelativePath, IsPreservedForOutputFile, OptionalInfo);
            return fileDataCopy;
        }

        protected ExternalFileData ExportProtected(Presentation destPres)
        {
            ExternalFileData exportFileData = (ExternalFileData)Presentation.ExternalFilesDataFactory.Create(GetType());
            exportFileData.InitializeWithData(m_DataProvider.Export(destPres), OriginalRelativePath, IsPreservedForOutputFile, OptionalInfo);
            return exportFileData;
        }


        public void InitializeWithData(string path, string originalRelativePath, bool preserveForOutput, string optionalInfo)
        {
            OptionalInfo = optionalInfo;
            OriginalRelativePath = originalRelativePath;
            m_PreserveForOutputFile = preserveForOutput;

            if (string.IsNullOrEmpty(path))
            {
                throw new MethodParameterIsNullException("The path of file can not be null");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found at " + path);
            }


            if (preserveForOutput && string.IsNullOrEmpty(originalRelativePath))
            {
                throw new MethodParameterIsEmptyStringException("For preserving file for output, original name of file cannot be null!");
            }

            DataProvider externalFileDataProvider = Presentation.DataProviderFactory.Create(MimeType);
            ((FileDataProvider)externalFileDataProvider).InitByCopyingExistingFile(path);

            InitializeWithData(externalFileDataProvider, originalRelativePath, preserveForOutput, optionalInfo);
        }

        public void InitializeWithData(Stream dataStream, string originalRelativePath, bool preserveForOutput, string optionalInfo)
        {
            OptionalInfo = optionalInfo;
            OriginalRelativePath = originalRelativePath;
            m_PreserveForOutputFile = preserveForOutput;

            if (dataStream == null)
            {
                throw new MethodParameterIsNullException("The dataStream parameter is null");
            }

            if (dataStream.Length == 0)
            {
                throw new exception.InputStreamIsTooShortException("Input dataStream contains no data!");
            }

            if (preserveForOutput && string.IsNullOrEmpty(originalRelativePath))
            {
                throw new MethodParameterIsEmptyStringException("For preserving file for output, original name of file cannot be null!");
            }

            DataProvider externalFileDataProvider = Presentation.DataProviderFactory.Create(MimeType);
            externalFileDataProvider.AppendData(dataStream, dataStream.Length);

            InitializeWithData(externalFileDataProvider, originalRelativePath, preserveForOutput, optionalInfo);
        }


        public void InitializeWithData(DataProvider dataProv, string originalRelativePath, bool preserveForOutput, string optionalInfo)
        {
            OptionalInfo = optionalInfo;
            OriginalRelativePath = originalRelativePath;
            m_PreserveForOutputFile = preserveForOutput;

            if (dataProv == null)
            {
                throw new MethodParameterIsNullException("The data provider can not be null");
            }

            if (dataProv.MimeType != MimeType)
            {
                throw new OperationNotValidException("The mime type of the given DataProvider is not correct !");
            }

            if (preserveForOutput && string.IsNullOrEmpty(originalRelativePath))
            {
                throw new MethodParameterIsEmptyStringException("For preserving file for output, original name of file cannot be null!");
            }

            DataProvider = dataProv;
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ExternalFileData otherz = other as ExternalFileData;
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

        public virtual void Delete()
        {
            Presentation.ExternalFilesDataManager.RemoveManagedObject(this);
        }

        public Stream OpenInputStream()
        {
            if (m_DataProvider == null)
            {
                throw new exception.IsNotInitializedException("ExternalFileData is not initialized with a file!");
            }


            return m_DataProvider.OpenInputStream();
        }


        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (IsPreservedForOutputFile && String.IsNullOrEmpty(OriginalRelativePath))
            {
                throw new XukException("For preserving files for output, the OriginalRelativePath of an ExternalFileData cannot be null or empty !");
            }

            if (OriginalRelativePath == null) OriginalRelativePath = "";
            destination.WriteAttributeString(XukStrings.OriginalRelativePath, OriginalRelativePath);

            if (OptionalInfo == null) OptionalInfo = "";
            destination.WriteAttributeString(XukStrings.OptionalInfo, OptionalInfo);


            destination.WriteAttributeString(XukStrings.IsPreservedForOutputFile,
                IsPreservedForOutputFile == true ? "true" : "false");

            if (DataProvider == null || String.IsNullOrEmpty(DataProvider.Uid))
            {
                throw new XukException("The DataProvider of an ExternalFileData cannot be null or empty !");
            }
            destination.WriteAttributeString(DataProvider.DataProvider_NAME.z(PrettyFormat), DataProvider.Uid);


        }

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string optionalInfo = source.GetAttribute(XukStrings.OptionalInfo);

            string strPreserve = source.GetAttribute(XukStrings.IsPreservedForOutputFile);
            bool isPreserved = strPreserve == "true" ? true : false;
            string path = source.GetAttribute(XukStrings.OriginalRelativePath);

            if (isPreserved && String.IsNullOrEmpty(path))
            {
                throw new XukException("For preserved files, the OriginalRelativePath of an ExternalFileData cannot be null or empty !");
            }

            string uid = XukAble.ReadXukAttribute(source, DataProvider.DataProvider_NAME);
            if (String.IsNullOrEmpty(uid))
            {
                throw new XukException("The DataProvider of an ExternalFileData cannot be null or empty !");
            }

            //if (!Presentation.DataProviderManager.IsManagerOf(uid))
            //{
            //    throw new IsNotManagerOfException(
            //            String.Format("DataProvider cannot be found {0}", uid));
            //}
            DataProvider prov = Presentation.DataProviderManager.GetManagedObject(uid);

            InitializeWithData(prov, path, isPreserved, optionalInfo);
        }
    }
}
