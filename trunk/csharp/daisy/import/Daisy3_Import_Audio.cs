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
using urakawa.xuk;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        protected AudioFormatConvertorSession m_AudioConversionSession;
        protected Dictionary<string, FileDataProvider> m_OriginalAudioFile_FileDataProviderMap = new Dictionary<string, FileDataProvider>(); // maps original audio file refered by smil to FileDataProvider of sdk.
        protected List<TreeNode> TreenodesWithoutManagedAudioMediaData;

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

            m_AudioConversionSession = new AudioFormatConvertorSession(
                //AudioFormatConvertorSession.TEMP_AUDIO_DIRECTORY,
                m_Project.Presentations.Get(0).DataProviderManager.DataFileDirectoryFullPath,
                m_Project.Presentations.Get(0).MediaDataManager.DefaultPCMFormat,
                m_autoDetectPcmFormat,
                m_SkipACM);
            AddSubCancellable(m_AudioConversionSession);

            m_OriginalAudioFile_FileDataProviderMap.Clear();
            TreenodesWithoutManagedAudioMediaData = new List<TreeNode>();

            string dirPath = Path.GetDirectoryName(m_Book_FilePath);

            int nSmil = 0;
            foreach (string smilPath in spineOfSmilFiles)
            {
                if (RequestCancellation) return;

                nSmil++;
                reportProgress_Throttle(100 * nSmil / spineOfSmilFiles.Count,
                    string.Format(UrakawaSDK_daisy_Lang.ParsingSmilFile, nSmil, spineOfSmilFiles.Count, smilPath));

                string fullSmilPath = Path.Combine(dirPath, smilPath);
                Console.WriteLine("smil file to be parsed: " + Path.GetFileName(smilPath));

                //parseSmil(fullSmilPath); // commented for ncx import feature
                if (!AudioNCXImport)
                {
                    parseSmil(fullSmilPath);
                }
                else
                {
                    parseSmilForNCX(fullSmilPath);
                }

            }

            RemoveSubCancellable(m_AudioConversionSession);
            //m_AudioConversionSession.DeleteSessionAudioFiles();
            m_AudioConversionSession = null;
        }

        private void parseSmil(string fullSmilPath)
        {
            if (RequestCancellation) return;
            XmlDocument smilXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullSmilPath, false, false);

            if (RequestCancellation) return;
            //we skip SMIL metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            //parseMetadata(smilXmlDoc);

            //XmlNodeList allTextNodes = smilXmlDoc.GetElementsByTagName("text");
            //if (allTextNodes.Count == 0)
            //{
            //    return;
            //}

            //reportProgress(-1, "Parsing SMIL: [" + Path.GetFileName(fullSmilPath) + "]");

            foreach (XmlNode textNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(smilXmlDoc, true, "text", null, false))
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

                string src = FileDataProvider.UriDecode(textNodeAttrSrc.Value);


                int index = src.LastIndexOf('#');
                if (index == src.Length - 1)
                {
                    return;
                }
                string srcFragmentId = src.Substring(index + 1);
                TreeNode textTreeNode = m_Project.Presentations.Get(0).RootNode.GetFirstDescendantOrSelfWithXmlID(srcFragmentId);
                if (textTreeNode == null)
                {
                    continue;
                }

                ManagedAudioMedia textTreeNodeAudio = textTreeNode.GetManagedAudioMedia();
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
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                        XmlNodeList seqChildren = textPeerNode.ChildNodes;
                        foreach (XmlNode seqChild in seqChildren)
                        {
                            if (seqChild.LocalName == "audio")
                            {
                                addAudio(textTreeNode, seqChild, true, fullSmilPath);
                            }
                        }
#if ENABLE_SEQ_MEDIA
                        SequenceMedia seqManAudioMedia = textTreeNode.GetManagedAudioSequenceMedia();
                        if (seqManAudioMedia == null)
                        {
                            Debug.Fail("This should never happen !");
                            break;
                        }

                        ManagedAudioMedia managedAudioMedia = textTreeNode.Presentation.MediaFactory.CreateManagedAudioMedia();
                        AudioMediaData mediaData = textTreeNode.Presentation.MediaDataFactory.CreateAudioMediaData();
                        managedAudioMedia.AudioMediaData = mediaData;

                        foreach (Media seqChild in seqManAudioMedia.ChildMedias.ContentsAs_Enumerable)
                        {
                            ManagedAudioMedia seqManMedia = (ManagedAudioMedia)seqChild;

                            // WARNING: WavAudioMediaData implementation differs from AudioMediaData:
                            // the latter is naive and performs a stream binary copy, the latter is optimized and re-uses existing WavClips. 
                            //  WARNING 2: The audio data from the given parameter gets emptied !
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

                        ChannelsProperty chProp = textTreeNode.GetChannelsProperty();
                        chProp.SetMedia(m_audioChannel, null);
                        chProp.SetMedia(m_audioChannel, managedAudioMedia);
#endif //ENABLE_SEQ_MEDIA
                        break;
                    }
                }
            }
        }

        protected void addAudio(TreeNode treeNode, XmlNode xmlNode, bool isSequence, string fullSmilPath)
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

            string src = FileDataProvider.UriDecode(audioAttrSrc.Value);

            XmlNode audioAttrClipBegin = audioAttrs.GetNamedItem("clipBegin");
            XmlNode audioAttrClipEnd = audioAttrs.GetNamedItem("clipEnd");

            Presentation presentation = treeNode.Presentation; // m_Project.Presentations.Get(0);
            ManagedAudioMedia media = null;




            string fullPath = Path.Combine(dirPath, src);
            fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');
            addOPF_GlobalAssetPath(fullPath);

            string ext = Path.GetExtension(src);


            if (ext.Equals(DataProviderFactory.AUDIO_WAV_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                FileDataProvider dataProv = null;

                if (!File.Exists(fullPath))
                {
                    Debug.Fail("File not found: " + fullPath);
                    media = null;
                }
                else
                {
                    //bool deleteSrcAfterCompletion = false;

                    string fullWavPath = fullPath;

                    FileDataProvider obj;
                    m_OriginalAudioFile_FileDataProviderMap.TryGetValue(fullWavPath, out obj);

                    if (obj != null)  //m_OriginalAudioFile_FileDataProviderMap.ContainsKey(fullWavPath))
                    {
                        if (m_AudioConversionSession.FirstDiscoveredPCMFormat == null)
                        {
                            DebugFix.Assert(obj.Presentation != presentation);

                            Object appData = obj.AppData;

                            DebugFix.Assert(appData != null);

                            if (appData != null && appData is WavClip.PcmFormatAndTime)
                            {
                                m_AudioConversionSession.FirstDiscoveredPCMFormat = new AudioLibPCMFormat();
                                m_AudioConversionSession.FirstDiscoveredPCMFormat.CopyFrom(((WavClip.PcmFormatAndTime)appData).mFormat);
                            }
                        }

                        if (obj.Presentation != presentation)
                        {
                            dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);


                            reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.CopyingAudio, Path.GetFileName(obj.DataFileFullPath)));


                            dataProv.InitByCopyingExistingFile(obj.DataFileFullPath);

                            //m_AudioConversionSession.RelocateDestinationFilePath(newfullWavPath, dataProv.DataFileFullPath);

                            m_OriginalAudioFile_FileDataProviderMap.Remove(fullWavPath);
                            m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                            Object appData = obj.AppData;

                            DebugFix.Assert(appData != null);

                            if (appData != null && appData is WavClip.PcmFormatAndTime)
                            {
                                dataProv.AppData = new WavClip.PcmFormatAndTime(((WavClip.PcmFormatAndTime)appData).mFormat, ((WavClip.PcmFormatAndTime)appData).mTime);
                            }
                        }
                        else
                        {
                            dataProv = obj; // m_OriginalAudioFile_FileDataProviderMap[fullWavPath];
                        }
                    }
                    else // create FileDataProvider
                    {
                        Stream wavStream = null;
                        try
                        {
                            wavStream = File.Open(fullWavPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                            uint dataLength;
                            AudioLibPCMFormat pcmInfo = null;

                            pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);

                            if (m_AudioConversionSession.FirstDiscoveredPCMFormat == null)
                            {
                                //m_AudioConversionSession.FirstDiscoveredPCMFormat = new PCMFormatInfo(pcmInfo);
                                m_AudioConversionSession.FirstDiscoveredPCMFormat = new AudioLibPCMFormat();
                                m_AudioConversionSession.FirstDiscoveredPCMFormat.CopyFrom(pcmInfo);
                            }


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

                                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ConvertingAudio, Path.GetFileName(fullWavPath)));
                                string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullWavPath);

                                if (RequestCancellation) return;



                                dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                                //Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullWavPath) + " = " + dataProv.DataFileRelativePath);
                                dataProv.InitByMovingExistingFile(newfullWavPath);

                                m_AudioConversionSession.RelocateDestinationFilePath(newfullWavPath, dataProv.DataFileFullPath);

                                m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                                if (RequestCancellation) return;
                            }
                            else // use original wav file by copying it to data directory
                            {
                                dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                                //Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullWavPath) + " = " + dataProv.DataFileRelativePath);
                                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.CopyingAudio, Path.GetFileName(fullWavPath)));
                                dataProv.InitByCopyingExistingFile(fullWavPath);
                                m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                                if (RequestCancellation) return;
                            }
                        }
                        finally
                        {
                            if (wavStream != null)
                            {
                                wavStream.Close();
                            }
                        }
                    }

                } // FileDataProvider  key check ends

                if (RequestCancellation) return;

                media = addAudioWav(dataProv, audioAttrClipBegin, audioAttrClipEnd, treeNode);
                //media = addAudioWav ( fullWavPath, deleteSrcAfterCompletion, audioAttrClipBegin, audioAttrClipEnd );

            }
            else if (ext.Equals(DataProviderFactory.AUDIO_MP3_EXTENSION, StringComparison.OrdinalIgnoreCase)
                || ext.Equals(DataProviderFactory.AUDIO_MP4_EXTENSION, StringComparison.OrdinalIgnoreCase)
                || ext.Equals(DataProviderFactory.AUDIO_MP4_EXTENSION_, StringComparison.OrdinalIgnoreCase))
            {
                if (!File.Exists(fullPath))
                {
                    Debug.Fail("File not found: {0}", fullPath);
                    return;
                }

                if (RequestCancellation) return;

                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.DecodingAudio, Path.GetFileName(fullPath)));


                if (RequestCancellation) return;

                string fullMp34PathOriginal = fullPath;

                FileDataProvider obj;
                m_OriginalAudioFile_FileDataProviderMap.TryGetValue(fullMp34PathOriginal, out obj);

                FileDataProvider dataProv = null;
                if (obj != null) //m_OriginalAudioFile_FileDataProviderMap.ContainsKey(fullMp3PathOriginal))
                {
                    if (m_AudioConversionSession.FirstDiscoveredPCMFormat == null)
                    {
                        DebugFix.Assert(obj.Presentation != presentation);

                        Object appData = obj.AppData;

                        DebugFix.Assert(appData != null);

                        if (appData != null && appData is WavClip.PcmFormatAndTime)
                        {
                            m_AudioConversionSession.FirstDiscoveredPCMFormat = new AudioLibPCMFormat();
                            m_AudioConversionSession.FirstDiscoveredPCMFormat.CopyFrom(((WavClip.PcmFormatAndTime)appData).mFormat);
                        }
                    }

                    if (obj.Presentation != presentation)
                    {
                        dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);

                        reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.CopyingAudio, Path.GetFileName(obj.DataFileFullPath)));

                        dataProv.InitByCopyingExistingFile(obj.DataFileFullPath);

                        //m_AudioConversionSession.RelocateDestinationFilePath(newfullWavPath, dataProv.DataFileFullPath);

                        m_OriginalAudioFile_FileDataProviderMap.Remove(fullMp34PathOriginal);
                        m_OriginalAudioFile_FileDataProviderMap.Add(fullMp34PathOriginal, dataProv);

                        Object appData = obj.AppData;

                        DebugFix.Assert(appData != null);

                        if (appData != null && appData is WavClip.PcmFormatAndTime)
                        {
                            dataProv.AppData = new WavClip.PcmFormatAndTime(((WavClip.PcmFormatAndTime)appData).mFormat, ((WavClip.PcmFormatAndTime)appData).mTime);
                        }
                    }
                    else
                    {
                        dataProv = obj; // m_OriginalAudioFile_FileDataProviderMap[fullMp3PathOriginal];
                    }
                }
                else
                {
                    string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullMp34PathOriginal);

                    dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                    //Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullMp34PathOriginal) + " = " + dataProv.DataFileRelativePath);
                    dataProv.InitByMovingExistingFile(newfullWavPath);

                    m_AudioConversionSession.RelocateDestinationFilePath(newfullWavPath, dataProv.DataFileFullPath);

                    m_OriginalAudioFile_FileDataProviderMap.Add(fullMp34PathOriginal, dataProv);

                    if (RequestCancellation) return;
                }

                if (dataProv != null)
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
                    media = addAudioWav(dataProv, audioAttrClipBegin, audioAttrClipEnd, treeNode);

                    if (RequestCancellation) return;

                    if (media == null)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }
                }
                //}
            }

            if (RequestCancellation) return;

            if (media == null)
            {
                if (!TreenodesWithoutManagedAudioMediaData.Contains(treeNode))
                {
                    TreenodesWithoutManagedAudioMediaData.Add(treeNode);
                }

                Debug.Fail("Creating ExternalAudioMedia ??");

                Time timeClipBegin = null;

                ExternalAudioMedia exmedia = presentation.MediaFactory.CreateExternalAudioMedia();
                exmedia.Src = src;
                if (audioAttrClipBegin != null &&
                    !string.IsNullOrEmpty(audioAttrClipBegin.Value))
                {
                    timeClipBegin = new Time();
                    try
                    {
                        timeClipBegin = new Time(audioAttrClipBegin.Value);
                    }
                    catch (Exception ex)
                    {
                        string str = "CLIP BEGIN TIME PARSE FAIL: " + audioAttrClipBegin.Value;
                        Console.WriteLine(str);
                        Debug.Fail(str);
                    }
                    exmedia.ClipBegin = timeClipBegin;
                }
                if (audioAttrClipEnd != null &&
                    !string.IsNullOrEmpty(audioAttrClipEnd.Value))
                {
                    Time timeClipEnd = null;
                    try
                    {
                        timeClipEnd = new Time(audioAttrClipEnd.Value);
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
                            exmedia.ClipEnd = timeClipEnd;
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
                    treeNode.GetChannelsProperty();
                if (chProp == null)
                {
                    chProp =
                        presentation.PropertyFactory.CreateChannelsProperty();
                    treeNode.AddProperty(chProp);
                }
                if (isSequence)
                {
#if ENABLE_SEQ_MEDIA
                    SequenceMedia mediaSeq = chProp.GetMedia(m_audioChannel) as SequenceMedia;
                    if (mediaSeq == null)
                    {
                        mediaSeq = presentation.MediaFactory.CreateSequenceMedia();
                        mediaSeq.AllowMultipleTypes = false;
                        chProp.SetMedia(m_audioChannel, mediaSeq);
                    }
                    mediaSeq.ChildMedias.Insert(mediaSeq.ChildMedias.Count, media);
#else
                    ManagedAudioMedia existingMedia = chProp.GetMedia(presentation.ChannelsManager.GetOrCreateAudioChannel()) as ManagedAudioMedia;
                    if (existingMedia == null)
                    {
                        chProp.SetMedia(presentation.ChannelsManager.GetOrCreateAudioChannel(), media);
                    }
                    else
                    {
                        // WARNING: WavAudioMediaData implementation differs from AudioMediaData:
                        // the latter is naive and performs a stream binary copy, the latter is optimized and re-uses existing WavClips. 
                        //  WARNING 2: The audio data from the given parameter gets emptied !
                        existingMedia.AudioMediaData.MergeWith(media.AudioMediaData);

                        //Stream stream = seqManMedia.AudioMediaData.OpenPcmInputStream();
                        //try
                        //{
                        //    mediaData.AppendPcmData(stream, null);
                        //}
                        //finally
                        //{
                        //    stream.Close();
                        //}

                    }
#endif //ENABLE_SEQ_MEDIA
                }
                else
                {
                    //#if DEBUG
                    //                    ((WavAudioMediaData) media.AudioMediaData).checkWavClips();
                    //#endif //DEBUG
                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateAudioChannel(), media);
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
                fullPath = fullPath + DataProviderFactory.AUDIO_WAV_EXTENSION;
                if (!File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        protected virtual void clipEndAdjustedToNull(Time clipB, Time clipE, Time duration, TreeNode treeNode)
        {
        }

        private ManagedAudioMedia addAudioWav(FileDataProvider dataProv, XmlNode audioAttrClipBegin, XmlNode audioAttrClipEnd, TreeNode treeNode)
        {
            if (m_autoDetectPcmFormat
                && m_AudioConversionSession.FirstDiscoveredPCMFormat != null
                && !m_AudioConversionSession.FirstDiscoveredPCMFormat.IsCompatibleWith(treeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data))
            {
                PCMFormatInfo pcmFormat = treeNode.Presentation.MediaDataManager.DefaultPCMFormat; //.Copy();
                pcmFormat.Data.CopyFrom(m_AudioConversionSession.FirstDiscoveredPCMFormat);
                //pcmFormat.Data.SampleRate = (ushort) m_audioProjectSampleRate;
                //pcmFormat.Data.NumberOfChannels = m_audioStereo ? (ushort) 2 : (ushort) 1;
                treeNode.Presentation.MediaDataManager.DefaultPCMFormat = pcmFormat;
            }

            if (RequestCancellation) return null;

            Time clipB = Time.Zero;
            Time clipE = Time.MaxValue;

            if (audioAttrClipBegin != null &&
                !string.IsNullOrEmpty(audioAttrClipBegin.Value))
            {
                try
                {
                    clipB = new Time(audioAttrClipBegin.Value);
                }
                catch (Exception ex)
                {
                    clipB = new Time();
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
                    clipE = new Time(audioAttrClipEnd.Value);
                }
                catch (Exception ex)
                {
                    clipE = new Time();
                    string str = "CLIP END TIME PARSE FAIL: " + audioAttrClipEnd.Value;
                    Console.WriteLine(str);
                    Debug.Fail(str);
                }
            }

            ManagedAudioMedia media = null;
            Presentation presentation = treeNode.Presentation; // m_Project.Presentations.Get(0);


            //if (deleteSrcAfterCompletion)
            //{
            WavAudioMediaData mediaData =
                (WavAudioMediaData)
                presentation.MediaDataFactory.CreateAudioMediaData();

            //  mediaData.AudioDuration DOES NOT WORK BECAUSE DEPENDS ON WAVCLIPS LIST!!!
            WavClip wavClip = new WavClip(dataProv);

            Time newClipE = clipE.Copy();
            if (newClipE.IsGreaterThan(wavClip.MediaDuration))
            {
                clipEndAdjustedToNull(clipB, newClipE, wavClip.MediaDuration, treeNode);
                //newClipE = wavClip.MediaDuration;
                newClipE = null;
            }

            //FileDataProvider dataProv = m_Src_FileDataProviderMap[fullWavPath];
            //System.Windows.Forms.MessageBox.Show ( clipB.ToString () + " : " + clipE.ToString () ) ;

            //bool isClipEndError = false;
            try
            {
                mediaData.AppendPcmData(dataProv, clipB, newClipE);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG
                Console.WriteLine("CLIP TIME ERROR1 (end < begin ?): " + clipB + " (" + (audioAttrClipBegin != null ? audioAttrClipBegin.Value : "N/A") + ") / " + clipE + " (" + (audioAttrClipEnd != null ? audioAttrClipEnd.Value : "N/A") + ") === " + wavClip.MediaDuration);

                //if (ex is exception.MethodParameterIsOutOfBoundsException && clipB != null && clipE != null && clipB.IsLessThanOrEqualTo(clipE))
                //{
                //    isClipEndError = true;
                //}
                //else
                //{
                //    Console.WriteLine("CLIP TIME ERROR1 (end < begin ?): " + clipB + " (" + (audioAttrClipBegin != null ? audioAttrClipBegin.Value : "N/A") + ") / " + clipE + " (" + (audioAttrClipEnd != null ? audioAttrClipEnd.Value : "N/A") + ")");
                //    return null;
                //}
            }

            //if (isClipEndError)
            //{
            //    // reduce clip end by 1 millisecond for rounding off tolerance
            //    isClipEndError = addAudioWavWithEndOfFileTolerance(mediaData, dataProv, clipB, clipE, treeNode);
            //    if (isClipEndError)
            //    {
            //        Console.WriteLine("CLIP TIME ERROR2 (end < begin ?): " + clipB + " (" + (audioAttrClipBegin != null ? audioAttrClipBegin.Value : "N/A") + ") / " + clipE + " (" + (audioAttrClipEnd != null ? audioAttrClipEnd.Value : "N/A") + ")");
            //        return null;
            //    }
            //}

            if (RequestCancellation) return null;

            media = presentation.MediaFactory.CreateManagedAudioMedia();
            media.AudioMediaData = mediaData;
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

                    Time clipDuration = new Time(pcmInfo.ConvertBytesToTime(dataLength));
                    if (!clipB.IsEqualTo(Time.Zero) || !clipE.IsEqualTo(Time.MaxValue))
                    {
                        clipDuration = clipE.GetTime(clipB);
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

        //protected virtual bool addAudioWavWithEndOfFileTolerance(WavAudioMediaData mediaData, FileDataProvider dataProv, Time clipB, Time clipE, TreeNode treeNode)
        //{
        //    bool isClipEndError = false;
        //    // reduce clip end by 1 millisecond for rounding off tolerance
        //    Console.WriteLine("Error encountered: reducing original clip by 1ms" + clipE);
        //    clipE.Substract(new Time(AudioLibPCMFormat.TIME_UNIT));
        //    Console.WriteLine("new clip " + clipE);
        //    try
        //    {
        //        mediaData.AppendPcmData(dataProv, clipB, clipE);
        //        isClipEndError = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        isClipEndError = true;
        //        Console.WriteLine("clip error after providing tolerance of 1ms also");
        //        //return null;

        //    }
        //    return isClipEndError;
        //}
    }
}
