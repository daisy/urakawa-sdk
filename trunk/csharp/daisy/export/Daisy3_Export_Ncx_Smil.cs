using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        //DAISY3Export_Ncx


        private uint m_SmilFileNameCounter;

        // to do create IDs
        private void CreateNcxAndSmilDocuments()
        {
            XmlDocument ncxDocument = CreateStub_NcxDocument();

            XmlNode ncxRootNode = XmlDocumentHelper.GetFirstChildElementWithName(ncxDocument, true, "ncx", null); //ncxDocument.GetElementsByTagName("ncx")[0];
            XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementWithName(ncxDocument, true, "navMap", null); //ncxDocument.GetElementsByTagName("navMap")[0];
            Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();
            m_FilesList_Smil = new List<string>();
            m_FilesList_Audio = new List<string>();
            m_SmilFileNameCounter = 0;
            List<XmlNode> playOrderList_Sorted = new List<XmlNode>();
            int totalPageCount = 0;
            int maxNormalPageNumber = 0;
            int maxDepth = 1;
            TimeSpan smilElapseTime = new TimeSpan();
            List<string> ncxCustomTestList = new List<string>();
            List<urakawa.core.TreeNode> specialParentNodesAddedToNavList = new List<urakawa.core.TreeNode>();

            foreach (urakawa.core.TreeNode urakawaNode in m_ListOfLevels)
            {
                bool IsNcxNativeNodeAdded = false;
                XmlDocument smilDocument = null;
                string smilFileName = null;
                XmlNode navPointNode = null;
                urakawa.core.TreeNode currentHeadingTreeNode = null;
                urakawa.core.TreeNode special_UrakawaNode = null;
                TimeSpan durationOfCurrentSmil = new TimeSpan();
                XmlNode mainSeq = null;
                XmlNode Seq_SpecialNode = null;
                //bool IsPageAdded = false;
                string firstPar_id = null;
                bool shouldAddNewSeq = false;
                string par_id = null;
                List<string> currentSmilCustomTestList = new List<string>();
                Stack<urakawa.core.TreeNode> specialParentNodeStack = new Stack<urakawa.core.TreeNode>();
                Stack<XmlNode> specialSeqNodeStack = new Stack<XmlNode>();

                urakawaNode.AcceptDepthFirst(
            delegate(urakawa.core.TreeNode n)
            {
                QualifiedName currentQName = n.GetXmlElementQName();

                if (IsHeadingNode(n))
                {
                    currentHeadingTreeNode = n;
                }

                if (currentQName != null &&
                        currentQName.LocalName != urakawaNode.GetXmlElementQName().LocalName
                        && doesTreeNodeTriggerNewSmil(n))
                {
                    return false;
                }

                if ((IsHeadingNode(n) || IsEscapableNode(n) || IsSkippableNode(n))
                    && (special_UrakawaNode != n))
                {
                    // if this candidate special node is child of existing special node then ad existing special node to stack for nesting.
                    if (special_UrakawaNode != null && Seq_SpecialNode != null
                        && n.IsDescendantOf(special_UrakawaNode))
                    {
                        specialParentNodeStack.Push(special_UrakawaNode);
                        specialSeqNodeStack.Push(Seq_SpecialNode);
                    }
                    special_UrakawaNode = n;
                    shouldAddNewSeq = true;
                }

                urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);
                /*
                if (externalAudio == null)
                    {
                    return true;
                    }
                */

                QualifiedName qName1 = currentHeadingTreeNode != null ? currentHeadingTreeNode.GetXmlElementQName() : null;
                bool isDoctitle_1 = (qName1 != null && qName1.LocalName == "doctitle");

                if (!IsNcxNativeNodeAdded && currentHeadingTreeNode != null && (currentHeadingTreeNode.GetDurationOfManagedAudioMediaFlattened() == null || currentHeadingTreeNode.GetDurationOfManagedAudioMediaFlattened().TimeDeltaAsMillisecondLong == 0))
                {
                    if (isDoctitle_1)
                    {
                        //urakawa.core.TreeNode n = textAudioNodesList[0];
                        CreateDocTitle(ncxDocument, ncxRootNode, n);
                        IsNcxNativeNodeAdded = true;
                    }
                }



                urakawa.media.timing.TimeDelta urakawaNodeDur = urakawaNode.GetDurationOfManagedAudioMediaFlattened();
                if (currentHeadingTreeNode == null && urakawaNodeDur != null && urakawaNodeDur.TimeDeltaAsMillisecondLong == 0)
                {
                    return true;
                    // carry on processing following lines. and in case this is not true, skip all the following lines
                }


                QualifiedName qName = currentHeadingTreeNode != null ? currentHeadingTreeNode.GetXmlElementQName() : null;
                bool isDoctitle_ = (qName != null && qName.LocalName == "doctitle");

                // create smil stub document
                if (smilDocument == null)
                {
                    smilDocument = CreateStub_SmilDocument();
                    mainSeq = XmlDocumentHelper.GetFirstChildElementWithName(smilDocument, true, "body", null).FirstChild;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeq, "id", GetNextID(ID_SmilPrefix));
                    smilFileName = GetNextSmilFileName;
                }


                // create smil nodes

                if (shouldAddNewSeq)
                {
                    if (Seq_SpecialNode != null)
                    {
                        if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                        {
                            Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                        }
                        Seq_SpecialNode = null;
                    }

                    Seq_SpecialNode = smilDocument.CreateElement(null, "seq", mainSeq.NamespaceURI);
                    string strSeqID = "";
                    // specific handling of IDs for notes for allowing predetermined refered IDs
                    if (special_UrakawaNode.GetXmlElementQName().LocalName == "note" || special_UrakawaNode.GetXmlElementQName().LocalName == "annotation")
                    {
                        strSeqID = ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value;
                    }
                    else
                    {
                        strSeqID = GetNextID(ID_SmilPrefix);
                    }
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "id", strSeqID);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", special_UrakawaNode.GetXmlElementQName().LocalName);

                    if (IsEscapableNode(special_UrakawaNode))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "end", "");
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "fill", "remove");
                    }

                    if (IsSkippableNode(special_UrakawaNode))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", special_UrakawaNode.GetXmlElementQName().LocalName);

                        if (special_UrakawaNode.GetXmlElementQName().LocalName == "noteref" || special_UrakawaNode.GetXmlElementQName().LocalName == "annoref")
                        {
                            XmlNode anchorNode = smilDocument.CreateElement(null, "a", Seq_SpecialNode.NamespaceURI);
                            Seq_SpecialNode.AppendChild(anchorNode);
                            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "external", "false");
                            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href", "#" + ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("idref").Value.Replace("#", ""));
                        }
                        if (!currentSmilCustomTestList.Contains(special_UrakawaNode.GetXmlElementQName().LocalName))
                        {
                            currentSmilCustomTestList.Add(special_UrakawaNode.GetXmlElementQName().LocalName);
                        }
                    }

                    // add smilref reference to seq_special  in dtbook document
                    if (IsEscapableNode(special_UrakawaNode) || IsSkippableNode(special_UrakawaNode))
                    {
                        XmlNode dtbEscapableNode = m_TreeNode_XmlNodeMap[special_UrakawaNode];
                        XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtbEscapableNode, "smilref", smilFileName + "#" + strSeqID);
                    }

                    // decide the parent node to which this new seq node is to be appended.
                    if (specialSeqNodeStack.Count == 0)
                    {
                        mainSeq.AppendChild(Seq_SpecialNode);
                    }
                    else
                    {
                        specialSeqNodeStack.Peek().AppendChild(Seq_SpecialNode);
                    }

                    shouldAddNewSeq = false;
                }

                if (externalAudio != null ||
                    (n.GetTextMedia() != null
                    && special_UrakawaNode != null && (IsEscapableNode(special_UrakawaNode) || IsSkippableNode(special_UrakawaNode) ||  (special_UrakawaNode.GetXmlProperty() != null && special_UrakawaNode.GetXmlProperty().LocalName.ToLower() == "doctitle"))
                    && m_TreeNode_XmlNodeMap[n].Attributes != null))
                {
                    // continue ahead 
                }
                else
                {
                    return true;
                }

                XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);

                // decide the parent node for this new par node.
                // if node n is child of current specialParentNode than append to it 
                //else check if it has to be appended to parent of this special node in stack or to main seq.
                if (special_UrakawaNode != null && (special_UrakawaNode == n || n.IsDescendantOf(special_UrakawaNode)))
                {
                    Seq_SpecialNode.AppendChild(parNode);
                }
                else
                {
                    bool IsParNodeAppended = false;
                    string strReferedID = par_id;
                    if (specialParentNodeStack.Count > 0)
                    {
                        // check and pop stack till specialParentNode of   iterating node n is found in stack
                        // the loop is also used to assign value of last imidiate seq or par to end attribute of parent seq while pop up
                        while (specialParentNodeStack.Count > 0 && !n.IsDescendantOf(special_UrakawaNode))
                        {
                            if (Seq_SpecialNode != null
                                            &&
                                            strReferedID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                            {
                                Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID + ".end";
                            }
                            strReferedID = Seq_SpecialNode.Attributes.GetNamedItem("id").Value;
                            special_UrakawaNode = specialParentNodeStack.Pop();
                            Seq_SpecialNode = specialSeqNodeStack.Pop();
                        }

                        // if parent of node n is retrieved from stack, apend the par node to it.
                        if (n.IsDescendantOf(special_UrakawaNode))
                        {
                            Seq_SpecialNode.AppendChild(parNode);
                            IsParNodeAppended = true;
                            /*
                            if (Seq_SpecialNode != null
                                &&
                                par_id != null && Seq_SpecialNode.Attributes.GetNamedItem ( "end" ) != null)
                                {
                                Seq_SpecialNode.Attributes.GetNamedItem ( "end" ).Value = "DTBuserEscape;" + par_id + ".end";
                                }
                            */
                        }
                        //System.Windows.Forms.MessageBox.Show ( "par_ id " + par_id + " count " + specialParentNodeStack.Count.ToString ());
                    }// stack > 0 check ends

                    if (specialSeqNodeStack.Count == 0 && !IsParNodeAppended)
                    {
                        mainSeq.AppendChild(parNode);
                        special_UrakawaNode = null;
                        if (Seq_SpecialNode != null)
                        {
                            //if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem ( "end" ) != null)
                            if (strReferedID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                            {
                                //System.Windows.Forms.MessageBox.Show ( par_id == null ? "null" : par_id );
                                //Seq_SpecialNode.Attributes.GetNamedItem ( "end" ).Value = "DTBuserEscape;" + par_id + ".end";
                                Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID + ".end";
                            }
                            Seq_SpecialNode = null;
                        }
                    }// check of append to main seq ends

                }


                par_id = GetNextID(ID_SmilPrefix);
                // check and assign first par ID
                if (firstPar_id == null)
                {
                    if (n.GetXmlElementQName() != null && currentHeadingTreeNode != null
                        && (n.IsDescendantOf(currentHeadingTreeNode) || n == currentHeadingTreeNode))
                    {
                        firstPar_id = par_id;
                    }
                }

                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);


                XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id", GetNextID(ID_SmilPrefix));
                string dtbookID = m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value;
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src", m_Filename_Content + "#" + dtbookID);
                parNode.AppendChild(SmilTextNode);
                if (externalAudio != null)
                {
                    XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString());
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString());
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src", Path.GetFileName(externalAudio.Src));
                    parNode.AppendChild(audioNode);

                    // add audio file name in audio files list for use in opf creation 
                    string audioFileName = Path.GetFileName(externalAudio.Src);
                    if (!m_FilesList_Audio.Contains(audioFileName)) m_FilesList_Audio.Add(audioFileName);

                    // add to duration 
                    durationOfCurrentSmil = durationOfCurrentSmil.Add(externalAudio.Duration.TimeDeltaAsTimeSpan);
                }

                // if node n is pagenum, add to pageList
                if (n.GetXmlElementQName() != null
                    && n.GetXmlElementQName().LocalName == "pagenum")
                {
                    if (!currentSmilCustomTestList.Contains("pagenum"))
                    {
                        currentSmilCustomTestList.Add("pagenum");
                    }

                    XmlNode pageListNode = XmlDocumentHelper.GetFirstChildElementWithName(ncxDocument, true, "pageList", null);
                    if (pageListNode == null)
                    {
                        pageListNode = ncxDocument.CreateElement(null, "pageList", ncxRootNode.NamespaceURI);
                        ncxRootNode.AppendChild(pageListNode);
                    }

                    XmlNode pageTargetNode = ncxDocument.CreateElement(null, "pageTarget", pageListNode.NamespaceURI);
                    pageListNode.AppendChild(pageTargetNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "id", GetNextID(ID_NcxPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "class", "pagenum");
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "playOrder", "");
                    string strTypeVal = n.GetXmlProperty().GetAttribute("page").Value;
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "type", strTypeVal);
                    string strPageValue = n.GetTextMediaFlattened();
                    ++totalPageCount;

                    playOrderList_Sorted.Add(pageTargetNode);

                    if (strTypeVal == "normal")
                    {
                        int tmp = int.Parse(strPageValue);
                        if (maxNormalPageNumber < tmp) maxNormalPageNumber = tmp;
                    }
                    if (strTypeVal != "special")
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "value", strPageValue);
                    }

                    XmlNode navLabelNode = ncxDocument.CreateElement(null, "navLabel", pageListNode.NamespaceURI);
                    pageTargetNode.AppendChild(navLabelNode);

                    XmlNode txtNode = ncxDocument.CreateElement(null, "text", pageListNode.NamespaceURI);
                    navLabelNode.AppendChild(txtNode);
                    txtNode.AppendChild(
                        ncxDocument.CreateTextNode(strPageValue));

                    if (externalAudio != null)
                        {
                        XmlNode audioNodeNcx = ncxDocument.CreateElement ( null, "audio", pageListNode.NamespaceURI );
                        navLabelNode.AppendChild ( audioNodeNcx );
                        XmlDocumentHelper.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString () );
                        XmlDocumentHelper.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString () );
                        XmlDocumentHelper.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "src", Path.GetFileName ( externalAudio.Src ) );
                        }

                    XmlNode contentNode = ncxDocument.CreateElement(null, "content", pageListNode.NamespaceURI);
                    pageTargetNode.AppendChild(contentNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + par_id);

                    // add reference to par in dtbook document
                    /*
                    string strBtbookID = SmilTextNode.Attributes.GetNamedItem ( "src" ).Value.Split ( '#' )[1];
                    XmlNodeList dtbookNodesList = m_DTBDocument.GetElementsByTagName ( "pagenum" );
                    foreach (XmlNode p in dtbookNodesList)
                        {
                        if (p.Attributes.GetNamedItem ( "id" ).Value == strBtbookID)
                            {
                            XmlDocumentHelper.CreateAppendXmlAttribute ( m_DTBDocument, p, "smilref", smilFileName + "#" + par_id );
                            }
                        }
                     */
                }
                else if (special_UrakawaNode != null
                    && m_NavListElementNamesList.Contains(special_UrakawaNode.GetXmlElementQName().LocalName) && !specialParentNodesAddedToNavList.Contains(special_UrakawaNode))
                {
                    string navListNodeName = special_UrakawaNode.GetXmlElementQName().LocalName;
                    specialParentNodesAddedToNavList.Add(special_UrakawaNode);
                    XmlNode navListNode = null;

                    //= getFirstChildElementsWithName ( ncxDocument, true, "navList", null );
                    foreach (XmlNode xn in XmlDocumentHelper.GetChildrenElementsWithName(ncxRootNode, true, "navList", ncxRootNode.NamespaceURI, true))
                    {
                        if (xn.Attributes.GetNamedItem("class").Value == navListNodeName)
                        {
                            navListNode = xn;
                        }
                    }

                    if (navListNode == null)
                    {
                        navListNode = ncxDocument.CreateElement(null, "navList", ncxRootNode.NamespaceURI);
                        ncxRootNode.AppendChild(navListNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navListNode, "class", navListNodeName);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navListNode, "id", GetNextID(ID_NcxPrefix));

                        XmlNode mainNavLabel = ncxDocument.CreateElement(null, "navLabel", navListNode.NamespaceURI);
                        navListNode.AppendChild(mainNavLabel);
                        XmlNode mainTextNode = ncxDocument.CreateElement(null, "text", navListNode.NamespaceURI);
                        mainNavLabel.AppendChild(mainTextNode);
                        mainTextNode.AppendChild(ncxDocument.CreateTextNode(navListNodeName));
                    }

                    XmlNode navTargetNode = ncxDocument.CreateElement(null, "navTarget", navListNode.NamespaceURI);
                    navListNode.AppendChild(navTargetNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "id", GetNextID(ID_NcxPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "class", navListNodeName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "playOrder", "");

                    playOrderList_Sorted.Add(navTargetNode);


                    XmlNode navLabelNode = ncxDocument.CreateElement(null, "navLabel", navListNode.NamespaceURI);
                    navTargetNode.AppendChild(navLabelNode);

                    XmlNode txtNode = ncxDocument.CreateElement(null, "text", navTargetNode.NamespaceURI);
                    navLabelNode.AppendChild(txtNode);
                    txtNode.AppendChild(
                        ncxDocument.CreateTextNode(n.GetTextMediaFlattened()));

                    // create audio node only if external audio media is not null
                    if (externalAudio != null)
                        {
                        XmlNode audioNodeNcx = ncxDocument.CreateElement ( null, "audio", navTargetNode.NamespaceURI );
                        navLabelNode.AppendChild ( audioNodeNcx );
                        XmlDocumentHelper.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString () );
                        XmlDocumentHelper.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString () );
                        XmlDocumentHelper.CreateAppendXmlAttribute ( ncxDocument, audioNodeNcx, "src", Path.GetFileName ( externalAudio.Src ) );
                        }

                    XmlNode contentNode = ncxDocument.CreateElement(null, "content", navTargetNode.NamespaceURI);
                    navTargetNode.AppendChild(contentNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + par_id);

                }

                if (!IsNcxNativeNodeAdded)
                {
                    if (isDoctitle_)
                    {
                        //urakawa.core.TreeNode n = textAudioNodesList[0];
                        CreateDocTitle(ncxDocument, ncxRootNode, n);
                        IsNcxNativeNodeAdded = true;
                        /*

                         */
                    }
                    else if (currentHeadingTreeNode != null)
                    {
                        // find node for heading


                        int indexOf_n = 0;

                        if (n.GetXmlElementQName() != null && (n.IsDescendantOf(currentHeadingTreeNode) || n == currentHeadingTreeNode))
                        {
                            //indexOf_n = audioNodeIndex;
                            //indexOf_n = 0;
                        }
                        else
                        {
                            return true;
                        }

                        // -- start copying from here to function

                        /*
                    
                         */
                        navPointNode = CreateNavPointWithoutContentNode(ncxDocument, urakawaNode, currentHeadingTreeNode, n, treeNode_NavNodeMap);
                        playOrderList_Sorted.Add(navPointNode);


                        /*
                    
                         */
                        // -- copied to function till here

                        // add content node
                        if (firstPar_id != null)
                        {
                            XmlNode contentNode = ncxDocument.CreateElement(null, "content", navMapNode.NamespaceURI);
                            navPointNode.AppendChild(contentNode);
                            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + firstPar_id);
                        }
                        int navPointDepth = GetDepthOfNavPointNode(ncxDocument, navPointNode);
                        if (maxDepth < navPointDepth) maxDepth = navPointDepth;

                        IsNcxNativeNodeAdded = true;
                    }
                }
                return true;
            },
                    delegate(urakawa.core.TreeNode n) { });

                // make specials to null
                special_UrakawaNode = null;
                if (Seq_SpecialNode != null)
                {
                    if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                    {
                        Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                    }
                }
                while (specialSeqNodeStack.Count > 0)
                {
                    string str_RefferedSeqID = Seq_SpecialNode.Attributes.GetNamedItem("id").Value;
                    Seq_SpecialNode = specialSeqNodeStack.Pop();
                    if (str_RefferedSeqID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                        Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + str_RefferedSeqID + ".end";
                    //System.Windows.Forms.MessageBox.Show ( "last " + smilFileName + " id " + par_id);
                }
                Seq_SpecialNode = null;


                // add metadata to smil document and write to file.
                if (smilDocument != null)
                {
                    // update duration in seq node
                    XmlNode mainSeqNode = XmlDocumentHelper.GetFirstChildElementWithName(smilDocument, true, "body", null).FirstChild; //smilDocument.GetElementsByTagName("body")[0].FirstChild;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "dur", durationOfCurrentSmil.ToString());
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "fill", "remove");
                    AddMetadata_Smil(smilDocument, smilElapseTime.ToString(), currentSmilCustomTestList);

                    XmlDocumentHelper.WriteXmlDocumentToFile(smilDocument,
                        Path.Combine(m_OutputDirectory, smilFileName));

                    smilElapseTime = smilElapseTime.Add(durationOfCurrentSmil);
                    m_FilesList_Smil.Add(smilFileName);
                    smilDocument = null;

                    // add smil custon test list items to ncx custom test list
                    foreach (string customTestName in currentSmilCustomTestList)
                    {
                        if (!ncxCustomTestList.Contains(customTestName))
                            ncxCustomTestList.Add(customTestName);
                    }

                }

            }

            // assign play orders 
            Dictionary<string, string> playOrder_ReferenceMap = new Dictionary<string, string>();
            int playOrderCounter = 1;
            foreach (XmlNode xn in playOrderList_Sorted)
            {
                XmlNode referedContentNode = XmlDocumentHelper.GetFirstChildElementWithName(xn, false, "content", xn.NamespaceURI);
                string contentNode_Src = referedContentNode.Attributes.GetNamedItem("src").Value;

                if (playOrder_ReferenceMap.ContainsKey(contentNode_Src))
                {
                    xn.Attributes.GetNamedItem("playOrder").Value = playOrder_ReferenceMap[contentNode_Src];
                }
                else
                {
                    xn.Attributes.GetNamedItem("playOrder").Value = playOrderCounter.ToString();
                    playOrder_ReferenceMap.Add(contentNode_Src, playOrderCounter.ToString());
                    ++playOrderCounter;
                    //System.Windows.Forms.MessageBox.Show ( contentNode_Src );
                }
            }

            XmlDocumentHelper.WriteXmlDocumentToFile(m_DTBDocument,
              Path.Combine(m_OutputDirectory, m_Filename_Content));

            // write ncs document to file
            m_TotalTime = smilElapseTime;
            AddMetadata_Ncx(ncxDocument, totalPageCount.ToString(), maxNormalPageNumber.ToString(), maxDepth.ToString(), ncxCustomTestList);
            XmlDocumentHelper.WriteXmlDocumentToFile(ncxDocument,
                Path.Combine(m_OutputDirectory, m_Filename_Ncx));
        }

        private bool IsHeadingNode(urakawa.core.TreeNode node)
        {
            QualifiedName currentQName = node.GetXmlElementQName() != null ? node.GetXmlElementQName() : null;
            if (currentQName != null &&
                    (currentQName.LocalName == "levelhd" || currentQName.LocalName == "hd" || currentQName.LocalName == "h1" || currentQName.LocalName == "h2" || currentQName.LocalName == "h3" || currentQName.LocalName == "h4"
                    || currentQName.LocalName == "h5" || currentQName.LocalName == "h6" || currentQName.LocalName == "doctitle"))
            {
                return true;
            }
            return false;
        }

        private bool IsEscapableNode(urakawa.core.TreeNode node)
        {
            string qName = node.GetXmlElementQName() != null ? node.GetXmlElementQName().LocalName : null;
            if (qName != null
                &&
                (qName == "list" || qName == "table" || qName == "tr"
                || qName == "note" || qName == "annotation" || qName == "sidebar" || qName == "prodnote"))
            {
                return true;
            }
            return false;
        }

        private bool IsSkippableNode(urakawa.core.TreeNode node)
        {
            string qName = node.GetXmlElementQName() != null ? node.GetXmlElementQName().LocalName : null;
            if (qName != null
                &&
                (qName == "pagenum" || qName == "noteref" || qName == "note"
                || qName == "annotation" || qName == "linenum"
                || IsOptionalSidebarOrProducerNote(node)))
            {
                return true;
            }
            return false;
        }

        private bool IsOptionalSidebarOrProducerNote(urakawa.core.TreeNode node)
        {
            string qName = node.GetXmlElementQName() != null ? node.GetXmlElementQName().LocalName : null;
            if (qName != null
                && (qName == "sidebar" || qName == "prodnote"))
            {
                foreach (urakawa.property.xml.XmlAttribute attr in node.GetXmlProperty().Attributes)
                {
                    if (attr.LocalName == "render" && attr.Value == "optional")
                    {
                        return true;
                    }
                }// foreach loop ends

            } // end check for sidebar and producer notes local name

            return false;
        }

        private XmlNode CreateDocTitle(XmlDocument ncxDocument, XmlNode ncxRootNode, urakawa.core.TreeNode n)
        {
            string txtMedia = n.GetTextMediaFlattened();
            urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

            XmlNode docNode = ncxDocument.CreateElement(null,
                "docTitle",
                 ncxRootNode.NamespaceURI);

            XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementWithName(ncxDocument, true, "navMap", null);

            ncxRootNode.InsertBefore(docNode, navMapNode);

            XmlNode docTxtNode = ncxDocument.CreateElement(null, "text", docNode.NamespaceURI);
            docNode.AppendChild(docTxtNode);
            docTxtNode.AppendChild(
            ncxDocument.CreateTextNode(txtMedia));

            if (externalAudio != null)
            {
                // create audio node
                XmlNode docAudioNode = ncxDocument.CreateElement(null, "audio", docNode.NamespaceURI);
                docNode.AppendChild(docAudioNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString());
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString());
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "src", Path.GetFileName(externalAudio.Src));
            }
            return docNode;
        }


        private XmlNode CreateNavPointWithoutContentNode(XmlDocument ncxDocument, urakawa.core.TreeNode urakawaNode, urakawa.core.TreeNode currentHeadingTreeNode, urakawa.core.TreeNode n, Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap)
        {
            XmlNode navMapNode = ncxDocument.GetElementsByTagName("navMap")[0];
            string txtMedia = n.GetTextMediaFlattened();
            urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

            // first create navPoints
            XmlNode navPointNode = ncxDocument.CreateElement(null, "navPoint", navMapNode.NamespaceURI);
            if (currentHeadingTreeNode != null) XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "class", currentHeadingTreeNode.GetProperty<urakawa.property.xml.XmlProperty>().LocalName);
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "id", GetNextID(ID_NcxPrefix));
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "playOrder", "");

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
                ncxDocument.CreateTextNode(txtMedia));

            // create audio node
            XmlNode audioNode = ncxDocument.CreateElement(null, "audio", navMapNode.NamespaceURI);
            navLabel.AppendChild(audioNode);
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString());
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString());
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "src", Path.GetFileName(externalAudio.Src));

            return navPointNode;
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
                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                    }
                    else
                    {
                        metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                        parentNode.AppendChild(metaNodeUid);

                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "name", "dtb:uid");
                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "content", md.NameContentAttribute.Value);
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

                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, ma.Name, ma.Value);
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
                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                }
                else
                {
                    metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                    parentNode.AppendChild(metaNodeUid);

                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "name", "dtb:uid");
                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "content",
                                                             md.NameContentAttribute.Value);
                }

                // add metadata optional attributes if any
                foreach (MetadataAttribute ma in md.OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    if (ma.Name == "id") continue;

                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, ma.Name, ma.Value);
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

            MetadataDefinition md = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition("dc:Identifier");
            return md != null && md.Synonyms.Find(
                                delegate(string s)
                                {
                                    return s.ToLower() == lower;
                                }) != null;
        }

        private void AddMetadata_Ncx(XmlDocument ncxDocument, string strTotalPages, string strMaxNormalPage, string strDepth, List<string> ncxCustomTestList)
        {
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementWithName(ncxDocument, true, "head", null); //ncxDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, ncxDocument, headNode);

            AddMetadata_Generator(ncxDocument, headNode);

            AddMetadataAsAttributes(ncxDocument, headNode, "dtb:depth", strDepth);
            AddMetadataAsAttributes(ncxDocument, headNode, "dtb:totalPageCount", strTotalPages);
            AddMetadataAsAttributes(ncxDocument, headNode, "dtb:maxPageNumber", strMaxNormalPage);


            // add custom test to headNode
            if (ncxCustomTestList.Count > 0)
            {
                // create dictionary for custom test
                Dictionary<string, string> bookStrucMap = new Dictionary<string, string>();
                bookStrucMap.Add("pagenum", "PAGE_NUMBER");
                bookStrucMap.Add("linenum", "LINE_NUMBER");
                bookStrucMap.Add("note", "NOTE");
                bookStrucMap.Add("noteref", "NOTE_REFERENCE");
                bookStrucMap.Add("annotation", "ANNOTATION");
                bookStrucMap.Add("prodnote", "OPTIONAL_PRODUCER_NOTE");
                bookStrucMap.Add("sidebar", "OPTIONAL_SIDEBAR");


                foreach (string customTestName in ncxCustomTestList)
                {
                    XmlNode customTestNode = ncxDocument.CreateElement(null, "smilCustomTest", headNode.NamespaceURI);
                    headNode.AppendChild(customTestNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "bookStruct", bookStrucMap[customTestName]);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "defaultState", "false");
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "id", customTestName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "override", "visible");
                }
            }

        }

        private void AddMetadata_Smil(XmlDocument smilDocument, string strElapsedTime, List<string> currentSmilCustomTestList)
        {
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementWithName(smilDocument, true, "head", null); //smilDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, smilDocument, headNode);

            AddMetadata_Generator(smilDocument, headNode);

            AddMetadataAsAttributes(smilDocument, headNode, "dtb:totalElapsedTime", strElapsedTime);


            //if (isCustomTestRequired)
            //{
            if (currentSmilCustomTestList.Count > 0)
            {
                XmlNode customAttributesNode = smilDocument.CreateElement(null, "customAttributes", headNode.NamespaceURI);
                headNode.AppendChild(customAttributesNode);
                foreach (string customTestName in currentSmilCustomTestList)
                {

                    XmlNode customTestNode = smilDocument.CreateElement(null, "customTest", headNode.NamespaceURI);
                    customAttributesNode.AppendChild(customTestNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, customTestNode, "defaultState", "false");
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, customTestNode, "id", customTestName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, customTestNode, "override", "visible");

                    //<customTest defaultState="false" id="note" override="visible" />
                }
            }

        }

        private XmlNode AddMetadataAsAttributes(XmlDocument doc, XmlNode headNode, string name, string content)
        {
            XmlNode metaNode = doc.CreateElement(null, "meta", headNode.NamespaceURI);
            headNode.AppendChild(metaNode);
            XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNode, "name", name);
            XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNode, "content", content);

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


            XmlDocumentHelper.CreateAppendXmlAttribute(NcxDocument, rootNode, "version", "2005-1");
            XmlDocumentHelper.CreateAppendXmlAttribute(NcxDocument, rootNode, "xml:lang", (string.IsNullOrEmpty(m_Presentation.Language) ? "en-US" : m_Presentation.Language));


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
