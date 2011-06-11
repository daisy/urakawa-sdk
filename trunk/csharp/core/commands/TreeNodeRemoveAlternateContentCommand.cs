using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.media.data;
using urakawa.progress;
using urakawa.xuk;

using urakawa.property.alt;

namespace urakawa.commands
{
    public class TreeNodeRemoveAlternateContentCommand: Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            TreeNodeRemoveAlternateContentCommand otherz = other as TreeNodeRemoveAlternateContentCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeRemoveAlternateContentCommand;
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }

        private AlternateContent m_AlternateContent ;
        public AlternateContent AlternateContent
        {
            private set { m_AlternateContent= value; }
            get { return m_AlternateContent; }
        }
        

        public void Init(TreeNode treeNode, AlternateContent altContent)
        {
            if (altContent == null)
            {
                throw new ArgumentNullException("altContent");
            }
            if (treeNode == null)
            {
                throw new ArgumentNullException("TreeNode");
            }

            TreeNode = treeNode;
            AlternateContent = altContent;
            
            //m_UsedMediaData.Add(NOTHING);

            ShortDescription = "Remove AlternateContent";
            LongDescription = "Remove alternate content to TreeNode";
        }


        public override void UnExecute()
        {
            AlternateContentProperty prop = TreeNode.GetOrCreateAlternateContentProperty();
            prop.AlternateContents.Insert(prop.AlternateContents.Count, m_AlternateContent);
        }

        public override void Execute()
        {
            AlternateContentProperty prop = TreeNode.GetAlternateContentProperty();
            prop.AlternateContents.Remove(m_AlternateContent);
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
