using System;
using System.Xml;
using urakawa.events.metadata;
using urakawa.xuk;

namespace urakawa.metadata
{
    [XukNameUglyPrettyAttribute("metadtattr", "MetadataAttribute")]
    public class MetadataAttribute : XukAble, IValueEquatable<MetadataAttribute>
    {
        public override bool PrettyFormat
        {
            set { throw new NotImplementedException("PrettyFormat"); }
            get
            {
                return XukAble.m_PrettyFormat_STATIC;
            }
        }

        public bool ValueEquals(MetadataAttribute otherz)
        {
            if (otherz == null)
            {
                return false;
            }

            if (otherz.Name != Name
                || otherz.NamespaceUri != NamespaceUri
                || otherz.Value != Value)
            {
                return false;
            }

            return true;
        }

        internal event EventHandler<ValueChangedEventArgs> ValueChanged;

        private void NotifyValueChanged(MetadataAttribute src, string newVal, string prevVal)
        {
            EventHandler<ValueChangedEventArgs> d = ValueChanged;
            if (d != null) d(this, new ValueChangedEventArgs(src, newVal, prevVal));
        }


        internal event EventHandler<NameChangedEventArgs> NameChanged;

        private void NotifyNameChanged(MetadataAttribute src, string newVal, string prevVal)
        {
            EventHandler<NameChangedEventArgs> d = NameChanged;
            if (d != null) d(this, new NameChangedEventArgs(src, newVal, prevVal));
        }


        internal event EventHandler<NamespaceChangedEventArgs> NamespaceChanged;

        private void NotifyNamespaceChanged(MetadataAttribute src, string newVal, string prevVal)
        {
            EventHandler<NamespaceChangedEventArgs> d = NamespaceChanged;
            if (d != null) d(this, new NamespaceChangedEventArgs(src, newVal, prevVal));
        }

        public MetadataAttribute()
        {
            m_Name = null;
            m_NamespaceUri = null;
            m_Value = null;
        }

        #region MetadataAttribute Members

        public virtual MetadataAttribute Copy()
        {
            MetadataAttribute cp = new MetadataAttribute();
            cp.Name = Name;
            cp.NamespaceUri = NamespaceUri;
            cp.Value = Value;
            return cp;
        }


        private string m_Value;
        public string Value
        {
            get { return m_Value; }
            set
            {
                string prevVal = m_Value;
                m_Value = value;
                if (m_Value != prevVal)
                {
                    NotifyValueChanged(this, m_Value, prevVal);
                }
            }
        }

        private string m_NamespaceUri;
        public string NamespaceUri
        {
            get { return m_NamespaceUri; }
            set
            {
                string prevVal = m_NamespaceUri;
                m_NamespaceUri = value;
                if (m_NamespaceUri != prevVal)
                {
                    NotifyNamespaceChanged(this, m_NamespaceUri, prevVal);
                }
            }
        }


        private string m_Name;
        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                string prevVal = m_Name;
                m_Name = value;
                if (m_Name != prevVal)
                {
                    NotifyNameChanged(this, m_Name, prevVal);
                }
            }
        }

        #endregion

        #region IXUKAble members

        protected override void Clear()
        {
            m_Name = null;
            m_NamespaceUri = null;
            m_Value = null;
            base.Clear();
        }

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string name = XukAble.ReadXukAttribute(source, XukAble.Name_NAME);
            if (string.IsNullOrEmpty(name))
            {
                throw new exception.XukException("Name attribute of MetadataAttribute element is missing");
            }
            Name = name;

            string ns = XukAble.ReadXukAttribute(source, XukAble.NamespaceUri_NAME);
            NamespaceUri = ns;

            string value = XukAble.ReadXukAttribute(source, XukAble.Value_NAME);
            //if (string.IsNullOrEmpty(value))
            //{   
            //    throw new exception.XukException("Value attribute of MetadataAttribute element is missing");
            //}
            Value = value;
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (String.IsNullOrEmpty(Name))
            {
                throw new exception.XukException("The MetadataAttribute has no name");
            }
            destination.WriteAttributeString(XukAble.Name_NAME.z(PrettyFormat), Name);

            if (!String.IsNullOrEmpty(Value))
            {
                destination.WriteAttributeString(XukAble.Value_NAME.z(PrettyFormat), Value);
                //throw new exception.XukException("The MetadataAttribute has no value");
            }
            
            if (!String.IsNullOrEmpty(NamespaceUri))
            {
                destination.WriteAttributeString(XukAble.NamespaceUri_NAME.z(PrettyFormat), NamespaceUri);
            }
        }
        
        #endregion
    }
}
