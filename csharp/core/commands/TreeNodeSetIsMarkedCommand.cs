﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.media.data;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.commands
{
    public class TreeNodeChangeTextCommand : Command
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

        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeChangeTextCommand;
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
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
            var textMedia = node.GetTextMedia();
            if (textMedia != null)
            {
                return textMedia.Text;
            }
            else
            {
                QualifiedName qname = node.GetXmlElementQName();
                if (qname != null && qname.LocalName.ToLower() == "img")
                {
                    var xmlAttr = node.GetXmlProperty().GetAttribute("alt");
                    if (xmlAttr != null)
                    {
                        return xmlAttr.Value;
                    }
                }
            }
            return null;
        }

        private static void SetText(TreeNode node, string txt)
        {
            var textMedia = node.GetTextMedia();
            if (textMedia != null)
            {
                textMedia.Text = txt;
                return;
            }
            else
            {
                QualifiedName qname = node.GetXmlElementQName();
                if (qname != null && qname.LocalName.ToLower() == "img")
                {
                    var xmlAttr = node.GetXmlProperty().GetAttribute("alt");
                    if (xmlAttr != null)
                    {
                        xmlAttr.Value = txt;
                        return;
                    }
                }
            }
            Debug.Fail("WTF ??");
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

    public class TreeNodeSetIsMarkedCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            TreeNodeSetIsMarkedCommand otherz = other as TreeNodeSetIsMarkedCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeSetIsMarkedCommand;
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }

        private bool m_NewIsMarked;
        public bool NewIsMarked
        {
            private set { m_NewIsMarked = value; }
            get { return m_NewIsMarked; }
        }

        public void Init(TreeNode treeNode, bool marked)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("TreeNode");
            }
            TreeNode = treeNode;
            NewIsMarked = marked;

            //m_UsedMediaData.Add(NOTHING);

            ShortDescription = "Set TreeNode IsMarked";
            LongDescription = "Set the IsMarked property of a TreeNode";
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }

        public override void Execute()
        {
            TreeNode.IsMarked = NewIsMarked;
        }

        public override void UnExecute()
        {
            TreeNode.IsMarked = !NewIsMarked;
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
