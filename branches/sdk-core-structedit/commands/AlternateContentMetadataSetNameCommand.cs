using System;
using System.Xml;
using urakawa.command;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

using urakawa.property.alt;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("acMetaSetNameCmd", "AlternateContentMetadataSetNameCommand")]
    public class AlternateContentMetadataSetNameCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentMetadataSetNameCommand otherz = other as AlternateContentMetadataSetNameCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }
        
        private string m_OriginalName;
        private string m_NewName;
        
        public string Name
        {
            private set { m_NewName = value; }
            get { return m_NewName; }
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

        public void Init(AlternateContentProperty altContentProperty, AlternateContent altContent,
            MetadataAttribute metadataAttribute, string name)
        {
            if (metadataAttribute == null)
            {
                throw new ArgumentNullException("metadataAttribute");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (altContentProperty == null && altContent == null)
            {
                throw new ArgumentNullException("altContentProperty && altContent");
            }
            if (altContentProperty != null && altContent != null)
            {
                throw new ArgumentException("altContentProperty && altContent");
            }
            MetadataAttribute = metadataAttribute;
            m_OriginalName = MetadataAttribute.Name;
            Name = name;
            m_AlternateContent = altContent;
            m_AlternateContentProperty = altContentProperty;

            ShortDescription = "Set metadata name";
            LongDescription = "Set the Name of the Metadata object in AlternateContent";
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
            MetadataAttribute.Name = m_NewName;
        }

        public override void UnExecute()
        {
            MetadataAttribute.Name = m_OriginalName;
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
