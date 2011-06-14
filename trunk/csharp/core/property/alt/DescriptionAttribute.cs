using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using urakawa.xuk;
using urakawa.progress;

namespace urakawa.property.alt
{
    public class DescriptionAttribute : XukAble, IValueEquatable<DescriptionAttribute>
    {
        public bool ValueEquals(DescriptionAttribute otherz)
        {
            if (otherz == null)
            {
                return false;
            }

            if (otherz.LocalName != LocalName
                || otherz.NamespaceUri != NamespaceUri
                || otherz.Value != Value)
            {
                return false;
            }

            return true;
        }

        private Description mParent;
        private string mLocalName;
        private string mNamespaceUri;
        private string mValue;


        public DescriptionAttribute()
        {
            mParent = null;
            mLocalName = null;
            mNamespaceUri = "";
            mValue = "";
        }

        public virtual DescriptionAttribute Copy()
        {
            DescriptionAttribute cp = new DescriptionAttribute();
            cp.SetQName(LocalName, NamespaceUri);
            cp.Value = Value;
            return cp;
        }


        /// <summary>
        /// Gets the value of gthe <see cref="DescriptionAttribute"/>
        /// </summary>
        /// <returns>The value</returns>
        public string Value
        {
            get { return mValue; }
            set
            {
                string prevVal = mValue;
                mValue = value;
                
            }
        }

        /// <summary>
        /// Gets the namespace of the <see cref="DescriptionAttribute"/>
        /// </summary>
        /// <returns>The namespace</returns>
        public string NamespaceUri
        {
            get { return mNamespaceUri; }
            set { SetQName(LocalName, value); }
        }

        /// <summary>
        /// Gets or sets the local localName of the <see cref="DescriptionAttribute"/>
        /// </summary>
        /// <returns>The local localName</returns>
        public string LocalName
        {
            get
            {
                if (mLocalName == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The DescriptionAttribute has not been initialized with a local name");
                }
                return mLocalName;
            }

            set { SetQName(value, NamespaceUri); }
        }

        /// <summary>
        /// Sets the QName of the <see cref="DescriptionAttribute"/> 
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
        /// If the <see cref="DescriptionAttribute"/> has already been set on a <see cref="Description"/>,
        /// setting the QName will overwrite any <see cref="DescriptionAttribute"/> of the owning <see cref="Description"/>
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
                Description parent = Parent;
                if (parent != null)
                {
                    parent.RemoveAttribute(this);
                }
                mLocalName = newLocalName;
                mNamespaceUri = newNamespaceUri;
                if (parent != null)
                {
                    parent.SetAttribute(this);
                }
            }
        }

        /// <summary>
        /// Gets the parent <see cref="Description"/> of <c>this</c>
        /// </summary>
        /// <returns></returns>
        public Description Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }


        /// <summary>
        /// Clears the <see cref="DescriptionAttribute"/> data
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
        /// Reads the attributes of a DescriptionAttribute xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string name = source.GetAttribute(XukStrings.LocalName);
            if (string.IsNullOrEmpty(name))
            {
                throw new exception.XukException("LocalName attribute of DescriptionAttribute element is missing");
            }

            string value = source.GetAttribute(XukStrings.Value);
            if (value == null)
            {
                throw new exception.XukException("Value attribute of DescriptionAttribute element is missing");
            }
            Value = value;

            string ns = source.GetAttribute(XukStrings.NamespaceUri);
            if (ns == null) ns = "";
            SetQName(name, ns);
        }

        /// <summary>
        /// Writes the attributes of a DescriptionAttribute element
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
                throw new exception.XukException("The DescriptionAttribute has no name");
            }
            destination.WriteAttributeString(XukStrings.LocalName, mLocalName);

            if (mValue == null)
            {
                throw new exception.XukException("The DescriptionAttribute has no value");
            }
            destination.WriteAttributeString(XukStrings.Value, mValue);

            if (mNamespaceUri != "")
            {
                destination.WriteAttributeString(XukStrings.NamespaceUri, mNamespaceUri);
            }
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.DescriptionAttribute;
        }

        public override string ToString()
        {
            string displayName = mLocalName ?? "null";
            if (NamespaceUri != "") displayName = NamespaceUri + ":" + displayName;
            return String.Format("{1}: {2}='{3}'", base.ToString(), displayName, Value.Replace("'", "''"));
        }


    }
}
