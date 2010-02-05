using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using urakawa.metadata;
using urakawa.metadata.daisy;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private readonly string m_outDirectory;
        private string m_Book_FilePath;

        public event ProgressChangedEventHandler ProgressChangedEvent;
        private void reportProgress(int percent, string msg)
        {
            reportSubProgress(-1, null);
            if (ProgressChangedEvent != null)
                ProgressChangedEvent(this, new ProgressChangedEventArgs(percent, msg));
        }

        public event ProgressChangedEventHandler SubProgressChangedEvent;
        private void reportSubProgress(int percent, string msg)
        {
            if (SubProgressChangedEvent != null)
                SubProgressChangedEvent(this, new ProgressChangedEventArgs(percent, msg));
        }

        private bool m_RequestCancellation;
        public bool RequestCancellation
        {
            get
            {
                return m_RequestCancellation;
            }
            set
            {
                m_RequestCancellation = value;
            }
        }

        private string m_Xuk_FilePath;
        public string XukPath
        {
            get { return m_Xuk_FilePath; }
        }

        private Project m_Project;
        public Project Project
        {
            get { return m_Project; }
        }

        protected Daisy3_Import(string bookfile, string outDir)
        {
            reportProgress(10, "Initializing import...");

            m_PackageUniqueIdAttr = null;
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

            reportProgress(50, "Initializing import...");

            if (RequestCancellation) return;
            initializeProject();

            reportProgress(100, "Import initialized.");
        }

        public Daisy3_Import(string bookfile)
            : this(bookfile, Path.GetDirectoryName(bookfile)) //Directory.GetParent(bookfile).FullName)
        { }

        public void DoImport()
        {
            if (RequestCancellation) return;
            transformBook();

            reportProgress(-1, "Saving XUK...");

            if (RequestCancellation) return;
            m_Xuk_FilePath = Path.Combine(m_outDirectory, Path.GetFileName(m_Book_FilePath) + ".xuk");
            m_Project.SaveXuk(new Uri(m_Xuk_FilePath));

            reportProgress(100, "XUK Saved.");

            if (RequestCancellation)
            {
                m_Xuk_FilePath = null;
                m_Project = null;
            }
        }

        private void initializeProject()
        {
            m_Project = new Project();
#if (DEBUG)
            m_Project.SetPrettyFormat(true);
#else
            m_Project.SetPrettyFormat(false);
#endif

            Presentation presentation = m_Project.AddNewPresentation();

            presentation.RootUri = new Uri(m_outDirectory);
            presentation.MediaDataManager.EnforceSinglePCMFormat = true;

            m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            m_textChannel.Name = "Our Text Channel";

            m_audioChannel = presentation.ChannelFactory.CreateAudioChannel();
            m_audioChannel.Name = "Our Audio Channel";

            m_ImageChannel = presentation.ChannelFactory.CreateImageChannel();
            m_ImageChannel.Name = "Our Image Channel";

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

            if (RequestCancellation) return;
            string fileExt = m_Book_FilePath.Substring(indexOfDot).ToLower();
            switch (fileExt)
            {
                case ".opf":
                    {
                        XmlDocument opfXmlDoc = readXmlDocument(m_Book_FilePath);

                        if (RequestCancellation) break;
                        reportProgress(-1, "Parsing OPF: [" + Path.GetFileName(m_Book_FilePath) + "]");
                        parseOpf(opfXmlDoc);
                        
                        break;
                    }
                case ".xml":
                    {
                        XmlDocument contentXmlDoc = readXmlDocument(m_Book_FilePath);

                        if (RequestCancellation) break;
                        reportProgress(-1, "Parsing metadata: [" + Path.GetFileName(m_Book_FilePath) + "]");
                        parseMetadata(contentXmlDoc);
                        
                        if (RequestCancellation) break;
                        reportProgress(-1, "Parsing content: [" + Path.GetFileName(m_Book_FilePath) + "]");
                        parseContentDocument(contentXmlDoc, null, m_Book_FilePath);
                        
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

            if (RequestCancellation) return;

            if (!String.IsNullOrEmpty(m_PublicationUniqueIdentifier))
            {
                Metadata meta = addMetadata("dc:Identifier", m_PublicationUniqueIdentifier, m_PublicationUniqueIdentifierNode);
                meta.IsMarkedAsPrimaryIdentifier = true;
            }
            //if no unique publication identifier could be determined, see how many identifiers there are
            //if there is only one, then make that the unique publication identifier
            //this code assumes that all metadata parsing has been completed, which seems to be the case
            //at the moment.  however, should additional documents start getting parsed for metadata,
            //then this code should be moved to a spot after that parsing has finished.
            else
            {
                if (m_Project.Presentations.Count > 0)
                {
                    List<Metadata> identifiers = new List<Metadata>();

                    foreach (Metadata md in m_Project.Presentations.Get(0).Metadatas.ContentsAs_YieldEnumerable)
                    {
                        //get this metadata's definition (and search synonyms too)
                        MetadataDefinition definition = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(
                            md.NameContentAttribute.Name, true);

                        //if this is a dc:identifier, then add it to our list
                        if (definition.Name == "dc:Identifier") identifiers.Add(md);
                    }

                    //if there is only one identifier, then make it the publication UID
                    if (identifiers.Count == 1)
                        identifiers[0].IsMarkedAsPrimaryIdentifier = true;
                }
            }

            if (RequestCancellation) return;

            reportProgress(100, "Transform complete");
        }

        private core.TreeNode getTreeNodeWithXmlElementId(string id)
        {
            Presentation pres = m_Project.Presentations.Get(0);
            return pres.RootNode.GetTreeNodeWithXmlElementId(id);
        }
    }
}
