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
        //private Time OBSOLETE_CreateFollowupNoteAndAnnotationNodes(
        //    XmlDocument smilDocument, XmlNode smilBodySeq, TreeNode specialNodeReferredNoteAnno, string smilFileName,
        //    List<string> currentSmilCustomTestList, List<string> ncxCustomTestList)
        //{

        //    //Dictionary<TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<TreeNode, XmlNode>();

        //    //List<XmlNode> playOrderList_Sorted = new List<XmlNode>();
        //    //int totalPageCount = 0;
        //    //int maxNormalPageNumber = 0;
        //    //int maxDepth = 1;
        //    //Time smilElapseTime = new Time();
        //    //List<string> ncxCustomTestList = new List<string> ();
        //    //List<TreeNode> specialParentNodesAddedToNavList = new List<TreeNode>();
        //    //m_ProgressPercentage = 20;
        //    reportProgress(-1, UrakawaSDK_daisy_Lang.CreateSmilAndNcxFiles);



        //    XmlNode navPointNode = null;
        //    TreeNode currentHeadingTreeNode = null;
        //    TreeNode specialNode = null;
        //    Time durationOfCurrentSmil = new Time();

        //    XmlNode specialNodeSeq = null;
        //    //bool IsPageAdded = false;
        //    string firstPar_id = null;
        //    bool shouldAddNewSeq = false;
        //    string par_id = null;
        //    //List<string> currentSmilCustomTestList = new List<string> ();
        //    Stack<TreeNode> specialNodeStack = new Stack<TreeNode>();
        //    Stack<XmlNode> specialNodeSeqStack = new Stack<XmlNode>();


        //    specialNodeReferredNoteAnno.AcceptDepthFirst(
        //delegate(TreeNode levelNodeDescendant)
        //{//1

        //    if (RequestCancellation) return false;

        //    if (IsHeadingNode(levelNodeDescendant))
        //    {//2
        //        currentHeadingTreeNode = levelNodeDescendant;
        //    }//-2

        //    if (levelNodeDescendant.HasXmlProperty &&
        //            levelNodeDescendant.GetXmlElementLocalName() != specialNodeReferredNoteAnno.GetXmlElementLocalName()
        //            && doesTreeNodeTriggerNewSmil(levelNodeDescendant))
        //    {//2
        //        return false;
        //    }//-2

        //    if ((IsHeadingNode(levelNodeDescendant) || IsEscapableNode(levelNodeDescendant) || IsSkippableNode(levelNodeDescendant))
        //        && (specialNode != levelNodeDescendant))
        //    {//2
        //        // if this candidate special node is child of existing special node then ad existing special node to stack for nesting.
        //        if (specialNode != null && specialNodeSeq != null
        //            && levelNodeDescendant.IsDescendantOf(specialNode))
        //        {
        //            specialNodeStack.Push(specialNode);
        //            specialNodeSeqStack.Push(specialNodeSeq);
        //        }
        //        specialNode = levelNodeDescendant;
        //        shouldAddNewSeq = true;
        //    }//-2

        //    ExternalAudioMedia externalAudio = GetExternalAudioMedia(levelNodeDescendant);


        //    //bool isDoctitle_1 = currentHeadingTreeNode != null && currentHeadingTreeNode.HasXmlProperty &&
        //    //                    currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle";


        //    Time urakawaNodeDur = specialNodeReferredNoteAnno.GetDurationOfManagedAudioMediaFlattened();
        //    if (currentHeadingTreeNode == null && urakawaNodeDur != null && urakawaNodeDur.AsTimeSpan == TimeSpan.Zero)
        //    {
        //        return true;
        //        // carry on processing following lines. and in case this is not true, skip all the following lines
        //    }


        //    //bool isDoctitle_ = currentHeadingTreeNode != null && currentHeadingTreeNode.HasXmlProperty &&
        //    //                    currentHeadingTreeNode.GetXmlElementLocalName() == "doctitle";


        //    // create smil stub document


        //    // create smil nodes

        //    if (shouldAddNewSeq)
        //    {//2
        //        if (specialNodeSeq != null)
        //        {//3
        //            if (par_id != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
        //            {//4
        //                specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
        //            }//-4
        //            specialNodeSeq = null;
        //        }//-3

        //        specialNodeSeq = smilDocument.CreateElement(null, "seq", smilBodySeq.NamespaceURI);
        //        string strSeqID = "";
        //        // specific handling of IDs for notes for allowing predetermined refered IDs
        //        if (specialNode.GetXmlElementLocalName() == "note" || specialNode.GetXmlElementLocalName() == "annotation")
        //        {//3
        //            strSeqID = ID_SmilPrefix + m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes.GetNamedItem("id").Value + "_1";
        //        }//-3
        //        else
        //        {//3
        //            strSeqID = GetNextID(ID_SmilPrefix);
        //        }//-3
        //        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "id", strSeqID);
        //        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "class", specialNode.GetXmlElementLocalName());

        //        if (IsEscapableNode(specialNode))
        //        {//3
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "end", "");
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "fill", "remove");
        //        }//-3

        //        if (IsSkippableNode(specialNode))
        //        {//3
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, specialNodeSeq, "customTest", specialNode.GetXmlElementLocalName());

        //            if (specialNode.GetXmlElementLocalName() == "noteref" || specialNode.GetXmlElementLocalName() == "annoref")
        //            {//4
        //                XmlNode anchorNode = smilDocument.CreateElement(null, "a", specialNodeSeq.NamespaceURI);
        //                specialNodeSeq.AppendChild(anchorNode);
        //                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "external", "false");
        //                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href",
        //                    "#" + ID_SmilPrefix
        //                    + m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes.GetNamedItem("idref").Value.Replace("#", ""));

        //            }//-4
        //            if (!currentSmilCustomTestList.Contains(specialNode.GetXmlElementLocalName()))
        //            {//4
        //                currentSmilCustomTestList.Add(specialNode.GetXmlElementLocalName());
        //            }//-4
        //        }//-3

        //        // add smilref reference to seq_special  in dtbook document
        //        if (IsEscapableNode(specialNode) || IsSkippableNode(specialNode))
        //        {//3
        //            XmlNode dtbEscapableNode = m_TreeNode_XmlNodeMap[specialNode];
        //            XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtbEscapableNode, "smilref", smilFileName + "#" + strSeqID, m_DTBDocument.DocumentElement.NamespaceURI);
        //        }//-3

        //        // decide the parent node to which this new seq node is to be appended.
        //        if (specialNodeSeqStack.Count == 0)
        //        {//3
        //            smilBodySeq.AppendChild(specialNodeSeq);
        //        }//-3
        //        else
        //        {//3
        //            specialNodeSeqStack.Peek().AppendChild(specialNodeSeq);
        //        }//-3

        //        shouldAddNewSeq = false;
        //    }//-2

        //    if (
        //        externalAudio != null
        //        ||
        //        (
        //        levelNodeDescendant.GetTextMedia() != null
        //        &&
        //        specialNode != null
        //        &&
        //        (
        //        IsEscapableNode(specialNode)
        //        || IsSkippableNode(specialNode)
        //        || (specialNode.HasXmlProperty
        //        &&
        //        specialNode.GetXmlProperty().LocalName.Equals("doctitle", StringComparison.OrdinalIgnoreCase)
        //        )
        //        )
        //        &&
        //        (
        //        m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes != null
        //        || m_TreeNode_XmlNodeMap[levelNodeDescendant.Parent].Attributes != null
        //        )
        //        )
        //        )
        //    {
        //        // continue ahead 
        //    }
        //    else
        //    {
        //        return true;
        //    }

        //    if (
        //        externalAudio != null
        //        || levelNodeDescendant.GetFirstAncestorWithManagedAudio() == null
        //        )
        //    {
        //        XmlNode parNode = smilDocument.CreateElement(null, "par", smilBodySeq.NamespaceURI);

        //        // decide the parent node for this new par node.
        //        // if node n is child of current specialParentNode than append to it 
        //        //else check if it has to be appended to parent of this special node in stack or to main seq.
        //        if (specialNode != null && (specialNode == levelNodeDescendant || levelNodeDescendant.IsDescendantOf(specialNode)))
        //        {
        //            //2
        //            specialNodeSeq.AppendChild(parNode);
        //        } //-2
        //        else
        //        {
        //            //2
        //            bool IsParNodeAppended = false;
        //            string strReferedID = par_id;
        //            if (specialNodeStack.Count > 0)
        //            {
        //                //3
        //                // check and pop stack till specialParentNode of   iterating node n is found in stack
        //                // the loop is also used to assign value of last imidiate seq or par to end attribute of parent seq while pop up
        //                while (specialNodeStack.Count > 0 && !levelNodeDescendant.IsDescendantOf(specialNode))
        //                {
        //                    //4
        //                    if (specialNodeSeq != null
        //                        &&
        //                        strReferedID != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
        //                    {
        //                        //5
        //                        specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID +
        //                                                                               ".end";
        //                    } //-5
        //                    strReferedID = specialNodeSeq.Attributes.GetNamedItem("id").Value;
        //                    specialNode = specialNodeStack.Pop();
        //                    specialNodeSeq = specialNodeSeqStack.Pop();
        //                } //-4

        //                // if parent of node n is retrieved from stack, apend the par node to it.
        //                if (levelNodeDescendant.IsDescendantOf(specialNode))
        //                {
        //                    //4
        //                    specialNodeSeq.AppendChild(parNode);
        //                    IsParNodeAppended = true;

        //                } //-4
        //                //System.Windows.Forms.MessageBox.Show ( "par_ id " + par_id + " count " + specialParentNodeStack.Count.ToString ());
        //            } // stack > 0 check ends //-3

        //            if (specialNodeSeqStack.Count == 0 && !IsParNodeAppended)
        //            {
        //                //3
        //                smilBodySeq.AppendChild(parNode);
        //                specialNode = null;
        //                if (specialNodeSeq != null)
        //                {
        //                    //4
        //                    //if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem ( "end" ) != null)
        //                    if (strReferedID != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
        //                    {
        //                        //5
        //                        //System.Windows.Forms.MessageBox.Show ( par_id == null ? "null" : par_id );
        //                        //Seq_SpecialNode.Attributes.GetNamedItem ( "end" ).Value = "DTBuserEscape;" + par_id + ".end";
        //                        specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID +
        //                                                                               ".end";
        //                    } //-5
        //                    specialNodeSeq = null;
        //                } //-4
        //            } // check of append to main seq ends//-3

        //        } //-2


        //        par_id = GetNextID(ID_SmilPrefix);
        //        // check and assign first par ID
        //        if (firstPar_id == null)
        //        {
        //            //2
        //            if (levelNodeDescendant.HasXmlProperty && currentHeadingTreeNode != null
        //                && (levelNodeDescendant.IsDescendantOf(currentHeadingTreeNode) || levelNodeDescendant == currentHeadingTreeNode))
        //            {
        //                //3
        //                firstPar_id = par_id;
        //            } //-3
        //        } //-2

        //        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);


        //        XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", smilBodySeq.NamespaceURI);
        //        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id", GetNextID(ID_SmilPrefix));
        //        string dtbookID = m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes != null
        //                              ? m_TreeNode_XmlNodeMap[levelNodeDescendant].Attributes.GetNamedItem("id").Value
        //                              : m_TreeNode_XmlNodeMap[levelNodeDescendant.Parent].Attributes.GetNamedItem("id").Value;
        //        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
        //                                                   FileDataProvider.UriEncode(m_Filename_Content + "#" + dtbookID));
        //        parNode.AppendChild(SmilTextNode);
        //        if (externalAudio != null)
        //        {
        //            //2
        //            XmlNode audioNode = smilDocument.CreateElement(null, "audio", smilBodySeq.NamespaceURI);
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
        //                                                       FormatTimeString(externalAudio.ClipBegin));
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
        //                                                       FormatTimeString(externalAudio.ClipEnd));
        //string extAudioSrc = AdjustAudioFileName(externalAudio, levelNodeDescendant);
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src",
        //                                                       FileDataProvider.UriEncode(Path.GetFileName(extAudioSrc)));
        //            parNode.AppendChild(audioNode);

        //            // add audio file name in audio files list for use in opf creation 
        //            string audioFileName = Path.GetFileName(extAudioSrc);
        //            if (!m_FilesList_SmilAudio.Contains(audioFileName)) m_FilesList_SmilAudio.Add(audioFileName);

        //            // add to duration 
        //            durationOfCurrentSmil.Add(externalAudio.Duration);
        //        } //-2

        //    }

        //    //}//-1
        //    if (levelNodeDescendant.HasXmlProperty && levelNodeDescendant.GetXmlElementLocalName() == "sent")
        //    {
        //        return false;
        //    }


        //    return true;
        //},
        //        delegate(TreeNode n) { });

        //    // make specials to null
        //    specialNode = null;
        //    if (specialNodeSeq != null)
        //    {//1
        //        if (par_id != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
        //        {//2
        //            specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
        //        }//-2
        //    }//-1
        //    while (specialNodeSeqStack.Count > 0)
        //    {//1
        //        string str_RefferedSeqID = specialNodeSeq.Attributes.GetNamedItem("id").Value;
        //        specialNodeSeq = specialNodeSeqStack.Pop();
        //        if (str_RefferedSeqID != null && specialNodeSeq.Attributes.GetNamedItem("end") != null)
        //            specialNodeSeq.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + str_RefferedSeqID + ".end";
        //        //System.Windows.Forms.MessageBox.Show ( "last " + smilFileName + " id " + par_id);
        //    }//-1
        //    specialNodeSeq = null;

        //    return durationOfCurrentSmil;
        //}
    
    }
}
