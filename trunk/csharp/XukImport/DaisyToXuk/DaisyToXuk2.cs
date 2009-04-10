using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa;
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

        private void parseSmil(string fullSmilPath)
        {
            Presentation presentation = m_Project.GetPresentation(0);

            XmlDocument smilXmlDoc = readXmlDocument(fullSmilPath);

            XmlNodeList listOfAudioNodes = smilXmlDoc.GetElementsByTagName("audio");
            if (listOfAudioNodes != null)
            {
                foreach (XmlNode audioNode in listOfAudioNodes)
                {
                    XmlAttributeCollection attributeCol = audioNode.Attributes;

                    if (attributeCol != null)
                    {
                        XmlNode attrAudioSrc = attributeCol.GetNamedItem("src");
                        if (attrAudioSrc != null && !String.IsNullOrEmpty(attrAudioSrc.Value))
                        {
                            XmlNode parent = audioNode.ParentNode;
                            if (parent != null && parent.Name == "a")
                            {
                                parent = parent.ParentNode;
                            }

                            if (parent != null)
                            {
                                XmlNodeList listOfAudioPeers = parent.ChildNodes;
                                foreach (XmlNode peerNode in listOfAudioPeers)
                                {
                                    if (peerNode.NodeType == XmlNodeType.Element && peerNode.Name == "text")
                                    {
                                        XmlAttributeCollection peerAttrs = peerNode.Attributes;

                                        if (peerAttrs != null)
                                        {
                                            XmlNode attrTextSrc = peerAttrs.GetNamedItem("src");
                                            if (attrTextSrc != null && !String.IsNullOrEmpty(attrTextSrc.Value))
                                            {
                                                int index = attrTextSrc.Value.LastIndexOf('#');
                                                if (index < (attrTextSrc.Value.Length - 1))
                                                {
                                                    string dtbookFragmentId = attrTextSrc.Value.Substring(index + 1);
                                                    core.TreeNode tNode = getTreeNodeWithXmlElementId(dtbookFragmentId);
                                                    if (tNode != null)
                                                    {
                                                        AbstractAudioMedia existingAudioMedia = tNode.GetAudioMedia();
                                                        if (existingAudioMedia != null)
                                                        {
                                                            //Ignore.
                                                            continue; // next audio peers
                                                            //System.Diagnostics.Debug.Fail("TreeNode already has media ??");
                                                        }

                                                        XmlNode attrClipBegin = attributeCol.GetNamedItem("clipBegin");
                                                        XmlNode attrClipEnd = attributeCol.GetNamedItem("clipEnd");

                                                        Media media = null;
                                                        if (attrAudioSrc.Value.EndsWith("wav"))
                                                        {
                                                            string fullWavPathOriginal = Path.Combine(m_outDirectory,
                                                                                              attrAudioSrc.Value);
                                                            if (! File.Exists(fullWavPathOriginal))
                                                            {
                                                                System.Diagnostics.Debug.Fail("File not found: {0}", fullWavPathOriginal);
                                                                continue; // next audio peers
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

                                                                    IWavFormatConverter wavConverter = new WavFormatConverter();

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

                                                                if (attrClipBegin != null &&
                                                                    !string.IsNullOrEmpty(attrClipBegin.Value))
                                                                {
                                                                    clipB = new Time(TimeSpan.Parse(attrClipBegin.Value));
                                                                }
                                                                if (attrClipEnd != null &&
                                                                    !string.IsNullOrEmpty(attrClipEnd.Value))
                                                                {
                                                                    clipE = new Time(TimeSpan.Parse(attrClipEnd.Value));
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
                                                        }
                                                        else
                                                        {
                                                            media = presentation.MediaFactory.CreateExternalAudioMedia();
                                                            ((ExternalAudioMedia)media).Src = attrAudioSrc.Value;
                                                            if (attrClipBegin != null &&
                                                                !string.IsNullOrEmpty(attrClipBegin.Value))
                                                            {
                                                                ((ExternalAudioMedia)media).ClipBegin =
                                                                    new Time(TimeSpan.Parse(attrClipBegin.Value));
                                                            }
                                                            if (attrClipEnd != null &&
                                                                !string.IsNullOrEmpty(attrClipEnd.Value))
                                                            {
                                                                ((ExternalAudioMedia)media).ClipEnd =
                                                                    new Time(TimeSpan.Parse(attrClipEnd.Value));
                                                            }
                                                        }

                                                        if (media != null)
                                                        {
                                                            ChannelsProperty chProp =
                                                                tNode.GetProperty<ChannelsProperty>();
                                                            if (chProp == null)
                                                            {
                                                                chProp =
                                                                    presentation.PropertyFactory.CreateChannelsProperty();
                                                                tNode.AddProperty(chProp);
                                                            }
                                                            chProp.SetMedia(m_audioChannel, media);
                                                        }
                                                        else
                                                        {
                                                            System.Diagnostics.Debug.Fail("Media is neither WAV nor MP3 ?");
                                                        }
                                                        break; // skip scanning of audio node peers
                                                    }
                                                    else
                                                    {
                                                        System.Diagnostics.Debug.Fail("XmlProperty with ID not found ??");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
