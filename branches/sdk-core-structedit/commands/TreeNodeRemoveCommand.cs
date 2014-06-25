using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.command;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.utilities;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("nodChTxtCmd", "TreeNodeRemoveCommand")]
    public class TreeNodeRemoveCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            TreeNodeRemoveCommand otherz = other as TreeNodeRemoveCommand;
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

        private TreeNode m_TreeNodeParent;
        public TreeNode TreeNodeParent
        {
            private set { m_TreeNodeParent = value; }
            get { return m_TreeNodeParent; }
        }

        private int m_TreeNodePos;
        public int TreeNodePos
        {
            private set { m_TreeNodePos = value; }
            get { return m_TreeNodePos; }
        }

        public void Init(TreeNode treeNode)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("TreeNode");
            }

            TreeNode = treeNode;

            TreeNodeParent = treeNode.Parent;

            if (treeNode.Parent != null)
            {
                TreeNodePos = treeNode.Parent.Children.IndexOf(treeNode);
            }

            CollectManagedMediaTreeNodeVisitor collectorVisitor = new CollectManagedMediaTreeNodeVisitor();
            treeNode.AcceptDepthFirst(collectorVisitor);

            List<IManaged> list = collectorVisitor.CollectedMedia;
            foreach (IManaged mm in list)
            {
                if (mm.MediaData != null && !m_UsedMediaData.Contains(mm.MediaData))
                {
                    m_UsedMediaData.Add(mm.MediaData);
                }
            }

            ShortDescription = "Remove TreeNode";
            LongDescription = "Remove the TreeNode";
        }

        public override void Execute()
        {
            if (TreeNode.Parent == null)
            {
                TreeNode.Presentation.RootNode = null;
            }
            else
            {
                //TreeNode.Parent.RemoveChild(TreeNode);
                TreeNode.Detach();
            }

            //Console.WriteLine("======================");
            //Console.WriteLine(TreeNode.GetTextFlattened());
            //Console.WriteLine("======================");
            //Console.WriteLine("======================");
            //if (TreeNodeParent != null)
            //{
            //    Console.WriteLine(TreeNodeParent.GetTextFlattened());
            //}
        }

        public override void UnExecute()
        {
            if (TreeNodeParent == null)
            {
                TreeNode.Presentation.RootNode = TreeNode;
            }
            else
            {
                TreeNodeParent.Insert(TreeNode, TreeNodePos);
            }

            //Console.WriteLine("=-------------------------=");
            //Console.WriteLine(TreeNode.GetTextFlattened());
            //Console.WriteLine("=-------------------------=");
            //Console.WriteLine("=-------------------------=");
            //if (TreeNodeParent != null)
            //{
            //    Console.WriteLine(TreeNodeParent.GetTextFlattened());
            //}
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
