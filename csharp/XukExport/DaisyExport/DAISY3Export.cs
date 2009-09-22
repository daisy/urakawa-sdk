using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml;

using urakawa;
using urakawa.core;
using urakawa.publish;
using urakawa.property.channel;
using urakawa.media;
using urakawa.xuk;


namespace DaisyExport
{
    public partial class DAISY3Export
    {
        private Presentation m_Presentation;
        private string m_OutputDirectory;
        private const string PUBLISH_AUDIO_CHANNEL_NAME = "Temporary External Audio Medias (Publish Visitor)";

        private const string m_Filename_Content = "dtbook.xml";
        private const string m_Filename_Ncx = "navigation.ncx";
        private const string m_Filename_Opf = "package.opf";

        private Dictionary<urakawa.core.TreeNode, XmlNode> m_TreeNode_XmlNodeMap; // dictionary created in create content document function, used in create ncx and smil function
        private List<urakawa.core.TreeNode> m_ListOfLevels; // list of level anddoctitle, docauthor nodes collected in createContentDoc function, for creating equivalent navPoints in create NCX funtion 

        private List<string> m_FilesList_Smil; //xmils files list generated in createNcx function
        private List<string> m_FilesList_Audio; // list of audio files generated in create ncx function.
        private List<string> m_FilesList_Image; // list of images, populated in create content document function
        private TimeSpan m_TotalTime;

        public DAISY3Export(Presentation presentation)
        {
            m_Presentation = presentation;

        }

        private bool doesTreeNodeTriggerNewSmil(TreeNode node)
        {
            QualifiedName qName = node.GetXmlElementQName();
            return qName != null && qName.LocalName.StartsWith("level");
        }

        public void ExportToDaisy3(string exportDirectory)
        {
        if (m_Presentation.RootNode.GetDurationOfManagedAudioMediaFlattened ().TimeDeltaAsMillisecondLong == 0)
            {
            System.Diagnostics.Debug.Fail ( "no audio found", "No audio node found in the project being exported" );
            }

            m_ID_Counter = 0;
            m_OutputDirectory = exportDirectory;

            //TreeNodeTestDelegate triggerDelegate  = delegate(urakawa.core.TreeNode node) { return node.GetManagedAudioMedia () != null ; };
            TreeNodeTestDelegate triggerDelegate = doesTreeNodeTriggerNewSmil;
            TreeNodeTestDelegate skipDelegate = delegate(urakawa.core.TreeNode node) { return false; };

            PublishFlattenedManagedAudioVisitor publishVisitor = new PublishFlattenedManagedAudioVisitor(triggerDelegate, skipDelegate);

            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }
            m_OutputDirectory = exportDirectory;

            publishVisitor.DestinationDirectory = new Uri(exportDirectory, UriKind.Absolute);

            publishVisitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();

            Channel publishChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
            publishChannel.Name = PUBLISH_AUDIO_CHANNEL_NAME;
            publishVisitor.DestinationChannel = publishChannel;

            m_Presentation.RootNode.AcceptDepthFirst(publishVisitor);

            // The verification is not strictly necessary, but we should use it for testing all kinds of books, before stable release.
            publishVisitor.VerifyTree(m_Presentation.RootNode);

            // following functions can be called only in this order.
            CreateDTBookDocument();
            CreateNcxAndSmilDocuments();
            CreateOpfDocument();

            m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
        }

        private urakawa.media.ExternalAudioMedia GetExternalAudioMedia(urakawa.core.TreeNode node)
        {
            List<urakawa.property.channel.Channel> channelsList = m_Presentation.ChannelsManager.GetChannelsByName(PUBLISH_AUDIO_CHANNEL_NAME);
            if (channelsList == null || channelsList.Count == 0)
                return null;

            if (channelsList == null || channelsList.Count > 1)
                throw new System.Exception("more than one publish channel cannot exist");

            Channel publishChannel = channelsList[0];

            urakawa.property.channel.ChannelsProperty mediaProperty = node.GetProperty<ChannelsProperty>();

            if (mediaProperty == null) return null;

            return (ExternalAudioMedia)mediaProperty.GetMedia(publishChannel);
        }

        private const string ID_DTBPrefix = "dtb_";
        private const string ID_SmilPrefix = "sm_";
        private const string ID_NcxPrefix = "ncx_";
        private const string ID_OpfPrefix = "opf_";
        private long m_ID_Counter;

        private string GetNextID(string prefix)
        {
            string strNumericFrag = (++m_ID_Counter).ToString();
            return prefix + strNumericFrag;
        }

        private static XmlNode getFirstChildElementsWithName(XmlNode root, bool deep, string localName, string namespaceUri)
        {
            foreach (XmlNode node in getChildrenElementsWithName(root, deep, localName, namespaceUri, true))
            {
                return node;
            }
            return null;
        }

        private static IEnumerable<XmlNode> getChildrenElementsWithName(XmlNode root, bool deep, string localName, string namespaceUri, bool breakOnFirstFound)
        {
            if (root.NodeType == XmlNodeType.Document)
            {
                XmlNode element = null;
                XmlDocument doc = (XmlDocument)root;
                IEnumerator docEnum = doc.GetEnumerator();
                while (docEnum.MoveNext())
                {
                    XmlNode node = (XmlNode)docEnum.Current;

                    if (node != null
                        && node.NodeType == XmlNodeType.Element)
                    {
                        element = node;
                        break; // first element is ok.
                    }
                }

                if (element == null)
                {
                    yield break;
                }

                foreach (XmlNode childNode in getChildrenElementsWithName(element, deep, localName, namespaceUri, breakOnFirstFound))
                {
                    yield return childNode;

                    if (breakOnFirstFound)
                    {
                        yield break;
                    }
                }

                yield break;
            }

            if (root.NodeType != XmlNodeType.Element)
            {
                yield break;
            }

            if (root.LocalName == localName || root.Name == localName)
            {
                if (!string.IsNullOrEmpty(namespaceUri))
                {
                    if (root.NamespaceURI == namespaceUri)
                    {
                        yield return root;

                        if (breakOnFirstFound)
                        {
                            yield break;
                        }
                    }
                }
                else
                {
                    yield return root;

                    if (breakOnFirstFound)
                    {
                        yield break;
                    }
                }
            }

            IEnumerator enumerator = root.GetEnumerator();
            while (enumerator.MoveNext())
            {
                XmlNode node = (XmlNode)enumerator.Current;

                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (deep)
                {
                    foreach (XmlNode childNode in getChildrenElementsWithName(node, deep, localName, namespaceUri, breakOnFirstFound))
                    {
                        yield return childNode;

                        if (breakOnFirstFound)
                        {
                            yield break;
                        }
                    }
                }
                else
                {
                    if (node.LocalName == localName || node.Name == localName)
                    {
                        if (!string.IsNullOrEmpty(namespaceUri))
                        {
                            if (node.NamespaceURI == namespaceUri)
                            {
                                yield return node;

                                if (breakOnFirstFound)
                                {
                                    yield break;
                                }
                            }
                        }
                        else
                        {
                            yield return node;

                            if (breakOnFirstFound)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }

            yield break;
        }
    }
}
