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
        private Time CreateFollowupNoteAndAnnotationNodes(XmlDocument smilDocument, XmlNode mainSeq, TreeNode urakawaNode, string smilFileName, List<string> currentSmilCustomTestList, List<string> ncxCustomTestList)
        {

            //Dictionary<TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<TreeNode, XmlNode>();

            //List<XmlNode> playOrderList_Sorted = new List<XmlNode>();
            //int totalPageCount = 0;
            //int maxNormalPageNumber = 0;
            //int maxDepth = 1;
            //Time smilElapseTime = new Time();
            //List<string> ncxCustomTestList = new List<string> ();
            //List<TreeNode> specialParentNodesAddedToNavList = new List<TreeNode>();
            //m_ProgressPercentage = 20;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreateSmilAndNcxFiles);



            XmlNode navPointNode = null;
            TreeNode currentHeadingTreeNode = null;
            TreeNode special_UrakawaNode = null;
            Time durOfCurrentSmil = new Time();

            XmlNode Seq_SpecialNode = null;
            //bool IsPageAdded = false;
            string firstPar_id = null;
            bool shouldAddNewSeq = false;
            string par_id = null;
            //List<string> currentSmilCustomTestList = new List<string> ();
            Stack<TreeNode> specialParentNodeStack = new Stack<TreeNode>();
            Stack<XmlNode> specialSeqNodeStack = new Stack<XmlNode>();


            urakawaNode.AcceptDepthFirst(
        delegate(TreeNode n)
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

            ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);


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
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href",
                            "#" + ID_SmilPrefix
                            + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("idref").Value.Replace("#", ""));

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
                    XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtbEscapableNode, "smilref",
                        FileDataProvider.UriEncode(smilFileName + "#" + strSeqID),
                        m_DTBDocument.DocumentElement.NamespaceURI);
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
                || (special_UrakawaNode.HasXmlProperty
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
                                                           FileDataProvider.UriEncode(m_Filename_Content + "#" + dtbookID));
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
                                                               FileDataProvider.UriEncode(Path.GetFileName(externalAudio.Src)));
                    parNode.AppendChild(audioNode);

                    // add audio file name in audio files list for use in opf creation 
                    string audioFileName = Path.GetFileName(externalAudio.Src);
                    if (!m_FilesList_SmilAudio.Contains(audioFileName)) m_FilesList_SmilAudio.Add(audioFileName);

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
                delegate(TreeNode n) { });

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
