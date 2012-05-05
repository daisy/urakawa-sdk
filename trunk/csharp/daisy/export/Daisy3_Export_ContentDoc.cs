using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.media;
using urakawa.media.data.video;
using urakawa.metadata;
using urakawa.core;
using urakawa.ExternalFiles;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private XmlDocument m_DTBDocument;
        private List<urakawa.core.TreeNode> m_NotesNodeList = new List<TreeNode>();

        private List<string> m_TempImageId = null;
        private Dictionary<TreeNode, List<XmlNode>> m_Image_ProdNoteMap = new Dictionary<TreeNode, List<XmlNode>>();

        // to do regenerate ids
        protected virtual void CreateDTBookDocument()
        {
            // check if there is preserved internal DTD 
            //string[] dtbFilesList = Directory.GetFiles(m_Presentation.DataProviderManager.DataFileDirectoryFullPath, "DTBookLocalDTD.dtd", SearchOption.AllDirectories);
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

                if (efd is ExternalFiles.DTDExternalFileData &&
                    efd.OriginalRelativePath == "DTBookLocalDTD.dtd" && !efd.IsPreservedForOutputFile
                    && strInternalDTD == null)
                {
                    StreamReader sr = new StreamReader(efd.OpenInputStream());
                    strInternalDTD = sr.ReadToEnd();
                }
                else if (efd is ExternalFiles.CSSExternalFileData || efd is XSLTExternalFileData)
                {
                    list_ExternalStyleSheets.Add(efd);
                }
            }

            if (RequestCancellation) return;

            //m_ProgressPercentage = 0;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreatingXMLFile);
            XmlDocument DTBookDocument = XmlDocumentHelper.CreateStub_DTBDocument(m_Presentation.Language, strInternalDTD, list_ExternalStyleSheets);
            if (list_ExternalStyleSheets != null) ExportStyleSheets(list_ExternalStyleSheets);

            m_ListOfLevels = new List<TreeNode>();
            Dictionary<string, string> old_New_IDMap = new Dictionary<string, string>();
            List<XmlAttribute> referencingAttributesList = new List<XmlAttribute>();



            // add metadata
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "head", null); //DTBookDocument.GetElementsByTagName("head")[0]

            Metadata mdId = AddMetadata_DtbUid(false, DTBookDocument, headNode);

            AddMetadata_Generator(DTBookDocument, headNode);

            // todo: filter-out unecessary metadata for DTBOOK (e.g. dtb:multimediatype)
            foreach (Metadata m in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (mdId == m) continue;

                XmlNode metaNode = DTBookDocument.CreateElement(null, "meta", headNode.NamespaceURI);
                headNode.AppendChild(metaNode);

                string name = m.NameContentAttribute.Name;

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
                    if (ma.Name == "id") continue;

                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, ma.Name, ma.Value);
                }
            }

            // add elements to book body
            m_TreeNode_XmlNodeMap = new Dictionary<TreeNode, XmlNode>();


            TreeNode rNode = m_Presentation.RootNode;
            XmlNode bookNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "book", null); //DTBookDocument.GetElementsByTagName("book")[0];
            if (bookNode == null)
            {
                bookNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(DTBookDocument, true, "body", null);
            }

            m_ListOfLevels.Add(m_Presentation.RootNode);

            m_TreeNode_XmlNodeMap.Add(rNode, bookNode);
            XmlNode currentXmlNode = null;
            bool isHeadingNodeAvailable = true;

            rNode.AcceptDepthFirst(
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

                        bool notBookRoot = xmlProp.LocalName != "book" && xmlProp.LocalName != "body";

                        if (!notBookRoot)
                        {
                            currentXmlNode = bookNode;
                        }
                        else
                        {
                            XmlNode xmlNodeParent = m_TreeNode_XmlNodeMap[n.Parent];

                            string nsUri = n.GetXmlNamespaceUri();

                            string prefix = xmlNodeParent.GetPrefixOfNamespace(nsUri);
                            if (prefix == null)
                            {
                                foreach (property.xml.XmlAttribute xmlAttr in xmlProp.Attributes.ContentsAs_Enumerable)
                                {
                                    string prefixAttr = xmlAttr.Prefix;
                                    string nameWithoutPrefix = xmlAttr.PrefixedLocalName != null ? xmlAttr.PrefixedLocalName : xmlAttr.LocalName;

                                    if (prefixAttr == XmlReaderWriterHelper.NS_PREFIX_XMLNS
                                        && xmlAttr.Value == nsUri)
                                    {
                                        prefix = nameWithoutPrefix;
                                        break;
                                    }
                                }
                            }

                            currentXmlNode = DTBookDocument.CreateElement(prefix, xmlProp.LocalName, nsUri);

                            // add current node to its parent
                            xmlNodeParent.AppendChild(currentXmlNode);

                            // add nodes to dictionary 
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
                                string nsUriPrefix = null;

                                if (prefix == XmlReaderWriterHelper.NS_PREFIX_XMLNS)
                                {
                                    nsUriPrefix = XmlReaderWriterHelper.NS_URL_XMLNS;
                                }
                                else if (prefix == XmlReaderWriterHelper.NS_PREFIX_XML)
                                {
                                    nsUriPrefix = XmlReaderWriterHelper.NS_URL_XML;
                                }
                                else
                                {
                                    nsUriPrefix = xmlProp.GetNamespaceUri(prefix);
                                }

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
                                        bookNode,
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
                                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument,
                                currentXmlNode,
                                xmlAttr.LocalName,
                                xmlAttr.Value,
                                xmlAttr.GetNamespaceUri());
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
                        // add id attribute in case it do not exists and it is required
                        if (IsIDRequired(currentXmlNode.LocalName))
                        {
                            if (currentXmlNode.Attributes.GetNamedItem("id") == null)
                            {
                                string id = GetNextID(ID_DTBPrefix);
                                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, currentXmlNode, "id", id);
                            }

                            if (xmlProp.LocalName != null
                                && xmlProp.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase)
                                && m_TempImageId != null)
                            {
                                string id = currentXmlNode.Attributes.GetNamedItem("id").Value;
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

                        // if QName is img and img src is on disk, copy it to output dir
                        if (currentXmlNode.LocalName != null
                            && currentXmlNode.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlAttribute imgSrcAttribute = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("src");
                            if (imgSrcAttribute != null &&
                                n.GetImageMedia() != null
                                && n.GetImageMedia() is media.data.image.ManagedImageMedia)
                            {
                                media.data.image.ManagedImageMedia managedImage = (media.data.image.ManagedImageMedia)n.GetImageMedia();

                                //if (FileDataProvider.isHTTPFile(managedImage.ImageMediaData.OriginalRelativePath))                                
                                //exportImageName = Path.GetFileName(managedImage.ImageMediaData.OriginalRelativePath);

                                string exportImageName = FileDataProvider.EliminateForbiddenFileNameCharacters(managedImage.ImageMediaData.OriginalRelativePath);

                                string destPath = Path.Combine(m_OutputDirectory, exportImageName);

                                if (!File.Exists(destPath))
                                {
                                    if (RequestCancellation) return false;
                                    managedImage.ImageMediaData.DataProvider.ExportDataStreamToFile(destPath, false);

                                }

                                imgSrcAttribute.Value = exportImageName;



                                if (!m_FilesList_Image.Contains(exportImageName))
                                {
                                    m_FilesList_Image.Add(exportImageName);
                                }

                                generateImageDescriptionInDTBook(n, currentXmlNode, exportImageName, DTBookDocument);
                            }
                        }
                        else if (currentXmlNode.LocalName != null
                            && currentXmlNode.LocalName.Equals("video", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlAttribute videoSrcAttribute = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("src");
                            if (videoSrcAttribute != null &&
                                n.GetVideoMedia() != null
                                && n.GetVideoMedia() is ManagedVideoMedia)
                            {
                                ManagedVideoMedia managedVideo = (ManagedVideoMedia)n.GetVideoMedia();

                                //if (FileDataProvider.isHTTPFile(managedVideo.VideoMediaData.OriginalRelativePath))                                
                                //exportVideoName = Path.GetFileName(managedVideo.VideoMediaData.OriginalRelativePath);

                                string exportVideoName = FileDataProvider.EliminateForbiddenFileNameCharacters(managedVideo.VideoMediaData.OriginalRelativePath);

                                string destPath = Path.Combine(m_OutputDirectory, exportVideoName);
                                if (!File.Exists(destPath))
                                {
                                    if (RequestCancellation) return false;
                                    managedVideo.VideoMediaData.DataProvider.ExportDataStreamToFile(destPath, false);

                                }

                                videoSrcAttribute.Value = exportVideoName;



                                if (!m_FilesList_Video.Contains(exportVideoName))
                                {
                                    m_FilesList_Video.Add(exportVideoName);
                                }

                            }
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

        private void ExportStyleSheets(List<ExternalFileData> list_ExternalStyleSheets)
        {
            if (list_ExternalStyleSheets == null ||
                (list_ExternalStyleSheets != null && list_ExternalStyleSheets.Count == 0))
            {
                return;
            }

            foreach (ExternalFileData efd in list_ExternalStyleSheets)
            {
                if (efd.IsPreservedForOutputFile && !m_FilesList_ExternalFiles.Contains(efd.OriginalRelativePath))
                {
                    string filePath = Path.Combine(m_OutputDirectory, efd.OriginalRelativePath);
                    efd.DataProvider.ExportDataStreamToFile(filePath, true);
                    m_FilesList_ExternalFiles.Add(efd.OriginalRelativePath);

                }
            }
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
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "src", Path.GetFileName ( externalMedia.Src ) );
            parNode.AppendChild ( audioNode );

            }
        */


    }
}
