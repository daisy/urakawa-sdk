using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AudioLib;
using urakawa.core.visitor;

namespace urakawa.core
{
    public partial class TreeNode
    {
        public List<Section> Outline = null;

        public class Section
        {
            public List<Section> SubSections = new List<Section>();
            public Section ParentSection = null;

            public TreeNode Heading = null;
            public TreeNode RealSectioningRootOrContent = null;

            private static TreeNode NoRealHeading = new TreeNode();
            private TreeNode RealHeading = NoRealHeading;
            public TreeNode GetRealHeading()
            {
                if (RealHeading != NoRealHeading)
                {
                    return RealHeading;
                }

                if (Heading != null)
                {
                    string headingName = Heading.GetXmlElementLocalName();
                    if (!string.IsNullOrEmpty(headingName) &&
                        headingName.Equals(@"hgroup", StringComparison.OrdinalIgnoreCase))
                    {
                        TreeNode highestRanked = null;
                        int highestRank = int.MaxValue;

                        foreach (TreeNode child in Heading.Children.ContentsAs_Enumerable)
                        {
                            string name = child.GetXmlElementLocalName();

                            int rank = GetHeadingRank(name);
                            if (rank >= 0 && rank < highestRank)
                            {
                                highestRank = rank;
                                highestRanked = child;
                            }
                        }

                        DebugFix.Assert(highestRanked != null);

                        if (highestRanked != null)
                        {
                            RealHeading = highestRanked;
                            return RealHeading;
                        }
                    }

                    RealHeading = Heading;
                    return RealHeading;
                }

                RealHeading = null;
                return RealHeading;
            }

#if DEBUG
            private void strIndent(StringBuilder strBuilder, int level)
            {
                for (int i = 0; i < level; i++)
                {
                    strBuilder.Append(' ');
                    strBuilder.Append(' ');
                    strBuilder.Append(' ');
                    strBuilder.Append(' ');
                }
            }

            public void ToString(StringBuilder strBuilder, int level)
            {
                //strBuilder.AppendLine();
                strIndent(strBuilder, level);
                strBuilder.Append(@"<li>");
                strBuilder.Append(@"<a");

                string uid = (RealSectioningRootOrContent != null ? RealSectioningRootOrContent.GetXmlElementId() : null);

                if (Heading != null)
                {
                    string headingName = Heading.GetXmlElementLocalName();
                    if (!string.IsNullOrEmpty(headingName) && headingName.Equals(@"hgroup", StringComparison.OrdinalIgnoreCase))
                    {
                        TreeNode highestRanked = null;
                        int highestRank = int.MaxValue;

                        foreach (TreeNode child in Heading.Children.ContentsAs_Enumerable)
                        {
                            string name = child.GetXmlElementLocalName();

                            int rank = GetHeadingRank(name);
                            if (rank >= 0 && rank < highestRank)
                            {
                                highestRank = rank;
                                highestRanked = child;
                            }
                        }

                        if (highestRanked != null)
                        {
                            if (string.IsNullOrEmpty(uid))
                            {
                                uid = Heading.GetXmlElementId();

                                if (string.IsNullOrEmpty(uid))
                                {
                                    uid = highestRanked.GetXmlElementId();
                                }
                            }

                            if (!string.IsNullOrEmpty(uid))
                            {
                                strBuilder.Append(" href=\"#" + uid + "\"");
                            }

                            strBuilder.Append(@">");

                            strBuilder.Append(highestRanked.GetTextFlattened());
                        }
                        else
                        {
                            Debugger.Break();

                            if (string.IsNullOrEmpty(uid))
                            {
                                uid = Heading.GetXmlElementId();
                            }

                            if (!string.IsNullOrEmpty(uid))
                            {
                                strBuilder.Append(" href=\"#" + uid + "\"");
                            }

                            strBuilder.Append(@">");

                            strBuilder.Append(Heading.GetTextFlattened());
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(uid))
                        {
                            uid = Heading.GetXmlElementId();
                        }

                        if (!string.IsNullOrEmpty(uid))
                        {
                            strBuilder.Append(" href=\"#" + uid + "\"");
                        }

                        strBuilder.Append(@">");

                        strBuilder.Append(Heading.GetTextFlattened());
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(uid))
                    {
                        strBuilder.Append(" href=\"#" + uid + "\"");
                    }

                    strBuilder.Append(@">");

                    if (RealSectioningRootOrContent != null)
                    {
                        string name = RealSectioningRootOrContent.GetXmlElementPrefixedLocalName();

                        if (name.Equals(@"body"))
                        {
                            strBuilder.Append(@"Untitled document");
                        }
                        else if (name.Equals(@"section"))
                        {
                            strBuilder.Append(@"Untitled section");
                        }
                        else if (name.Equals(@"article"))
                        {
                            strBuilder.Append(@"Article");
                        }
                        else if (name.Equals(@"aside"))
                        {
                            strBuilder.Append(@"Sidebar");
                        }
                        else if (name.Equals(@"nav"))
                        {
                            strBuilder.Append(@"Navigation");
                        }
                        else
                        {
                            strBuilder.Append(@"UNTITLED (");
                            strBuilder.Append(name);
                            strBuilder.Append(@")");
                        }
                    }
                    else
                    {
                        strBuilder.Append(@"UNTITLED");
                    }
                }

                strBuilder.Append(@"</a>");

                if (SubSections.Count > 0)
                {
                    strBuilder.AppendLine();
                    strIndent(strBuilder, level + 1);
                    strBuilder.AppendLine(@"<ol>");

                    foreach (Section section in SubSections)
                    {
                        DebugFix.Assert(section.ParentSection == this);

                        section.ToString(strBuilder, level + 2);
                    }

                    //strBuilder.AppendLine();
                    strIndent(strBuilder, level + 1);
                    strBuilder.AppendLine(@"</ol>");

                    strIndent(strBuilder, level);
                }

                strBuilder.AppendLine("</li>");
            }
#endif //DEBUG
        }

#if DEBUG
        public string ToStringOutline()
        {
            if (Outline == null)
            {
                return string.Empty;
            }

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine();
            strBuilder.AppendLine(@"<ol>");
            foreach (Section section in Outline)
            {
                DebugFix.Assert(section.ParentSection == null);

                section.ToString(strBuilder, 1);
            }
            //strBuilder.AppendLine();
            strBuilder.AppendLine(@"</ol>");

            return strBuilder.ToString();
        }
#endif //DEBUG

        public List<Section> GetOrCreateOutline()
        {
            string localXmlName = GetXmlElementLocalName();

            if (string.IsNullOrEmpty(localXmlName)
                || (
                !IsSectioningContent(localXmlName) && !IsSectioningRoot(localXmlName)
                ))
            {
                Outline = null;
                return null;
            }

            if (Outline != null)
            {
                return Outline;
            }

            TreeNode currentOutlinee = null;
            Section currentSection = null;
            Stack<TreeNode> stack = new Stack<TreeNode>();

            PreVisitDelegate enterNode = delegate(TreeNode node)
                {
                    node.Outline = null;

                    if (stack.Count > 0)
                    {
                        TreeNode top = stack.Peek();
                        if (IsHeading(top.GetXmlElementLocalName()))
                        {
                            return true;
                        }
                    }

                    string name = node.GetXmlElementLocalName();
                    if (IsSectioningContent(name) || IsSectioningRoot(name))
                    {
                        //if (currentOutlinee != null && currentSection != null && currentSection.Heading == null)
                        //{
                        //    currentSection.HeadingImplied = true;
                        //}

                        if (currentOutlinee != null)
                        {
                            stack.Push(currentOutlinee);
                        }

                        currentOutlinee = node;

                        currentSection = new Section();
                        currentSection.RealSectioningRootOrContent = currentOutlinee;

                        currentOutlinee.Outline = new List<Section>();
                        currentOutlinee.Outline.Add(currentSection);

                        return true;
                    }

                    if (currentOutlinee == null)
                    {
                        return true;
                    }

                    if (IsHeading(name))
                    {
                        Section section = null;

                        if (currentSection != null && currentSection.Heading == null)
                        {
                            currentSection.Heading = node;
                        }
                        else if (
                            currentOutlinee.Outline == null
                            || (
                            currentOutlinee.Outline.Count > 0
                            && (section = currentOutlinee.Outline[currentOutlinee.Outline.Count - 1]).Heading != null
                            && node.GetHeadingRank() <= section.Heading.GetHeadingRank()
                            )
                            )
                        {
                            Section newSection = new Section();
                            newSection.Heading = node;

                            if (currentOutlinee.Outline == null)
                            {
                                currentOutlinee.Outline = new List<Section>();
                            }
                            currentOutlinee.Outline.Add(newSection);

                            currentSection = newSection;
                        }
                        else
                        {
                            Section candidate = currentSection;

                            while (candidate != null)
                            {
                                if (candidate.Heading != null
                                   && node.GetHeadingRank() > candidate.Heading.GetHeadingRank())
                                {
                                    Section newSection = new Section();
                                    newSection.Heading = node;
                                    candidate.SubSections.Add(newSection);
                                    newSection.ParentSection = candidate;

                                    currentSection = newSection;

                                    break;
                                }

                                candidate = candidate.ParentSection;
                            }
                        }

                        stack.Push(node);
                    }

                    return true;
                };

            PostVisitDelegate exitNode = delegate(TreeNode node)
            {
                string name = node.GetXmlElementLocalName();

                if (stack.Count > 0)
                {
                    TreeNode top = stack.Peek();
                    if (top != null && top == node)
                    {
                        DebugFix.Assert(IsHeading(name));

                        stack.Pop();
                        return;
                    }
                }

                bool isSectioningContent = IsSectioningContent(name);
                bool isSectioningRoot = IsSectioningRoot(name);

                if (isSectioningContent || isSectioningRoot)
                {
                    if (stack.Count > 0)
                    {
                        currentOutlinee = stack.Pop();

                        if (currentOutlinee.Outline != null && currentOutlinee.Outline.Count > 0)
                        {
                            currentSection = currentOutlinee.Outline[currentOutlinee.Outline.Count - 1];
                        }

                        if (isSectioningContent)
                        {
                            if (currentSection != null && node.Outline != null)
                            {
                                for (int i = 0; i < node.Outline.Count; i++)
                                {
                                    Section section = node.Outline[i];

                                    currentSection.SubSections.Add(section);
                                    section.ParentSection = currentSection;
                                }
                            }
                        }
                        else if (isSectioningRoot)
                        {
                            while (currentSection != null && currentSection.SubSections != null && currentSection.SubSections.Count > 0)
                            {
                                currentSection = currentSection.SubSections[currentSection.SubSections.Count - 1];
                            }
                        }
                    }
                    else
                    {
                        if (currentOutlinee.Outline != null && currentOutlinee.Outline.Count > 0)
                        {
                            currentSection = currentOutlinee.Outline[0];
                        }
                    }
                }
            };

            AcceptDepthFirst(enterNode, exitNode);

            if (currentOutlinee != null)
            {
                DebugFix.Assert(Object.ReferenceEquals(currentOutlinee, this));
            }

            return currentOutlinee != null ? currentOutlinee.Outline : null;
        }

        public static bool IsLevel(string localXmlName)
        {
            if (String.IsNullOrEmpty(localXmlName))
                return false;

            return localXmlName.Equals(@"level1", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"level2", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"level3", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"level4", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"level5", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"level6", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"level", StringComparison.OrdinalIgnoreCase)
                ;
        }

        public static bool IsSectioningContent(string localXmlName)
        {
            if (string.IsNullOrEmpty(localXmlName))
                return false;

            return localXmlName.Equals(@"section", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"article", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"aside", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"nav", StringComparison.OrdinalIgnoreCase)
                   ;
        }

        public static bool IsSectioningRoot(string localXmlName)
        {
            if (string.IsNullOrEmpty(localXmlName))
                return false;

            return localXmlName.Equals(@"blockquote", StringComparison.OrdinalIgnoreCase)
                || localXmlName.Equals(@"figure", StringComparison.OrdinalIgnoreCase)
                || localXmlName.Equals(@"details", StringComparison.OrdinalIgnoreCase)
                || localXmlName.Equals(@"fieldset", StringComparison.OrdinalIgnoreCase)
                || localXmlName.Equals(@"td", StringComparison.OrdinalIgnoreCase)
                || localXmlName.Equals(@"body", StringComparison.OrdinalIgnoreCase)
                   ;
        }

        public static bool IsHeading(string localXmlName)
        {
            if (string.IsNullOrEmpty(localXmlName))
                return false;

            return localXmlName.Equals(@"h1", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"h2", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"h3", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"h4", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"h5", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"h6", StringComparison.OrdinalIgnoreCase)

                   || localXmlName.Equals(@"hgroup", StringComparison.OrdinalIgnoreCase)

                   || localXmlName.Equals(@"hd", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"levelhd", StringComparison.OrdinalIgnoreCase)
                   || localXmlName.Equals(@"doctitle", StringComparison.OrdinalIgnoreCase)
                   ;
        }

        public int GetHeadingRank()
        {
            string localXmlName = GetXmlElementLocalName();

            if (string.IsNullOrEmpty(localXmlName))
            {
                return -1;
            }

            if (localXmlName.Equals(@"hgroup", StringComparison.OrdinalIgnoreCase))
            {
                int highestRank = int.MaxValue;

                foreach (TreeNode child in Children.ContentsAs_Enumerable)
                {
                    string name = child.GetXmlElementLocalName();

                    int rank = GetHeadingRank(name);
                    if (rank >= 0 && rank < highestRank)
                    {
                        highestRank = rank;
                    }
                }

                return highestRank;
            }

            return GetHeadingRank(localXmlName);
        }

        public static int GetHeadingRank(string localXmlName)
        {
            if (string.IsNullOrEmpty(localXmlName)
                || !IsHeading(localXmlName)
                || localXmlName.Length != 2
                || (localXmlName[0] != 'h' && localXmlName[0] != 'H')
                )
            {
                return -1;
            }

            char c = localXmlName[1];
            switch (c)
            {
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;

                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;

                case '0':
                default:
                    return -1;
            }

            //int rank;
            //if (Int32.TryParse("" + localXmlName[1], out rank))
            //{
            //    return rank;
            //}

            //return -1;
        }
    }
}
