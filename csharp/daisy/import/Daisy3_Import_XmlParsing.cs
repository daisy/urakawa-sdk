using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Xml;
using AudioLib;
using urakawa.ExternalFiles;

#if USE_ISOLATED_STORAGE
using System.IO.IsolatedStorage;
#endif //USE_ISOLATED_STORAGE

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private XmlDocument readXmlDocument(string path)
        {   
            reportSubProgress(-1, "Reading XML document [" + Path.GetFileName(path) + "]...");                 // TODO LOCALIZE ReadXMLDoc

            XmlReaderSettings settings = new XmlReaderSettings();

            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.None;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.XmlResolver = new LocalXmlUrlResolver(true);

            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = false;
            settings.IgnoreWhitespace = true;

            using (XmlReader xmlReader = XmlReader.Create(path, settings))
            {
                XmlDocument xmldoc = new XmlDocument();
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

                reportSubProgress(100, "XML document loaded [" + path + "].");            // TODO LOCALIZE XmlDocLoaded
                return xmldoc;
            }
        }

        public class LocalXmlUrlResolver : XmlUrlResolver
        {
            private ICredentials m_Credentials;

            private readonly bool m_EnableHttpCaching;
            private const string m_DtdStoreDirName = "Downloaded-DTDs";                    // TODO LOCALIZE DtdStoreDirName

            public LocalXmlUrlResolver(bool enableHttpCaching)
            {
                m_EnableHttpCaching = enableHttpCaching;
            }

            public override Uri ResolveUri(Uri baseUri, string relativeUri)
            {
                if ((baseUri == null) || (!baseUri.IsAbsoluteUri && (baseUri.OriginalString.Length == 0)))
                {
                    var uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
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
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
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
    }
}
