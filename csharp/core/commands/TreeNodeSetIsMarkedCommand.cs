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
    [XukNameUglyPrettyAttribute("nodSetMrkCmd", "TreeNodeSetIsMarkedCommand")]
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
