using System;
using System.Xml;
using urakawa.events;
using urakawa.events.metadata;
using urakawa.progress;
using urakawa.xuk;
using XmlAttribute = urakawa.property.xml.XmlAttribute;

namespace urakawa.metadata
{
    /// <summary>
    /// Represents <see cref="Metadata"/> of a <see cref="Presentation"/>
    /// </summary>
    public class Metadata : WithPresentation, IChangeNotifier
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.Metadata;
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="Metadata"/>s should only be created via. the <see cref="MetadataFactory"/>
        /// </summary>
        public Metadata()
        {
            m_firstXukInXmlAttribute = true;

            //mName = "";
            //mContent = "";
            //mNameNamespace = "";

            //mAttributes = new Dictionary<string, string>();
            //mAttributes.Add(XukStrings.MetaDataContent, "");

            //NameChanged += this_NameChanged;
            //ContentChanged += this_ContentChanged;
            //OptionalAttributeChanged += this_OptionalAttributeChanged;
        }

        #region IChangeNotifier members

        /// <summary>
        /// Event fired after the <see cref="Metadata"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(DataModelChangedEventArgs args)
        {
            EventHandler<DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        #endregion

        private void OnNameContentAttributeValueChanged(object sender, XmlAttribute.ValueChangedEventArgs e)
        {
            NotifyChanged(new MetadataEventArgs(this));
        }

        private void OnOtherAttributesObjectRemoved(object sender, ObjectRemovedEventArgs<XmlAttribute> e)
        {
            e.m_RemovedObject.ValueChanged -= new EventHandler<XmlAttribute.ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
            NotifyChanged(new MetadataEventArgs(this));
        }

        private void OnOtherAttributesObjectAdded(object sender, ObjectAddedEventArgs<XmlAttribute> e)
        {
            e.m_AddedObject.ValueChanged += new EventHandler<XmlAttribute.ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
            NotifyChanged(new MetadataEventArgs(this));
        }



        #region IXUKAble members



        private XmlAttribute m_NameContentAttribute;
        public XmlAttribute NameContentAttribute
        {
            get
            {
                return m_NameContentAttribute;
            }
            set
            {
                if (value != m_NameContentAttribute)
                {
                    if (m_NameContentAttribute != null)
                    {
                        m_NameContentAttribute.ValueChanged -=
                            new EventHandler<XmlAttribute.ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
                    }

                    m_NameContentAttribute = value;

                    m_NameContentAttribute.ValueChanged +=
                        new EventHandler<XmlAttribute.ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
                }
            }
        }


        private ObjectListProvider<XmlAttribute> m_OtherAttributes;
        public ObjectListProvider<XmlAttribute> OtherAttributes
        {
            get
            {
                if (m_OtherAttributes == null)
                {
                    m_OtherAttributes = new ObjectListProvider<XmlAttribute>(this);
                    m_OtherAttributes.ObjectAdded += new EventHandler<ObjectAddedEventArgs<urakawa.property.xml.XmlAttribute>>(OnOtherAttributesObjectAdded);
                    m_OtherAttributes.ObjectRemoved += new EventHandler<ObjectRemovedEventArgs<urakawa.property.xml.XmlAttribute>>(OnOtherAttributesObjectRemoved);
                }
                return m_OtherAttributes;
            }
        }


        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            NameContentAttribute.XukOut(destination, baseUri, handler);

            if (OtherAttributes.Count > 0)
            {
                if (IsPrettyFormat())
                {
                    destination.WriteStartElement(XukStrings.XmlAttributes, XukNamespaceUri);
                }
                foreach (XmlAttribute a in OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    a.XukOut(destination, baseUri, handler);
                }
                if (IsPrettyFormat())
                {
                    destination.WriteEndElement();
                }
            }
        }
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukNamespaceUri)
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

        private bool m_firstXukInXmlAttribute;
        protected virtual void XukInXmlAttribute(XmlReader source, ProgressHandler handler)
        {
            if (source.LocalName == XukStrings.XmlAttribute && source.NamespaceURI == XukNamespaceUri)
            {
                var attr = new XmlAttribute();
                attr.XukIn(source, handler);

                if (m_firstXukInXmlAttribute)
                {
                    NameContentAttribute = attr;
                    m_firstXukInXmlAttribute = false;
                }
                else
                {
                    OtherAttributes.Insert(OtherAttributes.Count, attr);
                }
            }
            else if (!source.IsEmptyElement)
            {
                source.ReadSubtree().Close();
            }
        }

        private void XukInXmlAttributes(XmlReader source, ProgressHandler handler)
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

        #endregion

        #region IValueEquatable<Metadata> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            Metadata otherz = other as Metadata;
            if (otherz == null)
            {
                return false;
            }

            if (!NameContentAttribute.ValueEquals(otherz.NameContentAttribute))
            {
                return false;
            }

            if (OtherAttributes.Count != otherz.OtherAttributes.Count)
            {
                return false;
            }

            foreach (XmlAttribute attr in OtherAttributes.ContentsAs_YieldEnumerable)
            {
                bool oneIsEqual = false;
                foreach (XmlAttribute attrOther in otherz.OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    if (attrOther.ValueEquals(attr))
                    {
                        oneIsEqual = true;
                        break;
                    }
                }
                if (!oneIsEqual)
                {
                    return false;
                }
            }

            //if (Name != otherz.Name) return false;
            //if (Content != otherz.Content) return false;

            //if (NameNamespace != otherz.NameNamespace) return false;

            //List<string> names = OptionalAttributeNames;
            //List<string> otherNames = otherz.OptionalAttributeNames;
            //if (names.Count != otherNames.Count) return false;

            //foreach (string name in names)
            //{
            //    if (!otherNames.Contains(name)) return false;
            //    if (GetOptionalAttributeValue(name) != otherz.GetOptionalAttributeValue(name)) return false;
            //}


            return true;
        }

        #endregion




        /// <summary>
        /// Event fired after the name of the <see cref="Metadata"/> has changed
        /// </summary>
        //public event EventHandler<NameChangedEventArgs> NameChanged;

        /// <summary>
        /// Fires the <see cref="NameChanged"/> event
        /// </summary>
        /// <param name="newName">The new name</param>
        /// <param name="prevName">The name prior to the change</param>
        //protected void NotifyNameChanged(string newName, string prevName)
        //{
        //    EventHandler<NameChangedEventArgs> d = NameChanged;
        //    if (d != null) d(this, new NameChangedEventArgs(this, newName, prevName));
        //}

        /// <summary>
        /// Event fired after the content of the <see cref="Metadata"/> has changed
        /// </summary>
        //public event EventHandler<ContentChangedEventArgs> ContentChanged;

        /// <summary>
        /// Fires the <see cref="ContentChanged"/> event
        /// </summary>
        /// <param name="newContent">The new content</param>
        /// <param name="prevContent">The content prior to the change</param>
        //protected void NotifyContentChanged(string newContent, string prevContent)
        //{
        //    EventHandler<ContentChangedEventArgs> d = ContentChanged;
        //    if (d != null) d(this, new ContentChangedEventArgs(this, newContent, prevContent));
        //}

        /// <summary>
        /// Event fired after the optional attribute of the <see cref="Metadata"/> has changed
        /// </summary>
        //public event EventHandler<OptionalAttributeChangedEventArgs> OptionalAttributeChanged;

        /// <summary>
        /// Fires the <see cref="OptionalAttributeChanged"/> event
        /// </summary>
        /// <param name="name">The name of the optional attribute</param>
        /// <param name="newVal">The new value of the optional attribute</param>
        /// <param name="prevValue">The value of the optional attribute prior to the change</param>
        //protected void NotifyOptionalAttributeChanged(string name, string newVal, string prevValue)
        //{
        //    EventHandler<OptionalAttributeChangedEventArgs> d = OptionalAttributeChanged;
        //    if (d != null) d(this, new OptionalAttributeChangedEventArgs(this, name, newVal, prevValue));
        //}


        //private Dictionary<string, string> mAttributes;


        //void this_OptionalAttributeChanged(object sender, OptionalAttributeChangedEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        //void this_ContentChanged(object sender, ContentChangedEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        //void this_NameChanged(object sender, NameChangedEventArgs e)
        //{
        //    NotifyChanged(e);
        //}


        //private string mName;
        //public string Name
        //{
        //    get { return mName; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            throw new exception.MethodParameterIsNullException(
        //                "The name can not be null");
        //        }
        //        string prevName = mName;
        //        mName = value;
        //        if (prevName != mName) NotifyNameChanged(mName, prevName);
        //    }
        //}

        //private string mNameNamespace;
        //public string NameNamespace
        //{
        //    get { return mNameNamespace; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            throw new exception.MethodParameterIsNullException(
        //                "The nameNamespace can not be null");
        //        }
        //        string prevName = mNameNamespace;
        //        mNameNamespace = value;
        //        if (prevName != mNameNamespace) NotifyNameChanged(mNameNamespace, prevName);
        //    }
        //}

        //private string mContent;
        //public string Content
        //{
        //    get { return mContent; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            throw new exception.MethodParameterIsNullException(
        //                "The Content can not be null");
        //        }
        //        string prevContent = mContent;
        //        mContent = value;
        //        if (prevContent != mContent) NotifyNameChanged(mContent, prevContent);
        //    }
        //}

        /// <summary>
        /// Gets the value of a named attribute
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <returns>The value of the attribute - <see cref="String.Empty"/> if the attribute does not exist</returns>
        //public string GetOptionalAttributeValue(string name)
        //{
        //    if (mAttributes.ContainsKey(name))
        //    {
        //        return mAttributes[name];
        //    }
        //    return "";
        //}

        /// <summary>
        /// Sets the value of a named attribute
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="value">The new value for the attribute</param>
        //public void SetOptionalAttributeValue(string name, string value)
        //{
        //    if (value == null)
        //    {
        //        throw new exception.MethodParameterIsNullException(
        //            "A metadata attribute can not have null value");
        //    }
        //    if (name == XukStrings.MetaDataNameNamespace) NameNamespace = value;
        //    else if (name == XukStrings.MetaDataName) Name = value;
        //    else if (name == XukStrings.MetaDataContent) Content = name;
        //    else
        //    {
        //        string prevValue = GetOptionalAttributeValue(name);
        //        if (mAttributes.ContainsKey(name))
        //        {
        //            mAttributes[name] = value;
        //        }
        //        else
        //        {
        //            mAttributes.Add(name, value);
        //        }
        //        if (prevValue != name) NotifyOptionalAttributeChanged(name, value, prevValue);
        //    }
        //}

        /// <summary>
        /// Gets the names of all attributes with non-empty names
        /// </summary>
        /// <returns>A <see cref="List{String}"/> containing the attribute names</returns>
        //public List<string> OptionalAttributeNames
        //{
        //    get
        //    {
        //        List<string> names = new List<string>();
        //        foreach (string name in mAttributes.Keys)
        //        {
        //            if (!string.IsNullOrEmpty(mAttributes[name]))
        //            {
        //                names.Add(name);
        //            }
        //        }
        //        return names;
        //    }
        //}

        /// <summary>
        /// Reads the attributes of a Metadata xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        //protected override void XukInAttributes(XmlReader source)
        //{
        //    base.XukInAttributes(source);

        //    if (source.MoveToFirstAttribute())
        //    {
        //        bool moreAttrs = true;
        //        while (moreAttrs)
        //        {
        //            SetOptionalAttributeValue(source.Name, source.Value);
        //            moreAttrs = source.MoveToNextAttribute();
        //        }
        //        source.MoveToElement();
        //    }
        //}

        /// <summary>
        /// Writes the attributes of a Metadata element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        //protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        //{
        //    base.XukOutAttributes(destination, baseUri);

        //    if (!String.IsNullOrEmpty(Name))
        //    {
        //        destination.WriteAttributeString(XukStrings.MetaDataName, Name);
        //    }
        //    if (!String.IsNullOrEmpty(Content))
        //    {
        //        destination.WriteAttributeString(XukStrings.MetaDataContent, Content);
        //    }
        //    if (!String.IsNullOrEmpty(NameNamespace))
        //    {
        //        destination.WriteAttributeString(XukStrings.MetaDataNameNamespace, NameNamespace);
        //    }

        //    foreach (string a in OptionalAttributeNames)
        //    {
        //        int index = a.IndexOf(':');
        //        if (index < 0)
        //        {
        //            destination.WriteAttributeString(a, GetOptionalAttributeValue(a));
        //        }
        //        else
        //        {
        //            destination.WriteAttributeString(a.Substring(0, index), a.Substring(index), GetOptionalAttributeValue(a));
        //        }
        //    }
        //}
    }
}