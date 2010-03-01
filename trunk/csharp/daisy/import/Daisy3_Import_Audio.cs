using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa.core;
using urakawa.data;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.property.channel;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private AudioChannel m_audioChannel;
        private AudioFormatConvertorSession m_AudioConversionSession;
        private Dictionary<string, FileDataProvider> m_OriginalAudioFile_FileDataProviderMap = new Dictionary<string, FileDataProvider>(); // maps original audio file refered by smil to FileDataProvider of sdk.


        //private bool m_firstTimePCMFormat;

        //private Dictionary<string, string> m_convertedWavFiles = null;
        //private Dictionary<string, string> m_convertedMp3Files = null;

        private void parseSmiles(List<string> spineOfSmilFiles)
        {
            if (RequestCancellation) return;

            if (spineOfSmilFiles == null || spineOfSmilFiles.Count <= 0)
            {
                return;
            }

            //m_firstTimePCMFormat = true;

            m_AudioConversionSession = new AudioFormatConvertorSession(AudioFormatConvertorSession.TEMP_AUDIO_DIRECTORY,
                //m_Project.Presentations.Get(0).DataProviderManager.DataFileDirectoryFullPath,
                m_Project.Presentations.Get(0).MediaDataManager.DefaultPCMFormat);

            m_OriginalAudioFile_FileDataProviderMap.Clear();

            string dirPath = Path.GetDirectoryName(m_Book_FilePath);

            int nSmil = 0;
            foreach (string smilPath in spineOfSmilFiles)
            {
                if (RequestCancellation) return;

                nSmil++;
                reportProgress(100 * nSmil / spineOfSmilFiles.Count,
                    string.Format("Parsing SMIL file [{2}] {0} / {1}...", nSmil, spineOfSmilFiles.Count, smilPath));

                string fullSmilPath = Path.Combine(dirPath, smilPath);
                Console.WriteLine("smil file to be parsed: " + Path.GetFileName(smilPath));

                parseSmil(fullSmilPath);
            }

            //m_AudioConversionSession.DeleteSessionAudioFiles();
            m_AudioConversionSession = null;
        }

        private void parseSmil(string fullSmilPath)
        {
            if (RequestCancellation) return;
            XmlDocument smilXmlDoc = readXmlDocument(fullSmilPath);

            if (RequestCancellation) return;
            //we skip SMIL metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            //parseMetadata(smilXmlDoc);

            //XmlNodeList allTextNodes = smilXmlDoc.GetElementsByTagName("text");
            //if (allTextNodes.Count == 0)
            //{
            //    return;
            //}

            //reportProgress(-1, "Parsing SMIL: [" + Path.GetFileName(fullSmilPath) + "]");

            foreach (XmlNode textNode in XmlDocumentHelper.GetChildrenElementsWithName(smilXmlDoc, true, "text", null, false))
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
                    if (RequestCancellation) return;

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

                        SequenceMedia seqManAudioMedia = textTreeNode.GetManagedAudioSequenceMedia();
                        if (seqManAudioMedia == null)
                        {
                            Debug.Fail("This should never happen !");
                            break;
                        }

                        ManagedAudioMedia managedAudioMedia = textTreeNode.Presentation.MediaFactory.CreateManagedAudioMedia();
                        WavAudioMediaData mediaData = (WavAudioMediaData)textTreeNode.Presentation.MediaDataFactory.CreateAudioMediaData();
                        managedAudioMedia.AudioMediaData = mediaData;

                        foreach (Media seqChild in seqManAudioMedia.ChildMedias.ContentsAs_YieldEnumerable)
                        {
                            ManagedAudioMedia seqManMedia = (ManagedAudioMedia)seqChild;

                            Stream stream = seqManMedia.AudioMediaData.OpenPcmInputStream();
                            try
                            {
                                mediaData.AppendPcmData(stream, null);
                            }
                            finally
                            {
                                stream.Close();
                            }

                            seqManMedia.AudioMediaData.Delete(); // doesn't actually removes the FileDataProviders (need to call Presentation.Cleanup())
                            //textTreeNode.Presentation.DataProviderManager.RemoveDataProvider();
                        }

                        ChannelsProperty chProp = textTreeNode.GetChannelsProperty();
                        chProp.SetMedia(m_audioChannel, null);
                        chProp.SetMedia(m_audioChannel, managedAudioMedia);

                        break;
                    }
                }
            }
        }

        private void addAudio(TreeNode treeNode, XmlNode xmlNode, bool isSequence, string fullSmilPath)
        {
            if (RequestCancellation) return;

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

            if (audioAttrSrc.Value.ToLower().EndsWith("wav"))
            {
                string dirPathBook = Path.GetDirectoryName(m_Book_FilePath);
                FileDataProvider dataProv = null;
                string fullWavPathOriginal = Path.Combine(dirPathBook, audioAttrSrc.Value);
                if (!File.Exists(fullWavPathOriginal))
                {
                    Debug.Fail("File not found: {0}", fullWavPathOriginal);
                    media = null;
                }
                else
                {
                    //bool deleteSrcAfterCompletion = false;

                    string fullWavPath = fullWavPathOriginal;

                    uint dataLength;
                    AudioLibPCMFormat pcmInfo = null;

                    if (m_OriginalAudioFile_FileDataProviderMap.ContainsKey(fullWavPath))
                    {
                        dataProv = m_OriginalAudioFile_FileDataProviderMap[fullWavPath];
                    }
                    else // create FileDataProvider
                    {
                        Stream wavStream = null;
                        try
                        {
                            wavStream = File.Open(fullWavPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                            pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);

                            if (RequestCancellation) return;

                            //if (m_firstTimePCMFormat)
                            //{
                            //    presentation.MediaDataManager.DefaultPCMFormat = new PCMFormatInfo(pcmInfo);
                            //    m_firstTimePCMFormat = false;
                            //}

                            if (!presentation.MediaDataManager.DefaultPCMFormat.Data.IsCompatibleWith(pcmInfo))
                            {
                                wavStream.Close();
                                wavStream = null;

                                reportSubProgress(-1, "Converting audio: [" + Path.GetFileName(fullWavPath) + "]");
                                string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullWavPath);

                                if (RequestCancellation) return;



                                dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                                Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullWavPath) + " = " + dataProv.DataFileRelativePath);
                                dataProv.InitByMovingExistingFile(newfullWavPath);
                                m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                                if (RequestCancellation) return;
                            }
                            else // use original wav file by copying it to data directory
                            {
                                dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                                Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullWavPath) + " = " + dataProv.DataFileRelativePath);
                                reportSubProgress(-1, "Copying audio: [" + Path.GetFileName(fullWavPath) + "]");
                                dataProv.InitByCopyingExistingFile(fullWavPath);
                                m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                                if (RequestCancellation) return;
                            }
                        }
                        finally
                        {
                            if (wavStream != null) wavStream.Close();
                        }
                    }

                } // FileDataProvider  key check ends

                if (RequestCancellation) return;

                media = addAudioWav(dataProv, audioAttrClipBegin, audioAttrClipEnd);
                //media = addAudioWav ( fullWavPath, deleteSrcAfterCompletion, audioAttrClipBegin, audioAttrClipEnd );

            }
            else if (audioAttrSrc.Value.ToLower().EndsWith("mp3"))
            {
                string fullMp3PathOriginal = Path.Combine(dirPath, audioAttrSrc.Value);
                if (!File.Exists(fullMp3PathOriginal))
                {
                    Debug.Fail("File not found: {0}", fullMp3PathOriginal);
                    return;
                }

                if (RequestCancellation) return;
                
                reportSubProgress(-1, "Decoding audio: [" + Path.GetFileName(fullMp3PathOriginal) + "]");

                // At first look, this conversion seems redundant if the m_OriginalAudioFile_FileDataProviderMap already contains the fullMp3PathOriginal,
                // but this is ok because the m_AudioConversionSession won't convert again if already done (i.e. maintains its own mapping)
                // so this is just to get the newfullWavPath for later use down here...
                string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullMp3PathOriginal);

                if (RequestCancellation) return;

                FileDataProvider dataProv = null;
                if (m_OriginalAudioFile_FileDataProviderMap.ContainsKey(fullMp3PathOriginal))
                {
                    dataProv = m_OriginalAudioFile_FileDataProviderMap[fullMp3PathOriginal];
                }
                else
                {
                    dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                    Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullMp3PathOriginal) + " = " + dataProv.DataFileRelativePath);
                    dataProv.InitByMovingExistingFile(newfullWavPath);
                    m_OriginalAudioFile_FileDataProviderMap.Add(fullMp3PathOriginal, dataProv);

                    if (RequestCancellation) return;
                }

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

                    if (RequestCancellation) return;

                    //media = addAudioWav(newfullWavPath, true, audioAttrClipBegin, audioAttrClipEnd);
                    media = addAudioWav(dataProv, audioAttrClipBegin, audioAttrClipEnd);
                }
                //}
            }

            if (RequestCancellation) return;

            if (media == null)
            {
                Debug.Fail("Creating ExternalAudioMedia ??");

                Time timeClipBegin = null;

                media = presentation.MediaFactory.CreateExternalAudioMedia();
                ((ExternalAudioMedia)media).Src = audioAttrSrc.Value;
                if (audioAttrClipBegin != null &&
                    !string.IsNullOrEmpty(audioAttrClipBegin.Value))
                {
                    timeClipBegin = new Time(0);
                    try
                    {
                        timeClipBegin = Time.ParseTimeString(audioAttrClipBegin.Value);
                    }
                    catch (Exception ex)
                    {
                        string str = "CLIP BEGIN TIME PARSE FAIL: " + audioAttrClipBegin.Value;
                        Console.WriteLine(str);
                        Debug.Fail(str);
                    }
                    ((ExternalAudioMedia)media).ClipBegin = timeClipBegin;
                }
                if (audioAttrClipEnd != null &&
                    !string.IsNullOrEmpty(audioAttrClipEnd.Value))
                {
                    Time timeClipEnd = null;
                    try
                    {
                        timeClipEnd = Time.ParseTimeString(audioAttrClipEnd.Value);
                    }
                    catch (Exception ex)
                    {
                        string str = "CLIP END TIME PARSE FAIL: " + audioAttrClipEnd.Value;
                        Console.WriteLine(str);
                        Debug.Fail(str);
                    }

                    if (timeClipEnd != null)
                    {
                        try
                        {
                            ((ExternalAudioMedia)media).ClipEnd = timeClipEnd;
                        }
                        catch (Exception ex)
                        {
                            string str = "CLIP TIME ERROR (end < begin): " + timeClipBegin + " (" + (audioAttrClipBegin != null ? audioAttrClipBegin.Value : "N/A") + ") / " + timeClipEnd + " (" + audioAttrClipEnd.Value + ")";
                            Console.WriteLine(str);
                            //Debug.Fail(str);
                        }
                    }
                }
            }

            if (RequestCancellation) return;

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
                Debug.Fail("Media could not be created !");
            }
        }

        private string GenerateFileFullPath(string directoryPath)
        {
            for (int i = 0; i < 900000; i++)
            {
                string fullPath = Path.Combine(directoryPath, i.ToString());
                fullPath = fullPath + ".wav";
                if (!File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        private Media addAudioWav(FileDataProvider dataProv, XmlNode audioAttrClipBegin, XmlNode audioAttrClipEnd)
        {
            if (RequestCancellation) return null;

            Time clipB = Time.Zero;
            Time clipE = Time.MaxValue;

            if (audioAttrClipBegin != null &&
                !string.IsNullOrEmpty(audioAttrClipBegin.Value))
            {
                try
                {
                    clipB = Time.ParseTimeString(audioAttrClipBegin.Value);
                }
                catch (Exception ex)
                {
                    clipB = new Time(0);
                    string str = "CLIP BEGIN TIME PARSE FAIL: " + audioAttrClipBegin.Value;
                    Console.WriteLine(str);
                    Debug.Fail(str);
                }
            }
            if (audioAttrClipEnd != null &&
                !string.IsNullOrEmpty(audioAttrClipEnd.Value))
            {
                try
                {
                    clipE = Time.ParseTimeString(audioAttrClipEnd.Value);
                }
                catch (Exception ex)
                {
                    clipE = new Time(0);
                    string str = "CLIP END TIME PARSE FAIL: " + audioAttrClipEnd.Value;
                    Console.WriteLine(str);
                    Debug.Fail(str);
                }
            }

            Media media = null;
            Presentation presentation = m_Project.Presentations.Get(0);


            //if (deleteSrcAfterCompletion)
            //{
            presentation.MediaDataFactory.DefaultAudioMediaDataType =
                typeof(WavAudioMediaData);

            WavAudioMediaData mediaData =
                (WavAudioMediaData)
                presentation.MediaDataFactory.CreateAudioMediaData();

            //FileDataProvider dataProv = m_Src_FileDataProviderMap[fullWavPath];
            //System.Windows.Forms.MessageBox.Show ( clipB.ToString () + " : " + clipE.ToString () ) ;
            try
            {
                mediaData.AppendPcmData(dataProv, clipB, clipE);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CLIP TIME ERROR (end < begin ?): " + clipB + " (" + (audioAttrClipBegin != null ? audioAttrClipBegin.Value : "N/A") + ") / " + clipE + " (" + (audioAttrClipEnd != null ? audioAttrClipEnd.Value : "N/A") + ")");
                return null;
            }

            if (RequestCancellation) return null;

            media = presentation.MediaFactory.CreateManagedAudioMedia();
            ((ManagedAudioMedia)media).AudioMediaData = mediaData;
            //}
            /* else
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
            */
            return media;
        }
    }
}
