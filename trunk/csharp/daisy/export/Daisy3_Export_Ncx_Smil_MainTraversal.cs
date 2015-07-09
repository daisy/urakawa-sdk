using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.core;
using urakawa.media;
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
        private class ProcessLevelNodeData
        {
            public bool isDocTitleAdded;
            public int totalPageCount;
            public int maxNormalPageNumber;
            public int maxDepth;
        }

        private void processLevelNode(
            XmlDocument smilDocument, XmlNode smilBodySeq, string smilFileName, List<string> currentSmilCustomTestList, Time durationOfCurrentSmil,
            ProcessLevelNodeData data,
            TreeNode levelNode,
            Time smilElapseTime,
            List<string> ncxCustomTestList,
            List<XmlNode> playOrderList_Sorted,
            XmlDocument ncxDocument,
            XmlNode ncxRootNode,
            XmlNode navMapNode,
            Dictionary<TreeNode, XmlNode> treeNode_NavNodeMap,
            List<TreeNode> specialParentNodesAddedToNavList
            )
        {
            bool isNcxNativeNodeAdded = false;

            //XmlDocument smilDocument = null;
            //XmlNode smilBodySeq = null;
            //string smilFileName = null;
            //List<string> currentSmilCustomTestList = new List<string>();

            XmlNode navPointNode = null;
            TreeNode currentHeadingTreeNode = null;
            TreeNode specialNode = null;

            XmlNode specialNodeSeq = null;
            //bool IsPageAdded = false;
            string firstPar_id = null;
            string par_id = null;
            Stack<TreeNode> specialNodeStack = new Stack<TreeNode>();
            Stack<XmlNode> specialNodeSeqStack = new Stack<XmlNode>();

            TreeNode.StringChunkRange range = levelNode.GetTextFlattened_();
            if (range == null)
            {
                return;
            }

            bool levelDoesNotContainSignificantText = TreeNode.TextOnlyContainsPunctuation(range);
            if (levelDoesNotContainSignificantText)
            {
                return;
            }

            levelNode.AcceptDepthFirst(
        delegate(TreeNode levelNodeDescendant)
        {
            if (RequestCancellation) return false;

            if (!levelNodeDescendant.HasXmlProperty)
            {
                return false;
            }

            if (smilDocument == null)
            {
                smilDocument = CreateStub_SmilDocument();
                smilBodySeq = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "body", null).FirstChild;
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, smilBodySeq, "id", GetNextID(ID_SmilPrefix));
                smilFileName = GetNextSmilFileName;
                //m_ProgressPercentage += Convert.ToInt32((m_SmilFileNameCounter / m_ListOfLevels.Count) * 100 * 0.7);
                //reportProgress(m_ProgressPercentage, String.Format(UrakawaSDK_daisy_Lang.CreatingSmilFiles, m_SmilFileNameCounter, m_ListOfLevels.Count));
            }

            ExternalAudioMedia externalAudio = GetExternalAudioMedia(levelNodeDescendant);

            bool noAudioInAncestor = (levelNodeDescendant.GetFirstAncestorWithManagedAudio() == null);
            bool noAudioInDescendants = (levelNodeDescendant.GetFirstDescendantWithManagedAudio() == null);

#if ENABLE_SEQ_MEDIA
            bool noAudioLocal = (levelNodeDescendant.GetManagedAudioMediaOrSequenceMedia() == null);
#else
            bool noAudioLocal = (levelNodeDescendant.GetManagedAudioMedia() == null);
#endif
            DebugFix.Assert(noAudioLocal == (externalAudio == null));

            if (!noAudioLocal)
            {
                DebugFix.Assert(noAudioInDescendants && noAudioInAncestor);
            }
            if (!noAudioInAncestor)
            {
                DebugFix.Assert(noAudioInDescendants && noAudioLocal);
            }
            if (!noAudioInDescendants)
            {
                DebugFix.Assert(noAudioInAncestor && noAudioLocal);
            }


            bool isLevelNodeDescendant_Heading = IsHeadingNode(levelNodeDescendant);
            bool isLevelNodeDescendant_Escapable = IsEscapableNode(levelNodeDescendant);
            bool isLevelNodeDescendant_Skippable = IsSkippableNode(levelNodeDescendant);


            bool isSignificantTextWrapperWithNoAudio =
                // levelNodeDescendant.HasXmlProperty IMPLIED
                noAudioLocal //externalAudio == null
                && noAudioInAncestor
                && noAudioInDescendants
                //&& !isLevelNodeDescendant_Skippable
                //&& !isLevelNodeDescendant_Escapable
                &&
                (
                levelNodeDescendant.NeedsAudio()
                ||
                levelNodeDescendant.AtLeastOneChildSiblingIsSignificantTextOnly()
                );


            if (isLevelNodeDescendant_Heading)
            {
                currentHeadingTreeNode = levelNodeDescendant;
            }

            if (//levelNodeDescendant.HasXmlProperty && IMPLIED
                doesTreeNodeTriggerNewSmil(levelNodeDescendant)
                &&
                levelNodeDescendant.GetXmlElementLocalName() != levelNode.GetXmlElementLocalName())
            {
                if (m_ListOfLevels.IndexOf(levelNodeDescendant) > m_ListOfLevels.IndexOf(levelNode))
                {
                    return false;
                }
            }

            bool nodeIsImageAndHasDescriptionProdnotes = m_includeImageDescriptions && m_Image_ProdNoteMap.ContainsKey(levelNodeDescendant);


            if (!data.isDocTitleAdded
                && !isNcxNativeNodeAdded
                && currentHeadingTreeNode != null
                //&& currentHeadingTreeNode.HasXmlProperty IMPLIED!
                && currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle")
            {
                Time currentHeadingTreeNodeDuration = currentHeadingTreeNode.GetDurationOfManagedAudioMediaFlattened();

                if (currentHeadingTreeNodeDuration == null
                    || currentHeadingTreeNodeDuration.AsLocalUnits == 0)
                {
                    CreateDocTitle(ncxDocument, ncxRootNode, levelNodeDescendant, levelNode);
                    data.isDocTitleAdded = true;
                    isNcxNativeNodeAdded = true;
                }
            }



            //Time levelNodeDuration = levelNode.GetDurationOfManagedAudioMediaFlattened();
            //if (currentHeadingTreeNode == null && (levelNodeDuration == null || levelNodeDuration.AsLocalUnits == 0))
            //{
            //    return true;
            //}

            bool specialNodeIsNoteAnnoRef = false;
            bool specialNodeIsNoteAnno = false;


            if (
                // TODO noAudioInAncestor &&
                (specialNode == null || specialNode != levelNodeDescendant)
                &&
                (isLevelNodeDescendant_Heading || isLevelNodeDescendant_Escapable || isLevelNodeDescendant_Skippable || nodeIsImageAndHasDescriptionProdnotes))
            {
                // if this candidate special node is child of existing special node then ad existing special node to stack for nesting.
                if (specialNode != null
                    && specialNodeSeq != null
                    && levelNodeDescendant.IsDescendantOf(specialNode))
                {
                    specialNodeStack.Push(specialNode);
                    specialNodeSeqStack.Push(specialNodeSeq);
                }
                specialNode = levelNodeDescendant;


                if (specialNodeSeq != null)
                {
                    if (par_id != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
                    {
                        specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                    }
                    specialNodeSeq = null;
                }

                specialNodeSeq = smilDocument.CreateElement(null, "seq", smilBodySeq.NamespaceURI);


                specialNodeIsNoteAnnoRef = specialNode.GetXmlElementLocalName() == "noteref" || specialNode.GetXmlElementLocalName() == "annoref";
                specialNodeIsNoteAnno = specialNode.GetXmlElementLocalName() == "note" || specialNode.GetXmlElementLocalName() == "annotation";

                string strSeqID = "";
                // specific handling of IDs for notes for allowing predetermined refered IDs
                if (specialNodeIsNoteAnno
                    &&
                    (
                    !m_generateSmilNoteReferences
                     ||
                     (levelNode != m_Presentation.RootNode && !doesTreeNodeTriggerNewSmil(levelNode))
                    )
                    )
                {
                    strSeqID = ID_SmilPrefix + m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes.GetNamedItem("id").Value
                        //+ "_1"
                        ;
                }
                else
                {
                    strSeqID = GetNextID(ID_SmilPrefix);
                }

                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "id", strSeqID);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "class", nodeIsImageAndHasDescriptionProdnotes ? "prodnote" : specialNode.GetXmlElementLocalName());

                bool isSpecialNodeEscapable = IsEscapableNode(specialNode);
                bool isSpecialNodeSkippable = IsSkippableNode(specialNode);

                if (isSpecialNodeEscapable)
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "end", "");
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "fill", "remove");
                }

                if (isSpecialNodeSkippable || nodeIsImageAndHasDescriptionProdnotes)
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "customTest", nodeIsImageAndHasDescriptionProdnotes ? "prodnote" : specialNode.GetXmlElementLocalName());

                    if (nodeIsImageAndHasDescriptionProdnotes)
                    {
                        if (!currentSmilCustomTestList.Contains("prodnote"))
                        {
                            currentSmilCustomTestList.Add("prodnote");
                            //System.Windows.Forms.MessageBox.Show("prodnote added");
                        }
                    }
                    else if (!currentSmilCustomTestList.Contains(specialNode.GetXmlElementLocalName()))
                    {
                        currentSmilCustomTestList.Add(specialNode.GetXmlElementLocalName());
                    }
                }

                // add smilref reference to seq_special  in dtbook document
                if (isSpecialNodeEscapable || isSpecialNodeSkippable) // || nodeIsImageAndHasDescriptionProdnotes) WE DON'T WANT THE IMAGE ITSELF TO SMIL-REF THE FIRST PRODNOTE (link to Diagram desc XML file)
                {
                    XmlNode dtbEscapableNode = m_TreeNode_XmlNodeMap[specialNode];
                    XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtbEscapableNode, "smilref",
                        FileDataProvider.UriEncode(smilFileName + "#" + strSeqID),
                        m_DTBDocument.DocumentElement.NamespaceURI);
                }
                if (nodeIsImageAndHasDescriptionProdnotes)
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, m_Image_ProdNoteMap[levelNodeDescendant][0], "smilref",
                        FileDataProvider.UriEncode(smilFileName + "#" + strSeqID),
                        m_DTBDocument.DocumentElement.NamespaceURI);
                }
                // decide the parent node to which this new seq node is to be appended.
                if (specialNodeSeqStack.Count == 0)
                {
                    smilBodySeq.AppendChild(specialNodeSeq);
                }
                else
                {
                    specialNodeSeqStack.Peek().AppendChild(specialNodeSeq);
                }
            }

            // condition to continue (otherwise, walk next treenode)
            if (
                !noAudioLocal //externalAudio != null
                // note: externalAudio is null for all following conditions
                || isSignificantTextWrapperWithNoAudio
                || noAudioInAncestor && (
                    nodeIsImageAndHasDescriptionProdnotes
                //|| (levelNodeDescendant.GetTextMedia() != null && !TreeNode.TextOnlyContainsPunctuation(levelNodeDescendant.GetTextFlattened_()))
                    || (
                        levelNodeDescendant.GetTextMedia() != null
                        && specialNode != null
                        &&
                            (
                            IsEscapableNode(specialNode)
                            || IsSkippableNode(specialNode)
                            ||
                //(specialNode.HasXmlProperty && IMPLIED
                            specialNode.GetXmlProperty().LocalName.Equals("doctitle", StringComparison.OrdinalIgnoreCase)
                //)
                            )
                        &&
                            (
                            m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes != null
                            || m_TreeNode_XmlNodeMap[levelNodeDescendant.Parent].Attributes != null
                            )
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

            if (//levelNodeDescendant.HasXmlProperty && ( IMPLIED
                //externalAudio != null
                !noAudioLocal
                //|| noAudioInAncestor
                //)
                //write the element with text but no audio to smil
                || isSignificantTextWrapperWithNoAudio
                )
            {
                XmlNode parNode = smilDocument.CreateElement(null, "par", smilBodySeq.NamespaceURI);

                // decide the parent node for this new par node.
                // if node n is child of current specialParentNode than append to it 
                //else check if it has to be appended to parent of this special node in stack or to main seq.
                if (specialNode != null &&
                    (specialNode == levelNodeDescendant || levelNodeDescendant.IsDescendantOf(specialNode)))
                {
                    specialNodeSeq.AppendChild(parNode);
                }
                else
                {
                    bool IsParNodeAppended = false;
                    string strReferedID = par_id;
                    if (specialNodeStack.Count > 0)
                    {
                        // check and pop stack till specialParentNode of   iterating node n is found in stack
                        // the loop is also used to assign value of last imidiate seq or par to end attribute of parent seq while pop up
                        while (specialNodeStack.Count > 0 && !levelNodeDescendant.IsDescendantOf(specialNode))
                        {
                            if (specialNodeSeq != null
                                &&
                                strReferedID != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
                            {
                                specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" +
                                                                                       strReferedID + ".end";
                            }
                            strReferedID = specialNodeSeq.Attributes.GetNamedItem("id").Value;
                            specialNode = specialNodeStack.Pop();
                            specialNodeSeq = specialNodeSeqStack.Pop();
                        }

                        // if parent of node n is retrieved from stack, apend the par node to it.
                        if (levelNodeDescendant.IsDescendantOf(specialNode))
                        {
                            specialNodeSeq.AppendChild(parNode);
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

                    if (specialNodeSeqStack.Count == 0 && !IsParNodeAppended)
                    {
                        smilBodySeq.AppendChild(parNode);
                        specialNode = null;
                        if (specialNodeSeq != null)
                        {
                            //if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem ( "end" ) != null)
                            if (strReferedID != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
                            {
                                //System.Windows.Forms.MessageBox.Show ( par_id == null ? "null" : par_id );
                                //Seq_SpecialNode.Attributes.GetNamedItem ( "end" ).Value = "DTBuserEscape;" + par_id + ".end";
                                specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" +
                                                                                       strReferedID + ".end";
                            }
                            specialNodeSeq = null;
                        }
                    } // check of append to main seq ends

                }


                par_id = GetNextID(ID_SmilPrefix);
                // check and assign first par ID
                if (firstPar_id == null)
                {
                    if (//levelNodeDescendant.HasXmlProperty && IMPLIED
                        currentHeadingTreeNode != null
                        && (levelNodeDescendant.IsDescendantOf(currentHeadingTreeNode) || levelNodeDescendant == currentHeadingTreeNode))
                    {
                        firstPar_id = par_id;
                    }
                }

                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);


                string dtbookSmilRef = null;
                XmlNode dtBookNode = null;
                if (nodeIsImageAndHasDescriptionProdnotes)
                {
                    dtBookNode = m_Image_ProdNoteMap[levelNodeDescendant][0];
                    XmlNode attr = dtBookNode.Attributes != null ?
                                    dtBookNode.Attributes.GetNamedItem("smilref") : null;
                    dtbookSmilRef = attr != null ? attr.Value : null;
                }
                else
                {
                    dtBookNode = m_TreeNode_XmlNodeMap[levelNodeDescendant];
                    XmlNode attr = dtBookNode.Attributes != null ?
                                    dtBookNode.Attributes.GetNamedItem("smilref") : null;
                    dtbookSmilRef = attr != null ? attr.Value : null;
                }
                if (dtBookNode != null && string.IsNullOrEmpty(dtbookSmilRef))
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtBookNode, "smilref",
                        FileDataProvider.UriEncode(smilFileName + "#" + par_id),
                        m_DTBDocument.DocumentElement.NamespaceURI);
                }
                else
                {
                    bool debug = true;
                }


                XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", smilBodySeq.NamespaceURI);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id",
                                                           GetNextID(ID_SmilPrefix));

                bool isMath = m_TreeNode_XmlNodeMap[levelNodeDescendant].LocalName == DiagramContentModelHelper.Math;
                if (isMath)
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "type", DiagramContentModelHelper.NS_URL_MATHML);
                }

                string dtbookID = null;
                if (nodeIsImageAndHasDescriptionProdnotes)
                {
                    dtbookID = m_Image_ProdNoteMap[levelNodeDescendant][0].Attributes.GetNamedItem("id").Value;
                }
                else
                {
                    dtbookID = m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes != null && m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes.GetNamedItem("id") != null
                                   ? m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes.GetNamedItem("id").Value
                                   : m_TreeNode_XmlNodeMap[levelNodeDescendant.Parent].Attributes.GetNamedItem("id").Value;
                }
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
                                                           FileDataProvider.UriEncode(m_Filename_Content + "#" + dtbookID));
                parNode.AppendChild(SmilTextNode);


                XmlNode smilAnchorNode = null;
                if (specialNodeIsNoteAnnoRef)
                {
                    DebugFix.Assert(parNode.ParentNode == specialNodeSeq);

                    smilAnchorNode = smilDocument.CreateElement(null, "a", specialNodeSeq.NamespaceURI);

                    //specialNodeSeq.AppendChild(smilAnchorNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, smilAnchorNode, "external", "false");
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, smilAnchorNode, "href",
                        "#" + ID_SmilPrefix
                        + m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes.GetNamedItem("idref").Value.Replace("#", "")
                        //+ (m_generateSmilNoteReferences ? "_1" : "")
                        );
                }


                if (externalAudio == null)
                {
                    if (smilAnchorNode != null)
                    {
                        parNode.ParentNode.InsertBefore(smilAnchorNode, parNode);
                    }
                }
                else
                {
                    XmlNode audioNode = smilDocument.CreateElement(null, "audio", smilBodySeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "id",
                                                           GetNextID(ID_SmilPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
                                                               FormatTimeString(externalAudio.ClipBegin));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
                                                               FormatTimeString(externalAudio.ClipEnd));

                    string extAudioSrc = AdjustAudioFileName(externalAudio, levelNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src", FileDataProvider.UriEncode(Path.GetFileName(extAudioSrc)));

                    if (smilAnchorNode != null)
                    {
                        smilAnchorNode.AppendChild(audioNode);
                        parNode.AppendChild(smilAnchorNode);
                    }
                    else
                    {
                        parNode.AppendChild(audioNode);
                    }

                    // add audio file name in audio files list for use in opf creation 
                    string audioFileName = Path.GetFileName(extAudioSrc);
                    if (!m_FilesList_SmilAudio.Contains(audioFileName))
                    {
                        m_FilesList_SmilAudio.Add(audioFileName);
                    }

                    durationOfCurrentSmil.Add(externalAudio.Duration);
                }
            }











            if (//noAudioLocal &&
                nodeIsImageAndHasDescriptionProdnotes)
            {
                CreateSmilNodesForImageDescription(levelNodeDescendant, smilDocument, smilBodySeq, durationOfCurrentSmil, levelNodeDescendant.GetAlternateContentProperty(), smilFileName);
            }






            // if node n is pagenum, add to pageList
            if (//levelNodeDescendant.HasXmlProperty && IMPLIED
                levelNodeDescendant.GetXmlElementLocalName() == "pagenum")
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
                string strTypeVal = levelNodeDescendant.GetXmlProperty().GetAttribute("page").Value;
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "type", strTypeVal);
                string strPageValue = prepareNcxLabelText(levelNodeDescendant);
                data.totalPageCount++;

                playOrderList_Sorted.Add(pageTargetNode);

                if (strTypeVal == "normal")
                {
                    int tmp;
                    bool success = int.TryParse(strPageValue, out tmp);
                    if (success && data.maxNormalPageNumber < tmp)
                    {
                        data.maxNormalPageNumber = tmp;
                    }
                }

                if (strTypeVal == "front")
                {
                    try
                    {
                        int romanInt = Daisy3_Export.ParseRomanToInt(strPageValue);
                        if (romanInt > 0)
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "value",
                                romanInt.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        Console.WriteLine("ROMAN NUMBER PARSE ERROR: " + strPageValue);
                    }
                }
                else if (strTypeVal != "special")
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

                    string extAudioSrc = AdjustAudioFileName(externalAudio, levelNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", FileDataProvider.UriEncode(Path.GetFileName(extAudioSrc)));
                }

                XmlNode contentNode = ncxDocument.CreateElement(null, "content", pageListNode.NamespaceURI);
                pageTargetNode.AppendChild(contentNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", FileDataProvider.UriEncode(smilFileName + "#" + par_id));

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
            else if (//levelNodeDescendant.HasXmlProperty && IMPLIED
                levelNodeDescendant.GetXmlElementLocalName() == DiagramContentModelHelper.Math)
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

                string strMathML = prepareNcxLabelText(levelNodeDescendant); // grabs alttext, if any

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

                    string extAudioSrc = AdjustAudioFileName(externalAudio, levelNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", FileDataProvider.UriEncode(Path.GetFileName(extAudioSrc)));
                }

                XmlNode contentNode = ncxDocument.CreateElement(null, "content", navListNodeMathML.NamespaceURI);
                navTargetNode.AppendChild(contentNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", FileDataProvider.UriEncode(smilFileName + "#" + par_id));

            }
            else if (specialNode != null
              && m_NavListElementNamesList.Contains(specialNode.GetXmlElementLocalName())
                && !specialParentNodesAddedToNavList.Contains(specialNode)

                ||
                nodeIsImageAndHasDescriptionProdnotes)
            {
                string navListNodeName = nodeIsImageAndHasDescriptionProdnotes ? DiagramContentModelHelper.EPUB_DescribedAt : specialNode.GetXmlElementLocalName();
                specialParentNodesAddedToNavList.Add(specialNode);

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
                    ncxDocument.CreateTextNode(prepareNcxLabelText(levelNodeDescendant)));

                // create audio node only if external audio media is not null
                if (externalAudio != null)
                {
                    XmlNode audioNodeNcx = ncxDocument.CreateElement(null, "audio", navTargetNode.NamespaceURI);
                    navLabelNode.AppendChild(audioNodeNcx);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));

                    string extAudioSrc = AdjustAudioFileName(externalAudio, levelNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", FileDataProvider.UriEncode(Path.GetFileName(extAudioSrc)));
                }

                XmlNode contentNode = ncxDocument.CreateElement(null, "content", navTargetNode.NamespaceURI);
                navTargetNode.AppendChild(contentNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", FileDataProvider.UriEncode(smilFileName + "#" + par_id));
            }

            if (!isNcxNativeNodeAdded)
            {
                if (!data.isDocTitleAdded
                    && currentHeadingTreeNode != null
                    //&& currentHeadingTreeNode.HasXmlProperty IMPLIED
                    && currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle")
                {
                    CreateDocTitle(ncxDocument, ncxRootNode, levelNodeDescendant, levelNode);
                    data.isDocTitleAdded = true;
                    isNcxNativeNodeAdded = true;
                }
                else if (currentHeadingTreeNode != null)
                {
                    // find node for heading


                    int indexOf_n = 0;

                    if ( //levelNodeDescendant.HasXmlProperty && ( IMPLIED
                        levelNodeDescendant.IsDescendantOf(currentHeadingTreeNode) || levelNodeDescendant == currentHeadingTreeNode
                        //)
                        )
                    {
                        //indexOf_n = audioNodeIndex;
                        //indexOf_n = 0;
                    }
                    else
                    {
                        return true;
                    }


                    navPointNode = CreateNavPointWithoutContentNode(ncxDocument, levelNode, currentHeadingTreeNode, levelNodeDescendant, treeNode_NavNodeMap);
                    playOrderList_Sorted.Add(navPointNode);

                    // add content node
                    if (firstPar_id != null)
                    {
                        XmlNode contentNode = ncxDocument.CreateElement(null, "content", navMapNode.NamespaceURI);
                        navPointNode.AppendChild(contentNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", FileDataProvider.UriEncode(smilFileName + "#" + firstPar_id));
                    }
                    int navPointDepth = GetDepthOfNavPointNode(ncxDocument, navPointNode);
                    if (data.maxDepth < navPointDepth)
                    {
                        data.maxDepth = navPointDepth;
                    }

                    isNcxNativeNodeAdded = true;
                }
            }

            if (specialNodeIsNoteAnnoRef)
            {
                DebugFix.Assert(specialNode != null);

                TreeNode specialNodeReferredNoteAnno = GetReferedTreeNode(specialNode);
                DebugFix.Assert(specialNodeReferredNoteAnno != null);

                if (specialNodeReferredNoteAnno != null && m_generateSmilNoteReferences)
                {
                    processLevelNode(
                     smilDocument, smilBodySeq, smilFileName, currentSmilCustomTestList, durationOfCurrentSmil,
                    data,
                specialNodeReferredNoteAnno, smilElapseTime, ncxCustomTestList,
                    playOrderList_Sorted,
                    ncxDocument, ncxRootNode, navMapNode, treeNode_NavNodeMap, specialParentNodesAddedToNavList);
                }
            }

            if (//levelNodeDescendant.HasXmlProperty && IMPLIED
                levelNodeDescendant.GetXmlElementLocalName() == "sent"
                    && specialNode != null
                    && specialNodeIsNoteAnno)
            {
                return false;
            }

            if (isSignificantTextWrapperWithNoAudio)
            {
                return false;
            }

            return true;
        },









                delegate(TreeNode n) { }
                );













            // make specials to null
            specialNode = null;
            if (specialNodeSeq != null)
            {
                if (par_id != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
                {
                    specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                }
            }
            while (specialNodeSeqStack.Count > 0)
            {
                string str_RefferedSeqID = specialNodeSeq.Attributes.GetNamedItem("id").Value;
                specialNodeSeq = specialNodeSeqStack.Pop();
                if (str_RefferedSeqID != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
                    specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + str_RefferedSeqID + ".end";
                //System.Windows.Forms.MessageBox.Show ( "last " + smilFileName + " id " + par_id);
            }
            specialNodeSeq = null;

            if (RequestCancellation) return;
            // add metadata to smil document and write to file.
            if (smilDocument != null &&
                (
                levelNode == m_Presentation.RootNode
                //levelNodeDescendant.GetXmlElementLocalName() != levelNode.GetXmlElementLocalName()
                || doesTreeNodeTriggerNewSmil(levelNode)))
            {
                // update duration in seq node
                XmlNode mainSeqNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "body", null).FirstChild; //smilDocument.GetElementsByTagName("body")[0].FirstChild;
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "dur", FormatTimeString(durationOfCurrentSmil));
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "fill", "remove");
                AddMetadata_Smil(smilDocument, FormatTimeString(smilElapseTime), currentSmilCustomTestList);

                XmlReaderWriterHelper.WriteXmlDocument(smilDocument, Path.Combine(m_OutputDirectory, smilFileName), null);

                m_FilesList_Smil.Add(smilFileName);
                smilDocument = null;

                // add smil custon test list items to ncx custom test list
                foreach (string customTestName in currentSmilCustomTestList)
                {
                    if (!ncxCustomTestList.Contains(customTestName))
                        ncxCustomTestList.Add(customTestName);
                }

                smilElapseTime.Add(durationOfCurrentSmil);
            }
        }


        protected virtual void CreateNcxAndSmilDocuments()
        {
            XmlDocument ncxDocument = CreateStub_NcxDocument();

            XmlNode ncxRootNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "ncx", null); //ncxDocument.GetElementsByTagName("ncx")[0];
            XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "navMap", null); //ncxDocument.GetElementsByTagName("navMap")[0];
            Dictionary<TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<TreeNode, XmlNode>();
            m_FilesList_Smil = new List<string>();
            m_FilesList_SmilAudio = new List<string>();
            m_SmilFileNameCounter = 0;
            List<XmlNode> playOrderList_Sorted = new List<XmlNode>();

            Time smilElapseTime = new Time();
            List<string> ncxCustomTestList = new List<string>();
            List<TreeNode> specialParentNodesAddedToNavList = new List<TreeNode>();

            ProcessLevelNodeData data = new ProcessLevelNodeData();
            data.isDocTitleAdded = false;
            data.totalPageCount = 0;
            data.maxNormalPageNumber = 0;
            data.maxDepth = 1;


            //m_ProgressPercentage = 20;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreateSmilAndNcxFiles);

            foreach (TreeNode levelNode in m_ListOfLevels)
            //for ( int nodeCounter = 0 ; nodeCounter < m_ListOfLevels.Count ; nodeCounter++ )
            {
                //TreeNode urakawaNode  = m_ListOfLevels[nodeCounter] ;

                processLevelNode(
                     null, null, null, new List<string>(), new Time(),
                    data,
                levelNode, smilElapseTime, ncxCustomTestList,
                    playOrderList_Sorted,
                    ncxDocument, ncxRootNode, navMapNode, treeNode_NavNodeMap, specialParentNodesAddedToNavList);

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
            XmlReaderWriterHelper.WriteXmlDocument(m_DTBDocument, Path.Combine(m_OutputDirectory, m_Filename_Content), null);

            if (RequestCancellation)
            {
                m_DTBDocument = null;
                ncxDocument = null;
                return;
            }
            // write ncs document to file
            m_TotalTime = new Time(smilElapseTime);
            AddMetadata_Ncx(ncxDocument, data.totalPageCount.ToString(), data.maxNormalPageNumber.ToString(), data.maxDepth.ToString(), ncxCustomTestList);
            XmlReaderWriterHelper.WriteXmlDocument(ncxDocument, Path.Combine(m_OutputDirectory, m_Filename_Ncx),null);
        }
    }
}
