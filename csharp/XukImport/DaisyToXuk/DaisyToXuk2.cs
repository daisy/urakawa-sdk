using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa;
using urakawa.core;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.property.channel;
using core = urakawa.core;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private AudioChannel m_audioChannel;

        private void parseOPFAndPopulateDataModel()
        {
            m_firstTimePCMFormat = true;
            if (m_convertedWavFiles != null)
            {
                m_convertedWavFiles.Clear();
            }
            else
            {
                m_convertedWavFiles = new Dictionary<string, string>();
            }
            if (m_convertedMp3Files != null)
            {
                m_convertedMp3Files.Clear();
            }
            else
            {
                m_convertedMp3Files = new Dictionary<string, string>();
            }

            XmlDocument opfXmlDoc = readXmlDocument(m_Book_FilePath);

            parseOpfDcMetaData(opfXmlDoc);
            parseOpfMetaData(opfXmlDoc);

            List<string> spineListOfSmilFiles;
            string ncxPath;
            string DtBookPath;

            parseOpfManifestAndSpine(opfXmlDoc, out DtBookPath, out spineListOfSmilFiles, out ncxPath);

            if (DtBookPath != null)
            {
                string fullDtBookPath = Path.Combine(m_outDirectory, DtBookPath);
                XmlDocument bookXmlDoc = readXmlDocument(fullDtBookPath);
                parseDTBookXmlDocAndPopulateDataModel(bookXmlDoc, null);
            }

            if (ncxPath != null)
            {
                string fullNcxPath = Path.Combine(m_outDirectory, ncxPath);
                parseNcx(fullNcxPath);
            }

            if (spineListOfSmilFiles != null)
            {
                foreach (string smilPath in spineListOfSmilFiles)
                {
                    string fullSmilPath = Path.Combine(m_outDirectory, smilPath);
                    parseSmil(fullSmilPath);
                }
            }
        }

        private bool m_firstTimePCMFormat;
        private Dictionary<string, string> m_convertedWavFiles = null;
        private Dictionary<string, string> m_convertedMp3Files = null;

        private void parseSmil(string fullSmilPath)
        {
            Presentation presentation = m_Project.GetPresentation(0);

            XmlDocument smilXmlDoc = readXmlDocument(fullSmilPath);

            XmlNodeList allTextNodes = smilXmlDoc.GetElementsByTagName("text");
            if (allTextNodes == null || allTextNodes.Count == 0)
            {
                return;
            }
            foreach (XmlNode textNode in allTextNodes)
            {
                XmlAttributeCollection textNodeAttrs = textNode.Attributes;
                if (textNodeAttrs == null || textNodeAttrs.Count == 0)
                {
                    continue;
                }
                XmlNode textNodeAttrSrc = textNodeAttrs.GetNamedItem("src");
                if (textNodeAttrSrc == null || String.IsNullOrEmpty(textNodeAttrSrc.Value))
                {
                    continue;
                } int index = textNodeAttrSrc.Value.LastIndexOf('#');
                if (index == textNodeAttrSrc.Value.Length - 1)
                {
                    return;
                }
                string srcFragmentId = textNodeAttrSrc.Value.Substring(index + 1);
                core.TreeNode textTreeNode = getTreeNodeWithXmlElementId(srcFragmentId);
                if (textTreeNode == null)
                {
                    continue;
                }
                AbstractAudioMedia textTreeNodeAudio = textTreeNode.GetAudioMedia();
                if (textTreeNodeAudio != null)
                {
                    //Ignore.
                    continue;
                }
                XmlNode parent = textNode.ParentNode;
                if (parent != null && parent.Name == "a")
                {
                    parent = parent.ParentNode;
                }
                if (parent == null)
                {
                    continue;
                }
                if (parent.Name != "par")
                {
                    System.Diagnostics.Debug.Fail("Text node in SMIL has no parallel time container as parent ! {0}", parent.Name);
                    continue;
                }
                XmlNodeList textPeers = parent.ChildNodes;
                foreach (XmlNode textPeerNode in textPeers)
                {
                    if (textPeerNode.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }
                    if (textPeerNode.Name == "audio")
                    {
                        addAudio(textTreeNode, textPeerNode, false);
                        break;
                    }
                    else if (textPeerNode.Name == "a")
                    {
                        XmlNodeList aChildren = textPeerNode.ChildNodes;
                        foreach (XmlNode aChild in aChildren)
                        {
                            if (aChild.Name == "audio")
                            {
                                addAudio(textTreeNode, aChild, false);
                                break;
                            }
                        }
                    }
                    else if (textPeerNode.Name == "seq")
                    {
                        XmlNodeList seqChildren = textPeerNode.ChildNodes;
                        foreach (XmlNode seqChild in seqChildren)
                        {
                            if (seqChild.Name == "audio")
                            {
                                addAudio(textTreeNode, seqChild, true);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void addAudio(TreeNode treeNode, XmlNode xmlNode, bool isSequence)
        {
            XmlAttributeCollection audioAttrs = xmlNode.Attributes;

            if (audioAttrs == null || audioAttrs.Count == 0)
            {
                return;
            }
            XmlNode audioAttrSrc = audioAttrs.GetNamedItem("src");
            if (audioAttrSrc == null || String.IsNullOrEmpty(audioAttrSrc.Value))
            {
                return;
            }
            XmlNode audioAttrClipBegin = audioAttrs.GetNamedItem("clipBegin");
            XmlNode audioAttrClipEnd = audioAttrs.GetNamedItem("clipEnd");

            Presentation presentation = m_Project.GetPresentation(0);
            Media media = null;

            if (audioAttrSrc.Value.EndsWith("wav"))
            {
                media = addAudioWav(audioAttrSrc.Value, audioAttrClipBegin, audioAttrClipEnd);
            }
            else if (audioAttrSrc.Value.EndsWith("mp3"))
            {
                string fullMp3PathOriginal = Path.Combine(m_outDirectory, audioAttrSrc.Value);
                if (!File.Exists(fullMp3PathOriginal))
                {
                    System.Diagnostics.Debug.Fail("File not found: {0}", fullMp3PathOriginal);
                    return;
                }

                if (m_convertedMp3Files.ContainsKey(fullMp3PathOriginal))
                {
                    string fullWavPath = m_convertedWavFiles[fullMp3PathOriginal];
                    media = addAudioWav(fullWavPath, audioAttrClipBegin, audioAttrClipEnd);
                }
                else
                {
                    IWavFormatConverter wavConverter = new WavFormatConverter(true);

                    string newfullWavPath = wavConverter.UnCompressMp3File(fullMp3PathOriginal, Path.GetDirectoryName(fullMp3PathOriginal), presentation.MediaDataManager.DefaultPCMFormat);

                    if (newfullWavPath != null)
                    {
                        m_convertedMp3Files.Add(fullMp3PathOriginal, newfullWavPath);
                        media = addAudioWav(newfullWavPath, audioAttrClipBegin, audioAttrClipEnd);
                    }
                }
            }

            if (media == null)
            {
                media = presentation.MediaFactory.CreateExternalAudioMedia();
                ((ExternalAudioMedia)media).Src = audioAttrSrc.Value;
                if (audioAttrClipBegin != null &&
                    !string.IsNullOrEmpty(audioAttrClipBegin.Value))
                {
                    ((ExternalAudioMedia)media).ClipBegin =
                        new Time(TimeSpan.Parse(audioAttrClipBegin.Value));
                }
                if (audioAttrClipEnd != null &&
                    !string.IsNullOrEmpty(audioAttrClipEnd.Value))
                {
                    ((ExternalAudioMedia)media).ClipEnd =
                        new Time(TimeSpan.Parse(audioAttrClipEnd.Value));
                }
            }

            if (media != null)
            {
                ChannelsProperty chProp =
                    treeNode.GetProperty<ChannelsProperty>();
                if (chProp == null)
                {
                    chProp =
                        presentation.PropertyFactory.CreateChannelsProperty();
                    treeNode.AddProperty(chProp);
                }
                if (isSequence)
                {
                    SequenceMedia mediaSeq = chProp.GetMedia(m_audioChannel) as SequenceMedia;
                    if (mediaSeq == null)
                    {
                        mediaSeq = presentation.MediaFactory.CreateSequenceMedia();
                        mediaSeq.AllowMultipleTypes = false;
                        chProp.SetMedia(m_audioChannel, mediaSeq);
                    }
                    mediaSeq.AppendItem(media);
                }
                else
                {
                    chProp.SetMedia(m_audioChannel, media);
                }
            }
            else
            {
                System.Diagnostics.Debug.Fail("Media could not be created !");
            }
        }

        private Media addAudioWav(string src, XmlNode audioAttrClipBegin, XmlNode audioAttrClipEnd)
        {
            Media media = null;
            Presentation presentation = m_Project.GetPresentation(0);

            string fullWavPathOriginal = Path.Combine(m_outDirectory, src);
            if (!File.Exists(fullWavPathOriginal))
            {
                System.Diagnostics.Debug.Fail("File not found: {0}", fullWavPathOriginal);
                return null;
            }
            string fullWavPath = fullWavPathOriginal;
            if (m_convertedWavFiles.ContainsKey(fullWavPathOriginal))
            {
                fullWavPath = m_convertedWavFiles[fullWavPathOriginal];
            }
            PCMDataInfo pcmInfo = null;
            Stream wavStream = null;
            try
            {
                wavStream = File.Open(fullWavPath, FileMode.Open,
                                      FileAccess.Read, FileShare.Read);
                pcmInfo = PCMDataInfo.ParseRiffWaveHeader(wavStream);

                if (m_firstTimePCMFormat)
                {
                    presentation.MediaDataManager.DefaultPCMFormat =
                        pcmInfo.Copy();
                    m_firstTimePCMFormat = false;
                }
                if (!presentation.MediaDataManager.DefaultPCMFormat.IsCompatibleWith(pcmInfo))
                {
                    wavStream.Close();

                    if (m_convertedWavFiles.ContainsKey(fullWavPathOriginal))
                    {
                        throw new Exception("The previously converted WAV file is not with the correct PCM format !!");
                    }

                    IWavFormatConverter wavConverter = new WavFormatConverter(true);

                    string newfullWavPath = wavConverter.ConvertSampleRate(fullWavPath, Path.GetDirectoryName(fullWavPath), presentation.MediaDataManager.DefaultPCMFormat);

                    m_convertedWavFiles.Add(fullWavPath, newfullWavPath);

                    wavStream = File.Open(newfullWavPath, FileMode.Open,
                                          FileAccess.Read, FileShare.Read);
                    pcmInfo = PCMDataInfo.ParseRiffWaveHeader(wavStream);

                    if (!presentation.MediaDataManager.DefaultPCMFormat.IsCompatibleWith(pcmInfo))
                    {
                        wavStream.Close();
                        throw new Exception("Could not convert the WAV PCM format !!");
                    }
                }

                TimeDelta totalDuration = new TimeDelta(pcmInfo.Duration);

                TimeDelta clipDuration = new TimeDelta(totalDuration);

                Time clipB = Time.Zero;
                Time clipE = Time.MaxValue;

                if (audioAttrClipBegin != null &&
                    !string.IsNullOrEmpty(audioAttrClipBegin.Value))
                {
                    clipB = new Time(TimeSpan.Parse(audioAttrClipBegin.Value));
                }
                if (audioAttrClipEnd != null &&
                    !string.IsNullOrEmpty(audioAttrClipEnd.Value))
                {
                    clipE = new Time(TimeSpan.Parse(audioAttrClipEnd.Value));
                }
                if (!clipB.IsEqualTo(Time.Zero) || !clipE.IsEqualTo(Time.MaxValue))
                {
                    clipDuration = clipE.GetTimeDelta(clipB);
                }
                else
                {
                    System.Diagnostics.Debug.Fail("Audio clip with full duration ??");
                }
                long byteOffset = 0;
                if (!clipB.IsEqualTo(Time.Zero))
                {
                    byteOffset = pcmInfo.GetByteForTime(clipB);
                }
                if (byteOffset > 0)
                {
                    wavStream.Seek(byteOffset, SeekOrigin.Current);
                }

                presentation.MediaDataFactory.DefaultAudioMediaDataType =
                    typeof(WavAudioMediaData);

                WavAudioMediaData mediaData =
                    (WavAudioMediaData)
                    presentation.MediaDataFactory.CreateAudioMediaData();

                mediaData.InsertAudioData(wavStream, Time.Zero, clipDuration);

                media = presentation.MediaFactory.CreateManagedAudioMedia();
                ((ManagedAudioMedia)media).AudioMediaData = mediaData;   
            }
            finally
            {
                if (wavStream != null) wavStream.Close();
            }
            return media;
        }

        private void parseNcx(string ncxPath)
        {
            //Presentation presentation = DTBooktoXukConversion.m_Project.GetPresentation(0);
            Presentation presentation = m_Project.GetPresentation(0);
            //XmlDocument ncxXmlDoc = DTBooktoXukConversion.readXmlDocument(ncxPath);
            XmlDocument ncxXmlDoc = readXmlDocument(ncxPath);


            XmlNodeList listOfHeadRootNodes = ncxXmlDoc.GetElementsByTagName("head");
            if (listOfHeadRootNodes != null)
            {
                foreach (XmlNode headNodeRoot in listOfHeadRootNodes)
                {
                    XmlNodeList listOfMetaNodes = headNodeRoot.ChildNodes;
                    if (listOfMetaNodes != null)
                    {
                        foreach (XmlNode metaNode in listOfMetaNodes)
                        {
                            if (metaNode.NodeType == XmlNodeType.Element
                                && metaNode.Name == "meta")
                            {
                                XmlAttributeCollection attributeCol = metaNode.Attributes;

                                if (attributeCol != null)
                                {
                                    XmlNode attrName = attributeCol.GetNamedItem("name");
                                    XmlNode attrContent = attributeCol.GetNamedItem("content");
                                    if (attrName != null && attrContent != null && !String.IsNullOrEmpty(attrName.Value)
                                            && !String.IsNullOrEmpty(attrContent.Value))
                                    {
                                        Metadata md = presentation.MetadataFactory.CreateMetadata();
                                        md.Name = attrName.Value;
                                        md.Content = attrContent.Value;
                                        presentation.AddMetadata(md);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void parseOpfManifestAndSpine(XmlDocument opfXmlDoc, out string DtBookPath, out List<string> spineListOfSmilFiles, out string ncxPath)
        {
            spineListOfSmilFiles = new List<string>();

            ncxPath = null;
            DtBookPath = null;

            XmlNodeList listOfSpineRootNodes = opfXmlDoc.GetElementsByTagName("spine");
            if (listOfSpineRootNodes != null)
            {
                foreach (XmlNode spineNodeRoot in listOfSpineRootNodes)
                {
                    XmlNodeList listOfSpineItemNodes = spineNodeRoot.ChildNodes;
                    if (listOfSpineItemNodes != null)
                    {
                        foreach (XmlNode spineItemNode in listOfSpineItemNodes)
                        {
                            if (spineItemNode.NodeType == XmlNodeType.Element
                                && spineItemNode.Name == "itemref")
                            {
                                XmlAttributeCollection spineItemAttributes = spineItemNode.Attributes;

                                if (spineItemAttributes != null)
                                {
                                    XmlNode attrIdRef = spineItemAttributes.GetNamedItem("idref");
                                    if (attrIdRef != null)
                                    {
                                        spineListOfSmilFiles.Add(attrIdRef.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            XmlNodeList listOfManifestRootNodes = opfXmlDoc.GetElementsByTagName("manifest");
            if (listOfManifestRootNodes != null)
            {
                foreach (XmlNode manifNodeRoot in listOfManifestRootNodes)
                {
                    XmlNodeList listOfManifestItemNodes = manifNodeRoot.ChildNodes;
                    if (listOfManifestItemNodes != null)
                    {
                        foreach (XmlNode manifItemNode in listOfManifestItemNodes)
                        {
                            if (manifItemNode.NodeType == XmlNodeType.Element
                                && manifItemNode.Name == "item")
                            {
                                XmlAttributeCollection manifItemAttributes = manifItemNode.Attributes;

                                if (manifItemAttributes != null)
                                {
                                    XmlNode attrHref = manifItemAttributes.GetNamedItem("href");
                                    XmlNode attrMediaType = manifItemAttributes.GetNamedItem("media-type");
                                    if (attrHref != null && attrMediaType != null)
                                    {
                                        if (attrMediaType.Value == "application/smil")
                                        {
                                            XmlNode attrID = manifItemAttributes.GetNamedItem("id");
                                            if (attrID != null)
                                            {
                                                int i = spineListOfSmilFiles.IndexOf(attrID.Value);
                                                if (i >= 0)
                                                {
                                                    spineListOfSmilFiles[i] = attrHref.Value;
                                                }
                                            }
                                        }
                                        else if (attrMediaType.Value == "application/x-dtbncx+xml")
                                        {
                                            ncxPath = attrHref.Value;
                                        }
                                        else if (attrMediaType.Value == "application/x-dtbook+xml")
                                        {
                                            DtBookPath = attrHref.Value;
                                        }
                                        else if (attrMediaType.Value == "text/xml")
                                        {
                                            // Ignore (OPF)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void parseOpfDcMetaData(XmlDocument opfXmlDoc)
        {
            //Presentation presentation = DTBooktoXukConversion.m_Project.GetPresentation(0);
            Presentation presentation = m_Project.GetPresentation(0);

            XmlNodeList listOfMetaDataRootNodes = opfXmlDoc.GetElementsByTagName("dc-metadata");
            if (listOfMetaDataRootNodes != null)
            {
                foreach (XmlNode mdNodeRoot in listOfMetaDataRootNodes)
                {
                    XmlNodeList listOfMetaDataNodes = mdNodeRoot.ChildNodes;
                    if (listOfMetaDataNodes != null)
                    {
                        foreach (XmlNode mdNode in listOfMetaDataNodes)
                        {
                            if (mdNode.NodeType == XmlNodeType.Element)
                            {
                                Metadata md = presentation.MetadataFactory.CreateMetadata();
                                md.Name = mdNode.Name;
                                md.Content = mdNode.InnerText;
                                presentation.AddMetadata(md);
                            }
                        }
                    }
                }
            }
        }//parseOpfDcMetaData

        private void parseOpfMetaData(XmlDocument opfXmlDoc)
        {
            //Presentation presentation = DTBooktoXukConversion.m_Project.GetPresentation(0);
            Presentation presentation = m_Project.GetPresentation(0);

            XmlNodeList listOfMetaDataRootNodes = opfXmlDoc.GetElementsByTagName("x-metadata");
            if (listOfMetaDataRootNodes != null)
            {
                foreach (XmlNode mdNodeRoot in listOfMetaDataRootNodes)
                {
                    XmlNodeList listOfMetaDataNodes = mdNodeRoot.ChildNodes;
                    if (listOfMetaDataNodes != null)
                    {
                        foreach (XmlNode mdNode in listOfMetaDataNodes)
                        {
                            if (mdNode.NodeType == XmlNodeType.Element && mdNode.Name == "meta")
                            {
                                XmlAttributeCollection mdAttributes = mdNode.Attributes;

                                if (mdAttributes != null)
                                {
                                    XmlNode attrName = mdAttributes.GetNamedItem("name");
                                    XmlNode attrContent = mdAttributes.GetNamedItem("content");

                                    if (attrName != null && attrContent != null && !String.IsNullOrEmpty(attrName.Value)
                                        && !String.IsNullOrEmpty(attrContent.Value))
                                    {
                                        Metadata md = presentation.MetadataFactory.CreateMetadata();
                                        md.Name = attrName.Value;
                                        md.Content = attrContent.Value;
                                        presentation.AddMetadata(md);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }//parseOpfMetaData
    }

}
