using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.media;
using urakawa.media.data.video;
using urakawa.metadata;
using urakawa.core;
using urakawa.ExternalFiles;
using urakawa.metadata.daisy;
using urakawa.xuk;
using urakawa.media.data.audio;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private XmlDocument m_DTBDocument;
        private List<urakawa.core.TreeNode> m_NotesNodeList = new List<TreeNode>();

        private List<string> m_TempImageId = null;
        private Dictionary<TreeNode, List<XmlNode>> m_Image_ProdNoteMap = new Dictionary<TreeNode, List<XmlNode>>();

        protected virtual void CreateDTBookDocument()
        {
            // check if there is preserved internal DTD 
            //string[] dtbFilesList = Directory.GetFiles(m_Presentation.DataProviderManager.DataFileDirectoryFullPath, daisy.import.Daisy3_Import.INTERNAL_DTD_NAME, SearchOption.AllDirectories);
            //string strInternalDTD = null;
            //if (dtbFilesList.Length > 0)
            //{
            //    strInternalDTD = File.ReadAllText(dtbFilesList[0]);
            //    if (strInternalDTD.Trim() == "") strInternalDTD = null;
            //}



            List<ExternalFileData> list_ExternalStyleSheets = new List<ExternalFileData>();
            string strInternalDTD = null;
            foreach (ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (RequestCancellation) return;

                if (efd is ExternalFiles.DTDExternalFileData
                    && efd.OriginalRelativePath == daisy.import.Daisy3_Import.INTERNAL_DTD_NAME
                    && !efd.IsPreservedForOutputFile
                    && string.IsNullOrEmpty(strInternalDTD))
                {
                    StreamReader sr = new StreamReader(efd.OpenInputStream(), Encoding.UTF8);
                    strInternalDTD = sr.ReadToEnd();
                }
                else if (efd is ExternalFiles.CSSExternalFileData
                    || efd is XSLTExternalFileData
                    //&& !efd.OriginalRelativePath.StartsWith(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA)
                    )
                {
                    list_ExternalStyleSheets.Add(efd);
                }
            }

            if (RequestCancellation) return;

            //m_ProgressPercentage = 0;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreatingXMLFile);
            XmlDocument DTBookDocument = XmlDocumentHelper.CreateStub_DTBDocument(m_Presentation.Language, strInternalDTD, list_ExternalStyleSheets);

            string mathML_XSLT = null;

            foreach (ExternalFileData efd in list_ExternalStyleSheets)
            {
                string filename = efd.OriginalRelativePath;
                if (filename.StartsWith(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA))
                {
                    filename = filename.Substring(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA.Length);

                    mathML_XSLT = filename;
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

            string mathPrefix = m_Presentation.RootNode.GetXmlNamespacePrefix(DiagramContentModelHelper.NS_URL_MATHML);


            if (!string.IsNullOrEmpty(mathPrefix) && string.IsNullOrEmpty(mathML_XSLT))
            {
                string appDir = System.AppDomain.CurrentDomain.BaseDirectory;
                string xsltFileName = FileDataProvider.EliminateForbiddenFileNameCharacters(SupportedMetadata_Z39862005._builtInMathMLXSLT);
                string xsltFullPath = Path.Combine(appDir, xsltFileName);

                if (File.Exists(xsltFullPath))
                {
                    string destFile = Path.Combine(m_OutputDirectory, xsltFileName);
                    File.Copy(xsltFullPath, destFile, true);
                    try
                    {
                        File.SetAttributes(destFile, FileAttributes.Normal);
                    }
                    catch
                    {
                    }
                    m_FilesList_ExternalFiles.Add(xsltFileName);
                }
            }

            m_ListOfLevels = new List<TreeNode>();
            Dictionary<string, string> old_New_IDMap = new Dictionary<string, string>();
            List<XmlAttribute> referencingAttributesList = new List<XmlAttribute>();



            // add metadata
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "head", null); //DTBookDocument.GetElementsByTagName("head")[0]

            Metadata mdId = AddMetadata_DtbUid(false, DTBookDocument, headNode);

            AddMetadata_Generator(DTBookDocument, headNode);

            bool hasMathML_z39_86_extension_version = false;
            bool hasMathML_DTBook_XSLTFallback = false;

            // todo: filter-out unecessary metadata for DTBOOK (e.g. dtb:multimediatype)
            foreach (Metadata m in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (mdId == m) continue;

                XmlNode metaNode = DTBookDocument.CreateElement(null, "meta", headNode.NamespaceURI);
                headNode.AppendChild(metaNode);

                string name = m.NameContentAttribute.Name;

                if (name == SupportedMetadata_Z39862005._z39_86_extension_version)
                {
                    hasMathML_z39_86_extension_version = true;
                }
                else if (name == SupportedMetadata_Z39862005.MATHML_XSLT_METADATA)
                {
                    hasMathML_DTBook_XSLTFallback = true;
                }

                string prefix;
                string localName;
                urakawa.property.xml.XmlProperty.SplitLocalName(name, out prefix, out localName);

                if (!string.IsNullOrEmpty(prefix))
                {
                    // split the metadata name and make first alphabet upper, required for daisy 3.0

                    localName = localName.Substring(0, 1).ToUpper() + localName.Remove(0, 1);

                    name = prefix + ":" + localName;
                }

                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "name", name);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "content", m.NameContentAttribute.Value);

                // add metadata optional attributes if any
                foreach (MetadataAttribute ma in m.OtherAttributes.ContentsAs_Enumerable)
                {
                    if (ma.Name == "id" || ma.Name == Metadata.PrimaryIdentifierMark)
                    {
                        continue;
                    }

                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, ma.Name, ma.Value);
                }
            }

            if (!string.IsNullOrEmpty(mathML_XSLT))
            {
                DebugFix.Assert(hasMathML_z39_86_extension_version);
                DebugFix.Assert(hasMathML_DTBook_XSLTFallback);
            }

            if (!string.IsNullOrEmpty(mathPrefix) && !hasMathML_z39_86_extension_version)
            {
                XmlNode metaNode = DTBookDocument.CreateElement(null, "meta", headNode.NamespaceURI);
                headNode.AppendChild(metaNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "name", SupportedMetadata_Z39862005._z39_86_extension_version);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "content", "1.0");
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "scheme", DiagramContentModelHelper.NS_URL_MATHML);
            }

            if (!string.IsNullOrEmpty(mathPrefix) && !hasMathML_DTBook_XSLTFallback)
            {
                XmlNode metaNode = DTBookDocument.CreateElement(null, "meta", headNode.NamespaceURI);
                headNode.AppendChild(metaNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "name", SupportedMetadata_Z39862005.MATHML_XSLT_METADATA);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "content", string.IsNullOrEmpty(mathML_XSLT) ? SupportedMetadata_Z39862005._builtInMathMLXSLT : mathML_XSLT);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "scheme", DiagramContentModelHelper.NS_URL_MATHML);
            }

            // add elements to book body
            m_TreeNode_XmlNodeMap = new Dictionary<TreeNode, XmlNode>();



            XmlNode bookNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "book", null); //DTBookDocument.GetElementsByTagName("book")[0];
            if (bookNode == null)
            {
                bookNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "body", null);
            }


            XmlNode docRootNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "dtbook", null);
            if (docRootNode == null)
            {
                docRootNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "html", null);
            }

            if (false && !string.IsNullOrEmpty(mathML_XSLT)) // namespace prefix attribute automatically added for each m:math element because of MathML DTD
            {
                XmlDocumentHelper.CreateAppendXmlAttribute(
                DTBookDocument,
                bookNode,
                XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":" + "dtbook",
                bookNode.NamespaceURI,
                XmlReaderWriterHelper.NS_URL_XMLNS
                );
            }

            m_ListOfLevels.Add(m_Presentation.RootNode);

            m_TreeNode_XmlNodeMap.Add(m_Presentation.RootNode, bookNode);
            XmlNode currentXmlNode = null;
            bool isHeadingNodeAvailable = true;

            m_Presentation.RootNode.AcceptDepthFirst(
                    delegate(TreeNode n)
                    {
                        if (RequestCancellation) return false;
                        // add to list of levels if xml property has level string
                        //QualifiedName qName = n.GetXmlElementQName();
                        //if (qName != null &&
                        //    (qName.LocalName.StartsWith("level") || qName.LocalName == "doctitle" || qName.LocalName == "docauthor"))
                        //{
                        //    m_ListOfLevels.Add(n);
                        //}

                        if (doesTreeNodeTriggerNewSmil(n))
                        {
                            if (!isHeadingNodeAvailable && m_ListOfLevels.Count > 1)
                            {
                                m_ListOfLevels.RemoveAt(m_ListOfLevels.Count - 1);
                                //System.Windows.Forms.MessageBox.Show ( "removing :" + m_ListOfLevels.Count.ToString () );
                            }
                            m_ListOfLevels.Add(n);
                            isHeadingNodeAvailable = false;
                            reportProgress(-1, UrakawaSDK_daisy_Lang.CreatingXMLFile);
                        }

                        if (IsHeadingNode(n))
                        {
                            isHeadingNodeAvailable = true;
                        }
                        if (n.HasXmlProperty && (n.GetXmlElementLocalName() == "note" || n.GetXmlElementLocalName() == "annotation"))
                        {
                            m_NotesNodeList.Add(n);
                        }

                        property.xml.XmlProperty xmlProp = n.GetXmlProperty();

                        if (xmlProp == null)
                        {
                            string txtx = n.GetTextMedia() != null ? n.GetTextMedia().Text : null;
                            if (txtx != null)
                            {
                                XmlNode textNode = DTBookDocument.CreateTextNode(txtx);


                                ExternalAudioMedia extAudio = GetExternalAudioMedia(n);

                                if (extAudio == null)
                                {
                                    m_TreeNode_XmlNodeMap[n.Parent].AppendChild(textNode);
                                    m_TreeNode_XmlNodeMap.Add(n, textNode);
                                }
                                else
                                {
                                    Debug.Fail("TreeNode without XmlProperty but with TextMedia cannot have Audio attached to it ! (reason: at authoring time, an XmlProperty should have been added when audio was recorded for the pure-text TreeNode) => " + txtx);

                                    //XmlNode textParent = DTBookDocument.CreateElement(null, "sent", bookNode.NamespaceURI);
                                    //textParent.AppendChild(textNode);

                                    //m_TreeNode_XmlNodeMap[n.Parent].AppendChild(textParent);
                                    //m_TreeNode_XmlNodeMap.Add(n, textParent);
                                }
                            }

                            return true;
                        }

                        // create sml node in dtbook document

                        // code removed because XmlProperty stores proper namespaces, useful for inline MathML, SVG, whatever...
                        //string name = xmlProp.LocalName;
                        //string prefix = name.Contains(":") ? name.Split(':')[0] : null;
                        //string elementName = name.Contains(":") ? name.Split(':')[1] : name;
                        //currentXmlNode = DTBookDocument.CreateElement(prefix, elementName, bookNode.NamespaceURI);

                        bool notBookRoot = !"book".Equals(xmlProp.LocalName, StringComparison.OrdinalIgnoreCase)
                                           && !"body".Equals(xmlProp.LocalName, StringComparison.OrdinalIgnoreCase);


                        bool forceXmlNamespacePrefix = false;

                        if (//xmlProp.LocalName.Equals("math", StringComparison.OrdinalIgnoreCase)
                            xmlProp.GetNamespaceUri() == DiagramContentModelHelper.NS_URL_MATHML
                            && bookNode.LocalName.Equals("book", StringComparison.OrdinalIgnoreCase)
                        )
                        {
                            // Hack, because some MathML in DAISY is produced
                            // with redundant (and DTD-invalid) xmlns="http://MATHML"
                            forceXmlNamespacePrefix = true;
                        }

                        if (!notBookRoot)
                        {
                            currentXmlNode = bookNode;
                        }
                        else
                        {
                            XmlNode xmlNodeParent = m_TreeNode_XmlNodeMap[n.Parent];

                            string nsUri = n.GetXmlNamespaceUri();

                            if (string.IsNullOrEmpty(nsUri))
                            {
                                nsUri = xmlNodeParent.NamespaceURI;
                            }

                            DebugFix.Assert(!string.IsNullOrEmpty(nsUri));

                            if (string.IsNullOrEmpty(nsUri))
                            {
                                nsUri = bookNode.NamespaceURI;
                            }

                            if (string.IsNullOrEmpty(nsUri))
                            {
                                nsUri = DTBookDocument.DocumentElement.NamespaceURI;
                            }

                            if (string.IsNullOrEmpty(nsUri))
                            {
                                nsUri = DTBookDocument.NamespaceURI;
                            }

                            string prefix = (forceXmlNamespacePrefix || n.NeedsXmlNamespacePrefix()) ? n.GetXmlNamespacePrefix(nsUri) : null;

                            currentXmlNode = DTBookDocument.CreateElement(
                                prefix,
                                xmlProp.LocalName,
                                nsUri);

                            xmlNodeParent.AppendChild(currentXmlNode);

                            m_TreeNode_XmlNodeMap.Add(n, currentXmlNode);
                        }

                        // add attributes
                        for (int i = 0; i < xmlProp.Attributes.Count; i++)
                        {
                            property.xml.XmlAttribute xmlAttr = xmlProp.Attributes.Get(i);

                            string prefix = xmlAttr.Prefix;
                            string nameWithoutPrefix = xmlAttr.PrefixedLocalName != null ? xmlAttr.PrefixedLocalName : xmlAttr.LocalName;

                            if (!string.IsNullOrEmpty(prefix))
                            {
                                string nsUriPrefix = xmlProp.GetNamespaceUri(prefix);

                                if (string.IsNullOrEmpty(nsUriPrefix))
                                {
#if DEBUG
                                    Debugger.Break();
#endif //DEBUG
                                    nsUriPrefix = currentXmlNode.GetNamespaceOfPrefix(prefix);
                                }

                                if (!string.IsNullOrEmpty(nsUriPrefix)
                                    && string.IsNullOrEmpty(DTBookDocument.DocumentElement.GetNamespaceOfPrefix(prefix))
                                    && string.IsNullOrEmpty(bookNode.GetNamespaceOfPrefix(prefix)))
                                {
                                    XmlDocumentHelper.CreateAppendXmlAttribute(
                                        DTBookDocument,
                                        docRootNode, //bookNode,
                                        XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":" + prefix,
                                        nsUriPrefix,
                                        XmlReaderWriterHelper.NS_URL_XMLNS
                                        );
                                }
                            }

                            //todo: check ID attribute, normalize with fresh new list of IDs
                            // (warning: be careful maintaining ID REFS, such as idref attributes for annotation/annoref and prodnote/noteref
                            // (be careful because idref contain URIs with hash character),
                            // and also the special imgref and headers attributes which contain space-separated list of IDs, not URIs)

                            if (nameWithoutPrefix == "id") //  xmlAttr.LocalName == "id" || xmlAttr.LocalName == XmlReaderWriterHelper.XmlId)
                            {
                                string id_New = GetNextID(ID_DTBPrefix);
                                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument,
                                    currentXmlNode,
                                    "id", id_New);

                                if (!old_New_IDMap.ContainsKey(xmlAttr.Value))
                                {
                                    old_New_IDMap.Add(xmlAttr.Value, id_New);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Fail("Duplicate ID found in original DTBook document", "Original DTBook document has duplicate ID: " + xmlProp.Attributes.Get(i).Value);
                                }
                            }
                            else
                            {
                                XmlNode xmlNd = null;
                                if (currentXmlNode == bookNode
                                    &&
                                    (prefix == XmlReaderWriterHelper.NS_PREFIX_XMLNS
                                    || nameWithoutPrefix == "lang"))
                                {
                                    // Hack: to make sure DTD validation passes.
                                    xmlNd = docRootNode;
                                }
                                else
                                {
                                    xmlNd = currentXmlNode;
                                }

                                if (forceXmlNamespacePrefix
                                    && nameWithoutPrefix == XmlReaderWriterHelper.NS_PREFIX_XMLNS
                                    && xmlAttr.Value == DiagramContentModelHelper.NS_URL_MATHML
                                    )
                                {
                                    bool debug = true; // skip xmlns="http://MATH"
                                }
                                else
                                {
                                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument,
                                                        xmlNd,
                                                        xmlAttr.LocalName,
                                                        xmlAttr.Value,
                                                        xmlAttr.GetNamespaceUri());
                                }
                            }
                        } // for loop ends

                        if (!notBookRoot)
                        {
                            return true;
                        }


                        if (xmlProp.LocalName == "imggroup")
                        {
                            m_TempImageId = new List<string>(1);
                        }
                        if (m_TempImageId != null // !string.IsNullOrEmpty(m_TempImageId)
                            && (xmlProp.LocalName == "caption" || xmlProp.LocalName == "prodnote"))
                        {
                            string imgIds = "";
                            foreach (string imgId in m_TempImageId)
                            {
                                imgIds += imgId + " ";
                            }
                            imgIds = imgIds.Trim();
                            if (!string.IsNullOrEmpty(imgIds))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, currentXmlNode, "imgref", imgIds);
                            }
                        }

                        XmlAttributeCollection currentXmlNodeAttrs = currentXmlNode.Attributes;

                        // add id attribute in case it do not exists and it is required
                        if (IsIDRequired(currentXmlNode.LocalName) && currentXmlNodeAttrs != null)
                        {
                            XmlNode idAttr = currentXmlNodeAttrs.GetNamedItem("id");
                            if (idAttr == null)
                            {
                                string id = GetNextID(ID_DTBPrefix);
                                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, currentXmlNode, "id", id);
                            }
                            else if (xmlProp.LocalName != null
                                && xmlProp.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase)
                                && m_TempImageId != null)
                            {
                                string id = idAttr.Value;
                                m_TempImageId.Add(id);
                            }
                        }

                        // add text from text property

                        string txt = n.GetTextMedia() != null ? n.GetTextMedia().Text : null;
                        if (txt != null)
                        {
                            XmlNode textNode = DTBookDocument.CreateTextNode(txt);
                            currentXmlNode.AppendChild(textNode);

                            DebugFix.Assert(n.Children.Count == 0);
                        }

                        // if current xmlnode is referencing node, add its referencing attribute to referencingAttributesList
                        AddReferencingNodeToReferencedAttributesList(currentXmlNode, referencingAttributesList);

                        if (currentXmlNodeAttrs != null && currentXmlNode.LocalName != null)
                        {
                            bool isHTML = @"body".Equals(m_Presentation.RootNode.GetXmlElementLocalName(), StringComparison.OrdinalIgnoreCase); //n.Presentation
                            // TODO: special treatment of subfolders in file paths (restore full hierarchy, including OPF, XHTMLs, etc., not just referenced media assets)

                            if (currentXmlNode.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
                            {
                                XmlAttribute imgSrcAttribute =
                                    (XmlAttribute)currentXmlNodeAttrs.GetNamedItem("src");
                                if (imgSrcAttribute != null &&
                                    n.GetImageMedia() != null
                                    && n.GetImageMedia() is media.data.image.ManagedImageMedia)
                                {
                                    media.data.image.ManagedImageMedia managedImage =
                                        (media.data.image.ManagedImageMedia)n.GetImageMedia();

                                    //if (FileDataProvider.isHTTPFile(managedImage.ImageMediaData.OriginalRelativePath))                                
                                    //exportImageName = Path.GetFileName(managedImage.ImageMediaData.OriginalRelativePath);

                                    string exportImageName =
                                        //Path.GetFileName
                                        FileDataProvider.EliminateForbiddenFileNameCharacters
                                        (managedImage.ImageMediaData.OriginalRelativePath)
                                        ;

                                    string destPath = Path.Combine(m_OutputDirectory, exportImageName);

                                    if (!File.Exists(destPath))
                                    {
                                        if (RequestCancellation) return false;
                                        managedImage.ImageMediaData.DataProvider.ExportDataStreamToFile(destPath, false);
                                    }

                                    imgSrcAttribute.Value = FileDataProvider.UriEncode(exportImageName);

                                    if (!m_FilesList_Image.Contains(exportImageName))
                                    {
                                        m_FilesList_Image.Add(exportImageName);
                                    }

                                    generateImageDescriptionInDTBook(n, currentXmlNode, exportImageName, DTBookDocument);
                                }
                            }
                            else if (currentXmlNode.LocalName.Equals(DiagramContentModelHelper.Math, StringComparison.OrdinalIgnoreCase))
                            {
                                XmlAttribute imgSrcAttribute =
                                    (XmlAttribute)currentXmlNodeAttrs.GetNamedItem("altimg");
                                if (imgSrcAttribute != null &&
                                    n.GetImageMedia() != null
                                    && n.GetImageMedia() is media.data.image.ManagedImageMedia)
                                {
                                    media.data.image.ManagedImageMedia managedImage =
                                        (media.data.image.ManagedImageMedia)n.GetImageMedia();

                                    //if (FileDataProvider.isHTTPFile(managedImage.ImageMediaData.OriginalRelativePath))                                
                                    //exportImageName = Path.GetFileName(managedImage.ImageMediaData.OriginalRelativePath);

                                    string exportImageName =
                                        //Path.GetFileName
                                        FileDataProvider.EliminateForbiddenFileNameCharacters
                                        (managedImage.ImageMediaData.OriginalRelativePath)
                                        ;

                                    string destPath = Path.Combine(m_OutputDirectory, exportImageName);

                                    if (!File.Exists(destPath))
                                    {
                                        if (RequestCancellation) return false;
                                        managedImage.ImageMediaData.DataProvider.ExportDataStreamToFile(destPath, false);
                                    }

                                    imgSrcAttribute.Value = FileDataProvider.UriEncode(exportImageName);

                                    if (!m_FilesList_Image.Contains(exportImageName))
                                    {
                                        m_FilesList_Image.Add(exportImageName);
                                    }
                                }
                            }

#if SUPPORT_AUDIO_VIDEO
                            else if (currentXmlNode.LocalName.Equals("video", StringComparison.OrdinalIgnoreCase)
                                || (
                                currentXmlNode.LocalName.Equals("source", StringComparison.OrdinalIgnoreCase)
                                && currentXmlNode.ParentNode != null
                                && currentXmlNode.ParentNode.LocalName.Equals("video", StringComparison.OrdinalIgnoreCase)
                                )
                                )
                            {
                                XmlAttribute videoSrcAttribute =
                                    (XmlAttribute)currentXmlNodeAttrs.GetNamedItem("src");
                                if (videoSrcAttribute != null &&
                                    n.GetVideoMedia() != null
                                    && n.GetVideoMedia() is ManagedVideoMedia)
                                {
                                    ManagedVideoMedia managedVideo = (ManagedVideoMedia)n.GetVideoMedia();

                                    //if (FileDataProvider.isHTTPFile(managedVideo.VideoMediaData.OriginalRelativePath))                                
                                    //exportVideoName = Path.GetFileName(managedVideo.VideoMediaData.OriginalRelativePath);

                                    string exportVideoName =
                                        FileDataProvider.EliminateForbiddenFileNameCharacters(
                                            managedVideo.VideoMediaData.OriginalRelativePath);

                                    string destPath = Path.Combine(m_OutputDirectory, exportVideoName);



                                    if (!File.Exists(destPath))
                                    {
                                        if (RequestCancellation) return false;
                                        managedVideo.VideoMediaData.DataProvider.ExportDataStreamToFile(destPath, false);
                                    }

                                    videoSrcAttribute.Value = FileDataProvider.UriEncode(exportVideoName);

                                    if (!m_FilesList_Video.Contains(exportVideoName))
                                    {
                                        m_FilesList_Video.Add(exportVideoName);
                                    }
                                }
                            }
                            else if (currentXmlNode.LocalName.Equals("audio", StringComparison.OrdinalIgnoreCase)
                                || (
                                currentXmlNode.LocalName.Equals("source", StringComparison.OrdinalIgnoreCase)
                                && currentXmlNode.ParentNode != null
                                && currentXmlNode.ParentNode.LocalName.Equals("audio", StringComparison.OrdinalIgnoreCase)
                                )
                                )
                            {
                                XmlAttribute audioSrcAttribute =
                                    (XmlAttribute)currentXmlNodeAttrs.GetNamedItem("src");
                                ManagedAudioMedia managedAudio = n.GetManagedAudioMedia();
                                if (audioSrcAttribute != null &&
                                    managedAudio != null)
                                {

                                    //if (FileDataProvider.isHTTPFile(managedAudio.AudioMediaData.OriginalRelativePath))                                
                                    //exportAudioName = Path.GetFileName(managedAudio.AudioMediaData.OriginalRelativePath);

                                    string exportAudioName =
                                        FileDataProvider.EliminateForbiddenFileNameCharacters(
                                            managedAudio.AudioMediaData.OriginalRelativePath);

                                    string destPath = Path.Combine(m_OutputDirectory, exportAudioName);



                                    if (!File.Exists(destPath))
                                    {
                                        if (RequestCancellation) return false;
                                        managedAudio.AudioMediaData.DataProvider.ExportDataStreamToFile(destPath, false);
                                    }

                                    audioSrcAttribute.Value = FileDataProvider.UriEncode(exportAudioName);

                                    if (!m_FilesList_Audio.Contains(exportAudioName))
                                    {
                                        m_FilesList_Audio.Add(exportAudioName);
                                    }
                                }
                            }

#endif
                        }


                        return true;
                    },
                    delegate(urakawa.core.TreeNode n)
                    {
                        property.xml.XmlProperty xmlProp = n.GetXmlProperty();
                        //QualifiedName qName = n.GetXmlElementQName();
                        if (xmlProp != null && xmlProp.LocalName == "imggroup")
                        {
                            m_TempImageId = null;
                        }
                    });

            if (RequestCancellation) return;
            // set references to new ids
            foreach (XmlAttribute attr in referencingAttributesList)
            {
                string strIDToFind = attr.Value;
                if (strIDToFind.IndexOf('#') >= 0) //strIDToFind.Contains("#")
                {
                    strIDToFind = strIDToFind.Split('#')[1];
                }

                string str;
                old_New_IDMap.TryGetValue(strIDToFind, out str);

                if (!string.IsNullOrEmpty(str)) //old_New_IDMap.ContainsKey(strIDToFind))
                {
                    string id_New = str; // old_New_IDMap[strIDToFind];

                    attr.Value = "#" + id_New;
                }
            }

            m_DTBDocument = DTBookDocument;
            //CommonFunctions.WriteXmlDocumentToFile(DTBookDocument,
            //  Path.Combine(m_OutputDirectory, m_Filename_Content));

        }

        private bool IsIDRequired(string nodeLocalName)
        {
            if (string.IsNullOrEmpty(nodeLocalName)
                || nodeLocalName == "book"
                || nodeLocalName == "body"
                || nodeLocalName == "frontmatter"
                || nodeLocalName == "bodymatter"
                 || nodeLocalName == "rearmatter"
                || nodeLocalName.StartsWith("level"))
            {
                return false;
            }
            return true;
        }

        private bool AddReferencingNodeToReferencedAttributesList(XmlNode node, List<XmlAttribute> attributesList)
        {
            string nodeLocalName = node.LocalName;

            if (nodeLocalName == "noteref"
                || nodeLocalName == "annoref")
            {
                if (node.Attributes.GetNamedItem("idref") != null)
                {
                    attributesList.Add((XmlAttribute)node.Attributes.GetNamedItem("idref"));
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.Fail(node.Name + " Should have idref attribute!");
                }

            }
            else if (nodeLocalName == "a")
            {
                if (node.Attributes.GetNamedItem("href") != null)
                {
                    attributesList.Add((XmlAttribute)node.Attributes.GetNamedItem("href"));
                    return true;
                }
            }
            return false;
        }


        /*
        private XmlDocument SaveCurrentSmilAndCreateNextSmilDocument ( XmlDocument smilDocument )
            {
            CommonFunctions.WriteXmlDocumentToFile ( smilDocument,
                Path.Combine ( m_OutputDirectory, m_CurrentSmilFileName ) );
            m_CurrentSmilFileName = GetNextSmilFileName;
            return CreateStub_SmilDocument ();
            }


        private void AddNodeToSmil ( XmlDocument smilDocument, XmlNode dtbNode, urakawa.media.ExternalAudioMedia externalMedia )
            {
            XmlNode mainSeq = smilDocument.GetElementsByTagName ( "body" )[0].FirstChild;
            XmlNode parNode = smilDocument.CreateElement ( null, "par", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, parNode, "id", GetNextID ( ID_SmilPrefix ) );
            mainSeq.AppendChild ( parNode );

            XmlNode textNode = smilDocument.CreateElement ( null, "text", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "id", GetNextID ( ID_SmilPrefix ) );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "src", m_ContentFileName + "#" + dtbNode.Attributes.GetNamedItem ( "id" ).Value );
            parNode.AppendChild ( textNode );

            XmlNode audioNode = smilDocument.CreateElement ( null, "audio", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipBegin", externalMedia.ClipBegin.Format_Standard () );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipEnd", externalMedia.ClipEnd.Format_Standard () );
         
         string extAudioSrc = AdjustAudioFileName(externalAudio, levelNodeDescendant);

            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "src", Path.GetFileName ( extAudioSrc ) );
            parNode.AppendChild ( audioNode );

            }
        */


    }
}
