using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using urakawa.xuk;

namespace urakawa.data
{
    /// <summary>
    /// Implementation of interface <see cref="DataProvider"/> using files as data storage.
    /// <remarks>
    /// Note that the <see cref="DataProviderManager"/> owning a <see cref="FileDataProvider"/> 
    /// must be a <see cref="urakawa.data.DataProviderManager"/>. 
    /// Trying to initialize a <see cref="FileDataProvider"/> with a non-<see cref="DataProviderManager"/>
    /// implementation of <see cref="DataProviderManager"/> 
    /// will cause a <see cref="exception.MethodParameterIsWrongTypeException"/>
    /// </remarks>
    /// </summary>
    public class FileDataProvider : DataProvider
    {
        public static string EliminateForbiddenFileNameCharacters(string str)
        {
            string newStr = str
                .Replace(Path.DirectorySeparatorChar, '_')
                .Replace(Path.AltDirectorySeparatorChar, '_');

            if (newStr.IndexOf('/') >= 0) //newStr.Contains("/"))
            {
                newStr = newStr.Replace('/', '_');
            }
            if (newStr.IndexOf('\\') >= 0) //newStr.Contains("\\"))
            {
                newStr = newStr.Replace('\\', '_');
            }

            newStr = newStr.Replace(':', '-')
                .Replace('*', '-')
                .Replace('?', '-')
                .Replace('<', '-')
                .Replace('>', '-')
                .Replace('|', '-')
                .Replace('\"', '-')
                ;

            return newStr;
        }

        private static int MAX_ATTEMPTS = 10;
        public static void DeleteDirectory(string path)
        {
            int attempt = MAX_ATTEMPTS;
            while (attempt-- >= 0)
            {
                try
                {
                    Directory.Delete(path, true);
                    break;
                }
                catch (Exception e)
                {
                    Thread.Sleep(200);
                }
            }

            if (Directory.Exists(path))
            {
#if DEBUG
                Debugger.Break();
#endif // DEBUG
            }
        }
        public static void CreateDirectory(string path)
        {
            int attempt = MAX_ATTEMPTS;
            while (attempt-- >= 0)
            {
                try
                {
                    Directory.CreateDirectory(path);
                    break;
                }
                catch (Exception e)
                {
                    Thread.Sleep(200);
                }
            }

            if (!Directory.Exists(path))
            {
#if DEBUG
                Debugger.Break();
#endif // DEBUG
            }
        }

        public static bool isHTTPFile(string filepath)
        {
            return filepath.StartsWith("http://") || filepath.StartsWith("https://");
        }

        public static string EnsureLocalFilePathDownloadTempDirectory(string filepath)
        {
            string localpath = filepath;

            if (isHTTPFile(filepath))
            {
                localpath = new Uri(filepath, UriKind.Absolute).LocalPath; //AbsolutePath preserves %20, file:// etc.
                localpath = Path.Combine(Path.GetTempPath(), Path.GetFileName(localpath));
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.Proxy = null;
                    webClient.DownloadFile(filepath, localpath);

                    //byte[] imageContent = webClient.DownloadData(filepath);
                    //Stream stream = new MemoryStream(imageContent);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return localpath;
        }


        public override string GetTypeNameFormatted()
        {
            return XukStrings.FileDataProvider;
        }
        private object m_lock = new object();

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="FileDataProvider"/>s should only be created via. the <see cref="DataProviderFactory"/>
        /// </summary>
        public FileDataProvider()
        {
            mDataFileRelativePath = null;
        }

        private string mDataFileRelativePath;

        private List<CloseNotifyingStream> mOpenInputStreams = new List<CloseNotifyingStream>();

        private CloseNotifyingStream mOpenOutputStream = null;

        /// <summary>
        /// Gets the path of the file storing the data of the instance, realtive to the path of data file directory
        /// of the owning <see cref="DataProviderManager"/>
        /// </summary>
        public string DataFileRelativePath
        {
            get
            {
                if (mDataFileRelativePath == null)
                {
                    //Lazy initialization
                    mDataFileRelativePath = Presentation.DataProviderManager.GetNewDataFileRelPath(
                        DataProviderFactory.GetExtensionFromMimeType(MimeType));
                }
                return mDataFileRelativePath;
            }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if the data file of the <see cref="FileDataProvider"/>
        /// has been initialized
        /// </summary>
        public bool IsDataFileInitialized
        {
            get { return mDataFileRelativePath != null; }
        }

        /// <summary>
        /// Gets the full path of the file storing the data the instance
        /// </summary>
        /// <returns>The full path</returns>
        public string DataFileFullPath
        {
            get { return Path.Combine(Presentation.DataProviderManager.DataFileDirectoryFullPath, DataFileRelativePath); }
        }

        #region DataProvider Members

        public void InitByCopyingExistingFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new exception.DataMissingException(
                    String.Format("The data file {0} does not exist", path));
            }

            if (File.Exists(DataFileFullPath))
            {
                throw new exception.OperationNotValidException(
                    String.Format("The data file {0} already exists", DataFileFullPath));
            }

            foreach (DataProvider dp in Presentation.DataProviderManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (dp is FileDataProvider)
                {
                    FileDataProvider fdp = (FileDataProvider)dp;
                    if (fdp.DataFileFullPath == path)
                    {
                        throw new exception.OperationNotValidException(
                            String.Format("The data file {0} is already managed", path));
                    }
                }
            }

            //Directory.GetParent(filePath).FullName
            //if (Path.GetDirectoryName(path) != Presentation.DataProviderManager.DataFileDirectoryFullPath)
            //{
            //    throw new exception.OperationNotValidException(
            //        String.Format("The data file {0} is not in the data directory {1}", path, Presentation.DataProviderManager.DataFileDirectoryFullPath));
            //}

            File.Copy(path, DataFileFullPath);

            HasBeenInitialized = true;
        }

        public void InitByMovingExistingFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new exception.DataMissingException(
                    String.Format("The data file {0} does not exist", path));
            }

            if (File.Exists(DataFileFullPath))
            {
                throw new exception.OperationNotValidException(
                    String.Format("The data file {0} already exists", DataFileFullPath));
            }

            foreach (DataProvider dp in Presentation.DataProviderManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (dp is FileDataProvider)
                {
                    FileDataProvider fdp = (FileDataProvider)dp;
                    if (fdp.DataFileFullPath == path)
                    {
                        throw new exception.OperationNotValidException(
                            String.Format("The data file {0} is already managed", path));
                    }
                }
            }

            //Directory.GetParent(filePath).FullName
            //if (Path.GetDirectoryName(path) != Presentation.DataProviderManager.DataFileDirectoryFullPath)
            //{
            //    throw new exception.OperationNotValidException(
            //        String.Format("The data file {0} is not in the data directory {1}", path, Presentation.DataProviderManager.DataFileDirectoryFullPath));
            //}

            File.Move(path, DataFileFullPath);

            HasBeenInitialized = true;
        }

        private bool HasBeenInitialized;

        private void CheckDataFile()
        {
            if (File.Exists(DataFileFullPath))
            {
                if (HasBeenInitialized)
                {
                    return;
                }

                File.Delete(DataFileFullPath);
            }
            else
            {
                if (HasBeenInitialized)
                {
                    throw new exception.DataMissingException(
                        String.Format("The data file {0} does not exist", DataFileFullPath));
                }

                //string dirPath = Path.GetDirectoryName(DataFileFullPath);

                // this may be needed in the future when we allow the Data folder to contain subfolders, for example to organize images, audio ,etc.
                //if (!Directory.Exists(dirPath))
                //{
                //    Directory.CreateDirectory(dirPath); // may be needed in case one intermediary folder does not exist in the path
                //}
            }

            try
            {
                File.Create(DataFileFullPath).Close();
            }
            catch (Exception e)
            {
                throw new exception.OperationNotValidException(
                    String.Format("Could not create data file {0}: {1}", DataFileFullPath, e.Message),
                    e);
            }

            HasBeenInitialized = true;
        }

        /// <summary>
        /// Gets an input <see cref="Stream"/> providing read access to the <see cref="FileDataProvider"/>
        /// </summary>
        /// <returns>The input stream</returns>
        /// <exception cref="exception.DataMissingException">
        /// Thrown if the data stored in the <see cref="DataProvider"/> is missing from the underlying storage mechanism
        /// </exception>
        /// <exception cref="exception.OutputStreamOpenException">
        /// Thrown if an output <see cref="Stream"/> from the data provider is already/still open
        /// </exception>
        /// <exception cref="exception.OperationNotValidException">
        /// Thrown if the underlying data file could not be opened in read-mode - see inner <see cref="Exception"/> for datails of cause
        /// </exception>
        public override Stream OpenInputStream()
        {
            lock (m_lock)
            {
                return OpenInputStream_NoLock();
            }
        }
        private Stream OpenInputStream_NoLock()
        {
            if (mOpenOutputStream != null)
            {
                throw new exception.OutputStreamOpenException(
                    "Cannot open an input Stream while an output Stream is open");
            }
            FileStream inputFS;
            string fp = DataFileFullPath;
            CheckDataFile();
            try
            {
                inputFS = new FileStream(fp, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception e)
            {
                throw new exception.OperationNotValidException(
                    String.Format("Could not open file {0}", fp),
                    e);
            }
            CloseNotifyingStream res = new CloseNotifyingStream(inputFS);
            res.StreamClosed += InputStreamClosed_StreamClosed;
            mOpenInputStreams.Add(res);
            return res;
        }

        private void InputStreamClosed_StreamClosed(object sender, EventArgs e)
        {
            CloseNotifyingStream cnStm = sender as CloseNotifyingStream;
            if (cnStm != null)
            {
                lock (m_lock)
                {
                    cnStm.StreamClosed -= InputStreamClosed_StreamClosed;
                    if (mOpenInputStreams.Contains(cnStm)) mOpenInputStreams.Remove(cnStm);
                }
            }
        }

        /// <summary>
        /// Gets an output <see cref="Stream"/> providing write access to the <see cref="FileDataProvider"/>
        /// </summary>
        /// <returns>The ouput stream</returns>
        /// <exception cref="exception.DataMissingException">
        /// Thrown if the data stored in the <see cref="DataProvider"/> is missing from the underlying storage mechanism
        /// </exception>
        /// <exception cref="exception.InputStreamsOpenException">
        /// Thrown if another output <see cref="Stream"/> from the data provider is already/still open
        /// </exception>
        /// <exception cref="exception.OutputStreamOpenException">
        /// Thrown if another output <see cref="Stream"/> from the data provider is already/still open
        /// </exception>
        /// <exception cref="exception.OperationNotValidException">
        /// Thrown if the underlying data file could not be opened in write-mode - see inner <see cref="Exception"/> for datails of cause
        /// </exception>
        public override Stream OpenOutputStream()
        {
            lock (m_lock)
            {
                if (mOpenOutputStream != null)
                {
                    throw new exception.OutputStreamOpenException(
                        "Cannot open an output Stream while another output Stream is already open");
                }
                if (mOpenInputStreams.Count > 0)
                {
                    throw new exception.InputStreamsOpenException(
                        "Cannot open an output Stream while one or more input Streams are open");
                }
                CheckDataFile();
                string fp = DataFileFullPath;

                FileStream outputFS;
                try
                {
                    outputFS = new FileStream(fp, FileMode.Open, FileAccess.Write, FileShare.None);
                }
                catch (Exception e)
                {
                    throw new exception.OperationNotValidException(
                        String.Format("Could not open file {0}", fp),
                        e);
                }
                mOpenOutputStream = new CloseNotifyingStream(outputFS);
                mOpenOutputStream.StreamClosed += OutputStream_StreamClosed;
                return mOpenOutputStream;
            }
        }

        private void OutputStream_StreamClosed(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, mOpenOutputStream))
            {
                lock (m_lock)
                {
                    mOpenOutputStream.StreamClosed -= new EventHandler(OutputStream_StreamClosed);
                    mOpenOutputStream = null;
                }
            }
        }

        /// <summary>
        /// Deletes the file data provider, including any existing data files. Also detaches it self 
        /// the owning data provider manager
        /// </summary>
        /// <exception cref="exception.OutputStreamOpenException">
        /// Thrown if a output <see cref="Stream"/> from the <see cref="DataProvider"/> is currently open
        /// </exception>
        /// <exception cref="exception.InputStreamsOpenException">
        /// Thrown if one or more input <see cref="Stream"/>s from the <see cref="DataProvider"/> are currently open
        /// </exception>
        /// <exception cref="exception.OperationNotValidException">
        /// Thrown if an exception occurs while deleting the data file of <c>this</c>. 
        /// The occuring exception can be accessed as the inner exception of the thrown exception.
        /// </exception>
        public override void Delete()
        {
            lock (m_lock)
            {
                if (mOpenOutputStream != null)
                {
                    throw new exception.OutputStreamOpenException(
                        "Cannot delete the FileDataProvider while an output Stream is still open");
                }
                if (mOpenInputStreams.Count > 0)
                {
                    throw new exception.InputStreamsOpenException(
                        "Cannot delete the FileDataProvider while one or more input Streams are still open");
                }
                if (File.Exists(DataFileFullPath))
                {
                    try
                    {
                        File.Delete(DataFileFullPath);
                    }
                    catch (Exception e)
                    {
                        throw new exception.OperationNotValidException(String.Format(
                                                                           "Could not delete data file {0}: {1}",
                                                                           DataFileFullPath, e.Message), e);
                    }
                }
                Presentation.DataProviderManager.RemoveDataProvider(this, false);
            }
        }

        public void DeleteByMovingToFolder(string fullPathToDeletedDataFolder)
        {
            lock (m_lock)
            {
                if (mOpenOutputStream != null)
                {
                    throw new exception.OutputStreamOpenException(
                        "Cannot delete the FileDataProvider while an output Stream is still open");
                }
                if (mOpenInputStreams.Count > 0)
                {
                    throw new exception.InputStreamsOpenException(
                        "Cannot delete the FileDataProvider while one or more input Streams are still open");
                }
                if (File.Exists(DataFileFullPath))
                {
                    string fileName = Path.GetFileName(DataFileFullPath);
                    string filePathDest = Path.Combine(fullPathToDeletedDataFolder, fileName);
                    try
                    {
                        File.Move(DataFileFullPath, filePathDest);
                    }
                    catch (Exception e)
                    {
                        throw new exception.OperationNotValidException(String.Format(
                                                                           "Could not delete data file {0}: {1}",
                                                                           DataFileFullPath, e.Message), e);
                    }
                }
                Presentation.DataProviderManager.RemoveDataProvider(this, false);
            }
        }

        /// <summary>
        /// Copies the file data provider including the data 
        /// </summary>
        /// <returns>The copy</returns>
        public override DataProvider Copy()
        {
            lock (m_lock)
            {
                DataProvider c = Presentation.DataProviderFactory.Create<FileDataProvider>(MimeType);
                Stream thisData = OpenInputStream_NoLock();
                try
                {
                    if (thisData.Length > 0)
                    {
                        c.AppendData(thisData, thisData.Length);
                    }
                }
                finally
                {
                    thisData.Close();
                }
                return c;
            }
        }

        /// <summary>
        /// Exports <c>this</c> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="FileDataProvider"/></returns>
        public override DataProvider Export(Presentation destPres)
        {
            lock (m_lock)
            {
                FileDataProvider expFDP = destPres.DataProviderFactory.Create<FileDataProvider>(MimeType);
                Stream thisStm = OpenInputStream_NoLock();
                try
                {
                    if (thisStm.Length > 0)
                    {
                        expFDP.AppendData(thisStm, thisStm.Length);
                    }
                }
                finally
                {
                    thisStm.Close();
                }
                return expFDP;
            }
        }

        #endregion

        #region IXukAble Members

        /// <summary>
        /// Reads the attributes of a FileDataProvider xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string val = source.GetAttribute(XukStrings.DataFileRelativePath);
            if (string.IsNullOrEmpty(val))
            {
                throw new exception.XukException("dataFileRelativePath is missing from FileDataProvider element");
            }
            mDataFileRelativePath = val;

            if (!File.Exists(DataFileFullPath))
            {
                Debug.Fail(String.Format("The data file [{0}] does not exist", DataFileFullPath));
                //throw new exception.DataMissingException();
            }
            else
            {
                HasBeenInitialized = true; //Assume that the data file exists    
            }
        }

        /// <summary>
        /// Writes the attributes of a FileDataProvider element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            // We can't check the data file because in some cases like Save As,
            // the audio data only gets copied *after* the XUK
            //CheckDataFile(); //Ensure that data file exist even if no data has yet been written to it.

            destination.WriteAttributeString(XukStrings.DataFileRelativePath, DataFileRelativePath);

            if (!Presentation.Project.IsPrettyFormat())
            {
                //destination.WriteAttributeString(XukStrings.Uid, Uid);
            }

        }

        #endregion

        #region IValueEquatable<DataProvider> Members
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            FileDataProvider otherz = other as FileDataProvider;
            if (otherz == null)
            {
                return false;
            }

            if (otherz.MimeType != MimeType)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            if (Presentation.DataProviderManager.CompareByteStreamsDuringValueEqual
                && other.Presentation.DataProviderManager.CompareByteStreamsDuringValueEqual)
            {
                if (!DataProviderManager.CompareDataProviderContent(this, otherz))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
            }
            return true;
        }


        #endregion

    }
}