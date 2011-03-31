using System.Collections.Generic;
using System.Xml;
using System.IO;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    partial class Daisy3_Export
    {
        List<string> m_DtbAllowedInXMetadata = new List<string>();
        List<string> m_AllowedInDcMetadata = new List<string>();

        private void CreateOpfDocument()
        {
            //m_ProgressPercentage = 90;
            //reportProgress(m_ProgressPercentage, UrakawaSDK_daisy_Lang.AllFilesCreated);
            if (RequestCancellation) return;
            XmlDocument opfDocument = CreateStub_OpfDocument();

            XmlNode manifestNode = XmlDocumentHelper.GetFirstChildElementWithName(opfDocument, true, "manifest", null); //opfDocument.GetElementsByTagName("manifest")[0];
            const string mediaType_Smil = "application/smil";
            const string mediaType_Wav = "audio/x-wav";
            const string mediaType_Mpg = "audio/mpeg";
            const string mediaType_Opf = "text/xml";
            const string mediaType_Ncx = "application/x-dtbncx+xml";
            const string mediaType_Dtbook = "application/x-dtbook+xml";
            const string mediaType_Image_Jpg = "image/jpeg";
            const string mediaType_Image_Png = "image/png";
            const string mediaType_Image_Svg = "image/svg+xml";
            const string mediaType_Resource = "application/x-dtbresource+xml";
            const string mediaType_Css = "text/css";
            const string mediaType_Transform = "text/xsl";
            const string mediaType_DTD = "application/xml-dtd";

            m_AllowedInDcMetadata.Add("dc:title");
            m_AllowedInDcMetadata.Add("dc:creator");
            m_AllowedInDcMetadata.Add("dc:subject");
            m_AllowedInDcMetadata.Add("dc:description");
            m_AllowedInDcMetadata.Add("dc:publisher");
            m_AllowedInDcMetadata.Add("dc:contributor");
            m_AllowedInDcMetadata.Add("dc:date");
            m_AllowedInDcMetadata.Add("dc:type");
            m_AllowedInDcMetadata.Add("dc:format");
            m_AllowedInDcMetadata.Add("dc:identifier");
            m_AllowedInDcMetadata.Add("dc:source");
            m_AllowedInDcMetadata.Add("dc:language");
            m_AllowedInDcMetadata.Add("dc:relation");
            m_AllowedInDcMetadata.Add("dc:coverage");
            m_AllowedInDcMetadata.Add("dc:rights");

            m_DtbAllowedInXMetadata.Add("dtb:sourcedate");
            m_DtbAllowedInXMetadata.Add("dtb:sourceedition");
            m_DtbAllowedInXMetadata.Add("dtb:sourcepublisher");
            m_DtbAllowedInXMetadata.Add("dtb:sourcerights");
            m_DtbAllowedInXMetadata.Add("dtb:sourcetitle");
            m_DtbAllowedInXMetadata.Add("dtb:multimediatype");
            m_DtbAllowedInXMetadata.Add("dtb:multimediacontent");
            m_DtbAllowedInXMetadata.Add("dtb:narrator");
            m_DtbAllowedInXMetadata.Add("dtb:producer");
            m_DtbAllowedInXMetadata.Add("dtb:produceddate");
            m_DtbAllowedInXMetadata.Add("dtb:revision");
            m_DtbAllowedInXMetadata.Add("dtb:revisiondate");
            m_DtbAllowedInXMetadata.Add("dtb:revisiondescription");
            m_DtbAllowedInXMetadata.Add("dtb:totaltime");
            m_DtbAllowedInXMetadata.Add("dtb:audioformat");

            // add all files to manifest
            AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Ncx, "ncx", mediaType_Ncx);
            if (m_Filename_Content != null )  AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Content, GetNextID(ID_OpfPrefix), mediaType_Dtbook);
            AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Opf, GetNextID(ID_OpfPrefix), mediaType_Opf);

            // add external files to manifest
            foreach (string externalFileName in m_FilesList_ExternalFiles)
            {
                string strID = GetNextID(ID_OpfPrefix);
                switch (Path.GetExtension(externalFileName).ToLower())
                {
                    case ".css":
                        AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, mediaType_Css);
                        break;

                    case ".xslt":
                        AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, mediaType_Transform);
                        break;

                    case ".dtd":
                        AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, mediaType_DTD);
                        break;
                    default:
                        break;
                }


            }

            if (RequestCancellation) return;
            // add smil files to manifest
            List<string> smilIDListInPlayOrder = new List<string>();

            foreach (string smilFileName in m_FilesList_Smil)
            {
                string strID = GetNextID(ID_OpfPrefix);
                AddFilenameToManifest(opfDocument, manifestNode, smilFileName, strID, mediaType_Smil);
                smilIDListInPlayOrder.Add(strID);
            }

            if (RequestCancellation) return;

            foreach (string audioFileName in m_FilesList_Audio)
            {
                string strID = GetNextID(ID_OpfPrefix);

                if (string.Compare(Path.GetExtension(audioFileName), ".wav", true) == 0)
                {
                    AddFilenameToManifest(opfDocument, manifestNode, audioFileName, strID, mediaType_Wav);
                }
                else
                {
                    AddFilenameToManifest(opfDocument, manifestNode, audioFileName, strID, mediaType_Mpg);
                }
            }

            if (RequestCancellation) return;

            foreach (string imageFileName in m_FilesList_Image)
            {
                string strID = GetNextID(ID_OpfPrefix);

                if (string.Compare(Path.GetExtension(imageFileName), ".png", true) == 0)
                {
                    AddFilenameToManifest(opfDocument, manifestNode, imageFileName, strID, mediaType_Image_Png);
                }
                else
                {
                    AddFilenameToManifest(opfDocument, manifestNode, imageFileName, strID, mediaType_Image_Jpg);
                }
            }

            // copy resource files and place entry in manifest
            string sourceDirectoryPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string ResourceRes_Filename = "tpbnarrator.res";
            string resourceAudio_Filename = "tpbnarrator_res.mp3";

            string ResourceRes_Filename_fullPath = Path.Combine(sourceDirectoryPath, ResourceRes_Filename);
            string resourceAudio_Filename_fullPath = Path.Combine(sourceDirectoryPath, resourceAudio_Filename);
            if (File.Exists(ResourceRes_Filename_fullPath) && File.Exists(resourceAudio_Filename_fullPath))
            {
                if (RequestCancellation) return;
                File.Copy(ResourceRes_Filename_fullPath, Path.Combine(m_OutputDirectory, ResourceRes_Filename), true);
                File.Copy(resourceAudio_Filename_fullPath, Path.Combine(m_OutputDirectory, resourceAudio_Filename), true);

                // add entry to manifest
                AddFilenameToManifest(opfDocument, manifestNode, ResourceRes_Filename, "resource", mediaType_Resource);
                AddFilenameToManifest(opfDocument, manifestNode, resourceAudio_Filename, GetNextID(ID_OpfPrefix), mediaType_Mpg);
            }

            // create spine
            XmlNode spineNode = XmlDocumentHelper.GetFirstChildElementWithName(opfDocument, true, "spine", null); //opfDocument.GetElementsByTagName("spine")[0];

            foreach (string strSmilID in smilIDListInPlayOrder)
            {
                XmlNode itemRefNode = opfDocument.CreateElement(null, "itemref", spineNode.NamespaceURI);
                spineNode.AppendChild(itemRefNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, itemRefNode, "idref", strSmilID);

            }

            if (RequestCancellation) return;
            AddMetadata_Opf(opfDocument);

            if (RequestCancellation)
            {
                opfDocument = null;
                return;
            }
            SaveXukAction.WriteXmlDocument(opfDocument, Path.Combine(m_OutputDirectory, m_Filename_Opf));
        }

        private XmlNode AddFilenameToManifest(XmlDocument opfDocument, XmlNode manifestNode, string filename, string strID, string mediaType)
        {
            XmlNode itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
            manifestNode.AppendChild(itemNode);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, itemNode, "href", filename);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, itemNode, "id", strID);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, itemNode, "media-type", mediaType);

            return itemNode;
        }

        private void AddMetadata_Generator(XmlDocument doc, XmlNode parentNode)
        {
            AddMetadataAsAttributes(doc, parentNode, "dtb:generator", "Tobi and the Urakawa SDK: the open-source DAISY multimedia authoring toolkit");
            //m_ProgressPercentage = 100;
            //reportProgress(m_ProgressPercentage, UrakawaSDK_daisy_Lang.AllFilesCreated);                                       
        }

        private void AddMetadata_Opf(XmlDocument opfDocument)
        {
            XmlNode dc_metadataNode = XmlDocumentHelper.GetFirstChildElementWithName(opfDocument, true, "dc-metadata", null); //opfDocument.GetElementsByTagName("dc-metadata")[0];
            XmlNode x_metadataNode = XmlDocumentHelper.GetFirstChildElementWithName(opfDocument, true, "x-metadata", null); //opfDocument.GetElementsByTagName("x-metadata")[0];

            Metadata mdId = AddMetadata_DtbUid(true, opfDocument, dc_metadataNode);

            //AddMetadata_Generator(opfDocument, x_metadataNode);

            AddMetadataAsAttributes(opfDocument, x_metadataNode, "dtb:totalTime", FormatTimeString(m_TotalTime));

            if (true || m_Presentation.GetMetadata("dtb:multimediaType").Count == 0)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, "dtb:multimediaType",m_Filename_Content != null? "audioFullText" : "audioNCX");
            }

            if (true || m_Presentation.GetMetadata("dtb:multimediaContent").Count == 0)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, "dtb:multimediaContent",m_Filename_Content != null? "audio,text" : "audio");
            }

            AddMetadataAsInnerText(opfDocument, dc_metadataNode, "dc:format", "ANSI/NISO Z39.86-2005");


            foreach (Metadata m in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                string lowerName = m.NameContentAttribute.Name.ToLower();
                if (mdId == m
                    || lowerName == "dtb:totaltime"
                    || lowerName == "dc:format"
                    || lowerName == "dtb:multimediatype"
                    || lowerName == "dtb:multimediacontent") 
                    continue;

                XmlNode metadataNodeCreated = null;
                //if (m.NameContentAttribute.Name.StartsWith("dc:"))
                if (m_AllowedInDcMetadata.Contains(lowerName))
                {
                    metadataNodeCreated = AddMetadataAsInnerText(opfDocument, dc_metadataNode, m.NameContentAttribute.Name, m.NameContentAttribute.Value);
                    // add other metadata attributes if any
                    foreach (MetadataAttribute ma in m.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id") continue;
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metadataNodeCreated, ma.Name, ma.Value);
                    }
                }
                //else
                //items in x-metadata may start with dtb: ONLY if they are in the list of allowed dtb:* items
                //OR, items in x-metadata may be anything else (non-dtb:*).
                else if (
                    (lowerName.StartsWith("dtb:") && m_DtbAllowedInXMetadata.Contains(lowerName))
                    || !lowerName.StartsWith("dtb:")
                    )
                {
                    metadataNodeCreated = AddMetadataAsAttributes(opfDocument, x_metadataNode, m.NameContentAttribute.Name, m.NameContentAttribute.Value);
                    // add other metadata attributes if any
                    foreach (MetadataAttribute ma in m.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id") continue;
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metadataNodeCreated, ma.Name, ma.Value);
                    }
                }
                
            } // end of metadata for each loop


            //XmlNodeList totalTimeNodesList = opfDocument.GetElementsByTagName("dtb:totaltime");
            //if (totalTimeNodesList == null || (totalTimeNodesList != null && totalTimeNodesList.Count == 0))
            //{
            //    AddMetadataAsAttributes(opfDocument, x_metadataNode, "dtb:totalTime", m_TotalTime.ToString());
            //}

            // add uid to dc:identifier
            //XmlNodeList identifierList = opfDocument.GetElementsByTagName("dc:Identifier");
            //XmlNode identifierNode = null;
            //bool isUidReAssigned = false;
            //foreach (XmlNode identifierMetaNode in getChildrenElementsWithName(opfDocument, true, "dc:Identifier", null, false))
            //{
            //    if (identifierNode == null) identifierNode = identifierMetaNode;

            //    if (identifierMetaNode.Attributes.GetNamedItem("uid") != null)
            //    {
            //        identifierMetaNode.Attributes.GetNamedItem("uid").Value = "uid";
            //        isUidReAssigned = true;
            //    }
            //}
            //if (!isUidReAssigned)
            //{
            //    CommonFunctions.CreateAppendXmlAttribute(opfDocument, identifierNode, "id", "uid");
            //}
        }

        private XmlNode AddMetadataAsInnerText(XmlDocument doc, XmlNode metadataParentNode, string name, string content)
        {
            XmlNode node = null;

            if (name.Contains(":"))
            {
                // split the metadata name and make first alphabet upper, required for daisy 3.0
                string splittedName = name.Split(':')[1];
                splittedName = splittedName.Substring(0, 1).ToUpper() + splittedName.Remove(0, 1);

                node = doc.CreateElement(name.Split(':')[0], splittedName, metadataParentNode.Attributes.GetNamedItem("xmlns:dc").Value);
            }
            else
            {
                node = doc.CreateElement(null, name, metadataParentNode.NamespaceURI);
            }
            metadataParentNode.AppendChild(node);
            node.AppendChild(
                doc.CreateTextNode(content));

            return node;
        }

        private XmlDocument CreateStub_OpfDocument()
        {
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;

            document.CreateXmlDeclaration("1.0", "utf-8", null);
            document.AppendChild(document.CreateDocumentType("package",
                "+//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN",
                "http://openebook.org/dtds/oeb-1.2/oebpkg12.dtd",
                null));

            XmlNode rootNode = document.CreateElement(null,
                "package",
                "http://openebook.org/namespaces/oeb-package/1.0/");

            document.AppendChild(rootNode);


            XmlDocumentHelper.CreateAppendXmlAttribute(document, rootNode, "unique-identifier", "uid");

            XmlNode metadataNode = document.CreateElement(null, "metadata", rootNode.NamespaceURI);
            rootNode.AppendChild(metadataNode);

            XmlNode dcMetadataNode = document.CreateElement(null, "dc-metadata", rootNode.NamespaceURI);
            metadataNode.AppendChild(dcMetadataNode);
            XmlDocumentHelper.CreateAppendXmlAttribute(document, dcMetadataNode, "xmlns:dc", "http://purl.org/dc/elements/1.1/");
            XmlDocumentHelper.CreateAppendXmlAttribute(document, dcMetadataNode, "xmlns:oebpackage", "http://openebook.org/namespaces/oeb-package/1.0/");

            XmlNode xMetadataNode = document.CreateElement(null, "x-metadata", rootNode.NamespaceURI);
            metadataNode.AppendChild(xMetadataNode);

            XmlNode manifestNode = document.CreateElement(null, "manifest", rootNode.NamespaceURI);
            rootNode.AppendChild(manifestNode);

            XmlNode spineNode = document.CreateElement(null, "spine", rootNode.NamespaceURI);
            rootNode.AppendChild(spineNode);


            return document;
        }

        private void CreateExternalFiles()
        {
            foreach (ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_ListAsReadOnly)
            {
                reportProgress(-1, UrakawaSDK_daisy_Lang.CreatingExternalFiles);
                if (efd.IsPreservedForOutputFile && !m_FilesList_ExternalFiles.Contains(efd.OriginalRelativePath))
                {
                    string filePath = Path.Combine(m_OutputDirectory, efd.OriginalRelativePath);
                    efd.DataProvider.ExportDataStreamToFile(filePath, true);
                    m_FilesList_ExternalFiles.Add(efd.OriginalRelativePath);

                }
            }
        }
    }
}
