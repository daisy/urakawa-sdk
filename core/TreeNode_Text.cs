using System;
using System.Collections.Generic;
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

        // determined at XukIn time
        private TextDirection TextDirectionality = TextDirection.Unsure;
        public TextDirection GetTextDirectionality()
        {
            if (TextDirectionality != TextDirection.Unsure)
            {
                return TextDirectionality;
            }
            if (Parent != null)
            {
                return Parent.GetTextDirectionality();
            }
            return TextDirection.Unsure;
        }

        public enum TextDirection
        {
            Unsure = 0,
            LTR = 1,
            RTL = 2
        }

        [DebuggerDisplay("{ToString()}")]
        public sealed class StringChunk
        {
            public readonly TextDirection Direction = TextDirection.Unsure;

            public readonly TreeNode Owner = null;

            public StringChunk Next = null;
            public StringChunk Previous = null;

            private StringChunk()
            {
                ;
            }

            public StringChunk(TreeNode owner, XmlAttribute xmlAttr, TextDirection dir)
            {
                Direction = dir;
                m_XmlAttribute = xmlAttr;
                Owner = owner;
            }

            public StringChunk(TreeNode owner, AbstractTextMedia textMedia, TextDirection dir)
            {
                Direction = dir;
                m_TextMedia = textMedia;
                Owner = owner;
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
                return ConcatStringChunks(this, this, -1, null);
            }
        }

        [DebuggerDisplay("{ToString()}")]
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
            if (range == null)
            {
                return 0;
            }
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
            if (range == null)
            {
                return null;
            }

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

        public void UpdateTextCache_AfterInsert()
        {
            //int index = this.Parent.Children.IndexOf(this);

            TreeNode previousTextLocalNode = this.GetPreviousSiblingWithText();
            TreeNode nextTextLocalNode = this.GetNextSiblingWithText();

            if (previousTextLocalNode != null)
            {
                DebugFix.Assert(previousTextLocalNode.TextLocal != null);
                DebugFix.Assert(previousTextLocalNode.TextLocal.First != null);
                DebugFix.Assert(previousTextLocalNode.TextLocal.First == previousTextLocalNode.TextLocal.Last);
            }

            if (nextTextLocalNode != null)
            {
                DebugFix.Assert(nextTextLocalNode.TextLocal != null);
                DebugFix.Assert(nextTextLocalNode.TextLocal.First != null);
                DebugFix.Assert(nextTextLocalNode.TextLocal.First == nextTextLocalNode.TextLocal.Last);
            }

            if (previousTextLocalNode != null && nextTextLocalNode != null)
            {
                DebugFix.Assert(previousTextLocalNode.TextLocal.First.Next == nextTextLocalNode.TextLocal.First);
                DebugFix.Assert(nextTextLocalNode.TextLocal.First.Previous == previousTextLocalNode.TextLocal.First);
            }

            this.UpdateTextCache_Init();

            if (this.TextLocal != null) //text leaf
            {
                DebugFix.Assert(this.Children.Count == 0);

                DebugFix.Assert(this.TextLocal.First == this.TextLocal.Last);

                StringChunk previous = this.TextLocal.First.Previous;
                StringChunk next = this.TextLocal.First.Next;

                DebugFix.Assert(previous == null);
                DebugFix.Assert(next == null);

                if (previousTextLocalNode != null)
                {
                    this.TextLocal.First.Previous = previousTextLocalNode.TextLocal.First;
                    previousTextLocalNode.TextLocal.First.Next = this.TextLocal.First;
                }

                if (nextTextLocalNode != null)
                {
                    this.TextLocal.First.Next = nextTextLocalNode.TextLocal.First;
                    nextTextLocalNode.TextLocal.First.Previous = this.TextLocal.First;
                }


                TreeNode parent = this.Parent;
                while (parent != null)
                {
                    DebugFix.Assert(parent.TextLocal == null);

                    if (parent.TextFlattened != null)
                    {
                        if (nextTextLocalNode != null && parent.TextFlattened.First == nextTextLocalNode.TextLocal.First)
                        {
                            parent.TextFlattened.First = this.TextLocal.First;
                        }
                        if (previousTextLocalNode != null && parent.TextFlattened.Last == previousTextLocalNode.TextLocal.First)
                        {
                            parent.TextFlattened.Last = this.TextLocal.First;
                        }
                    }
                    parent = parent.Parent;
                }
            }
            else if (this.TextFlattened != null)
            {
                DebugFix.Assert(this.TextFlattened.First != null);
                DebugFix.Assert(this.TextFlattened.Last != null);

                StringChunk previous = this.TextFlattened.First.Previous;
                StringChunk next = this.TextFlattened.Last.Next;

                DebugFix.Assert(previous == null);
                DebugFix.Assert(next == null);

                if (previousTextLocalNode != null)
                {
                    this.TextFlattened.First.Previous = previousTextLocalNode.TextLocal.First;
                    previousTextLocalNode.TextLocal.First.Next = this.TextFlattened.First;
                }

                if (nextTextLocalNode != null)
                {
                    this.TextFlattened.Last.Next = nextTextLocalNode.TextLocal.First;
                    nextTextLocalNode.TextLocal.First.Previous = this.TextFlattened.Last;
                }


                TreeNode parent = this.Parent;
                while (parent != null)
                {
                    DebugFix.Assert(parent.TextLocal == null);

                    if (parent.TextFlattened != null)
                    {
                        if (nextTextLocalNode != null && parent.TextFlattened.First == nextTextLocalNode.TextLocal.First)
                        {
                            parent.TextFlattened.First = this.TextFlattened.First;
                        }
                        if (previousTextLocalNode != null &&
                            parent.TextFlattened.Last == previousTextLocalNode.TextLocal.First)
                        {
                            parent.TextFlattened.Last = this.TextFlattened.Last;
                        }
                    }
                    parent = parent.Parent;
                }
            }
        }

        public void UpdateTextCache_BeforeRemove()
        {
            if (this.TextLocal != null) //text leaf
            {
                DebugFix.Assert(this.Children.Count == 0);

                DebugFix.Assert(this.TextLocal.First == this.TextLocal.Last);

                StringChunk previous = this.TextLocal.First.Previous;
                StringChunk next = this.TextLocal.First.Next;

                if (previous != null)
                {
                    previous.Next = next;
                }
                if (next != null)
                {
                    next.Previous = previous;
                }

                this.TextLocal.First.Previous = null;
                this.TextLocal.First.Next = null;
                // No need, First and Last are the same pointers
                //this.TextLocal.Last.Previous = null;
                //this.TextLocal.Last.Next = null;

                TreeNode parent = this.Parent;
                while (parent != null)
                {
                    DebugFix.Assert(parent.TextLocal == null);

                    if (parent.TextFlattened != null)
                    {
                        if (parent.TextFlattened.First == this.TextLocal.First)
                        {
                            if (next != null && next.Owner.IsDescendantOf(parent))
                            {
                                parent.TextFlattened.First = next;
                            }
                            else
                            {
                                parent.TextFlattened.First = null;
                                parent.TextFlattened.Last = null;

                                parent.TextFlattened = null;
                            }
                        }
                        if (parent.TextFlattened != null)
                        {
                            if (parent.TextFlattened.Last == this.TextLocal.First)
                            {
                                if (previous != null && previous.Owner.IsDescendantOf(parent))
                                {
                                    parent.TextFlattened.Last = previous;
                                }
                                else
                                {
                                    parent.TextFlattened.First = null;
                                    parent.TextFlattened.Last = null;

                                    parent.TextFlattened = null;
                                }
                            }
                        }
                    }
                    parent = parent.Parent;
                }
            }
            else if (this.TextFlattened != null)
            {
                DebugFix.Assert(this.TextFlattened.First != null);
                DebugFix.Assert(this.TextFlattened.Last != null);

                StringChunk previous = this.TextFlattened.First.Previous;
                StringChunk next = this.TextFlattened.Last.Next;

                if (previous != null)
                {
                    previous.Next = next;
                }
                if (next != null)
                {
                    next.Previous = previous;
                }

                this.TextFlattened.First.Previous = null;
                this.TextFlattened.Last.Next = null;

                TreeNode parent = this.Parent;
                while (parent != null)
                {
                    DebugFix.Assert(parent.TextLocal == null);

                    if (parent.TextFlattened != null)
                    {
                        if (parent.TextFlattened.First == this.TextFlattened.First)
                        {
                            if (next != null && next.Owner.IsDescendantOf(parent))
                            {
                                parent.TextFlattened.First = next;
                            }
                            else
                            {
                                parent.TextFlattened.First = null;
                                parent.TextFlattened.Last = null;

                                parent.TextFlattened = null;
                            }
                        }
                        if (parent.TextFlattened != null)
                        {
                            if (parent.TextFlattened.Last == this.TextFlattened.Last)
                            {
                                if (previous != null && previous.Owner.IsDescendantOf(parent))
                                {
                                    parent.TextFlattened.Last = previous;
                                }
                                else
                                {
                                    parent.TextFlattened.First = null;
                                    parent.TextFlattened.Last = null;

                                    parent.TextFlattened = null;
                                }
                            }
                        }
                    }
                    parent = parent.Parent;
                }
            }
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

        private void UpdateTextCache_Init()
        {
            if (this.TextLocal != null || this.TextFlattened != null)
            {
#if DEBUG
                //.Break();
                bool debug = true;
#endif
                return;
            }

            StringChunk localText = GetTextChunk();
            if (localText != null)
            {
                TextLocal = new StringChunkRange(localText, localText);

                //if (Presentation.m_PreviousTextLocal != null)
                //{
                //    Presentation.m_PreviousTextLocal.Next = localText;
                //    localText.Previous = Presentation.m_PreviousTextLocal;
                //}
                //Presentation.m_PreviousTextLocal = localText;

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
            return Char.IsWhiteSpace(c)
                || c == '.'
                || c == ','
                || c == ';'
                || c == ':'
                || c == '|'
                || c == '-'
                || c == '~'
                || c == '#'
                || c == '^'
                || c == '_'
                || c == '/'
                || c == '\\'
                || c == '?'
                || c == '!'
                || c == '"'
                || c == '\''
                || c == '('
                || c == ')'
                || c == '{'
                || c == '}'
                || c == '['
                || c == ']';
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

        //public static TreeNode GetNextTreeNodeWithNoSignificantTextOnlySiblings(bool directionPrevious, TreeNode node, out TreeNode nested)
        //{
        //    TreeNode nextDirect = node;
        //    do
        //    {
        //        if (directionPrevious)
        //        {
        //            nextDirect = nextDirect.GetPreviousSiblingWithText();
        //        }
        //        else
        //        {
        //            nextDirect = nextDirect.GetNextSiblingWithText();
        //        }
        //    } while (nextDirect != null
        //        &&
        //        (
        //        //!nextDirect.HasXmlProperty ||
        //        TextOnlyContainsPunctuation(nextDirect.GetText()))
        //        );

        //    if (nextDirect == null)
        //    {
        //        nested = null;
        //        return null;
        //    }

        //    TreeNode root = node.Root;

        //    //TreeNode firstCommonAncestor = null;

        //    //TreeNode parent = nextDirect.Parent;
        //    //while (parent != null)
        //    //{
        //    //    if (parent.IsAncestorOf(node))
        //    //    {
        //    //        firstCommonAncestor = parent;
        //    //        break;
        //    //    }
        //    //    parent = parent.Parent;
        //    //}

        //    //if (firstCommonAncestor != null)
        //    //{
        //    //    root = firstCommonAncestor;

        //    //    foreach (TreeNode child in firstCommonAncestor.Children.ContentsAs_Enumerable)
        //    //    {
        //    //        if (!child.HasXmlProperty)
        //    //        {
        //    //            continue;
        //    //        }

        //    //        if (child == nextDirect
        //    //            || child.IsAncestorOf(nextDirect))
        //    //        {
        //    //            root = child;
        //    //            break;
        //    //        }
        //    //    }
        //    //}


        //    TreeNode next = EnsureTreeNodeHasNoSignificantTextOnlySiblings(directionPrevious, root, nextDirect);

        //    //TreeNode next = nextDirect;
        //    //TreeNode beforeAdjust = null;
        //    //while (next != null)
        //    //{
        //    //    beforeAdjust = next;
        //    //    next = EnsureTreeNodeHasNoSignificantTextOnlySiblings(directionPrevious, root, next);

        //    //    if (next != null && next.HasXmlProperty)
        //    //    {
        //    //        break;
        //    //    }

        //    //    while (next != null
        //    //        &&
        //    //        (
        //    //        //!next.HasXmlProperty ||
        //    //        TextOnlyContainsPunctuation(next.GetText()))
        //    //        )
        //    //    {
        //    //        if (directionPrevious)
        //    //        {
        //    //            next = next.GetPreviousSiblingWithText();
        //    //        }
        //    //        else
        //    //        {
        //    //            next = next.GetNextSiblingWithText();
        //    //        }
        //    //    }
        //    //}

        //    if (next == null
        //        //|| next != nextDirect && next.IsAncestorOf(node)
        //        )
        //    {
        //        next = node;
        //        do
        //        {
        //            if (directionPrevious)
        //            {
        //                next = next.GetPreviousSiblingWithText();
        //            }
        //            else
        //            {
        //                next = next.GetNextSiblingWithText();
        //            }
        //        } while (next != null
        //           &&
        //           (
        //            !next.HasXmlProperty ||
        //            TextOnlyContainsPunctuation(next.GetText()))
        //           );
        //    }

        //    if (next == null
        //        || !next.HasXmlProperty
        //        || (next.GetText() != null && TextOnlyContainsPunctuation(next.GetText()))
        //        || TextOnlyContainsPunctuation(next.GetTextFlattened_()))
        //    {
        //        nested = null;
        //        return null;
        //    }

        //    nested = null;
        //    return next;

        //    //if (beforeAdjust == null
        //    //        || beforeAdjust == next
        //    //        || !next.IsAncestorOf(beforeAdjust)
        //    //        || next.GetManagedAudioMedia() != null
        //    //    //|| next.GetFirstDescendantWithManagedAudio() == null
        //    //        )
        //    //{
        //    //    nested = null;
        //    //    return next;
        //    //    //m_UrakawaSession.PerformTreeNodeSelection(next, false, null);
        //    //}
        //    //else
        //    //{
        //    //    nested = beforeAdjust;
        //    //    return next;
        //    //    //m_UrakawaSession.PerformTreeNodeSelection(next, false, beforeAdjust);
        //    //}
        //}


        public static TreeNode AdjustSignificantTextOnlyAncestors(TreeNode deepTreeNode)
        {
            List<TreeNode> parents = new List<TreeNode>();

            TreeNode parent = deepTreeNode.Parent;
            while (parent != null)
            {
                parents.Insert(0, parent);
                parent = parent.Parent;
            }
#if DEBUG
            DebugFix.Assert(parents[0] == deepTreeNode.Root);
#endif

            foreach (TreeNode p in parents)
            {
                if (p.AtLeastOneChildSiblingIsSignificantTextOnly())
                {
                    return p;
                }
            }

            StringChunkRange range = deepTreeNode.GetText();
            if (range != null && range.First != null && !TreeNode.TextOnlyContainsPunctuation(range))
            {
#if DEBUG
                DebugFix.Assert(deepTreeNode.HasXmlProperty);
#endif
                return deepTreeNode;
            }

            return null;
        }

        public static TreeNode NavigateInsideSignificantText(TreeNode userRootTreeNode)
        {
            TreeNode firstDescendantWithText = null;
            StringChunkRange range = userRootTreeNode.GetText();
            if (range != null && range.First != null && !TreeNode.TextOnlyContainsPunctuation(range))
            {
                firstDescendantWithText = userRootTreeNode;
            }
            if (firstDescendantWithText == null)
            {
                firstDescendantWithText = userRootTreeNode.GetFirstDescendantWithText();
                if (firstDescendantWithText == null)
                {
                    return null;
                }
                while (firstDescendantWithText != null
                && (range = firstDescendantWithText.GetText()) != null
                && TreeNode.TextOnlyContainsPunctuation(range))
                {
                    firstDescendantWithText = firstDescendantWithText.GetNextSiblingWithText(userRootTreeNode);
                }
                if (firstDescendantWithText == null)
                {
                    return null;
                }
            }

            return AdjustSignificantTextOnlyAncestors(firstDescendantWithText);
        }

        public static TreeNode NavigatePreviousNextSignificantText(bool goPrevious, TreeNode userTreeNode)
        {
            TreeNode nearestSignificantTextSibling;
            return NavigatePreviousNextSignificantText(goPrevious, userTreeNode, out nearestSignificantTextSibling);
        }

        public static TreeNode NavigatePreviousNextSignificantText(bool goPrevious, TreeNode userTreeNode, out TreeNode nearestSignificantTextSibling)
        {
            //Note: GetText() == GetTextFlattened_internal(false)

            StringChunkRange range = null;
            nearestSignificantTextSibling = goPrevious ? userTreeNode.GetPreviousSiblingWithText() : userTreeNode.GetNextSiblingWithText();
            while (nearestSignificantTextSibling != null
                && (range = nearestSignificantTextSibling.GetText()) != null
                && TreeNode.TextOnlyContainsPunctuation(range))
            {
                nearestSignificantTextSibling = goPrevious ? nearestSignificantTextSibling.GetPreviousSiblingWithText() : nearestSignificantTextSibling.GetNextSiblingWithText();
            }
            if (nearestSignificantTextSibling == null)
            {
                return null;
            }

            return AdjustSignificantTextOnlyAncestors(nearestSignificantTextSibling);
        }

        //public static TreeNode EnsureTreeNodeHasNoSignificantTextOnlySiblings(bool directionPrevious, TreeNode rootBoundary, TreeNode proposed)
        //{
        //    if (rootBoundary == null)
        //    {
        //        if (proposed != null)
        //        {
        //            rootBoundary = proposed.Root;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }

        //    //checkProposed:

        //    if (proposed != null)
        //    {
        //        if (proposed == rootBoundary)
        //        {
        //            return rootBoundary;
        //        }

        //        if (!proposed.IsDescendantOf(rootBoundary))
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        if (directionPrevious)
        //        {
        //            proposed = rootBoundary.GetLastDescendantWithText();
        //        }
        //        else
        //        {
        //            proposed = rootBoundary.GetFirstDescendantWithText();
        //        }

        //        if (proposed == null)
        //        {
        //            StringChunkRange textRange = rootBoundary.GetText();
        //            if (textRange == null || TextOnlyContainsPunctuation(textRange))
        //            {
        //                return null;
        //            }
        //            //proposed = rootBoundary;
        //            return rootBoundary;
        //        }

        //        while (proposed != null &&
        //            //(!proposed.HasXmlProperty ||
        //            TextOnlyContainsPunctuation(proposed.GetText())
        //            //)
        //            )
        //        {
        //            if (directionPrevious)
        //            {
        //                proposed = proposed.GetPreviousSiblingWithText();
        //            }
        //            else
        //            {
        //                proposed = proposed.GetNextSiblingWithText();
        //            }
        //        }

        //        if (proposed == null)
        //        {
        //            return null;
        //        }


        //        TreeNode root = rootBoundary;

        //        //TreeNode par = proposed;
        //        //while (par != null)
        //        //{
        //        //    if (par.HasXmlProperty &&
        //        //        (
        //        //        par.Parent == null ||
        //        //        !par.Parent.AtLeastOneChildSiblingIsSignificantTextOnly()
        //        //        )
        //        //    )
        //        //    {
        //        //        root = par;
        //        //        break;
        //        //    }
        //        //    par = par.Parent;
        //        //}

        //        //goto checkProposed;
        //        return EnsureTreeNodeHasNoSignificantTextOnlySiblings(directionPrevious, root, proposed);
        //    }

        //    //if (rootBoundary == proposed)
        //    //{
        //    //    return rootBoundary;
        //    //}

        //    if (proposed.Parent == null)
        //    {
        //        return proposed;
        //    }

        //    if (proposed.Parent.AtLeastOneChildSiblingIsSignificantTextOnly())
        //    {
        //        return EnsureTreeNodeHasNoSignificantTextOnlySiblings(directionPrevious, rootBoundary, proposed.Parent);
        //    }

        //    //if (!skipTextOnlyNodes)
        //    //{
        //    //    return proposed;
        //    //}

        //    TreeNode topMostAncestorWithAtLeastOneSiblingIsSignificantTextOnly = null;
        //    TreeNode topMostAncestorWithoutSiblingIsSignificantTextOnly = null;
        //    TreeNode child = proposed; //.Parent;
        //    TreeNode parent = child.Parent;
        //    while (parent != null
        //        && (
        //        //child == rootBoundary ||
        //        child.IsDescendantOf(rootBoundary))
        //        )
        //    {
        //        if (parent.AtLeastOneChildSiblingIsSignificantTextOnly())
        //        {
        //            topMostAncestorWithAtLeastOneSiblingIsSignificantTextOnly = child;
        //            topMostAncestorWithoutSiblingIsSignificantTextOnly = null;
        //        }
        //        else
        //        {
        //            if (topMostAncestorWithAtLeastOneSiblingIsSignificantTextOnly != null
        //                //&& child.IsAncestorOf(topMostAncestorWithAtLeastOneSiblingIsSignificantTextOnly)
        //                )
        //            {
        //                topMostAncestorWithoutSiblingIsSignificantTextOnly = child;
        //            }
        //        }

        //        child = parent;
        //        parent = child.Parent;
        //    }
        //    if (topMostAncestorWithAtLeastOneSiblingIsSignificantTextOnly == null) // || last.Parent == null)
        //    {
        //        return proposed;
        //    }
        //    if (topMostAncestorWithoutSiblingIsSignificantTextOnly != null)
        //    {
        //        return topMostAncestorWithoutSiblingIsSignificantTextOnly;
        //    }
        //    return rootBoundary;
        //}

        public bool AtLeastOneChildSiblingIsSignificantTextOnly()
        {
            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                //if (child == proposed)
                //{
                //    //
                //}
                if (child.HasXmlProperty)
                {
                    continue;
                }
                StringChunkRange range = child.GetTextFlattened_();
                if (range != null)
                {
                    if (range.First != null && !String.IsNullOrEmpty(range.First.Str))
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

                        return true;
                    }
                }
            }

            return false;
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

                StringChunkRange range = child.GetTextFlattened_internal(false);
                if (range != null)
                {
                    if (range.First != null && !String.IsNullOrEmpty(range.First.Str))
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

        public TreeNode GetFirstAncestorWithTextMedia()
        {
            if (Parent == null)
            {
                return null;
            }

            AbstractTextMedia txt = Parent.GetTextMedia();
            if (txt != null)
            {
                return Parent;
            }

            return Parent.GetFirstAncestorWithTextMedia();
        }

        public TreeNode GetFirstAncestorWithText()
        {
            if (Parent == null)
            {
                return null;
            }

            StringChunkRange range = Parent.GetTextFlattened_internal(false);
            if (range != null)
            {
                if (range.First != null && !String.IsNullOrEmpty(range.First.Str))
                {
                    return Parent;
                }
            }

            return Parent.GetFirstAncestorWithText();
        }

        public TreeNode GetFirstDescendantWithText()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                StringChunkRange range = child.GetTextFlattened_internal(false);
                if (range != null)
                {
                    if (range.First != null && !String.IsNullOrEmpty(range.First.Str))
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
                StringChunkRange range = next.GetTextFlattened_internal(false);
                if (range != null)
                {
                    if (range.First != null && !String.IsNullOrEmpty(range.First.Str))
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
                StringChunkRange range = next.GetTextFlattened_internal(false);
                if (range != null)
                {
                    if (range.First != null && !String.IsNullOrEmpty(range.First.Str))
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
            StringChunkRange range = GetTextFlattened_();
            if (range == null)
            {
                return String.Empty;
            }
            return ConcatStringChunks(range, -1, null);
        }

        public StringChunkRange GetTextFlattened_()
        {
            return GetTextFlattened_internal(true);
        }

        public StringChunkRange GetText()
        {
            //return ConcatStringChunks(GetTextMediaFlattened(false), -1, null);
            return GetTextFlattened_internal(false);
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
                    return new StringChunk(this, textMedia, GetTextDirectionality());
                }

                return null;
            }

            if (ACCEPT_IMG_ALT_TEXT)
            {
                if (HasXmlProperty)
                {
                    string localName = GetXmlElementLocalName();

                    bool isMath = localName.Equals("math", StringComparison.OrdinalIgnoreCase);
                    if (localName.Equals("img", StringComparison.OrdinalIgnoreCase)
                            || localName.Equals("video", StringComparison.OrdinalIgnoreCase)
                        || isMath
                        )
                    {
                        XmlAttribute xmlAttr = GetXmlProperty().GetAttribute(isMath ? "alttext" : "alt");
                        if (xmlAttr != null)
                        {
                            if (!String.IsNullOrEmpty(xmlAttr.Value))
                            {
                                return new StringChunk(this, xmlAttr, GetTextDirectionality());
                            }

                            return null;
                        }
                    }

                    if (Children.Count == 0)
                    {
                        XmlAttribute xmlAttr = GetXmlProperty().GetAttribute("title");
                        if (xmlAttr != null)
                        {
                            if (!String.IsNullOrEmpty(xmlAttr.Value))
                            {
                                return new StringChunk(this, xmlAttr, GetTextDirectionality());
                            }

                            return null;
                        }
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

        private StringChunkRange GetTextFlattened_internal(bool deep)
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
                DebugFix.Assert(!String.IsNullOrEmpty(TextFlattened.First.Str));
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


            StringChunk localText = GetTextChunk();
            if (localText != null)
            {
#if DEBUG
                //Debugger.Break();
                bool debug = true;
#endif

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
                StringChunkRange range = node.GetTextFlattened_internal(true);
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
                            range.First.Previous = previous;
                        }
                        previous = range.First;
                    }

                    last = range.Last;
                }
            }

            if (first != null && last != null)
            {
#if DEBUG
                //Debugger.Break();
                bool debug = true;
#endif

                return new StringChunkRange(first, last);
            }

            return null;
        }
    }
}
