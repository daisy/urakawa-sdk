using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import : DualCancellableProgressReporter
    {
        private readonly string m_outDirectory;
        private string m_Book_FilePath;


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

        public Daisy3_Import(string bookfile, string outDir)
        {
            reportProgress(10, UrakawaSDK_daisy_Lang.InitializeImport);                               

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

            m_Xuk_FilePath = Path.Combine(m_outDirectory, Path.GetFileName(m_Book_FilePath) + ".xuk");

            reportProgress(50, UrakawaSDK_daisy_Lang.InitializeImport);                             

            if (RequestCancellation) return;
            initializeProject();

            reportProgress(100, UrakawaSDK_daisy_Lang.ImportInitialized);                                 
        }

        public override void DoWork()
        {
            if (RequestCancellation) return;
            transformBook();

            reportProgress(-1, UrakawaSDK_daisy_Lang.SaveXUK);                                       

            if (RequestCancellation) return;

            //m_Project.SaveXuk(new Uri(m_Xuk_FilePath));


            var action = new SaveXukAction(m_Project, m_Project, new Uri(m_Xuk_FilePath))
            {
                ShortDescription = UrakawaSDK_daisy_Lang.SavingXUKFile,                               
                LongDescription = UrakawaSDK_daisy_Lang.SerializeDOMIntoXUKFile             
            };

            action.Progress += (sender, e) =>
            {
                double val = e.Current;
                double max = e.Total;
                
                var percent = -1;
                if (val != max)
                {
                    percent = (int) ((val/max)*100);
                }

                reportProgress(percent, action.ShortDescription);
                reportSubProgress(-1, action.LongDescription);

                if (RequestCancellation)
                {
                    e.Cancel();
                }
            };
            action.Finished += (sender, e) => reportProgress(100, UrakawaSDK_daisy_Lang.XUKSaved);                 
            action.Cancelled += (sender, e) => reportProgress(0, UrakawaSDK_daisy_Lang.CancelledXUKSaving);        

            action.Execute();

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

            Presentation presentation = m_Project.AddNewPresentation(new Uri(m_outDirectory), Path.GetFileName(m_Book_FilePath));

            presentation.MediaDataManager.EnforceSinglePCMFormat = true;

            m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            m_textChannel.Name = "The Text Channel";

            m_audioChannel = presentation.ChannelFactory.CreateAudioChannel();
            m_audioChannel.Name = "The Audio Channel";

            m_ImageChannel = presentation.ChannelFactory.CreateImageChannel();
            m_ImageChannel.Name = "The Image Channel";

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
                        reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingOPF, Path.GetFileName(m_Book_FilePath)));  
                        parseOpf(opfXmlDoc);
                        
                        break;
                    }
                case ".xml":
                    {
                        XmlDocument contentXmlDoc = readXmlDocument(m_Book_FilePath);

                        if (RequestCancellation) break;
                        reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, Path.GetFileName(m_Book_FilePath)));  
                        parseMetadata(contentXmlDoc);
                        
                        if (RequestCancellation) break;
                        reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, Path.GetFileName(m_Book_FilePath)));  
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
                    {
                        identifiers[0].IsMarkedAsPrimaryIdentifier = true;

                        //if dtb:uid is our only identifier, rename it dc:identifier
                        if (identifiers[0].NameContentAttribute.Name == "dtb:uid")
                            identifiers[0].NameContentAttribute.Name = "dc:Identifier";
                    }
                }
            }

            //add any missing required metadata entries
            IEnumerable<Metadata> metadatas =
                m_Project.Presentations.Get(0).Metadatas.ContentsAs_YieldEnumerable;
            foreach (MetadataDefinition metadataDefinition in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
            {
                if (!metadataDefinition.IsReadOnly && metadataDefinition.Occurrence == MetadataOccurrence.Required)
                {
                    bool found = false;
                    foreach (Metadata m in metadatas)
                    {
                        if (m.NameContentAttribute.Name.ToLower() == metadataDefinition.Name.ToLower())
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Metadata metadata = m_Project.Presentations.Get(0).MetadataFactory.CreateMetadata();
                        metadata.NameContentAttribute = new MetadataAttribute();
                        metadata.NameContentAttribute.Name = metadataDefinition.Name;
                        metadata.NameContentAttribute.Value = SupportedMetadata_Z39862005.MagicStringEmpty;
                        m_Project.Presentations.Get(0).Metadatas.Insert
                            (m_Project.Presentations.Get(0).Metadatas.Count, metadata);
                    }
                    
                }
            }

            

            if (RequestCancellation) return;

            reportProgress(100, UrakawaSDK_daisy_Lang.TransformComplete);                                   
        }

        private core.TreeNode getTreeNodeWithXmlElementId(string id)
        {
            Presentation pres = m_Project.Presentations.Get(0);
            return pres.RootNode.GetTreeNodeWithXmlElementId(id);
        }
    }
}
