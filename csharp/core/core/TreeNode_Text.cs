using System;
using urakawa.media;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public static bool TextIsPunctuation(char text)
        {
            return text == ' ' || text == '.' || text == ',' || text == '?' || text == '!' || text == '"' || text == '\'' ||
                   text == '(' || text == ')' || text == '{' || text == '}' || text == '[' || text == ']';
        }

        public static bool TextOnlyContainsPunctuation(string text)
        {
            CharEnumerator enumtor = text.GetEnumerator();
            while (enumtor.MoveNext())
            {
                if (!TextIsPunctuation(enumtor.Current))
                    return false;
            }
            return true; // includes empty "text" (when space is trimmed on caller's side)
        }

        public static TreeNode EnsureTreeNodeHasNoSignificantTextOnlySiblings(TreeNode rootBoundary, TreeNode proposed)
        {
            if (rootBoundary == null
                || proposed != null
                && !(rootBoundary == proposed || proposed.IsDescendantOf(rootBoundary)))
            {
                return null;
            }

            if (proposed == null)
            {
                proposed = rootBoundary.GetFirstDescendantWithText(true);
                if (proposed == null)
                {
                    return null;
                }

                while (proposed != null && (proposed.GetXmlElementQName() == null
                    || TextOnlyContainsPunctuation(proposed.GetText(true).Trim())
                    ))
                {
                    proposed = proposed.GetNextSiblingWithText(true);
                }

                if (proposed == null)
                {
                    return null;
                }
            }

            //if (rootBoundary == proposed)
            //{
            //    return rootBoundary;
            //}

            if (proposed.Parent == null)
            {
                return proposed;
            }


            bool atLeastOneSiblingIsSignificantTextOnly = false;

            foreach (var child in proposed.Parent.Children.ContentsAs_YieldEnumerable)
            {
                if (child == proposed)
                {
                    //
                }
                if (child.GetXmlElementQName() != null)
                {
                    continue;
                }
                string text = child.GetTextFlattened(true);
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Trim();
                    if (TextOnlyContainsPunctuation(text))
                    {
                        continue; // we ignore insignificant punctuation
                    }

                    atLeastOneSiblingIsSignificantTextOnly = true;
                }
            }
            if (!atLeastOneSiblingIsSignificantTextOnly)
            {
                return proposed;
            }

            return EnsureTreeNodeHasNoSignificantTextOnlySiblings(rootBoundary, proposed.Parent);
        }

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

        public string GetTextFlattened(bool acceptAltText)
        {
            return GetTextMediaFlattened(true, acceptAltText);
        }

        public string GetText(bool acceptAltText)
        {
            return GetTextMediaFlattened(false, acceptAltText);
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
                return textMedia.Text + " ";
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
