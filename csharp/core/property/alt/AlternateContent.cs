using System;
using System.Xml;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.xuk;
using urakawa.progress;
using urakawa.media;

namespace urakawa.property.alt
{
    public class AlternateContent : WithPresentation
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContent;
        }

        //public AlternateContent()
        //{
        //    m_Role = "urakawa:defaultRole";

        //    m_Text = Presentation.MediaFactory.CreateTextMedia();
        //    m_Text.Text = "default description";
        //}

        //public override void XukIn(XmlReader source, IProgressHandler handler)
        //{
        //    m_Role = null;
        //    m_Text = null;
        //    base.XukIn(source, handler);
        //}

        private string m_Role;
        public string Role
        {
            get { return m_Role; }
            set { m_Role = value; }
        }

        private Description m_ShortDescription;
        public Description ShortDescription
        {
            get { return m_ShortDescription; }
            set { m_ShortDescription = value; }
            //m_ShortDescription = new Description();
            //m_ShortDescription.Name = XukStrings.ShortDescription;
        }

        private Description m_LongDescription;
        public Description LongDescription
        {
            get { return m_LongDescription; }
            set { m_LongDescription = value; }
            //m_LongDescription = new Description();
            //m_LongDescription.Name = XukStrings.LongDescription;
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


        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string role = source.GetAttribute(XukStrings.AlternateContentRole);
            if (!string.IsNullOrEmpty(role))
            {
                m_Role = role;
            }
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (!string.IsNullOrEmpty(m_Role))
            {
                destination.WriteAttributeString(XukStrings.AlternateContentRole, m_Role);
            }
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.TextMedia)
                {
                    if (m_Text != null)
                    {
                        throw new exception.XukException("AlternateContent Text XukIn, already set !");
                    }
                    m_Text = Presentation.MediaFactory.CreateTextMedia();
                    m_Text.XukIn(source, handler);
                }
                else if (source.LocalName == XukStrings.ManagedAudioMedia)
                {
                    if (m_Audio != null)
                    {
                        throw new exception.XukException("AlternateContent Audio XukIn, already set !");
                    }
                    m_Audio = Presentation.MediaFactory.CreateManagedAudioMedia();
                    m_Audio.XukIn(source, handler);
                }
                else if (source.LocalName == XukStrings.Description)
                {
                    Description desc = new Description();
                    desc.XukIn(source, handler);
                    if (desc.Name == XukStrings.ShortDescription)
                    {
                        if (m_ShortDescription != null)
                        {
                            throw new exception.XukException("AlternateContent short description XukIn, already set !");
                        }
                        m_ShortDescription = desc;
                    }
                    else if (desc.Name == XukStrings.LongDescription)
                    {
                        if (m_LongDescription != null)
                        {
                            throw new exception.XukException("AlternateContent long description XukIn, already set !");
                        }
                        m_LongDescription = desc;
                    }
                }
                else if (source.LocalName == XukStrings.ManagedImageMedia)
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

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            if (m_Text != null)
            {
                m_Text.XukOut(destination, baseUri, handler);
            }

            if (m_Audio != null)
            {
                m_Audio.XukOut(destination, baseUri, handler);
            }

            if (m_ShortDescription != null)
            {
                m_ShortDescription.XukOut(destination, baseUri, handler);
            }
            if (m_LongDescription != null)
            {
                m_LongDescription.XukOut(destination, baseUri, handler);
            }

            if (m_Image != null)
            {
                m_Image.XukOut(destination, baseUri, handler);
            }
        }
    }
}
