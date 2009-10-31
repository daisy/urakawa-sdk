using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
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
        private Dictionary<string, FileDataProvider> m_Src_FileDataProviderMap = new Dictionary<string,FileDataProvider> ();
        private Dictionary<string, string> m_MovedFilePaths = new Dictionary<string, string> ();

        //private bool m_firstTimePCMFormat;
        
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

            //m_firstTimePCMFormat = true;

            m_AudioConversionSession = new AudioFormatConvertorSession(m_Project.Presentations.Get(0));
            m_Src_FileDataProviderMap.Clear ();
            m_MovedFilePaths.Clear ();
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

            //we skip SMIL metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            //parseMetadata(smilXmlDoc);

            //XmlNodeList allTextNodes = smilXmlDoc.GetElementsByTagName("text");
            //if (allTextNodes.Count == 0)
            //{
            //    return;
            //}
            foreach (XmlNode textNode in getChildrenElementsWithName(smilXmlDoc, true, "text", null, false))
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
                if (parent != null && parent.LocalName == "a")
                {
                    parent = parent.ParentNode;
                }
                if (parent == null)
                {
                    continue;
                }
                if (parent.LocalName != "par")
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
                    if (textPeerNode.LocalName == "audio")
                    {
                        addAudio(textTreeNode, textPeerNode, false, fullSmilPath);
                        break;
                    }
                    else if (textPeerNode.LocalName == "a")
                    {
                        XmlNodeList aChildren = textPeerNode.ChildNodes;
                        foreach (XmlNode aChild in aChildren)
                        {
                            if (aChild.LocalName == "audio")
                            {
                                addAudio(textTreeNode, aChild, false, fullSmilPath);
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
                                addAudio(textTreeNode, seqChild, true, fullSmilPath);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void addAudio(TreeNode treeNode, XmlNode xmlNode, bool isSequence, string fullSmilPath)
        {
            //DirectoryInfo parentDir = Directory.GetParent(m_Book_FilePath);
            //string dirPath = parentDir.ToString();
            string dirPath = Path.GetDirectoryName(fullSmilPath);

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
                string dirPathBook = Path.GetDirectoryName(m_Book_FilePath);
                string fullWavPathOriginal = Path.Combine(dirPathBook, audioAttrSrc.Value);
                if (!File.Exists(fullWavPathOriginal))
                {
                    System.Diagnostics.Debug.Fail("File not found: {0}", fullWavPathOriginal);
                    media = null;
                }
                else
                {
                    bool deleteSrcAfterCompletion = false;

                    string fullWavPath = fullWavPathOriginal;

                    uint dataLength;
                    AudioLibPCMFormat pcmInfo = null;

                    Stream wavStream = null;
                    try
                    {
                        wavStream = File.Open(fullWavPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                        pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);


                        //if (m_firstTimePCMFormat)
                        //{
                        //    presentation.MediaDataManager.DefaultPCMFormat = new PCMFormatInfo(pcmInfo);
                        //    m_firstTimePCMFormat = false;
                        //}

                        if (!presentation.MediaDataManager.DefaultPCMFormat.Data.IsCompatibleWith(pcmInfo))
                        {
                            wavStream.Close();
                            wavStream = null;

                            //if (m_convertedWavFiles.ContainsKey(fullWavPathOriginal))
                            //{
                            //throw new Exception("The previously converted WAV file is not with the correct PCM format !!");
                            //}

                            //IWavFormatConverter wavConverter = new WavFormatConverter(true);

                            //string newfullWavPath = wavConverter.ConvertSampleRate(fullWavPath, Path.GetDirectoryName(fullWavPath), presentation.MediaDataManager.DefaultPCMFormat);
                            fullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullWavPath);

                            deleteSrcAfterCompletion = true;

                            //m_convertedWavFiles.Add(fullWavPath, newfullWavPath);
//#if DEBUG
//                            wavStream = File.Open(fullWavPath, FileMode.Open, FileAccess.Read, FileShare.Read);

//                            pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);

//                            if (!presentation.MediaDataManager.DefaultPCMFormat.Data.IsCompatibleWith(pcmInfo))
//                            {
//                                //wavStream.Close(); already done in FINALLY !
//                                throw new Exception("Could not convert the WAV PCM format !!");
//                            }
//#endif //DEBUG
                        }
                    }
                    finally
                    {
                        if (wavStream != null) wavStream.Close();
                    }

                    media = addAudioWav(fullWavPath, deleteSrcAfterCompletion, audioAttrClipBegin, audioAttrClipEnd);
                }
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
                    //if (m_firstTimePCMFormat)
                    //{
                    //    Stream wavStream = null;
                    //    try
                    //    {
                    //        wavStream = File.Open(newfullWavPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    //        uint dataLength;
                    //        AudioLibPCMFormat pcmInfo = null;

                    //        pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);

                    //        presentation.MediaDataManager.DefaultPCMFormat = new PCMFormatInfo(pcmInfo);
                    //    }
                    //    finally
                    //    {
                    //        if (wavStream != null) wavStream.Close();
                    //        m_firstTimePCMFormat = false;
                    //    }
                    //}

                    //m_convertedMp3Files.Add(fullMp3PathOriginal, newfullWavPath);
                if (m_MovedFilePaths.ContainsKey ( newfullWavPath ))
                    {
                    newfullWavPath = m_MovedFilePaths[newfullWavPath];
                    }
                else
                    {
                    FileDataProvider dataProv = (FileDataProvider)presentation.DataProviderFactory.Create ( DataProviderFactory.AUDIO_WAV_MIME_TYPE );
                    //System.Windows.Forms.MessageBox.Show ( newfullWavPath );
                    dataProv.InitByMovingExistingFile ( newfullWavPath );
                    m_MovedFilePaths.Add ( newfullWavPath, dataProv.DataFileFullPath );
                    m_Src_FileDataProviderMap.Add ( dataProv.DataFileFullPath, dataProv );
                    newfullWavPath = dataProv.DataFileFullPath;
                    }
                

                    media = addAudioWav(newfullWavPath, true, audioAttrClipBegin, audioAttrClipEnd);
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

        private Media addAudioWav(string fullWavPath, bool deleteSrcAfterCompletion, XmlNode audioAttrClipBegin, XmlNode audioAttrClipEnd)
        {
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
            
            Media media = null;
            Presentation presentation = m_Project.Presentations.Get(0);

            if (deleteSrcAfterCompletion)
            {
                presentation.MediaDataFactory.DefaultAudioMediaDataType =
                    typeof(WavAudioMediaData);

                WavAudioMediaData mediaData =
                    (WavAudioMediaData)
                    presentation.MediaDataFactory.CreateAudioMediaData();

                //FileDataProvider dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                //dataProv.InitByMovingExistingFile(fullWavPath);

                //m_AudioConversionSession.RelocateDestinationFilePath(fullWavPath, dataProv.DataFileFullPath);

                FileDataProvider dataProv = m_Src_FileDataProviderMap[fullWavPath];
                mediaData.AppendPcmData(dataProv, clipB, clipE);

                media = presentation.MediaFactory.CreateManagedAudioMedia();
                ((ManagedAudioMedia)media).AudioMediaData = mediaData;
            }
            else
            {
                uint dataLength;
                AudioLibPCMFormat pcmInfo = null;
                Stream wavStream = null;
                try
                {
                    wavStream = File.Open(fullWavPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);

                    //if (m_firstTimePCMFormat)
                    //{
                    //    presentation.MediaDataManager.DefaultPCMFormat = new PCMFormatInfo(pcmInfo);
                    //    m_firstTimePCMFormat = false;
                    //}

                    TimeDelta clipDuration = new TimeDelta(pcmInfo.ConvertBytesToTime(dataLength));
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
                        byteOffset = pcmInfo.ConvertTimeToBytes(clipB.TimeAsMillisecondFloat);
                    }
                    if (byteOffset > 0)
                    {
                        wavStream.Seek(byteOffset, SeekOrigin.Current);
                    }

                    presentation.MediaDataFactory.DefaultAudioMediaDataType =
                        typeof (WavAudioMediaData);

                    WavAudioMediaData mediaData =
                        (WavAudioMediaData)
                        presentation.MediaDataFactory.CreateAudioMediaData();

                    mediaData.InsertPcmData(wavStream, Time.Zero, clipDuration);

                    media = presentation.MediaFactory.CreateManagedAudioMedia();
                    ((ManagedAudioMedia) media).AudioMediaData = mediaData;
                }
                finally
                {
                    if (wavStream != null) wavStream.Close();
                }
            }

            return media;
        }
    }
}
