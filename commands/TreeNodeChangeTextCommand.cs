using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.command;
using urakawa.core;
using urakawa.media.data;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("nodChTxtCmd", "TreeNodeChangeTextCommand")]
    public class TreeNodeChangeTextCommand : CommandWithTreeNode
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            TreeNodeChangeTextCommand otherz = other as TreeNodeChangeTextCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }


        private TreeNode m_TreeNode;
        public override TreeNode TreeNode
        {
            protected set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }

        private string m_NewText;
        public string NewText
        {
            private set { m_NewText = value; }
            get { return m_NewText; }
        }
        private string m_OldText;
        public string OldText
        {
            private set { m_OldText = value; }
            get { return m_OldText; }
        }

        public void Init(TreeNode treeNode, string newTxt)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("TreeNode");
            }
            TreeNode = treeNode;

            if (string.IsNullOrEmpty(newTxt))
            {
                throw new ArgumentNullException("newTxt");
            }
            NewText = newTxt;

            string txt = GetText(TreeNode);
            if (string.IsNullOrEmpty(txt))
            {
                throw new ArgumentNullException("GetText(TreeNode)");
            }
            OldText = txt;

            //m_UsedMediaData.Add(NOTHING);

            ShortDescription = "Change TreeNode Text";
            LongDescription = "Change the text of a TreeNode";
        }

        public static string GetText(TreeNode node)
        {
            TreeNode.StringChunkRange txtChunkRange = node.GetText();
            TreeNode.StringChunk textLocal = txtChunkRange != null ? txtChunkRange.First : null; //node.GetTextChunk();

            if (textLocal != null)
            {
                //DebugFix.Assert(textMedia.First.IsAbstractTextMedia);
                //DebugFix.Assert(range.Last == null);

                return textLocal.Str;
            }
            //else
            //{
            //    QualifiedName qname = node.GetXmlElementQName();
            //    if (qname != null && qname.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
            //    {
            //        property.xml.XmlAttribute xmlAttr = node.GetXmlProperty().GetAttribute("alt");
            //        if (xmlAttr != null)
            //        {
            //            return xmlAttr.Value;
            //        }
            //    }
            //}
            return null;
        }

        private static void SetText(TreeNode node, string txt)
        {
            TreeNode.StringChunkRange txtChunkRange = node.GetText();
            TreeNode.StringChunk textLocal = txtChunkRange != null ? txtChunkRange.First : null; //node.GetTextChunk();

            DebugFix.Assert(textLocal != null);
            if (textLocal != null)
            {
                if (textLocal.IsAbstractTextMedia)
                {
                    DebugFix.Assert(textLocal.m_TextMedia != null);

#if DEBUG
                    media.AbstractTextMedia textMedia = node.GetTextMedia();
                    DebugFix.Assert(textLocal.m_TextMedia == textMedia);
#endif //DEBUG

                    textLocal.m_TextMedia.Text = txt;
                }
                else
                {
                    DebugFix.Assert(textLocal.m_XmlAttribute != null);

#if DEBUG
                    if (node.HasXmlProperty)
                    {
                        string localName = node.GetXmlElementLocalName();
                        bool isMath = localName.Equals("math", StringComparison.OrdinalIgnoreCase);
                        if (localName.Equals("img", StringComparison.OrdinalIgnoreCase)
                             || localName.Equals("video", StringComparison.OrdinalIgnoreCase)
                            || isMath
                            )
                        {
                            urakawa.property.xml.XmlAttribute xmlAttr = node.GetXmlProperty().GetAttribute(isMath ? "alttext" : "alt");
                            if (xmlAttr != null)
                            {
                                DebugFix.Assert(textLocal.m_XmlAttribute == xmlAttr);
                            }
                        }
                        else if (node.Children.Count == 0)
                        {
                            urakawa.property.xml.XmlAttribute xmlAttr = node.GetXmlProperty().GetAttribute("title");
                            if (xmlAttr != null)
                            {
                                DebugFix.Assert(textLocal.m_XmlAttribute == xmlAttr);
                            }
                        }
                    }
#endif //DEBUG

                    textLocal.m_XmlAttribute.Value = txt;
                }

                DebugFix.Assert(textLocal.Str == txt);
                //DebugFix.Assert(textLocal.IsAbstractTextMedia);
                //DebugFix.Assert(range.Last == null);
            }

            //media.AbstractTextMedia textMedia = node.GetTextMedia();
            //if (textMedia != null)
            //{
            //    textMedia.Text = txt;

            //    TreeNode.StringChunk textLocal = node.GetTextMedia(true);
            //    DebugFix.Assert(textLocal != null);
            //    if (textLocal != null)
            //    {
            //        DebugFix.Assert(textLocal.Str == txt);
            //        DebugFix.Assert(textLocal.IsAbstractTextMedia);
            //        //DebugFix.Assert(range.Last == null);
            //    }

            //    return;
            //}
            //else
            //{
            //    QualifiedName qname = node.GetXmlElementQName();
            //    if (qname != null && qname.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
            //    {
            //        urakawa.property.xml.XmlAttribute  xmlAttr = node.GetXmlProperty().GetAttribute("alt");
            //        if (xmlAttr != null)
            //        {
            //            xmlAttr.Value = txt;

            //            TreeNode.StringChunk textLocal = node.GetTextMedia(true);
            //            DebugFix.Assert(textLocal != null);
            //            if (textLocal != null)
            //            {
            //                DebugFix.Assert(textLocal.Str == txt);
            //                DebugFix.Assert(!textLocal.IsAbstractTextMedia);
            //                //DebugFix.Assert(range.Last == null);
            //            }

            //            return;
            //        }
            //    }
            //}
            //Debug.Fail("WTF ??");
        }

        public override void Execute()
        {
            SetText(TreeNode, NewText);
        }

        public override void UnExecute()
        {
            SetText(TreeNode, OldText);
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }
        private List<MediaData> m_UsedMediaData = new List<MediaData>();
        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                return m_UsedMediaData;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
