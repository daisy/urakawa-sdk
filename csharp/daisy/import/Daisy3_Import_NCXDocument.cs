using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using urakawa.core;
using urakawa.data;
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

        public virtual void ParseNCXDocument(XmlDocument ncxDocument)
        {
            m_SmilRefToNavPointTreeNodeMap = new Dictionary<string, TreeNode>();
            m_NavPointNode_NavLabelMap = new Dictionary<TreeNode, XmlNode>();
            //string ncxPath = Directory.GetFiles(Directory.GetParent(m_Book_FilePath).FullName, "*.ncx")[0];
            //ncxDocument = xuk.OpenXukAction.ParseXmlDocument(ncxPath, false);

            XmlNode navMap = ncxDocument.GetElementsByTagName("navMap")[0];
            Presentation presentation = m_Project.Presentations.Get(0);

            XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
            xmlProp.SetQName("book", navMap.NamespaceURI == null ? "" : navMap.NamespaceURI);

            presentation.PropertyFactory.DefaultXmlNamespaceUri = navMap.NamespaceURI;

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

            m_DocTitleXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument.DocumentElement, true, "docTitle", ncxDocument.DocumentElement.NamespaceURI);
            m_IsDocTitleCreated = false;

            ParseNCXNodes(presentation, navMap, treeNode);
            CollectPagesFromPageList(navMap);
        }


        private void CollectPagesFromPageList(XmlNode navMap)
        {
            m_PageReferencesMapDictionaryForNCX = new Dictionary<string, XmlNode>();

            //XmlNode pageListNode =  XmlDocumentHelper.GetFirstChildElementWithName(navMap, true, "pageList", navMap.NamespaceURI);
            XmlNode pageListNode = navMap.OwnerDocument.GetElementsByTagName("pageList")[0];
            if (pageListNode != null)
            {

                foreach (XmlNode pageTargetNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(pageListNode, true, "pageTarget", navMap.NamespaceURI, false))
                {
                    XmlNode contentNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(pageTargetNode, true, "content", pageTargetNode.NamespaceURI);

                    string urlDecoded = FileDataProvider.UriDecode(contentNode.Attributes.GetNamedItem("src").Value);
                    m_PageReferencesMapDictionaryForNCX.Add(urlDecoded, pageTargetNode);

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

                XmlNode contentNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(node, true, "content", node.NamespaceURI);


                string urlDecoded = FileDataProvider.UriDecode(contentNode.Attributes.GetNamedItem("src").Value);
                m_SmilRefToNavPointTreeNodeMap.Add(urlDecoded, treeNode);

                XmlNode navLabelXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(node, true, "navLabel", node.NamespaceURI);
                if (navLabelXmlNode != null) m_NavPointNode_NavLabelMap.Add(treeNode, navLabelXmlNode);
            }

            if (node.ChildNodes == null || node.ChildNodes.Count == 0) return;
            foreach (XmlNode n in node.ChildNodes)
            {
                ParseNCXNodes(presentation, n, treeNode != null ? treeNode : tNode);
            }
            //}
        }

        protected virtual TreeNode CreateTreeNodeForNavPoint(TreeNode parentNode, XmlNode navPoint)
        {
            TreeNode treeNode = parentNode.Presentation.TreeNodeFactory.Create();
            if (navPoint.LocalName == "navPoint")
            {
                parentNode.AppendChild(treeNode);
                XmlProperty xmlProp = parentNode.Presentation.PropertyFactory.CreateXmlProperty();
                treeNode.AddProperty(xmlProp);
                //XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navPoint, true, "text", navPoint.NamespaceURI);

                //+":" + textNode.InnerText;
                xmlProp.SetQName("level", "");
            }
            else if (navPoint.LocalName == "docTitle")
            {
                Presentation pres = m_Project.Presentations.Get(0);
                pres.RootNode.Insert(treeNode, 0);
                XmlProperty xmlProp = pres.PropertyFactory.CreateXmlProperty();
                treeNode.AddProperty(xmlProp);
                //XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navPoint, true, "text", navPoint.NamespaceURI);

                xmlProp.SetQName("doctitle", "");
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
            //foreach (string s in m_SmilRefToNavPointTreeNodeMap.Keys) System.Windows.Forms.MessageBox.Show(s + " : " + m_SmilRefToNavPointTreeNodeMap[s].GetXmlElementLocalName() );
            if (RequestCancellation) return;
            XmlDocument smilXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullSmilPath, false, false);

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

                TreeNode obj;
                m_SmilRefToNavPointTreeNodeMap.TryGetValue(ncxContentSRC, out obj);

                // for now we are assuming the first phrase as heading phrase. this need refinement such that phrase anywhere in section can be imported as heading)
                if (obj != null) //m_SmilRefToNavPointTreeNodeMap.ContainsKey(ncxContentSRC))
                {
                    m_IsDocTitleCreated = true;
                    navPointTreeNode = obj; // m_SmilRefToNavPointTreeNodeMap[ncxContentSRC];
                    //System.Windows.Forms.MessageBox.Show(ncxContentSRC + " section:" + navPointTreeNode.GetXmlElementLocalName() + " : " + Path.GetFileName( fullSmilPath ) );
                    //: audioWrapperNode =  CreateTreeNodeForAudioNode(navPointTreeNode, true);
                    //isHeading = true;

                }

                // handle doctitle if audio exists before first heading by adding doctitle node as first treenode
                if (navPointTreeNode == null && !m_IsDocTitleCreated
                    && (XmlDocumentHelper.GetFirstChildElementOrSelfWithName(parNode, false, "audio", null) != null || XmlDocumentHelper.GetFirstChildElementOrSelfWithName(parNode, false, "text", null) != null))
                {
                    m_IsDocTitleCreated = true;
                    navPointTreeNode = CreateTreeNodeForNavPoint(m_Project.Presentations.Get(0).RootNode, m_DocTitleXmlNode);
                    m_NavPointNode_NavLabelMap.Add(navPointTreeNode, m_DocTitleXmlNode);

                }
                //else if (m_PageReferencesMapDictionaryForNCX.ContainsKey(ncxContentSRC)

                if (m_PageReferencesMapDictionaryForNCX.ContainsKey(ncxContentSRC)
                    || (parNode.Attributes.GetNamedItem("customTest") != null
                    && parNode.Attributes.GetNamedItem("customTest").Value == "pagenum"))
                {
                    isPageInProcess = true;
                    if (navPointTreeNode == null) continue;
                    //:audioWrapperNode = CreateTreeNodeForAudioNode(navPointTreeNode, false);

                }



                if (navPointTreeNode == null) continue;

                //System.Windows.Forms.MessageBox.Show(parNode.LocalName);

                ManagedAudioMedia textTreeNodeAudio = navPointTreeNode.GetManagedAudioMedia();
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
                        audioWrapperNode = CreateTreeNodeForAudioNode(navPointTreeNode, isHeading, textPeerNode, fullSmilPath);
                        CheckAndAssignForHeadingAudio(navPointTreeNode, audioWrapperNode, textPeerNode);
                        XmlProperty xmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
                        audioWrapperNode.AddProperty(xmlProp);

                        // +":" + navPointTreeNode.GetTextFlattened(false);
                        xmlProp.SetQName("phrase", "");

                        string pageRefInSmil = Path.GetFileName(fullSmilPath) + "#" + parNode.Attributes.GetNamedItem("id").Value;

                        XmlNode xNode;
                        m_PageReferencesMapDictionaryForNCX.TryGetValue(pageRefInSmil, out xNode);

                        if (xNode != null //m_PageReferencesMapDictionaryForNCX.ContainsKey(pageRefInSmil)
                            && isPageInProcess)
                        {
                            isPageInProcess = false;
                            XmlNode pageTargetNode = xNode; //m_PageReferencesMapDictionaryForNCX[pageRefInSmil];

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
                                //addAudio(audioWrapperNode, aChild, false, fullSmilPath);
                                audioWrapperNode = CreateTreeNodeForAudioNode(navPointTreeNode, false, aChild, fullSmilPath);
                                addAudio(audioWrapperNode, aChild, false, fullSmilPath);
                                break;
                            }
                            else
                            {
                                AddAnchorNode(navPointTreeNode, textPeerNode, fullSmilPath);
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

        protected virtual TreeNode AddAnchorNode(TreeNode navPointTreeNode, XmlNode textPeerNode, string fullSmilPath)
        {
            TreeNode anchorNode = navPointTreeNode.Presentation.TreeNodeFactory.Create();
            navPointTreeNode.AppendChild(anchorNode);
            XmlNode xmlNodeAttr = textPeerNode.Attributes.GetNamedItem("href");
            string strReferedID = xmlNodeAttr.Value;
            XmlNode seqParent = textPeerNode.ParentNode;
            while (seqParent != null)
            {
                if (seqParent.LocalName == "seq" && seqParent.Attributes.GetNamedItem("customTest") != null) break;
                seqParent = seqParent.ParentNode;
            }
            string strClass = seqParent.Attributes.GetNamedItem("class").Value;
            if (strClass != null)
            {
                XmlProperty prop = anchorNode.GetOrCreateXmlProperty();
                prop.SetQName(strClass, "");

                prop.SetAttribute(xmlNodeAttr.Name, "", strReferedID);
            }
            return anchorNode;
        }

        protected virtual TreeNode CreateTreeNodeForAudioNode(TreeNode navPointTreeNode, bool isHeadingNode, XmlNode smilNode, string fullSmilPath)
        {
            TreeNode audioWrapperNode = null;
            if (isHeadingNode)
            {
                foreach (TreeNode txtNode in navPointTreeNode.Children.ContentsAs_Enumerable)
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

                if (smilNode == null)
                {
                    navPointTreeNode.AppendChild(audioWrapperNode);
                }
                else
                {
                    TreeNode obj;
                    m_SmilXmlNodeToTreeNodeMap.TryGetValue(smilNode, out obj);
                    if (obj == null) //!m_SmilXmlNodeToTreeNodeMap.ContainsKey(smilNode))
                    {
                        navPointTreeNode.AppendChild(audioWrapperNode);
                    }
                    else
                    {
                        navPointTreeNode.InsertAfter(audioWrapperNode, obj); //m_SmilXmlNodeToTreeNodeMap[smilNode]);
                        m_SmilXmlNodeToTreeNodeMap[smilNode] = audioWrapperNode;
                    }
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
            textMedia.Text = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(pageTargetNode, true, "text", pageTargetNode.NamespaceURI).InnerText;
            ChannelsProperty cProp = audioWrapperNode.Presentation.PropertyFactory.CreateChannelsProperty();
            cProp.SetMedia(audioWrapperNode.Presentation.ChannelsManager.GetOrCreateTextChannel(), textMedia);
            audioWrapperNode.AddProperty(cProp);
            System.Xml.XmlAttributeCollection pageAttributes = pageTargetNode.Attributes;
            if (pageAttributes != null)
            {
                XmlProperty xmlProp = audioWrapperNode.GetXmlProperty();
                xmlProp.SetQName("pagenum", "");
                string nsUri = audioWrapperNode.GetXmlNamespaceUri();
                foreach (System.Xml.XmlAttribute attr in pageAttributes)
                {
                    string uri = "";
                    if (!string.IsNullOrEmpty(attr.NamespaceURI))
                    {
                        if (attr.NamespaceURI != nsUri)
                        {
                            uri = attr.NamespaceURI;
                        }
                    }

                    xmlProp.SetAttribute(attr.Name, uri, attr.Value);
                }
            }
        }


        protected virtual TreeNode CheckAndAssignForHeadingAudio(TreeNode navPointTreeNode, TreeNode phraseTreeNode, XmlNode audioXmlNode)
        {

            XmlNode navLabelXmlNode = m_NavPointNode_NavLabelMap[navPointTreeNode];
            XmlNode headingAudio = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(navLabelXmlNode, true, "audio", navLabelXmlNode.NamespaceURI);
            XmlNode textNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(navLabelXmlNode, true, "text", navLabelXmlNode.NamespaceURI);

            double headingClipBegin = Math.Abs((new urakawa.media.timing.Time(headingAudio.Attributes.GetNamedItem("clipBegin").Value)).AsMilliseconds);
            double headingClipEnd = Math.Abs((new urakawa.media.timing.Time(headingAudio.Attributes.GetNamedItem("clipEnd").Value)).AsMilliseconds);

            double audioClipBegin = Math.Abs((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipBegin").Value)).AsMilliseconds);
            double audioClipEnd = Math.Abs((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipEnd").Value)).AsMilliseconds);

            if (headingAudio.Attributes.GetNamedItem("src").Value == audioXmlNode.Attributes.GetNamedItem("src").Value
                && Math.Abs(headingClipBegin - audioClipBegin) <= 1
                && Math.Abs(headingClipEnd - audioClipEnd) <= 1)
            {
                TextMedia textMedia = navPointTreeNode.Presentation.MediaFactory.CreateTextMedia();
                textMedia.Text = textNode.InnerText;

                ChannelsProperty cProp = navPointTreeNode.Presentation.PropertyFactory.CreateChannelsProperty();
                cProp.SetMedia(navPointTreeNode.Presentation.ChannelsManager.GetOrCreateTextChannel(), textMedia);

                //TreeNode txtWrapperNode = parentNode.Presentation.TreeNodeFactory.Create();
                phraseTreeNode.AddProperty(cProp);
                //treeNode.AppendChild(txtWrapperNode);

                XmlProperty TextNodeXmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
                phraseTreeNode.AddProperty(TextNodeXmlProp);
                TextNodeXmlProp.SetQName("hd", "");
            }

            return phraseTreeNode;
        }

    }
}
