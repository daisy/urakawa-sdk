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
            QualifiedName qname = GetXmlElementQName();
            if (qname != null)
            {
                return qname.LocalName;
            }
            return base.ToString();
        }

        protected string getDebugString()
        {
            QualifiedName qname = GetXmlElementQName();
            String str = (qname != null ? qname.LocalName : "");
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

            QualifiedName qName = Parent.GetXmlElementQName();
            if (qName != null && qName.LocalName == localName)
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
                QualifiedName qName = child.GetXmlElementQName();
                if (qName != null && qName.LocalName == localName)
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
                QualifiedName qName = previous.GetXmlElementQName();
                if (qName != null && qName.LocalName == localName)
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
                QualifiedName qName = next.GetXmlElementQName();
                if (qName != null && qName.LocalName == localName)
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

        private QualifiedName m_QualifiedName = null;
        public QualifiedName GetXmlElementQName()
        {
            //TODO QualifiedName fields are unmutable,
            // so unless the underlying XmlProperty fields change, caching is okay
            // (here we assume that once a TreeNode as been XukedIn,
            // its XML definition does not change)
            if (m_QualifiedName != null)
            {
                return m_QualifiedName;
            }
            XmlProperty xmlProp = GetProperty<XmlProperty>();
            if (xmlProp != null)
            {
                m_QualifiedName = new QualifiedName(xmlProp.LocalName, xmlProp.NamespaceUri);
                return m_QualifiedName;
            }
            return null;
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
                XmlAttribute idAttr = xmlProp.GetAttribute("id", "");
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
                XmlAttribute langAttr = xmlProp.GetAttribute("lang", "");
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
            QualifiedName qname = GetXmlElementQName();
            if (qname != null && qname.LocalName == elemName) return this;

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
            QualifiedName qname = GetXmlElementQName();
            if (qname == null)
            {
                return null;
            }

            string name = qname.LocalName;
            string NSPrefix = null;
            string localName = null;
            if (name.IndexOf(':') >= 0) //name.Contains(":"))
            {
                string[] arr = name.Split(':');
                NSPrefix = arr[0];
                localName = arr[1];
            }

            XmlProperty xmlProp = GetProperty<XmlProperty>();

            if (Parent == null && string.IsNullOrEmpty(xmlProp.NamespaceUri))
            {
                return Presentation.PropertyFactory.DefaultXmlNamespaceUri;
            }

            if (!string.IsNullOrEmpty(xmlProp.NamespaceUri)
                &&
                (
                string.IsNullOrEmpty(prefix)
                ||
                (!string.IsNullOrEmpty(NSPrefix) && prefix.Equals(NSPrefix))
                )
                )
            {
                return xmlProp.NamespaceUri;
            }


            foreach (XmlAttribute xmlAttr in xmlProp.Attributes)
            {
                string attrName = xmlAttr.LocalName;
                string attrNSPrefix = null;
                string attrLocalName = null;
                if (attrName.IndexOf(':') >= 0) //attrName.Contains(":"))
                {
                    string[] arr = attrName.Split(':');
                    attrNSPrefix = arr[0];
                    attrLocalName = arr[1];
                }

                if (string.IsNullOrEmpty(prefix))
                {
                    if (string.IsNullOrEmpty(attrNSPrefix) && attrName.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                    {
                        return xmlAttr.Value;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(attrNSPrefix) && attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                    {
                        if (prefix.Equals(attrLocalName))
                        {
                            return xmlAttr.Value;
                        }
                    }
                }
            }

            TreeNode node = Parent;
            while (node != null)
            {
                string ns = node.GetXmlNamespaceUri(prefix);
                if (!string.IsNullOrEmpty(ns))
                {
                    return ns;
                }
                node = node.Parent;
            }

            return null;
        }

        public string GetXmlNamespaceUri()
        {
            return GetXmlNamespaceUri(null);
        }

        protected void GetXmlFragment_(XmlTextWriter xmlWriter)
        {
            QualifiedName qname = GetXmlElementQName();
            if (qname != null)
            {
                string name = qname.LocalName;
                string NSPrefix = null;
                string localName = null;
                if (name.IndexOf(':') >= 0) //name.Contains(":"))
                {
                    string[] arr = name.Split(':');
                    NSPrefix = arr[0];
                    localName = arr[1];
                }

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

                foreach (XmlAttribute xmlAttr in xmlProp.Attributes)
                {
                    string attrName = xmlAttr.LocalName;
                    string attrNSPrefix = null;
                    string attrLocalName = null;
                    if (attrName.IndexOf(':') >= 0) //attrName.Contains(":"))
                    {
                        string[] arr = attrName.Split(':');
                        attrNSPrefix = arr[0];
                        attrLocalName = arr[1];
                    }

                    if (string.IsNullOrEmpty(attrNSPrefix))
                    {
                        if (attrName.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                        {
                            DebugFix.Assert(XmlReaderWriterHelper.NS_URL_XMLNS.Equals(xmlAttr.NamespaceUri));

                            xmlWriter.WriteAttributeString(attrName, xmlAttr.NamespaceUri, xmlAttr.Value);
                        }
                        else if (!string.IsNullOrEmpty(xmlAttr.NamespaceUri))
                        {
#if DEBUG
                            //Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrName, xmlAttr.NamespaceUri, xmlAttr.Value);
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString(attrName, xmlAttr.Value);
                        }
                    }
                    else
                    {
                        if (attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS) || attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XML))
                        {
                            //http://www.w3.org/2000/xmlns/
                            xmlWriter.WriteAttributeString(attrNSPrefix, attrLocalName, XmlReaderWriterHelper.NS_URL_XMLNS,
                                                           xmlAttr.Value);
                        }
                        else if (!string.IsNullOrEmpty(xmlAttr.NamespaceUri))
                        {
#if DEBUG
                            //Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrNSPrefix, attrLocalName, xmlAttr.NamespaceUri, xmlAttr.Value);
                        }
                        else if (!string.IsNullOrEmpty(nsUri))
                        {
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
                DebugFix.Assert(qname == null || mChildren.Count == 0);
                xmlWriter.WriteString(txt.Text);
            }
        }

        public string GetXmlFragment()
        {
            QualifiedName qname = GetXmlElementQName();
            if (qname == null)
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
