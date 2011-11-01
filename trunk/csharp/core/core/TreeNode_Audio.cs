using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using urakawa.data;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.property.channel;

namespace urakawa.core
{
    public class TreeNodeAndStreamSelection
    {
        public TreeNode m_TreeNode;
        public long m_LocalStreamLeftMark;
        public long m_LocalStreamRightMark;

        public bool TimeBeginEndEqualClipDuration(Time timeBegin, Time timeEnd, AudioMediaData mediaData)
        {
            bool equal = (
                //timeBegin.IsEqualTo(Time.Zero)
                mediaData.PCMFormat.Data.TimesAreEqualWithOneMillisecondTolerance(timeBegin.AsLocalUnits, Time.Zero.AsLocalUnits)
                )
                         &&
                         (
                             //timeEnd.IsEqualTo(Time.Zero)
                             mediaData.PCMFormat.Data.TimesAreEqualWithOneMillisecondTolerance(timeEnd.AsLocalUnits, Time.Zero.AsLocalUnits)
                             || mediaData.PCMFormat.Data.TimesAreEqualWithOneMillisecondTolerance(timeEnd.GetDifference(timeBegin).AsLocalUnits, mediaData.AudioDuration.AsLocalUnits)
                         );

            if (equal) return true;

            bool rightOk = false;
            if (m_LocalStreamRightMark == -1)
            {
                rightOk = true;
            }
            else
            {
                long timeBytes = mediaData.PCMFormat.Data.ConvertTimeToBytes(mediaData.AudioDuration.AsLocalUnits);
                rightOk = //m_LocalStreamRightMark == timeBytes
                    mediaData.PCMFormat.Data.BytesAreEqualWithOneMillisecondTolerance(m_LocalStreamRightMark, timeBytes)
                    ;
            }

            bool leftOk = m_LocalStreamLeftMark == -1
                //|| m_LocalStreamLeftMark == 0
                || mediaData.PCMFormat.Data.BytesAreEqualWithOneMillisecondTolerance(0, m_LocalStreamLeftMark)
                ;

            return leftOk && rightOk;
        }

        public ManagedAudioMedia ExtractManagedAudioMedia()
        {
            Media audioMedia = m_TreeNode.GetManagedAudioMediaOrSequenceMedia();
            if (audioMedia == null)
            {
                Debug.Fail("This should never happen !");
                throw new Exception("TreeNode doesn't have managed audio media ?!");
            }
            else if (audioMedia is SequenceMedia)
            {
                Debug.Fail("SequenceMedia is normally removed at import time...have you tried re-importing the DAISY book ?");
                throw new NotImplementedException("TODO: implement support for SequenceMedia of ManagedAudioMedia in audio delete functionality !");

                //var seqManAudioMedia = (SequenceMedia)audioMedia;

                //double timeOffset = 0;
                //long sumData = 0;
                //long sumDataPrev = 0;
                //foreach (Media media in seqManAudioMedia.ChildMedias.ContentsAs_YieldEnumerable)
                //{
                //    var manangedMediaSeqItem = (ManagedAudioMedia)media;
                //    if (!manangedMediaSeqItem.HasActualAudioMediaData)
                //    {
                //        continue;
                //    }

                //    AudioMediaData audioData = manangedMediaSeqItem.AudioMediaData;
                //    sumData += audioData.PCMFormat.Data.ConvertTimeToBytes(audioData.AudioDuration.AsMilliseconds);
                //    if (SelectionData.m_LocalStreamLeftMark < sumData)
                //    {
                //        timeOffset = audioData.PCMFormat.Data.ConvertBytesToTime(SelectionData.m_LocalStreamLeftMark - sumDataPrev);

                //        break;
                //    }
                //    sumDataPrev = sumData;
                //}
            }
            else if (audioMedia is ManagedAudioMedia)
            {
                AudioMediaData mediaData = ((ManagedAudioMedia)audioMedia).AudioMediaData;
                if (mediaData == null)
                {
                    Debug.Fail("This should never happen !");
                    throw new Exception("ManagedAudioMedia has empty MediaData ?!");
                }

                Time timeBegin = m_LocalStreamLeftMark == -1
                    ? Time.Zero
                    : new Time(mediaData.PCMFormat.Data.ConvertBytesToTime(m_LocalStreamLeftMark));

                Time timeEnd = m_LocalStreamRightMark == -1
                    ? Time.Zero
                    : new Time(mediaData.PCMFormat.Data.ConvertBytesToTime(m_LocalStreamRightMark));

                if (TimeBeginEndEqualClipDuration(timeBegin, timeEnd, mediaData))
                {
                    return ((ManagedAudioMedia)audioMedia).Copy();
                }
                else
                {
                    ManagedAudioMedia managedAudioMediaBackup = m_TreeNode.Presentation.MediaFactory.CreateManagedAudioMedia();

                    //var mediaDataBackup = (WavAudioMediaData)m_TreeNode.Presentation.MediaDataFactory.CreateAudioMediaData();

                    WavAudioMediaData mediaDataBackup = ((WavAudioMediaData)mediaData).Copy(timeBegin, timeEnd);
                    managedAudioMediaBackup.AudioMediaData = mediaDataBackup;

                    //Stream streamToBackup = timeEnd.IsEqualTo(Time.Zero)
                    //                            ? mediaData.OpenPcmInputStream(timeBegin)
                    //                            : mediaData.OpenPcmInputStream(timeBegin, timeEnd);
                    //try
                    //{
                    //    //Time timeDelta = mediaData.AudioDuration.Substract(new Time(timeBegin.TimeAsMillisecondFloat));
                    //    mediaDataBackup.AppendPcmData(streamToBackup, null);
                    //}
                    //finally
                    //{
                    //    streamToBackup.Close();
                    //}

                    return managedAudioMediaBackup;
                }
            }

            return null;
        }
    }

    public struct TreeNodeAndStreamDataLength
    {
        public TreeNode m_TreeNode;
        public long m_LocalStreamDataLength;
    }

    public struct StreamWithMarkers
    {
        public Stream m_Stream;
        public List<TreeNodeAndStreamDataLength> m_SubStreamMarkers;
    }

    public partial class TreeNode
    {
        public bool HasOrInheritsAudio()
        {
            ManagedAudioMedia media = GetManagedAudioMedia();
            if (media != null)
            {
                return true;
            }

            SequenceMedia seqManagedAudioMedia = GetManagedAudioSequenceMedia();
            if (seqManagedAudioMedia != null)
            {
                return true;
            }

            TreeNode ancerstor = GetFirstAncestorWithManagedAudio();
            if (ancerstor != null)
            {
                return true;
            }

            return false;
        }

        public TreeNode GetFirstAncestorWithManagedAudio()
        {
            if (Parent == null)
            {
                return null;
            }

            Media manMedia = Parent.GetManagedAudioMediaOrSequenceMedia();
            if (manMedia != null)
            {
                return Parent;
            }

            return Parent.GetFirstAncestorWithManagedAudio();
        }

        public TreeNode GetFirstDescendantWithManagedAudio()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                Media manMedia = child.GetManagedAudioMediaOrSequenceMedia();
                if (manMedia != null)
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithManagedAudio();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetPreviousSiblingWithManagedAudio()
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode previous = this;
            while ((previous = previous.PreviousSibling) != null)
            {
                Media manMedia = previous.GetManagedAudioMediaOrSequenceMedia();
                if (manMedia != null)
                {
                    return previous;
                }

                TreeNode previousIn = previous.GetFirstDescendantWithManagedAudio();
                if (previousIn != null)
                {
                    return previousIn;
                }
            }

            return Parent.GetPreviousSiblingWithManagedAudio();
        }

        public TreeNode GetNextSiblingWithManagedAudio()
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                Media manMedia = next.GetManagedAudioMediaOrSequenceMedia();
                if (manMedia != null)
                {
                    return next;
                }

                TreeNode nextIn = next.GetFirstDescendantWithManagedAudio();
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            return Parent.GetNextSiblingWithManagedAudio();
        }

        public StreamWithMarkers? OpenPcmInputStreamOfManagedAudioMedia()
        {
            StreamWithMarkers val;

            ManagedAudioMedia audioMedia = GetManagedAudioMedia();
            if (audioMedia != null && audioMedia.HasActualAudioMediaData)
            {
                val.m_Stream = audioMedia.AudioMediaData.OpenPcmInputStream();
                val.m_SubStreamMarkers = new List<TreeNodeAndStreamDataLength>(1);
                TreeNodeAndStreamDataLength tnasdl = new TreeNodeAndStreamDataLength();
                tnasdl.m_LocalStreamDataLength = val.m_Stream.Length;
                tnasdl.m_TreeNode = this;
                val.m_SubStreamMarkers.Add(tnasdl);
                return val;
            }
            SequenceMedia seq = GetManagedAudioSequenceMedia();
            if (seq != null)
            {
                Stream stream = seq.OpenPcmInputStreamOfManagedAudioMedia();
                if (stream != null)
                {
                    val.m_Stream = stream;
                    val.m_SubStreamMarkers = new List<TreeNodeAndStreamDataLength>(1);
                    TreeNodeAndStreamDataLength tnasdl = new TreeNodeAndStreamDataLength();
                    tnasdl.m_LocalStreamDataLength = val.m_Stream.Length;
                    tnasdl.m_TreeNode = this;
                    val.m_SubStreamMarkers.Add(tnasdl);
                    return val;
                }
            }
            return null;
        }

        public StreamWithMarkers? OpenPcmInputStreamOfManagedAudioMediaFlattened(DelegateAudioPcmStreamFound del)
        {
            StreamWithMarkers? val = OpenPcmInputStreamOfManagedAudioMedia();
            if (val != null)
            {
                if (del != null)
                {
                    del.Invoke(val.GetValueOrDefault().m_Stream.Length);
                }
                return val;
            }

            List<StreamWithMarkers> listStreamsWithMarkers = new List<StreamWithMarkers>();

            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                StreamWithMarkers? childVal = node.OpenPcmInputStreamOfManagedAudioMediaFlattened(del);
                if (childVal != null)
                {
                    listStreamsWithMarkers.Add(childVal.GetValueOrDefault());
                }
            }

            if (listStreamsWithMarkers.Count == 0)
            {
                return null;
            }

            StreamWithMarkers returnVal = new StreamWithMarkers();
            returnVal.m_SubStreamMarkers = new List<TreeNodeAndStreamDataLength>();

            List<Stream> listStreams = new List<Stream>();
            foreach (StreamWithMarkers strct in listStreamsWithMarkers)
            {
                listStreams.Add(strct.m_Stream);
                returnVal.m_SubStreamMarkers.AddRange(strct.m_SubStreamMarkers);
                strct.m_SubStreamMarkers.Clear();
            }

            returnVal.m_Stream = new SequenceStream(listStreams);

            listStreamsWithMarkers.Clear();
            listStreamsWithMarkers = null;

            return returnVal;
        }

        public Media GetManagedAudioMediaOrSequenceMedia()
        {
            ManagedAudioMedia managedAudioMedia = GetManagedAudioMedia();
            if (managedAudioMedia == null)
            {
                return GetManagedAudioSequenceMedia();
            }
            return managedAudioMedia;
        }

        public ManagedAudioMedia GetManagedAudioMedia()
        {
            AbstractAudioMedia media = GetAudioMedia();
            if (media != null)
            {
                return media as ManagedAudioMedia;
            }
            return null;
        }

        public AbstractAudioMedia GetAudioMedia()
        {
            Media med = GetMediaInAudioChannel();
            if (med != null)
            {
                return med as AbstractAudioMedia;
            }
            return null;
        }

        public SequenceMedia GetManagedAudioSequenceMedia()
        {
            SequenceMedia seqAudioMedia = GetAudioSequenceMedia();
            bool isSeqValid = seqAudioMedia != null
                && seqAudioMedia.ChildMedias.Count > 0 && !seqAudioMedia.AllowMultipleTypes;
            if (isSeqValid)
            {
                foreach (Media media in seqAudioMedia.ChildMedias.ContentsAs_Enumerable)
                {
                    if (!(media is ManagedAudioMedia))
                    {
                        isSeqValid = false;
                        break;
                    }
                }
            }
            if (isSeqValid)
            {
                return seqAudioMedia;
            }
            return null;
        }

        public SequenceMedia GetAudioSequenceMedia()
        {
            Media med = GetMediaInAudioChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }

        public Media GetMediaInAudioChannel()
        {
            return GetMediaInChannel<AudioChannel>();
        }

        public Time GetDurationOfManagedAudioMediaFlattened()
        {
            ManagedAudioMedia audioMedia = GetManagedAudioMedia();
            if (audioMedia != null && audioMedia.HasActualAudioMediaData)
            {
                return audioMedia.Duration;
            }

            SequenceMedia seq = GetManagedAudioSequenceMedia();
            if (seq != null)
            {
                Time durSeq = seq.GetDurationOfManagedAudioMedia();
                if (durSeq != null)
                {
                    return durSeq;
                }
            }

            Time dur = new Time();
            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                Time childDur = node.GetDurationOfManagedAudioMediaFlattened();
                if (childDur != null)
                {
                    dur.Add(childDur);
                }
            }
            if (dur.AsLocalUnits <= 0)
            {
                return null;
            }
            return dur;
        }
    }
}
