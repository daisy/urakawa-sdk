using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.media;
using urakawa.metadata;
using urakawa.core;
using urakawa.ExternalFiles;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private XmlDocument m_DTBDocument;
        // to do regenerate ids
        private void CreateDTBookDocument()
        {
            // check if there is preserved internal DTD 
            //string[] dtbFilesList = Directory.GetFiles(m_Presentation.DataProviderManager.DataFileDirectoryFullPath, "DTBookLocalDTD.dtd", SearchOption.AllDirectories);
            //string strInternalDTD = null;
            //if (dtbFilesList.Length > 0)
            //{
            //    strInternalDTD = File.ReadAllText(dtbFilesList[0]);
            //    if (strInternalDTD.Trim() == "") strInternalDTD = null;
            //}

            //string dtdFilePath = Path.Combine(m_Presentation.DataProviderManager.DataFileDirectoryFullPath,
                                              //"DTBookLocalDTD.dtd");
        m_FilesList_Image = new List<string> ();
        m_FilesList_ExternalFiles = new List<string> ();
        List<ExternalFileData> list_ExternalStyleSheets = new List<ExternalFileData> ();
            string strInternalDTD = null;
            foreach (ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_ListAsReadOnly)
                {
                if (efd is ExternalFiles.CSSExternalFileData &&
                    efd.OriginalRelativePath == "DTBookLocalDTD.dtd" && !efd.IsPreservedForOutputFile
                    && strInternalDTD == null)
                    {
                    StreamReader sr = new StreamReader ( efd.OpenInputStream () );
                    strInternalDTD = sr.ReadToEnd ();
                    
                    }
                else if (efd is ExternalFiles.CSSExternalFileData    ||   efd is XSLTExternalFileData)
                    {
                    list_ExternalStyleSheets.Add ( efd );
                    
                    }
                }
            
            //if (File.Exists(dtdFilePath))
            //{
                //strInternalDTD = File.ReadAllText(dtdFilePath);
            //}

            XmlDocument DTBookDocument = XmlDocumentHelper.CreateStub_DTBDocument(m_Presentation.Language, strInternalDTD, list_ExternalStyleSheets);
            if ( list_ExternalStyleSheets != null )  ExportStyleSheets ( list_ExternalStyleSheets );

            m_ListOfLevels = new List<TreeNode>();
            Dictionary<string, string> old_New_IDMap = new Dictionary<string, string>();
            List<XmlAttribute> referencingAttributesList = new List<XmlAttribute>();

            

            // add metadata
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementWithName(DTBookDocument, true, "head", null); //DTBookDocument.GetElementsByTagName("head")[0]

            Metadata mdId = AddMetadata_DtbUid(false, DTBookDocument, headNode);

            AddMetadata_Generator(DTBookDocument, headNode);

            // todo: filter-out unecessary metadata for DTBOOK (e.g. dtb:multimediatype)
            foreach (Metadata m in m_Presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (mdId == m) continue;

                XmlNode metaNode = DTBookDocument.CreateElement(null, "meta", headNode.NamespaceURI);
                headNode.AppendChild(metaNode);

                string name = m.NameContentAttribute.Name;

                if (name.Contains(":"))
                {
                    // split the metadata name and make first alphabet upper, required for daisy 3.0
                    string splittedName = name.Split(':')[1];
                    splittedName = splittedName.Substring(0, 1).ToUpper() + splittedName.Remove(0, 1);

                    name = name.Split(':')[0] + ":" + splittedName;
                }

                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "name", name);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, "content", m.NameContentAttribute.Value);

                // add metadata optional attributes if any
                foreach (MetadataAttribute ma in m.OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    if (ma.Name == "id") continue;

                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, metaNode, ma.Name, ma.Value);
                }
            }

            // add elements to book body
            m_TreeNode_XmlNodeMap = new Dictionary<TreeNode, XmlNode>();


            TreeNode rNode = m_Presentation.RootNode;
            XmlNode bookNode = XmlDocumentHelper.GetFirstChildElementWithName(DTBookDocument, true, "book", null); //DTBookDocument.GetElementsByTagName("book")[0];

            m_ListOfLevels.Add(m_Presentation.RootNode);

            m_TreeNode_XmlNodeMap.Add(rNode, bookNode);
            XmlNode currentXmlNode = null;

            rNode.AcceptDepthFirst(
                    delegate(TreeNode n)
                    {
                        // add to list of levels if xml property has level string
                        //QualifiedName qName = n.GetXmlElementQName();
                        //if (qName != null &&
                        //    (qName.LocalName.StartsWith("level") || qName.LocalName == "doctitle" || qName.LocalName == "docauthor"))
                        //{
                        //    m_ListOfLevels.Add(n);
                        //}

                        if (doesTreeNodeTriggerNewSmil(n))
                        {
                            m_ListOfLevels.Add(n);
                        }


                        property.xml.XmlProperty xmlProp = n.GetProperty<property.xml.XmlProperty>();

                        if (xmlProp != null && xmlProp.LocalName == "book") return true;

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

                        currentXmlNode = DTBookDocument.CreateElement(null, xmlProp.LocalName, (string.IsNullOrEmpty(xmlProp.NamespaceUri) ? bookNode.NamespaceURI : xmlProp.NamespaceUri));

                        // add attributes
                        if (xmlProp.Attributes != null && xmlProp.Attributes.Count > 0)
                        {
                            for (int i = 0; i < xmlProp.Attributes.Count; i++)
                            {
                                //todo: check ID attribute, normalize with fresh new list of IDs
                                // (warning: be careful maintaining ID REFS, such as idref attributes for annotation/annoref and prodnote/noteref
                                // (be careful because idref contain URIs with hash character),
                                // and also the special imgref and headers attributes which contain space-separated list of IDs, not URIs)

                                if (xmlProp.Attributes[i].LocalName == "id")
                                {
                                    string id_New = GetNextID(ID_DTBPrefix);
                                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument,
                                        currentXmlNode,
                                        "id", id_New);

                                    if (!old_New_IDMap.ContainsKey(xmlProp.Attributes[i].Value))
                                    {
                                        old_New_IDMap.Add(xmlProp.Attributes[i].Value, id_New);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.Fail("Duplicate ID found in original DTBook document", "Original DTBook document has duplicate ID: " + xmlProp.Attributes[i].Value);
                                    }
                                }
                                else
                                {
                                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument,
                                    currentXmlNode,
                                    xmlProp.Attributes[i].LocalName, 
                                    xmlProp.Attributes[i].Value,
                                    xmlProp.Attributes[i].NamespaceUri );
                                }
                            } // for loop ends
                        } // attribute nodes created

                        // add id attribute in case it do not exists and it is required
                        if (currentXmlNode.Attributes.GetNamedItem("id") == null && IsIDRequired(currentXmlNode.LocalName))
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, currentXmlNode, "id", GetNextID(ID_DTBPrefix));
                        }
                        // add text from text property

                        string txt = n.GetTextMedia() != null ? n.GetTextMedia().Text : null;
                        if (txt != null)
                        {
                            XmlNode textNode = DTBookDocument.CreateTextNode(txt);
                            currentXmlNode.AppendChild(textNode);

                            Debug.Assert(n.Children.Count == 0);
                        }

                        // add current node to its parent
                        m_TreeNode_XmlNodeMap[n.Parent].AppendChild(currentXmlNode);

                        // add nodes to dictionary 
                        m_TreeNode_XmlNodeMap.Add(n, currentXmlNode);

                        // if current xmlnode is referencing node, add its referencing attribute to referencingAttributesList
                        AddReferencingNodeToReferencedAttributesList(currentXmlNode, referencingAttributesList);

                        // if QName is img and img src is on disk, copy it to output dir
                        if (currentXmlNode.LocalName == "img")
                        {
                            XmlAttribute imgSrcAttribute = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("src");
                            if (imgSrcAttribute != null &&
                                n.GetImageMedia () != null
                                && n.GetImageMedia () is media.data.image.ManagedImageMedia)
                                {
                                media.data.image.ManagedImageMedia managedImage = (media.data.image.ManagedImageMedia )  n.GetImageMedia () ;
                                string exportImageName = managedImage.ImageMediaData.OriginalRelativePath.Replace ( "\\", "_" );
                                string destPath = Path.Combine ( m_OutputDirectory, exportImageName );

                                if ( !File.Exists ( destPath ))
                                    {
                                    FileStream fs = null;
                                    Stream imageStream = null;
                                    try
                                        {
                                        fs = File.Create ( destPath );
                                        imageStream = managedImage.ImageMediaData.OpenInputStream ();
                                        copyStreamData ( imageStream, fs );

                                        }
                                    finally
                                        {
                                        if (fs != null) fs.Close ();
                                        if (imageStream != null) imageStream.Close ();
                                        }
                                    }

                                imgSrcAttribute.Value = exportImageName;

                                if (!m_FilesList_Image.Contains ( exportImageName ))
                                    {
                                    m_FilesList_Image.Add ( exportImageName );
                                    }
                                
                                }
                            /*
                            if (imgSrcAttribute != null &&
                                (imgSrcAttribute.Value.StartsWith(m_Presentation.DataProviderManager.DataFileDirectory)))
                            {
                                string imgFileName = Path.GetFileName(imgSrcAttribute.Value);
                                string sourcePath = Path.Combine(m_Presentation.DataProviderManager.DataFileDirectoryFullPath,
                                    Uri.UnescapeDataString(imgFileName));
                                string destPath = Path.Combine(m_OutputDirectory, Uri.UnescapeDataString(imgFileName));
                                if (File.Exists(sourcePath))
                                {
                                    if (!File.Exists(destPath)) File.Copy(sourcePath, destPath);
                                    imgSrcAttribute.Value = imgFileName;

                                    if (!m_FilesList_Image.Contains(imgFileName))
                                        m_FilesList_Image.Add(imgFileName);
                                }
                                else
                                    System.Diagnostics.Debug.Fail("source image not found", sourcePath);
                            }
                             */ 
                        }

                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });

            // set references to new ids
            foreach (XmlAttribute attr in referencingAttributesList)
            {
                string strIDToFind = attr.Value;
                if (strIDToFind.Contains("#"))
                {
                    strIDToFind = strIDToFind.Split('#')[1];
                }
                if (old_New_IDMap.ContainsKey(strIDToFind))
                {
                    string id_New = old_New_IDMap[strIDToFind];

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
                attributesList.Add((XmlAttribute)node.Attributes.GetNamedItem("idref"));
                return true;
            }
            else if (nodeLocalName == "a")
            {
                attributesList.Add((XmlAttribute)node.Attributes.GetNamedItem("href"));
                return true;
            }
            return false;
        }

        private void ExportStyleSheets ( List<ExternalFileData> list_ExternalStyleSheets )
            {
            if (list_ExternalStyleSheets == null || 
                (list_ExternalStyleSheets != null && list_ExternalStyleSheets.Count == 0))
                {
                return;
                }

            foreach (ExternalFileData efd in list_ExternalStyleSheets)
                {
                if (efd.IsPreservedForOutputFile && !m_FilesList_ExternalFiles.Contains ( efd.OriginalRelativePath ))
                    {
                    string filePath = Path.Combine ( m_OutputDirectory, efd.OriginalRelativePath );
                    FileStream newFileStream = File.Create ( filePath );
                    Stream efdStream = efd.OpenInputStream ();
                    try
                        {
                        copyStreamData ( efdStream, newFileStream );
                        m_FilesList_ExternalFiles.Add ( efd.OriginalRelativePath );

                        }
                    finally
                        {
                        newFileStream.Close ();
                        efdStream.Close ();
                        newFileStream = null;
                        efdStream = null;
                        }
                    }
                }
            }


        private void copyStreamData ( Stream source, Stream dest )
            { //1
            int BUFFER_SIZE = 1024* 1024 ;

            if (source.Length <= BUFFER_SIZE)
                { // 2
                byte[] buffer = new byte[source.Length];
                int read = source.Read ( buffer, 0, (int)source.Length );
                dest.Write ( buffer, 0, read );
                } //-2
            else
                { // 2
                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesRead = 0;
                while ((bytesRead = source.Read ( buffer, 0, BUFFER_SIZE )) > 0)
                    { //3
                    dest.Write ( buffer, 0, bytesRead );
                    } // -3

                } //-2
            } // -1

        //private bool ShouldCreateNextSmilFile(urakawa.core.TreeNode node)
        //{
        //    QualifiedName qName = node.GetXmlElementQName();
        //    return qName != null && qName.LocalName == "level1";
        //}


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
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipBegin", externalMedia.ClipBegin.TimeAsTimeSpan.ToString () );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipEnd", externalMedia.ClipEnd.TimeAsTimeSpan.ToString () );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "src", Path.GetFileName ( externalMedia.Src ) );
            parNode.AppendChild ( audioNode );

            }
        */
    }
}
