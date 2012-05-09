using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using urakawa.data;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.metadata.daisy;
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

            XmlNode manifestNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "manifest", null); //opfDocument.GetElementsByTagName("manifest")[0];

            const string mediaType_Smil = "application/smil";
            const string mediaType_Ncx = "application/x-dtbncx+xml";
            const string mediaType_Dtbook = "application/x-dtbook+xml";
            const string mediaType_Resource = "application/x-dtbresource+xml";


            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Title);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Creator);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Subject);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Description);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Identifier);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Publisher);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Contributor);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Date);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Type);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Format);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.D_Source);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Language);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Relation);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Coverage);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Rights);

            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_DATE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_EDITION);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_PUBLISHER);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_RIGHTS);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_TITLE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_NARRATOR);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_PRODUCER);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_PRODUCED_DATE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_REVISION);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_REVISION_DATE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_REVISION_DESCRIPTION);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_TOTAL_TIME);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_AUDIO_FORMAT);

            // add all files to manifest
            AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Ncx, "ncx", mediaType_Ncx);
            if (m_Filename_Content != null) AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Content, GetNextID(ID_OpfPrefix), mediaType_Dtbook);
            AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Opf, GetNextID(ID_OpfPrefix), DataProviderFactory.XML_TEXT_MIME_TYPE);

            // add external files to manifest
            foreach (string externalFileName in m_FilesList_ExternalFiles)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(externalFileName);
                if (String.Equals(ext, ".css", StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.STYLE_CSS_MIME_TYPE);
                }
                else if (String.Equals(ext, DataProviderFactory.STYLE_XSLT_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    || String.Equals(ext, DataProviderFactory.STYLE_XSL_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.STYLE_XSLT_MIME_TYPE);
                }
                else if (String.Equals(ext, ".dtd", StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.DTD_MIME_TYPE);
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
                string ext = Path.GetExtension(audioFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, audioFileName, strID, mime);
            }

            if (RequestCancellation) return;

            foreach (string imageFileName in m_FilesList_Image)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(imageFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, imageFileName, strID, mime);
            }
            foreach (string videoFileName in m_FilesList_Video)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(videoFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, videoFileName, strID, mime);
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
                AddFilenameToManifest(opfDocument, manifestNode, resourceAudio_Filename, GetNextID(ID_OpfPrefix), DataProviderFactory.AUDIO_MP3_MIME_TYPE);
            }

            // create spine
            XmlNode spineNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "spine", null); //opfDocument.GetElementsByTagName("spine")[0];

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
            XmlReaderWriterHelper.WriteXmlDocument(opfDocument, Path.Combine(m_OutputDirectory, m_Filename_Opf));
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

        private readonly string m_GeneratorName_SDK = "the Urakawa SDK: the open-source DAISY multimedia authoring toolkit";
        private string m_GeneratorName;
        public string GeneratorName
        {
            get { return string.IsNullOrEmpty(m_GeneratorName) ? m_GeneratorName = "Tobi and " + m_GeneratorName_SDK : m_GeneratorName; }
            set { m_GeneratorName = value + " and " + m_GeneratorName_SDK; }
        }

        private void AddMetadata_Generator(XmlDocument doc, XmlNode parentNode)
        {
            AddMetadataAsAttributes(doc, parentNode, SupportedMetadata_Z39862005.DTB_GENERATOR, GeneratorName);
            //m_ProgressPercentage = 100;
            //reportProgress(m_ProgressPercentage, UrakawaSDK_daisy_Lang.AllFilesCreated);                                       
        }

        private void AddMetadata_Opf(XmlDocument opfDocument)
        {
            XmlNode dc_metadataNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "dc-metadata", null); //opfDocument.GetElementsByTagName("dc-metadata")[0];
            XmlNode x_metadataNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "x-metadata", null); //opfDocument.GetElementsByTagName("x-metadata")[0];

            Metadata mdId = AddMetadata_DtbUid(true, opfDocument, dc_metadataNode);

            //AddMetadata_Generator(opfDocument, x_metadataNode);

            AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_TOTAL_TIME, FormatTimeString(m_TotalTime));

            if (true || m_Presentation.GetMetadata(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE).Count == 0)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE, m_Filename_Content != null ? "audioFullText" : "audioNCX");
            }

            if (true || m_Presentation.GetMetadata(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT).Count == 0)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT, m_Filename_Content != null ? "audio,text" : "audio");
            }

            AddMetadataAsInnerText(opfDocument, dc_metadataNode, SupportedMetadata_Z39862005.DC_Format.ToLower(), "ANSI/NISO Z39.86-2005");


            foreach (Metadata m in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                //string lowerName = m.NameContentAttribute.Name.ToLower();
                if (mdId == m
                    || string.Equals(m.NameContentAttribute.Name, SupportedMetadata_Z39862005.DTB_TOTAL_TIME, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(m.NameContentAttribute.Name, SupportedMetadata_Z39862005.DC_Format, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(m.NameContentAttribute.Name, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(m.NameContentAttribute.Name, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT, StringComparison.OrdinalIgnoreCase)
                    )
                    continue;

                XmlNode metadataNodeCreated = null;
                //if (m.NameContentAttribute.Name.StartsWith(SupportedMetadata_Z39862005.DC + ":"))

                bool contains = false;
                foreach (string str in m_AllowedInDcMetadata)
                {
                    if (str.Equals(m.NameContentAttribute.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        contains = true;
                        break;
                    }
                }

                bool containsDtb = false;
                foreach (string str in m_DtbAllowedInXMetadata)
                {
                    if (str.Equals(m.NameContentAttribute.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        containsDtb = true;
                        break;
                    }
                }

                if (contains)
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
                    (
                    containsDtb
                    && m.NameContentAttribute.Name.StartsWith(SupportedMetadata_Z39862005.NS_PREFIX_DTB + ":", StringComparison.OrdinalIgnoreCase)
                    )
                    || !m.NameContentAttribute.Name.StartsWith(SupportedMetadata_Z39862005.NS_PREFIX_DTB + ":", StringComparison.OrdinalIgnoreCase)
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
            //string name = name_.ToLower();
            XmlNode node = null;

            string prefix;
            string localName;
            urakawa.property.xml.XmlProperty.SplitLocalName(name, out prefix, out localName);

            if (!string.IsNullOrEmpty(prefix))
            {
                // split the metadata name and make first alphabet upper, required for daisy 3.0

                localName = localName.Substring(0, 1).ToUpper() + localName.Remove(0, 1);

                string nsUri = null;
                if (metadataParentNode != null && metadataParentNode.Attributes != null)
                {
                    XmlNode attr = metadataParentNode.Attributes.GetNamedItem(XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":dc");
                    if (attr != null)
                    {
                        nsUri = attr.Value;
                    }
                }
                if (nsUri == null)
                {
                    nsUri = DiagramContentModelHelper.NS_URL_DC;
                }

                node = doc.CreateElement(prefix, localName, nsUri);
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
            XmlDocumentHelper.CreateAppendXmlAttribute(document, dcMetadataNode, XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":" + DiagramContentModelHelper.NS_PREFIX_DC, DiagramContentModelHelper.NS_URL_DC);
            XmlDocumentHelper.CreateAppendXmlAttribute(document, dcMetadataNode, XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":oebpackage", "http://openebook.org/namespaces/oeb-package/1.0/");

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
            foreach (ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                reportProgress(-1, UrakawaSDK_daisy_Lang.CreatingExternalFiles);

                string filename = efd.OriginalRelativePath;
                if (filename.StartsWith(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA))
                {
                    filename = filename.Substring(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA.Length);
                }

                if (efd.IsPreservedForOutputFile
                    && !m_FilesList_ExternalFiles.Contains(filename))
                {
                    string filePath = Path.Combine(m_OutputDirectory, filename);
                    efd.DataProvider.ExportDataStreamToFile(filePath, true);
                    m_FilesList_ExternalFiles.Add(filename);
                }
            }
        }
    }
}
