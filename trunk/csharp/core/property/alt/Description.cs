using System;
using System.Collections.Generic;
using System.Xml;

using urakawa.xuk;
using urakawa.progress;
using urakawa.media;
using urakawa.media.data.audio;

namespace urakawa.property.alt
{
    public class Description : WithPresentation
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.Description;
        }

        private IDictionary<string, DescriptionAttribute> mAttributes = new Dictionary<string, DescriptionAttribute>();


        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
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

        public List<DescriptionAttribute> Attributes
        {
            get { return new List<DescriptionAttribute>(mAttributes.Values); }
        }

        public bool SetAttribute(DescriptionAttribute newAttribute)
        {
            if (newAttribute == null)
            {
                throw new exception.MethodParameterIsNullException("Can not set a null xml attribute");
            }
            string key = String.Format("{1}:{0}", newAttribute.LocalName, newAttribute.NamespaceUri);
            string prevValue = null;

            DescriptionAttribute obj;
            mAttributes.TryGetValue(key, out obj);

            if (obj != null) //mAttributes.ContainsKey(key))
            {
                DescriptionAttribute prevAttr = obj; // mAttributes[key];
                prevValue = prevAttr.Value;
                RemoveAttribute(prevAttr);
            }
            mAttributes.Add(key, newAttribute);
            newAttribute.Parent = this;

            return (prevValue != null);
        }



        public DescriptionAttribute RemoveAttribute(string localName, string namespaceUri)
        {
            DescriptionAttribute attrToRemove = GetAttribute(localName, namespaceUri);
            if (attrToRemove == null)
            {
                throw new System.Exception(String.Format(
                                                                           "The description not have an attribute with QName {1}:{0}",
                                                                           localName, namespaceUri));
            }
            RemoveAttribute(attrToRemove);
            return attrToRemove;
        }

        public void RemoveAttribute(DescriptionAttribute attrToRemove)
        {
            string key = String.Format("{1}:{0}", attrToRemove.LocalName, attrToRemove.NamespaceUri);
            if (!Object.ReferenceEquals(mAttributes[key], attrToRemove))
            {
                throw new System.Exception(
                    "The given DescriptionAttribute does not belong to the XmlProperty");
            }

            mAttributes.Remove(key);
            attrToRemove.Parent = null;

        }

        public bool SetAttribute(string localName, string namespaceUri, string value)
        {
            DescriptionAttribute attr = GetAttribute(localName, namespaceUri);
            if (attr == null)
            {
                attr = new DescriptionAttribute();
                attr.SetQName(localName, namespaceUri);
                attr.Value = value;
                return SetAttribute(attr);
            }
            else
            {
                attr.Value = value;
                return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="DescriptionAttribute"/> with a given QName
        /// </summary>
        /// <param name="localName">The local localName part of the given QName</param>
        /// <param name="namespaceUri">The namespce uri part of the given QName</param>
        /// <returns>The <see cref="DescriptionAttribute"/> if found, otherwise <c>null</c></returns>
        public DescriptionAttribute GetAttribute(string localName, string namespaceUri)
        {
            string key = String.Format("{1}:{0}", localName, namespaceUri);

            DescriptionAttribute obj;
            mAttributes.TryGetValue(key, out obj);
            return obj;

        }

        public DescriptionAttribute GetAttribute(string localName)
        {
            return GetAttribute(localName, "");
        }


        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string name = source.GetAttribute(XukStrings.DescriptionName);
            if (!string.IsNullOrEmpty(name))
            {
                m_Name = name;
            }
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (!string.IsNullOrEmpty(m_Name))
            {
                destination.WriteAttributeString(XukStrings.DescriptionName, m_Name);
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
                        throw new exception.XukException("Description Text XukIn, already set !");
                    }
                    m_Text = Presentation.MediaFactory.CreateTextMedia();
                    m_Text.XukIn(source, handler);
                }
                else if (source.LocalName == XukStrings.ManagedAudioMedia)
                {
                    if (m_Audio != null)
                    {
                        throw new exception.XukException("Description Audio XukIn, already set !");
                    }
                    m_Audio = Presentation.MediaFactory.CreateManagedAudioMedia();
                    m_Audio.XukIn(source, handler);
                }
                else if (IsPrettyFormat() && source.LocalName == XukStrings.DescriptionAttributes)
                {
                    XukInDescriptionAttributes(source, handler);
                }
                else if (!IsPrettyFormat() && source.LocalName == XukStrings.DescriptionAttribute)
                {
                    XukInDescriptionAttribute(source, handler);
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

        protected virtual void XukInDescriptionAttribute(XmlReader source, IProgressHandler handler)
        {
            if (source.LocalName == XukStrings.DescriptionAttribute && source.NamespaceURI == XukAble.XUK_NS)
            {
                DescriptionAttribute attr = new DescriptionAttribute();
                attr.XukIn(source, handler);
                SetAttribute(attr);
            }
            else if (!source.IsEmptyElement)
            {
                source.ReadSubtree().Close();
            }
        }

        private void XukInDescriptionAttributes(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        XukInDescriptionAttribute(source, handler);
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
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

            List<DescriptionAttribute> attrs = Attributes;
            if (attrs.Count > 0)
            {
                if (IsPrettyFormat())
                {
                    destination.WriteStartElement(XukStrings.DescriptionAttributes, XukAble.XUK_NS);
                }
                foreach (DescriptionAttribute a in attrs)
                {
                    a.XukOut(destination, baseUri, handler);
                }
                if (IsPrettyFormat())
                {
                    destination.WriteEndElement();
                }
            }
        }
    }
}
