using System;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

using urakawa.property.alt;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("acMetaRemoveCmd", "AlternateContentMetadataRemoveCommand")]
    public class AlternateContentMetadataRemoveCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentMetadataRemoveCommand otherz = other as AlternateContentMetadataRemoveCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

    
        private int m_Index;
        private Metadata m_Metadata;
        public Metadata Metadata
        {
            private set { m_Metadata = value; }
            get { return m_Metadata; }
        }

        private MetadataAttribute m_MetadataAttribute;
        public MetadataAttribute MetadataAttribute
        {
            private set { m_MetadataAttribute = value; }
            get { return m_MetadataAttribute; }
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }

        private AlternateContentProperty m_AlternateContentProperty;
        public AlternateContentProperty AlternateContentProperty { get { return m_AlternateContentProperty; } }

        private AlternateContent m_AlternateContent;
        public AlternateContent AlternateContent { get { return m_AlternateContent; } }

        public void Init(TreeNode treeNode, AlternateContentProperty altContentProperty, AlternateContent altContent, Metadata metadata, MetadataAttribute metadataAttribute)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (metadataAttribute == null)
            {
                if (altContentProperty == null && altContent == null)
                {
                    throw new ArgumentNullException("altContentProperty && altContent");
                }
                if (altContentProperty != null && altContent != null)
                {
                    throw new ArgumentException("altContentProperty && altContent");
                }
            }

            TreeNode = treeNode;
            Metadata = metadata;
            MetadataAttribute = metadataAttribute;
            m_AlternateContent = altContent;
            m_AlternateContentProperty = altContentProperty;

            ShortDescription = "Remove metadata";
            LongDescription = "Remove the Metadata object from the AlternateContent";
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
            if (MetadataAttribute != null)
            {
                m_Index = Metadata.OtherAttributes.IndexOf(MetadataAttribute);
                if (m_Index < 0) return;
                Metadata.OtherAttributes.Remove(MetadataAttribute);
            }
            else
            {
                if (m_AlternateContent != null)
                {
                    m_Index = m_AlternateContent.Metadatas.IndexOf(Metadata);
                    if (m_Index < 0) return;
                    m_AlternateContent.Metadatas.Remove(Metadata);
                }
                else if (m_AlternateContentProperty != null)
                {
                    m_Index = m_AlternateContentProperty.Metadatas.IndexOf(Metadata);
                    if (m_Index < 0) return;
                    m_AlternateContentProperty.Metadatas.Remove(Metadata);
                }
            }
        }

        public override void UnExecute()
        {
            if (m_Index < 0) return;

            if (MetadataAttribute != null)
            {
                Metadata.OtherAttributes.Insert(m_Index, MetadataAttribute);
            }
            else
            {
                if (m_AlternateContent != null)
                {
                    m_AlternateContent.Metadatas.Insert(m_Index, Metadata);
                }
                else if (m_AlternateContentProperty != null)
                {
                    m_AlternateContentProperty.Metadatas.Insert(m_Index, Metadata);
                }
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
