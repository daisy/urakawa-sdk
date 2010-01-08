using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Cache;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private XmlDocument readXmlDocument(string path)
        {
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
                    Console.Write(e.Message);

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

        public class LocalXmlUrlResolver : XmlUrlResolver
        {
            private bool m_EnableHttpCaching;
            private ICredentials m_Credentials;
            private Dictionary<string, string> m_EmbeddedEntities;
            private string m_DownloadedDTDDirectoryPath;
            private string m_DownloadedDTDDirectoryName = "Tobi-Downloaded-DTDs" ;
            private System.IO.IsolatedStorage.IsolatedStorageFile m_IsolatedArea = System.IO.IsolatedStorage.IsolatedStorageFile.GetMachineStoreForApplication ();

            //resolve resources from cache (if possible) when m_EnableHttpCaching is set to true
            //resolve resources from source when enableHttpcaching is set to false 
            public LocalXmlUrlResolver(bool enableHttpCaching)
            {
                m_EnableHttpCaching = enableHttpCaching;
                
                //System.Windows.Forms.MessageBox.Show ( isolatedArea.GetDirectoryNames ("*.*)[0]  );
                m_DownloadedDTDDirectoryPath = Path.GetDirectoryName ( System.Reflection.Assembly.GetExecutingAssembly ().Location ) + Path.DirectorySeparatorChar + "Downloaded-DTDs";

                m_EmbeddedEntities = new Dictionary<String, String>();

                // -//W3C//DTD XHTML 1.0 Transitional//EN
                // http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd

                m_EmbeddedEntities.Add("dtbook110.dtd", "DTDs.Resources.dtbook110.dtd");

                m_EmbeddedEntities.Add("xhtml-lat1.ent", "DTDs.Resources.xhtml-lat1.ent");
                m_EmbeddedEntities.Add("xhtml-symbol.ent", "DTDs.Resources.xhtml-symbol.ent");
                m_EmbeddedEntities.Add("xhtml-special.ent", "DTDs.Resources.xhtml-special.ent");

                m_EmbeddedEntities.Add("HTMLlat1", "DTDs.Resources.xhtml-lat1.ent");
                m_EmbeddedEntities.Add("HTMLsymbol", "DTDs.Resources.xhtml-symbol.ent");
                m_EmbeddedEntities.Add("HTMLspecial", "DTDs.Resources.xhtml-special.ent");

                m_EmbeddedEntities.Add("//W3C//ENTITIES%20Latin%201%20for%20XHTML//EN", "DTDs.Resources.xhtml-lat1.ent");
                m_EmbeddedEntities.Add("//W3C//ENTITIES%20Symbols%20for%20XHTML//EN", "DTDs.Resources.xhtml-symbol.ent");
                m_EmbeddedEntities.Add("//W3C//ENTITIES%20Special%20for%20XHTML//EN", "DTDs.Resources.xhtml-special.ent");

                m_EmbeddedEntities.Add("//W3C//DTD%20XHTML%201.0%20Transitional//EN", "DTDs.Resources.xhtml1-transitional.dtd");
                m_EmbeddedEntities.Add("//W3C//DTD%20XHTML%201.1//EN", "DTDs.Resources.xhtml11.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20ncx%202005-1//EN", "DTDs.Resources.ncx-2005-1.dtd");
                m_EmbeddedEntities.Add("//W3C//DTD%20XHTML%201.1%20plus%20MathML%202.0%20plus%20SVG%201.1//EN", "DTDs.Resources.xhtml-math-svg-flat.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbook%202005-1//EN", "DTDs.Resources.dtbook-2005-1.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbook%202005-2//EN", "DTDs.Resources.dtbook-2005-2.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbook%202005-3//EN", "DTDs.Resources.dtbook-2005-3.dtd");
                m_EmbeddedEntities.Add("//W3C//ENTITIES%20MathML%202.0%20Qualified%20Names%201.0//EN", "DTDs.Resources.mathml2.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbsmil%202005-2//EN", "DTDs.Resources.dtbsmil-2005-2.dtd");
                m_EmbeddedEntities.Add("//ISBN%200-9673008-1-9//DTD%20OEB%201.2%20Package//EN", "DTDs.Resources.oebpkg12.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbsmil%202005-1//EN", "DTDs.Resources.dtbsmil-2005-1.dtd");
  
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
                if (m_IsolatedArea.GetFileNames ( Path.GetFileName ( absoluteUri.AbsolutePath ) ).Length > 0)
                    {
                    
                    IsolatedStorageFileStream storageStream =
                      new IsolatedStorageFileStream ( Path.GetFileName ( absoluteUri.AbsolutePath ),
                      FileMode.Open, m_IsolatedArea);
                    
                        return storageStream;
                        
                    }

                //resolve resources from cache (if possible)
                if (absoluteUri.Scheme == "http" && m_EnableHttpCaching && (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream)))
                {
                    WebRequest webReq = WebRequest.Create(absoluteUri);
                    webReq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                    if (m_Credentials != null)
                    {
                        webReq.Credentials = m_Credentials;
                    }
                    WebResponse resp = webReq.GetResponse();
                    Stream webStream = resp.GetResponseStream ();
                    // create local DTD file from webStream
                    CreateLocalDTDFileFromWebStream ( absoluteUri.AbsolutePath, webStream );
                    return webStream ;
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
                foreach (String key in m_EmbeddedEntities.Keys)
                {
                    if (absoluteUri.AbsolutePath.Contains(key))
                    {
                        dtdStream = DTDs.DTDs.Fetch(m_EmbeddedEntities[key]);
                        Console.WriteLine("XML Entity Resolver [" + m_EmbeddedEntities[key] + "]: " + (dtdStream != null ? dtdStream.Length + " bytes resource. " : "resource not found ?? ") + " ( " + absoluteUri + " )");
                        return dtdStream;
                    }
                }

                return null;
            }
            
            private void CreateLocalDTDFileFromWebStream (string strWebDTDPath, Stream webStream  )
                {
                if (!Directory.Exists ( m_DownloadedDTDDirectoryPath ))
                    {
                    Directory.CreateDirectory ( m_DownloadedDTDDirectoryPath );
                    }
                string localDTDFilePath = Path.Combine ( m_DownloadedDTDDirectoryPath,
                    Path.GetFileName ( strWebDTDPath ) );
                FileStream fs = null;
                try
                    {
                    
                    //fs = File.Create ( localDTDFilePath );
                    fs = new IsolatedStorageFileStream ( Path.GetFileName ( strWebDTDPath ),
  FileMode.Create, m_IsolatedArea );
                    copyStreamData ( webStream, fs, 1024 );
                    }
                
                finally
                    {
                    if (fs != null)
                        {
                        fs.Close ();
                        fs = null;
                        }
                    
                    }
                }

            public void copyStreamData ( Stream source, Stream dest, int BUFFER_SIZE )
                { //1
                
                if (source.CanSeek &&  source.Length <= BUFFER_SIZE )
                    { // 2
                    byte[] buffer = new byte[source.Length];
                    int read = source.Read ( buffer, 0, (int)source.Length );
                    dest.Write ( buffer, 0, read );
                    } //-2
                else
                    { // 2
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead = 0;
                    while ((bytesRead = source.Read ( buffer, 0, BUFFER_SIZE )) > 0)
                        { //3
                        dest.Write ( buffer, 0, bytesRead );
                        } // -3

                    } //-2
                } // -1

        }
    }
}
