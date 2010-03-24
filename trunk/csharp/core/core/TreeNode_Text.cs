using System;
using urakawa.media;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public Media GetMediaInTextChannel()
        {
            return GetMediaInChannel<TextChannel>();
        }

        public AbstractTextMedia GetTextMedia()
        {
            Media med = GetMediaInTextChannel();
            if (med != null)
            {
                return med as AbstractTextMedia;
            }
            return null;
        }

        public SequenceMedia GetTextSequenceMedia()
        {
            Media med = GetMediaInTextChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }
        public TreeNode GetLastDescendantWithText(bool acceptAltText)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

                string str = child.GetTextMediaFlattened(false, acceptAltText);
                if (!string.IsNullOrEmpty(str))
                {
                    return child;
                }

                TreeNode childIn = child.GetLastDescendantWithText(acceptAltText);
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetFirstDescendantWithText(bool acceptAltText)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_YieldEnumerable)
            {
                string str = child.GetTextMediaFlattened(false, acceptAltText);
                if (!string.IsNullOrEmpty(str))
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithText(acceptAltText);
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetPreviousSiblingWithText(bool acceptAltText)
        {
            return GetPreviousSiblingWithText(null, acceptAltText);
        }

        private TreeNode GetPreviousSiblingWithText(TreeNode upLimit, bool acceptAltText)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.PreviousSibling) != null)
            {
                string str = next.GetTextMediaFlattened(false, acceptAltText);
                if (!string.IsNullOrEmpty(str))
                {
                    return next;
                }

                TreeNode nextIn = next.GetLastDescendantWithText(acceptAltText);
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            if (upLimit == null || upLimit != Parent)
            {
                return Parent.GetPreviousSiblingWithText(upLimit, acceptAltText);
            }
            return null;
        }

        public TreeNode GetNextSiblingWithText(bool acceptAltText)
        {
            return GetNextSiblingWithText(null, acceptAltText);
        }

        private TreeNode GetNextSiblingWithText(TreeNode upLimit, bool acceptAltText)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                string str = next.GetTextMediaFlattened(false, acceptAltText);
                if (!string.IsNullOrEmpty(str))
                {
                    return next;
                }

                TreeNode nextIn = next.GetFirstDescendantWithText(acceptAltText);
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            if (upLimit == null || upLimit != Parent)
            {
                return Parent.GetNextSiblingWithText(upLimit, acceptAltText);
            }
            return null;
        }

        public string GetTextMediaFlattened(bool acceptAltText)
        {
            return GetTextMediaFlattened(true, acceptAltText);
        }

        private string GetTextMediaFlattened(bool deep, bool acceptAltText)
        {
            AbstractTextMedia textMedia = GetTextMedia();
            if (textMedia != null)
            {
                if (textMedia.Text.Length == 0)
                {
                    return null;
                }
                return textMedia.Text;
            }
            SequenceMedia seq = GetTextSequenceMedia();
            if (seq != null)
            {
                String strText = seq.GetMediaText();
                if (!String.IsNullOrEmpty(strText))
                {
                    return strText;
                }
            }
            if (acceptAltText)
            {
                QualifiedName qName = GetXmlElementQName();
                string imgAlt = null;
                if (qName != null && qName.LocalName.ToLower() == "img")
                {
                    XmlAttribute xmlAttr = GetXmlProperty().GetAttribute("alt");
                    if (xmlAttr != null)
                    {
                        imgAlt = xmlAttr.Value;
                    }
                }
                if (!String.IsNullOrEmpty(imgAlt))
                {
                    return imgAlt;
                }
            }

            if (!deep)
            {
                return null;
            }

            string str = "";
            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                str += node.GetTextMediaFlattened(true, acceptAltText);
            }
            if (str.Length == 0)
            {
                return null;
            }
            return str;
        }
    }
}
