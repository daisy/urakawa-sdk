using System;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.property.xml
{
    [XukNameUglyPrettyAttribute("xAt", "XmlAttribute")]
    public class XmlAttribute : XukAble, IValueEquatable<XmlAttribute>
    {
        public override bool PrettyFormat
        {
            set { throw new NotImplementedException("PrettyFormat"); }
            get
            {
                return XukAble.m_PrettyFormat_STATIC;
            }
        }

        public bool ValueEquals(XmlAttribute otherz)
        {
            if (otherz == null)
            {
                return false;
            }

            if (otherz.LocalName != LocalName
                || otherz.GetNamespaceUri() != GetNamespaceUri()
                || otherz.Value != Value)
            {
                return false;
            }

            return true;
        }

        internal class ValueChangedEventArgs : urakawa.events.DataModelChangedEventArgs
        {
            public ValueChangedEventArgs(XmlAttribute src, string newVal, string prevVal)
                : base(src)
            {
                SourceXmlAttribute = src;
                NewValue = newVal;
                PreviousValue = prevVal;
            }

            public readonly XmlAttribute SourceXmlAttribute;
            public readonly string NewValue;
            public readonly string PreviousValue;
        }

        internal event EventHandler<ValueChangedEventArgs> ValueChanged;

        private void NotifyValueChanged(XmlAttribute src, string newVal, string prevVal)
        {
            EventHandler<ValueChangedEventArgs> d = ValueChanged;
            if (d != null) d(this, new ValueChangedEventArgs(src, newVal, prevVal));
        }

        private XmlProperty mParent;
        private string mLocalName;
        private string mNamespaceUri;
        private string mValue;

        /// <summary>
        /// Constructor setting the parent <see cref="XmlProperty"/>
        /// </summary>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the parent is <c>null</c>
        /// </exception>
        public XmlAttribute()
        {
            mParent = null;
            mLocalName = null;
            mNamespaceUri = "";
            mValue = "";
        }

        #region XmlAttribute Members

        /// <summary>
        /// Creates a copy of the <see cref="XmlAttribute"/>
        /// </summary>
        /// <returns>The copy</returns>
        public virtual XmlAttribute Copy()
        {
            XmlAttribute cp = new XmlAttribute();
            string nsUri = GetNamespaceUri();
            cp.SetQName(LocalName, nsUri == null ? "" : nsUri);
            cp.Value = Value;
            return cp;
        }


        /// <summary>
        /// Gets the value of gthe <see cref="XmlAttribute"/>
        /// </summary>
        /// <returns>The value</returns>
        public string Value
        {
            get { return mValue; }
            set
            {
                string prevVal = mValue;
                mValue = value;
                if (mValue != prevVal)
                {
                    NotifyValueChanged(this, mValue, prevVal);
                }
            }
        }

        public string GetNamespaceUri()
        {
            if (!string.IsNullOrEmpty(NamespaceUri))
            {
                return NamespaceUri;
            }

            if (Parent != null)
            {
                if (!string.IsNullOrEmpty(Prefix))
                {
                    return Parent.GetNamespaceUri(Prefix);
                }

                return Parent.GetNamespaceUri();
            }

            return null;
        }

        /// <summary>
        /// Gets the namespace of the <see cref="XmlAttribute"/>
        /// </summary>
        /// <returns>The namespace</returns>
        private string NamespaceUri
        {
            get { return mNamespaceUri; }
            //private set { SetQName(LocalName, value); }
        }

        /// <summary>
        /// Gets or sets the local localName of the <see cref="XmlAttribute"/>
        /// </summary>
        /// <returns>The local localName</returns>
        public string LocalName
        {
            get
            {
                if (mLocalName == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The XmlAttribute has not been initialized with a local name");
                }
                return mLocalName;
            }

            //private set { SetQName(value, NamespaceUri); }
        }

        private string m_Prefix;
        public string Prefix { get { return m_Prefix; } }

        private string m_PrefixedLocalName;
        public string PrefixedLocalName { get { return m_PrefixedLocalName; } }


        /// <summary>
        /// Sets the QName of the <see cref="XmlAttribute"/> 
        /// </summary>
        /// <param name="newNamespaceUri">The namespace part of the new QName</param>
        /// <param name="newLocalName">The localName part of the new QName</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Throw when <paramref name="newNamespaceUri"/> or <paramref name="newLocalName"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsEmptyStringException">
        /// Thrown when <paramref name="newLocalName"/> is an <see cref="String.Empty"/>
        /// </exception>
        /// <remarks>
        /// If the <see cref="XmlAttribute"/> has already been set on a <see cref="XmlProperty"/>,
        /// setting the QName will overwrite any <see cref="XmlAttribute"/> of the owning <see cref="XmlProperty"/>
        /// with matching QName
        /// </remarks>
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
            if (newLocalName != mLocalName || newNamespaceUri != mNamespaceUri)
            {
                XmlProperty parent = Parent;
                if (parent != null)
                {
                    parent.RemoveAttribute(this);
                }
                mLocalName = newLocalName;
                mNamespaceUri = newNamespaceUri;

                string prefix;
                string realLocalName;
                XmlProperty.SplitLocalName(mLocalName, out prefix, out realLocalName);
                m_Prefix = prefix;
                m_PrefixedLocalName = realLocalName;

                //#if DEBUG
                //                //Debugger.Break();

                //                if (m_Prefix != null)
                //                {
                //                    DebugFix.Assert(!string.IsNullOrEmpty(mNamespaceUri));
                //                }
                //#endif //DEBUG

                if (parent != null)
                {
                    parent.SetAttribute(this);
                }
            }
        }

        /// <summary>
        /// Gets the parent <see cref="XmlProperty"/> of <c>this</c>
        /// </summary>
        /// <returns></returns>
        public XmlProperty Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="XmlAttribute"/> data
        /// </summary>
        protected override void Clear()
        {
            if (Parent != null)
            {
                Parent.RemoveAttribute(this);
            }
            mLocalName = null;
            mNamespaceUri = "";
            mValue = "";
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a XmlAttribute xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string name = XukAble.ReadXukAttribute(source, XukAble.LocalName_NAME);
            if (string.IsNullOrEmpty(name))
            {
                throw new exception.XukException("LocalName attribute of XmlAttribute element is missing");
            }

            string value = XukAble.ReadXukAttribute(source, XukAble.Value_NAME);
            if (value == null)
            {
                throw new exception.XukException("Value attribute of XmlAttribute element is missing");
            }
            Value = value;

            string ns = XukAble.ReadXukAttribute(source, XukAble.NamespaceUri_NAME);

            SetQName(name, ns == null ? "" : ns);
        }

        /// <summary>
        /// Writes the attributes of a XmlAttribute element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (String.IsNullOrEmpty(mLocalName))
            {
                throw new exception.XukException("The XmlAttribute has no name");
            }
            destination.WriteAttributeString(XukAble.LocalName_NAME.z(PrettyFormat), mLocalName);

            if (mValue == null)
            {
                throw new exception.XukException("The XmlAttribute has no value");
            }
            destination.WriteAttributeString(XukAble.Value_NAME.z(PrettyFormat), mValue);

            if (!string.IsNullOrEmpty(mNamespaceUri)
                && (mParent == null || !mNamespaceUri.Equals(mParent.GetNamespaceUri(), StringComparison.InvariantCulture))
                )
            {
                destination.WriteAttributeString(XukAble.NamespaceUri_NAME.z(PrettyFormat), mNamespaceUri);
            }
        }
        
        #endregion

        /// <summary>
        /// Gets a <see cref="string"/> representation of the attribute
        /// </summary>
        /// <returns>The <see cref="string"/> representation</returns>
        public override string ToString()
        {
            string displayName = mLocalName ?? "null";
            string nsUri = GetNamespaceUri();
            if (!string.IsNullOrEmpty(nsUri))
            {
                displayName = nsUri + ":" + displayName;
            }
            return String.Format("{1}: {2}='{3}'", base.ToString(), displayName, Value.Replace("'", "''"));
        }
    }
}