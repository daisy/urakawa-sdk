﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml ;
using System.IO;

using urakawa.core ;
using urakawa.xuk;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {

        private Dictionary<string, TreeNode> m_SmilRefToNavPointTreeNodeMap;
        protected Dictionary<TreeNode, XmlNode> m_NavPointNode_NavLabelMap;
        private Dictionary<string, XmlNode> m_PageReferencesMapDictionaryForNCX;
        protected Dictionary<XmlNode, TreeNode> m_SmilXmlNodeToTreeNodeMap;
        protected XmlNode m_DocTitleXmlNode;
        private bool m_IsDocTitleCreated = false;

        public void ParseNCXDocument(XmlDocument ncxDocument)
        {
            m_SmilRefToNavPointTreeNodeMap = new Dictionary<string, TreeNode>();
            m_NavPointNode_NavLabelMap = new Dictionary<TreeNode, XmlNode>();
            //string ncxPath = Directory.GetFiles(Directory.GetParent(m_Book_FilePath).FullName, "*.ncx")[0];
            //ncxDocument = xuk.OpenXukAction.ParseXmlDocument(ncxPath, false);

            XmlNode navMap = ncxDocument.GetElementsByTagName("navMap")[0];
            Presentation presentation = m_Project.Presentations.Get(0);
            XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
            xmlProp.LocalName = "book";
            presentation.PropertyFactory.DefaultXmlNamespaceUri = navMap.NamespaceURI;
            xmlProp.NamespaceUri = presentation.PropertyFactory.DefaultXmlNamespaceUri;
            TreeNode treeNode = null;
            if (presentation.RootNode == null)
            {
                treeNode = presentation.TreeNodeFactory.Create();
                presentation.RootNode = treeNode;
            }
            else
            {
                treeNode = presentation.RootNode;
            }
            treeNode.AddProperty(xmlProp);

            m_DocTitleXmlNode  = XmlDocumentHelper.GetFirstChildElementWithName(ncxDocument.DocumentElement, true, "docTitle", ncxDocument.DocumentElement.NamespaceURI);
            m_IsDocTitleCreated = false;

            ParseNCXNodes(presentation, navMap, treeNode);
            CollectPagesFromPageList(navMap);
        }

        
        private void CollectPagesFromPageList(XmlNode navMap)
        {
            m_PageReferencesMapDictionaryForNCX  = new Dictionary<string, XmlNode>();
            
            //XmlNode pageListNode =  XmlDocumentHelper.GetFirstChildElementWithName(navMap, true, "pageList", navMap.NamespaceURI);
            XmlNode pageListNode = navMap.OwnerDocument.GetElementsByTagName("pageList")[0];
            if (pageListNode != null)
            {
                
                foreach (XmlNode pageTargetNode in XmlDocumentHelper.GetChildrenElementsWithName(pageListNode, true, "pageTarget", navMap.NamespaceURI, false))
                {
                    XmlNode contentNode = XmlDocumentHelper.GetFirstChildElementWithName(pageTargetNode, true, "content", pageTargetNode.NamespaceURI);
                    m_PageReferencesMapDictionaryForNCX.Add(contentNode.Attributes.GetNamedItem("src").Value , pageTargetNode);
                    
                }
            }
        }

        private void ParseNCXNodes(Presentation presentation, XmlNode node, TreeNode tNode)
        {
            //if (node.ChildNodes == null || node.ChildNodes.Count == 0)
            //{
                //System.Windows.Forms.MessageBox.Show("returning " + node.LocalName);
                //return;
            //}
            //else
            //{
                //System.Windows.Forms.MessageBox.Show("working " + node.LocalName);
            TreeNode treeNode = null;
                if (node.LocalName == "navPoint")
                {
                    treeNode = CreateTreeNodeForNavPoint(tNode, node);
                    
                    XmlNode contentNode = XmlDocumentHelper.GetFirstChildElementWithName(node, true, "content", node.NamespaceURI);
                    m_SmilRefToNavPointTreeNodeMap.Add(contentNode.Attributes.GetNamedItem("src").Value, treeNode);

                    XmlNode navLabelXmlNode = XmlDocumentHelper.GetFirstChildElementWithName(node, true, "navLabel", node.NamespaceURI);
                    if (navLabelXmlNode != null) m_NavPointNode_NavLabelMap.Add(treeNode, navLabelXmlNode);
                }
            
                if (node.ChildNodes == null || node.ChildNodes.Count == 0) return;
                foreach (XmlNode n in node.ChildNodes)
                {
                    ParseNCXNodes(presentation, n, treeNode!= null? treeNode: tNode);
                }
            //}
        }

        protected virtual TreeNode CreateTreeNodeForNavPoint(TreeNode parentNode, XmlNode navPoint )
        {
            TreeNode treeNode = parentNode.Presentation.TreeNodeFactory.Create();
            if (navPoint.LocalName == "navPoint")
            {
                parentNode.AppendChild(treeNode);
                XmlProperty xmlProp = parentNode.Presentation.PropertyFactory.CreateXmlProperty();
                treeNode.AddProperty(xmlProp);
                //XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navPoint, true, "text", navPoint.NamespaceURI);
                xmlProp.LocalName = "level";//+":" + textNode.InnerText;
            }
            else if (navPoint.LocalName == "docTitle")
            {
                Presentation pres = Project.Presentations.Get(0) ;
                pres.RootNode.Insert (treeNode,0);
                XmlProperty xmlProp = pres.PropertyFactory.CreateXmlProperty();
                treeNode.AddProperty(xmlProp);
                //XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navPoint, true, "text", navPoint.NamespaceURI);
                xmlProp.LocalName = "doctitle";
            }
                    // create urakawa tree node
                    
                    //TextMedia textMedia = parentNode.Presentation.MediaFactory.CreateTextMedia();
                    //textMedia.Text = textNode.InnerText;

                    //ChannelsProperty cProp = parentNode.Presentation.PropertyFactory.CreateChannelsProperty();
                    //cProp.SetMedia(m_textChannel, textMedia);
            
                    //TreeNode txtWrapperNode = parentNode.Presentation.TreeNodeFactory.Create();
                    //txtWrapperNode.AddProperty(cProp);
                    //treeNode.AppendChild(txtWrapperNode);

                    //XmlProperty TextNodeXmlProp = parentNode.Presentation.PropertyFactory.CreateXmlProperty();
                    //txtWrapperNode.AddProperty(TextNodeXmlProp);
                    //TextNodeXmlProp.LocalName = "hd";
            
                    return treeNode;
        }


        private void parseSmilForNCX(string fullSmilPath)
        {
            //foreach (string s in m_SmilRefToNavPointTreeNodeMap.Keys) System.Windows.Forms.MessageBox.Show(s + " : " + m_SmilRefToNavPointTreeNodeMap[s].GetXmlElementQName().LocalName );
            if (RequestCancellation) return;
            XmlDocument smilXmlDoc = OpenXukAction.ParseXmlDocument(fullSmilPath, false);

            if (RequestCancellation) return;
            m_SmilXmlNodeToTreeNodeMap = new Dictionary<XmlNode, TreeNode>();
            //we skip SMIL metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            //parseMetadata(smilXmlDoc);

            //XmlNodeList allTextNodes = smilXmlDoc.GetElementsByTagName("text");
            //if (allTextNodes.Count == 0)
            //{
            //    return;
            //}

            //reportProgress(-1, "Parsing SMIL: [" + Path.GetFileName(fullSmilPath) + "]");
            TreeNode navPointTreeNode = null;

            XmlNamespaceManager firstDocNSManager = new XmlNamespaceManager(smilXmlDoc.NameTable);
            firstDocNSManager.AddNamespace("firstNS",
                smilXmlDoc.DocumentElement.NamespaceURI);
            bool isHeading = false;
            bool isPageInProcess = false;
            TreeNode audioWrapperNode = null;
            

            XmlNodeList smilNodeList = smilXmlDoc.DocumentElement.SelectNodes(".//firstNS:seq | .//firstNS:par",
                        firstDocNSManager);
            //foreach (XmlNode parNode in XmlDocumentHelper.GetChildrenElementsWithName(smilXmlDoc, true, "par", null, false))
            foreach (XmlNode parNode in smilNodeList)
            {   
                XmlAttributeCollection parNodeAttrs = parNode.Attributes;
                if (parNodeAttrs == null || parNodeAttrs.Count == 0)
                {
                    continue;
                }
                //XmlNode textNodeAttrSrc = parNodeAttrs.GetNamedItem("src");
                XmlNode parNodeID = parNodeAttrs.GetNamedItem("id");
                if (parNodeID == null || String.IsNullOrEmpty(parNodeID.Value))
                {
                    continue;
                }
                
                
                string ncxContentSRC = Path.GetFileName(fullSmilPath) + "#" + parNodeID.Value;
                
                // for now we are assuming the first phrase as heading phrase. this need refinement such that phrase anywhere in section can be imported as heading
                if (m_SmilRefToNavPointTreeNodeMap.ContainsKey(ncxContentSRC))
                {
                    m_IsDocTitleCreated = true;
                    navPointTreeNode = m_SmilRefToNavPointTreeNodeMap[ncxContentSRC];
                    //System.Windows.Forms.MessageBox.Show(ncxContentSRC + " section:" + navPointTreeNode.GetXmlElementQName().LocalName + " : " + Path.GetFileName( fullSmilPath ) );
                    //: audioWrapperNode =  CreateTreeNodeForAudioNode(navPointTreeNode, true);
                    //isHeading = true;
                    
                }

                // handle doctitle if audio exists before first heading by adding doctitle node as first treenode
                if (navPointTreeNode == null && !m_IsDocTitleCreated
                    &&    (XmlDocumentHelper.GetFirstChildElementWithName(parNode, false, "audio", null) != null    ||    XmlDocumentHelper.GetFirstChildElementWithName(parNode, false, "text", null) != null))
                {
                    m_IsDocTitleCreated = true;
                    navPointTreeNode = CreateTreeNodeForNavPoint(m_Project.Presentations.Get(0).RootNode, m_DocTitleXmlNode);
                    m_NavPointNode_NavLabelMap.Add(navPointTreeNode, m_DocTitleXmlNode);
                    
                }
                //else if (m_PageReferencesMapDictionaryForNCX.ContainsKey(ncxContentSRC)
                if (m_PageReferencesMapDictionaryForNCX.ContainsKey(ncxContentSRC)
                    ||    (parNode.Attributes.GetNamedItem("customTest") != null && parNode.Attributes.GetNamedItem("customTest").Value == "pagenum"))
                {
                    isPageInProcess = true;
                    if (navPointTreeNode == null) continue;
                    //:audioWrapperNode = CreateTreeNodeForAudioNode(navPointTreeNode, false);
                    
                }
                
                
                 
                if (navPointTreeNode == null) continue;
                
                    //System.Windows.Forms.MessageBox.Show(parNode.LocalName);
                    
                AbstractAudioMedia textTreeNodeAudio = navPointTreeNode.GetAudioMedia();
                if (textTreeNodeAudio != null)
                {
                    //Ignore.
                    continue;
                }
                
                
                XmlNodeList textPeers = parNode.ChildNodes;
                foreach (XmlNode textPeerNode in textPeers)
                //foreach (XmlNode textPeerNode in XmlDocumentHelper.GetChildrenElementsWithName(parNode, true, "audio", null, false))
                {
                    if (RequestCancellation) return;

                    //if (XmlDocumentHelper.GetFirstChildElementWithName(parNode, false, "audio", null) == null) continue;
                    if (textPeerNode.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }
                    if (textPeerNode.LocalName == "audio")
                    {   
                    audioWrapperNode = CreateTreeNodeForAudioNode(navPointTreeNode, isHeading, textPeerNode);
                    CheckAndAssignForHeadingAudio(navPointTreeNode, audioWrapperNode, textPeerNode);
                    XmlProperty xmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
                    audioWrapperNode.AddProperty(xmlProp);
                    xmlProp.LocalName = "phrase"; // +":" + navPointTreeNode.GetTextFlattened(false);
                
                    string pageRefInSmil = Path.GetFileName(fullSmilPath) + "#" + parNode.Attributes.GetNamedItem("id").Value;

                    if (m_PageReferencesMapDictionaryForNCX.ContainsKey(pageRefInSmil) && isPageInProcess)
                    {
                        isPageInProcess = false;
                        XmlNode pageTargetNode = m_PageReferencesMapDictionaryForNCX[pageRefInSmil];

                        AddPagePropertiesToAudioNode(audioWrapperNode, pageTargetNode);
                    }
                    isHeading = false;

                    
                        addAudio(audioWrapperNode, textPeerNode, false, fullSmilPath);
                        break;
                    }
                    else if (textPeerNode.LocalName == "a")
                    {

                        XmlNodeList aChildren = textPeerNode.ChildNodes;
                        foreach (XmlNode aChild in aChildren)
                        {
                            if (aChild.LocalName == "audio")
                            {
                                addAudio(audioWrapperNode, aChild, false, fullSmilPath);
                                break;
                            }
                        }
                    }
                    else if (textPeerNode.LocalName == "seq" || textPeerNode.LocalName == "par")
                    {
                        if (audioWrapperNode != null) m_SmilXmlNodeToTreeNodeMap.Add(textPeerNode, audioWrapperNode);
                  /*      
                        XmlNodeList seqChildren = textPeerNode.ChildNodes;
                        foreach (XmlNode seqChild in seqChildren)
                        //1{
                            if (seqChild.LocalName == "audio")
                            {//2
                                addAudio(audioWrapperNode, seqChild, true, fullSmilPath);
                            }//-2
                        }//-1
                        
                        SequenceMedia seqManAudioMedia = audioWrapperNode.GetManagedAudioSequenceMedia();
                        if (seqManAudioMedia == null)
                        {//1
                            //Debug.Fail("This should never happen !");
                            break;
                        }//-1

                        ManagedAudioMedia managedAudioMedia = audioWrapperNode.Presentation.MediaFactory.CreateManagedAudioMedia();
                        AudioMediaData mediaData = audioWrapperNode.Presentation.MediaDataFactory.CreateAudioMediaData();
                        managedAudioMedia.AudioMediaData = mediaData;

                        foreach (Media seqChild in seqManAudioMedia.ChildMedias.ContentsAs_YieldEnumerable)
                        {//1
                            ManagedAudioMedia seqManMedia = (ManagedAudioMedia)seqChild;
                            mediaData.MergeWith(seqManMedia.AudioMediaData);
                        }//-1
                         
                        ChannelsProperty chProp = audioWrapperNode.GetChannelsProperty();
                        chProp.SetMedia(m_audioChannel, null);
                        chProp.SetMedia(m_audioChannel, managedAudioMedia);
                */    
                        break;
                          
                    }
                
                }
            }
        }

        protected virtual TreeNode CreateTreeNodeForAudioNode(TreeNode navPointTreeNode ,  bool isHeadingNode, XmlNode smilNode)
        {
            TreeNode audioWrapperNode = null;
            if (isHeadingNode)
            {
                foreach (TreeNode txtNode in navPointTreeNode.Children.ContentsAs_YieldEnumerable)
                {
                    if (txtNode.GetTextMedia() != null)
                    {
                        audioWrapperNode = txtNode;
                        break;
                    }
                }
            }
            else
            {
                if (navPointTreeNode == null) return null;
                audioWrapperNode = navPointTreeNode.Presentation.TreeNodeFactory.Create();

                if (smilNode == null || !m_SmilXmlNodeToTreeNodeMap.ContainsKey(smilNode))
                {
                    navPointTreeNode.AppendChild(audioWrapperNode);
                }
                else
                {
                    navPointTreeNode.InsertAfter(audioWrapperNode, m_SmilXmlNodeToTreeNodeMap[smilNode]);
                    m_SmilXmlNodeToTreeNodeMap[smilNode] = audioWrapperNode;
                }
            }
            //XmlProperty xmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
            //audioWrapperNode.AddProperty(xmlProp);
            //xmlProp.LocalName = "phrase"; // +":" + navPointTreeNode.GetTextFlattened(false);
            return audioWrapperNode;
        }

        protected virtual void AddPagePropertiesToAudioNode(TreeNode audioWrapperNode, XmlNode pageTargetNode)
        {
            TextMedia textMedia = audioWrapperNode.Presentation.MediaFactory.CreateTextMedia();
            textMedia.Text = XmlDocumentHelper.GetFirstChildElementWithName(pageTargetNode, true, "text", pageTargetNode.NamespaceURI).InnerText;
            ChannelsProperty cProp = audioWrapperNode.Presentation.PropertyFactory.CreateChannelsProperty();
            cProp.SetMedia(m_textChannel, textMedia);
            audioWrapperNode.AddProperty(cProp);
            System.Xml.XmlAttributeCollection pageAttributes = pageTargetNode.Attributes;
            if (pageAttributes != null)
            {
                XmlProperty xmlProp = audioWrapperNode.GetXmlProperty();
                xmlProp.LocalName = "pagenum";
                foreach (System.Xml.XmlAttribute attr in pageAttributes)
                {
                    xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                }
            }

        }


        protected virtual TreeNode CheckAndAssignForHeadingAudio(TreeNode navPointTreeNode, TreeNode phraseTreeNode, XmlNode audioXmlNode)
        {
            
            XmlNode navLabelXmlNode = m_NavPointNode_NavLabelMap[navPointTreeNode];
            XmlNode headingAudio = XmlDocumentHelper.GetFirstChildElementWithName(navLabelXmlNode, true, "audio", navLabelXmlNode.NamespaceURI);
            XmlNode textNode  = XmlDocumentHelper.GetFirstChildElementWithName(navLabelXmlNode, true, "text", navLabelXmlNode.NamespaceURI);

            double headingClipBegin = Math.Abs ((new urakawa.media.timing.Time (headingAudio.Attributes.GetNamedItem("clipBegin").Value)).AsTimeSpan.TotalMilliseconds);
            double headingClipEnd = Math.Abs ((new urakawa.media.timing.Time(headingAudio.Attributes.GetNamedItem("clipEnd").Value)).AsTimeSpan.TotalMilliseconds );

            double audioClipBegin = Math.Abs ((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipBegin").Value)).AsTimeSpan.TotalMilliseconds);
            double audioClipEnd = Math.Abs ((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipEnd").Value)).AsTimeSpan.TotalMilliseconds);

            if (headingAudio.Attributes.GetNamedItem("src").Value == audioXmlNode.Attributes.GetNamedItem("src").Value
                && Math.Abs(headingClipBegin - audioClipBegin) <= 1
                && Math.Abs(headingClipEnd - audioClipEnd) <= 1)
            {
                TextMedia textMedia = navPointTreeNode.Presentation.MediaFactory.CreateTextMedia();
                textMedia.Text = textNode.InnerText;

                ChannelsProperty cProp = navPointTreeNode.Presentation.PropertyFactory.CreateChannelsProperty();
                cProp.SetMedia(m_textChannel, textMedia);

                //TreeNode txtWrapperNode = parentNode.Presentation.TreeNodeFactory.Create();
                phraseTreeNode.AddProperty(cProp);
                //treeNode.AppendChild(txtWrapperNode);

                XmlProperty TextNodeXmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
                phraseTreeNode.AddProperty(TextNodeXmlProp);
                TextNodeXmlProp.LocalName = "hd";
            }
              
            return phraseTreeNode;
        }

    }
}
