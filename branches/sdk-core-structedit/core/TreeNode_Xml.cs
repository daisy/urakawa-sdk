using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.media;
using urakawa.media.data.image;
using urakawa.media.data.video;
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
                return GetXmlElementPrefixedLocalName();
            }
            return base.ToString();
        }

        protected string getDebugString()
        {
            string localName = GetXmlElementPrefixedLocalName();
            String str = (localName != null ? localName : "NO XML");
            str += " /// ";
            string txt = GetTextFlattened();
            str += txt;
            return str;
        }


        public string GetXmlElementId()
        {
            XmlProperty xmlProp = GetXmlProperty();
            if (xmlProp != null)
            {
                return xmlProp.GetIdFromAttributes();
            }
            return null;
        }

        public TreeNode GetFirstDescendantOrSelfWithXmlID(string id)
        {
            if (HasXmlProperty && GetXmlElementId() == id)
            {
                return this;
            }

            return GetFirstDescendantWithXmlID(id);
        }

        public TreeNode GetFirstDescendantWithXmlID(string id)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                if (child.HasXmlProperty && child.GetXmlElementId() == id)
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithXmlID(id);
                if (childIn != null)
                {
                    return childIn;
                }
            }

            return null;
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

        //public TreeNode GetFirstChildWithXmlElementName(string elemName)
        //{
        //    if (HasXmlProperty && GetXmlElementLocalName() == elemName)
        //    {
        //        return this;
        //    }

        //    for (int i = 0; i < mChildren.Count; i++)
        //    {
        //        TreeNode child = mChildren.Get(i).GetFirstChildWithXmlElementName(elemName);
        //        if (child != null)
        //        {
        //            return child;
        //        }
        //    }

        //    return null;
        //}

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

        public TreeNode GetLastDescendantWithXmlElement(string localName)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

                if (child.HasXmlProperty && child.GetXmlElementLocalName() == localName)
                {
                    return child;
                }

                TreeNode childIn = child.GetLastDescendantWithXmlElement(localName);
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

                TreeNode previousIn = previous.GetLastDescendantWithXmlElement(localName);
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


        public TreeNode GetFirstDescendantWithXmlAttribute(string attrName)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                if (child.HasXmlProperty && child.GetXmlProperty().GetAttribute(attrName) != null)
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithXmlAttribute(attrName);
                if (childIn != null)
                {
                    return childIn;
                }
            }

            return null;
        }

        public TreeNode GetLastDescendantWithXmlAttribute(string attrName)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

                if (child.HasXmlProperty && child.GetXmlProperty().GetAttribute(attrName) != null)
                {
                    return child;
                }

                TreeNode childIn = child.GetLastDescendantWithXmlAttribute(attrName);
                if (childIn != null)
                {
                    return childIn;
                }
            }

            return null;
        }

        public TreeNode GetPreviousSiblingWithXmlAttribute(string attrName)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode previous = this;
            while ((previous = previous.PreviousSibling) != null)
            {
                if (previous.HasXmlProperty && previous.GetXmlProperty().GetAttribute(attrName) != null)
                {
                    return previous;
                }

                TreeNode previousIn = previous.GetLastDescendantWithXmlAttribute(attrName);
                if (previousIn != null)
                {
                    return previousIn;
                }
            }

            return Parent.GetPreviousSiblingWithXmlAttribute(attrName);
        }

        public TreeNode GetNextSiblingWithXmlAttribute(string attrName)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                if (next.HasXmlProperty && next.GetXmlProperty().GetAttribute(attrName) != null)
                {
                    return next;
                }

                TreeNode nextIn = next.GetFirstDescendantWithXmlAttribute(attrName);
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            return Parent.GetNextSiblingWithXmlAttribute(attrName);
        }

        public bool HasXmlProperty
        {
            get { return GetXmlProperty() != null; }
        }
        public XmlProperty GetOrCreateXmlProperty()
        {
            return GetOrCreateProperty<XmlProperty>();
        }
        public XmlProperty GetXmlProperty()
        {
            return GetProperty<XmlProperty>();
        }

        public bool NeedsXmlNamespacePrefix()
        {
            if (!HasXmlProperty)
            {
                return false;
            }
            string nsUri_NearestXmlns = null;

            TreeNode node = this;
            while (node != null)
            {
                XmlProperty xmlProp = node.GetXmlProperty();

                foreach (XmlAttribute xmlAttr in xmlProp.Attributes.ContentsAs_Enumerable)
                {
                    string attrNSPrefix = xmlAttr.Prefix;
                    string attrLocalName = xmlAttr.PrefixedLocalName != null
                                               ? xmlAttr.PrefixedLocalName
                                               : xmlAttr.LocalName;
                    if (String.IsNullOrEmpty(attrNSPrefix) &&
                        attrLocalName.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                    {
                        // xmlns="URI"
                        nsUri_NearestXmlns = xmlAttr.Value;
                        break;
                    }
                }
                if (!String.IsNullOrEmpty(nsUri_NearestXmlns))
                {
                    break;
                }

                node = node.Parent;
            }
            if (String.IsNullOrEmpty(nsUri_NearestXmlns))
            {
                nsUri_NearestXmlns = Presentation.PropertyFactory.DefaultXmlNamespaceUri;
            }

            string nsUri = GetXmlNamespaceUri();
            if (nsUri_NearestXmlns == nsUri)
            {
                return false;
            }

            return true;
        }

        public string GetXmlElementPrefixedLocalName()
        {
            if (!HasXmlProperty)
            {
                return null;
            }
            XmlProperty xmlProp = GetXmlProperty();
            string localName = xmlProp.LocalName;

            if (NeedsXmlNamespacePrefix())
            {
                string nsUri = GetXmlNamespaceUri();
                string prefix = GetXmlNamespacePrefix(nsUri);

#if DEBUG
                if (string.IsNullOrEmpty(prefix))
                {
                    Debugger.Break();
                }
#endif
                return prefix + ":" + localName;
            }

            return localName;
        }

        public string GetXmlElementLocalName()
        {
            //QualifiedName qName = GetXmlElementQName();
            //if (qName == null) return null;
            //return qName.LocalName;

            if (!HasXmlProperty)
            {
                return null;
            }
            XmlProperty xmlProp = GetXmlProperty();
            return xmlProp.LocalName;
        }

        //private QualifiedName m_QualifiedName = null;
        //private QualifiedName GetXmlElementQName()
        //{
        //    XmlProperty xmlProp = GetXmlProperty();
        //    if (xmlProp != null)
        //    {
        //        //TODOx QualifiedName fields are unmutable,
        //        // so unless the underlying XmlProperty fields change, caching is okay
        //        // (here we assume that once a TreeNode as been XukedIn,
        //        // its XML definition does not change)
        //        if (m_QualifiedName == null || xmlProp.QNameIsInvalidated)
        //        {
        //            string nsUri = GetXmlNamespaceUri();

        //            m_QualifiedName = new QualifiedName(xmlProp.LocalName, nsUri);

        //            xmlProp.QNameIsInvalidated = false;
        //        }
        //    }
        //    else
        //    {
        //        m_QualifiedName = null;
        //    }

        //    return m_QualifiedName;
        //}


        public string GetXmlElementLang()
        {
            XmlProperty xmlProp = GetXmlProperty();
            if (xmlProp != null)
            {
                return xmlProp.GetLangFromAttributes();
            }
            return null;
        }


        public string GetXmlNamespacePrefix(string uri)
        {
            if (!HasXmlProperty)
            {
                return null;
            }

            XmlProperty xmlProp = GetXmlProperty();
            return xmlProp.GetXmlNamespacePrefix(uri);
        }

        public string GetXmlNamespaceUri(string prefix)
        {
            if (!HasXmlProperty)
            {
                return null;
            }

            XmlProperty xmlProp = GetXmlProperty();
            return xmlProp.GetNamespaceUri(prefix);
        }

        public string GetXmlNamespaceUri()
        {
            return GetXmlNamespaceUri(null);
        }

        protected void GetXmlFragment_(XmlWriter xmlWriter, bool absoluteLinks)
        {
            if (HasXmlProperty)
            {
                string name = GetXmlElementPrefixedLocalName();
                string NSPrefix;
                string localName;
                XmlProperty.SplitLocalName(name, out NSPrefix, out localName);

                //DebugFix.Assert(string.IsNullOrEmpty(NSPrefix));

                string nsUri = GetXmlNamespaceUri(NSPrefix);

                XmlProperty xmlProp = GetXmlProperty();

                if (String.IsNullOrEmpty(NSPrefix))
                {
                    if (!String.IsNullOrEmpty(nsUri))
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
                    //#if DEBUG
                    //                    Debugger.Break();
                    //#endif //DEBUG
                    if (!String.IsNullOrEmpty(nsUri))
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

                ManagedImageMedia manImageMedia = GetImageMedia() as ManagedImageMedia;
                ManagedVideoMedia manVideoMedia = GetVideoMedia() as ManagedVideoMedia;

                foreach (XmlAttribute xmlAttr in xmlProp.Attributes.ContentsAs_Enumerable)
                {
                    string value = xmlAttr.Value;

                    string nsUriAttr = xmlAttr.GetNamespaceUri();

                    string attrNSPrefix = xmlAttr.Prefix;
                    string nameWithoutPrefix = xmlAttr.PrefixedLocalName != null ? xmlAttr.PrefixedLocalName : xmlAttr.LocalName;

                    if (manImageMedia != null &&
                        (nameWithoutPrefix == "href" || nameWithoutPrefix == "src"))
                    {
                        DebugFix.Assert(manImageMedia.ImageMediaData.OriginalRelativePath == value);
                        if (absoluteLinks)
                        {
                            value = ((FileDataProvider)manImageMedia.ImageMediaData.DataProvider).DataFileFullPath;
                        }
                    }

                    if (manVideoMedia != null &&
                        (nameWithoutPrefix == "href" || nameWithoutPrefix == "src"))
                    {
                        DebugFix.Assert(manVideoMedia.VideoMediaData.OriginalRelativePath == value);
                        if (absoluteLinks)
                        {
                            value = ((FileDataProvider)manVideoMedia.VideoMediaData.DataProvider).DataFileFullPath;
                        }
                    }


                    if (String.IsNullOrEmpty(attrNSPrefix))
                    {
                        if (nameWithoutPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                        {
                            DebugFix.Assert(XmlReaderWriterHelper.NS_URL_XMLNS.Equals(nsUriAttr));

                            xmlWriter.WriteAttributeString(XmlReaderWriterHelper.NS_PREFIX_XMLNS, XmlReaderWriterHelper.NS_URL_XMLNS, value);
                        }
                        else if (!String.IsNullOrEmpty(nsUriAttr) && nsUriAttr != nsUri)
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(nameWithoutPrefix, nsUriAttr, value);
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString(nameWithoutPrefix, value);
                        }
                    }
                    else
                    {
                        if (attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                        {
                            DebugFix.Assert(nsUriAttr == XmlReaderWriterHelper.NS_URL_XMLNS);
                            xmlWriter.WriteAttributeString(XmlReaderWriterHelper.NS_PREFIX_XMLNS, nameWithoutPrefix, XmlReaderWriterHelper.NS_URL_XMLNS,
                                                           value);
                        }
                        else if (attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XML))
                        {
                            DebugFix.Assert(nsUriAttr == XmlReaderWriterHelper.NS_URL_XML);
                            xmlWriter.WriteAttributeString(XmlReaderWriterHelper.NS_PREFIX_XML, nameWithoutPrefix, XmlReaderWriterHelper.NS_URL_XML,
                                                           value);
                        }
                        else if (!String.IsNullOrEmpty(nsUriAttr))
                        {
#if DEBUG
                            //DebugFix.Assert(nsUriAttr != nsUri);

                            string uriCheck = xmlProp.GetNamespaceUri(attrNSPrefix);
                            DebugFix.Assert(nsUriAttr == uriCheck);
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrNSPrefix, nameWithoutPrefix, nsUriAttr, value);
                        }
                        else if (!String.IsNullOrEmpty(nsUri))
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(attrNSPrefix, nameWithoutPrefix, nsUri, value);
                        }
                        else
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG
                            xmlWriter.WriteAttributeString(nameWithoutPrefix, value);
                        }
                    }
                }
            }

            if (mChildren.Count > 0)
            {
                for (int i = 0; i < mChildren.Count; i++)
                {
                    TreeNode child = mChildren.Get(i);

                    child.GetXmlFragment_(xmlWriter, absoluteLinks);
                }
            }
            else
            {
                AbstractTextMedia txt = GetTextMedia();
                if (txt != null)
                {
                    DebugFix.Assert(!HasXmlProperty || mChildren.Count == 0);

                    bool needsCData = false;
                    TreeNode node = null;
                    if (HasXmlProperty)
                    {
                        node = this;
                    }
                    else if (Parent != null && Parent.HasXmlProperty)
                    {
                        node = Parent;
                    }
                    if (node != null)
                    {
                        string name = node.GetXmlElementLocalName();
                        if (name == "script" || name == "style")
                        {
                            needsCData = true;
                        }
                    }

                    if (needsCData)
                    {
                        xmlWriter.WriteCData(txt.Text);
                    }
                    else
                    {
                        xmlWriter.WriteString(txt.Text);
                    }
                }
            }

            if (HasXmlProperty)
            {
                xmlWriter.WriteEndElement();
            }
        }

        public string GetXmlFragment(bool absoluteLinks)
        {
            if (!HasXmlProperty)
            {
                return null;
            }

            //StringWriter strWriter = new StringWriter();
            //strWriter.NewLine = Environment.NewLine;
            ////strWriter.NewLine = @"\n";
            ////strWriter.Encoding = Encoding.UTF8;
            
            StringBuilder strWriter = new StringBuilder();

            bool pretty = true;

            XmlWriterSettings settings = XmlReaderWriterHelper.GetDefaultXmlWriterConfiguration(pretty);
            XmlWriter xmlWriter = XmlWriter.Create(strWriter, settings);

            if (pretty && xmlWriter is XmlTextWriter)
            {
                ((XmlTextWriter)xmlWriter).Formatting = Formatting.Indented;
            }

            //XmlTextWriter xmlWriter = new XmlTextWriter(strWriter);
            //xmlWriter.Formatting = Formatting.Indented;
            //xmlWriter.Indentation = 4;
            //xmlWriter.IndentChar = ' ';
            //xmlWriter.Namespaces = true;
            //xmlWriter.QuoteChar = '"';

            GetXmlFragment_(xmlWriter, absoluteLinks);

            xmlWriter.Flush();
            xmlWriter.Close();

            //string xmlFragment = Encoding.UTF8.GetString(memStream.GetBuffer());
            string xmlFragment = strWriter.ToString();
            return xmlFragment;
        }
    }
}
