using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Xml;
using urakawa;
using urakawa.media.data;
using urakawa.property.channel;
using core = urakawa.core;
using System.Windows.Forms;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private readonly string m_outDirectory;
        private string m_Book_FilePath;

        private Project m_Project;
        public Project Project
        {
            get { return m_Project; }
        }

        public DaisyToXuk(string bookfile, string outDir)
        {
            m_Book_FilePath = bookfile;
            m_outDirectory = outDir;
            if (!m_outDirectory.EndsWith("" + Path.DirectorySeparatorChar))
            {
                m_outDirectory += Path.DirectorySeparatorChar;
            }
            if (!Directory.Exists(m_outDirectory))
            {
                Directory.CreateDirectory(m_outDirectory);
            }
            initializeProject();
            transformBook();

            string xukPath = Path.Combine(m_outDirectory, Path.GetFileName(m_Book_FilePath) + ".xuk");
            m_Project.SaveXuk(new Uri(xukPath));
        }

        public DaisyToXuk(string bookfile)
            : this(bookfile, Path.GetDirectoryName(bookfile)) //Directory.GetParent(bookfile).FullName)
        { }

        private void initializeProject()
        {
            m_Project = new Project();
            m_Project.SetPrettyFormat(true);

            Presentation presentation = m_Project.AddNewPresentation();

            presentation.RootUri = new Uri(m_outDirectory);
            presentation.MediaDataManager.EnforceSinglePCMFormat = true;

            // BEGIN OF INIT FACTORIES
            // => creating all kinds of objects in order to initialize the factories
            // and cache the mapping between XUK names (pretty or compressed) and actual types.
            Channel ch = presentation.ChannelFactory.Create();
            presentation.ChannelsManager.RemoveChannel(ch);
            ch = presentation.ChannelFactory.CreateAudioChannel();
            presentation.ChannelsManager.RemoveChannel(ch);
            ch = presentation.ChannelFactory.CreateTextChannel();
            presentation.ChannelsManager.RemoveChannel(ch);
            //
            DataProvider dp = presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            presentation.DataProviderManager.RemoveDataProvider(dp, true);
            //
            MediaData md = presentation.MediaDataFactory.CreateAudioMediaData();
            presentation.MediaDataManager.RemoveMediaData(md);
            //
            presentation.CommandFactory.CreateCompositeCommand();
            //
            presentation.MediaFactory.CreateExternalImageMedia();
            presentation.MediaFactory.CreateExternalVideoMedia();
            presentation.MediaFactory.CreateExternalTextMedia();
            presentation.MediaFactory.CreateExternalAudioMedia();
            presentation.MediaFactory.CreateManagedAudioMedia();
            presentation.MediaFactory.CreateSequenceMedia();
            presentation.MediaFactory.CreateTextMedia();
            //
            presentation.MetadataFactory.CreateMetadata();
            //
            presentation.PropertyFactory.CreateChannelsProperty();
            presentation.PropertyFactory.CreateXmlProperty();
            //
            presentation.TreeNodeFactory.Create();
            //
            // END OF INIT FACTORIES

            m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            m_textChannel.Name = "Our Text Channel";

            m_audioChannel = presentation.ChannelFactory.CreateAudioChannel();
            m_audioChannel.Name = "Our Audio Channel";

            /*string dataPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
           if (Directory.Exists(dataPath))
           {
               Directory.Delete(dataPath, true);
           }*/
        }

        private void transformBook()
        {
            //FileInfo DTBFilePathInfo = new FileInfo(m_Book_FilePath);
            //switch (DTBFilePathInfo.Extension)

            int indexOfDot = m_Book_FilePath.LastIndexOf('.');
            if (indexOfDot < 0 || indexOfDot == m_Book_FilePath.Length - 1)
            {
                return;
            }

            string fileExt = m_Book_FilePath.Substring(indexOfDot);
            switch (fileExt)
            {
                case ".opf":
                    {
                        XmlDocument opfXmlDoc = readXmlDocument(m_Book_FilePath);
                        parseOpf(opfXmlDoc);
                        break;
                    }
                case ".xml":
                    {
                        XmlDocument contentXmlDoc = readXmlDocument(m_Book_FilePath);
                        parseContentDocument(contentXmlDoc, null);
                        break;
                    }
                case ".epub":
                    {
                        unzipEPubAndParseOpf();
                        break;
                    }
                default:
                    break;
            }
        }

        private XmlDocument readXmlDocument(string path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.None;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.XmlResolver = new LocalXmlUrlResolver(true);

            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
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
                    // No message box: use debugging instead (inspect stack trace, watch values)
                    //MessageBox.Show(e.ToString());

                    // The Fail() method is better:
                    //System.Diagnostics.Debug.Fail(e.Message);

                    //Or you can explicitely break:
                    System.Diagnostics.Debugger.Break();
                }
                finally
                {
                    xmlReader.Close();
                }

                return xmldoc;
            }
        }

        private core.TreeNode getTreeNodeWithXmlElementId(string id)
        {
            Presentation pres = m_Project.GetPresentation(0);
            return getTreeNodeWithXmlElementId(pres.RootNode, id);
        }

        private static core.TreeNode getTreeNodeWithXmlElementId(core.TreeNode node, string id)
        {
            if (node.GetXmlElementId() == id) return node;

            for (int i = 0; i < node.ChildCount; i++)
            {
                core.TreeNode child = getTreeNodeWithXmlElementId(node.GetChild(i), id);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }
    }

    public class LocalXmlUrlResolver : XmlUrlResolver
    {
        bool enableHttpCaching;
        ICredentials credentials;

        //resolve resources from cache (if possible) when enableHttpCaching is set to true
        //resolve resources from source when enableHttpcaching is set to false 
        public LocalXmlUrlResolver(bool enableHttpCaching)
        {
            this.enableHttpCaching = enableHttpCaching;
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
                credentials = value;
                base.Credentials = value;
            }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null)
            {
                throw new ArgumentNullException("absoluteUri");
            }
             Uri localURI = mapUri(absoluteUri);
            if (localURI != null)
            {
                absoluteUri = localURI;
            }
            //resolve resources from cache (if possible)
            if (absoluteUri.Scheme == "http" && enableHttpCaching && (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream)))
            {
                WebRequest webReq = WebRequest.Create(absoluteUri);
                webReq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                if (credentials != null)
                {
                    webReq.Credentials = credentials;
                }
                WebResponse resp = webReq.GetResponse();
                return resp.GetResponseStream();
            }
            //otherwise use the default behavior of the XmlUrlResolver class (resolve resources from source)
            
            if (absoluteUri.Scheme == "file" && !File.Exists(absoluteUri.LocalPath))
            {
                return null;
            }
            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        public Uri mapUri(Uri absoluteUri)
        {
            bool flag = false;
            string dtdDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalDTD");
            if (absoluteUri.AbsolutePath.Contains("W3C//DTD%20XHTML%201.1//EN"))
            {              
                string xhtml = Path.Combine(dtdDir, "xhtml11.dtd");
                absoluteUri = new Uri(xhtml);
                flag = true;
            }
            else if (absoluteUri.AbsolutePath.Contains("NISO//DTD%20ncx%202005-1//EN"))
            {
                string ncx = Path.Combine(dtdDir,"ncx-2005-1.dtd");
                absoluteUri = new Uri(ncx);
                flag = true;
            }
            else if (absoluteUri.AbsolutePath.Contains("W3C//DTD XHTML%201.1%20plus%20MathML%202.0%20plus%20SVG%201.1//EN"))
            {
                string xhtmlMathSvg = Path.Combine(dtdDir, "xhtml-math-svg-flat.dtd");
                absoluteUri = new Uri(xhtmlMathSvg);
                flag = true;
            }
            else if (absoluteUri.AbsolutePath.Contains("NISO//DTD%20dtbook%202005-1//EN"))
            {
                string dtb = Path.Combine(dtdDir, "dtbook-2005-1.dtd");
                absoluteUri = new Uri(dtb);
                flag = true;
            }
            else if (absoluteUri.AbsolutePath.Contains("NISO//DTD%20dtbook%202005-2//EN"))
            {
                string dtb = Path.Combine(dtdDir, "dtbook-2005-2.dtd");
                absoluteUri = new Uri(dtb);
                flag = true;
            }
            else if (absoluteUri.AbsolutePath.Contains("NISO//DTD dtbook 2005-3//EN"))
            {
                string dtb = Path.Combine(dtdDir, "dtbook-2005-3");
                absoluteUri = new Uri(dtb);
                flag = true;
            }
            else if (absoluteUri.AbsolutePath.Contains("W3C//ENTITIES%20MathML%202.0%20Qualified%20Names%201.0//EN"))
            {
                string mathML = Path.Combine(dtdDir, "mathml2.dtd");
                absoluteUri = new Uri(mathML);
                flag = true;
            }
            else if (absoluteUri.AbsolutePath.Contains("NISO//DTD%20dtbsmil%202005-2//EN"))
            {
                string smilDtd = Path.Combine(dtdDir, "dtbsmil-2005-2.dtd");
                absoluteUri = new Uri(smilDtd);
                flag = true;
            }
            
            if (flag == true)
                return absoluteUri;
            else
                return null;
        }
   }
}
