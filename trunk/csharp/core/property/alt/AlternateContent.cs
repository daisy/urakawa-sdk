using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using urakawa.xuk;
using urakawa.media;
using urakawa.media.data;

namespace urakawa.property.alt
{
    public class AlternateContent:xuk.XukAble
    {

        public AlternateContent()
        {
            m_Medias = new ObjectListProvider<urakawa.media.Media>(this, true);
            m_Description = null;
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
            if (media == null) throw new System.Exception("Null media cannot be inserted");

            if (media is media.data.audio.ManagedAudioMedia
                || media is media.data.image.ManagedImageMedia
                || media is media.TextMedia)
            {
                m_Medias.Insert(m_Medias.Count, media);
            }
            else
            {
                throw new System.Exception("Unacceptable media type: " + media.GetType().ToString());
            }
        }

        public void RemoveMedia(Media media)
        {
            if (media == null) throw new System.Exception("Cannot remove null media");


            if (!m_Medias.ContentsAs_ReadOnlyCollectionWrapper.Contains(media)) throw new System.Exception("Media to remove is not in the alternate content");

            m_Medias.Remove(media);
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


    }
}
