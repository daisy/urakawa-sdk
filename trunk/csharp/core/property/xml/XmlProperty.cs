using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.core;
using urakawa.progress;
using urakawa.property;
using urakawa.xuk;

namespace urakawa.property.xml
{
    [XukNameUglyPrettyAttribute("xP", "XmlProperty")]
    public class XmlProperty : Property
    {
        //public override TreeNode TreeNodeOwner
        //{
        //    set
        //    {
        //        if (mOwner != null && value == null) //base.TreeNodeOwner NOT INIT YET!
        //        {
        //            TreeNode.UpdateTextDirectionality(mOwner, null);
        //        }

        //        base.TreeNodeOwner = value;

        //        if (base.TreeNodeOwner != null)
        //        {
        //            TreeNode.UpdateTextDirectionality(base.TreeNodeOwner, this);
        //        }
        //    }
        //}
        

        #region Event related members

        /// <summary>
        /// Event fired after the QName of the <see cref="XmlProperty"/> has changed
        /// </summary>
        public event EventHandler<urakawa.events.property.xml.QNameChangedEventArgs> QNameChanged;

        /// <summary>
        /// Fires the <see cref="QNameChanged"/> event
        /// </summary>
        /// <param name="src">The source, that is the <see cref="XmlProperty"/> whoose QName changed</param>
        /// <param name="newLocalName">The local name part of the new QName</param>
        /// <param name="newNamespaceUri">The namespace uri part of the new QName</param>
        /// <param name="prevLocalName">The local name part of the QName before the change</param>
        /// <param name="prevNamespaceUri">The namespace uri part of the QName before the change</param>
        protected void NotifyQNameChanged(XmlProperty src, string newLocalName, string newNamespaceUri,
                                          string prevLocalName, string prevNamespaceUri)
        {
            EventHandler<urakawa.events.property.xml.QNameChangedEventArgs> d = QNameChanged;
            if (d != null)
                d(this,
                  new urakawa.events.property.xml.QNameChangedEventArgs(src, newLocalName, newNamespaceUri,
                                                                        prevLocalName, prevNamespaceUri));
        }

        /// <summary>
        /// Event fired after an attribute of an <see cref="XmlProperty"/> has been set
        /// </summary>
        public event EventHandler<urakawa.events.property.xml.XmlAttributeSetEventArgs> XmlAttributeSet;

        /// <summary>
        /// Fires the <see cref="XmlAttributeSet"/> event
        /// </summary>
        /// <param name="src">The source, that is the <see cref="XmlProperty"/> on which an attribute was set</param>
        /// <param name="attrLN">The local name part of the QName of the attribute that was set</param>
        /// <param name="attrNS">The namespace uri part of the QName of the attribute that was set</param>
        /// <param name="newVal">The new value of the attribute - may be <c>null</c></param>
        /// <param name="prevVal">The previous value of the attribute - may be <c>null</c></param>
        protected void NotifyXmlAttributeSet(XmlProperty src, string attrLN, string attrNS, string newVal,
                                             string prevVal)
        {
            EventHandler<urakawa.events.property.xml.XmlAttributeSetEventArgs> d = XmlAttributeSet;
            if (d != null)
                d(this, new urakawa.events.property.xml.XmlAttributeSetEventArgs(src, attrLN, attrNS, newVal, prevVal));
        }

        private void this_qNameChanged(object sender, urakawa.events.property.xml.QNameChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        private void this_xmlAttributeSet(object sender, urakawa.events.property.xml.XmlAttributeSetEventArgs e)
        {
            NotifyChanged(e);
        }

        private void Attribute_valueChanged(object sender, XmlAttribute.ValueChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        private ObjectListProvider<XmlAttribute> mAttributes = null;

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="Property"/>s should only be created via. the <see cref="PropertyFactory"/>
        /// </summary>
        public XmlProperty()
        {
            mAttributes = new ObjectListProvider<XmlAttribute>(this, false);

            QNameChanged += new EventHandler<urakawa.events.property.xml.QNameChangedEventArgs>(this_qNameChanged);
            XmlAttributeSet +=
                new EventHandler<urakawa.events.property.xml.XmlAttributeSetEventArgs>(this_xmlAttributeSet);
        }

        internal string mLocalName = null;
        private string mNamespaceUri = "";
        //private IDictionary<string, XmlAttribute> mAttributes = new Dictionary<string, XmlAttribute>();


        public string GetIdFromAttributes()
        {
            XmlAttribute langAttr = GetAttribute(XmlReaderWriterHelper.XmlId, XmlReaderWriterHelper.NS_URL_XML);
            if (langAttr == null)
            {
                langAttr = GetAttribute("id");
            }
            if (langAttr != null)
            {
                return (string.IsNullOrEmpty(langAttr.Value) ? null : langAttr.Value);
            }
            return null;
        }

        public string GetLangFromAttributes()
        {
            XmlAttribute langAttr = GetAttribute(XmlReaderWriterHelper.XmlLang, XmlReaderWriterHelper.NS_URL_XML);
            if (langAttr == null)
            {
                langAttr = GetAttribute("lang");
            }
            if (langAttr != null)
            {
                return (string.IsNullOrEmpty(langAttr.Value) ? null : langAttr.Value);
            }
            return null;
        }

        /// <summary>
        /// Gets the local localName of <c>this</c>
        /// </summary>
        /// <returns>The local localName</returns>
        public string LocalName
        {
            get
            {
                if (mLocalName == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The XmlProperty has not been initialized with a local name");
                }
                return mLocalName;
            }
            //set { SetQName(value, NamespaceUri); }
        }

        /// <summary>
        /// Gets the namespace uri of <c>this</c>
        /// </summary>
        /// <returns>The namespace uri</returns>
        private string NamespaceUri
        {
            get { return mNamespaceUri; }
            //set { SetQName(LocalName, value); }
        }

        //private bool m_QNameIsInvalidated;
        //public bool QNameIsInvalidated
        //{
        //    get { return m_QNameIsInvalidated; }
        //    set { m_QNameIsInvalidated = value; }
        //}

        public static void SplitLocalName(string name, out string prefix, out string realLocalName)
        {
            prefix = null;
            realLocalName = null;

            if (name != null && name.IndexOf(':') >= 0) //mLocalName.Contains(":"))
            {
                string[] arr = name.Split(':');
                prefix = arr[0];
                realLocalName = arr[1];
            }
        }

        public string GetXmlNamespacePrefix(string uri)
        {
            foreach (XmlAttribute xmlAttr in Attributes.ContentsAs_Enumerable)
            {
                string attrNSPrefix = xmlAttr.Prefix;
                string attrLocalName = xmlAttr.PrefixedLocalName != null ? xmlAttr.PrefixedLocalName : xmlAttr.LocalName;

                if (XmlReaderWriterHelper.NS_PREFIX_XMLNS.Equals(attrNSPrefix)
                    && xmlAttr.Value == uri)
                {
                    return attrLocalName;
                }
            }

            if (TreeNodeOwner != null)
            {
                TreeNode node = TreeNodeOwner.Parent;
                while (node != null)
                {
                    string prefix = node.GetXmlNamespacePrefix(uri);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        return prefix;
                    }
                    node = node.Parent;
                }
            }

            //#if DEBUG
            //            Debugger.Break();
            //#endif //DEBUG
            return null;
        }

        public string GetNamespaceUri(string prefix)
        {
            if (XmlReaderWriterHelper.NS_PREFIX_XML.Equals(prefix))
            {
                return XmlReaderWriterHelper.NS_URL_XML;
            }

            if (XmlReaderWriterHelper.NS_PREFIX_XMLNS.Equals(prefix))
            {
                return XmlReaderWriterHelper.NS_URL_XMLNS;
            }

            string NSPrefix;
            string localName;
            SplitLocalName(LocalName, out NSPrefix, out localName);

            DebugFix.Assert(string.IsNullOrEmpty(NSPrefix));

            if (!string.IsNullOrEmpty(NamespaceUri)
                &&
                (
                string.IsNullOrEmpty(prefix)
                ||
                (!string.IsNullOrEmpty(NSPrefix) && prefix.Equals(NSPrefix))
                )
                )
            {
                DebugFix.Assert(string.IsNullOrEmpty(prefix));

                return NamespaceUri;
            }


            foreach (XmlAttribute xmlAttr in Attributes.ContentsAs_Enumerable)
            {
                string attrNSPrefix = xmlAttr.Prefix;
                string attrLocalName = xmlAttr.PrefixedLocalName != null ? xmlAttr.PrefixedLocalName : xmlAttr.LocalName;

                if (string.IsNullOrEmpty(prefix))
                {
                    if (string.IsNullOrEmpty(attrNSPrefix) && attrLocalName.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                    {
                        // xmlns="URI"
                        return xmlAttr.Value;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(attrNSPrefix) && attrNSPrefix.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS))
                    {
                        if (prefix.Equals(attrLocalName))
                        {
                            // xmlns:prefix="URI"
                            return xmlAttr.Value;
                        }
                    }
                }
            }

            if (TreeNodeOwner == null)
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG
                if (string.IsNullOrEmpty(prefix))
                {
                    return Presentation.PropertyFactory.DefaultXmlNamespaceUri;
                }

                return null;
            }

            TreeNode node = TreeNodeOwner.Parent;
            while (node != null)
            {
                string ns = node.GetXmlNamespaceUri(prefix);
                if (!string.IsNullOrEmpty(ns))
                {
                    return ns;
                }
                node = node.Parent;
            }

            if (string.IsNullOrEmpty(prefix))
            {
                return Presentation.PropertyFactory.DefaultXmlNamespaceUri;
            }

            //#if DEBUG
            //            Debugger.Break();
            //#endif //DEBUG
            return null;
        }

        public string GetNamespaceUri()
        {
            return GetNamespaceUri(null);
        }

        /// <summary>
        /// Sets the QName of <c>this</c> (i.e. the local localName and namespace uri)
        /// </summary>
        /// <param name="newLocalName">
        /// The local localName part of the new QName
        /// - must not be <c>null</c> or <see cref="String.Empty"/>
        /// </param>
        /// <param name="newNamespaceUri">
        /// The namespace uri part of the new QName - must not be <c>null</c>
        /// </param>
        public void SetQName(string newLocalName, string newNamespaceUri)
        {
            if (newLocalName == null)
            {
                throw new exception.MethodParameterIsNullException("The local localName must not be null");
            }
            if (newLocalName == String.Empty)
            {
                throw new exception.MethodParameterIsEmptyStringException("The local localName must not be empty");
            }
            if (newNamespaceUri == null)
            {
                throw new exception.MethodParameterIsNullException("The namespace uri must not be null");
            }

            //m_QNameIsInvalidated = true;

            string prefix;
            string localName;
            SplitLocalName(newLocalName, out prefix, out localName);
            if (!string.IsNullOrEmpty(prefix))
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG
                newLocalName = localName;
            }

            string prevLN = mLocalName;
            string prevNS = mNamespaceUri;
            mLocalName = newLocalName;
            mNamespaceUri = newNamespaceUri;
            NotifyQNameChanged(this, newLocalName, newNamespaceUri, prevLN, prevNS);
        }

        /// <summary>
        /// Gets a list of the <see cref="XmlAttribute"/>s of <c>this</c>
        /// </summary>
        /// <returns>The list</returns>
        public ObjectListProvider<XmlAttribute> Attributes
        {
            get { return mAttributes; }
        }

        /// <summary>
        /// Sets an <see cref="XmlAttribute"/>, possibly overwriting an existing one
        /// </summary>
        /// <param name="newAttribute">The <see cref="XmlAttribute"/> to set</param>
        /// <returns>A <see cref="bool"/> indicating if an existing <see cref="XmlAttribute"/> was overwritten</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the <see cref="XmlAttribute"/> to set is <c>null</c>
        /// </exception>
        public bool SetAttribute(XmlAttribute newAttribute)
        {
            if (newAttribute == null)
            {
                throw new exception.MethodParameterIsNullException("Can not set a null xml attribute");
            }
            //string key = String.Format("{1}:{0}", newAttribute.LocalName, newAttribute.NamespaceUri);

            string prevValue = null;

            XmlAttribute obj = GetAttribute(newAttribute.LocalName, newAttribute.GetNamespaceUri());

            //XmlAttribute obj;
            //mAttributes.TryGetValue(key, out obj);

            if (obj != null) //mAttributes.ContainsKey(key))
            {
                XmlAttribute prevAttr = obj; // mAttributes[key];
                prevValue = prevAttr.Value;
                RemoveAttribute(prevAttr);
            }

            //mAttributes.Add(key, newAttribute);
            mAttributes.Insert(mAttributes.Count, newAttribute);

            newAttribute.Parent = this;
            newAttribute.ValueChanged += new EventHandler<XmlAttribute.ValueChangedEventArgs>(Attribute_valueChanged);
            NotifyXmlAttributeSet(this, newAttribute.LocalName, newAttribute.GetNamespaceUri(), newAttribute.Value, prevValue);
            return (prevValue != null);
        }

        /// <summary>
        /// Sets an <see cref="XmlAttribute"/>, possibly overwriting an existing one
        /// </summary>
        /// <param name="localName">The local localName of the new attribute</param>
        /// <param name="namespaceUri">The namespace uri part of the new attribute</param>
        /// <param name="value">The value of the new attribute</param>
        /// <returns>A <see cref="bool"/> indicating if an existing <see cref="XmlAttribute"/> was overwritten</returns>
        /// <exception cref="exception.FactoryCannotCreateTypeException">
        /// <see cref="Property.PropertyFactory"/> for information on when this <see cref="Exception"/> is thrown
        /// </exception>
        public bool SetAttribute(string localName, string namespaceUri, string value)
        {
            XmlAttribute attr = GetAttribute(localName, namespaceUri);
            if (attr == null)
            {
                attr = new XmlAttribute();
                attr.SetQName(localName, namespaceUri == null ? "" : namespaceUri);
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
        /// Removes an <see cref="XmlAttribute"/> by QName
        /// </summary>
        /// <param name="localName">The localName part of the QName of the <see cref="XmlAttribute"/> to remove</param>
        /// <param name="namespaceUri">The namespaceUri part of the QName of the <see cref="XmlAttribute"/> to remove</param>
        /// <returns>The removes <see cref="XmlAttribute"/></returns>
        /// <exception cref="exception.XmlAttributeDoesNotExistsException">
        /// Thrown when the <see cref="XmlProperty"/> does not have an event with the given QName</exception>
        public XmlAttribute RemoveAttribute(string localName, string namespaceUri)
        {
            XmlAttribute attrToRemove = GetAttribute(localName, namespaceUri);
            if (attrToRemove == null)
            {
                throw new exception.XmlAttributeDoesNotExistsException(String.Format(
                                                                           "The XmlProperty does not have an attribute with QName {1}:{0}",
                                                                           localName, namespaceUri));
            }
            RemoveAttribute(attrToRemove);
            //string key = String.Format("{1}:{0}", attrToRemove.LocalName, attrToRemove.NamespaceUri);
            //if (!Object.ReferenceEquals(attrToRemove, attrToRemove))
            //{
            //    throw new exception.XmlAttributeDoesNotBelongException(
            //        "The given XmlAttribute does not belong to the XmlProperty");
            //}
            return attrToRemove;
        }

        /// <summary>
        /// Removes a given <see cref="XmlAttribute"/>
        /// </summary>
        /// <param name="attrToRemove">The <see cref="XmlAttribute"/> to remove</param>
        /// <exception cref="exception.XmlAttributeDoesNotBelongException">
        /// Thrown when the given <see cref="XmlAttribute"/> instance does not belong to the <see cref="XmlProperty"/>
        /// </exception>
        public void RemoveAttribute(XmlAttribute attrToRemove)
        {
            bool found = false;
            foreach (XmlAttribute attr in mAttributes.ContentsAs_Enumerable)
            {
                if (attr == attrToRemove)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                attrToRemove.ValueChanged -= new EventHandler<XmlAttribute.ValueChangedEventArgs>(Attribute_valueChanged);

                mAttributes.Remove(attrToRemove);

                attrToRemove.Parent = null;

                NotifyXmlAttributeSet(this, attrToRemove.LocalName, attrToRemove.GetNamespaceUri(), null, attrToRemove.Value);

                return;
            }

            throw new exception.XmlAttributeDoesNotExistsException(String.Format(
                                                                       "The XmlProperty does not have an attribute with QName {1}:{0}",
                                                                       attrToRemove.LocalName, attrToRemove.GetNamespaceUri()));
        }

        /// <summary>
        /// Gets the <see cref="XmlAttribute"/> with a given QName
        /// </summary>
        /// <param name="localName">The local localName part of the given QName</param>
        /// <param name="namespaceUri">The namespce uri part of the given QName</param>
        /// <returns>The <see cref="XmlAttribute"/> if found, otherwise <c>null</c></returns>
        public XmlAttribute GetAttribute(string name, string namespaceUri)
        {
            bool noNamespaceSpecified = string.IsNullOrEmpty(namespaceUri);

            string prefix;
            string localName;
            SplitLocalName(name, out prefix, out localName);

            if (string.IsNullOrEmpty(localName))
            {
                localName = name;
            }

            if (noNamespaceSpecified && !string.IsNullOrEmpty(prefix))
            {
                namespaceUri = GetNamespaceUri(prefix);
            }

            string nsUri = GetNamespaceUri();

            foreach (XmlAttribute attr in Attributes.ContentsAs_Enumerable)
            {
                string attrLocalName = attr.PrefixedLocalName != null ? attr.PrefixedLocalName : attr.LocalName;

                if (noNamespaceSpecified)
                {
                    bool namesMatch = string.IsNullOrEmpty(prefix) ?
                        //name == attrLocalName :
                        //name == attr.LocalName;
                    name.Equals(attrLocalName, StringComparison.OrdinalIgnoreCase) :
                    name.Equals(attr.LocalName, StringComparison.OrdinalIgnoreCase);

                    if (attr.GetNamespaceUri() == nsUri
                        && namesMatch)
                    {
                        return attr;
                    }
                }
                else
                {
                    if (attr.GetNamespaceUri() == namespaceUri
                        && attrLocalName.Equals(localName, StringComparison.OrdinalIgnoreCase)
                        //&& attrLocalName == localName
                        )
                    {
                        return attr;
                    }
                }
            }

            //TODO
            const bool strict = true;
            if (!strict && noNamespaceSpecified)
            {
                foreach (XmlAttribute attr in Attributes.ContentsAs_Enumerable)
                {
                    string attrLocalName = attr.PrefixedLocalName != null ? attr.PrefixedLocalName : attr.LocalName;

                    bool namesMatch = string.IsNullOrEmpty(prefix) ?
                        //name == attrLocalName :
                        //name == attr.LocalName;
                    name.Equals(attrLocalName, StringComparison.OrdinalIgnoreCase) :
                    name.Equals(attr.LocalName, StringComparison.OrdinalIgnoreCase);

                    if (namesMatch)
                    {
                        return attr;
                    }
                }
            }

            return null;

            //string key = String.Format("{1}:{0}", localName, namespaceUri);

            //XmlAttribute obj;
            //mAttributes.TryGetValue(key, out obj);
            //return obj;

            //if (mAttributes.ContainsKey(key))
            //{
            //    return mAttributes[key];
            //}
            //return null;
        }

        public XmlAttribute GetAttribute(string localName)
        {
            return GetAttribute(localName, null);
        }

        /// <summary>
        /// Gets a copy of <c>this</c>
        /// </summary>
        /// <returns>The copy</returns>
        public new XmlProperty Copy()
        {
            return CopyProtected() as XmlProperty;
        }

        /// <summary>
        /// Creates a copy of <c>this</c> including copies of any <see cref="XmlAttribute"/>s
        /// </summary>
        /// <returns>The copy</returns>
        protected override Property CopyProtected()
        {
            XmlProperty xmlProp = (XmlProperty)base.CopyProtected();
            string nsUri = GetNamespaceUri();

            xmlProp.SetQName(LocalName, nsUri == null ? "" : nsUri);
            foreach (XmlAttribute attr in Attributes.ContentsAs_Enumerable)
            {
                xmlProp.SetAttribute(attr.Copy());
            }
            return xmlProp;
        }

        /// <summary>
        /// Creates an export of <c>this</c> for a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The given destination presentaton</param>
        /// <returns>The exported xml property</returns>
        public new XmlProperty Export(Presentation destPres)
        {
            return ExportProtected(destPres) as XmlProperty;
        }

        /// <summary>
        /// Creates an export of <c>this</c> for a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The given destination presentaton</param>
        /// <returns>The exported xml property</returns>
        protected override Property ExportProtected(Presentation destPres)
        {
            XmlProperty xmlProp = base.ExportProtected(destPres) as XmlProperty;
            if (xmlProp == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The property factory can not create an XmlProperty matching QName {0}:{1}",
                                                                         GetXukNamespace(),
                                                                         GetXukName()));
            }
            string nsUri = GetNamespaceUri();

            xmlProp.SetQName(LocalName, nsUri == null ? "" : nsUri);
            foreach (XmlAttribute attr in Attributes.ContentsAs_Enumerable)
            {
                xmlProp.SetAttribute(attr.Copy());
            }
            return xmlProp;
        }

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="XmlAttribute"/> of QName and <see cref="XmlAttribute"/>s
        /// </summary>
        protected override void Clear()
        {
            mLocalName = null;
            mNamespaceUri = "";
            foreach (XmlAttribute attr in this.Attributes.ContentsAs_Enumerable)
            {
                RemoveAttribute(attr);
            }
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a XmlProperty xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string ln = XukAble.ReadXukAttribute(source, XukAble.LocalName_NAME);
            if (string.IsNullOrEmpty(ln))
            {
                throw new exception.XukException("LocalName attribute is missing from XmlProperty element");
            }
            string nsUri = XukAble.ReadXukAttribute(source, XukAble.NamespaceUri_NAME);
            SetQName(ln, nsUri == null ? "" : nsUri);
        }

        /// <summary>
        /// Reads a child of a XmlProperty xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (PrettyFormat && source.LocalName == XukStrings.XmlAttributes)
                {
                    XukInXmlAttributes(source, handler);
                }
                else if (!PrettyFormat
                    && XukAble.GetXukName(typeof(XmlAttribute)).Match(source.LocalName))
                {
                    XukInXmlAttribute(source, handler);
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

        /// <summary>
        /// Reads the <see cref="XmlAttribute"/>s from the <c>&lt;mXmlAttributes&gt;</c> child element
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected virtual void XukInXmlAttribute(XmlReader source, IProgressHandler handler)
        {
            if (source.NamespaceURI == XukAble.XUK_NS
                && XukAble.GetXukName(typeof(XmlAttribute)).Match(source.LocalName))
            {
                XmlAttribute attr = new XmlAttribute();
                attr.XukIn(source, handler);
                SetAttribute(attr);
            }
            else if (!source.IsEmptyElement)
            {
                source.ReadSubtree().Close();
            }
        }

        private void XukInXmlAttributes(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        XukInXmlAttribute(source, handler);
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        /// <summary>
        /// Writes the attributes of a XmlProperty element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukAble.LocalName_NAME.z(PrettyFormat), LocalName);

            if (!string.IsNullOrEmpty(mNamespaceUri))
            {
                string backup = mNamespaceUri;
                mNamespaceUri = null;
                string nsuri = GetNamespaceUri();
                mNamespaceUri = backup;

                if (!mNamespaceUri.Equals(nsuri, StringComparison.InvariantCulture))
                {
                    destination.WriteAttributeString(XukAble.NamespaceUri_NAME.z(PrettyFormat), NamespaceUri);
                }
            }
        }

        /// <summary>
        /// Write the child elements of a XmlProperty element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            ObjectListProvider<XmlAttribute> attrs = Attributes;
            if (attrs.Count > 0)
            {
                if (PrettyFormat)
                {
                    destination.WriteStartElement(XukStrings.XmlAttributes, XukAble.XUK_NS);
                }
                foreach (XmlAttribute a in attrs.ContentsAs_Enumerable)
                {
                    a.XukOut(destination, baseUri, handler);
                }
                if (PrettyFormat)
                {
                    destination.WriteEndElement();
                }
            }
        }

        #endregion

        #region IValueEquatable<Property> Members
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            XmlProperty otherz = other as XmlProperty;
            if (otherz == null)
            {
                return false;
            }
            if (LocalName != otherz.LocalName)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            string nsUri = GetNamespaceUri();
            string nsUriOther = otherz.GetNamespaceUri();

            if (nsUri != nsUriOther)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            ObjectListProvider<XmlAttribute> thisAttrs = Attributes;
            ObjectListProvider<XmlAttribute> otherAttrs = otherz.Attributes;
            if (thisAttrs.Count != otherAttrs.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            foreach (XmlAttribute thisAttr in thisAttrs.ContentsAs_Enumerable)
            {
                XmlAttribute otherAttr = otherz.GetAttribute(thisAttr.LocalName, thisAttr.GetNamespaceUri());
                if (otherAttr == null)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                if (otherAttr.Value != thisAttr.Value)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Gets a <see cref="string"/> representation of the <see cref="XmlProperty"/>
        /// </summary>
        /// <returns>The <see cref="string"/> representation</returns>
        public override string ToString()
        {
            string displayName = mLocalName ?? "null";
            string nsUri = GetNamespaceUri();

            if (!string.IsNullOrEmpty(nsUri))
            {
                displayName += String.Format(" xmlns='{0}'", nsUri.Replace("'", "''"));
            }
            string attrs = " ";
            foreach (XmlAttribute attr in Attributes.ContentsAs_Enumerable)
            {
                string attrDisplayName;
                try
                {
                    attrDisplayName = attr.LocalName;
                }
                catch (exception.IsNotInitializedException)
                {
                    continue;
                }
                string nsUriAttr = attr.GetNamespaceUri();
                if (!string.IsNullOrEmpty(nsUriAttr))
                {
                    attrDisplayName = nsUriAttr + ":" + attrDisplayName;
                }
                attrs += String.Format("{0}='{1}'", attrDisplayName, attr.Value.Replace("'", "''"));
            }
            return String.Format("{0}: <{1} {2}/>", base.ToString(), displayName, attrs);
        }
    }
}