using System;
using System.Text;
using AudioLib;
using urakawa.media;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public static char ToLowerA_Z(char c)
        {
            return (c >= 'A' && c <= 'Z') ? (char)(c + 32) : c;
        }

        public static char ToUpperA_Z(char c)
        {
            return (c >= 'a' && c <= 'z') ? (char)(c - 32) : c;
        }

        public static bool TextIsPunctuation(char text)
        {
            return text == ' ' || text == '.' || text == ',' || text == '?' || text == '!' || text == '"' || text == '\'' ||
                   text == '(' || text == ')' || text == '{' || text == '}' || text == '[' || text == ']';
        }

        public static bool TextOnlyContainsPunctuation(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!TextIsPunctuation(c))
                {
                    return false;
                }
            }

            //CharEnumerator enumtor = text.GetEnumerator();
            //while (enumtor.MoveNext()) //enumtor.Current
            //foreach (char c in text)
            //{

            //}

            return true; // includes empty "text" (when space is trimmed on caller's side)
        }

        public static TreeNode EnsureTreeNodeHasNoSignificantTextOnlySiblings(TreeNode rootBoundary, TreeNode proposed)
        {
            if (rootBoundary == null)
            {
                return null;
            }

        checkProposed:

            if (proposed != null
                && rootBoundary == proposed)
            {
                return rootBoundary;
            }

            if (proposed != null
                && !proposed.IsDescendantOf(rootBoundary))
            {
                return null;
            }

            if (proposed == null)
            {
                proposed = rootBoundary.GetFirstDescendantWithText(true);
                if (proposed == null)
                {
                    string text = rootBoundary.GetText(true);
                    if (string.IsNullOrEmpty(text) || TextOnlyContainsPunctuation(text.Trim()))
                    {
                        return null;
                    }
                    proposed = rootBoundary;
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

                goto checkProposed;
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

            foreach (TreeNode child in proposed.Parent.Children.ContentsAs_Enumerable)
            {
                if (child == proposed)
                {
                    //
                }
                if (child.GetXmlElementQName() != null)
                {
                    continue;
                }
                StringChunk strChunkStart = child.GetTextFlattened_(true);
                if (strChunkStart != null && !string.IsNullOrEmpty(strChunkStart.Str))
                {
                    StringBuilder stringBuilder = new StringBuilder(strChunkStart.GetLength());

                    //#if NET40
                    //                    stringBuilder.Clear();
                    //#else
                    //                    stringBuilder.Length = 0;
                    //#endif //NET40

                    TreeNode.ConcatStringChunks(strChunkStart, -1, stringBuilder);
                    string text = stringBuilder.ToString();
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

#if ENABLE_SEQ_MEDIA

        public SequenceMedia GetTextSequenceMedia()
        {
            Media med = GetMediaInTextChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }
#endif //ENABLE_SEQ_MEDIA

        public TreeNode GetLastDescendantWithText(bool acceptAltText)
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

                StringChunk strChunkStart = child.GetTextMediaFlattened(false, acceptAltText);
                if (strChunkStart != null && !string.IsNullOrEmpty(strChunkStart.Str))
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

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                StringChunk strChunkStart = child.GetTextMediaFlattened(false, acceptAltText);
                if (strChunkStart != null && !string.IsNullOrEmpty(strChunkStart.Str))
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
                StringChunk strChunkStart = next.GetTextMediaFlattened(false, acceptAltText);
                if (strChunkStart != null && !string.IsNullOrEmpty(strChunkStart.Str))
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
                StringChunk strChunkStart = next.GetTextMediaFlattened(false, acceptAltText);
                if (strChunkStart != null && !string.IsNullOrEmpty(strChunkStart.Str))
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

        public sealed class StringChunk
        {
            public StringChunk(string str)
            {
                Str = str;
            }

            public string Str;
            public StringChunk Next;

            public int GetLength()
            {
                int l = 0;

                StringChunk strChunk = this;
                while (strChunk != null)
                {
                    l += (strChunk.Str != null ? strChunk.Str.Length : 0);
                    strChunk = strChunk.Next;
                }

                return l;
            }

            public override string ToString()
            {
                return ConcatStringChunks(this, -1, null);
            }
        }


        public static string ConcatStringChunks(StringChunk strChunkStart, int maxLength, StringBuilder stringBuilder)
        {
            bool givenStringBuilderIsNull = stringBuilder == null;

            if (strChunkStart == null) return null;

            if (strChunkStart.Next == null)
            {
                string str = strChunkStart.Str ?? "";

                if (maxLength > 0 && str.Length > maxLength)
                {
                    str = str.Substring(0, maxLength);
                }

                if (givenStringBuilderIsNull)
                {
                    return str;
                }

                stringBuilder.Append(str);
                return null;
            }

            int totalLength = strChunkStart.GetLength();
            bool checkMax = maxLength > 0 && totalLength > maxLength;

            if (givenStringBuilderIsNull)
            {
                int capacity = checkMax ? maxLength : totalLength;
                stringBuilder = new StringBuilder(capacity);
            }

            int accumulatedLength = stringBuilder.Length;

            int sumLength = 0;
            StringChunk strChunk = strChunkStart;
            do
            {
                string str = strChunk.Str ?? "";

                if (checkMax)
                {
                    if (((sumLength + str.Length) - maxLength) > 0) //overflow
                    {
                        str = str.Substring(0, maxLength - sumLength); //str.Length - overflow
                    }
                }

                sumLength += str.Length;
                stringBuilder.Append(str);

                if (checkMax && sumLength >= maxLength)
                {
                    DebugFix.Assert(sumLength == maxLength);
                    break;
                }

                strChunk = strChunk.Next;
            } while (strChunk != null);

            accumulatedLength = stringBuilder.Length - accumulatedLength;

            DebugFix.Assert(accumulatedLength == (checkMax ? maxLength : totalLength));

            if (givenStringBuilderIsNull)
            {
                return stringBuilder.ToString();
            }

            return null;
        }

        public string GetTextFlattened(bool acceptAltText)
        {
            return ConcatStringChunks(GetTextFlattened_(acceptAltText), -1, null);
        }

        public StringChunk GetTextFlattened_(bool acceptAltText)
        {
            return GetTextMediaFlattened(true, acceptAltText);
        }

        public string GetText(bool acceptAltText)
        {
            return ConcatStringChunks(GetTextMediaFlattened(false, acceptAltText), -1, null);
        }

        private StringChunk GetTextMediaFlattened(bool deep, bool acceptAltText)
        {
            AbstractTextMedia textMedia = GetTextMedia();
            if (textMedia != null)
            {
                if (!String.IsNullOrEmpty(textMedia.Text))
                {
                    return new StringChunk(textMedia.Text);
                }

                return null;
            }

#if ENABLE_SEQ_MEDIA

            SequenceMedia seq = GetTextSequenceMedia();
            if (seq != null)
            {
                String strText = seq.GetMediaText();
                if (!String.IsNullOrEmpty(strText))
                {
                    return strText;
                }
            }
                        
#endif //ENABLE_SEQ_MEDIA


            if (acceptAltText)
            {
                QualifiedName qName = GetXmlElementQName();
                
                if (qName != null && qName.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
                {
                    XmlAttribute xmlAttr = GetXmlProperty().GetAttribute("alt");
                    if (xmlAttr != null)
                    {
                        if (!String.IsNullOrEmpty(xmlAttr.Value))
                        {
                            return new StringChunk(xmlAttr.Value);
                        }

                        return null;
                    }
                }
            }

            if (!deep)
            {
                return null;
            }

            StringChunk strChunkStart = null;
            StringChunk strChunkPrevious = null;
            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                StringChunk strChunk = node.GetTextMediaFlattened(true, acceptAltText);
                if (strChunk != null)
                {
                    if (strChunkStart == null)
                    {
                        strChunkStart = strChunk;
                    }
                    else
                    {
                        strChunkPrevious.Next = strChunk;
                    }
                    strChunkPrevious = strChunk;
                }
            }

            return strChunkStart;
        }
    }
}
