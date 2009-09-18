using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;

namespace DaisyExport
{
    public partial class DAISY3Export
    {
        //DAISY3Export_Ncx


        private uint m_SmilFileNameCounter;

        // to do create IDs
        private void CreateNcxAndSmilDocuments()
        {
            XmlDocument ncxDocument = CreateStub_NcxDocument();

            XmlNode ncxRootNode = getFirstChildElementsWithName(ncxDocument, true, "ncx", null); //ncxDocument.GetElementsByTagName("ncx")[0];
            XmlNode navMapNode = getFirstChildElementsWithName(ncxDocument, true, "navMap", null); //ncxDocument.GetElementsByTagName("navMap")[0];
            Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();
            m_FilesList_Smil = new List<string>();
            m_FilesList_Audio = new List<string>();
            m_SmilFileNameCounter = 0;
                        List<XmlNode> playOrderList_Sorted = new List<XmlNode> ();
            int totalPageCount = 0;
            int maxNormalPageNumber = 0;
            int maxDepth = 1;
            TimeSpan smilElapseTime = new TimeSpan();

            foreach (urakawa.core.TreeNode urakawaNode in m_ListOfLevels)
            {
                bool IsNcxNativeNodeAdded = false;
                XmlDocument smilDocument = null;
                string smilFileName = null;
                XmlNode navPointNode = null;
                urakawa.core.TreeNode currentHeadingTreeNode = null;
                TimeSpan durationOfCurrentSmil = new TimeSpan();
                List<urakawa.core.TreeNode> textAudioNodesList = new List<urakawa.core.TreeNode>();

                urakawaNode.AcceptDepthFirst(
            delegate(urakawa.core.TreeNode n)
            {
                QualifiedName currentQName = n.GetXmlElementQName();

                if (currentQName != null &&
                    (currentQName.LocalName == "hd" || currentQName.LocalName == "h1" || currentQName.LocalName == "h2" || currentQName.LocalName == "h3" || currentQName.LocalName == "h4"
                    || currentQName.LocalName == "h5" || currentQName.LocalName == "h6"))
                {
                    currentHeadingTreeNode = n;
                }

                if (currentQName != null &&
                        currentQName.LocalName != urakawaNode.GetXmlElementQName().LocalName && currentQName.LocalName.StartsWith("level"))
                    return false;

                urakawa.media.AbstractTextMedia txtMedia = n.GetTextMedia();
                urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

                if (txtMedia != null && externalAudio != null)
                {
                    textAudioNodesList.Add(n);
                    return true;
                }
                return true;
            },
                    delegate(urakawa.core.TreeNode n) { });


                QualifiedName qName = urakawaNode.GetXmlElementQName();
                bool isDoctitleOrDocAuthor = (qName != null &&
                        (qName.LocalName == "doctitle" || qName.LocalName == "docauthor"));

                if (textAudioNodesList.Count > 0 && (currentHeadingTreeNode != null || isDoctitleOrDocAuthor))
                {
                    // carry on processing following lines. and in case this is not true, skip all the following lines
                }
                else
                {
                    continue;
                }

                //caching playorder for navPoints because page numbers are added first in code below.
            int navPointPlayOrderListIndex = playOrderList_Sorted.Count ;
                

                // create smil stub document
                smilDocument = CreateStub_SmilDocument();
                smilFileName = GetNextSmilFileName;
                bool IsPageAdded = false;

                // create smil nodes

                string firstPar_id = null;
                foreach (urakawa.core.TreeNode n in textAudioNodesList)
                {
                    urakawa.media.AbstractTextMedia txtMedia = n.GetTextMedia();
                    urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);
                    string par_id = null;

                    
                        XmlNode mainSeq = getFirstChildElementsWithName(smilDocument, true, "body", null).FirstChild; //smilDocument.GetElementsByTagName("body")[0].FirstChild;
                        CommonFunctions.CreateAppendXmlAttribute(smilDocument, mainSeq, "id", GetNextID(ID_SmilPrefix));
                        XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);
                        par_id = GetNextID(ID_SmilPrefix);
                        if (n == textAudioNodesList[0]) firstPar_id = par_id;
                        CommonFunctions.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);
                        mainSeq.AppendChild(parNode);

                        XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, SmilTextNode, "id", GetNextID ( ID_SmilPrefix ) );
                        string dtbookID = m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value;
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, SmilTextNode, "src", m_Filename_Content + "#" + dtbookID );
                        parNode.AppendChild ( SmilTextNode );

                        XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
                        CommonFunctions.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString());
                        CommonFunctions.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString());
                        CommonFunctions.CreateAppendXmlAttribute(smilDocument, audioNode, "src", Path.GetFileName(externalAudio.Src));
                        parNode.AppendChild(audioNode);

                    
                        // add audio file name in audio files list for use in opf creation
                        string audioFileName = Path.GetFileName(externalAudio.Src);
                        if (!m_FilesList_Audio.Contains(audioFileName)) m_FilesList_Audio.Add(audioFileName);

                        // add to duration
                        durationOfCurrentSmil = durationOfCurrentSmil.Add(externalAudio.Duration.TimeDeltaAsTimeSpan);
                    

                    // if node n is pagenum, add to pageList
                    if (n.GetXmlElementQName() != null
                        && n.GetXmlElementQName().LocalName == "pagenum")
                    {
                        // add custom test to par node
                    CommonFunctions.CreateAppendXmlAttribute ( smilDocument, parNode, "customTest", "pagenum" );
                    IsPageAdded = true;

                        XmlNode pageListNode = getFirstChildElementsWithName(ncxDocument, true, "pageList", null);
                        if (pageListNode == null)
                        {
                            pageListNode = ncxDocument.CreateElement(null, "pageList", ncxRootNode.NamespaceURI);
                            ncxRootNode.AppendChild(pageListNode);
                        }

                        //XmlNode pageListNode = null;
                        //XmlNodeList listOfPages = ncxDocument.GetElementsByTagName("pageList");
                        //if (listOfPages == null || listOfPages.Count == 0)
                        //{
                        //    pageListNode = ncxDocument.CreateElement(null, "pageList", ncxRootNode.NamespaceURI);
                        //    ncxRootNode.AppendChild(pageListNode);
                        //}
                        //else
                        //{
                        //    pageListNode = listOfPages[0];
                        //}


                        XmlNode pageTargetNode = ncxDocument.CreateElement(null, "pageTarget", pageListNode.NamespaceURI);
                        pageListNode.AppendChild(pageTargetNode);

                        CommonFunctions.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "id", GetNextID(ID_NcxPrefix));
                        CommonFunctions.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "class", "pagenum");
                        CommonFunctions.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "playOrder", "");
                        string strTypeVal = n.GetXmlProperty().GetAttribute("page").Value;
                        CommonFunctions.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "type", strTypeVal);
                        string strPageValue = n.GetTextMediaFlattened();
                        ++totalPageCount;

                        // adding node to play order node list
                        playOrderList_Sorted.Add ( pageTargetNode );

                        if (strTypeVal == "normal")
                        {
                            int tmp = int.Parse(strPageValue);
                            if (maxNormalPageNumber < tmp) maxNormalPageNumber = tmp;
                        }
                        if (strTypeVal != "special")
                        {
                            CommonFunctions.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "value", strPageValue);
                        }

                        XmlNode navLabelNode = ncxDocument.CreateElement(null, "navLabel", pageListNode.NamespaceURI);
                        pageTargetNode.AppendChild(navLabelNode);

                        XmlNode txtNode = ncxDocument.CreateElement(null, "text", pageListNode.NamespaceURI);
                        navLabelNode.AppendChild(txtNode);
                        txtNode.AppendChild(
                            ncxDocument.CreateTextNode(strPageValue));

                        XmlNode audioNodeNcx = ncxDocument.CreateElement(null, "audio", pageListNode.NamespaceURI);
                        navLabelNode.AppendChild ( audioNodeNcx );
                        CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString () );
                        CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString () );
                        CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "src", Path.GetFileName ( externalAudio.Src ) );

                        XmlNode contentNode = ncxDocument.CreateElement(null, "content", pageListNode.NamespaceURI);
                        pageTargetNode.AppendChild(contentNode);
                        CommonFunctions.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + par_id);

                        // add reference to par in dtbook document
                        string strBtbookID = SmilTextNode.Attributes.GetNamedItem ( "src" ).Value.Split ( '#' )[1];
                        XmlNodeList dtbookNodesList = m_DTBDocument.GetElementsByTagName ( "pagenum" );
                        foreach (XmlNode p in dtbookNodesList)
                            {
                            if (p.Attributes.GetNamedItem ( "id" ).Value == strBtbookID)
                                {
                                CommonFunctions.CreateAppendXmlAttribute ( m_DTBDocument, p, "smilref", smilFileName + "#" + par_id );
                                                                                                }
                            }
                    }
                }// foreach for tree nodes n ends


                if (isDoctitleOrDocAuthor)
                {
                    urakawa.core.TreeNode n = textAudioNodesList[0];
                    urakawa.media.AbstractTextMedia txtMedia = n.GetTextMedia();
                    urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

                    XmlNode docNode = ncxDocument.CreateElement(null,
                        qName.LocalName == "doctitle" ? "docTitle" : "docAuthor",
                         ncxRootNode.NamespaceURI);

                    ncxRootNode.InsertBefore(docNode, navMapNode);

                    XmlNode docTxtNode = ncxDocument.CreateElement(null, "text", docNode.NamespaceURI);
                    docNode.AppendChild(docTxtNode);
                    docTxtNode.AppendChild(
                    ncxDocument.CreateTextNode(txtMedia.Text));

                    // create audio node
                    XmlNode docAudioNode = ncxDocument.CreateElement(null, "audio", docNode.NamespaceURI);
                    docNode.AppendChild(docAudioNode);
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString());
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString());
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "src", Path.GetFileName(externalAudio.Src));
                }
                else
                {
                    urakawa.core.TreeNode n = textAudioNodesList[0];
                    urakawa.media.AbstractTextMedia txtMedia = n.GetTextMedia();
                    urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

                    // first create navPoints
                    navPointNode = ncxDocument.CreateElement(null, "navPoint", navMapNode.NamespaceURI);
                    if (currentHeadingTreeNode != null) CommonFunctions.CreateAppendXmlAttribute(ncxDocument, navPointNode, "class", currentHeadingTreeNode.GetProperty<urakawa.property.xml.XmlProperty>().LocalName);
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, navPointNode, "id", GetNextID(ID_NcxPrefix));
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, navPointNode, "playOrder","" );

                    // insert node to play order list at cached index
                    playOrderList_Sorted.Insert (navPointPlayOrderListIndex, navPointNode );

                    urakawa.core.TreeNode parentNode = GetParentLevelNode(urakawaNode);

                    if (parentNode == null)
                    {
                        navMapNode.AppendChild(navPointNode);
                    }
                    else if (treeNode_NavNodeMap.ContainsKey(parentNode))
                    {
                        treeNode_NavNodeMap[parentNode].AppendChild(navPointNode);
                    }
                    else // surch up for node
                    {
                        int counter = 0;
                        while (parentNode != null && counter <= 6)
                        {
                            parentNode = GetParentLevelNode(parentNode);
                            if (parentNode != null && treeNode_NavNodeMap.ContainsKey(parentNode))
                            {
                                treeNode_NavNodeMap[parentNode].AppendChild(navPointNode);
                                break;
                            }
                            counter++;
                        }

                        if (parentNode == null || counter > 7)
                        {
                            navMapNode.AppendChild(navPointNode);
                        }
                    }


                    treeNode_NavNodeMap.Add(urakawaNode, navPointNode);

                    // create navLabel
                    XmlNode navLabel = ncxDocument.CreateElement(null, "navLabel", navPointNode.NamespaceURI);
                    navPointNode.AppendChild(navLabel);

                    // create text node
                    XmlNode txtNode = ncxDocument.CreateElement(null, "text", navMapNode.NamespaceURI);
                    navLabel.AppendChild(txtNode);
                    if (currentHeadingTreeNode != null)
                        txtNode.AppendChild(
                        ncxDocument.CreateTextNode(txtMedia.Text));

                    // create audio node
                    XmlNode audioNode = ncxDocument.CreateElement(null, "audio", navMapNode.NamespaceURI);
                    navLabel.AppendChild(audioNode);
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString());
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString());
                    CommonFunctions.CreateAppendXmlAttribute(ncxDocument, audioNode, "src", Path.GetFileName(externalAudio.Src));

                    // add content node
                    if (firstPar_id != null)
                    {
                        XmlNode contentNode = ncxDocument.CreateElement(null, "content", navMapNode.NamespaceURI);
                        navPointNode.AppendChild(contentNode);
                        CommonFunctions.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + firstPar_id);
                    }
                    int navPointDepth = GetDepthOfNavPointNode(ncxDocument, navPointNode);
                    if (maxDepth < navPointDepth) maxDepth = navPointDepth;
                }


                // add metadata to smil document and write to file.
                if (smilDocument != null)
                {
                    // update duration in seq node
                    XmlNode mainSeqNode = getFirstChildElementsWithName(smilDocument, true, "body", null).FirstChild; //smilDocument.GetElementsByTagName("body")[0].FirstChild;
                    CommonFunctions.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "dur", durationOfCurrentSmil.ToString());
                    CommonFunctions.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "fill", "remove");
                    AddMetadata_Smil(smilDocument, smilElapseTime.ToString(), IsPageAdded);

                    CommonFunctions.WriteXmlDocumentToFile(smilDocument,
                        Path.Combine(m_OutputDirectory, smilFileName));

                    smilElapseTime = smilElapseTime.Add(durationOfCurrentSmil);
                    m_FilesList_Smil.Add(smilFileName);
                }

            }
            // assign play orders
        Dictionary<string, string> playOrder_ReferenceMap = new Dictionary<string, string> ();
        int playOrderCounter = 1;
        foreach (XmlNode xn in playOrderList_Sorted)
            {
            XmlNode referedContentNode = getFirstChildElementsWithName ( xn, false, "content", xn.NamespaceURI );
            string contentNode_Src = referedContentNode.Attributes.GetNamedItem("src").Value ;

            if (playOrder_ReferenceMap.ContainsKey ( contentNode_Src ))
                {
                xn.Attributes.GetNamedItem ( "playOrder" ).Value = playOrder_ReferenceMap[contentNode_Src];
                }
            else
                {
                xn.Attributes.GetNamedItem ( "playOrder" ).Value = playOrderCounter.ToString ();
                playOrder_ReferenceMap.Add ( contentNode_Src, playOrderCounter.ToString () );
                ++playOrderCounter; 
                }

            }

        CommonFunctions.WriteXmlDocumentToFile(m_DTBDocument,
          Path.Combine(m_OutputDirectory, m_Filename_Content));

            // write ncs document to file
            m_TotalTime = smilElapseTime;
            AddMetadata_Ncx(ncxDocument, totalPageCount.ToString(), maxNormalPageNumber.ToString(), maxDepth.ToString());
            CommonFunctions.WriteXmlDocumentToFile(ncxDocument,
                Path.Combine(m_OutputDirectory, m_Filename_Ncx));
        }

        private urakawa.core.TreeNode GetParentLevelNode(urakawa.core.TreeNode node)
        {
            urakawa.core.TreeNode parentNode = node.Parent;
            QualifiedName qName = parentNode.GetXmlElementQName();

            while (qName == null ||
                (qName != null && !qName.LocalName.StartsWith("level")))
            {
                if (qName != null && qName.LocalName == "book")
                    return null;

                parentNode = parentNode.Parent;
                qName = parentNode.GetXmlElementQName();
            }
            return parentNode;
        }

        private int GetDepthOfNavPointNode(XmlDocument doc, XmlNode navPointNode)
        {
            XmlNode parent = navPointNode.ParentNode;

            int i = 1;
            for (i = 1; i <= 9; i++)
            {

                if (parent != null && parent.LocalName == "navMap")
                {
                    return i;
                }
                parent = parent.ParentNode;
            }
            return i;
        }

        private Metadata AddMetadata_DtbUid(bool asInnerText, XmlDocument doc, XmlNode parentNode)
        {
            Metadata mdUid = null;
            XmlNode metaNodeUid = null;
            foreach (Metadata md in m_Presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (!isUniqueIdName(md.NameContentAttribute.Name)) continue;

                foreach (MetadataAttribute mda in md.OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    if (mda.Name != "id") continue;

                    //AddMetadataAsAttributes(ncxDocument, headNode, "dtb:uid", md.NameContentAttribute.Value);

                    if (asInnerText)
                    {
                        metaNodeUid = AddMetadataAsInnerText(doc, parentNode, "dc:Identifier", md.NameContentAttribute.Value);
                        CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                    }
                    else
                    {
                        metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                        parentNode.AppendChild(metaNodeUid);

                        CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, "name", "dtb:uid");
                        CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, "content", md.NameContentAttribute.Value);
                    }
                    
                    mdUid = md;
                    break;
                }

                if (mdUid != null)
                {
                    // add metadata optional attributes if any
                    foreach (MetadataAttribute ma in md.OtherAttributes.ContentsAs_YieldEnumerable)
                    {
                        if (ma.Name == "id") continue;

                        CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, ma.Name, ma.Value);
                    }

                    return mdUid;
                }
            }

            foreach (Metadata md in m_Presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (!isUniqueIdName(md.NameContentAttribute.Name)) continue;

                //AddMetadataAsAttributes(ncxDocument, headNode, "dtb:uid", md.NameContentAttribute.Value);
                if (asInnerText)
                {
                    metaNodeUid = AddMetadataAsInnerText(doc, parentNode, "dc:Identifier", md.NameContentAttribute.Value);
                    CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                }
                else
                {
                    metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                    parentNode.AppendChild(metaNodeUid);

                    CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, "name", "dtb:uid");
                    CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, "content",
                                                             md.NameContentAttribute.Value);
                }

                // add metadata optional attributes if any
                foreach (MetadataAttribute ma in md.OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    if (ma.Name == "id") continue;

                    CommonFunctions.CreateAppendXmlAttribute(doc, metaNodeUid, ma.Name, ma.Value);
                }
                return md;
            }

            return null;
        }

        private static bool isUniqueIdName(string name)
        {
            string lower = name.ToLower();

            if ("dc:identifier" == lower)
            {
                return true;
            }

            MetadataDefinition md = SupportedMetadata_Z39862005.GetMetadataDefinition("dc:Identifier");
            return md != null && md.Synonyms.Find(
                                delegate(string s)
                                {
                                    return s.ToLower() == lower;
                                }) != null;
        }

        private void AddMetadata_Ncx(XmlDocument ncxDocument, string strTotalPages, string strMaxNormalPage, string strDepth)
        {
            XmlNode headNode = getFirstChildElementsWithName(ncxDocument, true, "head", null); //ncxDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, ncxDocument, headNode);

            AddMetadata_Generator(ncxDocument, headNode);

            AddMetadataAsAttributes(ncxDocument, headNode, "dtb:depth", strDepth);
            AddMetadataAsAttributes(ncxDocument, headNode, "dtb:totalPageCount", strTotalPages);
            AddMetadataAsAttributes(ncxDocument, headNode, "dtb:maxPageNumber", strMaxNormalPage);


            // add custom test to headNode
                        XmlNode customTestNode = ncxDocument.CreateElement ( null, "smilCustomTest", headNode.NamespaceURI );
            headNode.AppendChild ( customTestNode );
            CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, customTestNode, "bookStruct", "PAGE_NUMBER" );
            CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, customTestNode, "defaultState", "false" );
            CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, customTestNode, "id", "pagenum");
            CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, customTestNode, "override", "visible" );

        }

        private void AddMetadata_Smil ( XmlDocument smilDocument, string strElapsedTime, bool isCustomTestRequired )
        {
            XmlNode headNode = getFirstChildElementsWithName(smilDocument, true, "head", null); //smilDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, smilDocument, headNode);

            AddMetadata_Generator(smilDocument, headNode);

            AddMetadataAsAttributes(smilDocument, headNode, "dtb:totalElapsedTime", strElapsedTime);


            if (isCustomTestRequired)
                {
                XmlNode customAttributesNode = smilDocument.CreateElement ( null, "customAttributes", headNode.NamespaceURI );
                headNode.AppendChild ( customAttributesNode );
                XmlNode customTestNode = smilDocument.CreateElement ( null, "customTest", headNode.NamespaceURI );
                customAttributesNode.AppendChild ( customTestNode );
                CommonFunctions.CreateAppendXmlAttribute ( smilDocument, customTestNode, "defaultState", "false" );
                CommonFunctions.CreateAppendXmlAttribute ( smilDocument, customTestNode, "id", "pagenum" );
                CommonFunctions.CreateAppendXmlAttribute ( smilDocument, customTestNode, "override", "visible" );
                }
        }

        private XmlNode AddMetadataAsAttributes(XmlDocument doc, XmlNode headNode, string name, string content)
        {
            XmlNode metaNode = doc.CreateElement(null, "meta", headNode.NamespaceURI);
            headNode.AppendChild(metaNode);
            CommonFunctions.CreateAppendXmlAttribute(doc, metaNode, "name", name);
            CommonFunctions.CreateAppendXmlAttribute(doc, metaNode, "content", content);

            return metaNode;
        }



        private string GetNextSmilFileName
        {
            get
            {
                string strNumericFrag = (++m_SmilFileNameCounter).ToString();
                return strNumericFrag.PadLeft(4, '0') + ".smil";
            }
        }


        private XmlDocument CreateStub_NcxDocument()
        {
            XmlDocument NcxDocument = new XmlDocument();
            NcxDocument.XmlResolver = null;

            NcxDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            NcxDocument.AppendChild(NcxDocument.CreateDocumentType("ncx",
                "-//NISO//DTD ncx 2005-1//EN",
                "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd",
                null));

            XmlNode rootNode = NcxDocument.CreateElement(null,
                "ncx",
                "http://www.daisy.org/z3986/2005/ncx/");

            NcxDocument.AppendChild(rootNode);


            CommonFunctions.CreateAppendXmlAttribute(NcxDocument, rootNode, "version", "2005-1");
            CommonFunctions.CreateAppendXmlAttribute(NcxDocument, rootNode, "xml:lang", (string.IsNullOrEmpty(m_Presentation.Language) ? "en-US" : m_Presentation.Language));


            XmlNode headNode = NcxDocument.CreateElement(null, "head", rootNode.NamespaceURI);
            rootNode.AppendChild(headNode);

            XmlNode navMapNode = NcxDocument.CreateElement(null, "navMap", rootNode.NamespaceURI);
            rootNode.AppendChild(navMapNode);

            return NcxDocument;
        }

        private XmlDocument CreateStub_SmilDocument()
        {
            XmlDocument smilDocument = new XmlDocument();
            smilDocument.XmlResolver = null;

            smilDocument.AppendChild(smilDocument.CreateXmlDeclaration("1.0", "utf-8", null));
            smilDocument.AppendChild(smilDocument.CreateDocumentType("smil",
                "-//NISO//DTD dtbsmil 2005-2//EN",
                    "http://www.daisy.org/z3986/2005/dtbsmil-2005-2.dtd",
                null));
            XmlNode smilRootNode = smilDocument.CreateElement(null,
                "smil", "http://www.w3.org/2001/SMIL20/");

            //"http://www.w3.org/1999/xhtml" );
            smilDocument.AppendChild(smilRootNode);

            XmlNode headNode = smilDocument.CreateElement(null, "head", smilRootNode.NamespaceURI);
            smilRootNode.AppendChild(headNode);
            XmlNode bodyNode = smilDocument.CreateElement(null, "body", smilRootNode.NamespaceURI);
            smilRootNode.AppendChild(bodyNode);
            XmlNode mainSeqNode = smilDocument.CreateElement(null, "seq", smilRootNode.NamespaceURI);
            bodyNode.AppendChild(mainSeqNode);

            return smilDocument;
        }
    }
}
