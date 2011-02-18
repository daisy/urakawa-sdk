using System;
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

        public void ParseNCXDocument(XmlDocument ncxDocument)
        {
            m_SmilRefToNavPointTreeNodeMap = new Dictionary<string, TreeNode>();
            //string ncxPath = Directory.GetFiles(Directory.GetParent(m_Book_FilePath).FullName, "*.ncx")[0];
            //ncxDocument = xuk.OpenXukAction.ParseXmlDocument(ncxPath, false);

            XmlNode navMap = ncxDocument.GetElementsByTagName("navMap")[0];
            Presentation presentation = m_Project.Presentations.Get(0);
            XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
            xmlProp.LocalName = "book";
            presentation.PropertyFactory.DefaultXmlNamespaceUri = navMap.NamespaceURI;
            xmlProp.NamespaceUri = presentation.PropertyFactory.DefaultXmlNamespaceUri;
            TreeNode treeNode = presentation.TreeNodeFactory.Create();
            treeNode.AddProperty(xmlProp);
            presentation.RootNode = treeNode;

            
            ParseNCXNodes(presentation, navMap, treeNode);
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
                if (node.LocalName == "navPoint")
                {
                    TreeNode treeNode = presentation.TreeNodeFactory.Create();
                    tNode.AppendChild(treeNode);
                    XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                    treeNode.AddProperty(xmlProp);
                    XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(node, true, "text", node.NamespaceURI);
                    xmlProp.LocalName = "level";//+":" + textNode.InnerText;
                    // create urakawa tree node
                    
                    TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                    textMedia.Text = textNode.InnerText;

                    ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                    cProp.SetMedia(m_textChannel, textMedia);

                    TreeNode txtWrapperNode = presentation.TreeNodeFactory.Create();
                    txtWrapperNode.AddProperty(cProp);
                    treeNode.AppendChild(txtWrapperNode);

                    XmlNode contentNode = XmlDocumentHelper.GetFirstChildElementWithName(node, true, "content", node.NamespaceURI);
                    m_SmilRefToNavPointTreeNodeMap.Add(contentNode.Attributes.GetNamedItem("src").Value, treeNode);
                }
            
                if (node.ChildNodes == null || node.ChildNodes.Count == 0) return;
                foreach (XmlNode n in node.ChildNodes)
                {   
                    ParseNCXNodes(presentation, n, tNode);
                }
            //}
        }


        private void parseSmilForNCX(string fullSmilPath)
        {
            //foreach (string s in m_SmilRefToNavPointTreeNodeMap.Keys) System.Windows.Forms.MessageBox.Show(s + " : " + m_SmilRefToNavPointTreeNodeMap[s].GetXmlElementQName().LocalName );
            if (RequestCancellation) return;
            XmlDocument smilXmlDoc = OpenXukAction.ParseXmlDocument(fullSmilPath, false);

            if (RequestCancellation) return;
            //we skip SMIL metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            //parseMetadata(smilXmlDoc);

            //XmlNodeList allTextNodes = smilXmlDoc.GetElementsByTagName("text");
            //if (allTextNodes.Count == 0)
            //{
            //    return;
            //}

            //reportProgress(-1, "Parsing SMIL: [" + Path.GetFileName(fullSmilPath) + "]");
            TreeNode navPointTreeNode = null;
            foreach (XmlNode parNode in XmlDocumentHelper.GetChildrenElementsWithName(smilXmlDoc, true, "par", null, false))
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
                //int index = textNodeAttrSrc.Value.LastIndexOf('#');
                //if (index == textNodeAttrSrc.Value.Length - 1)
                //{
                    //m_SmilRefToNavPointTreeNodeM//return;
                //}
                string ncxContentSRC = Path.GetFileName(fullSmilPath) + "#" + parNodeID.Value;
                //string srcFragmentId = textNodeAttrSrc.Value.Substring(index + 1);
                //TreeNode textTreeNode = getTreeNodeWithXmlElementId(srcFragmentId);
                //if (textTreeNode == null)
                //{
                    //continue;
                //}
                if (m_SmilRefToNavPointTreeNodeMap.ContainsKey(ncxContentSRC))
                {
                    navPointTreeNode = m_SmilRefToNavPointTreeNodeMap[ncxContentSRC];
                    //System.Windows.Forms.MessageBox.Show(ncxContentSRC + " section:" + navPointTreeNode.GetXmlElementQName().LocalName + " : " + Path.GetFileName( fullSmilPath ) );
                    
                }
                if (navPointTreeNode == null) continue;
                TreeNode audioWrapperNode = navPointTreeNode.Presentation.TreeNodeFactory.Create();
                //audioWrapperNode.AddProperty(cProp);
                navPointTreeNode.AppendChild(audioWrapperNode);

                XmlProperty xmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
                audioWrapperNode.AddProperty(xmlProp);
                xmlProp.LocalName = "phrase" + ":" + navPointTreeNode.GetTextFlattened(false);
                AbstractAudioMedia textTreeNodeAudio = navPointTreeNode.GetAudioMedia();
                if (textTreeNodeAudio != null)
                {
                    //Ignore.
                    continue;
                }
                //XmlNode parent = parNode.ParentNode;
                //if (parent != null && parent.LocalName == "a")
                //{
                    //parent = parent.ParentNode;
                //}
                //if (parent == null)
                //{
                    //continue;
                //}
                //if (parent.LocalName != "par")
                //{
                    //System.Diagnostics.Debug.Fail("Text node in SMIL has no parallel time container as parent ! {0}", parent.Name);
                    //continue;
                //}
                //XmlNodeList textPeers = parent.ChildNodes;
                foreach (XmlNode textPeerNode in XmlDocumentHelper.GetChildrenElementsWithName(smilXmlDoc, true, "audio", null, false))
                {
                    if (RequestCancellation) return;

                    if (textPeerNode.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }
                    if (textPeerNode.LocalName == "audio")
                    {   
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
                    else if (textPeerNode.LocalName == "seq")
                    {
                        XmlNodeList seqChildren = textPeerNode.ChildNodes;
                        foreach (XmlNode seqChild in seqChildren)
                        {
                            if (seqChild.LocalName == "audio")
                            {
                                addAudio(audioWrapperNode, seqChild, true, fullSmilPath);
                            }
                        }

                        SequenceMedia seqManAudioMedia = audioWrapperNode.GetManagedAudioSequenceMedia();
                        if (seqManAudioMedia == null)
                        {
                            //Debug.Fail("This should never happen !");
                            break;
                        }

                        ManagedAudioMedia managedAudioMedia = audioWrapperNode.Presentation.MediaFactory.CreateManagedAudioMedia();
                        AudioMediaData mediaData = audioWrapperNode.Presentation.MediaDataFactory.CreateAudioMediaData();
                        managedAudioMedia.AudioMediaData = mediaData;

                        foreach (Media seqChild in seqManAudioMedia.ChildMedias.ContentsAs_YieldEnumerable)
                        {
                            ManagedAudioMedia seqManMedia = (ManagedAudioMedia)seqChild;

                            mediaData.MergeWith(seqManMedia.AudioMediaData);

                            //Stream stream = seqManMedia.AudioMediaData.OpenPcmInputStream();
                            //try
                            //{
                            //    mediaData.AppendPcmData(stream, null);
                            //}
                            //finally
                            //{
                            //    stream.Close();
                            //}

                            //seqManMedia.AudioMediaData.Delete(); // doesn't actually removes the FileDataProviders (need to call Presentation.Cleanup())
                            ////textTreeNode.Presentation.DataProviderManager.RemoveDataProvider();
                        }

                        ChannelsProperty chProp = audioWrapperNode.GetChannelsProperty();
                        chProp.SetMedia(m_audioChannel, null);
                        chProp.SetMedia(m_audioChannel, managedAudioMedia);

                        break;
                    }
                }
            }
        }



    }
}
