using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

using urakawa.property.alt;

namespace urakawa.commands
{
    public class AlternateContentSetManagedMediaCommand : Command
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
            return XukStrings.TreeNodeSetManagedAudioMediaCommand;
        }

        private AlternateContent m_AlternateContent;
        public AlternateContent AlternateContent
        {
            private set { m_AlternateContent = value; }
            get { return m_AlternateContent; }
        }
        
        private media.Media m_ManagedMedia;
        public media.Media ManagedMedia 
        {
            private set { m_ManagedMedia  = value; }
            get { return m_ManagedMedia; }
        }

        public void Init(AlternateContent altContent, media.Media managedMedia)
        {
            if (altContent == null)
            {
                throw new ArgumentNullException("altContent");
            }
            
            if (managedMedia == null)
            {
                throw new ArgumentNullException("managedMedia");
            }

            m_AlternateContent = altContent;
            ManagedMedia= managedMedia;

            if (managedMedia is ManagedAudioMedia)
            {
                m_UsedMediaData.Add( ((ManagedAudioMedia)managedMedia).AudioMediaData);
            }
            else if (managedMedia is ManagedImageMedia)
            {
                m_UsedMediaData.Add(((ManagedImageMedia)managedMedia).ImageMediaData);
            }
            ShortDescription = "Add new ManagedMedia";
            LongDescription = "Attach a ManagedMedia to a AlternateContent";
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
            if (m_AlternateContent.AlternateMedias.ContentsAs_ListAsReadOnly.Contains(ManagedMedia))
            {
                throw new exception.CannotExecuteException("ManagedMedia already exists inAlternateContent");
            }
            m_AlternateContent.AlternateMedias.Insert(m_AlternateContent.AlternateMedias.Count, ManagedMedia);
        }

        public override void UnExecute()
        {
            m_AlternateContent.AlternateMedias.Remove(ManagedMedia);
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

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
