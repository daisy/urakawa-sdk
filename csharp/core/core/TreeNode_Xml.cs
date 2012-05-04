using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using AudioLib;
using urakawa.media;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = urakawa.property.xml.XmlAttribute;

namespace urakawa.core
{
    [DebuggerDisplay("{getDebugString()}")]
    public partial class TreeNode
    {
        public override string ToString()
        {
            if (HasXmlProperty)
            {
                return GetXmlElementLocalName();
            }
            return base.ToString();
        }

        protected string getDebugString()
        {
            string localName = GetXmlElementLocalName();
            String str = (localName != null ? localName : "NO XML");
            str += " /// ";
            str += GetTextFlattened();
            return str;
        }


        public TreeNode GetFirstAncestorWithXmlElement(string localName)
        {
            if (Parent == null)
            {
                return null;
            }

            if (Parent.HasXmlProperty && Parent.GetXmlElementLocalName() == localName)
            {
                return Parent;
            }

            return Parent.GetFirstAncestorWithXmlElement(localName);
        }

        public TreeNode GetFirstDescendantWithXmlElement(string localName)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                if (child.HasXmlProperty && child.GetXmlElementLocalName() == localName)
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithXmlElement(localName);
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetPreviousSiblingWithXmlElement(string localName)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode previous = this;
            while ((previous = previous.PreviousSibling) != null)
            {
                if (previous.HasXmlProperty && previous.GetXmlElementLocalName() == localName)
                {
                    return previous;
                }

                TreeNode previousIn = previous.GetFirstDescendantWithXmlElement(localName);
                if (previousIn != null)
                {
                    return previousIn;
                }
            }

            return Parent.GetPreviousSiblingWithXmlElement(localName);
        }

        public TreeNode GetNextSiblingWithXmlElement(string localName)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                if (next.HasXmlProperty && next.GetXmlElementLocalName() == localName)
                {
                    return next;
                }

                TreeNode nextIn = next.GetFirstDescendantWithXmlElement(localName);
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            return Parent.GetNextSiblingWithXmlElement(localName);
        }


        public bool HasXmlProperty
        {
            get { return GetProperty<XmlProperty>() != null; }
        }
        public XmlProperty GetOrCreateXmlProperty()
        {
            return GetOrCreateProperty<XmlProperty>();
        }
        public XmlProperty GetXmlProperty()
        {
            return GetProperty<XmlProperty>();
        }

        public TreeNode GetTreeNodeWithXmlElementId(string id)
        {
            if (GetXmlElementId() == id) return this;

            for (int i = 0; i < Children.Count; i++)
            {
                TreeNode child = Children.Get(i).GetTreeNodeWithXmlElementId(id);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        public string GetXmlElementLocalName()
        {
            QualifiedName qName = GetXmlElementQName();
            if (qName == null) return null;

            return qName.LocalName;
        }

        private QualifiedName m_QualifiedName = null;
        private QualifiedName GetXmlElementQName()
        {
            XmlProperty xmlProp = GetProperty<XmlProperty>();
            if (xmlProp != null)
            {
                //TODO QualifiedName fields are unmutable,
                // so unless the underlying XmlProperty fields change, caching is okay
                // (here we assume that once a TreeNode as been XukedIn,
                // its XML definition does not change)
                if (m_QualifiedName == null || xmlProp.QNameIsInvalidated)
                {
                    string nsUri = GetXmlNamespaceUri();

                    m_QualifiedName = new QualifiedName(xmlProp.LocalName, nsUri);

                    xmlProp.QNameIsInvalidated = false;
                }
            }
            else
            {
                m_QualifiedName = null;
            }

            return m_QualifiedName;
        }
        ///<summary>
        /// returns the ID attribute value of the attached XmlProperty, if any
        ///</summary>
        ///<returns>null of there is no ID attribute</returns>
        public string GetXmlElementId()
        {
            XmlProperty xmlProp = GetProperty<XmlProperty>();
            if (xmlProp != null)
            {
                XmlAttribute idAttr = xmlProp.GetAttribute("id");
                if (idAttr == null)
                {
                    idAttr = xmlProp.GetAttribute(XmlReaderWriterHelper.XmlId, XmlReaderWriterHelper.NS_URL_XML);
                }
                if (idAttr != null)
                {
                    return (string.IsNullOrEmpty(idAttr.Value) ? null : idAttr.Value);
                }
            }
            return null;
        }

        public string GetXmlElementLang()
        {
            XmlProperty xmlProp = GetProperty<XmlProperty>();
            if (xmlProp != null)
            {
                XmlAttribute langAttr = xmlProp.GetAttribute("lang");
                if (langAttr == null)
                {
                    langAttr = xmlProp.GetAttribute(XmlReaderWriterHelper.XmlLang, XmlReaderWriterHelper.NS_URL_XML);
                }
                if (langAttr != null)
                {
                    return (string.IsNullOrEmpty(langAttr.Value) ? null : langAttr.Value);
                }
            }
            return null;
        }

        public TreeNode GetFirstChildWithXmlElementName(string elemName)
        {
            if (HasXmlProperty && GetXmlElementLocalName() == elemName) return this;

            for (int i = 0; i < mChildren.Count; i++)
            {
                TreeNode child = mChildren.Get(i).GetFirstChildWithXmlElementName(elemName);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        public string GetXmlNamespaceUri(string prefix)
        {
            if (!HasXmlProperty)
            {
                return null;
            }

            XmlProperty xmlProp = GetProperty<XmlProperty>();
            return xmlProp.GetNamespaceUri(prefix);
        }

        public string GetXmlNamespaceUri()
        {
            return GetXmlNamespaceUri(null);
        }

        protected void GetXmlFragment_(XmlTextWriter xmlWriter)
        {
            if (HasXmlProperty)
            {
                string name = GetXmlElementLocalName();
                string NSPrefix;
                string localName;
                XmlProperty.SplitLocalName(name, out NSPrefix, out localName);

                string nsUri = GetXmlNamespaceUri(NSPrefix);

                XmlProperty xmlProp = GetProperty<XmlProperty>();

                if (string.IsNullOrEmpty(NSPrefix))
                {
                    if (!string.IsNullOrEmpty(nsUri))
                    {
                        xmlWriter.WriteStartElement(name, nsUri);
                    }
                    else
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                        xmlWriter.WriteStartElement(name);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(nsUri))
                    {
                        xmlWriter.WriteStartElement(NSPrefix, localName, nsUri);
                    }
                    else
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                        xmlWriter.WriteStartElement(name);
                    }
                }

                foreach (XmlAttribute xmlAttr in xmlProp.Attributes.ContentsAs_Enumerable)
                {
                    string nsUriAttr = xmlAttr.GetNamespaceUri();

                    string attrName = xmlAttr.LocalName;
                    string attrNSPrefix;
                    string attrLocalName;
                    XmlProperty.SplitLocalName(attrName, out attrNSPrefix, out attrLocalName);

                    if (string.IsNullOrEmpty(attrNSPrefix))
                    {
                        if (attrName.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                        {
                            DebugFix.Assert(XmlReaderWriterHelper.NS_URL_XMLNS.Equals(nsUriAttr));

                            xmlWriter.WriteAttributeString(XmlReaderWriterHelper.NS_PREFIX_XMLNS, XmlReaderWriterHelper.NS_URL_XMLNS, xmlAttr.Value);
                        }
                        else if (!string.IsNullOrEmpty(nsUriAttr) && nsUriAttr != nsUri)
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrName, nsUriAttr, xmlAttr.Value);
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString(attrName, xmlAttr.Value);
                        }
                    }
                    else
                    {
                        if (attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                        {
                            DebugFix.Assert(nsUriAttr == XmlReaderWriterHelper.NS_URL_XMLNS);
                            xmlWriter.WriteAttributeString(XmlReaderWriterHelper.NS_PREFIX_XMLNS, attrLocalName, XmlReaderWriterHelper.NS_URL_XMLNS,
                                                           xmlAttr.Value);
                        }
                        else if (attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XML))
                        {
                            DebugFix.Assert(nsUriAttr == XmlReaderWriterHelper.NS_URL_XML);
                            xmlWriter.WriteAttributeString(XmlReaderWriterHelper.NS_PREFIX_XML, attrLocalName, XmlReaderWriterHelper.NS_URL_XML,
                                                           xmlAttr.Value);
                        }
                        else if (!string.IsNullOrEmpty(nsUriAttr))
                        {
#if DEBUG
                            DebugFix.Assert(nsUriAttr != nsUri);

                            string uriCheck = xmlProp.GetNamespaceUri(attrNSPrefix);
                            DebugFix.Assert(nsUriAttr == uriCheck);
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrNSPrefix, attrLocalName, nsUriAttr, xmlAttr.Value);
                        }
                        else if (!string.IsNullOrEmpty(nsUri))
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrNSPrefix, attrLocalName, nsUri, xmlAttr.Value);
                        }
                        else
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrLocalName, xmlAttr.Value);
                        }
                    }
                }

                for (int i = 0; i < mChildren.Count; i++)
                {
                    TreeNode child = mChildren.Get(i);

                    child.GetXmlFragment_(xmlWriter);
                }

                xmlWriter.WriteEndElement();
            }

            AbstractTextMedia txt = GetTextMedia();
            if (txt != null)
            {
                DebugFix.Assert(!HasXmlProperty || mChildren.Count == 0);
                xmlWriter.WriteString(txt.Text);
            }
        }

        public string GetXmlFragment()
        {
            if (!HasXmlProperty)
            {
                return null;
            }

            StringWriter strWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(strWriter);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.Indentation = 4;
            xmlWriter.IndentChar = ' ';
            xmlWriter.Namespaces = true;
            xmlWriter.QuoteChar = '"';

            GetXmlFragment_(xmlWriter);

            xmlWriter.Flush();
            xmlWriter.Close();

            //string xmlFragment = Encoding.UTF8.GetString(memStream.GetBuffer());
            string xmlFragment = strWriter.ToString();
            return xmlFragment;
        }
    }
}
