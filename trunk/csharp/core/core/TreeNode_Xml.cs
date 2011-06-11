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
            str += " /// ";
            str += GetTextFlattened(true);
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
