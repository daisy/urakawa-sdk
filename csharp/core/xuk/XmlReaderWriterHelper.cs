﻿using System;
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

        public const string XmlId = NS_PREFIX_XML + ":id";
        public const string XmlLang = NS_PREFIX_XML + ":lang";

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
                    ((XmlTextReader)xmlReader).WhitespaceHandling = WhitespaceHandling.All;
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

        public static XmlWriterSettings GetDefaultXmlWriterConfiguration(bool pretty)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Encoding = Encoding.UTF8;

            settings.NewLineHandling = NewLineHandling.Replace;
            settings.NewLineChars = Environment.NewLine;

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

        public static void WriteXmlDocument(XmlDocument xmlDoc, string path)
        {
            const bool pretty = true;

            xmlDoc.PreserveWhitespace = false;
            xmlDoc.XmlResolver = null;

            XmlWriterSettings settings = GetDefaultXmlWriterConfiguration(pretty);

            using (XmlWriter xmlWriter = XmlWriter.Create(path, settings))
            {
                if (pretty && xmlWriter is XmlTextWriter)
                {
                    ((XmlTextWriter)xmlWriter).Formatting = Formatting.Indented;
                }

                try
                {
                    xmlDoc.Save(xmlWriter);
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
                    xmlWriter.Close();
                }
            }
        }
    }
}
