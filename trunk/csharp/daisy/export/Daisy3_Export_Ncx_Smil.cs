using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.media.timing;
using urakawa.media.data.audio;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;
using urakawa.property.alt;
using urakawa.media.data.audio.codec;
using urakawa.data;

using AudioLib;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        //DAISY3Export_Ncx

        protected string FormatTimeString(Time time)
        {
            return time.Format_StandardExpanded();
        }

        protected uint m_SmilFileNameCounter;

        // to do create IDs
        protected virtual void CreateNcxAndSmilDocuments()
        {
            XmlDocument ncxDocument = CreateStub_NcxDocument();

            XmlNode ncxRootNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "ncx", null); //ncxDocument.GetElementsByTagName("ncx")[0];
            XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "navMap", null); //ncxDocument.GetElementsByTagName("navMap")[0];
            Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();
            m_FilesList_Smil = new List<string>();
            m_FilesList_Audio = new List<string>();
            m_SmilFileNameCounter = 0;
            List<XmlNode> playOrderList_Sorted = new List<XmlNode>();
            int totalPageCount = 0;
            int maxNormalPageNumber = 0;
            int maxDepth = 1;
            Time smilElapseTime = new Time();
            List<string> ncxCustomTestList = new List<string>();
            List<urakawa.core.TreeNode> specialParentNodesAddedToNavList = new List<urakawa.core.TreeNode>();
            bool isDocTitleAdded = false;

            //m_ProgressPercentage = 20;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreateSmilAndNcxFiles);

            foreach (urakawa.core.TreeNode urakawaNode in m_ListOfLevels)
            //for ( int nodeCounter = 0 ; nodeCounter < m_ListOfLevels.Count ; nodeCounter++ )
            {
                //urakawa.core.TreeNode urakawaNode  = m_ListOfLevels[nodeCounter] ;

                bool IsNcxNativeNodeAdded = false;
                XmlDocument smilDocument = null;
                string smilFileName = null;
                XmlNode navPointNode = null;
                urakawa.core.TreeNode currentHeadingTreeNode = null;
                urakawa.core.TreeNode special_UrakawaNode = null;
                Time durationOfCurrentSmil = new Time();
                XmlNode mainSeq = null;
                XmlNode Seq_SpecialNode = null;
                //bool IsPageAdded = false;
                string firstPar_id = null;
                bool shouldAddNewSeq = false;
                string par_id = null;
                List<string> currentSmilCustomTestList = new List<string>();
                Stack<urakawa.core.TreeNode> specialParentNodeStack = new Stack<urakawa.core.TreeNode>();
                Stack<XmlNode> specialSeqNodeStack = new Stack<XmlNode>();

                bool isBranchingActive = false;
                urakawa.core.TreeNode branchStartTreeNode = null;
                bool isTextOnlyMixedContent = false;

                urakawaNode.AcceptDepthFirst(
            delegate(urakawa.core.TreeNode n)
            {

                if (RequestCancellation) return false;

                if (IsHeadingNode(n))
                {
                    currentHeadingTreeNode = n;
                }

                if (n.HasXmlProperty &&
                        n.GetXmlElementLocalName() != urakawaNode.GetXmlElementLocalName()
                        && doesTreeNodeTriggerNewSmil(n))
                {
                    if (m_ListOfLevels.IndexOf(n) > m_ListOfLevels.IndexOf(urakawaNode))
                    {
                        return false;
                    }
                }

                bool nodeIsImageAndHasDescriptionProdnotes = m_includeImageDescriptions && m_Image_ProdNoteMap.ContainsKey(n);

                if ((IsHeadingNode(n) || IsEscapableNode(n) || IsSkippableNode(n) || nodeIsImageAndHasDescriptionProdnotes)
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

                if (!isDocTitleAdded
                    && !IsNcxNativeNodeAdded
                    && currentHeadingTreeNode != null
                    && currentHeadingTreeNode.HasXmlProperty
                    && currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle")
                {
                    Time currentHeadingTreeNodeDur = currentHeadingTreeNode.GetDurationOfManagedAudioMediaFlattened();

                    if (currentHeadingTreeNodeDur == null
                        || currentHeadingTreeNodeDur.AsLocalUnits == 0)
                    {
                        CreateDocTitle(ncxDocument, ncxRootNode, n);
                        isDocTitleAdded = true;
                        IsNcxNativeNodeAdded = true;
                    }
                }



                Time urakawaNodeDur = urakawaNode.GetDurationOfManagedAudioMediaFlattened();
                if (currentHeadingTreeNode == null && urakawaNodeDur != null && urakawaNodeDur.AsTimeSpan == TimeSpan.Zero)
                {
                    return true;
                    // carry on processing following lines. and in case this is not true, skip all the following lines
                }

                // create smil stub document
                if (smilDocument == null)
                {
                    smilDocument = CreateStub_SmilDocument();
                    mainSeq = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "body", null).FirstChild;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeq, "id", GetNextID(ID_SmilPrefix));
                    smilFileName = GetNextSmilFileName;
                    //m_ProgressPercentage += Convert.ToInt32((m_SmilFileNameCounter / m_ListOfLevels.Count) * 100 * 0.7);
                    //reportProgress(m_ProgressPercentage, String.Format(UrakawaSDK_daisy_Lang.CreatingSmilFiles, m_SmilFileNameCounter, m_ListOfLevels.Count));
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
                    if (special_UrakawaNode.GetXmlElementLocalName() == "note" || special_UrakawaNode.GetXmlElementLocalName() == "annotation")
                    {
                        strSeqID = ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value;
                    }
                    else
                    {
                        strSeqID = GetNextID(ID_SmilPrefix);
                    }
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "id", strSeqID);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", nodeIsImageAndHasDescriptionProdnotes ? "prodnote" : special_UrakawaNode.GetXmlElementLocalName());

                    if (IsEscapableNode(special_UrakawaNode))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "end", "");
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "fill", "remove");
                    }

                    if (IsSkippableNode(special_UrakawaNode) || nodeIsImageAndHasDescriptionProdnotes)
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", nodeIsImageAndHasDescriptionProdnotes ? "prodnote" : special_UrakawaNode.GetXmlElementLocalName());

                        if (special_UrakawaNode.GetXmlElementLocalName() == "noteref" || special_UrakawaNode.GetXmlElementLocalName() == "annoref")
                        {
                            XmlNode anchorNode = smilDocument.CreateElement(null, "a", Seq_SpecialNode.NamespaceURI);
                            Seq_SpecialNode.AppendChild(anchorNode);
                            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "external", "false");
                            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href", "#" + ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("idref").Value.Replace("#", ""));

                            isBranchingActive = true;

                            branchStartTreeNode = GetReferedTreeNode(special_UrakawaNode);

                        }

                        if (nodeIsImageAndHasDescriptionProdnotes)
                        {
                            if (!currentSmilCustomTestList.Contains("prodnote"))
                            {
                                currentSmilCustomTestList.Add("prodnote");
                                //System.Windows.Forms.MessageBox.Show("prodnote added");
                            }
                        }
                        else if (!currentSmilCustomTestList.Contains(special_UrakawaNode.GetXmlElementLocalName()))
                        {
                            currentSmilCustomTestList.Add(special_UrakawaNode.GetXmlElementLocalName());
                        }
                    }

                    // add smilref reference to seq_special  in dtbook document
                    if (IsEscapableNode(special_UrakawaNode) || IsSkippableNode(special_UrakawaNode)) // || nodeIsImageAndHasDescriptionProdnotes) WE DON'T WANT THE IMAGE ITSELF TO SMIL-REF THE FIRST PRODNOTE (link to Diagram desc XML file)
                    {
                        XmlNode dtbEscapableNode = m_TreeNode_XmlNodeMap[special_UrakawaNode];
                        XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtbEscapableNode, "smilref", smilFileName + "#" + strSeqID, m_DTBDocument.DocumentElement.NamespaceURI);
                    }
                    if (nodeIsImageAndHasDescriptionProdnotes)
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, m_Image_ProdNoteMap[n][0], "smilref", smilFileName + "#" + strSeqID, m_DTBDocument.DocumentElement.NamespaceURI);
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

                bool noAudioInAncestor = (n.GetFirstAncestorWithManagedAudio() == null);
                isTextOnlyMixedContent = noAudioInAncestor && IsTextOnlyMixedContent(n, externalAudio);

                if (
                    externalAudio != null
                    || nodeIsImageAndHasDescriptionProdnotes
                    ||
                    (externalAudio == null && noAudioInAncestor && n.GetTextMedia () != null )
                    ||
                    isTextOnlyMixedContent
                    ||
                    (
                    n.GetTextMedia() != null
                    && special_UrakawaNode != null
                    &&
                    (
                    IsEscapableNode(special_UrakawaNode)
                    || IsSkippableNode(special_UrakawaNode)
                    ||
                    (
                    special_UrakawaNode.GetXmlProperty() != null
                    && special_UrakawaNode.GetXmlProperty().LocalName.Equals("doctitle", StringComparison.OrdinalIgnoreCase)
                    )
                    )
                    &&
                    (
                    m_TreeNode_XmlNodeMap[n].Attributes != null
                    || m_TreeNode_XmlNodeMap[n.Parent].Attributes != null
                    )
                    )
                    )
                {
                    //if (n.GetTextMedia() != null) Console.WriteLine(n.GetTextMedia().Text);
                    // continue ahead 
                }
                else
                {
                    //if (n.GetTextMedia() != null) Console.WriteLine("Return: " +  n.GetTextMedia().Text);
                    return true;
                }

                

                if (n.HasXmlProperty
                    &&
                    (externalAudio != null
                    || n.GetFirstAncestorWithManagedAudio() == null) //write the element with text but no audio to smil
                    || isTextOnlyMixedContent
                    )
                {
                    XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);

                    // decide the parent node for this new par node.
                    // if node n is child of current specialParentNode than append to it 
                    //else check if it has to be appended to parent of this special node in stack or to main seq.
                    if (special_UrakawaNode != null &&
                        (special_UrakawaNode == n || n.IsDescendantOf(special_UrakawaNode)))
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
                                    Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" +
                                                                                           strReferedID + ".end";
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
                        } // stack > 0 check ends

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
                                    Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" +
                                                                                           strReferedID + ".end";
                                }
                                Seq_SpecialNode = null;
                            }
                        } // check of append to main seq ends

                    }


                    par_id = GetNextID(ID_SmilPrefix);
                    // check and assign first par ID
                    if (firstPar_id == null)
                    {
                        if (n.HasXmlProperty && currentHeadingTreeNode != null
                            && (n.IsDescendantOf(currentHeadingTreeNode) || n == currentHeadingTreeNode))
                        {
                            firstPar_id = par_id;
                        }
                    }

                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);


                    string dtbookSmilRef = null;
                    XmlNode dtBookNode = null;
                    if (nodeIsImageAndHasDescriptionProdnotes)
                    {
                        dtBookNode = m_Image_ProdNoteMap[n][0];
                        XmlNode attr = dtBookNode.Attributes != null ?
                                        dtBookNode.Attributes.GetNamedItem("smilref") : null;
                        dtbookSmilRef = attr != null ? attr.Value : null;
                    }
                    else
                    {
                        dtBookNode = m_TreeNode_XmlNodeMap[n];
                        XmlNode attr = dtBookNode.Attributes != null ?
                                        dtBookNode.Attributes.GetNamedItem("smilref") : null;
                        dtbookSmilRef = attr != null ? attr.Value : null;
                    }
                    if (dtBookNode != null && string.IsNullOrEmpty(dtbookSmilRef))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtBookNode, "smilref", smilFileName + "#" + par_id, m_DTBDocument.DocumentElement.NamespaceURI);
                    }
                    else
                    {
                        bool debug = true;
                    }


                    XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id",
                                                               GetNextID(ID_SmilPrefix));

                    bool isMath = m_TreeNode_XmlNodeMap[n].LocalName == DiagramContentModelHelper.Math;
                    if (isMath)
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "type", DiagramContentModelHelper.NS_URL_MATHML);
                    }

                    string dtbookID = null;
                    if (nodeIsImageAndHasDescriptionProdnotes)
                    {
                        dtbookID = m_Image_ProdNoteMap[n][0].Attributes.GetNamedItem("id").Value;
                    }
                    else
                    {
                        dtbookID = m_TreeNode_XmlNodeMap[n].Attributes != null && m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id") != null
                                       ? m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value
                                       : m_TreeNode_XmlNodeMap[n.Parent].Attributes.GetNamedItem("id").Value;
                    }
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
                                                               m_Filename_Content + "#" + dtbookID);
                    parNode.AppendChild(SmilTextNode);
                    if (externalAudio != null)
                    {
                        XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
                                                                   FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
                                                                   FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src",
                                                                   Path.GetFileName(externalAudio.Src));
                        parNode.AppendChild(audioNode);

                        // add audio file name in audio files list for use in opf creation 
                        string audioFileName = Path.GetFileName(externalAudio.Src);
                        if (!m_FilesList_Audio.Contains(audioFileName)) m_FilesList_Audio.Add(audioFileName);

                        // add to duration 
                        durationOfCurrentSmil.Add(externalAudio.Duration);
                    }
                }

                if (nodeIsImageAndHasDescriptionProdnotes)
                {
                    CreateSmilNodesForImageDescription(n, smilDocument, mainSeq, durationOfCurrentSmil, n.GetAlternateContentProperty(), smilFileName);
                }
                // if node n is pagenum, add to pageList
                if (n.HasXmlProperty
                    && n.GetXmlElementLocalName() == "pagenum")
                {
                    if (!currentSmilCustomTestList.Contains("pagenum"))
                    {
                        currentSmilCustomTestList.Add("pagenum");
                    }

                    XmlNode pageListNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "pageList", null);
                    if (pageListNode == null)
                    {
                        pageListNode = ncxDocument.CreateElement(null, "pageList", ncxRootNode.NamespaceURI);
                        //ncxRootNode.AppendChild(pageListNode);
                        ncxRootNode.InsertAfter(pageListNode, navMapNode);
                    }

                    XmlNode pageTargetNode = ncxDocument.CreateElement(null, "pageTarget", pageListNode.NamespaceURI);
                    pageListNode.AppendChild(pageTargetNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "id", GetNextID(ID_NcxPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "class", "pagenum");
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "playOrder", "");
                    string strTypeVal = n.GetXmlProperty().GetAttribute("page").Value;
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "type", strTypeVal);
                    string strPageValue = n.GetTextFlattened();
                    ++totalPageCount;

                    playOrderList_Sorted.Add(pageTargetNode);

                    if (strTypeVal == "normal")
                    {
                        int tmp;
                        bool success = int.TryParse(strPageValue, out tmp);
                        if (success && maxNormalPageNumber < tmp) maxNormalPageNumber = tmp;
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
                        XmlNode audioNodeNcx = ncxDocument.CreateElement(null, "audio", pageListNode.NamespaceURI);
                        navLabelNode.AppendChild(audioNodeNcx);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", Path.GetFileName(externalAudio.Src));
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
                else if (n.HasXmlProperty
                    && n.GetXmlElementLocalName() == DiagramContentModelHelper.Math)
                {
                    string idValue = ID_NcxPrefix + DiagramContentModelHelper.Math;

                    XmlNode navListNodeMathML = null;
                    foreach (XmlNode navListNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(ncxDocument, true, "navList", null, false))
                    {
                        if (navListNode.NodeType != XmlNodeType.Element || navListNode.LocalName != "navList")
                        {
#if DEBUG
                            Debugger.Break();
#endif
                            // DEBUG
                            continue;
                        }

                        XmlAttributeCollection attrs = navListNode.Attributes;
                        if (attrs == null) continue;

                        XmlNode idAttr = attrs.GetNamedItem("id");
                        if (idAttr == null) continue;

                        if (idAttr.Value == idValue)
                        {
                            navListNodeMathML = navListNode;
                            break;
                        }
                    }

                    if (navListNodeMathML == null)
                    {
                        navListNodeMathML = ncxDocument.CreateElement(null, "navList", ncxRootNode.NamespaceURI);
                        ncxRootNode.AppendChild(navListNodeMathML);
                        //ncxRootNode.InsertAfter(navListNodeMathML, navMapNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navListNodeMathML, "id", idValue);


                        XmlNode mainNavLabel = ncxDocument.CreateElement(null, "navLabel", navListNodeMathML.NamespaceURI);
                        navListNodeMathML.AppendChild(mainNavLabel);
                        XmlNode mainTextNode = ncxDocument.CreateElement(null, "text", navListNodeMathML.NamespaceURI);
                        mainNavLabel.AppendChild(mainTextNode);
                        mainTextNode.AppendChild(ncxDocument.CreateTextNode(DiagramContentModelHelper.Math));
                    }

                    XmlNode navTargetNode = ncxDocument.CreateElement(null, "navTarget", navListNodeMathML.NamespaceURI);
                    navListNodeMathML.AppendChild(navTargetNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "id", GetNextID(ID_NcxPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "class", DiagramContentModelHelper.Math);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "playOrder", "");

                    string strMathML = n.GetTextFlattened(); // grabs alttext, if any

                    playOrderList_Sorted.Add(navTargetNode);

                    XmlNode navLabelNode = ncxDocument.CreateElement(null, "navLabel", navListNodeMathML.NamespaceURI);
                    navTargetNode.AppendChild(navLabelNode);

                    XmlNode txtNode = ncxDocument.CreateElement(null, "text", navListNodeMathML.NamespaceURI);
                    navLabelNode.AppendChild(txtNode);
                    txtNode.AppendChild(ncxDocument.CreateTextNode(strMathML));

                    if (externalAudio != null)
                    {
                        XmlNode audioNodeNcx = ncxDocument.CreateElement(null, "audio", navListNodeMathML.NamespaceURI);
                        navLabelNode.AppendChild(audioNodeNcx);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", Path.GetFileName(externalAudio.Src));
                    }

                    XmlNode contentNode = ncxDocument.CreateElement(null, "content", navListNodeMathML.NamespaceURI);
                    navTargetNode.AppendChild(contentNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + par_id);

                }
                else if (special_UrakawaNode != null
                  && m_NavListElementNamesList.Contains(special_UrakawaNode.GetXmlElementLocalName())
                    && !specialParentNodesAddedToNavList.Contains(special_UrakawaNode)

                    ||
                    nodeIsImageAndHasDescriptionProdnotes)
                {
                    string navListNodeName = nodeIsImageAndHasDescriptionProdnotes ? DiagramContentModelHelper.EPUB_DescribedAt : special_UrakawaNode.GetXmlElementLocalName();
                    specialParentNodesAddedToNavList.Add(special_UrakawaNode);

                    XmlNode navListNode = null;
                    //= getFirstChildElementsWithName ( ncxDocument, true, "navList", null );
                    foreach (XmlNode xn in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(ncxRootNode, true, "navList", ncxRootNode.NamespaceURI, false))
                    {
                        if (xn.Attributes.GetNamedItem("class").Value == navListNodeName)
                        {
                            navListNode = xn;
                            break;
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
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "class", nodeIsImageAndHasDescriptionProdnotes ? "prodnote" : navListNodeName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "playOrder", "");

                    playOrderList_Sorted.Add(navTargetNode);


                    XmlNode navLabelNode = ncxDocument.CreateElement(null, "navLabel", navListNode.NamespaceURI);
                    navTargetNode.AppendChild(navLabelNode);

                    XmlNode txtNode = ncxDocument.CreateElement(null, "text", navTargetNode.NamespaceURI);
                    navLabelNode.AppendChild(txtNode);
                    txtNode.AppendChild(
                        ncxDocument.CreateTextNode(n.GetTextFlattened()));

                    // create audio node only if external audio media is not null
                    if (externalAudio != null)
                    {
                        XmlNode audioNodeNcx = ncxDocument.CreateElement(null, "audio", navTargetNode.NamespaceURI);
                        navLabelNode.AppendChild(audioNodeNcx);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", Path.GetFileName(externalAudio.Src));
                    }

                    XmlNode contentNode = ncxDocument.CreateElement(null, "content", navTargetNode.NamespaceURI);
                    navTargetNode.AppendChild(contentNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + par_id);

                }

                if (!IsNcxNativeNodeAdded)
                {
                    if (!isDocTitleAdded
                        && currentHeadingTreeNode != null
                        && currentHeadingTreeNode.HasXmlProperty
                        && currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle")
                    {
                        CreateDocTitle(ncxDocument, ncxRootNode, n);
                        isDocTitleAdded = true;
                        IsNcxNativeNodeAdded = true;
                    }
                    else if (currentHeadingTreeNode != null)
                    {
                        // find node for heading


                        int indexOf_n = 0;

                        if (n.HasXmlProperty && (n.IsDescendantOf(currentHeadingTreeNode) || n == currentHeadingTreeNode))
                        {
                            //indexOf_n = audioNodeIndex;
                            //indexOf_n = 0;
                        }
                        else
                        {
                            return true;
                        }

                        // -- start copying from here to function


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
                
                if (isBranchingActive)
                {
                    //IsBranchAssigned = true;

                    durationOfCurrentSmil.Add(CreateFollowupNoteAndAnnotationNodes(smilDocument, mainSeq, branchStartTreeNode, smilFileName, currentSmilCustomTestList, ncxCustomTestList));

                    isBranchingActive = false;

                }
                if (n.HasXmlProperty && n.GetXmlElementLocalName() == "sent"
                        && special_UrakawaNode != null
                        && (special_UrakawaNode.GetXmlElementLocalName() == "note"
                        || special_UrakawaNode.GetXmlElementLocalName() == "annotation"))
                {
                    return false;
                }
                if (isTextOnlyMixedContent)
                {
                    isTextOnlyMixedContent = false;
                    return false;
                }
                return true;
            },









                    delegate(urakawa.core.TreeNode n) { }
                    );













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

                if (RequestCancellation) return;
                // add metadata to smil document and write to file.
                if (smilDocument != null)
                {
                    // update duration in seq node
                    XmlNode mainSeqNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "body", null).FirstChild; //smilDocument.GetElementsByTagName("body")[0].FirstChild;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "dur", FormatTimeString(durationOfCurrentSmil));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "fill", "remove");
                    AddMetadata_Smil(smilDocument, FormatTimeString(smilElapseTime), currentSmilCustomTestList);

                    XmlReaderWriterHelper.WriteXmlDocument(smilDocument, Path.Combine(m_OutputDirectory, smilFileName));

                    smilElapseTime.Add(durationOfCurrentSmil);
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
                XmlNode referedContentNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xn, false, "content", xn.NamespaceURI);
                string contentNode_Src = referedContentNode.Attributes.GetNamedItem("src").Value;

                string str;
                playOrder_ReferenceMap.TryGetValue(contentNode_Src, out str);

                if (!string.IsNullOrEmpty(str)) //playOrder_ReferenceMap.ContainsKey(contentNode_Src))
                {
                    xn.Attributes.GetNamedItem("playOrder").Value = str; //playOrder_ReferenceMap[contentNode_Src];
                }
                else
                {
                    xn.Attributes.GetNamedItem("playOrder").Value = playOrderCounter.ToString();
                    playOrder_ReferenceMap.Add(contentNode_Src, playOrderCounter.ToString());
                    ++playOrderCounter;
                    //System.Windows.Forms.MessageBox.Show ( contentNode_Src );
                }
            }

            if (RequestCancellation)
            {
                m_DTBDocument = null;
                ncxDocument = null;
                return;
            }
            XmlReaderWriterHelper.WriteXmlDocument(m_DTBDocument, Path.Combine(m_OutputDirectory, m_Filename_Content));

            if (RequestCancellation)
            {
                m_DTBDocument = null;
                ncxDocument = null;
                return;
            }
            // write ncs document to file
            m_TotalTime = new Time(smilElapseTime);
            AddMetadata_Ncx(ncxDocument, totalPageCount.ToString(), maxNormalPageNumber.ToString(), maxDepth.ToString(), ncxCustomTestList);
            XmlReaderWriterHelper.WriteXmlDocument(ncxDocument, Path.Combine(m_OutputDirectory, m_Filename_Ncx));
        }

        private bool IsHeadingNode(urakawa.core.TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "levelhd"
                    || localName == "hd"
                    || localName == "h1"
                    || localName == "h2"
                    || localName == "h3"
                    || localName == "h4"
                    || localName == "h5"
                    || localName == "h6"
                    || localName == "doctitle")
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsEscapableNode(urakawa.core.TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "list"
                    || localName == "table"
                    || localName == "tr"
                    || localName == "note"
                    || localName == "annotation"
                    || localName == "sidebar"
                    || localName == "prodnote"
                    || localName == "endnote"
                    || localName == "footnote"
                    || localName == "rearnote"
                    || localName == DiagramContentModelHelper.Math
                    )
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsSkippableNode(urakawa.core.TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "pagenum"
                    || localName == "linenum"
                    || localName == "noteref"
                    || localName == "note"
                    || localName == "annoref"
                    || localName == "annotation"
                    || IsOptionalSidebarOrProducerNote(node))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOptionalSidebarOrProducerNote(urakawa.core.TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "sidebar" || localName == "prodnote")
                {
                    foreach (urakawa.property.xml.XmlAttribute xmlAttr in node.GetXmlProperty().Attributes.ContentsAs_Enumerable)
                    {
                        string prefix = xmlAttr.Prefix;
                        string nameWithoutPrefix = xmlAttr.PrefixedLocalName != null ? xmlAttr.PrefixedLocalName : xmlAttr.LocalName;

                        if (nameWithoutPrefix == "render" && xmlAttr.Value == "optional")
                        {
                            return true;
                        }
                    } // foreach loop ends

                } // end check for sidebar and producer notes local name
            }
            return false;
        }

        private bool IsTextOnlyMixedContent(urakawa.core.TreeNode node, media.ExternalAudioMedia externalAudio)
        {
            if (externalAudio != null) return false;
            
            bool isChildWithoutElement = false;
            bool isAudioInChildren = false;
            if (node.GetXmlProperty() != null && node.Children.Count > 0
                && !IsSkippableNode(node) && !IsEscapableNode(node))
            {   
                foreach (urakawa.core.TreeNode n in node.Children.ContentsAs_ListAsReadOnly)
                {
                    if (n.GetTextMedia () != null  && n.GetXmlProperty() == null)
                    {
                            isChildWithoutElement = true;
                        
                    }
              
                }
            }
            if (isChildWithoutElement)
            {
                node.AcceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (isAudioInChildren) return false;
                    if (GetExternalAudioMedia(n) != null) isAudioInChildren = true;
                    return true;
                },
                delegate(urakawa.core.TreeNode n) { }
                        );
            }
            return isChildWithoutElement  && !isAudioInChildren;
        }

        private XmlNode CreateDocTitle(XmlDocument ncxDocument, XmlNode ncxRootNode, urakawa.core.TreeNode n)
        {
            XmlNode docNode = ncxDocument.CreateElement(null,
                "docTitle",
                 ncxRootNode.NamespaceURI);

            XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "navMap", null);

            ncxRootNode.InsertBefore(docNode, navMapNode);

            XmlNode docTxtNode = ncxDocument.CreateElement(null, "text", docNode.NamespaceURI);
            docNode.AppendChild(docTxtNode);
            docTxtNode.AppendChild(
            ncxDocument.CreateTextNode(n.GetTextFlattened()));


            urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

            if (externalAudio != null)
            {
                // create audio node
                XmlNode docAudioNode = ncxDocument.CreateElement(null, "audio", docNode.NamespaceURI);
                docNode.AppendChild(docAudioNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "src", Path.GetFileName(externalAudio.Src));
            }
            return docNode;
        }


        private XmlNode CreateNavPointWithoutContentNode(XmlDocument ncxDocument, urakawa.core.TreeNode urakawaNode, urakawa.core.TreeNode currentHeadingTreeNode, urakawa.core.TreeNode n, Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap)
        {
            XmlNode navMapNode = ncxDocument.GetElementsByTagName("navMap")[0];

            urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

            // first create navPoints
            XmlNode navPointNode = ncxDocument.CreateElement(null, "navPoint", navMapNode.NamespaceURI);
            if (currentHeadingTreeNode != null) XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "class", currentHeadingTreeNode.GetXmlProperty().LocalName);
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "id", GetNextID(ID_NcxPrefix));
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "playOrder", "");

            urakawa.core.TreeNode parentNode = GetParentLevelNode(urakawaNode);

            if (parentNode == null)
            {
                navMapNode.AppendChild(navPointNode);
            }
            else
            {
                XmlNode obj;
                treeNode_NavNodeMap.TryGetValue(parentNode, out obj);

                if (obj != null) //treeNode_NavNodeMap.ContainsKey(parentNode))
                {
                    obj.AppendChild(navPointNode); //treeNode_NavNodeMap[parentNode]
                }
                else // surch up for node
                {
                    int counter = 0;
                    while (parentNode != null && counter <= 6)
                    {
                        parentNode = GetParentLevelNode(parentNode);

                        if (parentNode != null
                            //&& treeNode_NavNodeMap.ContainsKey(parentNode)
                            )
                        {
                            treeNode_NavNodeMap.TryGetValue(parentNode, out obj);
                            if (obj != null)
                            {
                                obj.AppendChild(navPointNode); //treeNode_NavNodeMap[parentNode]
                                break;
                            }
                        }
                        counter++;
                    }

                    if (parentNode == null || counter > 7)
                    {
                        navMapNode.AppendChild(navPointNode);
                    }
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
                ncxDocument.CreateTextNode(n.GetTextFlattened()));

            if (externalAudio != null)
            {
                // create audio node
                XmlNode audioNode = ncxDocument.CreateElement(null, "audio", navMapNode.NamespaceURI);
                navLabel.AppendChild(audioNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipBegin",
                                                           FormatTimeString(externalAudio.ClipBegin));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipEnd",
                                                           FormatTimeString(externalAudio.ClipEnd));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "src",
                                                           Path.GetFileName(externalAudio.Src));
            }
            return navPointNode;
        }

        private urakawa.core.TreeNode GetParentLevelNode(urakawa.core.TreeNode node)
        {
            urakawa.core.TreeNode parentNode = node.Parent;

            while (parentNode != null)
            {
                DebugFix.Assert(parentNode.HasXmlProperty);

                if (parentNode.GetXmlElementLocalName().StartsWith("level"))
                {
                    return parentNode;
                }
                parentNode = parentNode.Parent;
            }

            return null;
        }

        protected int GetDepthOfNavPointNode(XmlDocument doc, XmlNode navPointNode)
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
            foreach (Metadata md in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (!isUniqueIdName(md.NameContentAttribute.Name)) continue;

                foreach (MetadataAttribute mda in md.OtherAttributes.ContentsAs_Enumerable)
                {
                    if (mda.Name != "id") continue;

                    //AddMetadataAsAttributes(ncxDocument, headNode, "dtb:uid", md.NameContentAttribute.Value);

                    if (asInnerText)
                    {
                        metaNodeUid = AddMetadataAsInnerText(doc, parentNode, SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":Identifier", md.NameContentAttribute.Value);
                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                    }
                    else
                    {
                        metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                        parentNode.AppendChild(metaNodeUid);

                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Name, SupportedMetadata_Z39862005.DTB_UID);
                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Content, md.NameContentAttribute.Value);
                    }

                    mdUid = md;
                    break;
                }

                if (mdUid != null)
                {
                    // add metadata optional attributes if any
                    foreach (MetadataAttribute ma in md.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id") continue;

                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, ma.Name, ma.Value);
                    }

                    return mdUid;
                }
            }

            foreach (Metadata md in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (!isUniqueIdName(md.NameContentAttribute.Name)) continue;

                //AddMetadataAsAttributes(ncxDocument, headNode, "dtb:uid", md.NameContentAttribute.Value);
                if (asInnerText)
                {
                    metaNodeUid = AddMetadataAsInnerText(doc, parentNode, SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":Identifier", md.NameContentAttribute.Value);
                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                }
                else
                {
                    metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                    parentNode.AppendChild(metaNodeUid);

                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Name, SupportedMetadata_Z39862005.DTB_UID);
                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Content,
                                                             md.NameContentAttribute.Value);
                }

                // add metadata optional attributes if any
                foreach (MetadataAttribute ma in md.OtherAttributes.ContentsAs_Enumerable)
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
            if (SupportedMetadata_Z39862005.DC_Identifier.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            MetadataDefinition md = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(SupportedMetadata_Z39862005.DC_Identifier);
            //SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":Identifier"

            return md != null && md.Synonyms.Find(
                                delegate(string s)
                                {
                                    return name.Equals(s, StringComparison.OrdinalIgnoreCase);
                                }) != null;
        }

        protected void AddMetadata_Ncx(XmlDocument ncxDocument, string strTotalPages, string strMaxNormalPage, string strDepth, List<string> ncxCustomTestList)
        {
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "head", null); //ncxDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, ncxDocument, headNode);

            AddMetadata_Generator(ncxDocument, headNode);

            AddMetadataAsAttributes(ncxDocument, headNode, SupportedMetadata_Z39862005.DTB_DEPTH, strDepth);
            AddMetadataAsAttributes(ncxDocument, headNode, SupportedMetadata_Z39862005.DTB_TOTAL_PAGE_COUNT, strTotalPages);
            AddMetadataAsAttributes(ncxDocument, headNode, SupportedMetadata_Z39862005.DTB_MAX_PAGE_NUMBER, strMaxNormalPage);


            // add custom test to headNode
            if (ncxCustomTestList.Count > 0)
            {
                // create dictionary for custom test
                Dictionary<string, string> bookStrucMap = new Dictionary<string, string>();
                bookStrucMap.Add("pagenum", "PAGE_NUMBER");
                bookStrucMap.Add("linenum", "LINE_NUMBER");
                bookStrucMap.Add("note", "NOTE");
                bookStrucMap.Add("noteref", "NOTE_REFERENCE");
                bookStrucMap.Add("annoref", "NOTE_REFERENCE");
                bookStrucMap.Add("annotation", "ANNOTATION");
                bookStrucMap.Add("prodnote", "OPTIONAL_PRODUCER_NOTE");
                bookStrucMap.Add("sidebar", "OPTIONAL_SIDEBAR");
                bookStrucMap.Add("footnote", "NOTE");
                bookStrucMap.Add("endnote", "NOTE");

                foreach (string customTestName in ncxCustomTestList)
                {
                    if (!bookStrucMap.ContainsKey(customTestName)) continue;
                    XmlNode customTestNode = ncxDocument.CreateElement(null, "smilCustomTest", headNode.NamespaceURI);
                    headNode.AppendChild(customTestNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "bookStruct", bookStrucMap[customTestName]);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "defaultState", "false");

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "id", customTestName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "override", "visible");
                }
            }

        }

        protected void AddMetadata_Smil(XmlDocument smilDocument, string strElapsedTime, List<string> currentSmilCustomTestList)
        {
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "head", null); //smilDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, smilDocument, headNode);

            AddMetadata_Generator(smilDocument, headNode);

            AddMetadataAsAttributes(smilDocument, headNode, SupportedMetadata_Z39862005.DTB_TOTAL_ELAPSED_TIME, strElapsedTime);


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

        private urakawa.core.TreeNode GetReferedTreeNode(urakawa.core.TreeNode node)
        {
            string noteRefID = node.GetXmlProperty().GetAttribute("idref").Value;

            //System.Windows.Forms.MessageBox.Show ( "Attributes xml " + noteRefID );

            noteRefID = noteRefID.Replace("#", "");
            foreach (urakawa.core.TreeNode n in m_NotesNodeList)
            {
                string id = n.GetXmlElementId();

                if (!String.IsNullOrEmpty(id) && id == noteRefID)
                {

                    return n;
                }
            }

            return null;
        }

        protected string GetNextSmilFileName
        {
            get
            {
                string strNumericFrag = (++m_SmilFileNameCounter).ToString();
                return strNumericFrag.PadLeft(4, '0') + ".smil";
            }
        }


        protected XmlDocument CreateStub_NcxDocument()
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
            XmlDocumentHelper.CreateAppendXmlAttribute(NcxDocument, rootNode,
                XmlReaderWriterHelper.XmlLang,
                (string.IsNullOrEmpty(m_Presentation.Language)
                ? "en-US"
                : m_Presentation.Language));


            XmlNode headNode = NcxDocument.CreateElement(null, "head", rootNode.NamespaceURI);
            rootNode.AppendChild(headNode);

            XmlNode navMapNode = NcxDocument.CreateElement(null, "navMap", rootNode.NamespaceURI);
            rootNode.AppendChild(navMapNode);

            return NcxDocument;
        }

        protected XmlDocument CreateStub_SmilDocument()
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

            smilDocument.AppendChild(smilRootNode);

            XmlNode headNode = smilDocument.CreateElement(null, "head", smilRootNode.NamespaceURI);
            smilRootNode.AppendChild(headNode);
            XmlNode bodyNode = smilDocument.CreateElement(null, "body", smilRootNode.NamespaceURI);
            smilRootNode.AppendChild(bodyNode);
            XmlNode mainSeqNode = smilDocument.CreateElement(null, "seq", smilRootNode.NamespaceURI);
            bodyNode.AppendChild(mainSeqNode);

            return smilDocument;
        }


        private Time CreateFollowupNoteAndAnnotationNodes(XmlDocument smilDocument, XmlNode mainSeq, urakawa.core.TreeNode urakawaNode, string smilFileName, List<string> currentSmilCustomTestList, List<string> ncxCustomTestList)
        {

            //Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();

            //List<XmlNode> playOrderList_Sorted = new List<XmlNode>();
            //int totalPageCount = 0;
            //int maxNormalPageNumber = 0;
            //int maxDepth = 1;
            //Time smilElapseTime = new Time();
            //List<string> ncxCustomTestList = new List<string> ();
            //List<urakawa.core.TreeNode> specialParentNodesAddedToNavList = new List<urakawa.core.TreeNode>();
            //m_ProgressPercentage = 20;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreateSmilAndNcxFiles);



            XmlNode navPointNode = null;
            urakawa.core.TreeNode currentHeadingTreeNode = null;
            urakawa.core.TreeNode special_UrakawaNode = null;
            Time durOfCurrentSmil = new Time();

            XmlNode Seq_SpecialNode = null;
            //bool IsPageAdded = false;
            string firstPar_id = null;
            bool shouldAddNewSeq = false;
            string par_id = null;
            //List<string> currentSmilCustomTestList = new List<string> ();
            Stack<urakawa.core.TreeNode> specialParentNodeStack = new Stack<urakawa.core.TreeNode>();
            Stack<XmlNode> specialSeqNodeStack = new Stack<XmlNode>();


            urakawaNode.AcceptDepthFirst(
        delegate(urakawa.core.TreeNode n)
        {//1

            if (RequestCancellation) return false;

            if (IsHeadingNode(n))
            {//2
                currentHeadingTreeNode = n;
            }//-2

            if (n.HasXmlProperty &&
                    n.GetXmlElementLocalName() != urakawaNode.GetXmlElementLocalName()
                    && doesTreeNodeTriggerNewSmil(n))
            {//2
                return false;
            }//-2

            if ((IsHeadingNode(n) || IsEscapableNode(n) || IsSkippableNode(n))
                && (special_UrakawaNode != n))
            {//2
                // if this candidate special node is child of existing special node then ad existing special node to stack for nesting.
                if (special_UrakawaNode != null && Seq_SpecialNode != null
                    && n.IsDescendantOf(special_UrakawaNode))
                {
                    specialParentNodeStack.Push(special_UrakawaNode);
                    specialSeqNodeStack.Push(Seq_SpecialNode);
                }
                special_UrakawaNode = n;
                shouldAddNewSeq = true;
            }//-2

            urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);


            //bool isDoctitle_1 = currentHeadingTreeNode != null && currentHeadingTreeNode.HasXmlProperty &&
            //                    currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle";


            Time urakawaNodeDur = urakawaNode.GetDurationOfManagedAudioMediaFlattened();
            if (currentHeadingTreeNode == null && urakawaNodeDur != null && urakawaNodeDur.AsTimeSpan == TimeSpan.Zero)
            {
                return true;
                // carry on processing following lines. and in case this is not true, skip all the following lines
            }


            //bool isDoctitle_ = currentHeadingTreeNode != null && currentHeadingTreeNode.HasXmlProperty &&
            //                    currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle";


            // create smil stub document


            // create smil nodes

            if (shouldAddNewSeq)
            {//2
                if (Seq_SpecialNode != null)
                {//3
                    if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                    {//4
                        Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                    }//-4
                    Seq_SpecialNode = null;
                }//-3

                Seq_SpecialNode = smilDocument.CreateElement(null, "seq", mainSeq.NamespaceURI);
                string strSeqID = "";
                // specific handling of IDs for notes for allowing predetermined refered IDs
                if (special_UrakawaNode.GetXmlElementLocalName() == "note" || special_UrakawaNode.GetXmlElementLocalName() == "annotation")
                {//3
                    strSeqID = ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value + "_1";
                }//-3
                else
                {//3
                    strSeqID = GetNextID(ID_SmilPrefix);
                }//-3
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "id", strSeqID);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", special_UrakawaNode.GetXmlElementLocalName());

                if (IsEscapableNode(special_UrakawaNode))
                {//3
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "end", "");
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "fill", "remove");
                }//-3

                if (IsSkippableNode(special_UrakawaNode))
                {//3
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", special_UrakawaNode.GetXmlElementLocalName());

                    if (special_UrakawaNode.GetXmlElementLocalName() == "noteref" || special_UrakawaNode.GetXmlElementLocalName() == "annoref")
                    {//4
                        XmlNode anchorNode = smilDocument.CreateElement(null, "a", Seq_SpecialNode.NamespaceURI);
                        Seq_SpecialNode.AppendChild(anchorNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "external", "false");
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href", "#" + ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("idref").Value.Replace("#", ""));

                    }//-4
                    if (!currentSmilCustomTestList.Contains(special_UrakawaNode.GetXmlElementLocalName()))
                    {//4
                        currentSmilCustomTestList.Add(special_UrakawaNode.GetXmlElementLocalName());
                    }//-4
                }//-3

                // add smilref reference to seq_special  in dtbook document
                if (IsEscapableNode(special_UrakawaNode) || IsSkippableNode(special_UrakawaNode))
                {//3
                    XmlNode dtbEscapableNode = m_TreeNode_XmlNodeMap[special_UrakawaNode];
                    XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtbEscapableNode, "smilref", smilFileName + "#" + strSeqID, m_DTBDocument.DocumentElement.NamespaceURI);
                }//-3

                // decide the parent node to which this new seq node is to be appended.
                if (specialSeqNodeStack.Count == 0)
                {//3
                    mainSeq.AppendChild(Seq_SpecialNode);
                }//-3
                else
                {//3
                    specialSeqNodeStack.Peek().AppendChild(Seq_SpecialNode);
                }//-3

                shouldAddNewSeq = false;
            }//-2

            if (
                externalAudio != null
                ||
                (
                n.GetTextMedia() != null
                &&
                special_UrakawaNode != null
                &&
                (
                IsEscapableNode(special_UrakawaNode)
                || IsSkippableNode(special_UrakawaNode)
                || (special_UrakawaNode.GetXmlProperty() != null
                &&
                special_UrakawaNode.GetXmlProperty().LocalName.Equals("doctitle", StringComparison.OrdinalIgnoreCase)
                )
                )
                &&
                (
                m_TreeNode_XmlNodeMap[n].Attributes != null
                || m_TreeNode_XmlNodeMap[n.Parent].Attributes != null
                )
                )
                )
            {
                // continue ahead 
            }
            else
            {
                return true;
            }

            if (
                externalAudio != null
                || n.GetFirstAncestorWithManagedAudio() == null
                )
            {
                XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);

                // decide the parent node for this new par node.
                // if node n is child of current specialParentNode than append to it 
                //else check if it has to be appended to parent of this special node in stack or to main seq.
                if (special_UrakawaNode != null && (special_UrakawaNode == n || n.IsDescendantOf(special_UrakawaNode)))
                {
                    //2
                    Seq_SpecialNode.AppendChild(parNode);
                } //-2
                else
                {
                    //2
                    bool IsParNodeAppended = false;
                    string strReferedID = par_id;
                    if (specialParentNodeStack.Count > 0)
                    {
                        //3
                        // check and pop stack till specialParentNode of   iterating node n is found in stack
                        // the loop is also used to assign value of last imidiate seq or par to end attribute of parent seq while pop up
                        while (specialParentNodeStack.Count > 0 && !n.IsDescendantOf(special_UrakawaNode))
                        {
                            //4
                            if (Seq_SpecialNode != null
                                &&
                                strReferedID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                            {
                                //5
                                Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID +
                                                                                       ".end";
                            } //-5
                            strReferedID = Seq_SpecialNode.Attributes.GetNamedItem("id").Value;
                            special_UrakawaNode = specialParentNodeStack.Pop();
                            Seq_SpecialNode = specialSeqNodeStack.Pop();
                        } //-4

                        // if parent of node n is retrieved from stack, apend the par node to it.
                        if (n.IsDescendantOf(special_UrakawaNode))
                        {
                            //4
                            Seq_SpecialNode.AppendChild(parNode);
                            IsParNodeAppended = true;

                        } //-4
                        //System.Windows.Forms.MessageBox.Show ( "par_ id " + par_id + " count " + specialParentNodeStack.Count.ToString ());
                    } // stack > 0 check ends //-3

                    if (specialSeqNodeStack.Count == 0 && !IsParNodeAppended)
                    {
                        //3
                        mainSeq.AppendChild(parNode);
                        special_UrakawaNode = null;
                        if (Seq_SpecialNode != null)
                        {
                            //4
                            //if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem ( "end" ) != null)
                            if (strReferedID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                            {
                                //5
                                //System.Windows.Forms.MessageBox.Show ( par_id == null ? "null" : par_id );
                                //Seq_SpecialNode.Attributes.GetNamedItem ( "end" ).Value = "DTBuserEscape;" + par_id + ".end";
                                Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID +
                                                                                       ".end";
                            } //-5
                            Seq_SpecialNode = null;
                        } //-4
                    } // check of append to main seq ends//-3

                } //-2


                par_id = GetNextID(ID_SmilPrefix);
                // check and assign first par ID
                if (firstPar_id == null)
                {
                    //2
                    if (n.HasXmlProperty && currentHeadingTreeNode != null
                        && (n.IsDescendantOf(currentHeadingTreeNode) || n == currentHeadingTreeNode))
                    {
                        //3
                        firstPar_id = par_id;
                    } //-3
                } //-2

                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);


                XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id", GetNextID(ID_SmilPrefix));
                string dtbookID = m_TreeNode_XmlNodeMap[n].Attributes != null
                                      ? m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value
                                      : m_TreeNode_XmlNodeMap[n.Parent].Attributes.GetNamedItem("id").Value;
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
                                                           m_Filename_Content + "#" + dtbookID);
                parNode.AppendChild(SmilTextNode);
                if (externalAudio != null)
                {
                    //2
                    XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
                                                               FormatTimeString(externalAudio.ClipBegin));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
                                                               FormatTimeString(externalAudio.ClipEnd));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src",
                                                               Path.GetFileName(externalAudio.Src));
                    parNode.AppendChild(audioNode);

                    // add audio file name in audio files list for use in opf creation 
                    string audioFileName = Path.GetFileName(externalAudio.Src);
                    if (!m_FilesList_Audio.Contains(audioFileName)) m_FilesList_Audio.Add(audioFileName);

                    // add to duration 
                    durOfCurrentSmil.Add(externalAudio.Duration);
                } //-2

            }

            //}//-1
            if (n.HasXmlProperty && n.GetXmlElementLocalName() == "sent")
            {
                return false;
            }


            return true;
        },
                delegate(urakawa.core.TreeNode n) { });

            // make specials to null
            special_UrakawaNode = null;
            if (Seq_SpecialNode != null)
            {//1
                if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                {//2
                    Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                }//-2
            }//-1
            while (specialSeqNodeStack.Count > 0)
            {//1
                string str_RefferedSeqID = Seq_SpecialNode.Attributes.GetNamedItem("id").Value;
                Seq_SpecialNode = specialSeqNodeStack.Pop();
                if (str_RefferedSeqID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                    Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + str_RefferedSeqID + ".end";
                //System.Windows.Forms.MessageBox.Show ( "last " + smilFileName + " id " + par_id);
            }//-1
            Seq_SpecialNode = null;

            return durOfCurrentSmil;
        }
    }
}
