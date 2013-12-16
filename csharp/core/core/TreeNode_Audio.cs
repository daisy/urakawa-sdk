using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AudioLib;
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
                timeBegin.IsEqualTo(Time.Zero)
                //mediaData.PCMFormat.Data.TimesAreEqualWithMillisecondsTolerance(timeBegin.AsLocalUnits, Time.Zero.AsLocalUnits)
                )
                         &&
                         (
                             timeEnd.IsEqualTo(Time.Zero)
                             ||
                             timeEnd.GetDifference(timeBegin).IsEqualTo(mediaData.AudioDuration)
                //mediaData.PCMFormat.Data.TimesAreEqualWithMillisecondsTolerance(timeEnd.AsLocalUnits, Time.Zero.AsLocalUnits)
                //|| mediaData.PCMFormat.Data.TimesAreEqualWithMillisecondsTolerance(timeEnd.GetDifference(timeBegin).AsLocalUnits, mediaData.AudioDuration.AsLocalUnits)
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
                    mediaData.PCMFormat.Data.BytesAreEqualWithMillisecondsTolerance(m_LocalStreamRightMark, timeBytes)
                    ;
            }

            bool leftOk = m_LocalStreamLeftMark == -1
                //|| m_LocalStreamLeftMark == 0
                || mediaData.PCMFormat.Data.BytesAreEqualWithMillisecondsTolerance(0, m_LocalStreamLeftMark)
                ;

            return leftOk && rightOk;
        }

        public ManagedAudioMedia ExtractManagedAudioMedia()
        {

#if ENABLE_SEQ_MEDIA
            Media audioMedia = m_TreeNode.GetManagedAudioMediaOrSequenceMedia();
#else
            ManagedAudioMedia audioMedia = m_TreeNode.GetManagedAudioMedia();
#endif
            if (audioMedia == null)
            {
                Debug.Fail("This should never happen !");
                throw new Exception("TreeNode doesn't have managed audio media ?!");
            }

#if ENABLE_SEQ_MEDIA

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
#else
            else
            {
                AudioMediaData mediaData = audioMedia.AudioMediaData;
#endif //ENABLE_SEQ_MEDIA

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

    public
#if USE_NORMAL_LIST
struct
#else
 sealed class
#endif //USE_NORMAL_LIST
 TreeNodeAndStreamDataLength
    {
        public Object m_Tag1;
        public Object m_Tag2;

        public TreeNode m_TreeNode;
        public long m_LocalStreamDataLength;
    }

    public
#if USE_NORMAL_LIST
struct
#else
 sealed class
#endif //USE_NORMAL_LIST
 StreamWithMarkers
    {
        public Stream m_Stream;
        public Stream m_SecondaryStream;

        public
#if USE_NORMAL_LIST
            List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<TreeNodeAndStreamDataLength> m_SubStreamMarkers;
    }

    public partial class TreeNode
    {
        public bool NeedsAudio()
        {
            if (HasXmlProperty)
            {
                string localName = GetXmlElementLocalName();

                bool isMath = localName.Equals("math", //DiagramContentModelHelper.Math
                    StringComparison.OrdinalIgnoreCase);

                bool isSVG = localName.Equals("svg", //DiagramContentModelHelper.Svg
                    StringComparison.OrdinalIgnoreCase);

                if (!isMath
                    && GetXmlNamespaceUri() == "http://www.w3.org/1998/Math/MathML"
                    //DiagramContentModelHelper.NS_URL_MATHML
                )
                {
                    return false;
                }

                if (!isSVG
                    && GetXmlNamespaceUri() == "http://www.w3.org/2000/svg"
                    //DiagramContentModelHelper.NS_URL_SVG
                    )
                {
                    return false;
                }

                if (localName.Equals("img", StringComparison.OrdinalIgnoreCase)
                     || localName.Equals("video", StringComparison.OrdinalIgnoreCase)
                     || isMath
                    || isSVG
                    )
                {
                    //if (!isMath && !isSVG)
                    //{
                    //    DebugFix.Assert(Children.Count == 0);
                    //}
                    return true;
                }
            }

            if (GetTextMedia() != null
                && !TextOnlyContainsPunctuation(GetTextFlattened_()))
            {
                DebugFix.Assert(Children.Count == 0);
                return true;
            }

            return false;
        }

        public bool HasOrInheritsAudio()
        {
            ManagedAudioMedia media = GetManagedAudioMedia();
            if (media != null && media.IsWavAudioMediaData)
            {
                return true;
            }
                    
#if ENABLE_SEQ_MEDIA

            SequenceMedia seqManagedAudioMedia = GetManagedAudioSequenceMedia();
            if (seqManagedAudioMedia != null)
            {
                return true;
            }
                    
#endif //ENABLE_SEQ_MEDIA

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

#if ENABLE_SEQ_MEDIA
            Media manMedia = Parent.GetManagedAudioMediaOrSequenceMedia();
#else
            Media manMedia = Parent.GetManagedAudioMedia();
#endif
            
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
#if ENABLE_SEQ_MEDIA
            Media manMedia = child.GetManagedAudioMediaOrSequenceMedia();
#else
                Media manMedia = child.GetManagedAudioMedia();
#endif
                
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

        public TreeNode GetLastDescendantWithManagedAudio()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }
            
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

#if ENABLE_SEQ_MEDIA
            Media manMedia = child.GetManagedAudioMediaOrSequenceMedia();
#else
                Media manMedia = child.GetManagedAudioMedia();
#endif

                if (manMedia != null)
                {
                    return child;
                }

                TreeNode childIn = child.GetLastDescendantWithManagedAudio();
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

#if ENABLE_SEQ_MEDIA
            Media manMedia = previous.GetManagedAudioMediaOrSequenceMedia();
#else
                Media manMedia = previous.GetManagedAudioMedia();
#endif
                
                if (manMedia != null)
                {
                    return previous;
                }

                TreeNode previousIn = previous.GetLastDescendantWithManagedAudio();
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

#if ENABLE_SEQ_MEDIA
            Media manMedia = next.GetManagedAudioMediaOrSequenceMedia();
#else
                Media manMedia = next.GetManagedAudioMedia();
#endif
                
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
        
        public
#if USE_NORMAL_LIST
StreamWithMarkers?
#else
 StreamWithMarkers
#endif //USE_NORMAL_LIST
 OpenPcmInputStreamOfManagedAudioMedia()
        {
            return OpenPcmInputStreamOfManagedAudioMedia(false);
        }

        public
#if USE_NORMAL_LIST
StreamWithMarkers?
#else
 StreamWithMarkers
#endif //USE_NORMAL_LIST
 OpenPcmInputStreamOfManagedAudioMedia(bool openSecondaryStream)
        {

            StreamWithMarkers val = new StreamWithMarkers();

//#if USE_NORMAL_LIST
//            StreamWithMarkers val;
//#else
//            StreamWithMarkers val = new StreamWithMarkers();
//#endif //USE_NORMAL_LIST

            ManagedAudioMedia audioMedia = GetManagedAudioMedia();
            if (audioMedia != null && audioMedia.IsWavAudioMediaData)
            {
                val.m_Stream = audioMedia.AudioMediaData.OpenPcmInputStream();
                if (openSecondaryStream)
                {
                    val.m_SecondaryStream = audioMedia.AudioMediaData.OpenPcmInputStream();
                }

                val.m_SubStreamMarkers = new
#if USE_NORMAL_LIST
            List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<TreeNodeAndStreamDataLength>(
#if USE_NORMAL_LIST
1
#endif //USE_NORMAL_LIST
);

                TreeNodeAndStreamDataLength tnasdl = new TreeNodeAndStreamDataLength();
                tnasdl.m_LocalStreamDataLength = val.m_Stream.Length;
                tnasdl.m_TreeNode = this;
                val.m_SubStreamMarkers.Add(tnasdl);
                return val;
            }

                    
#if ENABLE_SEQ_MEDIA
            SequenceMedia seq = GetManagedAudioSequenceMedia();
            if (seq != null)
            {
                Stream stream = seq.OpenPcmInputStreamOfManagedAudioMedia();
                if (stream != null)
                {
                    val.m_Stream = stream;

                    val.m_SubStreamMarkers = new
#if USE_NORMAL_LIST
                    List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<TreeNodeAndStreamDataLength>(
#if USE_NORMAL_LIST
                    1
#endif //USE_NORMAL_LIST
);

                    TreeNodeAndStreamDataLength tnasdl = new TreeNodeAndStreamDataLength();
                    tnasdl.m_LocalStreamDataLength = val.m_Stream.Length;
                    tnasdl.m_TreeNode = this;
                    val.m_SubStreamMarkers.Add(tnasdl);
                    return val;
                }
            }
                    
#endif //ENABLE_SEQ_MEDIA

            return null;
        }

        public
#if USE_NORMAL_LIST
StreamWithMarkers?
#else
 StreamWithMarkers
#endif //USE_NORMAL_LIST
 OpenPcmInputStreamOfManagedAudioMediaFlattened(DelegateAudioPcmStreamFound del)
        {
            return OpenPcmInputStreamOfManagedAudioMediaFlattened(del, false);
        }

        public
#if USE_NORMAL_LIST
StreamWithMarkers?
#else
 StreamWithMarkers
#endif //USE_NORMAL_LIST
 OpenPcmInputStreamOfManagedAudioMediaFlattened(DelegateAudioPcmStreamFound del, bool openSecondaryStream)
        {
#if USE_NORMAL_LIST
StreamWithMarkers?
#else
            StreamWithMarkers
#endif //USE_NORMAL_LIST
 val = OpenPcmInputStreamOfManagedAudioMedia(openSecondaryStream);

            if (val != null)
            {
                if (del != null)
                {
                    del.Invoke(
val.
#if USE_NORMAL_LIST
            GetValueOrDefault().
#endif //USE_NORMAL_LIST
m_Stream.Length
);
                }
                return val;
            }

#if USE_NORMAL_LIST
                    List
#else
            LightLinkedList
#endif //USE_NORMAL_LIST
<StreamWithMarkers> listStreamsWithMarkers = new
#if USE_NORMAL_LIST
                    List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<StreamWithMarkers>();

            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);

#if USE_NORMAL_LIST
StreamWithMarkers?
#else
                StreamWithMarkers
#endif //USE_NORMAL_LIST
 childVal = node.OpenPcmInputStreamOfManagedAudioMediaFlattened(del, openSecondaryStream);

                if (childVal != null)
                {
                    listStreamsWithMarkers.Add(
childVal
#if USE_NORMAL_LIST
            .GetValueOrDefault()
#endif //USE_NORMAL_LIST
);
                }
            }

            if (listStreamsWithMarkers.Count == 0)
            {
                return null;
            }

            StreamWithMarkers returnVal = new StreamWithMarkers();

            returnVal.m_SubStreamMarkers = new
#if USE_NORMAL_LIST
                    List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<TreeNodeAndStreamDataLength>();

#if USE_NORMAL_LIST
                    List
#else
            LightLinkedList
#endif //USE_NORMAL_LIST
<Stream> listStreams = new

#if USE_NORMAL_LIST
                    List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<Stream>();

#if USE_NORMAL_LIST
                    List
#else
            LightLinkedList
#endif //USE_NORMAL_LIST
<Stream> listSecondaryStreams =

openSecondaryStream ? 
new

#if USE_NORMAL_LIST
                    List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<Stream>() : null;


#if USE_NORMAL_LIST
            foreach (StreamWithMarkers strct in listStreamsWithMarkers)
            {
                listStreams.Add(strct.m_Stream);
                if (openSecondaryStream)
                {
                    listSecondaryStreams.Add(strct.m_SecondaryStream);
                }

                returnVal.m_SubStreamMarkers.AddRange(strct.m_SubStreamMarkers);
                strct.m_SubStreamMarkers.Clear();
            }
#else
            LightLinkedList<StreamWithMarkers>.Item current = listStreamsWithMarkers.m_First;
            while (current != null)
            {
                StreamWithMarkers swm = current.m_data;

                listStreams.Add(swm.m_Stream);
                if (openSecondaryStream)
                {
                    listSecondaryStreams.Add(swm.m_SecondaryStream);
                }

                returnVal.m_SubStreamMarkers.AddRange(swm.m_SubStreamMarkers);
                swm.m_SubStreamMarkers.Clear();

                current = current.m_nextItem;
            }
#endif //USE_NORMAL_LIST

            returnVal.m_Stream = new SequenceStream(listStreams);
            if (openSecondaryStream)
            {
                returnVal.m_SecondaryStream = new SequenceStream(listSecondaryStreams);
            }

            listStreamsWithMarkers.Clear();
            listStreamsWithMarkers = null;

            return returnVal;
        }
         
#if ENABLE_SEQ_MEDIA

        public Media GetManagedAudioMediaOrSequenceMedia()
        {
            ManagedAudioMedia managedAudioMedia = GetManagedAudioMedia();
                   
            if (managedAudioMedia == null)
            {
                return GetManagedAudioSequenceMedia();
            }

            return managedAudioMedia;
        }
                    
#endif //ENABLE_SEQ_MEDIA

        public ManagedAudioMedia GetManagedAudioMedia()
        {
            AbstractAudioMedia media = GetAudioMedia();
            if (media != null
                && media is ManagedAudioMedia
                && ((ManagedAudioMedia)media).IsWavAudioMediaData)
            {
                DebugFix.Assert(((ManagedAudioMedia)media).HasActualAudioMediaData);

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

#if ENABLE_SEQ_MEDIA

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
        
#endif //ENABLE_SEQ_MEDIA

        public Media GetMediaInAudioChannel()
        {
            return GetMediaInChannel<AudioChannel>();
        }

        public Time GetDurationOfManagedAudioMediaFlattened()
        {
            ManagedAudioMedia audioMedia = GetManagedAudioMedia();
            if (audioMedia != null)
            {
                Time dur_ = audioMedia.Duration;
                if (dur_.AsLocalUnits <= 0)
                {
                    return null;
                }
                return dur_;
            }

#if ENABLE_SEQ_MEDIA

            SequenceMedia seq = GetManagedAudioSequenceMedia();
            if (seq != null)
            {
                Time durSeq = seq.GetDurationOfManagedAudioMedia();
                if (durSeq != null)
                {
                    return durSeq;
                }
            }
            
#endif //ENABLE_SEQ_MEDIA

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
