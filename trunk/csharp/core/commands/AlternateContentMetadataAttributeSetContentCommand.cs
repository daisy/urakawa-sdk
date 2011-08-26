using System;
using System.Xml;
using urakawa.command;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

using urakawa.property.alt;

namespace urakawa.commands
{
    public class AlternateContentMetadataAttributeSetContentCommand : Command
    {
        
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentMetadataAttributeSetContentCommand otherz = other as AlternateContentMetadataAttributeSetContentCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentMetadataAttributeSetContentCommand;
        }

        private string m_OriginalContent;
        private string m_NewContent;
        public string Content
        {
            private set { m_NewContent = value; }
            get { return m_NewContent; }
        }
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

        private AlternateContentProperty m_AlternateContentProperty;
        public AlternateContentProperty AlternateContentProperty { get { return m_AlternateContentProperty; } }

        private AlternateContent m_AlternateContent;
        public AlternateContent AlternateContent { get { return m_AlternateContent; } }

        public void Init(AlternateContentProperty altContentProperty, AlternateContent altContent, Metadata metadata, MetadataAttribute metadataAttribute, string content)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (metadataAttribute == null)
            {
                throw new ArgumentNullException("metadataAttribute");
            }
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (altContentProperty == null && altContent == null)
            {
                throw new ArgumentNullException("altContentProperty && altContent");
            }
            if (altContentProperty != null && altContent != null)
            {
                throw new ArgumentException("altContentProperty && altContent");
            }
            Metadata = metadata;
            MetadataAttribute = metadataAttribute;
            m_OriginalContent = metadataAttribute.Value;
            Content = content;
            m_AlternateContent = altContent;
            m_AlternateContentProperty = altContentProperty;

            ShortDescription = "Set metadata attribute content";
            LongDescription = "Set the Content of the Metadata object attribute in metadata";
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
            MetadataAttribute.Value= m_NewContent;
        }

        public override void UnExecute()
        {
            MetadataAttribute.Value = m_OriginalContent;
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
