using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.progress;
using urakawa.property;
using urakawa.xuk;

namespace urakawa.property.xml
{
    /// <summary>
    /// Default implementation of <see cref="XmlProperty"/> interface
    /// </summary>
    public class XmlProperty : Property
    {

        public override string GetTypeNameFormatted()
        {
            return XukStrings.XmlProperty;
        }
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

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="Property"/>s should only be created via. the <see cref="PropertyFactory"/>
        /// </summary>
        public XmlProperty()
        {
            QNameChanged += new EventHandler<urakawa.events.property.xml.QNameChangedEventArgs>(this_qNameChanged);
            XmlAttributeSet +=
                new EventHandler<urakawa.events.property.xml.XmlAttributeSetEventArgs>(this_xmlAttributeSet);
        }

        private string mLocalName = null;
        private string mNamespaceUri = "";
        private IDictionary<string, XmlAttribute> mAttributes = new Dictionary<string, XmlAttribute>();

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
            set { SetQName(value, NamespaceUri); }
        }

        /// <summary>
        /// Gets the namespace uri of <c>this</c>
        /// </summary>
        /// <returns>The namespace uri</returns>
        public string NamespaceUri
        {
            get { return mNamespaceUri; }
            set { SetQName(LocalName, value); }
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
        public List<XmlAttribute> Attributes
        {
            get { return new List<XmlAttribute>(mAttributes.Values); }
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
            string key = String.Format("{1}:{0}", newAttribute.LocalName, newAttribute.NamespaceUri);
            string prevValue = null;

            XmlAttribute obj;
            mAttributes.TryGetValue(key, out obj);

            if (obj != null ) //mAttributes.ContainsKey(key))
            {
                XmlAttribute prevAttr = obj; // mAttributes[key];
                prevValue = prevAttr.Value;
                RemoveAttribute(prevAttr);
            }
            mAttributes.Add(key, newAttribute);
            newAttribute.Parent = this;
            newAttribute.ValueChanged += new EventHandler<XmlAttribute.ValueChangedEventArgs>(Attribute_valueChanged);
            NotifyXmlAttributeSet(this, newAttribute.LocalName, newAttribute.NamespaceUri, newAttribute.Value, prevValue);
            return (prevValue != null);
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
            string key = String.Format("{1}:{0}", attrToRemove.LocalName, attrToRemove.NamespaceUri);
            if (!Object.ReferenceEquals(mAttributes[key], attrToRemove))
            {
                throw new exception.XmlAttributeDoesNotBelongException(
                    "The given XmlAttribute does not belong to the XmlProperty");
            }
            attrToRemove.ValueChanged -= new EventHandler<XmlAttribute.ValueChangedEventArgs>(Attribute_valueChanged);
            mAttributes.Remove(key);
            attrToRemove.Parent = null;
            NotifyXmlAttributeSet(this, attrToRemove.LocalName, attrToRemove.NamespaceUri, null, attrToRemove.Value);
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
        /// Gets the <see cref="XmlAttribute"/> with a given QName
        /// </summary>
        /// <param name="localName">The local localName part of the given QName</param>
        /// <param name="namespaceUri">The namespce uri part of the given QName</param>
        /// <returns>The <see cref="XmlAttribute"/> if found, otherwise <c>null</c></returns>
        public XmlAttribute GetAttribute(string localName, string namespaceUri)
        {
            string key = String.Format("{1}:{0}", localName, namespaceUri);
            
            XmlAttribute obj;
            mAttributes.TryGetValue(key, out obj);
            return obj;

            //if (mAttributes.ContainsKey(key))
            //{
            //    return mAttributes[key];
            //}
            //return null;
        }
        public XmlAttribute GetAttribute(string localName)
        {
            return GetAttribute(localName, "");
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
            XmlProperty xmlProp = (XmlProperty) base.CopyProtected();
            
            xmlProp.SetQName(LocalName, NamespaceUri);
            foreach (XmlAttribute attr in Attributes)
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
                                                                         XukNamespaceUri, XukLocalName));
            }
            xmlProp.SetQName(LocalName, NamespaceUri);
            foreach (XmlAttribute attr in Attributes)
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
            foreach (XmlAttribute attr in this.Attributes)
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

            string ln = source.GetAttribute(XukStrings.LocalName);
            if (string.IsNullOrEmpty(ln))
            {
                throw new exception.XukException("LocalName attribute is missing from XmlProperty element");
            }
            string ns = source.GetAttribute(XukStrings.NamespaceUri);
            if (ns == null) ns = "";
            SetQName(ln, ns);
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
                if (IsPrettyFormat() && source.LocalName == XukStrings.XmlAttributes)
                {
                    XukInXmlAttributes(source, handler);
                }
                else if (!IsPrettyFormat() && source.LocalName == XukStrings.XmlAttribute)
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
            if (source.LocalName == XukStrings.XmlAttribute && source.NamespaceURI == XukAble.XUK_NS)
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

            destination.WriteAttributeString(XukStrings.LocalName, LocalName);
            if (!String.IsNullOrEmpty(NamespaceUri)) destination.WriteAttributeString(XukStrings.NamespaceUri, NamespaceUri);
            
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

            List<XmlAttribute> attrs = Attributes;
            if (attrs.Count > 0)
            {
                if (IsPrettyFormat())
                {
                    destination.WriteStartElement(XukStrings.XmlAttributes, XukAble.XUK_NS);
                }
                foreach (XmlAttribute a in attrs)
                {
                    a.XukOut(destination, baseUri, handler);
                }
                if (IsPrettyFormat())
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
            if (NamespaceUri != otherz.NamespaceUri)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            List<XmlAttribute> thisAttrs = Attributes;
            List<XmlAttribute> otherAttrs = otherz.Attributes;
            if (thisAttrs.Count != otherAttrs.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            foreach (XmlAttribute thisAttr in thisAttrs)
            {
                XmlAttribute otherAttr = otherz.GetAttribute(thisAttr.LocalName, thisAttr.NamespaceUri);
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
            if (NamespaceUri != "") displayName += String.Format(" xmlns='{0}'", NamespaceUri.Replace("'", "''"));
            string attrs = " ";
            foreach (XmlAttribute attr in Attributes)
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
                if (attr.NamespaceUri != "") attrDisplayName = attr.NamespaceUri + ":" + attrDisplayName;
                attrs += String.Format("{0}='{1}'", attrDisplayName, attr.Value.Replace("'", "''"));
            }
            return String.Format("{0}: <{1} {2}/>", base.ToString(), displayName, attrs);
        }
    }
}