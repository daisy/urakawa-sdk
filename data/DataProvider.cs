using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa.data
{
    /// <summary>
    /// Interface for a generic <see cref="DataProvider"/> providing access to data storage 
    /// via input and output <see cref="Stream"/>s
    /// </summary>
    public abstract class DataProvider : WithPresentation
    {
        public static readonly UglyPrettyName DataProvider_NAME = new UglyPrettyName("dtPrv", "DataProvider");

        private Object m_AppData = null;
        public Object AppData
        {
            get { return m_AppData; }
            set { m_AppData = value; }
        }

        private string mMimeType;

        private void Reset()
        {
            mMimeType = null;

        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="DataProvider"/>s should only be created via. the <see cref="DataProviderFactory"/>
        /// </summary>
        protected DataProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the UID of the data provider in the context of the manager. 
        /// Convenience for <c>DataProviderManager.GetUidOfDataProvider(this)</c>
        /// </summary>
        //public override string Uid
        //{
        //    set { throw new NotImplementedException(); }
        //    get { return Presentation.DataProviderManager.GetUidOfDataProvider(this); }
        //}


        /// <summary>
        /// Appends data from a given input <see cref="Stream"/>
        /// </summary>
        /// <param name="data">The given input stream</param>
        /// <param name="count">The number of bytes to append</param>
        public void AppendData(Stream data, long count)
        {
            if (count <= 0)
            {
                return;
            }

            if (count > (data.Length - data.Position))
            {
                throw new exception.InputStreamIsTooShortException(
                    String.Format("The given data Stream is shorter than the requested {0:0} bytes",
                                  count));
            }

            Stream provOutputStream = OpenOutputStream();
            try
            {
                provOutputStream.Seek(0, SeekOrigin.End);

                const uint BUFFER_SIZE = 1024 * 300; // 300 KB MAX BUFFER
                StreamUtils.Copy(data, (ulong)count, provOutputStream, BUFFER_SIZE);

                //if (count <= BUFFER_SIZE)
                //{
                //    byte[] buffer = new byte[count];
                //    int bytesRead = data.Read(buffer, 0, (int)count);
                //    if (bytesRead > 0)
                //    {
                //        provOutputStream.Write(buffer, 0, bytesRead);
                //    }
                //    else
                //    {
                //        throw new exception.InputStreamIsTooShortException(
                //            String.Format("Can not read {0:0} bytes from the given data Stream",
                //                          count));
                //    }
                //}
                //else
                //{
                //    int bytesRead = 0;
                //    int totalBytesWritten = 0;
                //    byte[] buffer = new byte[BUFFER_SIZE];

                //    while ((bytesRead = data.Read(buffer, 0, (int)BUFFER_SIZE)) > 0)
                //    {
                //        if ((totalBytesWritten + bytesRead) > count)
                //        {
                //            int bytesToWrite = (int)(count - totalBytesWritten);
                //            provOutputStream.Write(buffer, 0, bytesToWrite);
                //            totalBytesWritten += bytesToWrite;
                //        }
                //        else
                //        {
                //            provOutputStream.Write(buffer, 0, bytesRead);
                //            totalBytesWritten += bytesRead;
                //        }
                //    }
                //}
            }
            finally
            {
                provOutputStream.Close();
            }
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> providing read access to the data
        /// </summary>
        /// <returns>The input <see cref="Stream"/></returns>
        /// <exception cref="exception.DataMissingException">
        /// Thrown if the data stored in the <see cref="DataProvider"/> is missing from the underlying storage mechanism
        /// </exception>
        /// <remarks>
        /// Make sure to close any <see cref="Stream"/> returned by this method when it is no longer needed. 
        /// If there are any open input <see cref="Stream"/>s, subsequent calls to methods
        /// <see cref="OpenOutputStream"/> and <see cref="Delete"/> will cause <see cref="exception.InputStreamsOpenException"/>s
        /// </remarks>
        public abstract Stream OpenInputStream();

        /// <summary>
        /// Gets a <see cref="Stream"/> providing write access to the data
        /// </summary>
        /// <returns>The output <see cref="Stream"/></returns>
        /// <exception cref="exception.DataMissingException">
        /// Thrown if the data stored in the <see cref="DataProvider"/> is missing from the underlying storage mechanism
        /// </exception>
        /// <exception cref="exception.OutputStreamOpenException">
        /// Thrown if another output <see cref="Stream"/> from the data provider is already/still open
        /// </exception>
        /// <remarks>
        /// Make sure to close any <see cref="Stream"/> returned by this method when it is no longer needed. 
        /// If there are any open input <see cref="Stream"/>s, subsequent calls to methods
        /// <see cref="OpenOutputStream"/> and <see cref="OpenInputStream"/> and <see cref="Delete"/> 
        /// will cause <see cref="exception.OutputStreamOpenException"/>s
        /// </remarks>
        public abstract Stream OpenOutputStream();

        /// <summary>
        /// Deletes any resources associated with <c>this</c> permanently. Additionally removes the <see cref="DataProvider"/>
        /// from it's <see cref="DataProviderManager"/>
        /// </summary>
        /// <exception cref="exception.OutputStreamOpenException">
        /// Thrown if a output <see cref="Stream"/> from the <see cref="DataProvider"/> is currently open
        /// </exception>
        /// <exception cref="exception.InputStreamsOpenException">
        /// Thrown if one or more input <see cref="Stream"/>s from the <see cref="DataProvider"/> are currently open
        /// </exception>
        public abstract void Delete();

        /// <summary>
        /// Creates a copy of <c>this</c> including a copy of the data
        /// </summary>
        /// <returns>The copy</returns>
        public abstract DataProvider Copy();

        /// <summary>
        /// Exports <c>this</c> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="DataProvider"/></returns>
        public abstract DataProvider Export(Presentation destPres);

        /// <summary>
        /// Writes data from data provider stream to a file external to project
        /// </summary>
        /// <param name="exportFilePath"></param>
        /// <param name="canOverwrite"></param>
        public virtual void ExportDataStreamToFile(string exportFilePath, bool canOverwrite)
        {
            if (exportFilePath == null)
            {
                throw new exception.MethodParameterIsNullException("external file path cannot be null");
            }

            if (File.Exists(exportFilePath) && !canOverwrite)
            {
                throw new System.Exception("Export file with same name already exists");
            }

            Stream source = OpenInputStream();

            if (source.Length == 0)
            {
                source.Close();
                throw new exception.InputStreamIsTooShortException("The data provider has no data to export to external file");
            }

            const uint BUFFER_SIZE = 1024 * 1024; // 1 MB MAX BUFFER

            string parentdir = Path.GetDirectoryName(exportFilePath);
            if (!Directory.Exists(parentdir))
            {
                FileDataProvider.CreateDirectory(parentdir);
            }

            FileStream exportFileStream = null;
            try
            {
                //exportFileStream = File.Create(exportFilePath);
                exportFileStream = new FileStream(exportFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            catch(Exception ex)
            {
                source.Close();

#if DEBUG
                Debugger.Break();
#endif
                throw;
            }

            try
            {
                StreamUtils.Copy(source, 0, exportFileStream, BUFFER_SIZE);

                //if (source.Length <= BUFFER_SIZE)
                //{
                //    byte[] buffer = new byte[source.Length];
                //    int read = source.Read(buffer, 0, (int)source.Length);
                //    exportFileStream.Write(buffer, 0, read);
                //}
                //else
                //{
                //    byte[] buffer = new byte[BUFFER_SIZE];
                //    int bytesRead = 0;
                //    while ((bytesRead = source.Read(buffer, 0, BUFFER_SIZE)) > 0)
                //    {
                //        exportFileStream.Write(buffer, 0, bytesRead);
                //    }

                //}
            }
            finally
            {
                exportFileStream.Close();
                source.Close();
            }
        }


        /// <summary>
        /// Gets or sets the MIME type of the media stored in the data provider
        /// </summary>
        /// <exception cref="IsNotInitializedException">
        /// Thrown when trying to get the <see cref="MimeType"/> before it has been initialized
        /// </exception>
        /// <exception cref="MethodParameterIsNullException">
        /// Thrown when trying to set the <see cref="MimeType"/> to <c>null</c>
        /// </exception>
        public string MimeType
        {
            get
            {
                if (mMimeType == null) throw new IsNotInitializedException("The DataProvider has not been initialized with a MimeType");
                return mMimeType;
            }

            set
            {
                if (value == null)
                {
                    throw new MethodParameterIsNullException("The MimeType cannot be null");
                }
                mMimeType = value;
            }
        }

        #region IXukAble members

        /// <summary>
        /// Clears the <see cref="XukAble"/> of any data - called at the beginning of <see cref="XukAble.XukIn"/>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a XukAble xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            MimeType = source.GetAttribute(XukStrings.MimeType) ?? "";

        }

        /// <summary>
        /// Writes the attributes of a XukAble element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.MimeType, MimeType);

        }

        #endregion
    }
}