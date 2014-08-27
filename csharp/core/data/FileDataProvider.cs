using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using AudioLib;
using urakawa.xuk;

namespace urakawa.data
{
    [XukNameUglyPrettyAttribute("fdp", "FileDataProvider")]
    public class FileDataProvider : DataProvider
    {
        private const char UNDERSCORE = '_';
        private const char HYPHEN = '-';
        private static readonly char[] INVALID_CHARS = Path.GetInvalidPathChars();
        private static readonly StringBuilder m_StrBuilder = new StringBuilder(30);
        public static string EliminateForbiddenFileNameCharacters(string str)
        {
            //StringBuilder strBuilder = new StringBuilder(str.Length);
#if NET40
            m_StrBuilder.Clear();
#else
            m_StrBuilder.Length = 0;
#endif //NET40
            m_StrBuilder.EnsureCapacity(str.Length);

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == Path.DirectorySeparatorChar
                    || c == Path.AltDirectorySeparatorChar
                    || c == '/'
                    || c == '\\')
                {
                    m_StrBuilder.Append(UNDERSCORE);
                }
                else if (c == ':'
                    || c == '*'
                    || c == '?'
                    || c == '<'
                    || c == '>'
                    || c == '|'
                    || c == '\"'
                    || c == '\''
                    || c == ','
                )
                {
                    m_StrBuilder.Append(HYPHEN);
                }
                else
                {
                    bool added = false;
                    foreach (char cc in INVALID_CHARS)
                    {
                        if (c == cc)
                        {
                            m_StrBuilder.Append(UNDERSCORE);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                    {
                        if (c == '.'
                            &&
                            (
                            m_StrBuilder.Length == 0
                            ||
                            m_StrBuilder[m_StrBuilder.Length - 1] == '.'
                            )
                            )
                        {
                            //let's avoid two consecutive dots, or dot at begining of filename
                            bool debug = true;
                        }
                        else
                        {
                            m_StrBuilder.Append(c);
                        }
                    }
                }
            }

            return m_StrBuilder.ToString();
        }

#if DEBUG
        static FileDataProvider()
        {
            string str1 = @"C:/dir/file.txt";
            string str2 = @"C:/dir/../dir/file.txt";
            string str3 = @"C:/dir/./file.txt";
            string str4 = @"C:/dir/../dir/./file.txt";
            string str5 = @"C:/dir/subdir/../file.txt";
            string str6 = @"C:/dir/subdir/./../file.txt";
            string str7 = @"C:/dir/subdir/.././file.txt";

            string p1 = NormaliseFullFilePath(str1);
            string p2 = NormaliseFullFilePath(str2);
            string p3 = NormaliseFullFilePath(str3);
            string p4 = NormaliseFullFilePath(str4);
            string p5 = NormaliseFullFilePath(str5);
            string p6 = NormaliseFullFilePath(str6);
            string p7 = NormaliseFullFilePath(str7);

            DebugFix.Assert(p1 == p2);
            DebugFix.Assert(p2 == p3);
            DebugFix.Assert(p3 == p4);
            DebugFix.Assert(p4 == p5);
            DebugFix.Assert(p5 == p6);
            DebugFix.Assert(p6 == p7);

            str1 = str1.Replace('/', '\\');
            str2 = str2.Replace('/', '\\');
            str3 = str3.Replace('/', '\\');
            str4 = str4.Replace('/', '\\');
            str5 = str5.Replace('/', '\\');
            str6 = str6.Replace('/', '\\');
            str7 = str7.Replace('/', '\\');

            p1 = NormaliseFullFilePath(str1);
            p2 = NormaliseFullFilePath(str2);
            p3 = NormaliseFullFilePath(str3);
            p4 = NormaliseFullFilePath(str4);
            p5 = NormaliseFullFilePath(str5);
            p6 = NormaliseFullFilePath(str6);
            p7 = NormaliseFullFilePath(str7);

            DebugFix.Assert(p1 == p2);
            DebugFix.Assert(p2 == p3);
            DebugFix.Assert(p3 == p4);
            DebugFix.Assert(p4 == p5);
            DebugFix.Assert(p5 == p6);
            DebugFix.Assert(p6 == p7);

            string pp1 = NormaliseFullFilePath(@"C:/dir/subdir/");
            string pp2 = NormaliseFullFilePath(@"C:/dir/subdir");

            DebugFix.Assert(pp1 == pp2);

            string pp3 = NormaliseFullFilePath(@"\\\\network-share/dir\\subdir/");
            string pp4 = NormaliseFullFilePath(@"//network-share\\dir/subdir");

            DebugFix.Assert(pp3 == pp4);
        }
#endif

        public static string NormaliseFullFilePath(string fullPath)
        {
            bool absolute = Path.IsPathRooted(fullPath);

            DebugFix.Assert(absolute);
            if (!absolute)
            {
                return fullPath;
            }

            char sep = Path.DirectorySeparatorChar;
            DebugFix.Assert(sep == '\\'); // We're on Windows!

            if (sep == '\\' && fullPath.IndexOf('/') >= 0)
            {
                fullPath = fullPath.Replace('/', sep);
            }
            else if (sep == '/' && fullPath.IndexOf('\\') >= 0)
            {
                fullPath = fullPath.Replace('\\', sep);
            }

            //fullPath = fullPath.TrimEnd(new char[] {sep});
            //fullPath = fullPath.TrimEnd(sep);
            if (
                fullPath.Length > 1 &&
                fullPath[fullPath.Length - 1] == sep
                //fullPath.LastIndexOf(sep) == fullPath.Length - 1
                )
            {
                fullPath = fullPath.Substring(0, fullPath.Length - 1);
            }

            // Path.GetFullPath accesses the filesystem when the file exists.
            // fullPath = Path.GetFullPath(fullPath);

            //DirectoryInfo dirInfo = new DirectoryInfo(fullPath);
            //string str0 = dirInfo.FullName;

            FileInfo fileInfo = new FileInfo(fullPath);
            fullPath = fileInfo.FullName;

            // Replaces '\\' with '/'
            //Uri uri = new Uri(fullPath, UriKind.Absolute);
            //string fullPath1 = uri.AbsolutePath; // LocalPath

            //UriBuilder uriBuilder = new UriBuilder();
            //uriBuilder.Host = String.Empty;
            //uriBuilder.Scheme = Uri.UriSchemeFile;
            //uriBuilder.Path = fullPath;
            //Uri fileUri = uriBuilder.Uri;
            //fullPath = fileUri.ToString();

            if (
                fullPath.Length > 1 &&
                fullPath[fullPath.Length - 1] == sep
                //fullPath.LastIndexOf(sep) == fullPath.Length - 1
                )
            {
#if DEBUG
                Debugger.Break();
#endif
                fullPath = fullPath.Substring(0, fullPath.Length - 1);
            }

            if (sep == '\\')
            {
                fullPath = fullPath.Replace(sep, '/');
            }

            return fullPath;
        }

        private static void recursiveDeleteDirectory(string rootDirPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(rootDirPath);

#if NET40
            IEnumerable<DirectoryInfo> allDirs = dirInfo.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
#else
            DirectoryInfo[] allDirs = dirInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
#endif
            foreach (DirectoryInfo subDirInfo in allDirs)
            {
                string subDirPath = Path.Combine(rootDirPath, subDirInfo.Name);
                //recursiveDeleteDirectory(subDirInfo.FullName);
                recursiveDeleteDirectory(subDirPath);
            }

#if NET40
            IEnumerable<FileInfo> allFiles = dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
#else
            FileInfo[] allFiles = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
#endif
            foreach (FileInfo fileInfo in allFiles)
            {
                try
                {
                    string filePath = Path.Combine(rootDirPath, fileInfo.Name);
                    try
                    {
                        File.SetAttributes(filePath, FileAttributes.Normal);
                    }
                    catch
                    {
                    }
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    try
                    {
                        fileInfo.Delete();
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee.Message);

                        string filePath = fileInfo.FullName;
                        FileInfo fi = new FileInfo(filePath);
                        fi.Delete();
                    }
                }
            }

            try
            {
                Directory.Delete(rootDirPath, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(rootDirPath);

                Console.WriteLine(ex.Message);

                try
                {
                    dirInfo.Delete(false);
                }
                catch (Exception exx)
                {
                    Console.WriteLine(exx.Message);
                }
            }
        }

        public static void TryDeleteDirectory(string dir, bool showDirIfError)
        {
            if (Directory.Exists(dir))
            {
                string error = DeleteDirectory(dir);
                if (!string.IsNullOrEmpty(error))
                {
                    if (showDirIfError && Directory.Exists(dir))
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = dir;
                        process.StartInfo.RedirectStandardError = false;
                        process.StartInfo.RedirectStandardOutput = false;
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        process.StartInfo.Arguments = "";
                        process.Start();
                    }

                    throw new IOException("Problem trying to delete folder (" + error + "): " + dir);
                }
            }
        }

        private static int MAX_ATTEMPTS = 3;
        public static string DeleteDirectory(string path)
        {
            string error = String.Empty;

            int attempt = MAX_ATTEMPTS;
            while (attempt-- >= 0)
            {
                try
                {
                    Directory.Delete(path, true);
                    break;
                }
                catch (Exception e) //System.UnauthorizedAccessException
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(200);

                    if (string.IsNullOrEmpty(error))
                    {
                        error = e.Message;
                    }

                    //if (Directory.Exists(path))
                    //{
                    //    try
                    //    {
                    //        recursiveDeleteDirectory(path);
                    //        break;
                    //    }
                    //    catch (Exception ee)
                    //    {
                    //        Console.WriteLine(ee.Message);
                    //        Thread.Sleep(200);
                    //    }
                    //}
                }
            }

            if (Directory.Exists(path))
            {
                Console.WriteLine("Directory cannot be deleted! ... (" + MAX_ATTEMPTS + " attempts)");
                Console.WriteLine(path);
                Console.WriteLine(error);
#if DEBUG
                Debugger.Break();
#endif // DEBUG
                try
                {
                    recursiveDeleteDirectory(path);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif // DEBUG
                }
            }

            return error;
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
                    Console.WriteLine(e.Message);
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

        public static void CopyDirectory(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                FileDataProvider.CreateDirectory(destPath);
            }

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(file));
                File.Copy(file, dest);
                try
                {
                    File.SetAttributes(dest, FileAttributes.Normal);
                }
                catch
                {
                }
            }

            foreach (string folder in Directory.GetDirectories(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(folder));
                FileDataProvider.CopyDirectory(folder, dest);
            }
        }

        public static void MoveDirectory(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                FileDataProvider.CreateDirectory(destPath);
            }

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(file));
                File.Move(file, dest);
                try
                {
                    File.SetAttributes(dest, FileAttributes.Normal);
                }
                catch
                {
                }
            }

            foreach (string folder in Directory.GetDirectories(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(folder));
                FileDataProvider.MoveDirectory(folder, dest);
            }

            FileDataProvider.DeleteDirectory(sourcePath);
        }

        public static bool isHTTPFile(string filepath)
        {
            return filepath.StartsWith("http://") || filepath.StartsWith("https://");
        }

        public static string UriEncode(string urlStr)
        {
            //return Uri.EscapeDataString(urlStr);
            return Uri.EscapeUriString(urlStr);
        }

        public static string UriDecode(string urlStr)
        {
            //http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
            //return HttpUtility.UrlPathEncode(urlStr);
            //return HttpUtility.UrlDecode(urlStr);
            return Uri.UnescapeDataString(urlStr);
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
            try
            {
                File.SetAttributes(DataFileFullPath, FileAttributes.Normal);
            }
            catch
            {
            }

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
            try
            {
                File.SetAttributes(DataFileFullPath, FileAttributes.Normal);
            }
            catch
            {
            }

            HasBeenInitialized = true;
        }

        private bool HasBeenInitialized;

        private void CheckDataFile()
        {
            string fp = DataFileFullPath;

            if (File.Exists(fp))
            {
                if (HasBeenInitialized)
                {
                    return;
                }

                File.Delete(fp);
            }
            else
            {
                if (HasBeenInitialized)
                {
                    throw new exception.DataMissingException(
                        String.Format("The data file {0} does not exist", fp));
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
                File.Create(fp).Close();
            }
            catch (Exception e)
            {
                throw new exception.OperationNotValidException(
                    String.Format("Could not create data file {0}: {1}", fp, e.Message),
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
            string fp = DataFileFullPath;

            if (mOpenOutputStream != null)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new exception.OutputStreamOpenException(
                    "Cannot open an input Stream while an output Stream is open: " + fp);
            }

            FileStream inputFS;

            CheckDataFile();
            try
            {
                inputFS = new FileStream(fp, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
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
                string fp = DataFileFullPath;

                if (mOpenOutputStream != null)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    throw new exception.OutputStreamOpenException(
                        "Cannot open an output Stream while another output Stream is already open: " + fp);
                }
                if (mOpenInputStreams.Count > 0)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    throw new exception.InputStreamsOpenException(
                        "Cannot open an output Stream while one or more input Streams are open: " + fp);
                }
                CheckDataFile();

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
                string fp = DataFileFullPath;

                if (mOpenOutputStream != null)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    throw new exception.OutputStreamOpenException(
                        "Cannot delete the FileDataProvider while an output Stream is still open: " + fp);
                }
                if (mOpenInputStreams.Count > 0)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    throw new exception.InputStreamsOpenException(
                        "Cannot delete the FileDataProvider while one or more input Streams are still open: " + fp);
                }
                if (File.Exists(DataFileFullPath))
                {
                    try
                    {
                        File.Delete(DataFileFullPath);
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        throw new exception.OperationNotValidException(String.Format(
                                                                           "Could not delete data file {0}: {1}",
                                                                           fp, e.Message), e);
                    }
                }
                Presentation.DataProviderManager.RemoveDataProvider(this, false);
            }
        }

        public void DeleteByMovingToFolder(string fullPathToDeletedDataFolder)
        {
            lock (m_lock)
            {
                string fp = DataFileFullPath;

                if (mOpenOutputStream != null)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    throw new exception.OutputStreamOpenException(
                        "Cannot delete the FileDataProvider while an output Stream is still open: " + fp);
                }
                if (mOpenInputStreams.Count > 0)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    throw new exception.InputStreamsOpenException(
                        "Cannot delete the FileDataProvider while one or more input Streams are still open: " + fp);
                }
                if (File.Exists(DataFileFullPath))
                {
                    string fileName = Path.GetFileName(DataFileFullPath);
                    string filePathDest = Path.Combine(fullPathToDeletedDataFolder, fileName);
                    try
                    {
                        File.Move(DataFileFullPath, filePathDest);
                        try
                        {
                            File.SetAttributes(filePathDest, FileAttributes.Normal);
                        }
                        catch
                        {
                        }
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        throw new exception.OperationNotValidException(String.Format(
                                                                           "Could not delete data file {0}: {1}",
                                                                           fp, e.Message), e);
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
                Console.WriteLine(String.Format("The data file [{0}] does not exist", DataFileFullPath));
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

            if (!Presentation.Project.PrettyFormat)
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