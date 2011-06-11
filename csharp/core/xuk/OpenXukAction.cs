using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Xml;
using AudioLib;
using urakawa.command;
using urakawa.events.progress;
using urakawa.ExternalFiles;
using urakawa.progress;

#if USE_ISOLATED_STORAGE
using System.IO.IsolatedStorage;
#endif //USE_ISOLATED_STORAGE

namespace urakawa.xuk
{
    public class LocalXmlUrlResolver : XmlUrlResolver
    {
        private ICredentials m_Credentials;

        private readonly bool m_EnableHttpCaching;
        private const string m_DtdStoreDirName = "Downloaded-DTDs";

        public LocalXmlUrlResolver(bool enableHttpCaching)
        {
            m_EnableHttpCaching = enableHttpCaching;
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if ((baseUri == null) || (!baseUri.IsAbsoluteUri && (baseUri.OriginalString.Length == 0)))
            {
                //var uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
                Uri uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri && (uri.OriginalString.Length > 0))
                {
                    uri = new Uri(Path.GetFullPath(relativeUri));
                }
                return uri;
            }

            return !String.IsNullOrEmpty(relativeUri) ? new Uri(baseUri, relativeUri) : baseUri;
        }

        public override ICredentials Credentials
        {
            set
            {
                m_Credentials = value;
                base.Credentials = value;
            }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null)
            {
                throw new ArgumentNullException("absoluteUri");
            }

            if (ofObjectToReturn != null && ofObjectToReturn != typeof(Stream))
            {
                return null;
            }

            // Resolve local known entities
            Stream localStream = mapUri(absoluteUri);
            if (localStream != null)
            {
                return localStream;
            }

            // if dtd is not found in packaged files, search in downloaded dtds
            /*
            string localDTDFullPath = Path.Combine ( m_DownloadedDTDDirectoryPath,
                Path.GetFileName ( absoluteUri.AbsolutePath ));
            if (File.Exists ( localDTDFullPath ))
                {
                return new FileStream ( localDTDFullPath, FileMode.Open, FileAccess.Read );
                }
            */

            string filename = Path.GetFileName(absoluteUri.AbsolutePath);

#if USE_ISOLATED_STORAGE

                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.GetFileNames(filename).Length > 0)
                    {
                        IsolatedStorageFileStream storageStream =
                            new IsolatedStorageFileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None, store);

                        return storageStream;
                    }
                }

                // NOTE: we could actually use the same code as below, which gives more control over the subdirectory and doesn't have any size limits:
#else
            string path = Path.Combine(ExternalFilesDataManager.STORAGE_FOLDER_PATH, m_DtdStoreDirName);
            path = Path.Combine(path, filename);

            if (File.Exists(path))
            {
                Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                return stream;
            }
#endif //USE_ISOLATED_STORAGE


            //resolve resources from cache (if possible)
            if (m_EnableHttpCaching &&
                (absoluteUri.Scheme.ToLower() == Uri.UriSchemeHttp || absoluteUri.Scheme.ToLower() == Uri.UriSchemeHttps) //absoluteUri.IsUnc?
                )
            {
                WebRequest webReq = WebRequest.Create(absoluteUri);
                webReq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                if (m_Credentials != null)
                {
                    webReq.Credentials = m_Credentials;
                }
                WebResponse resp = webReq.GetResponse();
                Stream webStream = resp.GetResponseStream();

                string localpath = CreateLocalDTDFileFromWebStream(absoluteUri.AbsolutePath, webStream);

                //webStream.Close();
                resp.Close();

                return File.Open(localpath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            // No need to look for a local file that does not exist.
            //if (absoluteUri.Scheme == "file" && !File.Exists(absoluteUri.LocalPath))
            //{
            //return null;
            //}

            //otherwise use the default behavior of the XmlUrlResolver class (resolve resources from source)
            try
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
            catch
            {
                return null;
            }
        }

        public Stream mapUri(Uri absoluteUri)
        {
            //if (!absoluteUri.AbsolutePath.EndsWith("opf")
            //    && !absoluteUri.AbsolutePath.EndsWith("xhtml")
            //    && !absoluteUri.AbsolutePath.EndsWith("html"))
            //{
            //    Debugger.Break();
            //}

            Stream dtdStream = null;
            foreach (String key in DTDs.DTDs.ENTITIES_MAPPING.Keys)
            {
                if (absoluteUri.AbsolutePath.Contains(key))
                {
                    dtdStream = DTDs.DTDs.Fetch(DTDs.DTDs.ENTITIES_MAPPING[key]);
                    Console.WriteLine("XML Entity Resolver [" + DTDs.DTDs.ENTITIES_MAPPING[key] + "]: " + (dtdStream != null ? dtdStream.Length + " bytes resource. " : "resource not found ?? ") + " ( " + absoluteUri + " )");
                    return dtdStream;
                }
            }

            return null;
        }

        private string CreateLocalDTDFileFromWebStream(string strWebDTDPath, Stream webStream)
        {
            FileStream fs = null;
            try
            {
#if USE_ISOLATED_STORAGE

                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        fs = new IsolatedStorageFileStream(Path.GetFileName(strWebDTDPath), FileMode.Create, FileAccess.Write, FileShare.None, store);
                    }

                    // NOTE: we could actually use the same code as below, which gives more control over the subdirectory and doesn't have any size limits:
#else
                string dirpath = Path.Combine(ExternalFilesDataManager.STORAGE_FOLDER_PATH, m_DtdStoreDirName);

                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }

                string filepath = Path.Combine(dirpath, Path.GetFileName(strWebDTDPath));
                fs = File.Create(filepath);
#endif //USE_ISOLATED_STORAGE

                const uint BUFFER_SIZE = 1024; // 1 KB MAX BUFFER
                StreamUtils.Copy(webStream, 0, fs, BUFFER_SIZE);

                return filepath;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        //public void copyStreamData(Stream source, Stream dest, int BUFFER_SIZE)
        //{ //1
        //    if (source.CanSeek && source.Length <= BUFFER_SIZE)
        //    { // 2
        //        byte[] buffer = new byte[source.Length];
        //        int read = source.Read(buffer, 0, (int)source.Length);
        //        dest.Write(buffer, 0, read);
        //    } //-2
        //    else
        //    { // 2
        //        byte[] buffer = new byte[BUFFER_SIZE];
        //        int bytesRead = 0;
        //        while ((bytesRead = source.Read(buffer, 0, BUFFER_SIZE)) > 0)
        //        { //3
        //            dest.Write(buffer, 0, bytesRead);
        //        } // -3
        //    } //-2
        //} // -1
    }
    
    ///<summary>
    ///  Action that deserializes a xuk data stream into a <see cref="XukAble"/>
    ///</summary>
    public class OpenXukAction : ProgressAction
    {
        public static XmlReaderSettings GetDefaultXmlReaderConfiguration(bool useLocalXmlResolver, bool preserveWhiteSpace)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.None;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            
            if (useLocalXmlResolver)
            {
                settings.XmlResolver = new LocalXmlUrlResolver(true);
            }
            else
            {
                settings.XmlResolver = null;
            }

            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = false;
            settings.IgnoreWhitespace = !preserveWhiteSpace;

            return settings;
        }

        public static XmlDocument ParseXmlDocument(string path, bool preserveWhiteSpace)
        {
            XmlReaderSettings settings = GetDefaultXmlReaderConfiguration(true, preserveWhiteSpace);

            using (XmlReader xmlReader = XmlReader.Create(path, settings))
            {
                if (preserveWhiteSpace && xmlReader is XmlTextReader)
                {
                    ((XmlTextReader) xmlReader).WhitespaceHandling = WhitespaceHandling.All;
                }
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.PreserveWhitespace = preserveWhiteSpace;
                xmldoc.XmlResolver = null;
                try
                {
                    xmldoc.Load(xmlReader);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    // No message box: use debugging instead (inspect stack trace, watch values)
                    //MessageBox.Show(e.ToString());

                    // The Fail() method is better:
                    //System.Diagnostics.Debug.Fail(e.Message);

                    //Or you can explicitely break:
#if DEBUG
                    Debugger.Break();
#endif
                }
                finally
                {
                    xmlReader.Close();
                }

                return xmldoc;
            }
        }

        public override void DoWork()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Execute();
            stopWatch.Stop();
            Console.WriteLine(@"......XUK-in milliseconds: " + stopWatch.ElapsedMilliseconds);
        }

        private Uri mSourceUri;
        private Stream mSourceStream;
        private XmlReader mXmlReader;
        private readonly IXukAble mDestXukAble;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source (can be null)</param>
        /// <param name="xukAble">The destination <see cref="IXukAble"/> (cannot be null)</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> (cannot be null)</param>
        //public OpenXukAction(IXukAble xukAble, Uri sourceUri, Stream sourceStream)
        //{
        //    if (sourceStream == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The source Stream of the OpenXukAction cannot be null");
        //    if (xukAble == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The destination IXukAble of the OpenXukAction cannot be null");
        //    mSourceUri = sourceUri;
        //    mDestXukAble = xukAble;
        //    mSourceStream = sourceStream;
        //    initializeXmlReader(mSourceStream);
        //}

        /// <summary>
        /// Constructor (DO NOT USE ! THE STREAM IS NULL. USE THE STREAM-BASED CTOR instead)
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source (can be null)</param>
        /// <param name="xukAble">The destination <see cref="IXukAble"/> (cannot be null)</param>
        /// <param name="reader">The source <see cref="XmlReader"/> (cannot be null)</param>
        //public OpenXukAction(IXukAble xukAble, Uri sourceUri, XmlReader reader)
        //{
        //    if (reader == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The source XmlReader of the OpenXukAction cannot be null");
        //    if (xukAble == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The destination IXukAble of the OpenXukAction cannot be null");
        //    mSourceUri = sourceUri;
        //    mDestXukAble = xukAble;
        //    mXmlReader = reader;
        //    XmlTextReader txtReader = reader as XmlTextReader;
        //    if (txtReader != null)
        //    {
        //        //TODO: where can we get the underlying stream ??
        //        //mSourceStream = txtReader.BaseStream;
        //    }
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source file (cannot be null)</param>
        /// <param name="xukAble">The destination <see cref="IXukAble"/> (cannot be null)</param>
        public OpenXukAction(IXukAble xukAble, Uri sourceUri)
        {
            if (sourceUri == null)
                throw new exception.MethodParameterIsNullException(
                    "The source URI of the OpenXukAction cannot be null");
            if (xukAble == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination IXukAble of the OpenXukAction cannot be null");

            mSourceUri = sourceUri;
            mDestXukAble = xukAble;

            int currentPercentage = 0;
            /*
            EventHandler<ProgressEventArgs> progressing = (sender, e) =>
            {
                double val = e.Current;
                double max = e.Total;
                //var percent = (int)((val / max) * 100);
                int percent = (int)((val / max) * 100);

                if (percent != currentPercentage)
                {
                    currentPercentage = percent;
                    reportProgress(currentPercentage, val + " / " + max);
                    //backWorker.ReportProgress(currentPercentage);
                }
                if (RequestCancellation)
                {
                    e.Cancel();
                }
            };
            */
            //dotnet2
            EventHandler<ProgressEventArgs> progressing = delegate (object sender, ProgressEventArgs e) {
                double val = e.Current;
                double max = e.Total;
                //var percent = (int)((val / max) * 100);
                int percent = (int)((val / max) * 100);
                
                if (percent != currentPercentage)
                {
                    currentPercentage = percent;
                    reportProgress  (currentPercentage, val + " / " + max);
                    //backWorker.ReportProgress(currentPercentage);
                }
                if (RequestCancellation)
                {
                    e.Cancel();
                }
            };
                
            Progress += progressing;
            //Finished += (sender, e) =>
            //{
                //Progress -= progressing;
            //};

            //dotnet2
            Finished += delegate (object sender,FinishedEventArgs e) 
            {
                Progress -= progressing;
            };
            //Cancelled += (sender, e) =>
            //{
                //Progress -= progressing;
            //};

            //dotnet2
            Cancelled += delegate (object sender,CancelledEventArgs e) 
            {
                Progress -= progressing;
            };
            if (!mSourceUri.IsFile)
                throw new exception.XukException("The XUK URI must point to a local file!");

            mSourceStream = new FileStream(mSourceUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            XmlReaderSettings settings = GetDefaultXmlReaderConfiguration(false, false);

            if (!mDestXukAble.IsPrettyFormat())
            {
                //
            }
            else
            {
                //
            }

            mXmlReader = XmlReader.Create(mSourceStream, settings, mSourceUri.ToString());
        }

        private void closeInput()
        {
            mXmlReader.Close();
            mXmlReader = null;
            mSourceStream.Close();
            mSourceStream.Dispose();
            mSourceStream = null;
        }


        #region Overrides of ProgressAction

        /// <summary>
        /// Gets the current and estimated total progress values
        /// </summary>
        /// <param name="cur">A <see cref="long"/> in which the current progress is returned</param>
        /// <param name="tot">A <see cref="long"/> in which the estimated total progress is returned</param>
        protected override void GetCurrentProgress(out long cur, out long tot)
        {
            if (mSourceStream != null)
            {
                cur = mSourceStream.Position;
                tot = mSourceStream.Length;
            }
            else
            {
                cur = 0;
                tot = 0;
            }
        }

        /// <summary>
        /// Gets a <c>bool</c> indicating if the <see cref="IAction"/> can execute
        /// </summary>
        /// <returns>The <c>bool</c></returns>
        public override bool CanExecute
        {
            get { return mXmlReader != null; }
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public override void Execute()
        {
            //mHasCancelBeenRequested = false;
            //Progress += OpenXukAction_progress;

            bool canceled = false;
            try
            {
                bool foundRoot = false;
                while (mXmlReader.Read())
                {
                    if (mXmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (mXmlReader.LocalName == XukStrings.XukPretty)
                        {
                            mDestXukAble.SetPrettyFormat(true);
                            foundRoot = true;
                            break;
                        }
                        else if (mXmlReader.LocalName == XukStrings.XukCompressed)
                        {
                            mDestXukAble.SetPrettyFormat(false);
                            foundRoot = true;
                            break;
                        }
                    }
                    else if (mXmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (mXmlReader.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
                if (!foundRoot)
                {
                    throw new exception.XukException("Could not find Xuk element in XukAble fragment");
                }

                bool foundXukAble = false;
                if (!mXmlReader.IsEmptyElement)
                {
                    while (mXmlReader.Read())
                    {
                        if (mXmlReader.NodeType == XmlNodeType.Element)
                        {
                            //If the element QName matches the Xuk QName equivalent of this, Xuk it in using this.XukIn
                            if (mXmlReader.LocalName == mDestXukAble.XukLocalName &&
                                mXmlReader.NamespaceURI == mDestXukAble.XukNamespaceUri)
                            {
                                foundXukAble = true;
                                mDestXukAble.XukIn(mXmlReader, this);
                            }
                            else if (!mXmlReader.IsEmptyElement)
                            {
                                mXmlReader.ReadSubtree().Close();
                            }
                        }
                        else if (mXmlReader.NodeType == XmlNodeType.EndElement)
                        {
                            break;
                        }

                        if (mXmlReader.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                    }
                }
                if (!foundXukAble)
                {
                    throw new exception.XukException("Found no required XukAble in Xuk file");
                }
            }
            catch (exception.ProgressCancelledException)
            {
                canceled = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Progress -= OpenXukAction_progress;

                closeInput();

                if (canceled) NotifyCancelled();
                else NotifyFinished();
            }
        }

        //private void OpenXukAction_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        //{
        //    if (mHasCancelBeenRequested) e.Cancel();
        //}

        private string m_ShortDescription = "Parsing XUK...";
        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public override string ShortDescription
        {
            get { return m_ShortDescription; }
            set { m_ShortDescription = value; }
        }

        private string m_LongDescription = "Parsing  a XUK XML file into an instance of the Urakawa SDK data model...";
        /// <summary>
        /// Get a long uman-readable description of the command
        /// </summary>
        public override string LongDescription
        {
            get { return m_LongDescription; }
            set { m_LongDescription = value; }
        }
        #endregion
    }
}