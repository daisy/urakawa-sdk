using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.xuk;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private XmlNode m_PackageUniqueIdAttr;
        private XmlNode m_PackagePrefixAttr;

        private string m_PublicationUniqueIdentifier;
        private XmlNode m_PublicationUniqueIdentifierNode;

        private string m_PublicationUniqueIdentifier_OPF;
        private XmlNode m_PublicationUniqueIdentifierNode_OPF;

        private void reInitialiseProjectSpine()
        {
            m_Xuk_FilePath = GetXukFilePath(m_outDirectory, m_Book_FilePath, null, true);

            //deleteDataDirectoryIfEmpty();
            string dataDir = m_Project.Presentations.Get(0).DataProviderManager.DataFileDirectoryFullPath;
            DebugFix.Assert(Directory.Exists(dataDir)); // because the call to DataFileDirectoryFullPath creates it!!

            if (Directory.Exists(dataDir))
            {
                string[] files = Directory.GetFiles(dataDir);
                if (files == null || files.Length == 0)
                {
                    string error = FileDataProvider.DeleteDirectory(dataDir);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Problem trying to delete folder (" + error + "): " + dataDir);
                    }
                }
            }

            initializeProject(@"_" + Path.GetFileName(m_Book_FilePath));
        }

        private void parseOpf(XmlDocument opfXmlDoc)
        {
            List<string> spine;
            Dictionary<string, string> spineAttributes;
            List<Dictionary<string, string>> spineItemsAttributes;
            string spineMimeType;
            string dtbookPath;
            string ncxPath;
            string navDocPath;
            string coverImagePath;

            if (RequestCancellation) return;

            XmlNode packageNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, true, "package", null);
            if (packageNode != null)
            {
                XmlAttributeCollection packageNodeAttrs = packageNode.Attributes;
                if (packageNodeAttrs != null && packageNodeAttrs.Count > 0)
                {
                    m_PackageUniqueIdAttr = packageNodeAttrs.GetNamedItem("unique-identifier");
                    m_PackagePrefixAttr = packageNodeAttrs.GetNamedItem("prefix");
                }
            }

            string idCoverImageFromMetadata = null;
            IEnumerable<XmlNode> metaElements = XmlDocumentHelper.GetChildrenElementsOrSelfWithName(opfXmlDoc, true, "meta", null, false);
            foreach (XmlNode metaElement in metaElements)
            {
                XmlAttributeCollection metaElementAttributes = metaElement.Attributes;

                if (metaElementAttributes == null || metaElementAttributes.Count <= 0)
                {
                    continue;
                }

                XmlNode attrName = metaElementAttributes.GetNamedItem("name");
                if (attrName == null || string.IsNullOrEmpty(attrName.Value)
                    || attrName.Value != @"cover")
                {
                    continue;
                }

                XmlNode attrContent = metaElementAttributes.GetNamedItem("content");
                if (attrContent == null && string.IsNullOrEmpty(attrContent.Value))
                {
                    continue;
                }

                idCoverImageFromMetadata = attrContent.Value;
                break;
            }

            parseOpfManifest(opfXmlDoc, out spine, out spineAttributes, out spineItemsAttributes, out spineMimeType, out dtbookPath, out ncxPath, out navDocPath, out coverImagePath, idCoverImageFromMetadata);

            if (RequestCancellation) return;

            m_IsSpine = spineMimeType == DataProviderFactory.XHTML_MIME_TYPE
                        || spineMimeType == DataProviderFactory.IMAGE_SVG_MIME_TYPE;
            DebugFix.Assert(m_IsSpine == !(spineMimeType == DataProviderFactory.SMIL_MIME_TYPE_));

            if (m_IsSpine)
            {
                reInitialiseProjectSpine();
            }
            XmlNode metadataXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc.DocumentElement, true, "metadata", null);
            parseMetadata(m_Book_FilePath, m_Project, opfXmlDoc, metadataXmlNode);

            m_PublicationUniqueIdentifier_OPF = m_PublicationUniqueIdentifier;
            m_PublicationUniqueIdentifierNode_OPF = m_PublicationUniqueIdentifierNode;

            if (dtbookPath != null && !m_IsSpine
                && !AudioNCXImport)
            {
                if (RequestCancellation) return;
                string fullDtbookPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), dtbookPath);
                XmlDocument dtbookXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullDtbookPath, true, true);

                if (parseContentDocParts(fullDtbookPath, m_Project, dtbookXmlDoc, dtbookPath, DocumentMarkupType.DTBOOK))
                {
                    return; // user cancel
                }

                //if (RequestCancellation) return;
                //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, dtbookPath));
                //parseMetadata(m_Book_FilePath, m_Project, dtbookXmlDoc);

                //if (RequestCancellation) return;
                //ParseHeadLinks(m_Book_FilePath, m_Project, dtbookXmlDoc);

                //if (RequestCancellation) return;
                //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, dtbookPath));
                //parseContentDocument(m_Book_FilePath, m_Project, dtbookXmlDoc, null, fullDtbookPath, null, DocumentMarkupType.DTBOOK);
            }

            //if (false && ncxPath != null) //we skip NCX metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            if (
                //(string.IsNullOrEmpty(dtbookPath) && !string.IsNullOrEmpty(ncxPath))  ||
                AudioNCXImport)
            {
                //m_IsAudioNCX = true;
                if (RequestCancellation) return;
                string fullNcxPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), ncxPath);
                XmlDocument ncxXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullNcxPath, false, false);

                if (RequestCancellation) return;
                reportProgress(-1, "Parsing metadata: [" + ncxPath + "]");

                XmlNode headXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxXmlDoc.DocumentElement, true, "head", null);
                parseMetadata(fullNcxPath, m_Project, ncxXmlDoc, headXmlNode);

                if (AudioNCXImport)
                {
                    ParseNCXDocument(ncxXmlDoc);
                }
            }

            if (RequestCancellation) return;
            switch (spineMimeType)
            {
                case DataProviderFactory.SMIL_MIME_TYPE_:
                    {
                        parseSmiles(spine);
                        break;
                    }
                case DataProviderFactory.XHTML_MIME_TYPE:
                case DataProviderFactory.IMAGE_SVG_MIME_TYPE:
                    {
                        if (!string.IsNullOrEmpty(coverImagePath) && !spine.Contains(coverImagePath))
                        {
                            string fullCoverImagePath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), coverImagePath);
                            fullCoverImagePath = FileDataProvider.NormaliseFullFilePath(fullCoverImagePath).Replace('/', '\\');

                            if (File.Exists(fullCoverImagePath))
                            {
                                ExternalFiles.ExternalFileData externalData = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create<ExternalFiles.CoverImageExternalFileData>();
                                try
                                {
                                    externalData.InitializeWithData(fullCoverImagePath, coverImagePath, true, null);

                                    addOPF_GlobalAssetPath(fullCoverImagePath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine(ex.StackTrace);

#if DEBUG
                                    Debugger.Break();
#endif
                                }
                            }
#if DEBUG
                            else
                            {
                                Debugger.Break();
                            }
#endif
                        }

                        if (!string.IsNullOrEmpty(navDocPath) && !spine.Contains(navDocPath))
                        {
                            string fullNavDocPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), navDocPath);
                            fullNavDocPath = FileDataProvider.NormaliseFullFilePath(fullNavDocPath).Replace('/', '\\');

                            if (File.Exists(fullNavDocPath))
                            {
                                ExternalFiles.ExternalFileData externalData = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create<ExternalFiles.NavDocExternalFileData>();
                                try
                                {
                                    externalData.InitializeWithData(fullNavDocPath, navDocPath, true, null);

                                    addOPF_GlobalAssetPath(fullNavDocPath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine(ex.StackTrace);

#if DEBUG
                                    Debugger.Break();
#endif
                                }
                            }
#if DEBUG
                            else
                            {
                                Debugger.Break();
                            }
#endif
                        }

                        if (!string.IsNullOrEmpty(ncxPath))
                        {
                            string fullNcxPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), ncxPath);
                            fullNcxPath = FileDataProvider.NormaliseFullFilePath(fullNcxPath).Replace('/', '\\');

                            if (File.Exists(fullNcxPath))
                            {
                                ExternalFiles.ExternalFileData externalData = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create<ExternalFiles.NCXExternalFileData>();
                                try
                                {
                                    externalData.InitializeWithData(fullNcxPath, ncxPath, true, null);

                                    addOPF_GlobalAssetPath(fullNcxPath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine(ex.StackTrace);

#if DEBUG
                                    Debugger.Break();
#endif
                                }
                            }
#if DEBUG
                            else
                            {
                                Debugger.Break();
                            }
#endif
                        }

                        parseContentDocuments(spine, spineAttributes, spineItemsAttributes, coverImagePath, navDocPath);

                        break;
                    }
            }
        }

        private void parseOpfManifest(XmlDocument opfXmlDoc,
                                out List<string> spine,
            out Dictionary<string, string> spineAttributes,
            out List<Dictionary<string, string>> spineItemsAttributes,
                                out string spineMimeType,
                                out string dtbookPath,
                                out string ncxPath,
                                out string navDocPath,
                                out string coverImagePath,
            string idCoverImageFromMetadata)
        {
            spine = new List<string>();
            spineAttributes = new Dictionary<string, string>();
            spineItemsAttributes = new List<Dictionary<string, string>>();
            spineMimeType = null;
            ncxPath = null;
            dtbookPath = null;
            navDocPath = null;
            coverImagePath = null;

            XmlNode spineNodeRoot = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, true, "spine", null);
            if (spineNodeRoot != null)
            {
                XmlAttributeCollection spineAttrs = spineNodeRoot.Attributes;

                if (spineAttrs != null && spineAttrs.Count > 0)
                {
                    foreach (XmlAttribute spineAttr in spineAttrs)
                    {
                        if (spineAttr.LocalName != "id" && spineAttr.LocalName != "toc")
                        {
                            spineAttributes.Add(spineAttr.Name, spineAttr.Value);
                        }
                    }
                }

                XmlNodeList listOfSpineItemNodes = spineNodeRoot.ChildNodes;
                foreach (XmlNode spineItemNode in listOfSpineItemNodes)
                {
                    if (RequestCancellation) return;

                    if (spineItemNode.NodeType != XmlNodeType.Element
                        || spineItemNode.LocalName != "itemref")
                    {
                        continue;
                    }
                    XmlAttributeCollection spineItemAttributes = spineItemNode.Attributes;

                    if (spineItemAttributes == null || spineItemAttributes.Count <= 0)
                    {
                        continue;
                    }

                    XmlNode attrIdRef = spineItemAttributes.GetNamedItem("idref");
                    if (attrIdRef != null && !string.IsNullOrEmpty(attrIdRef.Value))
                    {
                        spine.Add(attrIdRef.Value);

                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        spineItemsAttributes.Add(dict);
                        foreach (XmlAttribute spineItemAttr in spineItemAttributes)
                        {
                            if (spineItemAttr.LocalName != "id" && spineItemAttr.LocalName != "idref")
                            {
                                if (spineItemAttr.Name == @"properties")
                                {
                                    dict.Add(@"properties_spine", spineItemAttr.Value);
                                }
                                else
                                {
                                    dict.Add(spineItemAttr.Name, spineItemAttr.Value);
                                }
                            }
                        }
                    }
                }
            }

            XmlNode manifNodeRoot = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, true, "manifest", null);
            if (manifNodeRoot == null)
            {
                return;
            }

            XmlNodeList listOfManifestItemNodes = manifNodeRoot.ChildNodes;
            foreach (XmlNode manifItemNode in listOfManifestItemNodes)
            {
                if (RequestCancellation) return;

                if (manifItemNode.NodeType != XmlNodeType.Element
                    || manifItemNode.LocalName != "item")
                {
                    continue;
                }

                XmlAttributeCollection manifItemAttributes = manifItemNode.Attributes;
                if (manifItemAttributes == null)
                {
                    continue;
                }

                XmlNode attrHref = manifItemAttributes.GetNamedItem("href");
                XmlNode attrMediaType = manifItemAttributes.GetNamedItem("media-type");
                if (attrHref == null || String.IsNullOrEmpty(attrHref.Value)
                    || attrMediaType == null || String.IsNullOrEmpty(attrMediaType.Value))
                {
                    continue;
                }

                string href = FileDataProvider.UriDecode(attrHref.Value);

                XmlNode attrProperties = manifItemAttributes.GetNamedItem("properties");

                if (attrProperties != null && attrProperties.Value.Contains("cover-image"))
                {
                    DebugFix.Assert(attrMediaType.Value == DataProviderFactory.IMAGE_SVG_MIME_TYPE
                        || attrMediaType.Value == DataProviderFactory.IMAGE_JPG_MIME_TYPE
                        || attrMediaType.Value == DataProviderFactory.IMAGE_BMP_MIME_TYPE
                        || attrMediaType.Value == DataProviderFactory.IMAGE_PNG_MIME_TYPE
                        || attrMediaType.Value == DataProviderFactory.IMAGE_GIF_MIME_TYPE
                        );

                    coverImagePath = href;
                }

                XmlNode attrId = manifItemAttributes.GetNamedItem("id");
                if (string.IsNullOrEmpty(coverImagePath)
                    && attrId != null && attrId.Value == idCoverImageFromMetadata)
                {
                    coverImagePath = href;
                }

                if (attrMediaType.Value != DataProviderFactory.SMIL_MIME_TYPE // yep! (EPUB3)
                    &&
                    (attrMediaType.Value == DataProviderFactory.SMIL_MIME_TYPE_
                    || attrMediaType.Value == DataProviderFactory.XHTML_MIME_TYPE
                    || attrMediaType.Value == DataProviderFactory.IMAGE_SVG_MIME_TYPE))
                {
                    if (attrProperties != null && attrProperties.Value.Contains("nav"))
                    {
                        DebugFix.Assert(attrMediaType.Value == DataProviderFactory.XHTML_MIME_TYPE);

                        navDocPath = href;
                    }

                    if (attrId != null)
                    {
                        int i = spine.IndexOf(attrId.Value);
                        if (i >= 0)
                        {
                            spine[i] = href;
                            //if (!string.IsNullOrEmpty(spineMimeType)
                            //    && spineMimeType != attrMediaType.Value)
                            //{
                            //    System.Diagnostics.Debug.Fail(String.Format("Spine contains different mime-types ?! {0} vs {1}", attrMediaType.Value, spineMimeType));
                            //}
                            spineMimeType = attrMediaType.Value;

                            if (attrProperties != null)
                            {
                                Dictionary<string, string> dict = spineItemsAttributes[i];
                                
                                dict.Add(
                                    @"properties_manifest"
                                    //attrProperties.Name
                                    , attrProperties.Value);

                                //string val;
                                //dict.TryGetValue(attrProperties.Name, out val);
                                //if (!string.IsNullOrEmpty(val))
                                //{
                                //    dict.Remove(attrProperties.Name);
                                //    dict.Add(attrProperties.Name, val + @" " + attrProperties.Value);
                                //}
                                //else
                                //{
                                //    dict.Add(attrProperties.Name, attrProperties.Value);
                                //}
                            }

                            XmlNode attrMediaOverlay = manifItemAttributes.GetNamedItem("media-overlay");
                            if (attrMediaOverlay != null && !String.IsNullOrEmpty(attrMediaOverlay.Value))
                            {
                                foreach (XmlNode manifItemNode2 in listOfManifestItemNodes)
                                {
                                    XmlAttributeCollection manifItemAttributes2 = manifItemNode2.Attributes;
                                    if (manifItemAttributes2 == null)
                                    {
                                        continue;
                                    }
                                    XmlNode attrId2 = manifItemAttributes2.GetNamedItem("id");
                                    XmlNode attrHref2 = manifItemAttributes2.GetNamedItem("href");
                                    if (attrId2 != null && attrId2.Value == attrMediaOverlay.Value
                                        && attrHref2 != null && !string.IsNullOrEmpty(attrHref2.Value))
                                    {
                                        Dictionary<string, string> dict = spineItemsAttributes[i];
                                        dict.Add(attrMediaOverlay.Name, FileDataProvider.UriDecode(attrHref2.Value));
                                    }
                                }
                            }
                        }
                    }
                }
                else if (attrMediaType.Value == DataProviderFactory.DTBOOK_MIME_TYPE
                    || attrMediaType.Value == DataProviderFactory.XML_MIME_TYPE
                        && href.EndsWith(".xml"))
                {
                    dtbookPath = href;
                }
                else if (attrMediaType.Value == DataProviderFactory.NCX_MIME_TYPE
                    || attrMediaType.Value == DataProviderFactory.XML_MIME_TYPE
                        && href.EndsWith(".ncx"))
                {
                    ncxPath = href;
                }
                //else if (attrMediaType.Value == DataProviderFactory.STYLE_CSS_MIME_TYPE
                //    || attrMediaType.Value == DataProviderFactory.STYLE_PLS_MIME_TYPE)
                //{
                //    AddExternalFilesToXuk(m_Project.Presentations.Get(0), attrHref.Value);
                //}
                //else if (attrMediaType.Value == "image/jpeg"
                //|| attrMediaType.Value == "audio/mpeg"
                //|| attrMediaType.Value == DataProviderFactory.XML_TEXT_MIME_TYPE
                //|| attrMediaType.Value == "application/vnd.adobe.page-template+xml"
                //|| attrMediaType.Value == "application/oebps-page-map+xml"
                //|| attrMediaType.Value == DataProviderFactory.DTB_RES_MIME_TYPE)
                //{
                //    // Ignore
                //}
            }
        }

        //private void AddExternalFilesToXuk(Presentation presentation, string relPath)
        //{
        //    if (RequestCancellation) return;

        //    string fileFullPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath),
        //        relPath);

        //    ExternalFiles.ExternalFileData externalData = null;

        //    string ext = Path.GetExtension(fileFullPath);
        //    if (String.Equals(ext, DataProviderFactory.DTD_EXTENSION, StringComparison.OrdinalIgnoreCase))
        //    {
        //        externalData = presentation.ExternalFilesDataFactory.Create<ExternalFiles.DTDExternalFileData>();
        //    }
        //    if (String.Equals(ext, DataProviderFactory.STYLE_PLS_EXTENSION, StringComparison.OrdinalIgnoreCase))
        //    {
        //        externalData = presentation.ExternalFilesDataFactory.Create<ExternalFiles.PLSExternalFileData>();
        //    }
        //    if (externalData != null)
        //    {
        //        externalData.InitializeWithData(fileFullPath, relPath, true);
        //    }
        //}
    }
}
