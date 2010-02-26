using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.commands
{
    public class ManagedAudioMediaInsertDataCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ManagedAudioMediaInsertDataCommand otherz = other as ManagedAudioMediaInsertDataCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.ManagedAudioMediaInsertDataCommand;
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }
        private TreeNode m_CurrentTreeNode;
        public TreeNode CurrentTreeNode
        {
            private set { m_CurrentTreeNode = value; }
            get { return m_CurrentTreeNode; }
        }
        private ManagedAudioMedia m_ManagedAudioMediaTarget;
        public ManagedAudioMedia ManagedAudioMediaTarget
        {
            private set { m_ManagedAudioMediaTarget = value; }
            get { return m_ManagedAudioMediaTarget; }
        }
        private ManagedAudioMedia m_ManagedAudioMediaSource;
        public ManagedAudioMedia ManagedAudioMediaSource
        {
            private set { m_ManagedAudioMediaSource = value; }
            get { return m_ManagedAudioMediaSource; }
        }
        private Time m_TimeInsert;
        public Time TimeInsert
        {
            private set { m_TimeInsert = value; }
            get { return m_TimeInsert; }
        }

        public void Init(TreeNode treeNode, ManagedAudioMedia managedAudioMediaTarget, ManagedAudioMedia managedAudioMediaSource, Time insertTime, TreeNode currentTreeNode)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }
            if (currentTreeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }
            
            if (insertTime == null)
            {
                throw new ArgumentNullException("insertTime");
            }
            if (managedAudioMediaTarget == null)
            {
                throw new ArgumentNullException("managedAudioMediaTarget");
            }
            if (managedAudioMediaSource == null)
            {
                throw new ArgumentNullException("managedAudioMediaSource");
            }
            if (managedAudioMediaTarget.Presentation != managedAudioMediaSource.Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }
            if (managedAudioMediaTarget.Presentation != Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }

            if (!managedAudioMediaSource.HasActualAudioMediaData || !managedAudioMediaTarget.HasActualAudioMediaData)
            {
                throw new ArgumentException("HasActualAudioMediaData");
            }

            TreeNode = treeNode;
            CurrentTreeNode = currentTreeNode;
            TimeInsert = insertTime;

            ManagedAudioMediaSource = managedAudioMediaSource;
            ManagedAudioMediaTarget = managedAudioMediaTarget;

            m_UsedMediaData.Add(ManagedAudioMediaSource.AudioMediaData);
            //m_UsedMediaData.Add(ManagedAudioMediaTarget.AudioMediaData); belongs to TreeNode, so no need to preserve it explicitely

            ShortDescription = "Insert new audio";
            LongDescription = "Insert WaveAudioMediaData from a source ManagedAudioMedia into a target ManagedAudioMedia";
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }

        public override void Execute()
        {
            TimeDelta duration = ManagedAudioMediaSource.Duration;
            Stream stream = ManagedAudioMediaSource.AudioMediaData.OpenPcmInputStream();
            try
            {
                ManagedAudioMediaTarget.AudioMediaData.InsertPcmData(stream, TimeInsert, duration);
            }
            finally
            {
                stream.Close();
            }
        }

        public override void UnExecute()
        {
            TimeDelta duration = ManagedAudioMediaSource.Duration;
            ManagedAudioMediaTarget.AudioMediaData.RemovePcmData(TimeInsert, TimeInsert.AddTimeDelta(duration));
        }

        private List<MediaData> m_UsedMediaData = new List<MediaData>();
        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                return m_UsedMediaData;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
