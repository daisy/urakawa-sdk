using System;
using System.Diagnostics;
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
            str += "///";
            str += GetTextMediaFlattened();
            return str;
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

        ///<summary>
        /// returns the QName of the attached XmlProperty, if any
        ///</summary>
        ///<returns></returns>
        public QualifiedName GetXmlElementQName()
        {
            XmlProperty xmlProp = GetProperty<XmlProperty>();
            if (xmlProp != null)
            {
                return new QualifiedName(xmlProp.LocalName, xmlProp.NamespaceUri);
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
                if (idAttr != null)
                {
                    return (string.IsNullOrEmpty(idAttr.Value) ? null : idAttr.Value);
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
    }
}
