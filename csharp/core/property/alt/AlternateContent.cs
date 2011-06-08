using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using urakawa.xuk;
using urakawa.progress;
using urakawa.media;
using urakawa.media.data;
using urakawa.metadata;
using urakawa.exception;

namespace urakawa.property.alt
{
    public class AlternateContent:WithPresentation
    {

        public AlternateContent()
        {
            m_Medias = new ObjectListProvider<urakawa.media.Media>(this, true);
            m_Description = null;
            m_Metadata = new ObjectListProvider<Metadata>(this, true);
        }

        private ObjectListProvider <media.Media> m_Medias;
        public ObjectListProvider<media.Media> AlternateMedias
        {
            get {return m_Medias; }
        }



        private string m_Description;
        public string Description
        {
        get { return m_Description ; }
            set { m_Description = value; }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContent;
        }

        public void AddMedia(media.Media media)
        {
            if (media == null) throw new exception.MethodParameterIsNullException ("Null media cannot be inserted");

            if (media is media.data.audio.ManagedAudioMedia
                || media is media.data.image.ManagedImageMedia
                || media is media.TextMedia)
            {
                m_Medias.Insert(m_Medias.Count, media);
            }
            else
            {
                throw new exception.MethodParameterIsWrongTypeException("Unacceptable media type: " + media.GetType().ToString());
            }
        }

        public void RemoveMedia(Media media)
        {
            if (media == null) throw new exception.MethodParameterIsNullException("Cannot remove null media");


            if (!m_Medias.ContentsAs_ReadOnlyCollectionWrapper.Contains(media)) throw new exception.CannotExecuteException ("Media to remove is not in the alternate content");

            m_Medias.Remove(media);
        }

        private ObjectListProvider<Metadata> m_Metadata;
        public ObjectListProvider <Metadata> Metadatas
        {
            get { return m_Metadata; }
        }


        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            m_Description = source.GetAttribute(XukStrings.AlternateContentDescription);
                        
        }
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

                destination.WriteAttributeString(XukStrings.AlternateContentDescription, m_Description);
            
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            if (source.LocalName == XukStrings.Metadatas)
            {
                XukInMetadata(source, handler);
            }
            else
            {
                base.XukInChild(source, handler);
            }
        }

        private void XukInMetadata(XmlReader source, IProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    Metadata newMeta = Presentation.MetadataFactory.Create(source.LocalName, source.NamespaceURI);
                    if (newMeta != null)
                    {
                        newMeta.XukIn(source, handler);
                        m_Metadata.Insert(m_Metadata.Count, newMeta);
                    }
                    else if (!source.IsEmptyElement)
                    {
                        //Read past unidentified element
                        source.ReadSubtree().Close();
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

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.Metadatas, XukAble.XUK_NS);
            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                md.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

        }


    }
}
