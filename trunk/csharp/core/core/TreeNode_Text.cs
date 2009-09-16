using System;
using urakawa.media;
using urakawa.property.channel;

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
        public TreeNode GetLastDescendantWithText()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

                string str = child.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return child;
                }

                TreeNode childIn = child.GetLastDescendantWithText();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetFirstDescendantWithText()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_YieldEnumerable)
            {
                string str = child.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithText();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetPreviousSiblingWithText()
        {
            return GetPreviousSiblingWithText(null);
        }

        public TreeNode GetPreviousSiblingWithText(TreeNode upLimit)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.PreviousSibling) != null)
            {
                string str = next.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return next;
                }

                TreeNode nextIn = next.GetLastDescendantWithText();
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            if (upLimit == null || upLimit != Parent)
            {
                return Parent.GetPreviousSiblingWithText(upLimit);
            }
            return null;
        }

        public TreeNode GetNextSiblingWithText()
        {
            return GetNextSiblingWithText(null);
        }

        public TreeNode GetNextSiblingWithText(TreeNode upLimit)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                string str = next.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return next;
                }

                TreeNode nextIn = next.GetFirstDescendantWithText();
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            if (upLimit == null || upLimit != Parent)
            {
                return Parent.GetNextSiblingWithText(upLimit);
            }
            return null;
        }

        public string GetTextMediaFlattened()
        {
            return GetTextMediaFlattened(true);
        }

        private string GetTextMediaFlattened(bool deep)
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

            if (!deep)
            {
                return null;
            }

            string str = "";
            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                str += node.GetTextMediaFlattened();
            }
            if (str.Length == 0)
            {
                return null;
            }
            return str;
        }
    }
}
