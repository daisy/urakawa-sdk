using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.commands
{
    public class ManagedAudioMediaInsertDataCommand : Command
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.ManagedAudioMediaInsertDataCommand;
        }

        private ManagedAudioMedia m_ManagedAudioMediaTarget;
        private ManagedAudioMedia m_ManagedAudioMediaSource;
        private Time m_TimeInsert;

        public void Init(ManagedAudioMedia managedAudioMediaTarget, ManagedAudioMedia managedAudioMediaSource, Time insertTime)
        {
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

            m_TimeInsert = insertTime;

            m_ManagedAudioMediaSource = managedAudioMediaSource;
            m_ManagedAudioMediaTarget = managedAudioMediaTarget;

            m_ListOfUsedMediaData.Add(managedAudioMediaSource.AudioMediaData);
            m_ListOfUsedMediaData.Add(managedAudioMediaTarget.AudioMediaData);

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
            TimeDelta duration = m_ManagedAudioMediaSource.Duration;
            Stream stream = m_ManagedAudioMediaSource.AudioMediaData.GetAudioData();
            try
            {
                m_ManagedAudioMediaTarget.AudioMediaData.InsertAudioData(stream, m_TimeInsert, duration);
            }
            finally
            {
                stream.Close();
            }
        }

        public override void UnExecute()
        {
            TimeDelta duration = m_ManagedAudioMediaSource.Duration;
            m_ManagedAudioMediaTarget.AudioMediaData.RemoveAudioData(m_TimeInsert, m_TimeInsert.AddTimeDelta(duration));
        }

        private List<MediaData> m_ListOfUsedMediaData = new List<MediaData>();
        public override List<MediaData> ListOfUsedMediaData
        {
            get
            {
                return m_ListOfUsedMediaData;
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
