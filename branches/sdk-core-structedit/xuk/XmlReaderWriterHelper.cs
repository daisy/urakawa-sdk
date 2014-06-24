using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Xml;
using AudioLib;
using urakawa.ExternalFiles;
using urakawa.data;

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
                Uri uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri && (uri.OriginalString.Length > 0))
                {
                    uri = new Uri(Path.GetFullPath(relativeUri));
                }
                return uri;
            }

            if (!String.IsNullOrEmpty(relativeUri))
            {
                Uri uriOut = new Uri(baseUri, relativeUri);
                return uriOut;
            }
            else
            {
                return baseUri;
            }
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

            bool isHTTP = absoluteUri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                          || absoluteUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);

            // Resolve local known entities
            string uniqueResourceId;
            Stream localStream = mapUri(absoluteUri, out uniqueResourceId);
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

            string absPath = absoluteUri.AbsolutePath;

            string ext = Path.GetExtension(absPath);
            if (!isHTTP
                &&
                (
                DataProviderFactory.XML_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                || DataProviderFactory.HTML_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                || DataProviderFactory.XHTML_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                || DataProviderFactory.NCX_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                || ".opf".Equals(ext, StringComparison.OrdinalIgnoreCase)
                || DataProviderFactory.IMAGE_SVG_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                || DataProviderFactory.XSLT_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                || DataProviderFactory.XSL_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                )
            {
                if (File.Exists(absPath))
                {
                    Stream stream = File.Open(absPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return stream;
                }
            }

            string filename = Path.GetFileName(absPath);

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
                Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return stream;
            }
#endif //USE_ISOLATED_STORAGE


            //resolve resources from cache (if possible)
            if (m_EnableHttpCaching &&
                isHTTP //absoluteUri.IsUnc?
                )
            {
#if DEBUG
                Debugger.Break();
#endif
                WebRequest webReq = WebRequest.Create(absoluteUri);
                webReq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                if (m_Credentials != null)
                {
                    webReq.Credentials = m_Credentials;
                }
                WebResponse resp = null;
                try
                {
                    resp = webReq.GetResponse();
                    Stream webStream = resp.GetResponseStream();

                    string localpath = CreateLocalDTDFileFromWebStream(absoluteUri.AbsolutePath, webStream);

                    return File.Open(localpath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
                finally
                {
                    //webStream.Close();
                    if (resp != null)
                    {
                        resp.Close();
                    }
                }
            }

            // No need to look for a local file that does not exist.
            //if (absoluteUri.Scheme == "file" && !File.Exists(absoluteUri.LocalPath))
            //{
            //return null;
            //}

            //otherwise use the default behavior of the XmlUrlResolver class (resolve resources from source)
            try
            {
#if DEBUG
                //Debugger.Break();
#endif
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
            catch
            {
                return null;
            }
        }

        public static Stream mapUri(Uri absoluteUri, out string uniqueResourceId)
        {
            //if (!absoluteUri.AbsolutePath.EndsWith("opf")
            //    && !absoluteUri.AbsolutePath.EndsWith("xhtml")
            //    && !absoluteUri.AbsolutePath.EndsWith("html"))
            //{
            //    Debugger.Break();
            //}

            string normalisedUri = absoluteUri.AbsolutePath.Replace("%20", " ").Replace(" //", "//").Replace("// ", "//");

            Stream dtdStream = null;
            foreach (String key in DTDs.DTDs.ENTITIES_MAPPING.Keys)
            {
                if (normalisedUri.Contains(key))
                {
                    string resource = DTDs.DTDs.ENTITIES_MAPPING[key];
                    dtdStream = DTDs.DTDs.Fetch(resource);
                    if (dtdStream == null)
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                    }

                    Console.WriteLine("XML Entity Resolver [" + resource + "]: " + (dtdStream != null ? dtdStream.Length + " bytes resource. " : "resource not found?! ") + " ( " + absoluteUri + " )");

                    uniqueResourceId = resource;
                    return dtdStream;
                }
            }

            uniqueResourceId = null;
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
                    FileDataProvider.CreateDirectory(dirpath);
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

    public class XmlReaderWriterHelper
    {
        public const string NS_PREFIX_XML = "xml";
        public const string NS_URL_XML = "http://www.w3.org/XML/1998/namespace";

        public const string NS_PREFIX_XMLNS = "xmlns";
        public const string NS_URL_XMLNS = "http://www.w3.org/2000/xmlns/";

        public const string XmlId = NS_PREFIX_XML + ":id";
        public const string XmlLang = NS_PREFIX_XML + ":lang";

        public static XmlReaderSettings GetDefaultXmlReaderConfiguration(bool useLocalXmlResolver, bool preserveWhiteSpace, bool validate)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            if (validate)
            {
#if NET40
                //settings.ProhibitDtd = false;
                settings.DtdProcessing = DtdProcessing.Parse;
#else
                settings.ProhibitDtd = false;
#endif //NET40

                settings.ValidationType = ValidationType.DTD;
                settings.ValidationEventHandler += XmlValidationEventHandler;
            }
            else
            {
#if NET40
                //settings.ProhibitDtd = false;
                settings.DtdProcessing = DtdProcessing.Ignore;
#else
                settings.ProhibitDtd = false;
#endif //NET40

                settings.ValidationType = ValidationType.None;
            }

            settings.ConformanceLevel = ConformanceLevel.Document;

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

        static void XmlValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            bool debug = true; // ignore
        }

        public static XmlDocument ParseXmlDocumentFromString(string content, bool preserveWhiteSpace, bool validate)
        {
            XmlDocument xmldoc = null;

            XmlReaderSettings settings = GetDefaultXmlReaderConfiguration(true, preserveWhiteSpace, validate);

            XmlReader xmlReader = null;
            try
            {
                TextReader reader = new StringReader(content);

                xmlReader = XmlReader.Create(reader, settings);

                if (xmlReader is XmlTextReader)
                {
                    if (preserveWhiteSpace)
                    {
                        ((XmlTextReader)xmlReader).WhitespaceHandling = WhitespaceHandling.All;
                    }
                    else
                    {
                        ((XmlTextReader)xmlReader).WhitespaceHandling = WhitespaceHandling.None;
                    }
                }
                xmldoc = new XmlDocument();
                xmldoc.PreserveWhitespace = preserveWhiteSpace;
                xmldoc.XmlResolver = null;
                xmldoc.Load(xmlReader);

            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new Exception("ParseXmlFromString", ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    try
                    {
                        xmlReader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!! xmlReader.Close();" + ex.Message);
                    }
                }
            }

            return xmldoc;
        }

        public static XmlDocument ParseXmlDocument(string filePath, bool preserveWhiteSpace, bool validate)
        {
            XmlDocument xmldoc = null;

            XmlReaderSettings settings = GetDefaultXmlReaderConfiguration(true, preserveWhiteSpace, validate);

            XmlReader xmlReader = null;
            Stream stream = null;
            try
            {
                DebugFix.Assert(File.Exists(filePath));

                stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                TextReader reader = new StreamReader(stream, Encoding.UTF8);

                Uri uri = new Uri(@"file:///" + filePath.Replace('\\', '/'), UriKind.Absolute);
                string uriStr = uri.ToString();

                xmlReader = XmlReader.Create(reader, settings, uriStr);

                if (xmlReader is XmlTextReader)
                {
                    if (preserveWhiteSpace)
                    {
                        ((XmlTextReader)xmlReader).WhitespaceHandling = WhitespaceHandling.All;
                    }
                    else
                    {
                        ((XmlTextReader)xmlReader).WhitespaceHandling = WhitespaceHandling.None;
                    }
                }
                xmldoc = new XmlDocument();
                xmldoc.PreserveWhitespace = preserveWhiteSpace;
                xmldoc.XmlResolver = null;
                xmldoc.Load(xmlReader);

            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new Exception("ParseXmlFromFile", ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    try
                    {
                        xmlReader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!! xmlReader.Close();" + ex.Message);
                    }
                }
                if (stream != null)
                {
                    try
                    {
                        stream.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!! stream.Close();" + ex.Message);
                    }
                    try
                    {
                        stream.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!! stream.Dispose();" + ex.Message);
                    }
                }
            }

            return xmldoc;
        }

        public static XmlWriterSettings GetDefaultXmlWriterConfiguration(bool pretty)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Encoding = Encoding.UTF8;

            settings.NewLineHandling = NewLineHandling.Replace;
            settings.NewLineChars = "\n"; // Environment.NewLine;

            if (!pretty)
            {
                settings.Indent = false;
                settings.NewLineOnAttributes = false;
            }
            else
            {
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineOnAttributes = true;
            }

            return settings;
        }

        public static void WriteXmlDocument(XmlDocument xmlDoc, string path, XmlWriterSettings settings)
        {
            string parentDir = Path.GetDirectoryName(path);
            if (!Directory.Exists(parentDir))
            {
                FileDataProvider.CreateDirectory(parentDir);
            }

            const bool pretty = true;

            xmlDoc.PreserveWhitespace = false;
            xmlDoc.XmlResolver = null;

            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            if (settings == null)
            {
                settings = GetDefaultXmlWriterConfiguration(pretty);
            }
            XmlWriter xmlWriter = null;
            try
            {
                xmlWriter = XmlWriter.Create(fileStream, settings);

                if (pretty && xmlWriter is XmlTextWriter)
                {
                    ((XmlTextWriter)xmlWriter).Formatting = Formatting.Indented;
                }

                xmlDoc.Save(xmlWriter);
            }
            finally
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }
    }
}
