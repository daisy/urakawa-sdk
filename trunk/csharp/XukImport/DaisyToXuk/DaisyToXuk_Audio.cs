using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa;
using urakawa.core;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.data.utilities;
using urakawa.media.timing;
using urakawa.property.channel;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private AudioChannel m_audioChannel;
        private AudioFormatConvertorSession m_AudioConversionSession;
        private bool m_firstTimePCMFormat;
        //private Dictionary<string, string> m_convertedWavFiles = null;
        //private Dictionary<string, string> m_convertedMp3Files = null;

        private void parseSmiles(List<string> spineOfSmilFiles)
        {
            /*
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
            */

            if (spineOfSmilFiles == null || spineOfSmilFiles.Count <= 0)
            {
                return;
            }

            m_firstTimePCMFormat = true;
            m_AudioConversionSession = new AudioFormatConvertorSession(m_Project.Presentations.Get(0));

            //DirectoryInfo opfParentDir = Directory.GetParent(m_Book_FilePath);
            //string dirPath = opfParentDir.ToString();
            string dirPath = Path.GetDirectoryName(m_Book_FilePath);

            foreach (string smilPath in spineOfSmilFiles)
            {
                string fullSmilPath = Path.Combine(dirPath, smilPath);
                parseSmil(fullSmilPath);
            }

            m_AudioConversionSession.DeleteSessionAudioFiles();
            m_AudioConversionSession = null;
        }

        private void parseSmil(string fullSmilPath)
        {
            XmlDocument smilXmlDoc = readXmlDocument(fullSmilPath);

            //we skip NCX metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            //parseMetadata(smilXmlDoc);

            XmlNodeList allTextNodes = smilXmlDoc.GetElementsByTagName("text");
            if (allTextNodes.Count == 0)
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
                }
                int index = textNodeAttrSrc.Value.LastIndexOf('#');
                if (index == textNodeAttrSrc.Value.Length - 1)
                {
                    return;
                }
                string srcFragmentId = textNodeAttrSrc.Value.Substring(index + 1);
                TreeNode textTreeNode = getTreeNodeWithXmlElementId(srcFragmentId);
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
                    //System.Diagnostics.Debug.Fail("Text node in SMIL has no parallel time container as parent ! {0}", parent.Name);
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
            return; //TODO REMOVE THIS LINE, THIS IS FOR TESTING ONLY !

            //DirectoryInfo parentDir = Directory.GetParent(m_Book_FilePath);
            //string dirPath = parentDir.ToString();
            string dirPath = Path.GetDirectoryName(m_Book_FilePath);

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

            Presentation presentation = m_Project.Presentations.Get(0);
            Media media = null;

            if (audioAttrSrc.Value.EndsWith("wav"))
            {
                media = addAudioWav(audioAttrSrc.Value, audioAttrClipBegin, audioAttrClipEnd);
            }
            else if (audioAttrSrc.Value.EndsWith("mp3"))
            {
                string fullMp3PathOriginal = Path.Combine(dirPath, audioAttrSrc.Value);
                if (!File.Exists(fullMp3PathOriginal))
                {
                    System.Diagnostics.Debug.Fail("File not found: {0}", fullMp3PathOriginal);
                    return;
                }

                //if (m_convertedMp3Files.ContainsKey(fullMp3PathOriginal))
                //{
                //string fullWavPath = m_convertedMp3Files[fullMp3PathOriginal];
                //media = addAudioWav(fullWavPath, audioAttrClipBegin, audioAttrClipEnd);
                //}
                //else
                //{
                //IWavFormatConverter wavConverter = new WavFormatConverter(true);

                //string newfullWavPath = wavConverter.UnCompressMp3File(fullMp3PathOriginal, Path.GetDirectoryName(fullMp3PathOriginal), presentation.MediaDataManager.DefaultPCMFormat);
                string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullMp3PathOriginal);

                if (newfullWavPath != null)
                {
                    //m_convertedMp3Files.Add(fullMp3PathOriginal, newfullWavPath);
                    media = addAudioWav(newfullWavPath, audioAttrClipBegin, audioAttrClipEnd);
                }
                //}
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
                    mediaSeq.ChildMedias.Insert(mediaSeq.ChildMedias.Count, media);
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
            Presentation presentation = m_Project.Presentations.Get(0);

            string dirPath = Path.GetDirectoryName(m_Book_FilePath);
            string fullWavPathOriginal = Path.Combine(dirPath, src);
            if (!File.Exists(fullWavPathOriginal))
            {
                System.Diagnostics.Debug.Fail("File not found: {0}", fullWavPathOriginal);
                return null;
            }
            string fullWavPath = fullWavPathOriginal;
            //if (m_convertedWavFiles.ContainsKey(fullWavPathOriginal))
            //{
            //fullWavPath = m_convertedWavFiles[fullWavPathOriginal];
            //}
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

                    //if (m_convertedWavFiles.ContainsKey(fullWavPathOriginal))
                    //{
                    //throw new Exception("The previously converted WAV file is not with the correct PCM format !!");
                    //}

                    //IWavFormatConverter wavConverter = new WavFormatConverter(true);

                    //string newfullWavPath = wavConverter.ConvertSampleRate(fullWavPath, Path.GetDirectoryName(fullWavPath), presentation.MediaDataManager.DefaultPCMFormat);
                    string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullWavPath);

                    //m_convertedWavFiles.Add(fullWavPath, newfullWavPath);

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
    }
}
