using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using AudioLib;
using ICSharpCode.SharpZipLib.Zip;
using urakawa;
using urakawa.core;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.property.channel;
using core = urakawa.core;
using urakawa.property.xml;

namespace XukImport
{
    /// <summary>
    /// This Class takes care of creating  XUK files of EPUB files.
    /// </summary>
    public partial class DaisyToXuk
    {
        private XmlDocument ncxXmlDoc;
        Presentation presentation;
        
        private void unZipePub()
        {
            ZipInputStream unzipEpub = new ZipInputStream(File.OpenRead(m_Book_FilePath));
            ZipEntry theEntry; //Files in the archive


            string directoryName = Path.GetTempPath();   //Temporary directory to store unzipped files
            string unzippedEpubDir = Path.Combine(directoryName, "epub");

            if (Directory.Exists(unzippedEpubDir))
            {
                Directory.Delete(unzippedEpubDir, true);
            }
            if (!directoryName.EndsWith("" + Path.DirectorySeparatorChar))
            {
                directoryName += Path.DirectorySeparatorChar;
            }
            while ((theEntry = unzipEpub.GetNextEntry()) != null)
            {
                string fileName = Path.GetFileName(theEntry.Name);

                if (!String.IsNullOrEmpty(fileName))
                {
                    if (theEntry.Name.IndexOf(".ini") < 0)
                    {
                        string fullPath = unzippedEpubDir + Path.DirectorySeparatorChar + theEntry.Name;
                        string fullDirPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                        FileStream streamWriter = File.Create(fullPath);

                        byte[] data = new byte[2 * 1024]; // 2 KB buffer
                        int bytesRead = 0;
                        try
                        {
                            while ((bytesRead = unzipEpub.Read(data, 0, data.Length)) > 0)
                            {
                                streamWriter.Write(data, 0, bytesRead);
                            }
                        }
                        finally
                        {
                            streamWriter.Close();
                        }
                    }
                }
            }

            var dirEpub = new DirectoryInfo(unzippedEpubDir);
            FileInfo[] opfFile = dirEpub.GetFiles("*.opf ", SearchOption.AllDirectories);

            foreach (FileInfo info in opfFile)
            {
                string filepath = info.FullName;
                string fullfilepath = Path.Combine(unzippedEpubDir, filepath);

                XmlDocument xmldoc = readXmlDocument(fullfilepath);
                parseEPUBandPopulateDataModel(fullfilepath);
            }
            unzipEpub.Close();

            // Directory.Delete(directoryName, true);
        }//unZipePub()

        private void parseEpubOpfMetadata(XmlDocument opfXmlDoc)
        {
            presentation = m_Project.GetPresentation(0);

            XmlNodeList listOfMetaDataRootNodes = opfXmlDoc.GetElementsByTagName("metadata");
            if (listOfMetaDataRootNodes != null)
            {
                foreach (XmlNode mdNodeRoot in listOfMetaDataRootNodes)
                {
                    XmlNodeList listOfMetaDataNodes = mdNodeRoot.ChildNodes;
                    if (listOfMetaDataNodes != null)
                    {
                        foreach (XmlNode mdNode in listOfMetaDataNodes)
                        {
                            if (mdNode.NodeType == XmlNodeType.Element && mdNode.Name.StartsWith("dc"))
                            {
                                Metadata md = presentation.MetadataFactory.CreateMetadata();
                                md.Name = mdNode.Name;
                                md.Content = mdNode.InnerText;
                                presentation.AddMetadata(md);
                            }

                            if (mdNode.NodeType == XmlNodeType.Element && mdNode.Name == "meta")
                            {
                                XmlAttributeCollection mdAttributes = mdNode.Attributes;

                                if (mdAttributes != null)
                                {
                                    XmlNode attrName = mdAttributes.GetNamedItem("name");
                                    XmlNode attrContent = mdAttributes.GetNamedItem("content");
                                    if (attrName != null && attrContent != null)
                                    {
                                        Metadata md = presentation.MetadataFactory.CreateMetadata();
                                        md.Name = attrName.Value;
                                        md.Content = attrContent.Value;
                                        presentation.AddMetadata(md);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }//parseePubOpfMetaData


        private void parseEPUBandPopulateDataModel(string epubFilePath)
        {
            string dirPath = Path.GetDirectoryName(epubFilePath);

            XmlDocument opfXmlDoc = readXmlDocument(epubFilePath);

            parseEpubOpfMetadata(opfXmlDoc);
            
            List<string> spineListOfHtmlFiles;
            string ncxPath;
            string DtBookPath;
            string fullNcxPath;           

            parseEpubOpfManifestAndSpine(opfXmlDoc, out DtBookPath, out spineListOfHtmlFiles, out ncxPath);
            
            if (ncxPath != null)
            {
                fullNcxPath = Path.Combine(dirPath, ncxPath);
                parseEpubNcx(fullNcxPath);
            }
            if (spineListOfHtmlFiles.Count != 0)
            {
                parseHtml(dirPath, spineListOfHtmlFiles);
            }

        }//parseEPUBandPopulateDataModel()

        private void parseEpubOpfManifestAndSpine(XmlDocument opfXmlDoc, out string DtBookPath, out List<string> spineListOfHtmlFiles, out string ncxPath)
        {
            spineListOfHtmlFiles = new List<string>();

            ncxPath = null;
            DtBookPath = null;

            XmlNodeList listOfSpineRootNodes = opfXmlDoc.GetElementsByTagName("spine");
            for (int x = 0; x < listOfSpineRootNodes.Count; x++)
            {
                XmlAttributeCollection spineAttributes = listOfSpineRootNodes[x].Attributes;
                string id = spineAttributes[0].Value;

                if (spineAttributes != null)
                {
                    XmlNode attrToc = spineAttributes.GetNamedItem("toc");
                    XmlNode attrPageMap = spineAttributes.GetNamedItem("page-map");
                    if (attrToc != null && attrPageMap != null)
                    {
                        if (attrToc.Value == "ncx" || attrPageMap.Value == "map")
                        {
                            if (listOfSpineRootNodes != null)
                            {
                                foreach (XmlNode spineNodeRoot in listOfSpineRootNodes)
                                {
                                    XmlNodeList listOfSpineItemNodes = spineNodeRoot.ChildNodes;
                                    if (listOfSpineItemNodes != null)
                                    {
                                        foreach (XmlNode spineItemNode in listOfSpineItemNodes)
                                        {
                                            if (spineItemNode.NodeType == XmlNodeType.Element
                                                && spineItemNode.Name == "itemref")
                                            {
                                                XmlAttributeCollection spineItemAttributes = spineItemNode.Attributes;

                                                if (spineItemAttributes != null)
                                                {
                                                    XmlNode attrIdRef = spineItemAttributes.GetNamedItem("idref");
                                                    if (attrIdRef != null)
                                                    {
                                                        spineListOfHtmlFiles.Add(attrIdRef.Value);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            XmlNodeList listOfManifestRootNodes = opfXmlDoc.GetElementsByTagName("manifest");
            if (listOfManifestRootNodes != null)
            {
                foreach (XmlNode manifNodeRoot in listOfManifestRootNodes)
                {
                    XmlNodeList listOfManifestItemNodes = manifNodeRoot.ChildNodes;
                    if (listOfManifestItemNodes != null)
                    {
                        foreach (XmlNode manifItemNode in listOfManifestItemNodes)
                        {
                            if (manifItemNode.NodeType == XmlNodeType.Element
                                && manifItemNode.Name == "item")
                            {
                                XmlAttributeCollection manifItemAttributes = manifItemNode.Attributes;

                                if (manifItemAttributes != null)
                                {
                                    XmlNode attrHref = manifItemAttributes.GetNamedItem("href");
                                    XmlNode attrMediaType = manifItemAttributes.GetNamedItem("media-type");
                                    if (attrHref != null && attrMediaType != null)
                                    {
                                        if (attrMediaType.Value == "application/xhtml+xml")
                                        {
                                            XmlNode attrID = manifItemAttributes.GetNamedItem("id");
                                            if (attrID != null)
                                            {
                                                int i = spineListOfHtmlFiles.IndexOf(attrID.Value);
                                                if (i >= 0)
                                                {
                                                    spineListOfHtmlFiles[i] = attrHref.Value;
                                                }
                                            }
                                        }
                                        else if (attrMediaType.Value == "application/x-dtbncx+xml" || attrMediaType.Value == "text/xml")
                                        {
                                            ncxPath = attrHref.Value;
                                        }
                                        else if (attrMediaType.Value == "application/x-dtbook+xml" || attrMediaType.Value == "application/oebps-page-map+xml")
                                        {
                                            DtBookPath = attrHref.Value;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }//parseEpubOpfManifestAndSpine

        private void parseEpubNcx(string fullNcxPath)
        {
            presentation = m_Project.GetPresentation(0);
            ncxXmlDoc = readXmlDocument(fullNcxPath);

            XmlNodeList listOfHeadRootNodes = ncxXmlDoc.GetElementsByTagName("head");
            if (listOfHeadRootNodes != null)
            {
                foreach (XmlNode headNodeRoot in listOfHeadRootNodes)
                {
                    XmlNodeList listOfMetaNodes = headNodeRoot.ChildNodes;
                    if (listOfMetaNodes != null)
                    {
                        foreach (XmlNode metaNode in listOfMetaNodes)
                        {
                            if (metaNode.NodeType == XmlNodeType.Element
                                && metaNode.Name == "meta")
                            {
                                XmlAttributeCollection attributeCol = metaNode.Attributes;

                                if (attributeCol != null)
                                {
                                    XmlNode attrName = attributeCol.GetNamedItem("name");
                                    XmlNode attrContent = attributeCol.GetNamedItem("content");
                                    if (attrName != null && attrContent != null && !String.IsNullOrEmpty(attrName.Value)
                                        && !String.IsNullOrEmpty(attrContent.Value))
                                    {
                                        Metadata md = presentation.MetadataFactory.CreateMetadata();
                                        md.Name = attrName.Value;
                                        md.Content = attrContent.Value;
                                        presentation.AddMetadata(md);
                                    }
                                }
                            }
                        }
                    }
                }
            }            
        }//parseEpubNcx      
        private void parseHtml(string dirPath, List<string> htmlFilesList)
        {
            presentation = m_Project.ListOfPresentations[0];
            
            if (presentation.RootNode == null)
                presentation.RootNode = presentation.TreeNodeFactory.Create();
            
            for (int i = 0; i < htmlFilesList.Count; i++)
            {
                urakawa.core.TreeNode htmlRoot = presentation.TreeNodeFactory.Create();
                presentation.RootNode.AppendChild(htmlRoot);
                XmlDocument htmlDocument = null;
               
                try
                {
                    htmlDocument = readXmlDocument(Path.Combine(dirPath, htmlFilesList[i]));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(" HTML File name : " + htmlFilesList[i] + "\n" + ex.ToString());
                }
                if (htmlDocument != null)
                {
                    XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                    htmlRoot.AddProperty(xmlProp);
                    xmlProp.LocalName = "title";

                    string text = htmlDocument.GetElementsByTagName("title")[0].InnerText;
                    TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                    textMedia.Text = text;
                    ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                    cProp.SetMedia(m_textChannel, textMedia);
                    htmlRoot.AddProperty(cProp);

                    parseDTBookXmlDocAndPopulateDataModel(htmlDocument.GetElementsByTagName("body")[0], htmlRoot);
                }
            }            
          }//pasehtml        
    }//Class
}//Namespace