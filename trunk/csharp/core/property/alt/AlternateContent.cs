using System;
using System.Xml;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.metadata;
using urakawa.xuk;
using urakawa.progress;
using urakawa.media;

namespace urakawa.property.alt
{
    [XukNameUglyPrettyAttribute("AC", "AlternateContent")]
    public class AlternateContent : WithPresentation
    {
        public AlternateContent()
        {
            m_Metadata = new ObjectListProvider<Metadata>(this, true);
        }


        public bool IsEmpty
        {
            get
            {
                return m_Metadata.Count == 0 && Text == null && Image == null && Audio == null;
            }
        }

        //public override void XukIn(XmlReader source, IProgressHandler handler)
        //{
        //    m_Role = null;
        //    m_Text = null;
        //    base.XukIn(source, handler);
        //}

        private ObjectListProvider<Metadata> m_Metadata;
        public ObjectListProvider<Metadata> Metadatas
        {
            get
            {
                return m_Metadata;
            }
        }

        private ManagedImageMedia m_Image;
        public ManagedImageMedia Image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }

        private TextMedia m_Text;
        public TextMedia Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        private ManagedAudioMedia m_Audio;
        public ManagedAudioMedia Audio
        {
            get { return m_Audio; }
            set { m_Audio = value; }
        }
        
#if PUBLISH_ALT_CONTENT
        private ExternalAudioMedia m_ExternalAudio;
        public ExternalAudioMedia ExternalAudio
        {
            get { return m_ExternalAudio; }
            set { m_ExternalAudio = value; }
        }
#endif //PUBLISH_ALT_CONTENT

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);
        }

        private void XukInMetadata(XmlReader source, IProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    if (source.NamespaceURI == XukAble.XUK_NS
                        && XukAble.GetXukName(typeof(Metadata)).Match(source.LocalName))
                    {
                        Metadata md = Presentation.MetadataFactory.CreateMetadata();
                        md.XukIn(source, handler);
                        m_Metadata.Insert(m_Metadata.Count, md);
                    }
                }
                else if (source.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (source.EOF)
                {
                    throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.Metadatas)
                {
                    XukInMetadata(source, handler);
                }
                else if (XukAble.GetXukName(typeof(TextMedia)).Match(source.LocalName))
                {
                    if (m_Text != null)
                    {
                        throw new exception.XukException("AlternateContent Text XukIn, already set !");
                    }
                    m_Text = Presentation.MediaFactory.CreateTextMedia();
                    m_Text.XukIn(source, handler);
                }
                else if (XukAble.GetXukName(typeof(ManagedAudioMedia)).Match(source.LocalName))
                {
                    if (m_Audio != null)
                    {
                        throw new exception.XukException("AlternateContent Audio XukIn, already set !");
                    }
                    m_Audio = Presentation.MediaFactory.CreateManagedAudioMedia();
                    m_Audio.XukIn(source, handler);
                }
                else if (XukAble.GetXukName(typeof(ManagedImageMedia)).Match(source.LocalName))
                {
                    if (m_Image != null)
                    {
                        throw new exception.XukException("AlternateContent Image XukIn, already set !");
                    }
                    m_Image = Presentation.MediaFactory.CreateManagedImageMedia();
                    m_Image.XukIn(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close(); //Read past unknown child 
            }
        }

        public AlternateContent Copy()
        {
            AlternateContent ac = Presentation.AlternateContentFactory.Create(GetType());

            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                ac.m_Metadata.Insert(ac.m_Metadata.Count, md.Copy());
            }

            if (m_Text != null)
            {
                ac.m_Text = m_Text.Copy();
            }
            if (m_Audio != null)
            {
                ac.m_Audio = m_Audio.Copy();
            }
            if (m_Image != null)
            {
                ac.m_Image = m_Image.Copy();
            }

            return ac;
        }
        public AlternateContent Export(Presentation destPres)
        {
            AlternateContent ac = destPres.AlternateContentFactory.Create(GetType());

            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                ac.m_Metadata.Insert(ac.m_Metadata.Count, md.Export(destPres));
            }

            if (m_Text != null)
            {
                ac.m_Text = m_Text.Export(destPres);
            }
            if (m_Audio != null)
            {
                ac.m_Audio = m_Audio.Export(destPres);
            }
            if (m_Image != null)
            {
                ac.m_Image = m_Image.Export(destPres);
            }

            return ac;
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.Metadatas, XukAble.XUK_NS);
            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                md.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

            if (m_Text != null)
            {
                m_Text.XukOut(destination, baseUri, handler);
            }

            if (m_Audio != null)
            {
                m_Audio.XukOut(destination, baseUri, handler);
            }

            if (m_Image != null)
            {
                m_Image.XukOut(destination, baseUri, handler);
            }
        }
    }
}
