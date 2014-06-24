using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.progress;
using urakawa.xuk;

using urakawa.property.alt;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("acSetManMedCmd", "AlternateContentSetManagedMediaCommand")]
    public class AlternateContentSetManagedMediaCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentSetManagedMediaCommand otherz = other as AlternateContentSetManagedMediaCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        
        
        private AlternateContent m_AlternateContent;
        public AlternateContent AlternateContent
        {
            private set { m_AlternateContent = value; }
            get { return m_AlternateContent; }
        }

        private Media m_Media;
        public Media Media
        {
            private set { m_Media = value; }
            get { return m_Media; }
        }
        private Media m_OldMedia;
        public Media OldMedia
        {
            private set { m_OldMedia = value; }
            get { return m_OldMedia; }
        }
        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }
        public void Init(TreeNode treeNode, AlternateContent altContent, Media media)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }
            if (altContent == null)
            {
                throw new ArgumentNullException("altContent");
            }

            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            TreeNode = treeNode;
            AlternateContent = altContent;
            Media = media;

            if (media is ManagedAudioMedia)
            {
                OldMedia = m_AlternateContent.Audio;
                m_UsedMediaData.Add(((ManagedAudioMedia)media).AudioMediaData);
                if (OldMedia != null)
                    m_UsedMediaData.Add(((ManagedAudioMedia)OldMedia).AudioMediaData);
            }
            else if (media is ManagedImageMedia)
            {
                OldMedia = m_AlternateContent.Image;
                m_UsedMediaData.Add(((ManagedImageMedia)media).ImageMediaData);
                if (OldMedia != null)
                    m_UsedMediaData.Add(((ManagedImageMedia)OldMedia).ImageMediaData);
            }
            else if (media is TextMedia)
            {
                OldMedia = m_AlternateContent.Text;
            }
            else
            {
                throw new ArgumentException("media should be ManagedAudioMedia, ManagedImageMedia, or TextMedia");
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
            if (Media is ManagedAudioMedia)
            {
                m_AlternateContent.Audio = (ManagedAudioMedia)Media;
            }
            else if (Media is ManagedImageMedia)
            {
                m_AlternateContent.Image = (ManagedImageMedia)Media;
            }
            else if (Media is TextMedia)
            {
                m_AlternateContent.Text = (TextMedia)Media;
            }
        }

        public override void UnExecute()
        {
            if (Media is ManagedAudioMedia)
            {
                m_AlternateContent.Audio = (ManagedAudioMedia)OldMedia;
            }
            else if (Media is ManagedImageMedia)
            {
                m_AlternateContent.Image = (ManagedImageMedia)OldMedia;
            }
            else if (Media is TextMedia)
            {
                m_AlternateContent.Text = (TextMedia)OldMedia;
            }
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
