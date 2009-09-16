using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.core.visitor;
using urakawa.events;
using urakawa.exception;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.utilities;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = urakawa.property.xml.XmlAttribute;

namespace urakawa.core
{
    public struct TreeNodeAndStreamSelection
    {
        public TreeNode m_TreeNode;
        public long m_LocalStreamLeftMark;
        public long m_LocalStreamRightMark;
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

            foreach (TreeNode child in Children.ContentsAs_YieldEnumerable)
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

        public StreamWithMarkers? OpenPcmInputStreamOfManagedAudioMediaFlattened()
        {
            StreamWithMarkers? val = OpenPcmInputStreamOfManagedAudioMedia();
            if (val != null)
            {
                return val;
            }

            List<StreamWithMarkers> listStreamsWithMarkers = new List<StreamWithMarkers>();

            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                StreamWithMarkers? childVal = node.OpenPcmInputStreamOfManagedAudioMediaFlattened();
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
                foreach (Media media in seqAudioMedia.ChildMedias.ContentsAs_YieldEnumerable)
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

        public TimeDelta GetDurationOfManagedAudioMediaFlattened()
        {
            ManagedAudioMedia audioMedia = GetManagedAudioMedia();
            if (audioMedia != null && audioMedia.HasActualAudioMediaData)
            {
                return audioMedia.Duration;
            }

            SequenceMedia seq = GetManagedAudioSequenceMedia();
            if (seq != null)
            {
                TimeDelta durSeq = seq.GetDurationOfManagedAudioMedia();
                if (durSeq != null)
                {
                    return durSeq;
                }
            }

            TimeDelta dur = new TimeDelta();
            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                TimeDelta childDur = node.GetDurationOfManagedAudioMediaFlattened();
                if (childDur != null)
                {
                    dur.AddTimeDelta(childDur);
                }
            }
            if (dur.TimeDeltaAsMillisecondDouble <= 0)
            {
                return null;
            }
            return dur;
        }
    }
}
