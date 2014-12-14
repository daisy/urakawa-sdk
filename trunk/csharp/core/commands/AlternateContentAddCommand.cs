using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.progress;
using urakawa.xuk;

using urakawa.property.alt;

namespace urakawa.commands
{
    public abstract class AlternateContentCommand : CommandWithTreeNode
    {
    }

    [XukNameUglyPrettyAttribute("acAddCmd", "AlternateContentAddCommand")]
    public class AlternateContentAddCommand : AlternateContentCommand
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentAddCommand otherz = other as AlternateContentAddCommand;
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

            if (AlternateContent.Audio != null)
            {
                m_UsedMediaData.Add(AlternateContent.Audio.AudioMediaData);
            }
            else if (AlternateContent.Image != null)
            {
                m_UsedMediaData.Add(AlternateContent.Image.ImageMediaData);
            }

            ShortDescription = "Add AlternateContent";
            LongDescription = "Add alternate content to TreeNode";
        }

        private bool m_TreeNodeNoAltProp = false;
        public override void Execute()
        {
            m_TreeNodeNoAltProp = !TreeNode.HasAlternateContentProperty;
            AlternateContentProperty prop = TreeNode.GetOrCreateAlternateContentProperty();
            prop.AlternateContents.Insert(prop.AlternateContents.Count, m_AlternateContent);
        }

        public override void UnExecute()
        {
            AlternateContentProperty prop = TreeNode.GetAlternateContentProperty();
            prop.AlternateContents.Remove(m_AlternateContent);
            if (m_TreeNodeNoAltProp) TreeNode.RemoveProperty(prop);
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
