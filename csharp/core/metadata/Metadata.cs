using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.events;
using urakawa.events.metadata;
using urakawa.progress;
using urakawa.xuk;

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
            m_firstXukInAttribute = true;
        }

        //easily set and get the id attribute (in "OtherAttributes")
        //in Metadata, this Id is used to mark the publication's primary unique identifier
        //multiple identifiers are allowed but only one can be the primary UID.
        public bool IsMarkedAsPrimaryIdentifier
        {
            get
            {
                foreach (var attr in OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    if (attr.Name == "id")
                    {
                        return true;
                    }
                }
                return false;

                //return OtherAttributes.ContentsAs_YieldEnumerable.Any(attr => attr.Name == "id");
            }
            set
            {
                if (value)
                {
                    bool foundID = false;
                    foreach (var attr in OtherAttributes.ContentsAs_YieldEnumerable)
                    {
                        if (attr.Name == "id")
                        {
                            foundID = true;
                            break;
                        }
                    }
                    if (!foundID)
                    {
                        MetadataAttribute newIdAttr = new MetadataAttribute();
                        newIdAttr.Name = "id";
                        newIdAttr.Value = "I'm an Unique Identifier marker !";
                        OtherAttributes.Insert(0, newIdAttr);
                    }
                }
                else
                {
                    foreach (var attr in OtherAttributes.ContentsAs_ListCopy)
                    {
                        if (attr.Name == "id")
                        {
                            OtherAttributes.Remove(attr);
                        }
                    }
                }
            }
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

        private void OnNameContentAttributeValueChanged(object sender, ValueChangedEventArgs e)
        {
            NotifyChanged(new MetadataEventArgs(this));
        }
        private void OnNameContentAttributeNameChanged(object sender, NameChangedEventArgs e)
        {
            NotifyChanged(new MetadataEventArgs(this));
        }
        private void OnNameContentAttributeNamespaceChanged(object sender, NamespaceChangedEventArgs e)
        {
            NotifyChanged(new MetadataEventArgs(this));
        }

        private void OnOtherAttributesObjectRemoved(object sender, ObjectRemovedEventArgs<MetadataAttribute> e)
        {
            e.m_RemovedObject.ValueChanged -= new EventHandler<ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
            e.m_RemovedObject.NameChanged -= new EventHandler<NameChangedEventArgs>(OnNameContentAttributeNameChanged);
            e.m_RemovedObject.NamespaceChanged -= new EventHandler<NamespaceChangedEventArgs>(OnNameContentAttributeNamespaceChanged);
            NotifyChanged(new MetadataEventArgs(this));
        }

        private void OnOtherAttributesObjectAdded(object sender, ObjectAddedEventArgs<MetadataAttribute> e)
        {
            e.m_AddedObject.ValueChanged += new EventHandler<ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
            e.m_AddedObject.NameChanged += new EventHandler<NameChangedEventArgs>(OnNameContentAttributeNameChanged);
            e.m_AddedObject.NamespaceChanged += new EventHandler<NamespaceChangedEventArgs>(OnNameContentAttributeNamespaceChanged);
            NotifyChanged(new MetadataEventArgs(this));
        }



        #region IXUKAble members



        private MetadataAttribute m_NameContentAttribute;
        public MetadataAttribute NameContentAttribute
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
                            new EventHandler<ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
                        m_NameContentAttribute.NameChanged -= new EventHandler<NameChangedEventArgs>(OnNameContentAttributeNameChanged);
                        m_NameContentAttribute.NamespaceChanged -= new EventHandler<NamespaceChangedEventArgs>(OnNameContentAttributeNamespaceChanged);
                    }

                    m_NameContentAttribute = value;

                    m_NameContentAttribute.ValueChanged +=
                        new EventHandler<ValueChangedEventArgs>(OnNameContentAttributeValueChanged);
                    m_NameContentAttribute.NameChanged += new EventHandler<NameChangedEventArgs>(OnNameContentAttributeNameChanged);
                    m_NameContentAttribute.NamespaceChanged += new EventHandler<NamespaceChangedEventArgs>(OnNameContentAttributeNamespaceChanged);
                }
            }
        }


        private ObjectListProvider<MetadataAttribute> m_OtherAttributes;
        public ObjectListProvider<MetadataAttribute> OtherAttributes
        {
            get
            {
                if (m_OtherAttributes == null)
                {
                    m_OtherAttributes = new ObjectListProvider<MetadataAttribute>(this);

                    m_OtherAttributes.ObjectAdded += new EventHandler<ObjectAddedEventArgs<MetadataAttribute>>(OnOtherAttributesObjectAdded);
                    m_OtherAttributes.ObjectRemoved += new EventHandler<ObjectRemovedEventArgs<MetadataAttribute>>(OnOtherAttributesObjectRemoved);
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
                    destination.WriteStartElement(XukStrings.MetadataOtherAttributes, XukNamespaceUri);
                }
                foreach (MetadataAttribute a in OtherAttributes.ContentsAs_YieldEnumerable)
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
                if (m_firstXukInAttribute && source.LocalName == XukStrings.MetadataAttribute)
                {
                    XukInMetadataAttribute(source, handler);
                }
                else if (IsPrettyFormat() && source.LocalName == XukStrings.MetadataOtherAttributes)
                {
                    XukInMetadataOtherAttributes(source, handler);
                }
                else if (!IsPrettyFormat() && source.LocalName == XukStrings.MetadataAttribute)
                {
                    XukInMetadataAttribute(source, handler);
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

        private bool m_firstXukInAttribute;
        protected virtual void XukInMetadataAttribute(XmlReader source, ProgressHandler handler)
        {
            if (source.LocalName == XukStrings.MetadataAttribute && source.NamespaceURI == XukNamespaceUri)
            {
                MetadataAttribute attr = new MetadataAttribute();
                attr.XukIn(source, handler);

                if (m_firstXukInAttribute)
                {
                    NameContentAttribute = attr;
                    m_firstXukInAttribute = false;
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

        private void XukInMetadataOtherAttributes(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        XukInMetadataAttribute(source, handler);
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

            foreach (MetadataAttribute attr in OtherAttributes.ContentsAs_YieldEnumerable)
            {
                bool oneIsEqual = false;
                foreach (MetadataAttribute attrOther in otherz.OtherAttributes.ContentsAs_YieldEnumerable)
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

            return true;
        }

        #endregion
    }
}