using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;
using AudioLib;
using urakawa.ExternalFiles;
using urakawa.data;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    partial class Daisy3_Export
    {
        protected List<string> m_DtbAllowedInXMetadata = new List<string>();
        protected List<string> m_AllowedInDcMetadata = new List<string>();

        protected virtual void CreateOpfDocument()
        {
            //m_ProgressPercentage = 90;
            //reportProgress(m_ProgressPercentage, UrakawaSDK_daisy_Lang.AllFilesCreated);
            if (RequestCancellation) return;
            XmlDocument opfDocument = CreateStub_OpfDocument();

            XmlNode manifestNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "manifest", null); //opfDocument.GetElementsByTagName("manifest")[0];


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

            AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Ncx, "ncx", DataProviderFactory.NCX_MIME_TYPE);

            if (m_Filename_Content != null)
            {
                AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Content, GetNextID(ID_OpfPrefix), DataProviderFactory.DTBOOK_MIME_TYPE);
            }

            AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Opf, GetNextID(ID_OpfPrefix), DataProviderFactory.XML_MIME_TYPE);

            foreach (string externalFileName in m_FilesList_ExternalFiles)
            {
                // ALREADY escaped!
                //externalFileName = FileDataProvider.EliminateForbiddenFileNameCharacters(externalFileName);

                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(externalFileName);
                if (DataProviderFactory.CSS_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.CSS_MIME_TYPE);
                }
                else if (DataProviderFactory.XSLT_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                    || DataProviderFactory.XSL_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.XSLT_MIME_TYPE_);
                }
                else if (DataProviderFactory.DTD_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.DTD_MIME_TYPE);
                }
            }

            if (RequestCancellation) return;

            List<string> smilIDListInPlayOrder = new List<string>();

            foreach (string smilFileName in m_FilesList_Smil)
            {
                string strID = GetNextID(ID_OpfPrefix);

                AddFilenameToManifest(opfDocument, manifestNode, smilFileName, strID, DataProviderFactory.SMIL_MIME_TYPE_);
                smilIDListInPlayOrder.Add(strID);
            }

            if (RequestCancellation) return;

            foreach (string audioFileName in m_FilesList_SmilAudio)
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


#if SUPPORT_AUDIO_VIDEO
            if (RequestCancellation) return;

            foreach (string videoFileName in m_FilesList_Video)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(videoFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, videoFileName, strID, mime);
            }

            foreach (string audioFileName in m_FilesList_Audio)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(audioFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, audioFileName, strID, mime);
            }
#endif

            if (RequestCancellation) return;

            bool textOnly = m_TotalTime == null || m_TotalTime.AsLocalUnits == 0;
            if (true || !textOnly)
            {
                // copy resource files and place entry in manifest
                string sourceDirectoryPath = System.AppDomain.CurrentDomain.BaseDirectory;
                string ResourceRes_Filename = "tpbnarrator.res";
                string resourceAudio_Filename = "tpbnarrator_res.mp3";

                string ResourceRes_Filename_fullPath = Path.Combine(sourceDirectoryPath, ResourceRes_Filename);
                string resourceAudio_Filename_fullPath = Path.Combine(sourceDirectoryPath, resourceAudio_Filename);
                if (File.Exists(ResourceRes_Filename_fullPath) && File.Exists(resourceAudio_Filename_fullPath))
                {
                    if (RequestCancellation) return;

                    string destRes = Path.Combine(m_OutputDirectory, ResourceRes_Filename);

                    if (!textOnly)
                    {
                        File.Copy(ResourceRes_Filename_fullPath, destRes, true);
                        try
                        {
                            File.SetAttributes(destRes, FileAttributes.Normal);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        string resXml = File.ReadAllText(ResourceRes_Filename_fullPath);

                        int i = -1;
                        int j = 0;
                        while ((i = resXml.IndexOf("<audio", j)) >= 0)
                        {
                            j = resXml.IndexOf("/>", i);
                            if (j > i)
                            {
                                int len = j - i + 2;
                                resXml = resXml.Remove(i, len);

                                string fill = "";
                                for (int k = 1; k <= len; k++)
                                {
                                    fill += ' '; //k.ToString();
                                }

                                resXml = resXml.Insert(i, fill);
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                                break;
                            }
                        }

                        StreamWriter streamWriter = File.CreateText(destRes);
                        try
                        {
                            streamWriter.Write(resXml);
                        }
                        finally
                        {
                            streamWriter.Close();
                        }
                    }

                    AddFilenameToManifest(opfDocument, manifestNode, ResourceRes_Filename, "resource", DataProviderFactory.DTB_RES_MIME_TYPE);

                    if (!textOnly)
                    {
                        string destFile = Path.Combine(m_OutputDirectory, resourceAudio_Filename);
                        File.Copy(resourceAudio_Filename_fullPath, destFile, true);
                        try
                        {
                            File.SetAttributes(destFile, FileAttributes.Normal);
                        }
                        catch
                        {
                        }

                        AddFilenameToManifest(opfDocument, manifestNode, resourceAudio_Filename, GetNextID(ID_OpfPrefix), DataProviderFactory.AUDIO_MP3_MIME_TYPE);
                    }
                }
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

            XmlReaderWriterHelper.WriteXmlDocument(opfDocument, OpfFilePath, null);
        }

        public string OpfFilePath
        {
            get { return Path.Combine(m_OutputDirectory, m_Filename_Opf); }
        }

        protected XmlNode AddFilenameToManifest(XmlDocument opfDocument, XmlNode manifestNode, string filename, string strID, string mediaType)
        {
            XmlNode itemNode = opfDocument.CreateElement(null, "item", manifestNode.NamespaceURI);
            manifestNode.AppendChild(itemNode);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, itemNode, "href", FileDataProvider.UriEncode(filename));
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

        protected virtual void AddMetadata_Opf(XmlDocument opfDocument)
        {
            XmlNode dc_metadataNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "dc-metadata", null); //opfDocument.GetElementsByTagName("dc-metadata")[0];
            XmlNode x_metadataNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "x-metadata", null); //opfDocument.GetElementsByTagName("x-metadata")[0];

            Metadata mdId = AddMetadata_DtbUid(true, opfDocument, dc_metadataNode);

            //AddMetadata_Generator(opfDocument, x_metadataNode);

            bool textOnly = m_TotalTime == null || m_TotalTime.AsLocalUnits == 0;

            if (true || !textOnly)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_TOTAL_TIME, FormatTimeString(m_TotalTime));
            }

            //type 1 "audioOnly"
            //type 2 "audioNCX"
            //type 3 "audioPartText"
            //type 4 "audioFullText"
            //type 5 "textPartAudio"
            //type 6 "textNCX"
            //http://www.daisy.org/z3986/specifications/daisy_202.html#dtbclass
            if (true || m_Presentation.GetMetadata(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE).Count == 0)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE, m_Filename_Content != null ?
                    (textOnly ? "textNCX" : "audioFullText") //"textPartAudio"
                    : "audioNCX");
            }

            //audio,text,image ???
            if (true || m_Presentation.GetMetadata(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT).Count == 0)
            {
                AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT, m_Filename_Content != null ?
                    (textOnly ? "text" : "audio,text") //"audio,text"
                    : "audio");
            }

            AddMetadataAsInnerText(opfDocument, dc_metadataNode, SupportedMetadata_Z39862005.DC_Format.ToLower(), "ANSI/NISO Z39.86-2005");


            bool hasMathML_z39_86_extension_version = false;
            bool hasMathML_DTBook_XSLTFallback = false;

            foreach (Metadata m in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                string name = m.NameContentAttribute.Name;

                if (name == SupportedMetadata_Z39862005._z39_86_extension_version)
                {
                    hasMathML_z39_86_extension_version = true;
                }
                else if (name == SupportedMetadata_Z39862005.MATHML_XSLT_METADATA)
                {
                    hasMathML_DTBook_XSLTFallback = true;
                }

                //string lowerName = m.NameContentAttribute.Name.ToLower();
                if (mdId == m
                    || SupportedMetadata_Z39862005.DTB_TOTAL_TIME.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || SupportedMetadata_Z39862005.DC_Format.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT.Equals(name, StringComparison.OrdinalIgnoreCase)
                    )
                    continue;

                XmlNode metadataNodeCreated = null;
                //if (m.NameContentAttribute.Name.StartsWith(SupportedMetadata_Z39862005.DC + ":"))

                bool contains = false;
                foreach (string str in m_AllowedInDcMetadata)
                {
                    if (str.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        contains = true;
                        break;
                    }
                }

                bool containsDtb = false;
                foreach (string str in m_DtbAllowedInXMetadata)
                {
                    if (str.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        containsDtb = true;
                        break;
                    }
                }

                if (contains)
                {
                    metadataNodeCreated = AddMetadataAsInnerText(opfDocument, dc_metadataNode, name, m.NameContentAttribute.Value);
                    // add other metadata attributes if any
                    foreach (MetadataAttribute ma in m.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id" || ma.Name == Metadata.PrimaryIdentifierMark)
                        {
                            continue;
                        }
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
                    metadataNodeCreated = AddMetadataAsAttributes(opfDocument, x_metadataNode, name, m.NameContentAttribute.Value);

                    // add other metadata attributes if any
                    foreach (MetadataAttribute ma in m.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id" || ma.Name == Metadata.PrimaryIdentifierMark)
                        {
                            continue;
                        }
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metadataNodeCreated, ma.Name, ma.Value);
                    }
                }

            } // end of metadata for each loop

            string mathML_XSLT = null;

            foreach (ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (efd is XSLTExternalFileData)
                {
                    string filename = efd.OriginalRelativePath;
                    if (filename.StartsWith(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA))
                    {
                        filename = filename.Substring(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA.Length);

                        mathML_XSLT = filename;
                        break;
                    }
                }
            }

            string mathPrefix = m_Presentation.RootNode.GetXmlNamespacePrefix(DiagramContentModelHelper.NS_URL_MATHML);

            if (!string.IsNullOrEmpty(mathML_XSLT))
            {
                DebugFix.Assert(hasMathML_z39_86_extension_version);
                DebugFix.Assert(hasMathML_DTBook_XSLTFallback);
            }

            if (!string.IsNullOrEmpty(mathPrefix) && !hasMathML_z39_86_extension_version)
            {
                XmlNode metaNode = opfDocument.CreateElement(null, "meta", x_metadataNode.NamespaceURI);
                x_metadataNode.AppendChild(metaNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "name", SupportedMetadata_Z39862005._z39_86_extension_version);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "content", "1.0");
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "scheme", DiagramContentModelHelper.NS_URL_MATHML);
            }

            if (!string.IsNullOrEmpty(mathPrefix) && !hasMathML_DTBook_XSLTFallback)
            {
                XmlNode metaNode = opfDocument.CreateElement(null, "meta", x_metadataNode.NamespaceURI);
                x_metadataNode.AppendChild(metaNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "name", SupportedMetadata_Z39862005.MATHML_XSLT_METADATA);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "content", string.IsNullOrEmpty(mathML_XSLT) ? SupportedMetadata_Z39862005._builtInMathMLXSLT : mathML_XSLT);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "scheme", DiagramContentModelHelper.NS_URL_MATHML);
            }

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

        protected XmlNode AddMetadataAsInnerText(XmlDocument doc, XmlNode metadataParentNode, string name, string content)
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

        protected virtual XmlDocument CreateStub_OpfDocument()
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

        protected virtual void CreateExternalFiles()
        {
            foreach (ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                reportProgress(-1, UrakawaSDK_daisy_Lang.CreatingExternalFiles);

                string filename = efd.OriginalRelativePath;
                if (filename.StartsWith(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA))
                {
                    filename = filename.Substring(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA.Length);
                }

                filename = FileDataProvider.EliminateForbiddenFileNameCharacters(filename);

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
