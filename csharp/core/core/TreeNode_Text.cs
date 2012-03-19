using System;
using System.Diagnostics;
using System.Text;
using AudioLib;
using urakawa.events.media;
using urakawa.media;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public const bool ACCEPT_IMG_ALT_TEXT = true;

        public sealed class StringChunk
        {
            public StringChunk Next;

            private StringChunk()
            {
                ;
            }

            public StringChunk(XmlAttribute xmlAttr)
            {
                m_XmlAttribute = xmlAttr;
            }

            public StringChunk(AbstractTextMedia textMedia)
            {
                m_TextMedia = textMedia;
            }

            internal readonly XmlAttribute m_XmlAttribute;
            internal readonly AbstractTextMedia m_TextMedia;

            public string Str
            {
                get { return m_XmlAttribute != null ? m_XmlAttribute.Value : m_TextMedia.Text; }
            }

            public bool IsAbstractTextMedia
            {
                get { return m_TextMedia != null; }
            }


            public int GetLength(StringChunk last)
            {
                return GetLengthStringChunks(this, last);
            }

            public override string ToString()
            {
                return ToString(null);
            }

            public string ToString(StringChunk last)
            {
                return ConcatStringChunks(this, last, -1, null);
            }
        }

        public sealed class StringChunkRange
        {
            public StringChunkRange(StringChunk first, StringChunk last)
            {
                First = first;
                Last = last;
            }

            public StringChunk First;
            public StringChunk Last;

            public int GetLength()
            {
                return GetLengthStringChunks(this);
            }

            public override string ToString()
            {
                return ConcatStringChunks(this, -1, null);
            }
        }

        public static int GetLengthStringChunks(StringChunkRange range)
        {
            return GetLengthStringChunks(range.First, range.Last);
        }

        public static int GetLengthStringChunks(StringChunk first, StringChunk last)
        {
            int l = 0;

            StringChunk strChunk = first;
            while (strChunk != null)
            {
                if (!ACCEPT_IMG_ALT_TEXT && !strChunk.IsAbstractTextMedia)
                {
                    continue;
                }

                l += (strChunk.Str != null ? strChunk.Str.Length : 0);

                if (strChunk == last)
                {
                    break;
                }

                strChunk = strChunk.Next;
            }

            return l;
        }

        public static string ConcatStringChunks(StringChunkRange range, int maxLength, StringBuilder stringBuilder)
        {
            return ConcatStringChunks(range.First, range.Last, maxLength, stringBuilder);
        }

        public static string ConcatStringChunks(StringChunk first, StringChunk last, int maxLength, StringBuilder stringBuilder)
        {
            bool givenStringBuilderIsNull = stringBuilder == null;

            if (first == null) return null;

            if (first.Next == null)
            {
                if (!ACCEPT_IMG_ALT_TEXT && !first.IsAbstractTextMedia)
                {
                    return null;
                }

                string str = first.Str ?? "";

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

            int totalLength = first.GetLength(last);
            bool checkMax = maxLength > 0 && totalLength > maxLength;

            if (givenStringBuilderIsNull)
            {
                int capacity = checkMax ? maxLength : totalLength;
                stringBuilder = new StringBuilder(capacity);
            }

            int accumulatedLength = stringBuilder.Length;

            int sumLength = 0;
            StringChunk strChunk = first;
            do
            {
                if (!ACCEPT_IMG_ALT_TEXT && !strChunk.IsAbstractTextMedia)
                {
                    continue;
                }

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

                if (strChunk == last)
                {
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

        private StringChunkRange m_TextFlattened;
        private StringChunkRange TextFlattened
        {
            set { m_TextFlattened = value; }
            get { return m_TextFlattened; }
        }
        //private StringChunkRange m_TextFlattenedNoAltText;
        //private StringChunkRange TextFlattenedNoAltText
        //{
        //    set { m_TextFlattenedNoAltText = value; }
        //    get { return m_TextFlattenedNoAltText; }
        //}
        private StringChunkRange m_TextLocal;
        private StringChunkRange TextLocal
        {
            set { m_TextLocal = value; }
            get { return m_TextLocal; }
        }

        public void XukInAfter_TextMediaCache()
        {
            StringChunk localText = GetTextChunk();
            if (localText != null)
            {
                TextLocal = new StringChunkRange(localText, localText);

                if (Presentation.m_PreviousTextLocal != null)
                {
                    Presentation.m_PreviousTextLocal.Next = localText;
                }
                Presentation.m_PreviousTextLocal = localText;

                // NO NEED TO LISTEN TO CHANGES, BECAUSE STRINGCHUNK STORES POINTERS TO AbstractTextMedia and XmlAttribute :)
                //AbstractTextMedia txtMedia = GetTextMedia();
                //DebugFix.Assert(txtMedia != null);
                //if (txtMedia!=null)
                //{
                //    txtMedia.TextChanged+=new EventHandler<events.media.TextChangedEventArgs>(
                //        delegate(object src, TextChangedEventArgs eventArgs)
                //            {
                //                TextFlattened.First.Str = eventArgs.NewText;
                //            }
                //        );
                //}
            }
            else
            {
                StringChunk first = null;
                StringChunk last = null;
                //StringChunk firstNoAltText = null;
                //StringChunk lastNoAltText = null;
                foreach (TreeNode child in Children.ContentsAs_Enumerable)
                {
                    if (child.TextLocal != null)
                    {
                        DebugFix.Assert(child.TextFlattened == null);
                        //DebugFix.Assert(child.TextFlattenedNoAltText == null);

                        if (first == null)
                        {
                            first = child.TextLocal.First;
                        }
                        last = child.TextLocal.First;

                        //if (child.TextLocal.IsAbstractTextMedia)
                        //{
                        //    if (firstNoAltText == null)
                        //    {
                        //        firstNoAltText = child.TextLocal;
                        //    }
                        //    lastNoAltText = child.TextLocal;
                        //}
                    }

                    if (child.TextFlattened != null)
                    {
                        DebugFix.Assert(child.TextLocal == null);

                        if (first == null)
                        {
                            first = child.TextFlattened.First;
                        }
                        last = child.TextFlattened.Last;
                    }

                    //if (child.TextFlattenedNoAltText != null)
                    //{
                    //    DebugFix.Assert(child.TextLocal == null);

                    //    if (firstNoAltText == null)
                    //    {
                    //        firstNoAltText = child.TextFlattenedNoAltText.First;
                    //    }
                    //    lastNoAltText = child.TextFlattenedNoAltText.Last;
                    //}
                }

                if (first == null)
                {
                    TextFlattened = null;
                }
                else
                {
                    DebugFix.Assert(last != null);

                    TextFlattened = new StringChunkRange(first, last);
                }


                //if (firstNoAltText == null)
                //{
                //    TextFlattenedNoAltText = null;
                //}
                //else
                //{
                //    DebugFix.Assert(firstNoAltText.IsAbstractTextMedia);
                //    if (lastNoAltText != null)
                //    {
                //        DebugFix.Assert(lastNoAltText.IsAbstractTextMedia);
                //    }

                //    TextFlattenedNoAltText = new StringChunkRange(firstNoAltText, lastNoAltText == firstNoAltText ? null : lastNoAltText);
                //}
            }
        }

        public static char ToLowerA_Z(char c)
        {
            return (c >= 'A' && c <= 'Z') ? (char)(c + 32) : c;
        }

        public static char ToUpperA_Z(char c)
        {
            return (c >= 'a' && c <= 'z') ? (char)(c - 32) : c;
        }

        public static bool TextIsPunctuation(char c)
        {
            return char.IsWhiteSpace(c) || c == '.' || c == ',' || c == '?' || c == '!' || c == '"' || c == '\'' ||
                   c == '(' || c == ')' || c == '{' || c == '}' || c == '[' || c == ']';
        }

        public static bool TextOnlyContainsPunctuation(StringChunkRange range)
        {
            DebugFix.Assert(range != null);
            if (range == null) return true;

            StringChunk strChunk = range.First;
            while (strChunk != null)
            {
                if (!ACCEPT_IMG_ALT_TEXT && !strChunk.IsAbstractTextMedia)
                {
                    continue;
                }

                string text = strChunk.Str;
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

                if (strChunk == range.Last)
                {
                    break;
                }

                strChunk = strChunk.Next;
            }

            return true; // includes empty "text" (when whitespace is trimmed)
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
                proposed = rootBoundary.GetFirstDescendantWithText();
                if (proposed == null)
                {
                    StringChunkRange textRange = rootBoundary.GetText();
                    if (textRange == null || TextOnlyContainsPunctuation(textRange))
                    {
                        return null;
                    }
                    proposed = rootBoundary;
                }

                while (proposed != null && (proposed.GetXmlElementQName() == null
                    || TextOnlyContainsPunctuation(proposed.GetText())
                    ))
                {
                    proposed = proposed.GetNextSiblingWithText();
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
                StringChunkRange range = child.GetTextFlattened_();
                if (range != null)
                {
                    if (range.First != null && !string.IsNullOrEmpty(range.First.Str))
                    {
                        //StringBuilder stringBuilder = new StringBuilder(range.GetLength());

                        //#if NET40
                        //                    stringBuilder.Clear();
                        //#else
                        //                    stringBuilder.Length = 0;
                        //#endif //NET40

                        //TreeNode.ConcatStringChunks(range.First, range.Last, -1, stringBuilder);
                        //string text = stringBuilder.ToString();
                        //text = text.Trim();
                        if (TextOnlyContainsPunctuation(range))
                        {
                            continue; // we ignore insignificant punctuation
                        }

                        atLeastOneSiblingIsSignificantTextOnly = true;
                    }
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

        public TreeNode GetLastDescendantWithText()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

                StringChunkRange range = child.GetTextMediaFlattened(false);
                if (range != null)
                {
                    if (range.First != null && !string.IsNullOrEmpty(range.First.Str))
                    {
                        return child;
                    }
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

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                StringChunkRange range = child.GetTextMediaFlattened(false);
                if (range != null)
                {
                    if (range.First != null && !string.IsNullOrEmpty(range.First.Str))
                    {
                        return child;
                    }
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

        private TreeNode GetPreviousSiblingWithText(TreeNode upLimit)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.PreviousSibling) != null)
            {
                StringChunkRange range = next.GetTextMediaFlattened(false);
                if (range != null)
                {
                    if (range.First != null && !string.IsNullOrEmpty(range.First.Str))
                    {
                        return next;
                    }
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

        private TreeNode GetNextSiblingWithText(TreeNode upLimit)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                StringChunkRange range = next.GetTextMediaFlattened(false);
                if (range != null)
                {
                    if (range.First != null && !string.IsNullOrEmpty(range.First.Str))
                    {
                        return next;
                    }
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

        public string GetTextFlattened()
        {
            return ConcatStringChunks(GetTextFlattened_(), -1, null);
        }

        public StringChunkRange GetTextFlattened_()
        {
            return GetTextMediaFlattened(true);
        }

        public StringChunkRange GetText()
        {
            //return ConcatStringChunks(GetTextMediaFlattened(false), -1, null);
            return GetTextMediaFlattened(false);
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


        public StringChunk GetTextChunk()
        {
            AbstractTextMedia textMedia = GetTextMedia();
            if (textMedia != null)
            {
                if (!String.IsNullOrEmpty(textMedia.Text))
                {
                    return new StringChunk(textMedia);
                }

                return null;
            }

            if (ACCEPT_IMG_ALT_TEXT)
            {
                QualifiedName qName = GetXmlElementQName();

                if (qName != null && qName.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
                {
                    XmlAttribute xmlAttr = GetXmlProperty().GetAttribute("alt");
                    if (xmlAttr != null)
                    {
                        if (!String.IsNullOrEmpty(xmlAttr.Value))
                        {
                            return new StringChunk(xmlAttr);
                        }

                        return null;
                    }
                }
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


            return null;
        }

        private StringChunkRange GetTextMediaFlattened(bool deep)
        {
            if (TextLocal != null &&
                (ACCEPT_IMG_ALT_TEXT || TextLocal.First.IsAbstractTextMedia))
            {
                return TextLocal;
            }

            if (!deep)
            {
                return null;
            }

            if (TextFlattened != null)
            {
                DebugFix.Assert(TextFlattened.First != null);
                DebugFix.Assert(!string.IsNullOrEmpty(TextFlattened.First.Str));
                return TextFlattened;
            }

            //if (ACCEPT_IMG_ALT_TEXT)
            //{
            //    if (TextFlattened != null)
            //    {
            //        return TextFlattened;
            //    }
            //}
            //else
            //{
            //    if (TextFlattenedNoAltText != null)
            //    {
            //        return TextFlattenedNoAltText;
            //    }
            //}


#if DEBUG
            Debugger.Break();
#endif

            StringChunk localText = GetTextChunk();
            if (localText != null)
            {
                return new StringChunkRange(localText, localText);
            }

            if (!deep)
            {
                return null;
            }

            StringChunk first = null;
            StringChunk last = null;
            StringChunk previous = null;
            for (int index = 0; index < mChildren.Count; index++)
            {
                TreeNode node = mChildren.Get(index);
                StringChunkRange range = node.GetTextMediaFlattened(true);
                if (range != null)
                {
                    if (range.First != null)
                    {
                        if (first == null)
                        {
                            first = range.First;
                        }
                        else
                        {
                            previous.Next = range.First;
                        }
                        previous = range.First;
                    }

                    last = range.Last;
                }
            }

            return new StringChunkRange(first, last);
        }
    }
}
